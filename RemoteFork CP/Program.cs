using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using RemoteFork.Network;

namespace RemoteFork {
    public class Program {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        public static ILoggerFactory LoggerFactory = new LoggerFactory()
            .AddConsole(LogLevel.Debug)
            .AddDebug(LogLevel.Debug)
            .AddFile("log.txt");

        private static Server server;

        public static void Main(string[] args) {
            //var handle = GetConsoleWindow();
            //ShowWindow(handle, SW_HIDE);

            server = new Server();
            
            server.Start(SettingsManager.Settings.IpIPAddress, SettingsManager.Settings.Port);
        }
        
        internal class Server {
            private static readonly ILogger Log = LoggerFactory.CreateLogger<Server>();

            private readonly IWebHostBuilder builder = WebHost.CreateDefaultBuilder()
                //.UseEnvironment("Development")
                .UseStartup<Startup>()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory());

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
                //toolStripStatusLabel1.Text = $"{Resources.Main_ServerRegistration}...";
                string url =
                    $"http://getlist2.obovse.ru/remote/index.php?v={Assembly.GetExecutingAssembly().GetName().Version}&do=list&localip={ip}:{port}&proxy={SettingsManager.Settings.UseProxy}";
                string result = HTTPUtility.GetRequest(url);
                Log.LogInformation(url);
                //Assembly.GetExecutingAssembly().GetName().Version, ip, port, mcbUseProxy.Checked);
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
            }

            private void Stop() {
                webHost.StopAsync();
            }
        }
    }
}
