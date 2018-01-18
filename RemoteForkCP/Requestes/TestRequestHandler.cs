using System.Reflection;
using System.Web;
using Microsoft.AspNetCore.Http;
using RemoteFork.Server;

namespace RemoteFork.Requestes {
    public class TestRequestHandler : BaseRequestHandler<string> {
        public const string UrlPath = "test";

        public override string Handle(HttpRequest request, HttpResponse response) {
            if (HttpUtility.UrlDecode(request.QueryString.Value).Contains("|")) {
                string device = request.QueryString.Value.Substring(1);

                if (!Devices.Contains(device)) {
                    Devices.Add(device);
                }
            }
            
            return $"<html><h1>ForkPlayer DLNA Work!</h1><br><b>RemoteFork Server. v. {Assembly.GetExecutingAssembly().GetName().Version}</b> with Ace Stream</html>";
        }
    }
}
