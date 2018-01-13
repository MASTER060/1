using System;
using RemoteFork.Network;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemoteFork.Plugins;
using RemoteFork.Server;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace RemoteFork.Requestes {
    public class AceStreamRequestHandler : BaseRequestHandler {
        private static readonly ILogger Log = Program.LoggerFactory.CreateLogger<AceStreamRequestHandler>();

        public const string UrlPath = "acestream";

        //public struct TorrentPlayList {
        //    public string IDX;
        //    public string Name;
        //    public string Link;
        //    public string Description;
        //    public string ImageLink;
        //}

        public override string Handle(HttpRequest request, HttpResponse response) {
            try {
                string url = System.Web.HttpUtility.UrlDecode(request.QueryString.Value);
                if (request.Method == "POST") {
                    if (request.Form.ContainsKey("s")) {
                        url = request.Form["s"];
                    }
                }
                string result = "";
                if (url.StartsWith("B") || url.StartsWith("M")) {
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

                    //var httpClient = new RestClient(url);
                    //var httpRequest = new RestRequest(Method.GET);
                    //var httpClient = new System.Net.Http.HttpClient(handler);
                    HTTPUtility.GetRequest(url, header);
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    //ServicePointManager.ServerCertificateValidationCallback +=
                    //    (sender, cert, chain, sslPolicyErrors) => true;

                    result = HTTPUtility.GetRequest(url);
                }
                response.Headers.Add("Connection", "Close");

                return GetFileList(result, response);


            } catch (Exception exception) {
                Log.LogError(exception, exception.Message);
                return exception.Message;
            }
        }

        public class AceFile {
            public string error { get; set; }
            public Dictionary<string, string> result { get; set; }
        }

        public class AceId {
            public string error { get; set; }
            public string contentID { get; set; }
        }

        public string GetID(string fileTorrentString64, HttpResponse resp) {
            //var httpClient = new RestClient("http://api.torrentstream.net/upload/raw");
            //var request = new RestRequest(fileTorrentString64, Method.POST);
            
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //ServicePointManager.ServerCertificateValidationCallback +=
            //    (sender, cert, chain, sslPolicyErrors) => true;
            string responseFromServer = HTTPUtility.PostRequest("http://api.torrentstream.net/upload/raw", fileTorrentString64);

            var s = JsonConvert.DeserializeObject<AceId>(responseFromServer);
            if (s.error != null) {
                return "error get content ID: " + s.error;
            }
            return s.contentID;
        }


        public string GetFileList(string fileTorrentString64, HttpResponse response) {
            var result = new List<Item>();
            string id = GetID(fileTorrentString64, response);
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
            var wc = new WebClient();
            wc.Headers.Add("user-agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
            wc.Encoding = Encoding.UTF8;

            string aceMadiaInfo;
            string ip = SettingsManager.Settings.IpAddress.ToString();
            string port = SettingsManager.Settings.AceStreamPort.ToString();
            try {
                aceMadiaInfo = wc.DownloadString("http://" + ip + ":" + port +
                                                 "/server/api?method=get_media_files&content_id=" + id);
                wc.Dispose();
            } catch (Exception ex) {
                result.Add(
                    new Item {
                        Name = ex.Message + " " + "http://" + ip + ":" + port +
                               "/server/api?method=get_media_files&content_id=" + id,
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
            if (s.result == null) {
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
            var myList = new List<KeyValuePair<string, string>>(s.result);
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
    }
}
