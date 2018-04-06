using System;
using System.Reflection;
using System.Text;
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

            server.Start();
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

            internal void Start() {
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

                    _webHost.Run();

                    _webHost.Dispose();
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
                    _webHost.StopAsync().Wait();
                }
            }
        }
    }
}
