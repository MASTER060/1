using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using RemoteFork.Log;
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

        public IHttpClient GetHttpClient() {
            return _httpClient;
        }

        public void ConsoleLog(string text) {
            Console.WriteLine(text);
        }

        public ILogger GetLogger() {
            return _logger;
        }
    }
}
