using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using RemoteFork.Properties;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using NLog;
using HttpListenerResponse = Unosquare.Net.HttpListenerResponse;

namespace RemoteFork.Network {
    public static class HTTPUtility {
        private static readonly ILogger Log = LogManager.GetLogger("HttpUtility", typeof(HTTPUtility));

        private static readonly CookieContainer CookieContainer = new CookieContainer();
        private static bool _clearCookies;
        private static StreamReader stream;

        public static void GetByteRequest(HttpListenerResponse response, string link,
            Dictionary<string, string> header = null, bool wcc = false) {
            try {
                Log.Debug("HttpUtility->GetByteRequest");
                HttpWebRequest wc = null;
                if (wcc) {
                    wc = (HttpWebRequest) WebRequest.Create(link);
                    wc.Proxy = WebRequest.DefaultWebProxy;
                }
                _clearCookies = false;
                if (header != null) {
                    foreach (var h in header) {
                        try {
                            if (h.Key == "Cookie") {
                                _clearCookies = true;
                                CookieContainer.SetCookies(new Uri(link), h.Value.Replace(";", ","));
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
                        } catch (Exception ex) {
                            Log.Error(ex);
                        }
                    }
                }
                if (wcc) {
                    var response1 = (HttpWebResponse) wc.GetResponse();
                    Log.Debug(response1.Headers);
                    if (!string.IsNullOrEmpty(response1.Headers["Content-Length"]))
                        response.ContentLength64 = Convert.ToInt64(response1.Headers["Content-Length"]);

                    stream = new StreamReader(response1.GetResponseStream());
                    stream.BaseStream.CopyTo(response.OutputStream);
                } else {
                    using (var handler = new HttpClientHandler()) {
                        SetHandler(handler);
                        using (var httpClient = new HttpClient(handler)) {

                            AddHeader(httpClient, header);
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            ServicePointManager.ServerCertificateValidationCallback +=
                                (sender, cert, chain, sslPolicyErrors) => { return true; };
                            var response2 = httpClient.GetAsync(link).Result;
                            var r = response2.Content.ReadAsByteArrayAsync().Result;

                            if (r.Length > 0) {
                                response.ContentLength64 = r.Length;
                                response.OutputStream.Write(r, 0, r.Length);
                            }
                        }
                    }
                }
            } catch (Exception exception) {
                Log.Error(exception);

                WriteResponse(response, exception.ToString());
            }
        }

        internal static void WriteResponse(HttpListenerResponse response, string data) {
            Log.Debug("Response: {0}", data);

            try {
                using (var writer = new StreamWriter(response.OutputStream)) {
                    writer.Write(data);
                    writer.Flush();
                }
            } catch (Exception exception) {
                Log.Error(exception);
            }
        }

        internal static void WriteResponse(HttpListenerResponse response, HttpStatusCode status, string responseText) {
            response.StatusCode = (int) status;
            WriteResponse(response, responseText);
        }

        public static string GetRequest(string link, Dictionary<string, string> header = null, bool verbose = false,
            bool databyte = false, bool autoredirect = true) {
            try {
                _clearCookies = false;
                if (header != null) {
                    foreach (var entry in header) {
                        Log.Info(entry.ToString());
                        if (entry.Key == "Cookie") {
                            _clearCookies = true;
                            CookieContainer.SetCookies(new Uri(link), entry.Value.Replace(";", ","));
                        }
                    }
                }
                using (var handler = new HttpClientHandler() {AllowAutoRedirect = autoredirect}) {

                    SetHandler(handler);
                    using (var httpClient = new HttpClient(handler)) {
                        AddHeader(httpClient, header);
                        Console.WriteLine($"Get {link}");
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;
                        var response = httpClient.GetAsync(link).Result;

                        if (_clearCookies) {
                            var cookies = handler.CookieContainer.GetCookies(new Uri(link));
                            foreach (Cookie co in cookies) {
                                co.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));
                            }
                        }
                        Log.Info("return get headers=" + verbose);
                        //return response.ToString();
                        Console.WriteLine("HEADS");

                        if (verbose) {
                            string sh = "";
                            try {
                                var headers = response.Headers.Concat(response.Content.Headers);

                                foreach (var i in headers) {
                                    foreach (var j in i.Value) {
                                        Console.WriteLine($"{i.Key}: {j}");
                                        sh += i.Key + ": " + j + "\n";
                                    }

                                }
                            } catch (Exception exception) {
                                Log.Error(exception);
                            }
                            return string.Format("{0}\n{1}", sh, ReadContext(response.Content));
                        } else {
                            return ReadContext(response.Content, databyte);
                        }
                    }
                }
            } catch (Exception exception) {
                Console.WriteLine($"HttpUtility->GetRequest: {exception}");
                Log.Error("HttpUtility->GetRequest: {0}", exception.Message);
                return exception.Message;
            }
        }

