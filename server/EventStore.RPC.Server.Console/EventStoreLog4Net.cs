using System;
using EventStore.ClientAPI;
using log4net;

namespace EventStore.RPC.Server.Console
{
    internal class EventStoreLog4Net : ILogger
    {
        private static readonly ILog Log = LogManager.GetLogger("EventStore");

        public void Error(string format, params object[] args)
        {
            Log.ErrorFormat(format, args);
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            Log.Error(string.Format(format, args), ex);
        }

        public void Info(string format, params object[] args)
        {
            Log.InfoFormat(format, args);
        }

        public void Info(Exception ex, string format, params object[] args)
        {
            Log.Info(string.Format(format, args), ex);
        }

        public void Debug(string format, params object[] args)
        {
            Log.DebugFormat(format, args);
        }

        public void Debug(Exception ex, string format, params object[] args)
        {
            Log.Debug(string.Format(format, args), ex);
        }
    }
}