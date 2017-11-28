using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Net;

namespace RemoteFork.Server {
    internal class HttpServer : IDisposable
    {
        // private static readonly ILog Log = LogManager.GetLogger(typeof(HttpServer));

        private WebServer _webServer;

        private CancellationTokenSource _cts;
        private CancellationTokenSource _cts2;
        private CancellationTokenSource _cts3;
        private CancellationTokenSource _cts4;

        private Task _task;
        private Task _task2;
        private Task _task3;
        private Task _task4;

        public HttpServer(IPAddress ip, int port)
        {
            try
            {
                //Log.Info(m => m("Server start"));
                Console.WriteLine(new UriBuilder
                {
                    Scheme = "http",
                    Host = ip.ToString(),
                    Port = port,
                    Path = "/"
                }.ToString());
                _cts = new CancellationTokenSource();
                _cts2 = new CancellationTokenSource();
                _cts3 = new CancellationTokenSource();
                _cts4 = new CancellationTokenSource();

                _webServer = WebServer.Create(
                    new UriBuilder
                    {
                        Scheme = "http",
                        Host = ip.ToString(),
                        Port = port,
                        Path = "/"
                    }.ToString()
                );
                _webServer.RegisterModule(new RequestDispatcher(_webServer));

                _task = _webServer?.RunAsync(_cts.Token);
                _task2 = _webServer?.RunAsync(_cts2.Token);
                _task3 = _webServer?.RunAsync(_cts3.Token);
                _task4 = _webServer?.RunAsync(_cts4.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        ~HttpServer()
        {
            Dispose(false);
        }

        public void Stop()
        {
            Console.WriteLine("_cts.Cancel();");
            _webServer.Dispose();
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            Stop();

            if (_webServer != null)
            {
                _webServer.Dispose();
                _webServer = null;
            }

            if (_cts != null)
            {
                _cts.Dispose();
                _cts = null;
            }

            if (_task != null)
            {
                _task?.Dispose();
                _task = null;
            }
        }

    }
}