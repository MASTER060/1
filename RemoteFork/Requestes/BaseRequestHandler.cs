using System;
using System.Collections.Specialized;
using System.Web;
using NLog;
using Unosquare.Net;

namespace RemoteFork.Requestes {
    internal abstract class BaseRequestHandler : IRequestHandler {
        private static readonly ILogger Log = LogManager.GetLogger("BaseRequestHandler", typeof(BaseRequestHandler));

        public virtual void Handle(HttpListenerContext context) {
            Console.WriteLine("SET DEFAULT HEADERS");
            SetDefaultReponseHeaders(context.Response);

            Handle(context.Request, context.Response);
        }

        public virtual void Handle(HttpListenerContext context, bool datatype) {
            if (!datatype) {
                SetDefaultReponseHeaders(context.Response);
            } else Console.WriteLine("NO DEFAULT HEADERS");
            Handle(context.Request, context.Response);
        }

        public virtual void Handle(HttpListenerRequest request, HttpListenerResponse response) {
        }

        protected virtual void SetDefaultReponseHeaders(HttpListenerResponse response) {
            //response.SendChunked = false;
            //response.ContentEncoding = Encoding.UTF8;

            //response.Headers.Add("Connection", "Close");
            response.ContentType = "text/html";
        }

        protected static string GetHostUrl(HttpListenerRequest request) {
            return new UriBuilder {
                Scheme = request.Url.Scheme,
                Host = request.Url.Host,
                Port = request.Url.Port,
                Path = "/",
            }.ToString();
        }

        internal static string CreateUrl(HttpListenerRequest request, string path, NameValueCollection query = null) {
            return new UriBuilder {
                Scheme = request.Url.Scheme,
                Host = request.Url.Host,
                Port = request.Url.Port,
                Path = HttpUtility.UrlPathEncode(path),
                Query = Network.HTTPUtility.QueryParametersToString(query)
            }.ToString();
        }
    }
}
