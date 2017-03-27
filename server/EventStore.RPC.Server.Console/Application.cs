using System;
using System.Configuration;
using System.Linq;
using System.Net;
using EventStore.ClientAPI;
using Grpc.Core;

namespace EventStore.RPC.Server.Console
{
    internal class Application
    {
        private Grpc.Core.Server _server;
        private IEventStoreConnection _eventStoreConnection;

        private static readonly string EventStoreUri = ConfigurationManager.AppSettings.Get("EventStore.URI");
        private static readonly string EventStoreGossipSeedEndpoints =
            ConfigurationManager.AppSettings.Get("EventStore.GossipSeedEndpoints");
        private static readonly string RpcHost = ConfigurationManager.AppSettings.Get("EventStore.RPC.Host");
        private static readonly string RpcPort = ConfigurationManager.AppSettings.Get("EventStore.RPC.Port");

        public void Start()
        {
            var connectionSettings = ConnectionSettings.Create()
                .UseCustomLogger(new EventStoreLog4Net())
                .KeepReconnecting();
            if (!string.IsNullOrEmpty(EventStoreGossipSeedEndpoints))
            {
                var clusterSettings = ClusterSettings.Create()
                    .DiscoverClusterViaGossipSeeds()
                    .SetGossipSeedEndPoints(EventStoreGossipSeedEndpoints.Split(',').Select(x =>
                    {
                        var parts = x.Split(':');
                        return new IPEndPoint(IPAddress.Parse(parts[0]), Convert.ToInt32(parts[1]));
                    }).ToArray());
                _eventStoreConnection = EventStoreConnection.Create(connectionSettings, clusterSettings);
            }
            else
            {
                _eventStoreConnection = EventStoreConnection.Create(connectionSettings, new Uri(EventStoreUri));
            }
            _eventStoreConnection.ConnectAsync().Wait();

            GrpcEnvironment.SetLogger(new GrpcLog4Net("GRPC"));
            _server = new Grpc.Core.Server
            {
                Services = {EventStore.BindService(new EventStoreImpl(_eventStoreConnection))},
                Ports = {new ServerPort(RpcHost, Convert.ToInt32(RpcPort), ServerCredentials.Insecure)}
            };

            _server.Start();
        }

        public void Stop()
        {
            _server.ShutdownAsync().Wait();
            _eventStoreConnection.Close();
        }
    }
}