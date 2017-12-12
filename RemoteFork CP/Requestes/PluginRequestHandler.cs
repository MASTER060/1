using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RemoteFork.Network;
using RemoteFork.Plugins;
using RemoteFork.Server;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace RemoteFork.Requestes {
    internal class PluginRequestHandler : BaseRequestHandler {
        private static readonly ILogger Log = Program.LoggerFactory.CreateLogger<PluginRequestHandler>();

        internal static readonly string UrlPath = "/plugin";
        internal static readonly string ParamPluginKey = "plugin";

        internal static readonly Regex PluginParamRegex = new Regex($@"{ParamPluginKey}(\w+)[\\]?", RegexOptions.Compiled);

        public override string Handle(HttpRequest request, HttpResponse response) {
            var pluginKey = ParsePluginKey(request);

            if (!string.IsNullOrEmpty(pluginKey)) {
                var plugin = PluginManager.Instance.GetPlugin(pluginKey);

                if (plugin != null) {
                    Log.LogDebug("Execute: {0}", plugin.Name);

                    var pluginResponse = plugin.Instance.GetList(new PluginContext(pluginKey, request, request.Query.ConvertToNameValue()));

                    if (pluginResponse != null) {
                        if (pluginResponse.source != null) {
                            Log.LogDebug(
                                "Plugin Playlist.source not null! Write to response Playlist.source and ignore other methods. Plugin: {0}",
                                pluginKey);
                            return pluginResponse.source;
                        } else {
                            return ResponseSerializer.ToXml(pluginResponse);
                        }
                    }
                }
            }
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return $"Plugin is not defined in request. Plugin: {pluginKey}";
        }

        private static string ParsePluginKey(HttpRequest request) {
            string pluginParam = string.Empty;
            if (request.Query.ContainsKey(string.Empty)) {
                pluginParam = request.Query[string.Empty].FirstOrDefault(s => PluginParamRegex.IsMatch(s ?? string.Empty));
            }

            var pluginParamMatch = PluginParamRegex.Match(pluginParam ?? string.Empty);

            return pluginParamMatch.Success ? pluginParamMatch.Groups[1].Value : string.Empty;
        }

        internal static string CreatePluginUrl(HttpRequest request, string pluginName, NameValueCollection parameters = null) {
            var query = new NameValueCollection {
                {string.Empty, string.Concat(ParamPluginKey, pluginName, Path.DirectorySeparatorChar, ".xml")},
                {"host", request.Host.ToUriComponent()}
            };

            if (parameters != null) {
                foreach (var parameter in parameters.AllKeys) {
                    query.Add(parameter, parameters[parameter]);
                }
            }

            return CreateUrl(request, UrlPath, query);
        }
    }
}