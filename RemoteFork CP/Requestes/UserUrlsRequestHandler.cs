using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using RemoteFork.Plugins;
using RemoteFork.Server;

namespace RemoteFork.Requestes {
    public class UserUrlsRequestHandler : BaseRequestHandler {
        public const string UrlPath = "userurls";
        public const string ParamUrls = "urls.m3u";

        public override string Handle(HttpRequest request, HttpResponse response) {
            var result = new List<Item>();

            if ((SettingsManager.Settings.UserUrls != null) && (SettingsManager.Settings.UserUrls.Length > 0)) {
                result.AddRange(from string url in SettingsManager.Settings.UserUrls
                    select new Item {
                        Name = url.Split('\\').Last().Split('/').Last(),
                        Link = url,
                        Type = ItemType.FILE
                    });
            }

            response.ContentType = MimeTypes.Get(ParamUrls.Substring(ParamUrls.IndexOf('.')));

            return ResponseSerializer.ToM3U(result.ToArray());
        }
    }
}
