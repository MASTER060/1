using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RemoteFork.Network {
    public static class HTTPUtility {
        private static readonly ILogger Log = Program.LoggerFactory.CreateLogger("HTTPUtility");

        private static readonly CookieContainer CookieContainer = new CookieContainer();

        public static void GetByteRequest(HttpResponse response, string url,
            Dictionary<string, string> header = null, bool wcc = false) {
            try {
                Log.LogDebug("HttpUtility->GetByteRequest");
                HttpWebRequest wc = null;
                if (wcc) {
                    wc = (HttpWebRequest)WebRequest.Create(url);
                    wc.Proxy = WebRequest.DefaultWebProxy;
                }
                if (header != null) {
                    foreach (var h in header) {
                        try {
                            if (h.Key == "Cookie") {
                                CookieContainer.SetCookies(new Uri(url), h.Value.Replace(";", ","));
                            }
                            if (wcc) {
                                if (h.Key == "Range") {
                                    var x = h.Value.Split('=')[1].Split('-');
                                    if (x.Length == 1) wc.AddRange(Convert.ToInt64(x[0]));
                                    else if (x.Length == 2) {
                                        if (string.IsNullOrEmpty(x[1])) {
                                            if (Convert.ToInt64(x[0]) > 0) wc.AddRange(Convert.ToInt64(x[0]));
                                        } else wc.AddRange(Convert.ToInt64(x[0]), Convert.ToInt64(x[1]));
                                    }
                                } else wc.Headers.Add(h.Key, h.Value);
                            }
                        } catch (Exception exception) {
                            Log.LogError(exception, exception.Message);
                        }
                    }
                }
                if (wcc) {
                    var response1 = (HttpWebResponse)wc.GetResponse();
                    Log.LogDebug(response1.Headers.ToString());
                    if (!string.IsNullOrEmpty(response1.Headers["Content-Length"])) {
                        response.ContentLength = Convert.ToInt64(response1.Headers["Content-Length"]);
                    }

                    var stream = new StreamReader(response1.GetResponseStream());
                    stream.BaseStream.CopyTo(response.Body);
                } else {
                    using (var handler = new HttpClientHandler()) {
                        SetHandler(handler);
                        using (var httpClient = new HttpClient(handler)) {

                            AddHeader(httpClient, header);
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            ServicePointManager.ServerCertificateValidationCallback +=
                                (sender, cert, chain, sslPolicyErrors) => true;
                            var response2 = httpClient.GetAsync(url).Result;
                            var r = response2.Content.ReadAsByteArrayAsync().Result;

                            if (r.Length > 0) {
                                response.ContentLength = r.Length;
                                response.Body.Write(r, 0, r.Length);
                            }
                        }
                    }
                }
            } catch (Exception exception) {
                Log.LogError(exception, exception.Message);
            }
        }

        public static string GetRequest(string url, Dictionary<string, string> header = null, bool verbose = false,
            bool databyte = false, bool autoredirect = true) {
            try {
                using (var handler = new HttpClientHandler() { AllowAutoRedirect = autoredirect }) {

                    SetHandler(handler);
                    using (var httpClient = new HttpClient(handler)) {
                        AddHeader(httpClient, header);
                        Log.LogDebug($"Get {url}");

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;

                        var response = httpClient.GetAsync(url).Result;

                        return Request(handler, response, url, header, verbose);
                    }
                }
            } catch (Exception exception) {
                Log.LogError(exception, "HttpUtility->GetRequest: {0}", exception.Message);
                return exception.Message;
            }
        }

        public static string PostRequest(string url, string data,
            Dictionary<string, string> header = null, bool verbose = false, bool autoredirect = true) {
            try {
                using (var handler = new HttpClientHandler() {AllowAutoRedirect = autoredirect}) {
                    SetHandler(handler);
                    using (var httpClient = new HttpClient(handler)) {
                        AddHeader(httpClient, header);

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;

                        var queryString =
                            new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                        var response = httpClient.PostAsync(url, queryString).Result;
                        return Request(handler, response, url, header, verbose);
                    }
                }
            } catch (Exception exception) {
                Log.LogError(exception, exception.Message);
                return exception.Message;
            }
        }

        private static string Request(HttpClientHandler handler, HttpResponseMessage response, string url, Dictionary<string, string> header, bool verbose) {
            string result = string.Empty;

            if (CheckHeader(url, header)) {
                var cookies = handler.CookieContainer.GetCookies(new Uri(url));
                foreach (Cookie co in cookies) {
                    co.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));
                }
            }
            Log.LogInformation("return post headers=" + verbose);
            if (verbose) {
                var headers = response.Headers.Concat(response.Content.Headers);
                string sh = "";
                foreach (var i in headers) {
                    foreach (string j in i.Value) {
                        sh += i.Key + ": " + j + Environment.NewLine;
                    }
                }
                result = $"{sh}{Environment.NewLine}{ReadContext(response.Content)}";
            } else {
                result = ReadContext(response.Content);
            }


            return result;
        }

        private static void SetHandler(HttpClientHandler handler) {
            handler.Proxy = WebRequest.DefaultWebProxy;
            handler.CookieContainer = CookieContainer;
            handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        }

        public static void AddHeader(HttpClient httpClient, Dictionary<string, string> header) {

            if (header != null) {
                foreach (var h in header) {
                    try {
                        Log.LogDebug($"{h.Key} set={h.Value}");
                        //if(h.Key== "Content-Type") httpClient.DefaultRequestHeaders.Add(h.Key, h.Value);
                        // else 
                        if (!httpClient.DefaultRequestHeaders.TryAddWithoutValidation(h.Key, h.Value)) {
                            Log.LogDebug("NOT ADD");
                        }
                    } catch (Exception exception) {
                        Log.LogError(exception, "HttpUtility->AddHeader: {0}", exception.Message);
                    }
                }
            } else if (!httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(SettingsManager.Settings.UserAgent)) {
                Log.LogDebug("HttpUtility->AddUserAgent: {0}", SettingsManager.Settings.UserAgent);
            }
        }

        private static string ReadContext(HttpContent context, bool databyte = false) {

            try {
                return context.ReadAsStringAsync().Result;
            } catch (Exception exception1) {
                Log.LogError(exception1, exception1.Message);

                Log.LogInformation($"charset={context.Headers.ContentType.CharSet}");
                var result = context.ReadAsByteArrayAsync().Result;
                try {
                    var encoding = Encoding.Default;
                    result = Encoding.Convert(encoding, Encoding.Default, result);
                } catch {
                    try {
                        var encoding = Encoding.GetEncoding(context.Headers.ContentType.CharSet);
                        result = Encoding.Convert(encoding, Encoding.Default, result);
                    } catch {
                        try {
                            var encoding = Encoding.UTF8;
                            result = Encoding.Convert(encoding, Encoding.Default, result);
                        } catch {
                            try {
                                var encoding = Encoding.ASCII;
                                result = Encoding.Convert(encoding, Encoding.Default, result);
                            } catch {
                                try {
                                    var encoding = Encoding.Unicode;
                                    result = Encoding.Convert(encoding, Encoding.Default, result);
                                } catch (Exception exception2) {
                                    Log.LogError(exception2, "HttpUtility->ReadContext: {0}", exception2.Message);
                                }
                            }
                        }
                    }
                }
                return Encoding.Default.GetString(result);
            }
        }

        private static bool CheckHeader(string url, Dictionary<string, string> header = null) {
            bool result = false;
            if (header != null) {
                foreach (var entry in header) {
                    Log.LogInformation(entry.ToString());
                    if (entry.Key == "Cookie") {
                        result = true;
                        CookieContainer.SetCookies(new Uri(url), entry.Value.Replace(";", ","));
                    }
                }
            }
            return result;
        }

        public static string QueryParametersToString(NameValueCollection queries) {
            if ((queries == null) || (queries.Count == 0)) {
                return string.Empty;
            }

            string query = string.Join("&", queries.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queries[a])));

            return query;
        }

        public static NameValueCollection ConvertToNameValue(this IQueryCollection queries) {
            var result = new NameValueCollection();

            if ((queries != null) && (queries.Count != 0)) {
                foreach (var query in queries) {
                    result.Add(query.Key, query.Value);
                }
            }

            return result;
        }
    }
}