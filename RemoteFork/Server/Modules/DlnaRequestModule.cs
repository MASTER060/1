using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NLog;
using RemoteFork.Network;
using RemoteFork.Requestes;
using Unosquare.Labs.EmbedIO;
using HttpListenerContext = Unosquare.Net.HttpListenerContext;

namespace RemoteFork.Server.Modules {
    internal class DlnaRequestModule : RequestModule {
        public override string Name { get; } = "RequestDispatcher";

        public DlnaRequestModule() {
            AddHandler(RootRequestHandler.TreePath, HttpVerbs.Get, TreeRequestHandlerAsync);
            AddHandler(RootRequestHandler.RootPath, HttpVerbs.Get, FileRequestHandlerAsync);
            AddHandler(ModuleMap.AnyPath, HttpVerbs.Get, RootRequestHandlerAsync);

            Log = LogManager.GetLogger("RequestDlnaModule", typeof(RequestModule));
        }

        internal static Task<bool> RootRequestHandlerAsync(HttpListenerContext context, CancellationToken ctx) {
            if (context.Request.Url.LocalPath.StartsWith(ProxyM3u8RequestHandler.UrlPath)) {
                return GlobalRequestModule.ProxyM3u8RequestHandlerAsync(context, ctx);
            }
            return Task.FromResult(true);
        }

        internal static Task<bool> TreeRequestHandlerAsync(HttpListenerContext context, CancellationToken ctx) {
            try {
                Log.Debug($"TreeRequestHandlerAsync: {context.Request.Url.AbsolutePath}");

                AddDefaultHeader(context);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                IRequestHandler handler = null;
                if (context.Request.QueryString.GetValues(null).Any(s => s.Equals(RootRequestHandler.RootPath))) {
                    handler = new RootRequestHandler();
                } else if (context.Request.QueryString.GetValues(string.Empty)
                               ?.FirstOrDefault(s => s.StartsWith(Uri.UriSchemeFile)) != null) {
                    handler = new DlnaDirectoryRequestHandler();
                } else if (context.Request.QueryString.GetValues(string.Empty)
                               ?.FirstOrDefault(s => string.Equals(s, UserUrlsRequestHandler.ParamUrls,
                                   StringComparison.OrdinalIgnoreCase)) != null) {
                    handler = new UserUrlsRequestHandler();
                } else if (context.Request.QueryString.GetValues(string.Empty)
                               ?.FirstOrDefault(
                                   s => PluginRequestHandler.PluginParamRegex.IsMatch(s ?? string.Empty)) !=
                           null) {
                    handler = new PluginRequestHandler();
                }

                if (handler != null) {
                    handler.Handle(context);
                } else {
                    Log.Debug("Resource Not found");

                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                    HTTPUtility.WriteResponse(context.Response, HttpStatusCode.NotFound,
                        $"Resource Not found: {HttpUtility.UrlDecode(context.Request.Url.AbsolutePath)}");
                }
            } catch (Exception exception) {
                Log.Error(exception);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return Task.FromResult(true);
        }

        internal static Task<bool> FileRequestHandlerAsync(HttpListenerContext context, CancellationToken ctx) {
            try {
                Log.Debug($"FileRequestHandlerAsync: {context.Request.Url.AbsolutePath}");

                AddDefaultHeader(context);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                var handler = new DlnaFileRequestHandler();
                handler.Handle(context);
            } catch (Exception exception) {
                Log.Error(exception);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return Task.FromResult(true);
        }
    }
}
