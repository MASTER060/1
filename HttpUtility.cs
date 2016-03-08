using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RemoteFork {
    public static class HttpUtility {
        private const string DefaultUserAgent =
            "Mozilla/5.0 (Web0S; Linux/SmartTV) AppleWebKit/537.41 (KHTML, like Gecko) Large Screen WebAppManager Safari/537.41";

        public static async Task<string> GetRequest(string link, Dictionary<string, string> header = null) {
            try {
                var client = new HttpClient();
                if (header != null) {
                    foreach (var h in header) {
                        try {
                            client.DefaultRequestHeaders.Add(h.Key, h.Value);
                        } catch (Exception) {
                        }
                    }
                }
                client.DefaultRequestHeaders.UserAgent.ParseAdd(DefaultUserAgent);

                return await client.GetStringAsync(link).ConfigureAwait(false);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        public static async Task<string> PostRequest(string link, Dictionary<string, string> data,
            Dictionary<string, string> header = null) {
            try {
                var client = new HttpClient();
                if (header != null) {
                    foreach (var h in header) {
                        try {
                            client.DefaultRequestHeaders.Add(h.Key, h.Value);
                        } catch (Exception) {
                        }
                    }
                }
                client.DefaultRequestHeaders.UserAgent.ParseAdd(DefaultUserAgent);


                HttpContent content = new FormUrlEncodedContent(data);
                var result = await client.PostAsync(link, content).ConfigureAwait(false);

                return await result.Content.ReadAsStringAsync();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }
    }
}
