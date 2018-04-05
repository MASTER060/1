using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using RemoteFork.Log;
using RemoteFork.Network;
using RemoteFork.Server;
using RemoteFork.Settings;

namespace RemoteFork {
    public class Program {
        private static readonly Logger Log = new Logger(typeof(Program));
        private static Server server;

        public static void Main(string[] args) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            server = new Server(ProgramSettings.Settings.IpAddress, ProgramSettings.Settings.Port);

            Task.Run(server.Start);

            while (Console.ReadKey().Key != ConsoleKey.Escape) {
            }
            server.Stop();
        }

        public static void Restart() {
            if (server != null) {
                try {
                    server.Restart();
                } catch (Exception exception) {
                    Log.LogError(exception);
                }
            }
        }

        internal class Server {
            private static readonly Logger Log = new Logger(typeof(Server));
            
            private IWebHost _webHost;

            private readonly string _ip;
            private readonly ushort _port;

            internal Server(string ip, ushort port) {
                this._ip = ip;
                this._port = port;
            }

            internal async Task Start() {
                var builder = WebHost.CreateDefaultBuilder()
                    //.UseEnvironment("Development")
                    .UseStartup<Startup>()
                    .UseKestrel()
#if DEBUG
                    .UseContentRoot(Environment.CurrentDirectory);
#else
                .UseContentRoot(AppDomain.CurrentDomain.BaseDirectory);
#endif
                
                if (ProgramSettings.Settings.UseProxy) {
                    HTTPUtility.CreateProxy(ProgramSettings.Settings.ProxyAddress,
                        ProgramSettings.Settings.ProxyUserName, ProgramSettings.Settings.ProxyPassword);
                } else {
                    HTTPUtility.CreateProxy();
                }

                ServerRegistration();
                try {
                    var listen = new StringBuilder();
                    listen.Append($"http://{_ip}:{_port}");
                    if (ProgramSettings.Settings.ListenLocalhost) {
                        if (_ip != "0.0.0.0" && _ip != "127.0.0.1" && _ip != "localhost") {
                            listen.Append($";http://localhost:{_port}");
                        }
                    }

                    _webHost = builder
                        .UseUrls(listen.ToString())
                        .Build();
                    
                    await _webHost.RunAsync(Current.AppCancellationSource.Token);

                    _webHost.Dispose();

                    if (Current.AppCancellationSource == null) {
                        Current.AppCancellationSource = new CancellationTokenSource();

                        await server.Start();
                    }
                } catch (Exception exception) {
                    Log.LogError(exception);
                }
            }

            private void ServerRegistration() {
                Task.Run(() => {
                    string url =
                        $"http://getlist2.obovse.ru/remote/index.php?v={Assembly.GetExecutingAssembly().GetName().Version}&do=list&localip={_ip}:{_port}&proxy=false";
                    string result = HTTPUtility.GetRequest(url);
                    Log.LogInformation(result);
                    //if (result.Split('|')[0] == "new_version") {
                    //    if (mcbCheckUpdate.Checked) {
                    //        MenuItemNewVersion.Text = result.Split('|')[1];
                    //        MenuItemNewVersion.Visible = true;
                    //        urlnewversion = result.Split('|')[2];
                    //        newversion = result.Split('|')[3];
                    //    }
                    //}
                    //timerR.Enabled = mcbUseProxy.Checked;
                    //toolStripStatusLabel1.Text = $"{Resources.Main_ServerRegistration}: OK";
                    //Log.Debug("StartServer->Result: {0}", result);
                });
            }

            internal void Stop() {
                if (_webHost != null) {
                    Current.AppCancellationSource.Cancel();
                }
            }

            internal void Restart() {
                if (_webHost != null) {
                    var token = Current.AppCancellationSource;
                    Current.AppCancellationSource = null;
                    token.Cancel();
                }
            }
        }

        public static class Current {
            public static CancellationTokenSource AppCancellationSource = new CancellationTokenSource();
        }
    }
}
