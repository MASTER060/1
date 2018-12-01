using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using RemoteFork.Network;
using RemoteFork.Plugins;
using RemoteFork.Settings;
using RemoteFork.Torrents;

namespace RemoteFork.Requestes {
    public class DlnaTorrentRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "dlna_torrent";

        public override async Task<string> Handle(HttpRequest request, HttpResponse response) {
            Log.LogDebug("HandleStream get file");

            string file = string.Empty;

            if (request.Query.ContainsKey(string.Empty)) {
                file = request.Query[string.Empty][0];
                file = HttpUtility.UrlDecode(file);
            }

            if (ProgramSettings.Settings.Dlna && !string.IsNullOrEmpty(file)) {
                try {
                    var fileRequest = await FileRequest.Create(request, file);

                    if (fileRequest.File.Exists && Tools.Tools.CheckAccessPath(fileRequest.File.FullName)) {
                        var data = await File.ReadAllBytesAsync(fileRequest.File.FullName);

                        string source = string.Empty;
                        var items = GetItems(data, ref source);

                        if (!string.IsNullOrEmpty(source)) {
                            return source;
                        } else {
                            return ResponseSerializer.ToM3U(items.ToArray());
                        }
                    } else {
                        Log.LogDebug("File not found: {0}", file);
                        response.StatusCode = (int) HttpStatusCode.NotFound;
                    }
                } catch (Exception exception) {
                    Log.LogError(exception);
                    response.StatusCode = (int) HttpStatusCode.NotFound;
                }
            } else {
                Log.LogDebug("Incorrect parameter: {0}", file);

                response.StatusCode = (int) HttpStatusCode.NoContent;
            }

            return null;
        }

        private List<Item> GetItems(byte[] data, ref string source) {
            var items = new List<Item>();
            string contentId = FileList.GetContentId(data);
            var files = FileList.GetFileList(contentId, "content_id");

            if (files.Count > 0) {
                string stream = string.Format("{0}/ace/getstream?{1}={2}", AceStreamEngine.GetServer, "content_id", contentId);
                if (files.Count > 1) {
                    source = HTTPUtility.GetRequest(stream);
                } else {
                    string name = Path.GetFileName(files.First().Value);
                    var item = new Item() {
                        Name = Path.GetFileName(name),
                        Link = stream,
                        Type = ItemType.FILE
                    };
                    items.Add(item);
                }
            }

            return items;
        }
    }
}
