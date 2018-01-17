using Microsoft.Extensions.Logging;

namespace RemoteFork.Log
{
    public class Logger : Plugins.ILogger {
        private readonly ILogger _logger;

        public Logger(string pluginName) {
            _logger = Program.LoggerFactory.CreateLogger(pluginName);
        }

        public void Info(string message) {
            _logger.LogInformation(message);
        }

        public void Info(string format, params object[] args) {
            _logger.LogInformation(format, args);
        }

        public void Error(string message) {
            _logger.LogError(message);
        }

        public void Error(string format, params object[] args) {
            _logger.LogError(format, args);
        }

        public void Debug(string message) {
            _logger.LogDebug(message);
        }

        public void Debug(string format, params object[] args) {
            _logger.LogDebug(format, args);
        }
    }
}
