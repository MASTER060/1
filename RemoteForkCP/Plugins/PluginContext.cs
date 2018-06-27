using Microsoft.AspNetCore.Http;
using RemoteFork.Requestes;
using System.Collections.Specialized;
using RemoteFork.Settings;
using RemoteFork.Updater;

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

        public string GetLatestVersionNumber(string id) {
            if (ProgramSettings.Settings.CheckUpdate) {
                if (UpdateController.IsUpdateAvaiable(id)) {
                    var updater = UpdateController.GetUpdater(id);
                    return updater.GetLatestVersionNumber(false).Result;
                }
            }

            return null;
        }
    }
}
