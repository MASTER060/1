using System;
using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RemoteFork.Requestes {
    public abstract class BaseRequestHandler : IRequestHandler {
        private static readonly ILogger Log = Program.LoggerFactory.CreateLogger<BaseRequestHandler>();

        public virtual string Handle(HttpContext context) {
            Log.LogDebug("SET DEFAULT HEADERS");
            SetDefaultReponseHeaders(context.Response);

            return Handle(context.Request, context.Response);
        }

        public virtual string Handle(HttpContext context, bool datatype) {
            if (!datatype) {
                SetDefaultReponseHeaders(context.Response);
            } else {
                Log.LogDebug("NO DEFAULT HEADERS");
            }
            return Handle(context.Request, context.Response);
        }

        public virtual string Handle(HttpRequest request, HttpResponse response) {
            return string.Empty;
        }

        protected virtual void SetDefaultReponseHeaders(HttpResponse response) {
            //response.SendChunked = false;
            //response.ContentEncoding = Encoding.UTF8;

            //response.Headers.Add("Connection", "Close");
            response.ContentType = "text/html";
        }

        protected static string GetHostUrl(HttpRequest request) {
            return new UriBuilder {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Port = request.Host.Port.Value,
                Path = "/",
            }.ToString();
        }

        internal static string CreateUrl(HttpRequest request, string path, NameValueCollection query = null) {
            return new UriBuilder {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Port = request.Host.Port.Value,
                Path = HttpUtility.UrlPathEncode(path),
                Query = Network.HTTPUtility.QueryParametersToString(query)
            }.ToString();
        }
    }
}
