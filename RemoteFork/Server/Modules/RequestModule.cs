using System.Reflection;
using NLog;
using Unosquare.Labs.EmbedIO;
using Unosquare.Net;

namespace RemoteFork.Server.Modules {
    internal abstract class RequestModule : WebModuleBase {
        protected static ILogger Log;

        protected static void AddDefaultHeader(HttpListenerContext context) {
            context.Response.AddHeader("Server", $"RemoteFork/{Assembly.GetExecutingAssembly().GetName().Version}");
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.KeepAlive = false;
        }
    }
}
