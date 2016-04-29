using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using RemoteFork.Properties;

namespace RemoteFork {
    public partial class DlnaConfigurate : Form {
        public DlnaConfigurate() {
            InitializeComponent();

            LoadFilteringType();
            LoadDirectories();
        }

        private void bSave_Click(object sender, EventArgs e) {
            SaveFilteringType();
            SaveDirectories();

            Settings.Default.Save();
        }

        private void bCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void LoadDirectories() {
            if (Settings.Default.DlnaDirectories != null && Settings.Default.DlnaDirectories.Count > 0) {
                foreach (var directory in Settings.Default.DlnaDirectories) {
                    lbDirectories.Items.Add(directory);
                }
            }
        }

        private void LoadFilteringType() {
            switch (Settings.Default.DlnaFilterType) {
                case 0:
                    rbAll.Checked = true;
                    break;
                case 1:
                    rbIncludeSelected.Checked = true;
                    break;
                case 2:
                    rbExcludeSelected.Checked = true;
                    break;
            }
        }

        private void SaveDirectories() {
            var collection = new StringCollection();
            foreach (var item in lbDirectories.Items) {
                collection.Add(item.ToString());
            }

            Settings.Default.DlnaDirectories = collection;
        }

        private void SaveFilteringType() {
            if (rbAll.Checked) {
                Settings.Default.DlnaFilterType = 0;
            } else if (rbIncludeSelected.Checked) {
                Settings.Default.DlnaFilterType = 1;
            } else if (rbExcludeSelected.Checked) {
                Settings.Default.DlnaFilterType = 2;
            }
        }

        private void bClearDirectories_Click(object sender, EventArgs e) {
            lbDirectories.Items.Clear();
        }

        private void bRemoveDirectories_Click(object sender, EventArgs e) {
            if (lbDirectories.SelectedIndices.Count > 0) {
                foreach (int id in lbDirectories.SelectedIndices) {
                    lbDirectories.Items.RemoveAt(id);
                }
            }
        }

        private void bAddDirectorues_Click(object sender, EventArgs e) {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                if (!lbDirectories.Items.Contains(folderBrowserDialog1.SelectedPath)) {
                    lbDirectories.Items.Add(folderBrowserDialog1.SelectedPath);
                }
            }
        }
    }
}
