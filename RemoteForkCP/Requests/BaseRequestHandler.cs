using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using RemoteFork.Log;

namespace RemoteFork.Requests {
    public abstract class BaseRequestHandler<T> {
        protected static readonly Logger Log = new Logger(typeof(BaseRequestHandler<T>));

        public virtual async Task<T> Handle(HttpContext context) {
            SetDefaultResponseHeaders(context.Response);

            return await Handle(context.Request, context.Response);
        }

        public virtual async Task<T> Handle(HttpContext context, bool datatype) {
            if (!datatype) {
                SetDefaultResponseHeaders(context.Response);
            } else {
                Log.LogDebug("NO DEFAULT HEADERS");
            }
            return await Handle(context.Request, context.Response);
        }

        public abstract Task<T> Handle(HttpRequest request, HttpResponse response);

        protected virtual void SetDefaultResponseHeaders(HttpResponse response) {
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
                Query = Tools.Tools.QueryParametersToString(query)
            }.ToString();
        }
    }
}
