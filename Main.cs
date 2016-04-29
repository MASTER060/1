using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using RemoteFork.Network;
using RemoteFork.Properties;
using RemoteFork.Server;

namespace RemoteFork {
    public partial class Main : Form {
        public static HashSet<string> Devices = new HashSet<string>();

        private HttpServer httpServer;
        private Thread thread;

        private readonly List<ToolStripMenuItem> deviceMenuItems = new List<ToolStripMenuItem>();

        #region Form

        public Main() {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e) {
            Text += " " + Settings.Default.AppVersion;
            notifyIcon1.Text += " " + Settings.Default.AppVersion;

            cbAutoStart.Checked = Settings.Default.ServerAutoStart;
            cbDlna.Checked = Settings.Default.Dlna;
            tbPort.Text = Settings.Default.Port.ToString();

            object[] ipAddresses = Tools.GetIPAddresses();
            cbIp.Items.AddRange(ipAddresses);

            IPAddress ip;
            if (IPAddress.TryParse(Settings.Default.IpIPAddress, out ip)) {
                if (cbIp.Items.Contains(ip)) {
                    cbIp.SelectedItem = ip;
                } else {
                    cbIp.SelectedIndex = 0;
                }
            } else {
                cbIp.SelectedIndex = 0;
            }

            if (Settings.Default.ServerAutoStart) {
                bStartServer.PerformClick();
            }

            notifyIcon1.Visible = true;
            HideForm();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e) {
            StopServer();
        }

        private void Main_Resize(object sender, EventArgs e) {
            switch (WindowState) {
                case FormWindowState.Minimized:
                    HideForm();
                    break;
            }
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
            httpServer = new MyHttpServer((IPAddress) cbIp.SelectedItem, int.Parse(tbPort.Text));
            thread = new Thread(httpServer.Listen);
            thread.Start();

            var result = HttpUtility.GetRequest(
                string.Format(
                    "http://getlist2.obovse.ru/remote/index.php?v={0}&do=list&localip={1}:{2}",
                    Settings.Default.AppVersion,
                    cbIp.SelectedItem, tbPort.Text)).Result;
            Console.WriteLine(result);
        }

        private void StopServer() {
            httpServer.Stop();
        }

        #endregion Server

        #region Settings

        private void bStartServer_Click(object sender, EventArgs e) {
            toolStripStatusLabel1.Text = "Запуск сервера..";
            try {
                StartServer();

                bStartServer.Enabled = false;
                bStopServer.Enabled = true;
                toolStripStatusLabel1.Text = "Сервер запущен";
            } catch (Exception ex) {
                toolStripStatusLabel1.Text = "Ошибка!";
                Console.WriteLine(ex);
            }
        }

        private void bStopServer_Click(object sender, EventArgs e) {
            toolStripStatusLabel1.Text = "Остановка сервера..";
            try {
                StopServer();

                bStartServer.Enabled = true;
                bStopServer.Enabled = false;
                toolStripStatusLabel1.Text = "Сервер остановлен";
            } catch (Exception ex) {
                toolStripStatusLabel1.Text = "Ошибка!";
                Console.WriteLine(ex);
            }
        }

        private void cbAutoStart_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ServerAutoStart = cbAutoStart.Checked;
        }

        private void cbDlna_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.Dlna = cbDlna.Checked;
            Settings.Default.Save();
        }

        private void cbIp_SelectedIndexChanged(object sender, EventArgs e) {
            Settings.Default.IpIPAddress = cbIp.SelectedItem.ToString();
            Settings.Default.Save();
        }

        #endregion Settings

        #region notifyIcon

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ShowForm();
            }
        }

        private void loadPlaylistToolStripMenuItem1_MouseHover(object sender, EventArgs e) {
            Console.WriteLine("read Sets");
            foreach (var deviceMenuItem in deviceMenuItems) {
                loadPlaylistToolStripMenuItem1.DropDownItems.Remove(deviceMenuItem);
            }
            deviceMenuItems.Clear();

            if (Devices.Count > 0) {
                foreach (var device in Devices) {
                    string[] array = device.Split('|');
                    string name = array[0] + " (" + array[2] + ")";

                    var item = new ToolStripMenuItem {
                        Name = "device" + array[0] + "ToolStripMenuItem",
                        Tag = device,
                        Text = name
                    };
                    item.Click += devicesToolStripMenuItem1_Click;

                    deviceMenuItems.Add(item);
                    loadPlaylistToolStripMenuItem1.DropDownItems.Add(item);
                }

                devicesToolStripMenuItem1.Visible = false;
            } else {
                devicesToolStripMenuItem1.Text = "Нет активных устройств";
                devicesToolStripMenuItem1.Visible = true;
            }
        }

        private async void devicesToolStripMenuItem1_Click(object sender, EventArgs e) {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem) sender;
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                StreamReader streamReader = new StreamReader(openFileDialog1.FileName);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                if (text.Length < 102401 &&
                    (text.Contains("EXTM3U") || text.Contains("<title>") || text.Contains("http://"))) {
                    string url = "http://forkplayer.tv/remote/index.php?do=uploadfile&fname=" +
                                 openFileDialog1.FileName + "&initial=" + clickedItem.Tag;

                    var data = new Dictionary<string, string> {{"text", text}};
                    string text2 = await HttpUtility.PostRequest(url, data);

                    MessageBox.Show(text2);
                } else {
                    MessageBox.Show("Неверный файл плейлиста!");
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowForm();
        }

        private void openTestToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start("http://" + cbIp.SelectedItem + ":" + tbPort.Text + "/test");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                StopServer();
            } finally {
                Close();
            }
        }

        #endregion notifyIcon    

        private void llDlnaConfigurate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var form = new DlnaConfigurate();
            form.ShowDialog();
        }
    }
}
