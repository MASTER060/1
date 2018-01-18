using System;
using RemoteFork.Network;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RemoteFork.Plugins;
using RemoteFork.Settings;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace RemoteFork.Requestes {
    public class AceStreamRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "acestream";

        public override string Handle(HttpRequest request, HttpResponse response) {
            try {
                string url = HttpUtility.UrlDecode(request.QueryString.Value);
                if (request.Method == "POST") {
                    if (request.Form.ContainsKey("s")) {
                        url = request.Form["s"];
                    }
                }
                string result = "";
                if (url.StartsWith("B")) {
                    result = url.Substring(1);
                } else if (url.StartsWith("U")) {
                    url = url.Substring(1);
                    if (url.Contains("?box_mac")) {
                        url = url.Remove(url.IndexOf("?box_mac"));
                    }
                    var header = new Dictionary<string, string>();
                    if (url.Contains("OPT:")) {
                        var headers = url.Substring(url.IndexOf("OPT:") + 4).Replace("--", "|").Split('|');
                        for (int i = 0; i < headers.Length; i++) {
                            if (headers[i] == "ContentType") {
                                if (!string.IsNullOrEmpty(request.Headers["Range"])) {
                                    header["Range"] = request.Headers["Range"];
                                }
                                response.Headers.Add("Accept-Ranges", "bytes");
                                response.ContentType = headers[++i];
                                continue;
                            }
                            header[headers[i]] = headers[++i];
                        }
                        url = url.Substring(0, url.IndexOf("OPT:"));
                    } else {
                        header = null;
                    }

                    HTTPUtility.GetRequest(url, header);

                    result = HTTPUtility.GetRequest(url);
                }
                response.Headers.Add("Connection", "Close");

                return GetFileList(result);
            } catch (Exception exception) {
                Log.LogError(exception, exception.Message);
                return exception.Message;
            }
        }

        public string GetID(string fileTorrentString64) {
            string responseFromServer = HTTPUtility.PostRequest("http://api.torrentstream.net/upload/raw", fileTorrentString64);

            var s = JsonConvert.DeserializeObject<AceId>(responseFromServer);
            if (s.Error != null) {
                return "error get content ID: " + s.Error;
            }
            return s.ContentID;
        }

        public string GetFileList(string fileTorrentString64) {
            var result = new List<Item>();
            string id = GetID(fileTorrentString64);
            if (id.StartsWith("error")) {
                result.Add(
                    new Item {
                        Name = id,
                        Link = "",
                        Type = ItemType.FILE,
                        ImageLink = "http://obovse.ru/ForkPlayer2.5/img/file.png"
                    }
                );
                return ResponseSerializer.ToXml(result.ToArray());
            }

            string aceMadiaInfo;
            string ip = ProgramSettings.Settings.IpAddress;
            string port = ProgramSettings.Settings.AceStreamPort.ToString();
            string url = string.Concat("http://", ip, ":", port, "/server/api?method=get_media_files&content_id=", id);
            try {
                aceMadiaInfo = HTTPUtility.GetRequest(url);
            } catch (Exception exception) {
                result.Add(
                    new Item {
                        Name = exception.Message + " " + url,
                        Link = "",
                        Description =
                            "<div id=\"poster\" style=\"float:left;padding:4px;	background-color:#EEEEEE;margin:0px 13px 1px 0px;\"><img src=\"http://static.torrentstream.org/sites/org/img/wiki-logo.png\" style=\"width:180px;float:left;\" /></div>http://" +
                            ip + ":" + port +
                            " /server/api ?method=get_media_files &content_id= " + id +
                            " <br><span style=\"color:#3090F0\">Проверьте, установлен и запущен ли на компьютере Ace Stream!<br>Скачать и установить последнюю версию на компьютер можно по ссылке http://wiki.acestream.org</span>",
                        Type = ItemType.FILE,
                        ImageLink = "http://static.torrentstream.org/sites/org/img/wiki-logo.png"
                    }
                );
                return ResponseSerializer.ToXml(result.ToArray());
            }
            var s = JsonConvert.DeserializeObject<AceFile>(aceMadiaInfo);
            if (s.Result == null) {
                result.Add(
                    new Item {
                        Name = aceMadiaInfo,
                        Link = "",
                        Type = ItemType.FILE,
                        ImageLink = "http://obovse.ru/ForkPlayer2.5/img/file.png"
                    }
                );
                return ResponseSerializer.ToXml(result.ToArray());
            }
            var myList = new List<KeyValuePair<string, string>>(s.Result);
            myList.Sort(
                (firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value)
            );

            result.AddRange(myList.Select(h => new Item {
                Name = h.Value,
                Link = "http://" + ip + ":" + port + "/ace/getstream?id=" + id + "&_idx=" + h.Key,
                Type = ItemType.FILE,
                ImageLink = "http://obovse.ru/ForkPlayer2.5/img/file.png"
            }));

            result.AddRange(myList.Select(h => new Item {
                Name = "(hls) " + h.Value,
                Link = "http://" + ip + ":" + port + "/ace/manifest.m3u8?id=" + id + "&_idx=" + h.Key,
                Type = ItemType.FILE,
                ImageLink = "http://obovse.ru/ForkPlayer2.5/img/file.png"
            }));
            var playlist = new Playlist {
                Items = result.ToArray(),
                IsIptv = "false"
            };
            return ResponseSerializer.ToXml(playlist);
        }

        [Serializable]
        public class AceFile {
            public string Error { get; set; }
            public Dictionary<string, string> Result { get; set; }
        }

        [Serializable]
        public class AceId {
            public string Error { get; set; }
            public string ContentID { get; set; }
        }
    }
}
