using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RemoteFork.Settings;

namespace RemoteFork.Network {
    public static class HTTPUtility {
        private static readonly ILogger Log = Program.LoggerFactory.CreateLogger("HTTPUtility");

        private static readonly CookieContainer CookieContainer = new CookieContainer();

        static HTTPUtility() {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
        }

        public static byte[] GetBytesRequest(string url, Dictionary<string, string> header = null, bool autoredirect = true) {
            try {
                using (var handler = new HttpClientHandler() {AllowAutoRedirect = autoredirect}) {

                    SetHandler(handler);
                    using (var httpClient = new System.Net.Http.HttpClient(handler)) {
                        AddHeader(httpClient, header);
                        Log.LogDebug($"Get {url}");

                        var response = httpClient.GetAsync(url).Result;

                        return response.Content.ReadAsByteArrayAsync().Result;
                    }
                }
            } catch (Exception exception) {
                Log.LogError(exception, "HttpUtility->GetRequest: {0}", exception.Message);
                return new byte[0];
            }
        }

        public static string GetRequest(string url, Dictionary<string, string> header = null, bool verbose = false,
            bool databyte = false, bool autoredirect = true) {
            try {
                using (var handler = new HttpClientHandler() {AllowAutoRedirect = autoredirect}) {

                    SetHandler(handler);
                    using (var httpClient = new System.Net.Http.HttpClient(handler)) {
                        AddHeader(httpClient, header);
                        Log.LogDebug($"Get {url}");

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
                    using (var httpClient = new System.Net.Http.HttpClient(handler)) {
                        AddHeader(httpClient, header);

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

        public static string PostRequest(string url, byte[] data,
            Dictionary<string, string> header = null, bool verbose = false, bool autoredirect = true) {
            try {
                using (var handler = new HttpClientHandler() {AllowAutoRedirect = autoredirect}) {
                    SetHandler(handler);
                    using (var httpClient = new System.Net.Http.HttpClient(handler)) {
                        AddHeader(httpClient, header);

                        var content = new ByteArrayContent(data);

                        var response = httpClient.PostAsync(url, content).Result;
                        return Request(handler, response, url, header, verbose);
                    }
                }
            } catch (Exception exception) {
                Log.LogError(exception, exception.Message);
                return exception.Message;
            }
        }

        private static string Request(HttpClientHandler handler, HttpResponseMessage response, string url,
            Dictionary<string, string> header, bool verbose) {
            string result;

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

        public static void AddHeader(System.Net.Http.HttpClient httpClient, Dictionary<string, string> header) {
            if (header != null) {
                foreach (var h in header) {
                    try {
                        Log.LogDebug($"{h.Key} set={h.Value}");
                        if (!httpClient.DefaultRequestHeaders.TryAddWithoutValidation(h.Key, h.Value)) {
                            Log.LogDebug("NOT ADD");
                        }
                    } catch (Exception exception) {
                        Log.LogError(exception, "HttpUtility->AddHeader: {0}", exception.Message);
                    }
                }
            } else if (!httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(ProgramSettings.Settings.UserAgent)) {
                Log.LogDebug("HttpUtility->AddUserAgent: {0}", ProgramSettings.Settings.UserAgent);
            }
        }

        private static string ReadContext(HttpContent context) {
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
    }
}
