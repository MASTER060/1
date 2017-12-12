using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using RemoteFork.Network;
using RemoteFork.Plugins;
using RemoteFork.Properties;
using System.Text.RegularExpressions;
using System.Web;
using MetroFramework;
using NLog;
using RemoteFork.Server;

namespace RemoteFork.Forms {
    internal partial class Main : MetroFramework.Forms.MetroForm {
        private static readonly ILogger Log = LogManager.GetLogger("Main", typeof(Main));

        public static HashSet<string> Devices = new HashSet<string>();
        private bool loading = true;
        private HttpServer _httpServer;
        
        #region SERVER

        private void StartServer() {
            _httpServer = new HttpServer(mcbServerIp.SelectedItem.ToString(),
                int.Parse(mtbServerPort.Text));
            _httpServer.Start();

            ServerRegistration();
        }

        private string urlnewversion = string.Empty;
        private string newversion = string.Empty;

        private void ServerRegistration() {
            toolStripStatusLabel1.Text = $"{Resources.Main_ServerRegistration}...";
            string result = HTTPUtility.GetRequest(
                $"http://getlist2.obovse.ru/remote/index.php?v={Assembly.GetExecutingAssembly().GetName().Version}&do=list&localip={mcbServerIp.SelectedItem}:{mtbServerPort.Text}&proxy={mcbUseProxy.Checked}");
            Log.Debug("http://getlist2.obovse.ru/remote/index.php?v={0}&do=list&localip={1}:{2}&proxy={3}",
                Assembly.GetExecutingAssembly().GetName().Version, mcbServerIp.SelectedItem, mtbServerPort.Text,
                mcbUseProxy.Checked);
            if (result.Split('|')[0] == "new_version") {
                if (mcbCheckUpdate.Checked) {
                    MenuItemNewVersion.Text = result.Split('|')[1];
                    MenuItemNewVersion.Visible = true;
                    urlnewversion = result.Split('|')[2];
                    newversion = result.Split('|')[3];
                }
            }
            timerR.Enabled = mcbUseProxy.Checked;
            toolStripStatusLabel1.Text = $"{Resources.Main_ServerRegistration}: OK";
            Log.Debug("StartServer->Result: {0}", result);
        }

        private void StopServer() {
            if (_httpServer != null) {
                _httpServer.Stop();
                _httpServer = null;
            }
        }

        #endregion SERVER
        
        #region PROGRAM
        
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
        
        #region SETTINGS

        private void LoadSettings() {
            notifyIcon1.Text = Text += $" {Assembly.GetExecutingAssembly().GetName().Version}";

            mcbLogs.SelectedIndex = Settings.Default.LogLevel;

            mcbAutoStartWindows.Checked = Settings.Default.AutoStartWindows;
            mtAutoStartServer.Checked = Settings.Default.AutoStartServer;
            mcbDlna.Checked = Settings.Default.Dlna;
            mtbServerPort.Text = Settings.Default.Port.ToString();
            mcbUseProxy.Checked = Settings.Default.proxy;
            mcbCheckUpdate.Checked = Settings.Default.CheckUpdate;
            mcbThvpAutoStart.Checked = Settings.Default.THVPAutoStart;

            mtbUserAgent.Text = Settings.Default.UserAgent;

            object[] ipAddresses = Tools.GetIPAddresses();
            mcbServerIp.Items.AddRange(ipAddresses);

            if (IPAddress.TryParse(Settings.Default.IpIPAddress, out IPAddress ip)) {
                if (mcbServerIp.Items.Contains(ip)) {
                    mcbServerIp.SelectedItem = ip;
                } else {
                    mcbServerIp.SelectedIndex = 0;
                }
            } else {
                mcbServerIp.SelectedIndex = 0;
            }

            if (Settings.Default.AutoStartServer) {
                mbStartServer.PerformClick();
            }

            if (Settings.Default.THVPAutoStart) {
                ThvpStart();
            }

            loading = false;

        }

        #endregion SETTINGS

