using System.Reflection;
using System.Web;
using RemoteFork.Forms;
using RemoteFork.Network;
using Unosquare.Net;

namespace RemoteFork.Requestes {
    internal class TestRequestHandler : BaseRequestHandler {
        internal static readonly string UrlPath = "/test";

        public override void Handle(HttpListenerRequest request, HttpListenerResponse response) {
            //string rawUrl = HttpUtility.UrlDecode(request.Url.PathAndQuery);

            if (HttpUtility.UrlDecode(request.Url.Query).Contains("|")) {
                Properties.Settings.Default.proxy = false;
                string device = request.Url.Query.Substring(1);

                if (!Main.Devices.Contains(device)) {
                    Main.Devices.Add(device);
                }
            }

            HTTPUtility.WriteResponse(response,
                $"<html><h1>ForkPlayer DLNA Work!</h1><br><b>RemoteFork Server. v. {Assembly.GetExecutingAssembly().GetName().Version}</b> with Ace Stream</html>");
        }
    }
}
