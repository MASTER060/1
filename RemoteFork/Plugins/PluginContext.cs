using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NLog;
using PluginApi.Plugins;
using RemoteFork.Network;
using RemoteFork.Requestes;
using Unosquare.Net;
using ILogger = PluginApi.Plugins.ILogger;

namespace RemoteFork.Plugins {
    internal class PluginContext : IPluginContext {
        private readonly IHTTPClient _httpClient = new HttpClient();

        private readonly ILogger _logger;

        private readonly string _pluginName;
        private readonly HttpListenerRequest _request;
        private readonly NameValueCollection _requestParams;

        public PluginContext(string pluginName, HttpListenerRequest request, NameValueCollection requestParams) {
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

        public void ConsoleLog(string s)
        {
            Console.WriteLine(s);
        }

        public IHTTPClient GetHttpClient() {
            return _httpClient;
        }
        public ILogger GetLogger() {
            return _logger;
        }

        internal class HttpClient : IHTTPClient {
            public string GetRequest(string link, Dictionary<string, string> header = null) {
                return HttpUtility.GetRequest(link, header);
            }

            public string PostRequest(string link, string data, Dictionary<string, string> header = null) {
                return HttpUtility.PostRequest(link, data, header);
            }
        }

        internal class Logger : ILogger {
            private readonly NLog.ILogger _log;

            public Logger(string pluginName) {
                _log = LogManager.GetLogger(pluginName);
            }

            public void Info(string message) {
                _log.Info(message);
            }

            public void Info(string format, params object[] args) {
                _log.Info(format, args);
            }

            public void Error(string message) {
                _log.Error(message);
            }

            public void Error(string format, params object[] args) {
                _log.Error(format, args);
            }

            public void Debug(string message) {
                _log.Debug(message);
            }

            public void Debug(string format, params object[] args) {
                _log.Debug(format, args);
            }
        }
    }
}