        private void CheckAutostart() {
            try {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                if (key != null) {
                    if (mcbAutoStartWindows.Checked) {
                        key.SetValue("RemoteFork", Application.ExecutablePath);
                    } else {
                        if (key.GetValue("RemoteFork") != null) {
                            key.DeleteValue("RemoteFork");
                        }
                    }
                    key.Close();
                }
            } catch (Exception e) {
                toolStripStatusLabel1.Text = Resources.Main_Start_From_Admin_mini;
                mcbAutoStartWindows.Enabled = false;
                mcbAutoStartWindows.Text = $"{Resources.Main_Autostart} ({Resources.Main_Can_Not_Access_Registry})";
                var t = new ToolTip();
                t.SetToolTip(mcbAutoStartWindows, $"{e.Message} {Resources.Main_Start_From_Admin_maxi}");
                if (mcbAutoStartWindows.Checked) {
                    //  cbAutoStart.Checked = false;
                    // MessageBox.Show("Не удалось установить RemoteFork в автозапуск при старте Windows. Запустите программу от имени администратора!", e.Message);
                }
            }
        }

        #region TIMER

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
                HTTPUtility.GetRequest(
                    "http://195.88.208.101/obovse.ru/smarttv/api.php?do=xhrremote2&direct&proxy=1&v=get");
            //  Console.WriteLine(result);
            if (result.StartsWith("/parserlink")) {
                Console.WriteLine("parserlink remote");
                var result2 = string.Empty;

                var requestStrings = HttpUtility.UrlDecode(result.Substring(12)).Split('|');
                var handle = new Requestes.ParseLinkRequestHandler();
                if (requestStrings != null) {
                    Console.WriteLine("Parsing: {0}", requestStrings[0]);

                    string curlResponse = requestStrings[0].StartsWith("curl")
                        ? handle.CurlRequest(HttpUtility.UrlDecode(requestStrings[0]))
                        : HTTPUtility.GetRequest(requestStrings[0]);
                    //Console.WriteLine("Response original " + curlResponse);
                    if (requestStrings.Length == 1) {
                        result2 = curlResponse;
                    } else {
                        requestStrings[1] = HttpUtility.UrlDecode(requestStrings[1]);
                        requestStrings[2] = HttpUtility.UrlDecode(requestStrings[2]);
                        Console.WriteLine("1: {0}", requestStrings[1]);
                        Console.WriteLine("2: {0}", requestStrings[2]);
                        if (!requestStrings[1].Contains(".*?")) {
                            if (string.IsNullOrEmpty(requestStrings[1]) && string.IsNullOrEmpty(requestStrings[2])) {
                                result2 = curlResponse;
                            } else {
                                int num1 = curlResponse.IndexOf(requestStrings[1], StringComparison.Ordinal);
                                if (num1 == -1) {
                                    result2 = string.Empty;
                                } else {
                                    num1 += requestStrings[1].Length;
                                    var num2 = curlResponse.IndexOf(requestStrings[2], num1, StringComparison.Ordinal);
                                    result2 = num2 == -1 ? string.Empty : curlResponse.Substring(num1, num2 - num1);
                                }
                            }
                        } else {
                            Log.Debug($"ParseLinkRequest: {requestStrings[1]}(.*?){requestStrings[2]}");
                            Console.WriteLine($"ParseLinkRequest: {requestStrings[1]}(.*?){requestStrings[2]}");

                            string pattern = requestStrings[1] + "(.*?)" + requestStrings[2];
                            var regex = new Regex(pattern, RegexOptions.Multiline);
                            var match = regex.Match(curlResponse);
                            if (match.Success) result2 = match.Groups[1].Captures[0].ToString();
                        }
                    }
                    //Console.WriteLine("result="+result2);
                    string xu = "http://195.88.208.101/obovse.ru/smarttv/api.php?do=xhrremote2&proxy=1&v=post&u=" +
                                HttpUtility.UrlEncode(result);
                    Console.WriteLine(xu);
                    Console.WriteLine(
                        $"answ={HTTPUtility.PostRequest(xu, "s=" + HttpUtility.UrlEncode(result2))}");
                }
            }

            toolStripStatusLabel1.Text = toolStripStatusLabel1.Text.Replace("...", "");
        }

        #endregion TIMER
        
        #region THVP

        public void ThvpStart() {
            var ps1 = Process.GetProcessesByName("thvp");
            if (ps1.Length <= 0) {
                var thvpPath = Path.Combine(Directory.GetCurrentDirectory(), "THVP/thvp.exe");
                if (File.Exists(thvpPath)) {
                    Process.Start(thvpPath);
                }
            }
        }

        #endregion THVP

        #endregion PROGRAM

        #region FORM

        public Main() {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e) {
            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);

            LoadSettings();

            InitializePlugins();
            InitializeDlna();

