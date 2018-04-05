using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.Http;
using RemoteFork.Plugins;
using RemoteFork.Server;
using RemoteFork.Settings;

namespace RemoteFork.Requestes {
    public class DlnaRootRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "treeview";

        public override string Handle(HttpRequest request, HttpResponse response) {
            var result = new List<Item>();

            if (ProgramSettings.Settings.Dlna) {
                if (ProgramSettings.Settings.DlnaFilterType == FilterMode.INCLUSION) {
                    if (ProgramSettings.Settings.DlnaDirectories != null) {
                        foreach (string directory in ProgramSettings.Settings.DlnaDirectories) {
                            if (FileManager.DirectoryExists(directory)) {
                                result.Add(DlnaDirectoryRequestHandler.CreateDirectoryItem(request, directory));

                                Log.LogDebug($"Filtering directory: {directory}");
                            }
                        }
                    }
                } else {
                    var drives = FileManager.GetDrives();

                    foreach (var drive in drives.Where(i => Tools.Tools.CheckAccessPath(i.Name))) {
                        if (drive.IsReady) {
                            string mainText =
                                $"{drive.Name} ({Tools.Tools.FSize(drive.AvailableFreeSpace)} свободно из {Tools.Tools.FSize(drive.TotalSize)})";
                            string subText = $"<br>Метка диска: {drive.VolumeLabel}<br>Тип носителя: {drive.DriveType}";

                            result.Add(new Item {
                                Name = mainText + subText,
                                Link = DlnaDirectoryRequestHandler.CreateDriveItem(request, drive.Name),
                                Type = ItemType.DIRECTORY
                            });

                            Log.LogDebug($"Drive: {mainText}{subText}");
                        }
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

            return ResponseSerializer.ToM3U(result.ToArray());
        }
    }
}
