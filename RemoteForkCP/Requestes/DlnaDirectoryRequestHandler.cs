using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Http;
using RemoteFork.Plugins;
using RemoteFork.Server;
using RemoteFork.Settings;

namespace RemoteFork.Requestes {
    public class DlnaDirectoryRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "dlna_directory";

        public override string Handle(HttpRequest request, HttpResponse response) {
            string rootDirectory = string.Empty;
            if (request.Query.ContainsKey(string.Empty)) {
                rootDirectory = request.Query[string.Empty].FirstOrDefault(s => s.EndsWith(".xml"));
            }

            if (ProgramSettings.Settings.Dlna && !string.IsNullOrEmpty(rootDirectory)) {
                rootDirectory =
                    new Uri(rootDirectory.Remove(rootDirectory.IndexOf(".xml", StringComparison.Ordinal))).LocalPath;

                var directoriesInfo = FileManager.GetDirectories(rootDirectory);

                var result = new List<Item>();

                foreach (var directory in directoriesInfo.Where(d => Tools.Tools.CheckAccessPath(d.Key))) {
                    result.Add(CreateDirectoryItem(request, directory.Key, directory.Value));

                    Log.LogDebug("Directory: {0}", directory);
                }

                var filesInfo = FileManager.GetFiles(rootDirectory);

                foreach (var file in filesInfo.Where(f => Tools.Tools.CheckAccessPath(f.Key))) {
                    result.Add(
                        new Item {
                            //Name = $"{file.Value} ({Tools.Tools.FSize(file.Length)})",
                            Name = $"{file.Value} ({Tools.Tools.FSize(new FileInfo(file.Key).Length)})",
                            Link = CreateUrl(request, DlnaFileRequestHandler.URL_PATH,
                                new NameValueCollection() {
                                    {string.Empty, HttpUtility.UrlEncode(file.Key)}
                                }),
                            Type = ItemType.FILE
                        }
                    );

                    Log.LogDebug("File: {0}", file);
                }

                return ResponseSerializer.ToXml(result);
            } else {
                Log.LogDebug("Directory Not Found: {0}", rootDirectory);
                response.StatusCode = (int) HttpStatusCode.NotFound;
                return $"Directory Not Found: {rootDirectory}";
            }
        }

        internal static Item CreateDirectoryItem(HttpRequest request, string directory) {
            return CreateDirectoryItem(request, directory, directory);
        }

        internal static Item CreateDirectoryItem(HttpRequest request, string directoryPath, string directoryName) {

            return new Item {
                Name = directoryName,
                Link = CreateUrl(
                    request,
                    URL_PATH,
                    new NameValueCollection() {
                        {
                            string.Empty,
                            string.Concat(directoryPath, ".xml")
                        }
                    }
                ),
                Type = ItemType.DIRECTORY
            };
        }

        internal static string CreateDriveItem(HttpRequest request, string directory) {
            var query = new NameValueCollection() {
                {
                    string.Empty,
                    string.Concat(directory, ".xml")
                }
            };

            return CreateUrl(request, URL_PATH, query);
        }
    }
}