            mtbFileBufferSize.Value =
                (Settings.Default.FileBufferSize / 1024).Clamp(mtbFileBufferSize.Minimum, mtbFileBufferSize.Maximum);

            try {
                CheckAutostart();
            } catch (Exception exception) {
                Console.WriteLine(exception);
            } finally {
                notifyIcon1.Visible = true;
                HideForm();
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

        #region SETTINGS

        #region MAIN

        private void bStartServer_Click(object sender, EventArgs e) {
            toolStripStatusLabel1.Text = $"{Resources.Main_Starting_Server}...";
            try {
                StartServer();

                mbStartServer.Enabled = false;
                mbStopServer.Enabled = true;
                toolStripStatusLabel1.Text = $"{Resources.Main_Starting_Server}: OK"; 
            } catch (Exception exception) {
                toolStripStatusLabel1.Text = Resources.Main_Error;
                Log.Error(exception);
            }
        }

        private void bStopServer_Click(object sender, EventArgs e) {
            toolStripStatusLabel1.Text = $"{Resources.Main_Stopping_Server}...";
            try {
                StopServer();

                mbStartServer.Enabled = true;
                mbStopServer.Enabled = false;
                toolStripStatusLabel1.Text = $"{Resources.Main_Stopping_Server}: OK";
            } catch (Exception exception) {
                toolStripStatusLabel1.Text = Resources.Main_Error;
                Log.Error(exception);
            }
        }

        private void cbAutoStart_CheckedChanged(object sender, EventArgs e) {
            CheckAutostart();

            Settings.Default.AutoStartWindows = mcbAutoStartWindows.Checked;
            Settings.Default.Save();
        }

        private void mtAutoStartServer_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.AutoStartServer = mtAutoStartServer.Checked;
            Settings.Default.Save();
        }

        private void cbSeverIp_SelectedIndexChanged(object sender, EventArgs e) {
            Settings.Default.IpIPAddress = mcbServerIp.SelectedItem.ToString();
            Settings.Default.Save();
        }

        private void mcbCheckUpdate_CheckedChanged(object sender, EventArgs e) {
            if (loading) return;

            Settings.Default.CheckUpdate = mcbCheckUpdate.Checked;
            Settings.Default.Save();
        }

