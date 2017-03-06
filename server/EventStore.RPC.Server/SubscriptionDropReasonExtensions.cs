using System;
using EventStore.ClientAPI;

namespace EventStore.RPC.Server
{
    public static class SubscriptionDropReasonExtensions
    {
        public static SubscribeToStreamFromResponse.Types.DropReason ToDropReason(this SubscriptionDropReason dropReason)
        {
            switch (dropReason)
            {
                case SubscriptionDropReason.UserInitiated:
                    return SubscribeToStreamFromResponse.Types.DropReason.UserInitiated;
                case SubscriptionDropReason.NotAuthenticated:
                    return SubscribeToStreamFromResponse.Types.DropReason.NotAuthenticated;
                case SubscriptionDropReason.AccessDenied:
                    return SubscribeToStreamFromResponse.Types.DropReason.AccessDenied;
                case SubscriptionDropReason.SubscribingError:
                    return SubscribeToStreamFromResponse.Types.DropReason.SubscribingError;
                case SubscriptionDropReason.ServerError:
                    return SubscribeToStreamFromResponse.Types.DropReason.ServerError;
                case SubscriptionDropReason.ConnectionClosed:
                    return SubscribeToStreamFromResponse.Types.DropReason.ConnectionClosed;
                case SubscriptionDropReason.CatchUpError:
                    return SubscribeToStreamFromResponse.Types.DropReason.CatchUpError;
                case SubscriptionDropReason.ProcessingQueueOverflow:
                    return SubscribeToStreamFromResponse.Types.DropReason.ProcessingQueueOverflow;
                case SubscriptionDropReason.EventHandlerException:
                    return SubscribeToStreamFromResponse.Types.DropReason.EventHandlerException;
                case SubscriptionDropReason.MaxSubscribersReached:
                    return SubscribeToStreamFromResponse.Types.DropReason.MaxSubscribersReached;
                case SubscriptionDropReason.PersistentSubscriptionDeleted:
                    return SubscribeToStreamFromResponse.Types.DropReason.PersistentSubscriptionDeleted;
                case SubscriptionDropReason.Unknown:
                    return SubscribeToStreamFromResponse.Types.DropReason.Unknown;
                case SubscriptionDropReason.NotFound:
                    return SubscribeToStreamFromResponse.Types.DropReason.NotFound;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dropReason), dropReason, null);
            }
        }
    }
}