        public static string PostRequest(string link, string data,
            Dictionary<string, string> header = null, bool verbose = false, bool autoredirect = true) {
            try {
                _clearCookies = false;
                if (header != null) {
                    foreach (var entry in header) {
                        Log.Info(entry.ToString());
                        if (entry.Key == "Cookie") {
                            _clearCookies = true;
                            CookieContainer.SetCookies(new Uri(link), entry.Value.Replace(";", ","));
                        }
                    }
                }

                using (var handler = new HttpClientHandler() {AllowAutoRedirect = autoredirect}) {
                    SetHandler(handler);
                    using (var httpClient = new HttpClient(handler)) {
                        AddHeader(httpClient, header);

                        var queryString =
                            new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;
                        var response = httpClient.PostAsync(link, queryString).Result;
                        if (_clearCookies) {
                            var cookies = handler.CookieContainer.GetCookies(new Uri(link));
                            foreach (Cookie co in cookies) {
                                co.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));
                            }
                        }
                        Log.Info("return post headers=" + verbose);
                        if (verbose) {
                            var headers = response.Headers.Concat(response.Content.Headers);
                            string sh = "";
                            foreach (var i in headers) {
                                foreach (string j in i.Value) {
                                    Console.WriteLine($"{i.Key}: {j}");
                                    sh += i.Key + ": " + j + "\n";
                                }
                            }
                            return $"{sh}\n{ReadContext(response.Content)}";
                        } else {
                            return ReadContext(response.Content);
                        }
                    }
                }
            } catch (Exception exception) {
                Log.Error(exception);
                return exception.Message;
            }
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
                        Console.WriteLine($"{h.Key} set={h.Value}");
                        //if(h.Key== "Content-Type") httpClient.DefaultRequestHeaders.Add(h.Key, h.Value);
                        // else 
                        if (!httpClient.DefaultRequestHeaders.TryAddWithoutValidation(h.Key, h.Value)) {
                            Console.WriteLine("NOT ADD");
                        }
                    } catch (Exception ex) {
                        Log.Debug("HttpUtility->AddHeader: {0}", ex.Message);
                    }
                }
            } else if (!httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(Settings.Default.UserAgent)) {
                Log.Debug("HttpUtility->AddUserAgent: {0}", Settings.Default.UserAgent);
            }
        }

        private static string ReadContext(HttpContent context, bool databyte = false) {

            try {
                return context.ReadAsStringAsync().Result;
            } catch (Exception exception1) {
                Log.Error(exception1);

                Log.Info($"charset={context.Headers.ContentType.CharSet}");
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
                                    Log.Error("HttpUtility->ReadContext: {0}", exception2.Message);
                                }
                            }
                        }
                    }
                }
                return Encoding.Default.GetString(result);
            }
        }

        public static string QueryParametersToString(NameValueCollection queries) {
            if ((queries == null) || (queries.Count == 0)) {
                return string.Empty;
            }

            string query = string.Join("&", queries.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queries[a])));

            return query;
        }
    }
}
