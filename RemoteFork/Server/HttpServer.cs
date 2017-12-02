using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RemoteFork.Server.Modules;
using Unosquare.Labs.EmbedIO;

namespace RemoteFork.Server {
    internal class HttpServer : IDisposable {
        private static readonly ILogger Log = LogManager.GetLogger("HttpServer", typeof(HttpServer));

        private const byte TaskCount = 4;
        private readonly WebServer _webServer;

        private readonly List<CancellationTokenSource> cancellation = new List<CancellationTokenSource>(TaskCount);
        private readonly List<Task> tasks = new List<Task>(TaskCount);

        public HttpServer(string ip, int port) {
            try {
                Log.Debug(new UriBuilder {
                    Scheme = "http",
                    Host = ip,
                    Port = port,
                    Path = "/"
                }.ToString());

                for (int cIndex = 0; cIndex < TaskCount; cIndex++) {
                    cancellation.Add(new CancellationTokenSource());
                }

                _webServer = WebServer.Create(
                    new UriBuilder {
                        Scheme = "http",
                        Host = ip,
                        Port = port,
                        Path = "/"
                    }.ToString()
                );

                _webServer.RegisterModule(new GlobalRequestModule());
                _webServer.RegisterModule(new DlnaRequestModule());

                for (int tIndex = 0; tIndex < TaskCount; tIndex++) {
                    tasks.Add(_webServer?.RunAsync(cancellation[tIndex].Token));
                }
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }

        public void Start() {
            if (_webServer != null) {
            }
        }

        public void Stop() {
            try {
                _webServer.Dispose();
            } catch (Exception exception) {
                Log.Debug($"_webServer.Dispose: {exception.Message}");
            }
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                _webServer?.Dispose();
            }
        }

        ~HttpServer() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
