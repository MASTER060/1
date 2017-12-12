using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RemoteFork.Network;

namespace RemoteFork.Requestes {
    internal class ProxyM3u8RequestHandler : BaseRequestHandler {
        private static readonly ILogger Log = Program.LoggerFactory.CreateLogger<ProxyM3u8RequestHandler>();

        internal static readonly string UrlPath = "/proxym3u8";

        public override string Handle(HttpRequest request, HttpResponse response) {
            try {
                string url = System.Web.HttpUtility.UrlDecode(request.QueryString.Value);
                if (url.Substring(0, 1) == "B") {
                    if (url.Contains("endbase64")) {
                        url = Encoding.UTF8.GetString(Convert.FromBase64String(url.Substring(1, url.IndexOf("endbase64") - 1))) + url.Substring(url.IndexOf("endbase64") + 9);
                    } else url = Encoding.UTF8.GetString(Convert.FromBase64String(url.Substring(1, url.Length - 2)));
                }
                string ts = "";
                bool usertype = false;
                var header = new Dictionary<string, string>();
                Log.LogDebug("Proxy url: " + url);
                if (url.Contains("OPT:")) {
                    if (url.IndexOf("OPEND:/") == url.Length - 7) {
                        url = url.Replace("OPEND:/", "");
                        Log.LogDebug("Req root m3u8 " + url);
                    } else {
                        ts = url.Substring(url.IndexOf("OPEND:/") + 7);
                        Log.LogDebug("Req m3u8 ts " + ts);
                    }
                    if (url.Contains("OPEND:/")) url = url.Substring(0, url.IndexOf("OPEND:/"));
                    var headers = url.Substring(url.IndexOf("OPT:") + 4).Replace("--", "|").Split('|');
                    url = url.Substring(0, url.IndexOf("OPT:"));
                    for (int i = 0; i < headers.Length; i++) {
                        if (headers[i] == "ContentType") {
                            if (request.Headers.ContainsKey("Range")) {
                                header["Range"] = request.Headers["Range"];
                            }
                            response.Headers.Add("Accept-Ranges", "bytes");

                            Log.LogDebug("reproxy with ContentType");

                            usertype = true;
                            response.ContentType = headers[++i];
                            continue;
                        }
                        header[headers[i]] = headers[++i];

                        Log.LogDebug($"{headers[i - 1]}={headers[i]}");
                    }
                }
                if (!usertype) {
                    if (ts != "") {
                        url = url.Substring(0, url.LastIndexOf("/") + 1) + ts;

                        Log.LogDebug($"Full ts url {url}");

                        response.Headers.Add("Content-Type", "video/mp2t");

                    } else {
                        response.Headers.Add("Content-Type", "application/vnd.apple.mpegurl");
                    }
                }
                //response.Headers.Remove("Tranfer-Encoding");
                //response.Headers.Remove("Keep-Alive");
                response.Headers.Add("Connection", "Close");

                // response.AddHeader("Accept-Ranges", "bytes");
                Log.LogDebug($"Real url:{url}");
                Log.LogDebug($"usertype:{usertype}");
                HTTPUtility.GetByteRequest(response, url, header, usertype);
            } catch (Exception exception) {
                Log.LogDebug($"ParseRq={exception}");
                return exception.ToString();
            }
            //
            return string.Empty;
        }
    }
}
