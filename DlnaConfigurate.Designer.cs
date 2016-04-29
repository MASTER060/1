namespace RemoteFork {
    partial class DlnaConfigurate {
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
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.rbIncludeSelected = new System.Windows.Forms.RadioButton();
            this.rbExcludeSelected = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbDirectories = new System.Windows.Forms.ListBox();
            this.bAddDirectorues = new System.Windows.Forms.Button();
            this.bRemoveDirectories = new System.Windows.Forms.Button();
            this.bClearDirectories = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Location = new System.Drawing.Point(6, 19);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(111, 17);
            this.rbAll.TabIndex = 0;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "Без ограничений";
            this.rbAll.UseVisualStyleBackColor = true;
            // 
            // rbIncludeSelected
            // 
            this.rbIncludeSelected.AutoSize = true;
            this.rbIncludeSelected.Location = new System.Drawing.Point(6, 42);
            this.rbIncludeSelected.Name = "rbIncludeSelected";
            this.rbIncludeSelected.Size = new System.Drawing.Size(120, 17);
            this.rbIncludeSelected.TabIndex = 1;
            this.rbIncludeSelected.Text = "Только указанные";
            this.rbIncludeSelected.UseVisualStyleBackColor = true;
            // 
            // rbExcludeSelected
            // 
            this.rbExcludeSelected.AutoSize = true;
            this.rbExcludeSelected.Location = new System.Drawing.Point(6, 65);
            this.rbExcludeSelected.Name = "rbExcludeSelected";
            this.rbExcludeSelected.Size = new System.Drawing.Size(115, 17);
            this.rbExcludeSelected.TabIndex = 2;
            this.rbExcludeSelected.Text = "Кроме указанных";
            this.rbExcludeSelected.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbAll);
            this.groupBox1.Controls.Add(this.rbExcludeSelected);
            this.groupBox1.Controls.Add(this.rbIncludeSelected);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 92);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры фильтрации";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bClearDirectories);
            this.groupBox2.Controls.Add(this.bRemoveDirectories);
            this.groupBox2.Controls.Add(this.bAddDirectorues);
            this.groupBox2.Controls.Add(this.lbDirectories);
            this.groupBox2.Location = new System.Drawing.Point(12, 110);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 227);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Директории";
            // 
            // lbDirectories
            // 
            this.lbDirectories.FormattingEnabled = true;
            this.lbDirectories.Location = new System.Drawing.Point(6, 55);
            this.lbDirectories.Name = "lbDirectories";
            this.lbDirectories.Size = new System.Drawing.Size(248, 160);
            this.lbDirectories.TabIndex = 6;
            // 
            // bAddDirectorues
            // 
            this.bAddDirectorues.Location = new System.Drawing.Point(6, 19);
            this.bAddDirectorues.Name = "bAddDirectorues";
            this.bAddDirectorues.Size = new System.Drawing.Size(75, 23);
            this.bAddDirectorues.TabIndex = 3;
            this.bAddDirectorues.Text = "Добавить";
            this.bAddDirectorues.UseVisualStyleBackColor = true;
            this.bAddDirectorues.Click += new System.EventHandler(this.bAddDirectorues_Click);
            // 
            // bRemoveDirectories
            // 
            this.bRemoveDirectories.Location = new System.Drawing.Point(93, 19);
            this.bRemoveDirectories.Name = "bRemoveDirectories";
            this.bRemoveDirectories.Size = new System.Drawing.Size(75, 23);
            this.bRemoveDirectories.TabIndex = 4;
            this.bRemoveDirectories.Text = "Удалить";
            this.bRemoveDirectories.UseVisualStyleBackColor = true;
            this.bRemoveDirectories.Click += new System.EventHandler(this.bRemoveDirectories_Click);
            // 
            // bClearDirectories
            // 
            this.bClearDirectories.Location = new System.Drawing.Point(179, 19);
            this.bClearDirectories.Name = "bClearDirectories";
            this.bClearDirectories.Size = new System.Drawing.Size(75, 23);
            this.bClearDirectories.TabIndex = 5;
            this.bClearDirectories.Text = "Очистить";
            this.bClearDirectories.UseVisualStyleBackColor = true;
            this.bClearDirectories.Click += new System.EventHandler(this.bClearDirectories_Click);
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(116, 343);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 8;
            this.bSave.Text = "Сохранить";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(197, 343);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 9;
            this.bCancel.Text = "Отмена";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // DlnaConfigurate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 376);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlnaConfigurate";
            this.Text = "DlnaConfigurate";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.RadioButton rbIncludeSelected;
        private System.Windows.Forms.RadioButton rbExcludeSelected;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bClearDirectories;
        private System.Windows.Forms.Button bRemoveDirectories;
        private System.Windows.Forms.Button bAddDirectorues;
        private System.Windows.Forms.ListBox lbDirectories;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}