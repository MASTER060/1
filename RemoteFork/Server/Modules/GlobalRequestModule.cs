using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RemoteFork.Requestes;
using Unosquare.Labs.EmbedIO;
using HttpListenerContext = Unosquare.Net.HttpListenerContext;

namespace RemoteFork.Server.Modules {
    internal class GlobalRequestModule : RequestModule {
        public override string Name { get; } = "RequestDispatcher";

        public GlobalRequestModule() {
            Log = LogManager.GetLogger("RequestDispatcher", typeof(RequestModule));

            AddHandler(TestRequestHandler.UrlPath, HttpVerbs.Get, TestRequestHandlerAsync);
            AddHandler(ParseLinkRequestHandler.UrlPath, HttpVerbs.Get, ParseLinkRequestHandlerAsync);
            AddHandler(AceStreamRequestHandler.UrlPath, HttpVerbs.Get, AceStreamRequestHandlerAsync);
            AddHandler(ProxyM3u8RequestHandler.UrlPath, HttpVerbs.Get, ProxyM3u8RequestHandlerAsync);
        }

        internal static Task<bool> TestRequestHandlerAsync(HttpListenerContext context, CancellationToken ctx) {
            try {
                Log.Debug($"TestRequestHandlerAsync: {context.Request.Url.AbsolutePath}");

                AddDefaultHeader(context);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                var handler = new TestRequestHandler();
                handler.Handle(context);
            } catch (Exception exception) {
                Log.Error(exception);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return Task.FromResult(true);
        }

        internal static Task<bool> ParseLinkRequestHandlerAsync(HttpListenerContext context, CancellationToken ctx) {
            try {
                Log.Debug($"ParseLinkRequestHandlerAsync: {context.Request.Url.AbsolutePath}");

                AddDefaultHeader(context);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                var handler = new ParseLinkRequestHandler();
                handler.Handle(context);
            } catch (Exception exception) {
                Log.Error(exception);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return Task.FromResult(true);
        }

        internal static Task<bool> ProxyM3u8RequestHandlerAsync(HttpListenerContext context, CancellationToken ctx) {
            Console.WriteLine("RequestDispatcher");
            try {
                Log.Debug($"ProxyM3u8RequestHandlerAsync: {context.Request.Url.AbsolutePath}");

                AddDefaultHeader(context);

                context.Response.StatusCode = (int) HttpStatusCode.OK;
                var handler = new ProxyM3u8RequestHandler();
                handler.Handle(context, true);
            } catch (Exception exception) {
                Log.Error(exception);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return Task.FromResult(true);
        }

        internal static Task<bool> AceStreamRequestHandlerAsync(HttpListenerContext context, CancellationToken ctx) {
            try {
                Log.Debug($"AceStreamRequestHandlerAsync: {context.Request.Url.AbsolutePath}");

                AddDefaultHeader(context);

                context.Response.StatusCode = (int) HttpStatusCode.OK;
                var handler = new AceStreamRequestHandler();
                handler.Handle(context, true);
            } catch (Exception exception) {
                Log.Error(exception);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return Task.FromResult(true);
        }
    }
}
