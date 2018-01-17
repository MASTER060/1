using System.Collections.Generic;
using RemoteFork.Plugins;

namespace RemoteFork.Network {
    public class HttpClient : IHttpClient {
        public string GetRequest(string link, Dictionary<string, string> header = null) {
            return HTTPUtility.GetRequest(link, header);
        }

        public byte[] GetBytesRequest(string link, Dictionary<string, string> header = null) {
            return HTTPUtility.GetBytesRequest(link, header);
        }

        public string PostRequest(string link, string data, Dictionary<string, string> header = null) {
            return HTTPUtility.PostRequest(link, data, header);
        }

        public string PostRequest(string link, byte[] data, Dictionary<string, string> header = null) {
            return HTTPUtility.PostRequest(link, data, header);
        }
    }
}
