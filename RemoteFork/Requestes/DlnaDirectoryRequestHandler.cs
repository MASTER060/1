using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using NLog;
using RemoteFork.Plugins;
using RemoteFork.Server;
using Unosquare.Net;

namespace RemoteFork.Requestes {
    internal class DlnaDirectoryRequestHandler : BaseRequestHandler {
        private static readonly ILogger Log = LogManager.GetLogger("DlnaDirectoryRequestHandler", typeof(DlnaDirectoryRequestHandler));

        public override void Handle(HttpListenerRequest request, HttpListenerResponse response) {
            string rootDirectory = request.QueryString.GetValues(null)?.FirstOrDefault(s => s.StartsWith(Uri.UriSchemeFile));

            if (!string.IsNullOrEmpty(rootDirectory)) {
                rootDirectory = new Uri(rootDirectory).LocalPath;

                var directories = Directory.GetDirectories(rootDirectory).OrderBy(d => d);
                var directoriesInfo = directories.Select(directory => new DirectoryInfo(directory)).ToList();

                var result = new List<Item>();

                foreach (var directory in directoriesInfo.Where(Tools.CheckAccessPath)) {
                    result.Add(CreateDirectoryItem(request, directory));

                    Log.Debug("Directory: {0}", directory);
                }

                var files = Directory.GetFiles(rootDirectory).OrderBy(f => f);
                var filesInfo = files.Select(file => new FileInfo(file)).ToList();

                foreach (var file in filesInfo.Where(Tools.CheckAccessPath)) {
                    result.Add(
                        new Item {
                            Name = $"{file.Name} ({Tools.FSize(file.Length)})",
                            Link = CreateUrl(request, RootRequestHandler.RootPath,
                                new NameValueCollection { [null] = new Uri(file.FullName).AbsoluteUri }),
                            Type = ItemType.FILE
                        }
                    );

                    Log.Debug("File: {0}", file);
                }

                WriteResponse(response, ResponseSerializer.ToM3U(result.ToArray()));
            } else {
                Log.Debug("Directory Not Found: {0}", rootDirectory);

                WriteResponse(response, HttpStatusCode.NotFound, $"Directory Not Found: {rootDirectory}");
            }
        }

        internal static Item CreateDirectoryItem(HttpListenerRequest request, string directory) {
            return CreateDirectoryItem(request, new DirectoryInfo(directory));
        }

        internal static Item CreateDirectoryItem(HttpListenerRequest request, DirectoryInfo directory) {

            return new Item {
                Name = directory.Name,
                Link = CreateUrl(
                    request,
                    RootRequestHandler.TreePath,
                    new NameValueCollection {[null] = new Uri(directory.FullName + Path.DirectorySeparatorChar).AbsoluteUri}
                ),
                Type = ItemType.DIRECTORY
            };
        }
    }
}
