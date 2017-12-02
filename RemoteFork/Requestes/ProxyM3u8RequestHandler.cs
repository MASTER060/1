using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteFork.Network;
using Unosquare.Net;

namespace RemoteFork.Requestes {
    internal class ProxyM3u8RequestHandler : BaseRequestHandler {
        internal static readonly string UrlPath = "/proxym3u8";

        public override void Handle(HttpListenerRequest request, HttpListenerResponse response) {
            try {
                string url = System.Web.HttpUtility.UrlDecode(request.Url.PathAndQuery).Substring(UrlPath.Length);
                if (url.Substring(0, 1) == "B") {
                    if (url.Contains("endbase64")) {
                        url = Encoding.UTF8.GetString(Convert.FromBase64String(url.Substring(1, url.IndexOf("endbase64") - 1))) + url.Substring(url.IndexOf("endbase64") + 9);
                    } else url = Encoding.UTF8.GetString(Convert.FromBase64String(url.Substring(1, url.Length - 2)));
                }
                string ts = "";
                bool usertype = false;
                var header = new Dictionary<string, string>();
                Console.WriteLine("Proxy url: " + url);
                if (url.Contains("OPT:")) {
                    if (url.IndexOf("OPEND:/") == url.Length - 7) {
                        url = url.Replace("OPEND:/", "");
                        Console.WriteLine("Req root m3u8 " + url);
                    } else {
                        ts = url.Substring(url.IndexOf("OPEND:/") + 7);
                        Console.WriteLine("Req m3u8 ts " + ts);
                    }
                    if (url.Contains("OPEND:/")) url = url.Substring(0, url.IndexOf("OPEND:/"));
                    var headers = url.Substring(url.IndexOf("OPT:") + 4).Replace("--", "|").Split('|');
                    url = url.Substring(0, url.IndexOf("OPT:"));
                    for (int i = 0; i < headers.Length; i++) {
                        if (headers[i] == "ContentType") {
                            if (request.Headers.AllKeys.Any(k => k.Equals("Range"))) {
                                header["Range"] = request.Headers["Range"];
                            }
                            response.AddHeader("Accept-Ranges", "bytes");
                            Console.WriteLine("reproxy with ContentType");
                            usertype = true;
                            response.ContentType = headers[++i];
                            continue;
                        }
                        header[headers[i]] = headers[++i];
                        Console.WriteLine($"{headers[i - 1]}={headers[i]}");
                    }
                }
                if (!usertype) {
                    if (ts != "") {
                        url = url.Substring(0, url.LastIndexOf("/") + 1) + ts;
                        Console.WriteLine($"Full ts url {url}");
                        response.AddHeader("Content-Type", "video/mp2t");

                    } else response.AddHeader("Content-Type", "application/vnd.apple.mpegurl");
                }
                //response.Headers.Remove("Tranfer-Encoding");
                //response.Headers.Remove("Keep-Alive");
                response.AddHeader("Connection", "Close");

                // response.AddHeader("Accept-Ranges", "bytes");
                Console.WriteLine($"Real url:{url}");
                Console.WriteLine($"usertype:{usertype}");
                HTTPUtility.GetByteRequest(response, url, header, usertype);
            } catch (Exception e) {
                Console.WriteLine($"ParseRq={e}");
                HTTPUtility.WriteResponse(response, e.ToString());
            }
            //
        }
    }
}
