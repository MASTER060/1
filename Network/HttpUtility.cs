using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace RemoteFork.Network {
    public static class HttpUtility {
        private const string DEFAULT_USER_AGENT =
            "Mozilla/5.0 (Web0S; Linux/SmartTV) AppleWebKit/537.41 (KHTML, like Gecko) Large Screen WebAppManager Safari/537.41";

        private static readonly CookieContainer cookieContainer = new CookieContainer();

        public static string GetRequest(string link, Dictionary<string, string> header = null) {
            try {
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer }) {
                    using (var httpClient = new HttpClient(handler)) {
                        if (header != null) {
                            foreach (var h in header) {
                                try {
                                    httpClient.DefaultRequestHeaders.Add(h.Key, h.Value);
                                } catch (Exception ex) {
                                    Console.WriteLine(ex);
                                }
                            }
                        }
                        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(DEFAULT_USER_AGENT);

                        var response = httpClient.GetAsync(link).Result;
                        return ReadContext(response.Content);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        public static string PostRequest(string link, Dictionary<string, string> data,
            Dictionary<string, string> header = null) {
            try {
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer }) {
                    using (var httpClient = new HttpClient(handler)) {
                        if (header != null) {
                            foreach (var h in header) {
                                try {
                                    httpClient.DefaultRequestHeaders.Add(h.Key, h.Value);
                                } catch (Exception ex) {
                                    Console.WriteLine(ex);
                                }
                            }
                        }
                        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(DEFAULT_USER_AGENT);

                        HttpContent content = new FormUrlEncodedContent(data);

                        var response = httpClient.PostAsync(link, content).Result;
                        return ReadContext(response.Content);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        private static string ReadContext(HttpContent context) {
            var result = context.ReadAsByteArrayAsync().Result;
            try {
                var encoding = Encoding.GetEncoding(context.Headers.ContentType.CharSet);
                result = Encoding.Convert(encoding, Encoding.Default,
                result);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return Encoding.Default.GetString(result);
        }
    }
}
