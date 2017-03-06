using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Google.Protobuf;
using Grpc.Core;
using log4net;

namespace EventStore.RPC.Server
{
    public class EventStoreImpl : EventStore.EventStoreBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EventStoreImpl));
        private readonly IEventStoreConnection _eventStoreConnection;

        public EventStoreImpl(IEventStoreConnection eventStoreConnection)
        {
            _eventStoreConnection = eventStoreConnection;
        }

        public override Task<AppendToStreamResponse> AppendToStream(AppendToStreamRequest request,
            ServerCallContext context)
        {
            var events = request.Events.Select(e => new ClientAPI.EventData(
                e.EventId.ToGuid(),
                e.EventType,
                e.IsJson,
                e.Data.ToByteArray(),
                e.Metadata.ToByteArray()
            ));
            try
            {
                var result = _eventStoreConnection
                    .AppendToStreamAsync(request.StreamId, request.ExpectedVersion, events)
                    .Result;
                return Task.FromResult(new AppendToStreamResponse
                {
                    NextExpectedVersion = result.NextExpectedVersion,
                    Position = new Position
                    {
                        CommitPosition = result.LogPosition.CommitPosition,
                        PreparePosition = result.LogPosition.PreparePosition
                    }
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AppendToStreamResponse
                {
                    Error = new Error
                    {
                        Type = ex.GetType().ToString(),
                        Text = ex.ToString()
                    }
                });
            }
        }

        public override async Task SubscribeToStreamFrom(
            IAsyncStreamReader<SubscribeToStreamFromRequest> requestStream,
            IServerStreamWriter<SubscribeToStreamFromResponse> responseStream,
            ServerCallContext context)
        {
            Log.Debug("Waiting for subscription request...");
            var success = await requestStream.MoveNext();
            if (!success) return;
            Log.Debug("Subscription request received");
            var request = requestStream.Current;
            Log.Debug("Starting subscription...");
            var sub = _eventStoreConnection.SubscribeToStreamFrom(request.StreamId, 0,
                CatchUpSubscriptionSettings.Default,
                (subscription, @event) =>
                {
                    try
                    {
                        Log.Debug("Event appeared");
                        responseStream.WriteAsync(new SubscribeToStreamFromResponse
                            {
                                Event =
                                    new ResolvedEvent
                                    {
                                        Event = new RecordedEvent
                                        {
                                            Created = @event.OriginalEvent.Created.Ticks,
                                            CreatedEpoch = @event.OriginalEvent.CreatedEpoch,
                                            Data = ByteString.CopyFrom(@event.OriginalEvent.Data),
                                            EventId = ByteString.CopyFrom(@event.OriginalEvent.EventId.ToByteArray()),
                                            EventNumber = @event.OriginalEvent.EventNumber,
                                            EventStreamId = @event.OriginalEvent.EventStreamId,
                                            EventType = @event.OriginalEvent.EventType,
                                            IsJson = @event.OriginalEvent.IsJson,
                                            Metadata = ByteString.CopyFrom(@event.OriginalEvent.Metadata)
                                        },
                                        Position = new Position
                                        {
                                            CommitPosition = @event.OriginalPosition.HasValue
                                                ? @event.OriginalPosition.Value.CommitPosition
                                                : 0,
                                            PreparePosition = @event.OriginalPosition.HasValue
                                                ? @event.OriginalPosition.Value.PreparePosition
                                                : 0
                                        }
                                    }
                            })
                            .Wait();
                        Log.Debug($"Event sent to subscriber {@event.OriginalEvent.EventNumber}");
                    }
                    catch (InvalidOperationException e)
                    {
                        Log.Warn("Failed to send event to subscriber", e);
                        throw;
                    }
                },
                null,
                (subscription, dropReason, ex) =>
                {
                    try
                    {
                        Log.Debug("Subscription dropped");
                        responseStream.WriteAsync(new SubscribeToStreamFromResponse
                            {
                                DropReason = dropReason.ToDropReason(),
                                Error = ex == null
                                    ? null
                                    : new Error
                                    {
                                        Text = ex.Message,
                                        Type = ex.GetType().Name
                                    },
                            })
                            .Wait();
                        Log.Debug("Subscription drop sent to subscriber");
                    }
                    catch (InvalidOperationException e)
                    {
                        Log.Warn("Failed to send drop reason to subscriber", e);
                    }
                });
            Log.Debug("Subscription started");
            Log.Debug("Waiting for subscription close...");
            while (await requestStream.MoveNext())
            {
                Log.Debug("Receiving another subscription request... ignoring");
            }
            Log.Debug("Subscription closed");
            Log.Debug("Stoping subscription...");
            await Task.Run(() =>
            {
                try
                {
                    sub.Stop(TimeSpan.FromSeconds(10));
                }
                catch (TimeoutException e)
                {
                    Log.Error("Took too long to stop subscription", e);
                }
            });
            Log.Debug("Subscription stopped");
        }
    }
}