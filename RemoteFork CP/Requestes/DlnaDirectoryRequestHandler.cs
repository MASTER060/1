using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RemoteFork.Plugins;
using RemoteFork.Server;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace RemoteFork.Requestes {
    public class DlnaDirectoryRequestHandler : BaseRequestHandler {
        private static readonly ILogger Log = Program.LoggerFactory.CreateLogger<DlnaDirectoryRequestHandler>();

        public const string UrlPath = "directory";
        
        public override string Handle(HttpRequest request, HttpResponse response) {
            string rootDirectory = string.Empty;
            if (request.Query.ContainsKey(string.Empty)) {
                rootDirectory = request.Query[string.Empty].FirstOrDefault(s => s.EndsWith(".xml"));
            }

            if (!string.IsNullOrEmpty(rootDirectory)) {
                rootDirectory = new Uri(rootDirectory.Remove(rootDirectory.IndexOf(".xml", StringComparison.InvariantCulture))).LocalPath;

                var directories = Directory.GetDirectories(rootDirectory).OrderBy(d => d);
                var directoriesInfo = directories.Select(directory => new DirectoryInfo(directory)).ToList();

                var result = new List<Item>();

                foreach (var directory in directoriesInfo.Where(Tools.CheckAccessPath)) {
                    result.Add(CreateDirectoryItem(request, directory));

                    Log.LogDebug("Directory: {0}", directory);
                }

                var files = Directory.GetFiles(rootDirectory).OrderBy(f => f);
                var filesInfo = files.Select(file => new FileInfo(file)).ToList();

                foreach (var file in filesInfo.Where(Tools.CheckAccessPath)) {
                    result.Add(
                        new Item {
                            Name = $"{file.Name} ({Tools.FSize(file.Length)})",
                            Link = CreateUrl(request, DlnaFileRequestHandler.UrlPath,
                                new NameValueCollection() {
                                    {string.Empty, new Uri(file.FullName).AbsoluteUri}
                                }),
                            Type = ItemType.FILE
                        }
                    );

                   Log.LogDebug("File: {0}", file);
                }

                return ResponseSerializer.ToXml(result.ToArray());
            } else {
                Log.LogDebug("Directory Not Found: {0}", rootDirectory);
                response.StatusCode = (int) HttpStatusCode.NotFound;
                return $"Directory Not Found: {rootDirectory}";
            }
        }

        internal static Item CreateDirectoryItem(HttpRequest request, string directory) {
            return CreateDirectoryItem(request, new DirectoryInfo(directory));
        }

        internal static Item CreateDirectoryItem(HttpRequest request, DirectoryInfo directory) {

            return new Item {
                Name = directory.Name,
                Link = CreateUrl(
                    request,
                    UrlPath,
                    new NameValueCollection() {
                        {
                            string.Empty,
                            string.Concat(directory.FullName, ".xml")
                        }
                    }
                ),
                Type = ItemType.DIRECTORY
            };
        }

        internal static string CreateDriveItem(HttpRequest request, DriveInfo directory) {
            var query = new NameValueCollection() {
                {
                    string.Empty,
                    string.Concat(directory.Name, ".xml")
                }
            };

            return CreateUrl(request, UrlPath, query);
        }
    }
}
