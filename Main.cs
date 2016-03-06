using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using RemoteFork.Properties;

namespace RemoteFork {
    public partial class Main : Form {
        private HttpServer httpServer;
        private Thread thread;

        #region Form

        public Main() {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e) {
            tbIp.Text = Settings.Default.GetIpAuto ? Tools.GetIPAddress() : Settings.Default.IpIPAddress;
            cbDlna.Checked = Settings.Default.Dlna;
            cbAutoIp.Checked = Settings.Default.GetIpAuto;
            cbAutoStart.Checked = Settings.Default.ServerAutoStart;

            if (Settings.Default.GetIpAuto) {
                tbIp.ReadOnly = true;
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
            httpServer = new MyHttpServer(IPAddress.Parse(tbIp.Text), int.Parse(tbPort.Text));
            thread = new Thread(httpServer.Listen);
            thread.Start();
        }

        private void StopServer() {
            if (httpServer != null) {
                httpServer.Stop();
            }
            if (thread != null) {
                thread.Abort();
            }
        }

        #endregion Server

        #region Settings

        private void bStartServer_Click(object sender, EventArgs e) {
            toolStripStatusLabel1.Text = "Сервер запущен";
            try {
                StartServer();
                bStartServer.Enabled = false;
                bStopServer.Enabled = true;
            } catch (Exception) {
                toolStripStatusLabel1.Text = "Ошибка!";
            }
        }

        private void bStopServer_Click(object sender, EventArgs e) {
            toolStripStatusLabel1.Text = "Остановка сервера..";
            try {
                StopServer();
                bStartServer.Enabled = true;
                bStopServer.Enabled = false;
            } catch (Exception) {
                toolStripStatusLabel1.Text = "Ошибка!";
            }
        }

        private void cbAutoStart_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ServerAutoStart = cbAutoStart.Checked;
        }

        private void cbAutoIp_CheckedChanged(object sender, EventArgs e) {
            if (cbAutoIp.Checked) {
                Settings.Default.GetIpAuto = true;
                tbIp.ReadOnly = true;
                tbIp.Text = Tools.GetIPAddress();
            } else {
                Settings.Default.GetIpAuto = false;
                tbIp.ReadOnly = false;
                Settings.Default.IpIPAddress = tbIp.Text;
            }
            Settings.Default.Save();
        }

        private void tbIp_TextChanged(object sender, EventArgs e) {
            Settings.Default.IpIPAddress = tbIp.Text;
            Settings.Default.Save();
        }

        private void cbDlna_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.Dlna = cbDlna.Checked;
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
            if (!string.IsNullOrWhiteSpace(Settings.Default.Devices)) {
                string[] array = Settings.Default.Devices.Split('~');
                devicesToolStripMenuItem1.Text = array[0] + " (" + array[2] + ")";
            } else {
                devicesToolStripMenuItem1.Text = "Нет активных устройств";
            }
        }

        private void devicesToolStripMenuItem1_Click(object sender, EventArgs e) {
            bool flag = openFileDialog1.ShowDialog() == DialogResult.OK;
            if (flag) {
                StreamReader streamReader = new StreamReader(openFileDialog1.FileName);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                bool flag2 = text.Length < 102401 &&
                             (text.IndexOf("EXTM3U") >= 0 || text.IndexOf("<title>") >= 0 ||
                              text.IndexOf("http://") >= 0);
                if (flag2) {
                    WebRequest webRequest =
                        WebRequest.Create("http://forkplayer.tv/remote/index.php?do=uploadfile&fname=" +
                                          openFileDialog1.FileName + "&initial=" + Settings.Default.Devices.Replace('~', '|'));
                    webRequest.Method = "POST";
                    webRequest.Timeout = 120000;
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    byte[] bytes = Encoding.GetEncoding(65001).GetBytes("text=" + WebUtility.UrlEncode(text));
                    webRequest.ContentLength = (long)bytes.Length;
                    Stream requestStream = webRequest.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                    WebResponse response = webRequest.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader streamReader2 = new StreamReader(responseStream);
                    string text2 = streamReader2.ReadToEnd();
                    streamReader2.Close();
                    responseStream.Close();
                    response.Close();
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
            Process.Start("http://" + tbIp.Text + ":" + tbPort.Text + "/test");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                StopServer();
            } finally {
                Close();
            }
        }

        #endregion notifyIcon
    }
}
