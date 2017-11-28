using System;
using NLog;
using Unosquare.Labs.EmbedIO;

namespace RemoteFork {

    public sealed class EmbedIOLogger : Log.ILog {
        private static readonly ILogger Logger = LogManager.GetLogger("WebServer", typeof(WebServer));

        public void Info(object message) {
            Logger.Info(message);
        }

        public void Error(object message) {
            Logger.Error(message);
        }

        public void Error(object message, Exception exception) {
            Logger.Error(exception);
        }

        public void InfoFormat(string format, params object[] args) {
            Logger.Info(format, args);
        }

        public void WarnFormat(string format, params object[] args) {
            Logger.Warn(format, args);
        }

        public void ErrorFormat(string format, params object[] args) {
            Logger.Error(format, args);
        }

        public void DebugFormat(string format, params object[] args) {
            Logger.Debug(format, args);
        }

        public void Debug(string message) {
            Logger.Debug(message);
        }
    }
}
