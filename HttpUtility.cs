using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace RemoteFork {
    public static class HttpUtility {
        public static async Task<string> GetRequest(Uri link) {
            try {
                var client = new HttpClient();

                return await client.GetStringAsync(link);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }
}