        private void cbThvpAutoStart_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.THVPAutoStart = mcbThvpAutoStart.Checked;
            Settings.Default.Save();
        }

        private void cbUseProxy_CheckedChanged(object sender, EventArgs e) {
            if (loading) return;
            if (mcbUseProxy.Checked) {
                if (MetroMessageBox.Show(this,
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

        #endregion MAIN
        
        #region DLNA

        private void cbDlna_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.Dlna = mcbDlna.Checked;
            metroPanel6.Enabled = metroPanel7.Enabled = mcbDlna.Checked;

            Settings.Default.Save();
        }

        private void InitializeDlna() {
            mcbDlnaFilterType.SelectedIndex = Settings.Default.DlnaFilterType;
            mtDlnaHiddenFiles.Checked = Settings.Default.DlnaHiidenFiles;

            if ((Settings.Default.DlnaFileExtensions != null) && (Settings.Default.DlnaFileExtensions.Count > 0)) {
                mtbDlnaFileExtensions.Text = string.Join(",", Settings.Default.DlnaFileExtensions.Cast<string>());
                mlSampleTypes.Visible = false;
            }

            if ((Settings.Default.DlnaDirectories != null) && (Settings.Default.DlnaDirectories.Count > 0)) {
                foreach (var directory in Settings.Default.DlnaDirectories) mlvDlna.Items.Add(directory, directory, string.Empty);
            }

            metroPanel6.Enabled = metroPanel7.Enabled = mcbDlna.Checked;
        }

        private void mtbTypes_Enter(object sender, EventArgs e) {
            mlSampleTypes.Visible = false;
        }

        private void mtbTypes_Leave(object sender, EventArgs e) {
            mlSampleTypes.Visible = string.IsNullOrWhiteSpace(mtbDlnaFileExtensions.Text);
        }

        private void tsmiDlnaListAdd_Click(object sender, EventArgs e) {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                string path = folderBrowserDialog1.SelectedPath;
                if (!mlvDlna.Items.ContainsKey(path)) {
                    mlvDlna.Items.Add(path, path, string.Empty);

                    if (!Settings.Default.DlnaDirectories.Contains(path)) {
                        Settings.Default.DlnaDirectories.Add(path);
                        Settings.Default.Save();
                    }
                } else {
                    MetroMessageBox.Show(this, "Выбранный каталог уже добавлен.");
                }
            }
        }

        private void tsmiDlnaListRemove_Click(object sender, EventArgs e) {
            if (mlvDlna.SelectedIndices.Count > 0) {
                foreach (int id in mlvDlna.SelectedIndices) {
                    mlvDlna.Items.RemoveAt(id);

                    if (Settings.Default.DlnaDirectories.Contains(mlvDlna.Items[id].Text)) {
                        Settings.Default.DlnaDirectories.Remove(mlvDlna.Items[id].Text);
                    }
                }

                Settings.Default.Save();
            }
        }

        private void tsmiDlnaListClear_Click(object sender, EventArgs e) {
            mlvDlna.Items.Clear();
            Settings.Default.DlnaDirectories.Clear();
            Settings.Default.Save();
        }

        private void mcbDlnaFilterType_SelectedIndexChanged(object sender, EventArgs e) {
            Settings.Default.DlnaFilterType = (byte) mcbDlnaFilterType.SelectedIndex;
            Settings.Default.Save();
        }

        private void mtDlnaHiddenFiles_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.DlnaHiidenFiles = mtDlnaHiddenFiles.Checked;
            Settings.Default.Save();
        }

        private void mtbDlnaFileExtensions_KeyUp(object sender, KeyEventArgs e) {
            string text = mtbDlnaFileExtensions.Text;
            Settings.Default.DlnaFileExtensions.Clear();
            if (!string.IsNullOrWhiteSpace(text)) {
                if (Regex.IsMatch(text, "^((\\D[\\w]+,)\\s*)+(\\D[\\w]+)\\s*$",
                    RegexOptions.IgnoreCase & RegexOptions.Multiline)) {
                    foreach (var item in text.Split(',')) {
                        Settings.Default.DlnaFileExtensions.Add(item.Trim());
                    }
                }
            }
            Settings.Default.Save();
        }

        #endregion DLNA

        #region PLUGINS

        private void cbPlugins_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.Plugins = mcbPlugins.Checked;
            mclbPlugins.Enabled = mcbPlugins.Checked;

            Settings.Default.Save();
        }

        private void InitializePlugins() {
            var plugins = PluginManager.Instance.GetPlugins(false);

            foreach (var plugin in plugins) {
                var item = new ListViewItem {
                    Checked = Settings.Default.EnablePlugins != null
                              && Settings.Default.EnablePlugins.Contains(plugin.Value.Key),
                    Text = plugin.Value.ToString(),
                    Tag = plugin.Value.Key
                };
                mclbPlugins.Items.Add(item);
            }

            mclbPlugins.Enabled = mcbPlugins.Checked;
        }

        private void mclbPlugins_ItemChecked(object sender, ItemCheckedEventArgs e) {
            var item = e.Item;
            string key = item.Tag.ToString();
            if (item.Checked) {
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

        #endregion PLUGINS

        #region  DEVELOPER

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

        private void cbLogs_SelectedIndexChanged(object sender, EventArgs e) {
            var cb = (ComboBox)sender;

            Settings.Default.LogLevel = (byte)cb.SelectedIndex;
            Settings.Default.Save();

            LogManager.GlobalThreshold = AppLogLevel.FromOrdinal(cb.SelectedIndex);
        }

        private void mtbFileBufferSize_ValueChanged(object sender, EventArgs e) {
            mlFileBufferSize.Text = mtbFileBufferSize.Value.ToString();
            Settings.Default.FileBufferSize = mtbFileBufferSize.Value * 1024;
            Settings.Default.Save();
        }

        #endregion DEVELOPER

        #endregion SETTINGS

        #region NOTIFYICON

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

                    var data = "text=" + HttpUtility.UrlEncode(text);
                    var text2 = HTTPUtility.PostRequest(url, data);

                    MetroMessageBox.Show(this, text2);
                } else {
                    MetroMessageBox.Show(this, Resources.Main_Invalid_Playlist_File);
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

        private void runToolStripMenuItem_Click(object sender, EventArgs e) {
            ThvpStart();
        }

        private void gotoToolStripMenuItem_Click(object sender, EventArgs e) {

            Process.Start("http://thvp.ru/about");
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

        #endregion NOTIFYICON

        #endregion FORM
    }
}
