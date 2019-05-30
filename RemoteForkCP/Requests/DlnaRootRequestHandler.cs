using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RemoteFork.Items;
using RemoteFork.Plugins;
using RemoteFork.Server;
using RemoteFork.Settings;
using RemoteFork.Updater;

namespace RemoteFork.Requests {
    public class DlnaRootRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "treeview";

        public override async Task<string> Handle(HttpRequest request, HttpResponse response) {
            var items = new List<IItem>();

            await Task.Run((() => {

                if (ProgramSettings.Settings.CheckUpdate) {
                    if (UpdateController.IsUpdateAvaiable("RemoteFork")) {
                        items.Add(
                            new FileItem() {
                                Title =
                                    $"Доступна новая версия: {UpdateController.GetUpdater("RemoteFork").GetLatestVersionNumber(false).Result}",
                                Link = "http://newversion.m3u"
                            }
                        );
                    }
                }

                if (ProgramSettings.Settings.Dlna) {
                    if (ProgramSettings.Settings.DlnaFilterType == FilterMode.INCLUSION) {
                        if (ProgramSettings.Settings.DlnaDirectories != null) {
                            foreach (string directory in ProgramSettings.Settings.DlnaDirectories) {
                                Console.WriteLine(directory);
                                if (FileManager.DirectoryExists(directory)) {
                                    Console.WriteLine(true);
                                    items.Add(DlnaDirectoryRequestHandler.CreateDirectoryItem(request, directory));

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
                                string subText =
                                    $"<br>Метка диска: {drive.VolumeLabel}<br>Тип носителя: {drive.DriveType}";

                                items.Add(new DirectoryItem() {
                                    Title = mainText + subText,
                                    Link = DlnaDirectoryRequestHandler.CreateDriveItem(request, drive.Name)
                                });

                                Log.LogDebug($"Drive: {mainText}{subText}");
                            }
                        }
                    }
                }


                if ((ProgramSettings.Settings.UserUrls != null) && (ProgramSettings.Settings.UserUrls.Length > 0)) {
                    items.Add(
                        new DirectoryItem() {
                            Title = "Пользовательские ссылки",
                            Link = CreateUrl(request, UserUrlsRequestHandler.URL_PATH,
                                new NameValueCollection() {
                                    {string.Empty, UserUrlsRequestHandler.PARAM_URLS}
                                })
                        }
                    );

                    Log.LogDebug("User urls: {0}", ProgramSettings.Settings.UserUrls.Length);
                }

                foreach (var plugin in PluginManager.Instance.GetPlugins()) {
                    var item = new DirectoryItem() {
                        Title = plugin.Value.Name,
                        Link = PluginRequestHandler.CreatePluginUrl(request, plugin.Key)
                    };
                    if (ProgramSettings.Settings.PluginIcons) {
                        item.ImageLink = PluginIconRequestHandler.CreateImageUrl(request, plugin.Value);
                    }

                    items.Add(item);

                    Log.LogDebug("Plugin: {0}", plugin.Value.ToString());
                }
            }));

            PlayList playList = null;

            if (ProgramSettings.Settings.StartPageModernStyle) {
                playList = new RootPlayList();
            } else {
                playList = new PlayList();
            }

            playList.Items = items.ToArray();

            return ResponseManager.CreateResponse(playList);
        }

        private class RootPlayList : PlayList {
            [JsonProperty("typeList", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string TypeList = "start";
        }
    }
}
