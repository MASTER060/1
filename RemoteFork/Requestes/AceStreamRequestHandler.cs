using System;
using RemoteFork.Network;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using RemoteFork.Properties;
using Newtonsoft.Json;
using NLog;
using RemoteFork.Plugins;
using RemoteFork.Server;
using HttpListenerRequest = Unosquare.Net.HttpListenerRequest;
using HttpListenerResponse = Unosquare.Net.HttpListenerResponse;

namespace RemoteFork.Requestes {
    internal class AceStreamRequestHandler : BaseRequestHandler {
        private static readonly ILogger Log = LogManager.GetLogger("BaseRequestHandler", typeof(BaseRequestHandler));

        internal static readonly string UrlPath = "/acestream";

        public struct TorrentPlayList {
            public string IDX;
            public string Name;
            public string Link;
            public string Description;
            public string ImageLink;
        }

        public override void Handle(HttpListenerRequest request, HttpListenerResponse response) {
            try {
                string url = System.Web.HttpUtility.UrlDecode(request.Url.PathAndQuery)?.Substring(UrlPath.Length);
                if (request.HttpMethod == "POST") {
                    var getPostParam = new StreamReader(request.InputStream, true);
                    string postData = getPostParam.ReadToEnd();
                    //Console.WriteLine("POST ace"+ postData);
                    url = postData.Substring(2);
                }
                string s = "";
                if (url.StartsWith("B")) {
                    s = url.Substring(1);
                } else if (url.StartsWith("U")) {
                    url = url.Substring(1);
                    if (url.Contains("?box_mac")) {
                        url = url.Substring(0, url.IndexOf("?box_mac"));
                    }
                    var header = new Dictionary<string, string>();
                    if (url.Contains("OPT:")) {
                        var headers = url.Substring(url.IndexOf("OPT:") + 4).Replace("--", "|").Split('|');
                        for (int i = 0; i < headers.Length; i++) {
                            if (headers[i] == "ContentType") {
                                if (!string.IsNullOrEmpty(request.Headers["Range"])) {
                                    header["Range"] = request.Headers["Range"];
                                }
                                response.AddHeader("Accept-Ranges", "bytes");
                                Console.WriteLine("reproxy with ContentType");
                                response.ContentType = headers[++i];
                                continue;
                            }
                            header[headers[i]] = headers[++i];
                            Console.WriteLine(headers[i - 1] + "=" + headers[i]);
                        }
                        url = url.Substring(0, url.IndexOf("OPT:"));
                    } else header = null;

                    Console.WriteLine($"Get torrent by url:{url}");

                    var handler = new System.Net.Http.HttpClientHandler() {AllowAutoRedirect = true};
                    var httpClient = new System.Net.Http.HttpClient(handler);
                    HTTPUtility.AddHeader(httpClient, header);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;

                    s = Convert.ToBase64String(httpClient.GetAsync(url).Result.Content.ReadAsByteArrayAsync().Result);
                    // Console.WriteLine("sb64="+s);

                }
                response.AddHeader("Connection", "Close");
                Console.WriteLine($"b64={s.Length}");
                GetFileList(s, response);
                // response.AddHeader("Accept-Ranges", "bytes");


            } catch (Exception exception) {
                Log.Error(exception);
                HTTPUtility.WriteResponse(response, exception.Message);
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

        public string GetID(string fileTorrentString64, HttpListenerResponse resp) {
            var handler = new System.Net.Http.HttpClientHandler() {AllowAutoRedirect = true};
            var httpClient = new System.Net.Http.HttpClient(handler);
            HTTPUtility.AddHeader(httpClient, null);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
            var queryString =
                new System.Net.Http.StringContent(fileTorrentString64, Encoding.UTF8, "application/octet-stream");
            string responseFromServer = httpClient.PostAsync("http://api.torrentstream.net/upload/raw", queryString).Result
                .Content.ReadAsStringAsync().Result;

            /*
            var FileTorrent = System.Text.Encoding.Default.GetBytes(FileTorrentString64);
            FileTorrent = System.Text.Encoding.Unicode.GetBytes(FileTorrentString64);
                        
            Console.WriteLine(FileTorrent);
            System.Net.WebRequest request = System.Net.WebRequest.Create("http://api.torrentstream.net/upload/raw");
            request.Method = "POST";
            request.ContentType = "application/octet-stream";
            request.ContentLength = FileTorrent.Length;
            
            System.IO.Stream dataStream = request.GetRequestStream();
            dataStream.Write(FileTorrent, 0, FileTorrent.Length);
            dataStream.Close();

            System.Net.WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();*/

            Console.WriteLine($"GetID={responseFromServer}");
            var s = JsonConvert.DeserializeObject<AceId>(responseFromServer);
            if (s.error != null) {
                return "error get content ID: " + s.error;
            }
            return s.contentID;
        }


        public void GetFileList(string fileTorrentString64, HttpListenerResponse response) {
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
                HTTPUtility.WriteResponse(response, ResponseSerializer.ToXml(result.ToArray()));
                return;
            }
            var wc = new WebClient();
            wc.Headers.Add("user-agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
            wc.Encoding = Encoding.UTF8;

            string aceMadiaInfo;
            string defaultIP = Settings.Default.IpIPAddress;
            try {
                aceMadiaInfo = wc.DownloadString("http://" + defaultIP + ":" + Settings.Default.AceStreamPort +
                                                 "/server/api?method=get_media_files&content_id=" + id);
                wc.Dispose();
            } catch (Exception ex) {
                result.Add(
                    new Item {
                        Name = ex.Message + " " + "http://" + defaultIP + ":" + Settings.Default.AceStreamPort +
                               "/server/api?method=get_media_files&content_id=" + id,
                        Link = "",
                        Description =
                            "<div id=\"poster\" style=\"float:left;padding:4px;	background-color:#EEEEEE;margin:0px 13px 1px 0px;\"><img src=\"http://static.torrentstream.org/sites/org/img/wiki-logo.png\" style=\"width:180px;float:left;\" /></div>http://" +
                            defaultIP + ":" + Settings.Default.AceStreamPort +
                            " /server/api ?method=get_media_files &content_id= " + id +
                            " <br><span style=\"color:#3090F0\">Проверьте, установлен и запущен ли на компьютере Ace Stream!<br>Скачать и установить последнюю версию на компьютер можно по ссылке http://wiki.acestream.org</span>",
                        Type = ItemType.FILE,
                        ImageLink = "http://static.torrentstream.org/sites/org/img/wiki-logo.png"
                    }
                );
                HTTPUtility.WriteResponse(response, ResponseSerializer.ToXml(result.ToArray()));
                return;
            }
            Console.WriteLine(aceMadiaInfo);
            var s = JsonConvert.DeserializeObject<AceFile>(aceMadiaInfo);
            Console.WriteLine(s.error);
            if (s.result == null) {
                result.Add(
                    new Item {
                        Name = aceMadiaInfo,
                        Link = "",
                        Type = ItemType.FILE,
                        ImageLink = "http://obovse.ru/ForkPlayer2.5/img/file.png"
                    }
                );
                HTTPUtility.WriteResponse(response, ResponseSerializer.ToXml(result.ToArray()));
                return;
            }
            var myList = new List<KeyValuePair<string, string>>(s.result);
            myList.Sort(
                (firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value)
            );



            foreach (var h in myList) {
                Console.WriteLine($"{h.Key} set={h.Value}");
                result.Add(
                    new Item {
                        Name = h.Value,
                        Link = "http://" + Settings.Default.IpIPAddress + ":" + Settings.Default.AceStreamPort + "/ace/getstream?id=" + id +
                               "&_idx=" + h.Key,
                        Type = ItemType.FILE,
                        ImageLink = "http://obovse.ru/ForkPlayer2.5/img/file.png"
                    }
                );
            }

            foreach (var h in myList) {
                Console.WriteLine($"{h.Key} set={h.Value}");
                result.Add(
                    new Item {
                        Name = "(hls) " + h.Value,
                        Link = "http://" + Settings.Default.IpIPAddress + ":" + Settings.Default.AceStreamPort + "/ace/manifest.m3u8?id=" +
                               id + "&_idx=" + h.Key,
                        Type = ItemType.FILE,
                        ImageLink = "http://obovse.ru/ForkPlayer2.5/img/file.png"
                    }
                );
            }
            var playlist = new PluginApi.Plugins.Playlist {
                Items = result.ToArray(),
                IsIptv = "false"
            };
            HTTPUtility.WriteResponse(response, ResponseSerializer.ToXml(playlist));
        }
    }
}
