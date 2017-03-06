using System;
using log4net;

namespace EventStore.RPC.Server.Console
{
    internal class GrpcLog4Net : Grpc.Core.Logging.ILogger
    {
        private readonly ILog _log;

        public GrpcLog4Net(string name)
        {
            _log = LogManager.GetLogger(name);
        }

        public Grpc.Core.Logging.ILogger ForType<T>()
        {
            return new GrpcLog4Net(typeof(T).Name);
        }

        public void Debug(string message)
        {
            _log.Debug(message);
        }

        public void Debug(string format, params object[] formatArgs)
        {
            _log.DebugFormat(format, formatArgs);
        }

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void Info(string format, params object[] formatArgs)
        {
            _log.InfoFormat(format, formatArgs);
        }

        public void Warning(string message)
        {
            _log.Warn(message);
        }

        public void Warning(string format, params object[] formatArgs)
        {
            _log.WarnFormat(format, formatArgs);
        }

        public void Warning(Exception exception, string message)
        {
            _log.Warn(message, exception);
        }

        public void Error(string message)
        {
            _log.Error(message);
        }

        public void Error(string format, params object[] formatArgs)
        {
            _log.ErrorFormat(format, formatArgs);
        }

        public void Error(Exception exception, string message)
        {
            _log.Error(message, exception);
        }
    }
}