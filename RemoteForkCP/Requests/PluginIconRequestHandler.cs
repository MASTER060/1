using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RemoteFork.Log;
using RemoteFork.Plugins;

namespace RemoteFork.Requests {
    public class PluginIconRequestHandler : BaseRequestHandler<Stream> {
        private static readonly Logger Log = new Logger(typeof(PluginIconRequestHandler));

        public const string URL_PATH = "plugin_icon";

        public override async Task<Stream> Handle(HttpRequest request, HttpResponse response) {
            var pluginId = request.Query[PluginRequestHandler.PARAM_PLUGIN_KEY];

            if (!string.IsNullOrEmpty(pluginId)) {
                var plugin = PluginManager.Instance.GetPlugin(pluginId);

                if (plugin != null) {
                    Log.LogDebug("Getting Icon For plugin: {0}", plugin.ToString());

                    Stream stream = null;

                    await Task.Run((() => {
                        var pluginImage = new PluginImage(plugin.Attribute.ImageLink);
                        
                        response.ContentType = pluginImage.ContentType;

                        stream = pluginImage.GetStream(plugin);
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
            response.ContentType = PluginImage.MimeTypePng;
        }

        internal static string CreateImageUrl(HttpRequest request, PluginInstance plugin) {
            try {
                var pluginImage = new PluginImage(plugin.Attribute.ImageLink);

                if (pluginImage.IsLocalImage) {
                    return CreateUrl(request,
                                     URL_PATH,
                                     new NameValueCollection {
                                         {PluginRequestHandler.PARAM_PLUGIN_KEY, plugin.Id}
                                     }
                    );
                }
            } catch (Exception) {
                // ignored
            }

            return plugin.Attribute.ImageLink;
        }
    }

    internal class PluginImage {
        public const string MimeTypePng = "image/png";
        
        private const string PrefixFile = "file:";

        private const string PrefixPlugin = "plugin:";

        private readonly string _imageLink;

        public PluginImage(string imageLink) {
            _imageLink = string.IsNullOrEmpty(imageLink) ? string.Empty : imageLink;
        }

        public bool IsLocalImage => IsLocalFile || IsPluginFile;

        private bool IsLocalFile => _imageLink.StartsWith(PrefixFile);

        private bool IsPluginFile => _imageLink.StartsWith(PrefixPlugin);

        public string ContentType {
            get {
                try {
                    var extension = Path.GetExtension(LocalPath);

                    switch (extension) {
                        case ".jpg":
                        case ".jpeg":
                            return MediaTypeNames.Image.Jpeg;
                        case ".png":
                            return MimeTypePng;
                        case ".gif":
                            return MediaTypeNames.Image.Gif;
                    }
                } catch (Exception) {
                    // ignored
                }

                return MimeTypePng;
            }
        }

        private string LocalPath {
            get {
                if (IsLocalFile) {
                    return _imageLink.Substring(PrefixFile.Length);
                }

                if (IsPluginFile) {
                    return _imageLink.Substring(PrefixPlugin.Length); 
                }

                return _imageLink;
            }
        }
        
        public Stream GetStream(PluginInstance plugin) {
            try {
                if (IsLocalFile) {
                    return new FileStream(Path.Combine(PluginManager.PluginsPath, LocalPath), FileMode.Open, FileAccess.Read);
                }

                if (IsPluginFile) {
                    return plugin.Assembly.GetManifestResourceStream(LocalPath);
                }
            } catch (Exception e) {
                //
            }

            return null;
        }
    }
}