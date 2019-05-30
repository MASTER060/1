using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RemoteFork.Items;
using RemoteFork.Plugins;
using RemoteFork.Tools;

namespace RemoteFork.Requests {
    public class PluginRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "plugin";

        public const string PARAM_PLUGIN_KEY = "pluginId";

        internal static readonly Regex PluginParamRegex =
            new Regex($@"{PARAM_PLUGIN_KEY}(\w+)[\\]?", RegexOptions.Compiled);

        public override async Task<string> Handle(HttpRequest request, HttpResponse response) {
            string pluginKey = request.Query[PARAM_PLUGIN_KEY];

            if (!string.IsNullOrEmpty(pluginKey)) {
                var plugin = PluginManager.Instance.GetPlugin(pluginKey);

                if (plugin != null) {
                    Log.LogDebug("Execute: {0}", plugin.ToString());

                    try {
                        PlayList playList = null;

                        await Task.Run((() => {
                            var query = request.Query.ConvertToNameValue();
                            var context = new PluginContext(pluginKey, request, query);
                            playList = plugin.GetPlayList(context);
                        }));

                        if (playList != null) {
                            // var playList = new PlayList(pluginResponse);

                            if (!string.IsNullOrEmpty(playList.Source)) {
                                Log.LogDebug(
                                    "Plugin Playlist.source not null! Write to response Playlist.source and ignore other methods. Plugin: {0}",
                                    pluginKey);
                                return playList.Source;
                            } else {
                                return ResponseManager.CreateResponse(playList);
                            }
                        }
                    } catch (Exception exception) {
                        Log.LogError(exception);
                        response.StatusCode = (int) HttpStatusCode.BadRequest;
                        return $"Plugin: {pluginKey}";
                    }
                }
            }

            response.StatusCode = (int) HttpStatusCode.NotFound;
            return $"Plugin is not defined in request. Plugin: {pluginKey}";
        }

        internal static string CreatePluginUrl(HttpRequest request,
                                               string pluginName,
                                               NameValueCollection parameters = null) {
            var query = new NameValueCollection {
                {PARAM_PLUGIN_KEY, pluginName},
                {"host", request.Host.ToUriComponent()}
            };

            if (parameters != null) {
                foreach (string parameter in parameters.AllKeys) {
                    query.Add(parameter, parameters[parameter]);
                }
            }

            return CreateUrl(request, URL_PATH, query);
        }
    }
}