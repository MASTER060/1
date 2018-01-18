using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using RemoteFork.Requestes;

namespace RemoteFork.Plugins {
    internal class PluginContext : IPluginContext {
        private readonly string _pluginName;
        private readonly HttpRequest _request;
        private readonly NameValueCollection _requestParams;

        public PluginContext(string pluginName, HttpRequest request, NameValueCollection requestParams) {
            _pluginName = pluginName;
            _request = request;
            _requestParams = requestParams;
        }

        public NameValueCollection GetRequestParams() {
            return _requestParams;
        }

        public string CreatePluginUrl(NameValueCollection parameters) {
            return PluginRequestHandler.CreatePluginUrl(_request, _pluginName, parameters);
        }
    }
}
