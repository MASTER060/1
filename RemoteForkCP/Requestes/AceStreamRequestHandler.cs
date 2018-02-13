using System;
using RemoteFork.Network;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using RemoteFork.Plugins;
using RemoteFork.Torrents;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace RemoteFork.Requestes {
    public class AceStreamRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "acestream/acestream";

        public override string Handle(HttpRequest request, HttpResponse response) {
            try {
                string url = HttpUtility.UrlDecode(request.QueryString.Value);
                if (request.Method == "POST") {
                    using (var readStream = new StreamReader(request.Body, Encoding.UTF8)) {
                        url = readStream.ReadToEnd();
                    }
                }

                if (url.StartsWith("s=B")) {
                    url = url.Substring(3);

                    string contentId = FileList.GetContentId(url);
                    return GetFileList(FileList.GetFileList(contentId, "content_id"), contentId, "content_id");
                } else if (url.StartsWith("torrenturl=")) {
                    url = url.Substring(11);
                    url = HttpUtility.UrlDecode(url);
                    if (url.StartsWith("torrent://")) {
                        url = url.Substring(10);
                    }

                    var data = HTTPUtility.GetBytesRequest(url);
                    string contentId = FileList.GetContentId(data);
                    return GetFileList(FileList.GetFileList(contentId, "content_id"), contentId, "content_id");
                } else if (url.StartsWith("magnet=")) {
                    url = url.Substring(7);

                    return GetFileList(FileList.GetFileList(url, "magnet"), url, "magnet");
                }
                return string.Empty;
            } catch (Exception exception) {
                Log.LogError(exception, exception.Message);
                return exception.Message;
            }
        }

        public string GetFileList(Dictionary<string,string> files, string key, string type) {
            var items = new List<Item>();

            if (files.Count > 0) {
                if (files.Count > 1) {
                    string stream = string.Format("{0}/ace/getstream?{1}={2}", AceStreamEngine.GetServer, type, key);
                    return HTTPUtility.GetRequest(stream);
                } else {
                    string stream = string.Format("{0}/ace/getstream?{1}={2}", AceStreamEngine.GetServer, type, key);
                    string name = Path.GetFileName(files.First().Value);
                    var item = new Item() {
                        Name = Path.GetFileName(name),
                        ImageLink = "http://obovse.ru/ForkPlayer2.5/img/file.png",
                        Link = stream,
                        Type = ItemType.FILE
                    };
                    items.Add(item);

                    stream = string.Format("{0}/ace/manifest.m3u8?{1}={2}", AceStreamEngine.GetServer, type, key);
                    item = new Item() {
                        Name = "(hls) " + Path.GetFileName(name),
                        ImageLink = "http://obovse.ru/ForkPlayer2.5/img/file.png",
                        Link = stream,
                        Type = ItemType.FILE
                    };
                    items.Add(item);
                }
            }

            var playlist = new Playlist {
                Items = items.ToArray(),
                IsIptv = "false"
            };
            return ResponseSerializer.ToXml(playlist);
        }
    }
}
