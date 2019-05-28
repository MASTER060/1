using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RemoteFork.Items;
using RemoteFork.Settings;
using RemoteFork.Tools;

namespace RemoteFork.Requests {
    public class UserUrlsRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "userurls";
        public const string PARAM_URLS = "urls.m3u";

        public override async Task<string> Handle(HttpRequest request, HttpResponse response) {
            var items = new List<IItem>();
            var task = Task.Run((() => {
                if ((ProgramSettings.Settings.UserUrls != null) && (ProgramSettings.Settings.UserUrls.Length > 0)) {
                    items.AddRange(from string url in ProgramSettings.Settings.UserUrls
                        select new FileItem() {
                            Title = url.Split('\\').Last().Split('/').Last(),
                            Link = url
                        });
                }
            }));
            await task;

            response.ContentType = MimeTypes.Get(PARAM_URLS.Substring(PARAM_URLS.IndexOf('.')));

            return ResponseManager.CreateResponse(items);
        }
    }
}
