using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using RemoteFork.Server;

namespace RemoteFork.Requests {
    public class TestRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "test";

        public override async Task<string> Handle(HttpRequest request, HttpResponse response) {
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
