using System.Collections.Generic;

namespace PluginApi.Plugins
{
    public interface IHttpClient
    {
        string GetRequest(string link, Dictionary<string, string> header = null);

        string PostRequest(string link, string data, Dictionary<string, string> header = null);
    }
}
