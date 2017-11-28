namespace RemoteFork.Forms {
    partial class PlayUrl {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.mbSave = new MetroFramework.Controls.MetroButton();
            this.mbAddLink = new MetroFramework.Controls.MetroButton();
            this.mlvLinks = new MetroFramework.Controls.MetroListView();
            this.mtbNewLink = new MetroFramework.Controls.MetroTextBox();
            this.metroContextMenu1 = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.mbCancel = new MetroFramework.Controls.MetroButton();
            this.metroContextMenu1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mbSave
            // 
            this.mbSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbSave.Location = new System.Drawing.Point(577, 210);
            this.mbSave.Name = "mbSave";
            this.mbSave.Size = new System.Drawing.Size(75, 23);
            this.mbSave.TabIndex = 2;
            this.mbSave.Text = "Сохранить";
            this.mbSave.UseSelectable = true;
            this.mbSave.Click += new System.EventHandler(this.bPlay_Click);
            // 
            // mbAddLink
            // 
            this.mbAddLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbAddLink.Location = new System.Drawing.Point(658, 181);
            this.mbAddLink.Name = "mbAddLink";
            this.mbAddLink.Size = new System.Drawing.Size(75, 23);
            this.mbAddLink.TabIndex = 3;
            this.mbAddLink.Text = "Добавить";
            this.mbAddLink.UseSelectable = true;
            this.mbAddLink.Click += new System.EventHandler(this.mbAddLink_Click);
            // 
            // mlvLinks
            // 
            this.mlvLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mlvLinks.ContextMenuStrip = this.metroContextMenu1;
            this.mlvLinks.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.mlvLinks.FullRowSelect = true;
            this.mlvLinks.Location = new System.Drawing.Point(12, 12);
            this.mlvLinks.Name = "mlvLinks";
            this.mlvLinks.OwnerDraw = true;
            this.mlvLinks.Size = new System.Drawing.Size(721, 163);
            this.mlvLinks.TabIndex = 4;
            this.mlvLinks.UseCompatibleStateImageBehavior = false;
            this.mlvLinks.UseSelectable = true;
            this.mlvLinks.View = System.Windows.Forms.View.List;
            // 
            // mtbNewLink
            // 
            this.mtbNewLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.mtbNewLink.CustomButton.Image = null;
            this.mtbNewLink.CustomButton.Location = new System.Drawing.Point(310, 1);
            this.mtbNewLink.CustomButton.Name = "";
            this.mtbNewLink.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.mtbNewLink.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.mtbNewLink.CustomButton.TabIndex = 1;
            this.mtbNewLink.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.mtbNewLink.CustomButton.UseSelectable = true;
            this.mtbNewLink.CustomButton.Visible = false;
            this.mtbNewLink.Lines = new string[0];
            this.mtbNewLink.Location = new System.Drawing.Point(12, 181);
            this.mtbNewLink.MaxLength = 32767;
            this.mtbNewLink.Name = "mtbNewLink";
            this.mtbNewLink.PasswordChar = '\0';
            this.mtbNewLink.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.mtbNewLink.SelectedText = "";
            this.mtbNewLink.SelectionLength = 0;
            this.mtbNewLink.SelectionStart = 0;
            this.mtbNewLink.ShortcutsEnabled = true;
            this.mtbNewLink.Size = new System.Drawing.Size(640, 23);
            this.mtbNewLink.TabIndex = 5;
            this.mtbNewLink.UseSelectable = true;
            this.mtbNewLink.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.mtbNewLink.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroContextMenu1
            // 
            this.metroContextMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDelete});
            this.metroContextMenu1.Name = "metroContextMenu1";
            this.metroContextMenu1.Size = new System.Drawing.Size(119, 26);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(118, 22);
            this.tsmiDelete.Text = "Удалить";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // mbCancel
            // 
            this.mbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbCancel.Location = new System.Drawing.Point(658, 210);
            this.mbCancel.Name = "mbCancel";
            this.mbCancel.Size = new System.Drawing.Size(75, 23);
            this.mbCancel.TabIndex = 2;
            this.mbCancel.Text = "Отменить";
            this.mbCancel.UseSelectable = true;
            this.mbCancel.Click += new System.EventHandler(this.mbCancel_Click);
            // 
            // PlayUrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 241);
            this.Controls.Add(this.mtbNewLink);
            this.Controls.Add(this.mlvLinks);
            this.Controls.Add(this.mbAddLink);
            this.Controls.Add(this.mbCancel);
            this.Controls.Add(this.mbSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlayUrl";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Пользовательские ссылки";
            this.Load += new System.EventHandler(this.PlayUrl_Load);
            this.metroContextMenu1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private MetroFramework.Controls.MetroButton mbSave;
        private MetroFramework.Controls.MetroButton mbAddLink;
        private MetroFramework.Controls.MetroListView mlvLinks;
        private MetroFramework.Controls.MetroTextBox mtbNewLink;
        private MetroFramework.Controls.MetroContextMenu metroContextMenu1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private MetroFramework.Controls.MetroButton mbCancel;
    }
}