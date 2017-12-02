using NLog;

namespace RemoteFork {
    internal class AppLogLevel {
        private const int NONE = 0,
                          INFO = 1,
                          ERROR = 2,
                          DEBUG = 3;

        public static LogLevel FromOrdinal(int ordinal) {
            switch (ordinal) {
                case INFO:
                    return LogLevel.Info;
                case ERROR:
                    return LogLevel.Error;
                case DEBUG:
                    return LogLevel.Debug;
                case NONE:
                default:
                    return LogLevel.Off;
            }
        }
    }
}
