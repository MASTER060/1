using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RemoteFork.Network;
using RemoteFork.Plugins;
using RemoteFork.Tools;

namespace RemoteFork.Requests {
    public class PluginIconRequestHandler : BaseRequestHandler<Stream> {
        public const string URL_PATH = "plugin_icon";

        public override async Task<Stream> Handle(HttpRequest request, HttpResponse response) {
            var pluginId = request.Query[PluginRequestHandler.PARAM_PLUGIN_KEY];

            if (!string.IsNullOrEmpty(pluginId)) {
                var plugin = PluginManager.Instance.GetPlugin(pluginId);

                if (plugin != null) {
                    Log.LogDebug("Getting Icon For plugin: {0}", plugin.ToString());

                    Stream stream = null;

                    await Task.Run((() => {
                        var pluginImage = new PluginImage(plugin.Attribute.ImageLink, pluginId);

                        // response.ContentType = pluginImage.ContentType;

                        stream = pluginImage.GetStream().Result;
                    }));

                    return stream;
                }
            } else {
                Log.LogDebug($"Plugin is not defined in request. Plugin: {pluginId}");

                response.StatusCode = (int) HttpStatusCode.NotFound;
            }

            return null;
        }

        protected override void SetDefaultResponseHeaders(HttpResponse response) {
            response.ContentType = MimeTypes.Get(".png");
        }

        internal static string CreateImageUrl(HttpRequest request, PluginInstance plugin) {
            try {
                return CreateUrl(request,
                    URL_PATH,
                    new NameValueCollection {
                        {PluginRequestHandler.PARAM_PLUGIN_KEY, plugin.Id}
                    }
                );
            } catch (Exception exception) {
                Log.LogError(exception);
            }

            return plugin.Attribute.ImageLink;
        }
    }

    internal class PluginImage {
        private readonly string _imageLink;
        private readonly string _pluginName;

        public PluginImage(string imageLink, string pluginName) {
            _imageLink = string.IsNullOrEmpty(imageLink) ? string.Empty : imageLink;
            _pluginName = string.IsNullOrEmpty(pluginName) ? string.Empty : pluginName;
        }

        public bool IsLocalFile {
            get {
                if (File.Exists(LocalFilePath)) {
                    return true;
                }

                return false;
            }
        }

        private string LocalFilePath {
            get { return Path.Combine(PluginManager.PluginsPath, _pluginName + ".png"); }
        }

        public async Task<Stream> GetStream() {
            MemoryStream iconData = null;

            try {
                byte[] buffer;

                if (!IsLocalFile) {
                    buffer = await HTTPUtility.GetBytesRequestAsync(_imageLink);

                    await File.WriteAllBytesAsync(LocalFilePath, buffer);
                } else {
                    buffer = await File.ReadAllBytesAsync(LocalFilePath);
                }

                iconData = new MemoryStream(buffer);

            } catch (Exception exception) {
            }

            return iconData;
        }
    }
}

