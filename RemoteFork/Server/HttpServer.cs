using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;

namespace RemoteFork.Server {
    internal class HttpServer : IDisposable {
        private const byte TaskCount = 4;
        private WebServer _webServer;

        private readonly List<CancellationTokenSource> cancellation = new List<CancellationTokenSource>(TaskCount);
        private readonly List<Task> tasks = new List<Task>(TaskCount);

        public HttpServer(IPAddress ip, int port) {
            try {
                //Log.Info(m => m("Server start"));
                Console.WriteLine(new UriBuilder {
                    Scheme = "http",
                    Host = ip.ToString(),
                    Port = port,
                    Path = "/"
                }.ToString());

                for (int cIndex = 0; cIndex < TaskCount; cIndex++) {
                    cancellation.Add(new CancellationTokenSource());
                }

                _webServer = WebServer.Create(
                    new UriBuilder {
                        Scheme = "http",
                        Host = ip.ToString(),
                        Port = port,
                        Path = "/"
                    }.ToString()
                );
                _webServer.RegisterModule(new RequestDispatcher(_webServer));

                for (int tIndex = 0; tIndex < TaskCount; tIndex++) {
                    tasks.Add(_webServer?.RunAsync(cancellation[tIndex].Token));
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        ~HttpServer() {
            Dispose(false);
        }

        public void Stop() {
            Console.WriteLine("_cts.Cancel();");
            if (_webServer != null) {
                _webServer.Dispose();
            }
            /*
            _cts.Cancel();

            try
            {
                _task.Wait();
            }
            catch (AggregateException)
            {
                // We'd also actually verify the exception cause was that the task
                // was cancelled.
                _webServer.Dispose();
            }
            */
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            Stop();

            if (_webServer != null) {
                _webServer.Dispose();
                _webServer = null;
            }

            foreach (var tokenSource in cancellation) {
                if (tokenSource != null) {
                    tokenSource.Dispose();
                }
            }
            cancellation.Clear();

            foreach (var task in tasks) {
                if (task != null) {
                    task.Dispose();
                }
            }
            tasks.Clear();
        }
    }
}
