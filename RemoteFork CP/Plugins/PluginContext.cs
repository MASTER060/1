using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RemoteFork.Network;
using RemoteFork.Requestes;

namespace RemoteFork.Plugins {
    internal class PluginContext : IPluginContext {
        private readonly HttpClient _httpClient = new HttpClient();

        private readonly ILogger _logger;

        private readonly string _pluginName;
        private readonly HttpRequest _request;
        private readonly NameValueCollection _requestParams;

        public PluginContext(string pluginName, HttpRequest request, NameValueCollection requestParams) {
            _pluginName = pluginName;
            _request = request;
            _requestParams = requestParams;
            _logger = new Logger(pluginName);
        }

        public NameValueCollection GetRequestParams() {
            return _requestParams;
        }

        public string CreatePluginUrl(NameValueCollection parameters) {
            return PluginRequestHandler.CreatePluginUrl(_request, _pluginName, parameters);
        }

        public void ConsoleLog(string s) {
            Console.WriteLine(s);
        }

        public IHttpClient GetHttpClient() {
            return _httpClient;
        }

        public ILogger GetLogger() {
            return _logger;
        }

        internal class HttpClient : IHttpClient {
            public string GetRequest(string link, Dictionary<string, string> header = null) {
                return HTTPUtility.GetRequest(link, header);
            }

            public string PostRequest(string link, string data, Dictionary<string, string> header = null) {
                return HTTPUtility.PostRequest(link, data, header);
            }
        }

        internal class Logger : ILogger {
            private readonly Microsoft.Extensions.Logging.ILogger _log;

            public Logger(string pluginName) {
                _log = Program.LoggerFactory.CreateLogger(pluginName);
            }

            public void Info(string message) {
                _log.LogInformation(message);
            }

            public void Info(string format, params object[] args) {
                _log.LogInformation(format, args);
            }

            public void Error(string message) {
                _log.LogError(message);
            }

            public void Error(string format, params object[] args) {
                _log.LogError(format, args);
            }

            public void Debug(string message) {
                _log.LogDebug(message);
            }

            public void Debug(string format, params object[] args) {
                _log.LogDebug(format, args);
            }
        }
    }
}
