using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RemoteFork {
    public class Form1 : Form {
        public HttpServer httpServer;
        public string[] sets;
        public bool act = true;

        private Thread thread = null;

        public void SaveSets() {
            string text = "";
            for (int i = 0; i < sets.Length; i++) {
                bool flag = i > 0;
                if (flag) {
                    text += "|";
                }
                text += sets[i];
            }
            Console.WriteLine(Environment.CurrentDirectory + "\\main.cfg");
            File.WriteAllText(Environment.CurrentDirectory + "\\main.cfg", text);
        }

        public string[] GetSets() {
            string text = "";
            bool flag = File.Exists(Environment.CurrentDirectory + "\\main.cfg");
            if (flag) {
                try {
                    using (StreamReader streamReader = new StreamReader(Environment.CurrentDirectory + "\\main.cfg")) {
                        text = streamReader.ReadToEnd();
                    }
                } catch (Exception) {
                    text = "";
                }
            }
            bool flag2 = text == "";
            if (flag2) {
                text = "ServerAutoStart|GetIpAuto||||";
            }
            return text.Split('|');
        }

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            sets = GetSets();
            SaveSets();
            bool flag = sets[1] == "GetIpAuto";
            if (flag) {
                cbAutoIp.Checked = true;
                tbIp.ReadOnly = true;
            }
            bool flag2 = sets[0] == "ServerAutoStart";
            if (flag2) {
                bStartServer.PerformClick();
            }
            WindowState = FormWindowState.Minimized;
        }

        private void bStartServer_Click(object sender, EventArgs e) {
            Text = "RemoteFork1.2 Запуск сервера..";
            lStatus.Text = "Сервер запущен";
            bool flag = sets[1] == "GetIpAuto";
            tbIp.Text = flag ? Tools.GetIPAddress("") : sets[2];
            bool flag2 = sets[4] == "";
            cbDlna.Checked = flag2;
            try {
                httpServer = new MyHttpServer(IPAddress.Parse(tbIp.Text), int.Parse(tbPort.Text));
                thread = new Thread(httpServer.Listen);
                thread.Start();
            } catch (Exception) {
                Text = "Ошибка!";
                lStatus.Text = "Ошибка!";
            }
        }

        private void bStopServer_Click(object sender, EventArgs e) {
            Text = "Остановка сервера..";
            lStatus.Text = "Остановка сервера..";
            try {
                if (httpServer != null) {
                    httpServer.Stop();
                }
                if (thread != null) {
                    thread.Abort();
                }
            } catch (Exception) {
                Text = "Ошибка!";
                lStatus.Text = "Ошибка!";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if (httpServer != null) {
                httpServer.Stop();
            }
            if (thread != null) {
                thread.Abort();
            }
        }

        private void Form1_Resize(object sender, EventArgs e) {
            bool flag = WindowState == FormWindowState.Minimized;
            if (flag) {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void cbAutoIp_CheckedChanged(object sender, EventArgs e) {
            bool @checked = cbAutoIp.Checked;
            if (@checked) {
                sets[1] = "GetIpAuto";
                tbIp.ReadOnly = true;
                tbIp.Text = Tools.GetIPAddress("");
            } else {
                sets[1] = "GetIpManual";
                tbIp.ReadOnly = false;
                sets[2] = tbIp.Text;
            }
            SaveSets();
        }

        private void tbIp_TextChanged(object sender, EventArgs e) {
            sets[2] = tbIp.Text;
            SaveSets();
        }

        private void cbDlna_CheckedChanged(object sender, EventArgs e) {
            bool @checked = cbDlna.Checked;
            if (@checked) {
                sets[4] = "";
            } else {
                sets[4] = "NoDLNA";
            }
            SaveSets();
        }

        private void Form1_Activated(object sender, EventArgs e) {
            bool flag = act;
            if (flag) {
                notifyIcon1.Visible = true;
                Hide();
                act = false;
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                httpServer.Stop();
                thread.Abort();
            } finally {
                Close();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            bool flag = e.Button == MouseButtons.Left;
            if (flag) {
                Show();
                WindowState = FormWindowState.Normal;
                act = true;
            }
        }

        private void загрузитьПлейлистToolStripMenuItem1_MouseHover(object sender, EventArgs e) {
            Console.WriteLine("read sets");
            sets = GetSets();
            bool flag = sets[3] != "";
            if (flag) {
                string[] array = sets[3].Split('~');
                нетАктивныхУстройствToolStripMenuItem1.Text = array[0] + " (" + array[2] + ")";
            } else {
                нетАктивныхУстройствToolStripMenuItem1.Text = "Нет активных устройств";
            }
        }

        private void нетАктивныхУстройствToolStripMenuItem1_Click(object sender, EventArgs e) {
            sets = GetSets();
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
                                          openFileDialog1.FileName + "&initial=" + sets[3].Replace('~', '|'));
                    webRequest.Method = "POST";
                    webRequest.Timeout = 120000;
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    byte[] bytes = Encoding.GetEncoding(65001).GetBytes("text=" + WebUtility.UrlEncode(text));
                    webRequest.ContentLength = (long) bytes.Length;
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

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e) {
            base.Show();
            WindowState = FormWindowState.Normal;
            act = true;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start("http://" + tbIp.Text + ":" + tbPort.Text + "/test");
        }

        #region Form

        private IContainer components = null;
        private TextBox tbIp;
        private Button bStopServer;
        private Button bStartServer;
        private CheckBox cbAutoIp;
        private OpenFileDialog openFileDialog1;
        private NotifyIcon notifyIcon1;
        private CheckBox cbDlna;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem выходToolStripMenuItem;
        private ToolStripMenuItem загрузитьПлейлистToolStripMenuItem1;
        private ToolStripMenuItem нетАктивныхУстройствToolStripMenuItem1;
        private ToolStripMenuItem настройкиToolStripMenuItem;
        private ToolStripMenuItem открытьToolStripMenuItem;
        private Label IP;
        private TextBox tbPort;
        private Label label1;
        private Label lStatus;

        protected override void Dispose(bool disposing) {
            bool flag = disposing && components != null;
            if (flag) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tbIp = new System.Windows.Forms.TextBox();
            this.bStopServer = new System.Windows.Forms.Button();
            this.bStartServer = new System.Windows.Forms.Button();
            this.cbAutoIp = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.загрузитьПлейлистToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.нетАктивныхУстройствToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.открытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbDlna = new System.Windows.Forms.CheckBox();
            this.IP = new System.Windows.Forms.Label();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lStatus = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbIp
            // 
            this.tbIp.Location = new System.Drawing.Point(49, 64);
            this.tbIp.Name = "tbIp";
            this.tbIp.Size = new System.Drawing.Size(152, 20);
            this.tbIp.TabIndex = 1;
            this.tbIp.TextChanged += new System.EventHandler(this.tbIp_TextChanged);
            // 
            // bStopServer
            // 
            this.bStopServer.Location = new System.Drawing.Point(112, 12);
            this.bStopServer.Name = "bStopServer";
            this.bStopServer.Size = new System.Drawing.Size(94, 23);
            this.bStopServer.TabIndex = 2;
            this.bStopServer.Text = "Stop server";
            this.bStopServer.UseVisualStyleBackColor = true;
            this.bStopServer.Click += new System.EventHandler(this.bStopServer_Click);
            // 
            // bStartServer
            // 
            this.bStartServer.Location = new System.Drawing.Point(12, 12);
            this.bStartServer.Name = "bStartServer";
            this.bStartServer.Size = new System.Drawing.Size(94, 23);
            this.bStartServer.TabIndex = 3;
            this.bStartServer.Text = "Start server";
            this.bStartServer.UseVisualStyleBackColor = true;
            this.bStartServer.Click += new System.EventHandler(this.bStartServer_Click);
            // 
            // cbAutoIp
            // 
            this.cbAutoIp.AutoSize = true;
            this.cbAutoIp.Location = new System.Drawing.Point(12, 41);
            this.cbAutoIp.Name = "cbAutoIp";
            this.cbAutoIp.Size = new System.Drawing.Size(61, 17);
            this.cbAutoIp.TabIndex = 5;
            this.cbAutoIp.Text = "Auto IP";
            this.cbAutoIp.UseVisualStyleBackColor = true;
            this.cbAutoIp.CheckedChanged += new System.EventHandler(this.cbAutoIp_CheckedChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "RemoteFork 1.2";
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.загрузитьПлейлистToolStripMenuItem1,
            this.открытьToolStripMenuItem,
            this.настройкиToolStripMenuItem,
            this.выходToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(184, 92);
            // 
            // загрузитьПлейлистToolStripMenuItem1
            // 
            this.загрузитьПлейлистToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.нетАктивныхУстройствToolStripMenuItem1});
            this.загрузитьПлейлистToolStripMenuItem1.Name = "загрузитьПлейлистToolStripMenuItem1";
            this.загрузитьПлейлистToolStripMenuItem1.Size = new System.Drawing.Size(183, 22);
            this.загрузитьПлейлистToolStripMenuItem1.Text = "Загрузить плейлист";
            this.загрузитьПлейлистToolStripMenuItem1.MouseHover += new System.EventHandler(this.загрузитьПлейлистToolStripMenuItem1_MouseHover);
            // 
            // нетАктивныхУстройствToolStripMenuItem1
            // 
            this.нетАктивныхУстройствToolStripMenuItem1.Name = "нетАктивныхУстройствToolStripMenuItem1";
            this.нетАктивныхУстройствToolStripMenuItem1.Size = new System.Drawing.Size(206, 22);
            this.нетАктивныхУстройствToolStripMenuItem1.Text = "Нет активных устройств";
            this.нетАктивныхУстройствToolStripMenuItem1.Click += new System.EventHandler(this.нетАктивныхУстройствToolStripMenuItem1_Click);
            // 
            // открытьToolStripMenuItem
            // 
            this.открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
            this.открытьToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.открытьToolStripMenuItem.Text = "Открыть тест";
            this.открытьToolStripMenuItem.Click += new System.EventHandler(this.открытьToolStripMenuItem_Click);
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            this.настройкиToolStripMenuItem.Click += new System.EventHandler(this.настройкиToolStripMenuItem_Click);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // cbDlna
            // 
            this.cbDlna.AutoSize = true;
            this.cbDlna.Checked = true;
            this.cbDlna.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDlna.Location = new System.Drawing.Point(12, 116);
            this.cbDlna.Name = "cbDlna";
            this.cbDlna.Size = new System.Drawing.Size(116, 17);
            this.cbDlna.TabIndex = 7;
            this.cbDlna.Text = "RemoteFork DLNA";
            this.cbDlna.UseVisualStyleBackColor = true;
            this.cbDlna.CheckedChanged += new System.EventHandler(this.cbDlna_CheckedChanged);
            // 
            // IP
            // 
            this.IP.AutoSize = true;
            this.IP.Location = new System.Drawing.Point(9, 67);
            this.IP.Name = "IP";
            this.IP.Size = new System.Drawing.Size(20, 13);
            this.IP.TabIndex = 8;
            this.IP.Text = "IP:";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(49, 90);
            this.tbPort.Name = "tbPort";
            this.tbPort.ReadOnly = true;
            this.tbPort.Size = new System.Drawing.Size(152, 20);
            this.tbPort.TabIndex = 1;
            this.tbPort.Text = "8027";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Port:";
            // 
            // lStatus
            // 
            this.lStatus.Location = new System.Drawing.Point(11, 136);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(195, 13);
            this.lStatus.TabIndex = 9;
            this.lStatus.Text = "Status";
            this.lStatus.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 160);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.IP);
            this.Controls.Add(this.cbDlna);
            this.Controls.Add(this.cbAutoIp);
            this.Controls.Add(this.bStartServer);
            this.Controls.Add(this.bStopServer);
            this.Controls.Add(this.tbPort);
            this.Controls.Add(this.tbIp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RemoteFork 1.2";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Form
    }
}
