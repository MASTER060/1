using System.Reflection;
using System.Web;
using Microsoft.AspNetCore.Http;
using RemoteFork.Server;

namespace RemoteFork.Requestes {
    internal class TestRequestHandler : BaseRequestHandler {
        internal static readonly string UrlPath = "/test";

        public override string Handle(HttpRequest request, HttpResponse response) {
            //string rawUrl = HttpUtility.UrlDecode(request.Url.PathAndQuery);

            if (HttpUtility.UrlDecode(request.QueryString.Value).Contains("|")) {
                //Settings.UseProxy = false;
                string device = request.QueryString.Value.Substring(1);

                if (!Devices.Contains(device)) {
                    Devices.Add(device);
                }
            }
            
            return $"<html><h1>ForkPlayer DLNA Work!</h1><br><b>RemoteFork Server. v. {Assembly.GetExecutingAssembly().GetName().Version}</b> with Ace Stream</html>";
        }
    }
}
