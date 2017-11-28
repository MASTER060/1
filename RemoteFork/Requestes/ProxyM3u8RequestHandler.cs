using System;
using System.Collections.Generic;
using System.Text;
using RemoteFork.Network;
using Unosquare.Net;

namespace RemoteFork.Requestes {
    internal class ProxyM3u8RequestHandler : BaseRequestHandler {
        internal static readonly string UrlPath = "/proxym3u8";

        public override void Handle(HttpListenerRequest request, HttpListenerResponse response) {
            try {
                string url = System.Web.HttpUtility.UrlDecode(request.RawUrl)?.Substring(UrlPath.Length);
                if (url.Substring(0, 1) == "B") {
                    if (url.IndexOf("endbase64") > 0) {
                        url = Encoding.UTF8.GetString(Convert.FromBase64String(url.Substring(1, url.IndexOf("endbase64") - 1))) + url.Substring(url.IndexOf("endbase64") + 9);
                    } else url = Encoding.UTF8.GetString(Convert.FromBase64String(url.Substring(1, url.Length - 2)));
                }
                string ts = "";
                bool usertype = false;
                var header = new Dictionary<string, string>();
                Console.WriteLine("Proxy url: " + url);
                if (url.IndexOf("OPT:") > 0) {
                    if (url.IndexOf("OPEND:/") == url.Length - 7) {
                        url = url.Replace("OPEND:/", "");
                        Console.WriteLine("Req root m3u8 " + url);
                    } else {
                        ts = url.Substring(url.IndexOf("OPEND:/") + 7);
                        Console.WriteLine("Req m3u8 ts " + ts);
                    }
                    if (url.IndexOf("OPEND:/") > 0) url = url.Substring(0, url.IndexOf("OPEND:/"));
                    var headers = url.Substring(url.IndexOf("OPT:") + 4).Replace("--", "|").Split('|');
                    url = url.Substring(0, url.IndexOf("OPT:"));
                    for (var i = 0; i < headers.Length; i++) {
                        if (headers[i] == "ContentType") {
                            if (!string.IsNullOrEmpty(request.Headers.Get("Range"))) {
                                header["Range"] = request.Headers.Get("Range");
                            }
                            response.Headers.Add("Accept-Ranges", "bytes");
                            Console.WriteLine("reproxy with ContentType");
                            usertype = true;
                            response.ContentType = headers[++i];
                            continue;
                        }
                        header[headers[i]] = headers[++i];
                        Console.WriteLine(headers[i - 1] + "=" + headers[i]);
                    }
                }
                if (!usertype) {
                    if (ts != "") {
                        url = url.Substring(0, url.LastIndexOf("/") + 1) + ts;
                        Console.WriteLine($"Full ts url {url}");
                        response.Headers.AddWithoutValidate("Content-Type", "video/mp2t");

                    } else response.Headers.AddWithoutValidate("Content-Type", "application/vnd.apple.mpegurl");
                }
                response.Headers.Remove("Tranfer-Encoding");
                response.Headers.Remove("Keep-Alive");
                response.Headers.AddWithoutValidate("Connection", "Close");


                // response.AddHeader("Accept-Ranges", "bytes");
                Console.WriteLine($"Real url:{url}");
                Console.WriteLine($"usertype:{usertype}");
                HttpUtility.GetByteRequest(response, url, header, usertype);
            } catch (Exception e) {
                Console.WriteLine($"ParseRq={e}");
                WriteResponse(response, e.ToString());
            }
            //
        }
    }
}
