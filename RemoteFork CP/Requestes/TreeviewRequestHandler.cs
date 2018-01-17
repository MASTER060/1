using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RemoteFork.Plugins;
using RemoteFork.Settings;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace RemoteFork.Requestes {
    public class TreeviewRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "treeview";

        private static readonly ILogger Log = Program.LoggerFactory.CreateLogger<TreeviewRequestHandler>();

        public override string Handle(HttpRequest request, HttpResponse response) {
            var result = new List<Item>();

            if (ProgramSettings.Settings.DlnaFilterType == FilterMode.INCLUSION) {
                if (ProgramSettings.Settings.DlnaDirectories != null) {
                    foreach (var directory in ProgramSettings.Settings.DlnaDirectories) {
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

            if ((ProgramSettings.Settings.UserUrls != null) && (ProgramSettings.Settings.UserUrls.Length > 0)) {
                result.Add(
                    new Item {
                        Name = "Пользовательские ссылки",
                        Link = CreateUrl(request, UserUrlsRequestHandler.URL_PATH,
                            new NameValueCollection() {
                                {string.Empty, UserUrlsRequestHandler.PARAM_URLS}
                            }),
                        Type = ItemType.DIRECTORY
                    }
                );

                Log.LogDebug("User urls: {0}", ProgramSettings.Settings.UserUrls.Length);
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
