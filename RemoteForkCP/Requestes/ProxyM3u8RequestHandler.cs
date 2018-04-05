using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using RemoteFork.Network;

namespace RemoteFork.Requestes {
    public class ProxyM3u8RequestHandler : BaseRequestHandler<byte[]> {
        public const string UrlPath = "proxym3u8";

        public override byte[] Handle(HttpRequest request, HttpResponse response) {
            try {
                string url = HttpUtility.UrlDecode(request.Path.Value.Replace("/" + UrlPath, ""));
                if (url.StartsWith("B")) {
                    if (url.Contains("endbase64")) {
                        url = Encoding.UTF8.GetString(
                                  Convert.FromBase64String(url.Substring(1, url.IndexOf("endbase64") - 1))) +
                              url.Substring(url.IndexOf("endbase64") + 9);
                    } else {
                        url = Encoding.UTF8.GetString(Convert.FromBase64String(url.Substring(1, url.Length - 2)));
                    }
                }
                string ts = string.Empty;
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
                    if (url.Contains("OPEND:/")) {
                        url = url.Substring(0, url.IndexOf("OPEND:/"));
                    }
                    var headers = url.Substring(url.IndexOf("OPT:") + 4).Replace("--", "|").Split('|');
                    url = url.Remove(url.IndexOf("OPT:"));
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
                    if (!string.IsNullOrEmpty(ts)) {
                        url = url.Remove(url.LastIndexOf("/") + 1) + ts;

                        Log.LogDebug($"Full ts url {url}");

                        response.ContentType = "video/mp2t";

                    } else {
                        response.ContentType = "application/vnd.apple.mpegurl";
                    }
                }
                //response.Headers.Remove("Tranfer-Encoding");
                //response.Headers.Remove("Keep-Alive");
                response.Headers.Add("Connection", "Close");

                // response.AddHeader("Accept-Ranges", "bytes");
                Log.LogDebug($"Real url:{url}");
                Log.LogDebug($"usertype:{usertype}");
                var data = HTTPUtility.GetBytesRequest(url, header, usertype);
                return data;
            } catch (Exception exception) {
                Log.LogError(exception);
                return Encoding.UTF8.GetBytes(exception.ToString());
            }
        }
    }
}
