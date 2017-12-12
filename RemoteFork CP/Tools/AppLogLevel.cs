using Microsoft.Extensions.Logging;

namespace RemoteFork {
    internal class AppLogLevel {
        private const int NONE = 0,
                          INFO = 1,
                          ERROR = 2,
                          DEBUG = 3;

        public static LogLevel FromOrdinal(int ordinal) {
            switch (ordinal) {
                case INFO:
                    return LogLevel.Information;
                case ERROR:
                    return LogLevel.Error;
                case DEBUG:
                    return LogLevel.Debug;
                case NONE:
                default:
                    return LogLevel.None;
            }
        }
    }
}
