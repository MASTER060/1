namespace RemoteFork.Forms {
    partial class Main {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devicesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.loadPlaylistToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tHVPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemNewVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerR = new System.Windows.Forms.Timer(this.components);
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.tpMain = new System.Windows.Forms.TabPage();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.mtbServerPort = new MetroFramework.Controls.MetroTextBox();
            this.mlServerPort = new MetroFramework.Controls.MetroLabel();
            this.mcbServerIp = new MetroFramework.Controls.MetroComboBox();
            this.mlServerIp = new MetroFramework.Controls.MetroLabel();
            this.mlServerSettings = new MetroFramework.Controls.MetroLabel();
            this.mbStopServer = new MetroFramework.Controls.MetroButton();
            this.mbStartServer = new MetroFramework.Controls.MetroButton();
            this.tpDeveloper = new System.Windows.Forms.TabPage();
            this.metroPanel5 = new MetroFramework.Controls.MetroPanel();
            this.mtbUserAgent = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.mcbLogs = new MetroFramework.Controls.MetroComboBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.tpDLNA = new System.Windows.Forms.TabPage();
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            this.mcbDlna = new MetroFramework.Controls.MetroCheckBox();
            this.tpPlugins = new System.Windows.Forms.TabPage();
            this.metroPanel4 = new MetroFramework.Controls.MetroPanel();
            this.mcbPlugins = new MetroFramework.Controls.MetroCheckBox();
            this.mcbCheckUpdate = new MetroFramework.Controls.MetroToggle();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.mcbAutoStart = new MetroFramework.Controls.MetroToggle();
            this.mcbThvpAutoStart = new MetroFramework.Controls.MetroToggle();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.mcbUseProxy = new MetroFramework.Controls.MetroToggle();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.metroTabControl1.SuspendLayout();
            this.tpMain.SuspendLayout();
            this.metroPanel1.SuspendLayout();
            this.metroPanel2.SuspendLayout();
            this.tpDeveloper.SuspendLayout();
            this.metroPanel5.SuspendLayout();
            this.tpDLNA.SuspendLayout();
            this.metroPanel3.SuspendLayout();
            this.tpPlugins.SuspendLayout();
            this.metroPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.exitToolStripMenuItem.Text = "Выход";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.settingsToolStripMenuItem.Text = "Настройки";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // openTestToolStripMenuItem
            // 
            this.openTestToolStripMenuItem.Name = "openTestToolStripMenuItem";
            this.openTestToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.openTestToolStripMenuItem.Text = "Открыть тест";
            this.openTestToolStripMenuItem.Click += new System.EventHandler(this.openTestToolStripMenuItem_Click);
            // 
            // devicesToolStripMenuItem1
            // 
            this.devicesToolStripMenuItem1.Name = "devicesToolStripMenuItem1";
            this.devicesToolStripMenuItem1.Size = new System.Drawing.Size(206, 22);
            this.devicesToolStripMenuItem1.Text = "Нет активных устройств";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPlaylistToolStripMenuItem1,
            this.pluginsToolStripMenuItem,
            this.tHVPToolStripMenuItem,
            this.playUrlToolStripMenuItem,
            this.openTestToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem,
            this.MenuItemNewVersion});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(221, 180);
            // 
            // loadPlaylistToolStripMenuItem1
            // 
            this.loadPlaylistToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.devicesToolStripMenuItem1});
            this.loadPlaylistToolStripMenuItem1.Name = "loadPlaylistToolStripMenuItem1";
            this.loadPlaylistToolStripMenuItem1.Size = new System.Drawing.Size(220, 22);
            this.loadPlaylistToolStripMenuItem1.Text = "Загрузить плейлист";
            this.loadPlaylistToolStripMenuItem1.DropDownOpening += new System.EventHandler(this.loadPlaylistToolStripMenuItem1_DropDownOpening);
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            this.pluginsToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.pluginsToolStripMenuItem.Text = "Плагины";
            this.pluginsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.pluginsToolStripMenuItem_DropDownOpening);
            // 
            // tHVPToolStripMenuItem
            // 
            this.tHVPToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.gotoToolStripMenuItem});
            this.tHVPToolStripMenuItem.Name = "tHVPToolStripMenuItem";
            this.tHVPToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.tHVPToolStripMenuItem.Text = "THVP BitTorrent ";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.runToolStripMenuItem.Text = "Запуск THVP";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // gotoToolStripMenuItem
            // 
            this.gotoToolStripMenuItem.Name = "gotoToolStripMenuItem";
            this.gotoToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.gotoToolStripMenuItem.Text = "http://thvp.ru/about";
            this.gotoToolStripMenuItem.Click += new System.EventHandler(this.gotoToolStripMenuItem_Click);
            // 
            // playUrlToolStripMenuItem
            // 
            this.playUrlToolStripMenuItem.Name = "playUrlToolStripMenuItem";
            this.playUrlToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.playUrlToolStripMenuItem.Text = "Пользовательские ссылки";
            this.playUrlToolStripMenuItem.Click += new System.EventHandler(this.playUrlToolStripMenuItem_Click);
            // 
            // MenuItemNewVersion
            // 
            this.MenuItemNewVersion.Name = "MenuItemNewVersion";
            this.MenuItemNewVersion.Size = new System.Drawing.Size(220, 22);
            this.MenuItemNewVersion.Text = "Скачать новую версию!";
            this.MenuItemNewVersion.Visible = false;
            this.MenuItemNewVersion.Click += new System.EventHandler(this.MenuItemNewVersion_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "RemoteFork";
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "playlist.m3u";
            this.openFileDialog1.Filter = "Playlist (*.m3u)|*.m3u|All files (*.*)|*.*";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(20, 423);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(299, 22);
            this.statusStrip1.TabIndex = 19;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(114, 17);
            this.toolStripStatusLabel1.Text = "Сервер остановлен";
            // 
            // timerR
            // 
            this.timerR.Interval = 5000;
            this.timerR.Tick += new System.EventHandler(this.timerR_Tick);
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.tpMain);
            this.metroTabControl1.Controls.Add(this.tpDeveloper);
            this.metroTabControl1.Controls.Add(this.tpDLNA);
            this.metroTabControl1.Controls.Add(this.tpPlugins);
            this.metroTabControl1.Location = new System.Drawing.Point(20, 63);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(297, 357);
            this.metroTabControl1.TabIndex = 0;
            this.metroTabControl1.UseSelectable = true;
            // 
            // tpMain
            // 
            this.tpMain.Controls.Add(this.metroPanel1);
            this.tpMain.Location = new System.Drawing.Point(4, 38);
            this.tpMain.Name = "tpMain";
            this.tpMain.Size = new System.Drawing.Size(289, 315);
            this.tpMain.TabIndex = 0;
            this.tpMain.Text = "Управление";
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.metroLabel5);
            this.metroPanel1.Controls.Add(this.mcbThvpAutoStart);
            this.metroPanel1.Controls.Add(this.mcbAutoStart);
            this.metroPanel1.Controls.Add(this.metroLabel4);
            this.metroPanel1.Controls.Add(this.metroLabel3);
            this.metroPanel1.Controls.Add(this.mcbCheckUpdate);
            this.metroPanel1.Controls.Add(this.metroPanel2);
            this.metroPanel1.Controls.Add(this.mbStopServer);
            this.metroPanel1.Controls.Add(this.mbStartServer);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(0, 0);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(289, 315);
            this.metroPanel1.TabIndex = 4;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // metroPanel2
            // 
            this.metroPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.metroPanel2.Controls.Add(this.metroLabel6);
            this.metroPanel2.Controls.Add(this.mcbUseProxy);
            this.metroPanel2.Controls.Add(this.mtbServerPort);
            this.metroPanel2.Controls.Add(this.mlServerPort);
            this.metroPanel2.Controls.Add(this.mcbServerIp);
            this.metroPanel2.Controls.Add(this.mlServerIp);
            this.metroPanel2.Controls.Add(this.mlServerSettings);
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 10;
            this.metroPanel2.Location = new System.Drawing.Point(5, 78);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(279, 101);
            this.metroPanel2.TabIndex = 4;
            this.metroPanel2.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // mtbServerPort
            // 
            // 
            // 
            // 
            this.mtbServerPort.CustomButton.Image = null;
            this.mtbServerPort.CustomButton.Location = new System.Drawing.Point(119, 1);
            this.mtbServerPort.CustomButton.Name = "";
            this.mtbServerPort.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.mtbServerPort.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.mtbServerPort.CustomButton.TabIndex = 1;
            this.mtbServerPort.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.mtbServerPort.CustomButton.UseSelectable = true;
            this.mtbServerPort.CustomButton.Visible = false;
            this.mtbServerPort.Lines = new string[] {
        "8027"};
            this.mtbServerPort.Location = new System.Drawing.Point(48, 49);
            this.mtbServerPort.MaxLength = 32767;
            this.mtbServerPort.Name = "mtbServerPort";
            this.mtbServerPort.PasswordChar = '\0';
            this.mtbServerPort.ReadOnly = true;
            this.mtbServerPort.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.mtbServerPort.SelectedText = "";
            this.mtbServerPort.SelectionLength = 0;
            this.mtbServerPort.SelectionStart = 0;
            this.mtbServerPort.ShortcutsEnabled = true;
            this.mtbServerPort.Size = new System.Drawing.Size(141, 23);
            this.mtbServerPort.TabIndex = 6;
            this.mtbServerPort.Text = "8027";
            this.mtbServerPort.UseSelectable = true;
            this.mtbServerPort.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.mtbServerPort.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // mlServerPort
            // 
            this.mlServerPort.AutoSize = true;
            this.mlServerPort.Location = new System.Drawing.Point(3, 49);
            this.mlServerPort.Name = "mlServerPort";
            this.mlServerPort.Size = new System.Drawing.Size(37, 19);
            this.mlServerPort.TabIndex = 5;
            this.mlServerPort.Text = "Port:";
            // 
            // mcbServerIp
            // 
            this.mcbServerIp.FormattingEnabled = true;
            this.mcbServerIp.ItemHeight = 23;
            this.mcbServerIp.Location = new System.Drawing.Point(48, 17);
            this.mcbServerIp.Name = "mcbServerIp";
            this.mcbServerIp.Size = new System.Drawing.Size(141, 29);
            this.mcbServerIp.TabIndex = 4;
            this.mcbServerIp.UseSelectable = true;
            this.mcbServerIp.SelectedIndexChanged += new System.EventHandler(this.cbSeverIp_SelectedIndexChanged);
            // 
            // mlServerIp
            // 
            this.mlServerIp.AutoSize = true;
            this.mlServerIp.Location = new System.Drawing.Point(3, 20);
            this.mlServerIp.Name = "mlServerIp";
            this.mlServerIp.Size = new System.Drawing.Size(23, 19);
            this.mlServerIp.TabIndex = 3;
            this.mlServerIp.Text = "IP:";
            // 
            // mlServerSettings
            // 
            this.mlServerSettings.AutoSize = true;
            this.mlServerSettings.Location = new System.Drawing.Point(3, 0);
            this.mlServerSettings.Name = "mlServerSettings";
            this.mlServerSettings.Size = new System.Drawing.Size(47, 19);
            this.mlServerSettings.TabIndex = 2;
            this.mlServerSettings.Text = "Server";
            // 
            // mbStopServer
            // 
            this.mbStopServer.Location = new System.Drawing.Point(190, 4);
            this.mbStopServer.Name = "mbStopServer";
            this.mbStopServer.Size = new System.Drawing.Size(94, 23);
            this.mbStopServer.TabIndex = 1;
            this.mbStopServer.Text = "Остановить";
            this.mbStopServer.UseSelectable = true;
            this.mbStopServer.Click += new System.EventHandler(this.bStopServer_Click);
            // 
            // mbStartServer
            // 
            this.mbStartServer.Location = new System.Drawing.Point(8, 4);
            this.mbStartServer.Name = "mbStartServer";
            this.mbStartServer.Size = new System.Drawing.Size(94, 23);
            this.mbStartServer.TabIndex = 0;
            this.mbStartServer.Text = "Запустить";
            this.mbStartServer.UseSelectable = true;
            this.mbStartServer.Click += new System.EventHandler(this.bStartServer_Click);
            // 
            // tpDeveloper
            // 
            this.tpDeveloper.Controls.Add(this.metroPanel5);
            this.tpDeveloper.Location = new System.Drawing.Point(4, 38);
            this.tpDeveloper.Name = "tpDeveloper";
            this.tpDeveloper.Size = new System.Drawing.Size(289, 315);
            this.tpDeveloper.TabIndex = 4;
            this.tpDeveloper.Text = "Разработка";
            // 
            // metroPanel5
            // 
            this.metroPanel5.Controls.Add(this.mtbUserAgent);
            this.metroPanel5.Controls.Add(this.metroLabel2);
            this.metroPanel5.Controls.Add(this.mcbLogs);
            this.metroPanel5.Controls.Add(this.metroLabel1);
            this.metroPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel5.HorizontalScrollbarBarColor = true;
            this.metroPanel5.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel5.HorizontalScrollbarSize = 10;
            this.metroPanel5.Location = new System.Drawing.Point(0, 0);
            this.metroPanel5.Name = "metroPanel5";
            this.metroPanel5.Size = new System.Drawing.Size(289, 315);
            this.metroPanel5.TabIndex = 0;
            this.metroPanel5.VerticalScrollbarBarColor = true;
            this.metroPanel5.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel5.VerticalScrollbarSize = 10;
            // 
            // mtbUserAgent
            // 
            // 
            // 
            // 
            this.mtbUserAgent.CustomButton.Image = null;
            this.mtbUserAgent.CustomButton.Location = new System.Drawing.Point(160, 2);
            this.mtbUserAgent.CustomButton.Name = "";
            this.mtbUserAgent.CustomButton.Size = new System.Drawing.Size(117, 117);
            this.mtbUserAgent.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.mtbUserAgent.CustomButton.TabIndex = 1;
            this.mtbUserAgent.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.mtbUserAgent.CustomButton.UseSelectable = true;
            this.mtbUserAgent.CustomButton.Visible = false;
            this.mtbUserAgent.Lines = new string[0];
            this.mtbUserAgent.Location = new System.Drawing.Point(3, 62);
            this.mtbUserAgent.MaxLength = 32767;
            this.mtbUserAgent.Multiline = true;
            this.mtbUserAgent.Name = "mtbUserAgent";
            this.mtbUserAgent.PasswordChar = '\0';
            this.mtbUserAgent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mtbUserAgent.SelectedText = "";
            this.mtbUserAgent.SelectionLength = 0;
            this.mtbUserAgent.SelectionStart = 0;
            this.mtbUserAgent.ShortcutsEnabled = true;
            this.mtbUserAgent.Size = new System.Drawing.Size(280, 122);
            this.mtbUserAgent.TabIndex = 5;
            this.mtbUserAgent.UseSelectable = true;
            this.mtbUserAgent.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.mtbUserAgent.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.mtbUserAgent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbUserAgent_KeyDown);
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(3, 40);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(78, 19);
            this.metroLabel2.TabIndex = 4;
            this.metroLabel2.Text = "User-Agent:";
            // 
            // mcbLogs
            // 
            this.mcbLogs.FormattingEnabled = true;
            this.mcbLogs.ItemHeight = 23;
            this.mcbLogs.Items.AddRange(new object[] {
            "Выключить",
            "Информация",
            "Ошибки",
            "Отладка"});
            this.mcbLogs.Location = new System.Drawing.Point(104, 3);
            this.mcbLogs.Name = "mcbLogs";
            this.mcbLogs.Size = new System.Drawing.Size(179, 29);
            this.mcbLogs.TabIndex = 3;
            this.mcbLogs.UseSelectable = true;
            this.mcbLogs.SelectedIndexChanged += new System.EventHandler(this.cbLogs_SelectedIndexChanged);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(3, 7);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(95, 19);
            this.metroLabel1.TabIndex = 2;
            this.metroLabel1.Text = "Логирование:";
            // 
            // tpDLNA
            // 
            this.tpDLNA.Controls.Add(this.metroPanel3);
            this.tpDLNA.Location = new System.Drawing.Point(4, 38);
            this.tpDLNA.Name = "tpDLNA";
            this.tpDLNA.Size = new System.Drawing.Size(289, 315);
            this.tpDLNA.TabIndex = 2;
            this.tpDLNA.Text = "DLNA";
            // 
            // metroPanel3
            // 
            this.metroPanel3.Controls.Add(this.mcbDlna);
            this.metroPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(0, 0);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Size = new System.Drawing.Size(289, 315);
            this.metroPanel3.TabIndex = 0;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // mcbDlna
            // 
            this.mcbDlna.AutoSize = true;
            this.mcbDlna.Checked = true;
            this.mcbDlna.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mcbDlna.Location = new System.Drawing.Point(3, 3);
            this.mcbDlna.Name = "mcbDlna";
            this.mcbDlna.Size = new System.Drawing.Size(153, 15);
            this.mcbDlna.TabIndex = 2;
            this.mcbDlna.Text = "Включить DLNA сервер";
            this.mcbDlna.UseSelectable = true;
            this.mcbDlna.CheckedChanged += new System.EventHandler(this.cbDlna_CheckedChanged);
            // 
            // tpPlugins
            // 
            this.tpPlugins.Controls.Add(this.metroPanel4);
            this.tpPlugins.Location = new System.Drawing.Point(4, 38);
            this.tpPlugins.Name = "tpPlugins";
            this.tpPlugins.Size = new System.Drawing.Size(289, 315);
            this.tpPlugins.TabIndex = 3;
            this.tpPlugins.Text = "Плагины";
            // 
            // metroPanel4
            // 
            this.metroPanel4.Controls.Add(this.mcbPlugins);
            this.metroPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel4.HorizontalScrollbarBarColor = true;
            this.metroPanel4.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel4.HorizontalScrollbarSize = 10;
            this.metroPanel4.Location = new System.Drawing.Point(0, 0);
            this.metroPanel4.Name = "metroPanel4";
            this.metroPanel4.Size = new System.Drawing.Size(289, 315);
            this.metroPanel4.TabIndex = 0;
            this.metroPanel4.VerticalScrollbarBarColor = true;
            this.metroPanel4.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel4.VerticalScrollbarSize = 10;
            // 
            // mcbPlugins
            // 
            this.mcbPlugins.AutoSize = true;
            this.mcbPlugins.Checked = true;
            this.mcbPlugins.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mcbPlugins.Location = new System.Drawing.Point(3, 3);
            this.mcbPlugins.Name = "mcbPlugins";
            this.mcbPlugins.Size = new System.Drawing.Size(129, 15);
            this.mcbPlugins.TabIndex = 2;
            this.mcbPlugins.Text = "Включить плагины";
            this.mcbPlugins.UseSelectable = true;
            this.mcbPlugins.CheckedChanged += new System.EventHandler(this.cbPlugins_CheckedChanged);
            // 
            // mcbCheckUpdate
            // 
            this.mcbCheckUpdate.AutoSize = true;
            this.mcbCheckUpdate.Checked = true;
            this.mcbCheckUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mcbCheckUpdate.Location = new System.Drawing.Point(5, 185);
            this.mcbCheckUpdate.Name = "mcbCheckUpdate";
            this.mcbCheckUpdate.Size = new System.Drawing.Size(80, 17);
            this.mcbCheckUpdate.TabIndex = 6;
            this.mcbCheckUpdate.Text = "On";
            this.mcbCheckUpdate.UseSelectable = true;
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(91, 183);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(154, 19);
            this.metroLabel3.TabIndex = 7;
            this.metroLabel3.Text = "Проверять обновления";
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(95, 31);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(195, 19);
            this.metroLabel4.TabIndex = 8;
            this.metroLabel4.Text = "Запускать при старте Windows";
            // 
            // mcbAutoStart
            // 
            this.mcbAutoStart.AutoSize = true;
            this.mcbAutoStart.Checked = true;
            this.mcbAutoStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mcbAutoStart.Location = new System.Drawing.Point(5, 33);
            this.mcbAutoStart.Name = "mcbAutoStart";
            this.mcbAutoStart.Size = new System.Drawing.Size(80, 17);
            this.mcbAutoStart.TabIndex = 9;
            this.mcbAutoStart.Text = "On";
            this.mcbAutoStart.UseSelectable = true;
            this.mcbAutoStart.CheckedChanged += new System.EventHandler(this.cbAutoStart_CheckedChanged);
            // 
            // mcbThvpAutoStart
            // 
            this.mcbThvpAutoStart.AutoSize = true;
            this.mcbThvpAutoStart.Checked = true;
            this.mcbThvpAutoStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mcbThvpAutoStart.Location = new System.Drawing.Point(5, 55);
            this.mcbThvpAutoStart.Name = "mcbThvpAutoStart";
            this.mcbThvpAutoStart.Size = new System.Drawing.Size(80, 17);
            this.mcbThvpAutoStart.TabIndex = 10;
            this.mcbThvpAutoStart.Text = "On";
            this.mcbThvpAutoStart.UseSelectable = true;
            this.mcbThvpAutoStart.CheckedChanged += new System.EventHandler(this.cbThvpAutoStart_CheckedChanged);
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(91, 53);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(165, 19);
            this.metroLabel5.TabIndex = 11;
            this.metroLabel5.Text = "Автостарт THVP BitTorrent";
            // 
            // mcbUseProxy
            // 
            this.mcbUseProxy.AutoSize = true;
            this.mcbUseProxy.Location = new System.Drawing.Point(3, 78);
            this.mcbUseProxy.Name = "mcbUseProxy";
            this.mcbUseProxy.Size = new System.Drawing.Size(80, 17);
            this.mcbUseProxy.TabIndex = 8;
            this.mcbUseProxy.Text = "Off";
            this.mcbUseProxy.UseSelectable = true;
            this.mcbUseProxy.CheckedChanged += new System.EventHandler(this.cbUseProxy_CheckedChanged);
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Location = new System.Drawing.Point(89, 76);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(154, 19);
            this.metroLabel6.TabIndex = 9;
            this.metroLabel6.Text = "Через внешний сервер";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 465);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.metroTabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "RemoteFork";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.Resize += new System.EventHandler(this.Main_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.metroTabControl1.ResumeLayout(false);
            this.tpMain.ResumeLayout(false);
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel1.PerformLayout();
            this.metroPanel2.ResumeLayout(false);
            this.metroPanel2.PerformLayout();
            this.tpDeveloper.ResumeLayout(false);
            this.metroPanel5.ResumeLayout(false);
            this.metroPanel5.PerformLayout();
            this.tpDLNA.ResumeLayout(false);
            this.metroPanel3.ResumeLayout(false);
            this.metroPanel3.PerformLayout();
            this.tpPlugins.ResumeLayout(false);
            this.metroPanel4.ResumeLayout(false);
            this.metroPanel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem devicesToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadPlaylistToolStripMenuItem1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem pluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playUrlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tHVPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gotoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuItemNewVersion;
        private System.Windows.Forms.Timer timerR;
        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private System.Windows.Forms.TabPage tpMain;
        private System.Windows.Forms.TabPage tpDLNA;
        private System.Windows.Forms.TabPage tpPlugins;
        private System.Windows.Forms.TabPage tpDeveloper;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroTextBox mtbServerPort;
        private MetroFramework.Controls.MetroLabel mlServerPort;
        private MetroFramework.Controls.MetroComboBox mcbServerIp;
        private MetroFramework.Controls.MetroLabel mlServerIp;
        private MetroFramework.Controls.MetroLabel mlServerSettings;
        private MetroFramework.Controls.MetroButton mbStopServer;
        private MetroFramework.Controls.MetroButton mbStartServer;
        private MetroFramework.Controls.MetroPanel metroPanel5;
        private MetroFramework.Controls.MetroPanel metroPanel3;
        private MetroFramework.Controls.MetroCheckBox mcbDlna;
        private MetroFramework.Controls.MetroPanel metroPanel4;
        private MetroFramework.Controls.MetroCheckBox mcbPlugins;
        private MetroFramework.Controls.MetroTextBox mtbUserAgent;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroComboBox mcbLogs;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroToggle mcbThvpAutoStart;
        private MetroFramework.Controls.MetroToggle mcbAutoStart;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroToggle mcbCheckUpdate;
        private MetroFramework.Controls.MetroLabel metroLabel6;
        private MetroFramework.Controls.MetroToggle mcbUseProxy;
    }
}