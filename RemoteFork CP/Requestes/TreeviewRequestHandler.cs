using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RemoteFork.Plugins;
using RemoteFork.Server;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace RemoteFork.Requestes {
    internal class TreeviewRequestHandler : BaseRequestHandler {
        private static readonly ILogger Log = Program.LoggerFactory.CreateLogger<TreeviewRequestHandler>();

        internal static readonly string UrlPath = "/treeview";

        //internal static readonly string RootPath = "/";

        public override string Handle(HttpRequest request, HttpResponse response) {
            var result = new List<Item>();

            if (SettingsManager.Settings.DlnaFilterType == SettingsManager.FilterMode.INCLUSION) {
                if (SettingsManager.Settings.DlnaDirectories != null) {
                    foreach (var directory in SettingsManager.Settings.DlnaDirectories) {
                        if (Directory.Exists(directory)) {
                            result.Add(DlnaDirectoryRequestHandler.CreateDirectoryItem(request, directory));

                            Log.LogDebug($"Filtering directory: {directory}");
                        }
                    }
                }
            } else {
                var drives = DriveInfo.GetDrives();

                foreach (var drive in drives.Where(i => Tools.CheckAccessPath(i.Name))) {
                    if (drive.IsReady) {
                        string mainText =
                            $"{drive.Name} ({Tools.FSize(drive.AvailableFreeSpace)} свободно из {Tools.FSize(drive.TotalSize)})";
                        string subText = $"<br>Метка диска: {drive.VolumeLabel}<br>Тип носителя: {drive.DriveType}";

                        result.Add(new Item {
                            Name = mainText + subText,
                            Link = DlnaDirectoryRequestHandler.CreateDriveItem(request, drive),
                            Type = ItemType.DIRECTORY
                        });

                        Log.LogDebug($"Drive: {mainText}{subText}");
                    }
                }
            }

            if ((SettingsManager.Settings.UserUrls != null) && (SettingsManager.Settings.UserUrls.Length > 0)) {
                result.Add(
                    new Item {
                        Name = "Пользовательские ссылки",
                        Link = CreateUrl(request, UserUrlsRequestHandler.UrlPath,
                            new NameValueCollection() {
                                {string.Empty, UserUrlsRequestHandler.ParamUrls}
                            }),
                        Type = ItemType.DIRECTORY
                    }
                );

                Log.LogDebug("User urls: {0}", SettingsManager.Settings.UserUrls.Length);
            }

            foreach (var plugin in PluginManager.Instance.GetPlugins()) {
                result.Add(
                    new Item {
                        Name = plugin.Value.Name,
                        Link = PluginRequestHandler.CreatePluginUrl(request, plugin.Key),
                        ImageLink = plugin.Value.ImageLink,
                        Type = ItemType.DIRECTORY
                    }
                );

                Log.LogDebug("Plugin: {0}", plugin.Value.Name);
            }
            
            return  ResponseSerializer.ToM3U(result.ToArray());
        }
    }
}
