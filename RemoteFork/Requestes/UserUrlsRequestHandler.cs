using System.Collections.Generic;
using System.Linq;
using RemoteFork.Network;
using RemoteFork.Plugins;
using RemoteFork.Properties;
using RemoteFork.Server;
using Unosquare.Labs.EmbedIO;
using HttpListenerRequest = Unosquare.Net.HttpListenerRequest;
using HttpListenerResponse = Unosquare.Net.HttpListenerResponse;

namespace RemoteFork.Requestes {
    internal class UserUrlsRequestHandler : BaseRequestHandler {
        internal static readonly string ParamUrls = "urls.m3u";

        public override void Handle(HttpListenerRequest request, HttpListenerResponse response) {
            var result = new List<Item>();

            if ((Settings.Default.UserUrls != null) && (Settings.Default.UserUrls.Count > 0)) {
                result.AddRange(from string url in Settings.Default.UserUrls
                    select new Item {
                        Name = url.Split('\\').Last().Split('/').Last(),
                        Link = url,
                        Type = ItemType.FILE
                    });
            }

            response.ContentType = Constants.DefaultMimeTypes[ParamUrls.Substring(ParamUrls.IndexOf('.'))];

            HTTPUtility.WriteResponse(response, ResponseSerializer.ToM3U(result.ToArray()));
        }
    }
}
