using System;
using RemoteFork.Network;
using Unosquare.Net;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RemoteFork.Properties;
using Newtonsoft.Json;
using RemoteFork.Plugins;
using RemoteFork.Server;

namespace RemoteFork.Requestes {
    internal class AceStream : BaseRequestHandler {
        internal static readonly string UrlPath = "/acestream";

        public override void Handle(HttpListenerRequest request, HttpListenerResponse response) {
            try {
                var url = System.Web.HttpUtility.UrlDecode(request.RawUrl)?.Substring(UrlPath.Length);
                if (request.HttpMethod == "POST") {
                    var getPostParam = new StreamReader(request.InputStream, true);
                    string postData = getPostParam.ReadToEnd();
                    //Console.WriteLine("POST ace"+ postData);
                    url = postData.Substring(2);
                }
                string s = "";
                if (url.Substring(0, 1) == "B") {
                    s = url.Substring(1);
                } else if (url.Substring(0, 1) == "U") {
                    url = url.Substring(1);
                    if (url.IndexOf("?box_mac") > 0) url = url.Substring(0, url.IndexOf("?box_mac"));
                    var header = new Dictionary<string, string>();
                    if (url.IndexOf("OPT:") > 0) {
                        var headers = url.Substring(url.IndexOf("OPT:") + 4).Replace("--", "|").Split('|');
                        for (var i = 0; i < headers.Length; i++) {
                            if (headers[i] == "ContentType") {
                                if (!string.IsNullOrEmpty(request.Headers.Get("Range"))) {
                                    header["Range"] = request.Headers.Get("Range");
                                }
                                response.Headers.Add("Accept-Ranges", "bytes");
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
                    HttpUtility.AddHeader(httpClient, header);
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;

                    s = Convert.ToBase64String(httpClient.GetAsync(url).Result.Content.ReadAsByteArrayAsync().Result);
                    // Console.WriteLine("sb64="+s);

                }
                response.Headers.AddWithoutValidate("Connection", "Close");
                Console.WriteLine($"b64={s.Length}");
                GetFileList(s, response);
                // response.AddHeader("Accept-Ranges", "bytes");


            } catch (Exception e) {
                Console.WriteLine($"ParseAce={e}");
                WriteResponse(response, e.ToString());
            }
        }

        private string PortAce = "6878";

        public struct TorrentPlayList {
            public string IDX;
            public string Name;
            public string Link;
            public string Description;
            public string ImageLink;
        }

        public class AceFile {
            public string error { get; set; }
            public Dictionary<string, string> result { get; set; }
        }

        public class AceId {
            public string error { get; set; }
            public string content_id { get; set; }
        }

        public string GetID(string fileTorrentString64, HttpListenerResponse resp) {
            var handler = new System.Net.Http.HttpClientHandler() {AllowAutoRedirect = true};
            var httpClient = new System.Net.Http.HttpClient(handler);
            HttpUtility.AddHeader(httpClient, null);
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
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
            Console.WriteLine("GetID=" + responseFromServer);
            var s = JsonConvert.DeserializeObject<AceId>(responseFromServer);
            if (s.error != null) {
                return "error get content ID: " + s.error;
            }
            return s.content_id;
        }


        public void GetFileList(string fileTorrentString64, HttpListenerResponse response) {
            var result = new List<Item>();
            string ID = GetID(fileTorrentString64, response);
            if (ID.IndexOf("error") == 0) {
                result.Add(
                    new Item {
                        Name = ID,
                        Link = "",
                        Type = ItemType.FILE,
                        ImageLink = "http://obovse.ru/ForkPlayer2.5/img/file.png"
                    }
                );
                WriteResponse(response, ResponseSerializer.ToXml(result.ToArray()));
                return;
            }
            System.Net.WebClient WC = new System.Net.WebClient();
            WC.Headers.Add("user-agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
            WC.Encoding = Encoding.UTF8;

            string aceMadiaInfo;
            string defaultIP = Settings.Default.IpIPAddress;
            try {
                aceMadiaInfo = WC.DownloadString("http://" + defaultIP + ":" + PortAce +
                                                 "/server/api?method=get_media_files&content_id=" + ID);
                WC.Dispose();
            } catch (Exception ex) {
                result.Add(
                    new Item {
                        Name = ex.Message + " " + "http://" + defaultIP + ":" + PortAce +
                               "/server/api?method=get_media_files&content_id=" + ID,
                        Link = "",
                        Description =
                            "<div id=\"poster\" style=\"float:left;padding:4px;	background-color:#EEEEEE;margin:0px 13px 1px 0px;\"><img src=\"http://static.torrentstream.org/sites/org/img/wiki-logo.png\" style=\"width:180px;float:left;\" /></div>http://" +
                            defaultIP + ":" + PortAce +
                            " /server/api ?method=get_media_files &content_id= " + ID +
                            " <br><span style=\"color:#3090F0\">Проверьте, установлен и запущен ли на компьютере Ace Stream!<br>Скачать и установить последнюю версию на компьютер можно по ссылке http://wiki.acestream.org</span>",
                        Type = ItemType.FILE,
                        ImageLink = "http://static.torrentstream.org/sites/org/img/wiki-logo.png"
                    }
                );
                WriteResponse(response, ResponseSerializer.ToXml(result.ToArray()));
                return;
            }
            Console.WriteLine(aceMadiaInfo);
            AceFile s = JsonConvert.DeserializeObject<AceFile>(aceMadiaInfo);
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
                WriteResponse(response, ResponseSerializer.ToXml(result.ToArray()));
                return;
            }
            List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>(s.result);
            myList.Sort(
                (firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value)
            );



            foreach (var h in myList) {
                Console.WriteLine($"{h.Key} set={h.Value}");
                result.Add(
                    new Item {
                        Name = h.Value,
                        Link = "http://" + Settings.Default.IpIPAddress + ":" + PortAce + "/ace/getstream?id=" + ID +
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
                        Link = "http://" + Settings.Default.IpIPAddress + ":" + PortAce + "/ace/manifest.m3u8?id=" +
                               ID + "&_idx=" + h.Key,
                        Type = ItemType.FILE,
                        ImageLink = "http://obovse.ru/ForkPlayer2.5/img/file.png"
                    }
                );
            }
            var playlist = new PluginApi.Plugins.Playlist();
            playlist.Items = result.ToArray();
            playlist.IsIptv = "false";
            WriteResponse(response, ResponseSerializer.ToXml(playlist));
            /*
            switch (FunctionsGetTorrentPlayList)
            {
                case "GetFileListJSON":
                    GetFileListJSON:
                    {

                        AceMadiaInfo = Convert.ToString(ReCoder(WC.DownloadString("http://" + IPAdress + ":" + PortAce + "/server/api?method=get_media_files&content_id=" + ID)));
                        WC.Dispose();


                        string PlayListJson = AceMadiaInfo;
                        PlayListJson = PlayListJson.Replace(",", null);
                        PlayListJson = PlayListJson.Replace(":", null);
                        PlayListJson = PlayListJson.Replace("}", null);
                        PlayListJson = PlayListJson.Replace("{", null);
                        PlayListJson = PlayListJson.Replace("result", null);
                        PlayListJson = PlayListJson.Replace("error", null);
                        PlayListJson = PlayListJson.Replace("null", null);
                        PlayListJson = PlayListJson.Replace("\"\"", "\"");
                        PlayListJson = PlayListJson.Replace("\" \"", "\"");

                        string[] ListSplit = PlayListJson.Split("\"".ToCharArray());
                        PlayListTorrent = new TorrentPlayList[((ListSplit.Length / 2) - 2) + 1];
                        int N = 0;
                        for (int I = 1; I <= ListSplit.Length - 2; I++)
                        {
                            PlayListTorrent[N].IDX = ListSplit[I];
                            PlayListTorrent[N].Name = ListSplit[I + 1];
                            PlayListTorrent[N].Link = "http://" + IPAdress + ":" + PortAce + "/ace/getstream?id=" + ID + "&_idx=" + PlayListTorrent[N].IDX;
                            PlayListTorrent[N].ImageLink = IconFile(PlayListTorrent[N].Name);
                            I += 1;
                            N += 1;
                        }


                        break;
                    }
                case "GetFileListM3U":
                    {
                        AceMadiaInfo = WC.DownloadString("http://" + IPAdress + ":" + PortAce + "/ace/manifest.m3u8?id=" + ID + "&format=json&use_api_events=1&use_stop_notifications=1");
                        //MsgBox(AceMadiaInfo)
                        if (AceMadiaInfo.StartsWith("{\"response\": {\"event_url\": \"") == true)
                        {
                            goto GetFileListJSON;
                        }
                        if (AceMadiaInfo.StartsWith("{\"response\": null, \"error\": \"") == true)
                        {
                            PlayListTorrent = new TorrentPlayList[1];
                            PlayListTorrent[0].Name = "ОШИБКА: " + (new System.Text.RegularExpressions.Regex("(?<={\"response\": null, \"error\": \").*?(?=\")")).Matches(AceMadiaInfo)[0].Value;
                            PlayListTorrent[0].ImageLink = ICO_Error;
                            return PlayListTorrent;
                        }

                        //"Получение потока в формате HLS
                        AceMadiaInfo = WC.DownloadString("http://" + IPAdress + ":" + PortAce + "/ace/manifest.m3u8?id=" + ID);

                        System.Text.RegularExpressions.Regex Regex = new System.Text.RegularExpressions.Regex("(?<=EXTINF:-1,).*(.*)");
                        System.Text.RegularExpressions.Regex RegexLink = new System.Text.RegularExpressions.Regex("(http:).*(?=.*?)");
                        System.Text.RegularExpressions.MatchCollection Itog = Regex.Matches(AceMadiaInfo);
                        System.Text.RegularExpressions.MatchCollection ItogLink = RegexLink.Matches(AceMadiaInfo);

                        PlayListTorrent = new TorrentPlayList[Itog.Count];
                        int N = 0;
                        foreach (System.Text.RegularExpressions.Match Match in Itog)
                        {
                            PlayListTorrent[N].Name = Match.Value;
                            PlayListTorrent[N].ImageLink = IconFile(Match.Value);
                            PlayListTorrent[N].Link = ItogLink[N].Value;
                            N += 1;
                        }

                        break;
                    }
            }


            return PlayListTorrent;
            */
        }

    }
}
