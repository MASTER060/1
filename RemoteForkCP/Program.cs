using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using RemoteFork.Log;
using RemoteFork.Network;
using RemoteFork.Server;
using RemoteFork.Settings;

namespace RemoteFork {
    public class Program {
        private static Server server;

        public static void Main(string[] args) {
            server = new Server();
            
            server.Start(ProgramSettings.Settings.IpAddress, ProgramSettings.Settings.Port);
        }

        internal class Server {
            private static readonly Logger Log = new Logger(typeof(Server));

            private readonly IWebHostBuilder builder = WebHost.CreateDefaultBuilder()
                //.UseEnvironment("Development")
                .UseStartup<Startup>()
                .UseKestrel()
#if DEBUG
                .UseContentRoot(Environment.CurrentDirectory);
#else
                .UseContentRoot(AppDomain.CurrentDomain.BaseDirectory);
#endif

            private IWebHost webHost;

            private string ip;
            private ushort port;

            internal void Start(string ip, ushort port) {
                this.ip = ip;
                this.port = port;

                ServerRegistration();

                webHost = builder
                    .UseUrls($"http://{ip}:{port}")
                    .Build();
                webHost.Run();
            }

            private void ServerRegistration() {
                var task = new Task(() => {
                    ;
                    //toolStripStatusLabel1.Text = $"{Resources.Main_ServerRegistration}...";
                    string url =
                        $"http://getlist2.obovse.ru/remote/index.php?v={Assembly.GetExecutingAssembly().GetName().Version}&do=list&localip={ip}:{port}&proxy=false";
                    string result =  HTTPUtility.GetRequest(url);
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
                task.Start();

            }

            private void Stop() {
                webHost.StopAsync();
            }
        }
    }
}
