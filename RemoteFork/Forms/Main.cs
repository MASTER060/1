using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using Common.Logging;
using RemoteFork.Network;
using RemoteFork.Plugins;
using RemoteFork.Properties;
using RemoteFork.Server;
using System.Text.RegularExpressions;

namespace RemoteFork.Forms {
    internal partial class Main : MetroFramework.Forms.MetroForm {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Main));

        public static HashSet<string> Devices = new HashSet<string>();
        private bool loading = true;
        private HttpServer _httpServer;

        #region Settings

        private void LoadSettings() {
            notifyIcon1.Text = Text += $" {Assembly.GetExecutingAssembly().GetName().Version}";

            mcbLogs.SelectedIndex = Settings.Default.LogLevel;

            mcbAutoStart.Checked = Settings.Default.ServerAutoStart;
            mcbDlna.Checked = Settings.Default.Dlna;
            mtbServerPort.Text = Settings.Default.Port.ToString();
            mcbUseProxy.Checked = Settings.Default.proxy;

            object[] ipAddresses = Tools.GetIPAddresses();
            mcbServerIp.Items.AddRange(ipAddresses);

            IPAddress ip;
            if (IPAddress.TryParse(Settings.Default.IpIPAddress, out ip)) {
                if (mcbServerIp.Items.Contains(ip)) {
                    mcbServerIp.SelectedItem = ip;
                } else {
                    mcbServerIp.SelectedIndex = 0;
                }
            } else {
                mcbServerIp.SelectedIndex = 0;
            }
            if (Settings.Default.ServerAutoStart || true) {
                mbStartServer.PerformClick();
            }

            if (Settings.Default.THVPAutoStart) {
                ThvpStart();
            }
            mcbThvpAutoStart.Checked = Settings.Default.THVPAutoStart;

            mtbUserAgent.Text = Settings.Default.UserAgent;
            loading = false;

        }

        #endregion Settings

        private void tbUserAgent_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Escape:
                    mtbUserAgent.Text = Settings.Default.UserAgent;
                    break;
                case Keys.Enter:
                    Settings.Default.UserAgent = mtbUserAgent.Text;
                    Settings.Default.Save();
                    break;
            }
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                StopServer();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Form

        public Main() {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e) {

            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);

            LoadSettings();
            try {
                notifyIcon1.Visible = true;
                HideForm();
                //webBrowser1.Navigate("http://tree.tv/feedback");
                Autost();

            } catch (Exception exception) {
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e) {
            StopServer();
        }

        private void Main_Resize(object sender, EventArgs e) {
            if (WindowState == FormWindowState.Minimized) HideForm();
        }

        private void ShowForm() {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void HideForm() {
            Hide();
            WindowState = FormWindowState.Minimized;
        }

        #endregion Form

        #region Server

        private void StartServer() {
            _httpServer = new HttpServer((IPAddress) mcbServerIp.SelectedItem, int.Parse(mtbServerPort.Text));

            ServerRegistration();
        }

        private string urlnewversion = "";
        private string newversion = "";

        private void ServerRegistration() {
            toolStripStatusLabel1.Text = $"{Resources.Main_ServerRegistration}...";
            var result = HttpUtility.GetRequest(
                string.Format(
                    "http://getlist2.obovse.ru/remote/index.php?v={0}&do=list&localip={1}:{2}&proxy={3}",
                    Assembly.GetExecutingAssembly().GetName().Version,
                    mcbServerIp.SelectedItem,
                    mtbServerPort.Text, mcbUseProxy.Checked));
            Console.WriteLine("http://getlist2.obovse.ru/remote/index.php?v={0}&do=list&localip={1}:{2}&proxy={3}",
                Assembly.GetExecutingAssembly().GetName().Version, mcbServerIp.SelectedItem, mtbServerPort.Text,
                mcbUseProxy.Checked);
            if (result.Split('|')[0] == "new_version") {
                MenuItemNewVersion.Text = result.Split('|')[1];
                MenuItemNewVersion.Visible = true;
                urlnewversion = result.Split('|')[2];
                newversion = result.Split('|')[3];
            }
            if (mcbUseProxy.Checked) timerR.Enabled = true;
            else timerR.Enabled = false;
            toolStripStatusLabel1.Text = $"{Resources.Main_ServerRegistration}: OK";
            Log.Debug(m => m("StartServer->Result: {0}", result));
        }

        private void StopServer() {
            if (_httpServer != null) {
                _httpServer.Stop();
                _httpServer = null;
            }
        }

        #endregion Server

        #region Settings

        private void bStartServer_Click(object sender, EventArgs e) {
            toolStripStatusLabel1.Text = $"{Resources.Main_Starting_Server}...";
            try {
                StartServer();

                mbStartServer.Enabled = false;
                mbStopServer.Enabled = true;
                toolStripStatusLabel1.Text = $"{Resources.Main_Starting_Server}: OK";



            } catch (Exception ex) {
                toolStripStatusLabel1.Text = Resources.Main_Error;
                Log.Error(m => m("StartServer->Errot: {0}", ex));
            }
        }

        private void bStopServer_Click(object sender, EventArgs e) {
            toolStripStatusLabel1.Text = $"{Resources.Main_Stopping_Server}...";
            try {
                StopServer();

                mbStartServer.Enabled = true;
                mbStopServer.Enabled = false;
                toolStripStatusLabel1.Text = $"{Resources.Main_Stopping_Server}: OK";
            } catch (Exception ex) {
                toolStripStatusLabel1.Text = Resources.Main_Error;
                Log.Error(m => m("StopServer->Errot: {0}", ex));
            }
        }

        private void cbAutoStart_CheckedChanged(object sender, EventArgs e) {
            //Console.WriteLine("cbAutoStart_CheckedChanged");
            Autost();
            Settings.Default.ServerAutoStart = mcbAutoStart.Checked;
            Settings.Default.Save();
        }

        private void Autost() {
            try {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                if (key.GetValue("RemoteFork") != null) key.DeleteValue("RemoteFork");
                if (mcbAutoStart.Checked) key.SetValue("RemoteFork", Application.ExecutablePath);

            } catch (Exception e) {
                toolStripStatusLabel1.Text = Resources.Main_Start_From_Admin_mini;
                mcbAutoStart.Enabled = false;
                mcbAutoStart.Text = $"{Resources.Main_Autostart} ({Resources.Main_Can_Not_Access_Registry})";
                var t = new ToolTip();
                t.SetToolTip(mcbAutoStart, $"{e.Message} {Resources.Main_Start_From_Admin_maxi}");
                if (mcbAutoStart.Checked) {
                    //  cbAutoStart.Checked = false;
                    // MessageBox.Show("Не удалось установить RemoteFork в автозапуск при старте Windows. Запустите программу от имени администратора!", e.Message);
                }
            }

        }

        private void cbDlna_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.Dlna = mcbDlna.Checked;
            Settings.Default.Save();
        }

        private void cbSeverIp_SelectedIndexChanged(object sender, EventArgs e) {
            Settings.Default.IpIPAddress = mcbServerIp.SelectedItem.ToString();
            Settings.Default.Save();
        }

        private void llDlnaConfigurate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var form = new DlnaConfigurate();
            form.ShowDialog();
        }

        private void cbPlugins_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.Plugins = mcbPlugins.Checked;
            Settings.Default.Save();
        }

        private void llPluginsConfigurate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var form = new PluginsConfigurate();
            form.ShowDialog();
        }

        private void cbLogs_SelectedIndexChanged(object sender, EventArgs e) {
            var cb = (ComboBox) sender;

            Settings.Default.LogLevel = (byte) cb.SelectedIndex;
            Settings.Default.Save();

            NLog.LogManager.GlobalThreshold = AppLogLevel.FromOrdinal(cb.SelectedIndex);
        }

        #endregion Settings

        #region notifyIcon

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ShowForm();
            }
        }

        private void loadPlaylistToolStripMenuItem1_DropDownOpening(object sender, EventArgs e) {
            loadPlaylistToolStripMenuItem1.DropDownItems.Clear();

            if (Devices.Count > 0) {
                foreach (string device in Devices) {
                    var array = device.Split('|');
                    string name = array[0] + " (" + array[2] + ")";

                    var item = new ToolStripMenuItem {
                        Name = "device" + array[0] + "ToolStripMenuItem",
                        Tag = device,
                        Text = name
                    };
                    item.Click += devicesToolStripMenuItem_Click;

                    loadPlaylistToolStripMenuItem1.DropDownItems.Add(item);
                }
            } else {
                loadPlaylistToolStripMenuItem1.DropDownItems.Add(Resources.Main_No_Active_Devices);
            }
        }

        private void devicesToolStripMenuItem_Click(object sender, EventArgs e) {
            var clickedItem = (ToolStripMenuItem) sender;

            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                var streamReader = new StreamReader(openFileDialog1.FileName);
                var text = streamReader.ReadToEnd();
                streamReader.Close();
                if ((text.Length < 102401) && (text.Contains("EXTM3U") || text.Contains("<title>") ||
                                               text.Contains("http://"))) {
                    var url = "http://forkplayer.tv/remote/index.php?do=uploadfile&fname=" + openFileDialog1.FileName +
                              "&initial=" + clickedItem.Tag;

                    var data = "text=" + System.Web.HttpUtility.UrlEncode(text);
                    var text2 = HttpUtility.PostRequest(url, data);

                    MessageBox.Show(text2);
                } else {
                    MessageBox.Show(Resources.Main_Invalid_Playlist_File);
                }
            }
        }

        private void playUrlToolStripMenuItem_Click(object sender, EventArgs e) {
            var form = new PlayUrl();
            form.ShowDialog();
        }

        private void pluginsToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            pluginsToolStripMenuItem.DropDownItems.Clear();

            var plugins = PluginManager.Instance.GetPlugins(false);

            if (plugins.Count > 0) {
                foreach (var plugin in plugins) {
                    var item = new ToolStripMenuItem(plugin.Value.ToString()) {
                        Tag = plugin.Value.Key,
                        Checked = (Settings.Default.EnablePlugins != null) &&
                                  Settings.Default.EnablePlugins.Contains(plugin.Value.Key),
                        CheckOnClick = true
                    };
                    item.CheckedChanged += pluginsToolStripMenuItem_CheckedChanged;
                    pluginsToolStripMenuItem.DropDownItems.Add(item);
                }
            } else {
                pluginsToolStripMenuItem.DropDownItems.Add(Resources.Main_No_Installed_Plugins);
            }
        }

        private void pluginsToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            var clickedItem = (ToolStripMenuItem) sender;
            var key = clickedItem.Tag.ToString();

            if (clickedItem.Checked) {
                if (!Settings.Default.EnablePlugins.Contains(key)) {
                    Settings.Default.EnablePlugins.Add(key);
                }
            } else {
                if (Settings.Default.EnablePlugins.Contains(key)) {
                    Settings.Default.EnablePlugins.Remove(key);
                }
            }

            Settings.Default.Save();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowForm();
        }

        private void openTestToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start($"http://{mcbServerIp.SelectedItem}:{mtbServerPort.Text}/test");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                StopServer();
            } finally {
                Close();
            }
        }

        #endregion notifyIcon

        #region THVP

        public void ThvpStart() {
            var ps1 = Process.GetProcessesByName("thvp");
            if (ps1.Length > 0) {
                MessageBox.Show(Resources.Main_THVP_Already_Running);
            } else {
                var thvpPath = Path.Combine(Directory.GetCurrentDirectory(), "THVP/thvp.exe");
                if (File.Exists(thvpPath)) {
                    Process.Start(thvpPath);
                }
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e) {
            ThvpStart();
        }

        private void cbThvpAutoStart_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.THVPAutoStart = mcbThvpAutoStart.Checked;
            Settings.Default.Save();
        }

        private void gotoToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start("http://thvp.ru/about");
        }

        #endregion

        private void cbUseProxy_CheckedChanged(object sender, EventArgs e) {
            if (loading) return;
            if (mcbUseProxy.Checked) {
                if (MessageBox.Show(
                        Resources.Main_TurnOn_Not_Recommended,
                        Resources.Main_Enable_Proxy, MessageBoxButtons.YesNo) != DialogResult.Yes) {
                    mcbUseProxy.Checked = false;
                    return;
                } else {
                    ServerRegistration();
                }
            }
            Settings.Default.proxy = mcbUseProxy.Checked;
            Settings.Default.Save();
        }

        private void MenuItemNewVersion_Click(object sender, EventArgs e) {
            //Process.Start(urlnewversion);
            ShowForm();
            toolStripStatusLabel1.Text = Resources.Main_Downloading_New_Version;
            var myWebClient = new WebClient();
            myWebClient.DownloadFile(urlnewversion,
                Environment.CurrentDirectory + "\\UnpackNewVersion" + newversion + "AndReplaceFiles.zip");
            toolStripStatusLabel1.Text = Resources.Main_Download_Completed;
            Process.Start(Environment.CurrentDirectory);
        }

        private void timerR_Tick(object sender, EventArgs e) {
            if (!Settings.Default.proxy) {
                mcbUseProxy.Checked = false;
                toolStripStatusLabel1.Text = Resources.Main_Server_Disabled;
                timerR.Enabled = false;
                return;
            }
            toolStripStatusLabel1.Text += "...";
            //Console.WriteLine("timer");
            string result =
                HttpUtility.GetRequest(
                    "http://195.88.208.101/obovse.ru/smarttv/api.php?do=xhrremote2&direct&proxy=1&v=get");
            //  Console.WriteLine(result);
            if (result.IndexOf("/parserlink") == 0) {
                Console.WriteLine("parserlink remote");
                var result2 = string.Empty;

                var requestStrings = System.Web.HttpUtility.UrlDecode(result.Substring(12)).Split('|');
                var Handle = new Requestes.ParseLinkRequestHandler();
                if (requestStrings != null) {
                    Console.WriteLine("Parsing: {0}", requestStrings[0]);

                    string curlResponse = requestStrings[0].StartsWith("curl")
                        ? Handle.CurlRequest(System.Web.HttpUtility.UrlDecode(requestStrings[0]))
                        : HttpUtility.GetRequest(requestStrings[0]);
                    //Console.WriteLine("Response original " + curlResponse);
                    if (requestStrings.Length == 1) {
                        result2 = curlResponse;
                    } else {
                        requestStrings[1] = System.Web.HttpUtility.UrlDecode(requestStrings[1]);
                        requestStrings[2] = System.Web.HttpUtility.UrlDecode(requestStrings[2]);
                        Console.WriteLine("1: {0}", requestStrings[1]);
                        Console.WriteLine("2: {0}", requestStrings[2]);
                        if (!requestStrings[1].Contains(".*?")) {
                            if (string.IsNullOrEmpty(requestStrings[1]) && string.IsNullOrEmpty(requestStrings[2])) {
                                result2 = curlResponse;
                            } else {
                                var num1 = curlResponse.IndexOf(requestStrings[1], StringComparison.Ordinal);
                                if (num1 == -1) {
                                    result2 = string.Empty;
                                } else {
                                    num1 += requestStrings[1].Length;
                                    var num2 = curlResponse.IndexOf(requestStrings[2], num1, StringComparison.Ordinal);
                                    result2 = num2 == -1 ? string.Empty : curlResponse.Substring(num1, num2 - num1);
                                }
                            }
                        } else {
                            Log.Debug(m => m("ParseLinkRequest: {0}", requestStrings[1] + "(.*?)" + requestStrings[2]));
                            Console.WriteLine("ParseLinkRequest: {0}", requestStrings[1] + "(.*?)" + requestStrings[2]);

                            string pattern = requestStrings[1] + "(.*?)" + requestStrings[2];
                            var regex = new Regex(pattern, RegexOptions.Multiline);
                            var match = regex.Match(curlResponse);
                            if (match.Success) result2 = match.Groups[1].Captures[0].ToString();
                        }
                    }
                    //Console.WriteLine("result="+result2);
                    string xu = "http://195.88.208.101/obovse.ru/smarttv/api.php?do=xhrremote2&proxy=1&v=post&u=" +
                                System.Web.HttpUtility.UrlEncode(result);
                    Console.WriteLine(xu);
                    Console.WriteLine(
                        $"answ={HttpUtility.PostRequest(xu, "s=" + System.Web.HttpUtility.UrlEncode(result2))}");
                }
            }

            toolStripStatusLabel1.Text = toolStripStatusLabel1.Text.Replace("...", "");
        }
    }
}
