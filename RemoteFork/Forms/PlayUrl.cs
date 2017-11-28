using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using RemoteFork.Properties;

namespace RemoteFork.Forms {
    public partial class PlayUrl : MetroFramework.Forms.MetroForm {
        public PlayUrl() {
            InitializeComponent();
        }

        private void bPlay_Click(object sender, EventArgs e) {
            var collection = new StringCollection();
            foreach (ListViewItem url in mlvLinks.Items) {
                collection.Add(url.Text);
            }
            Settings.Default.UserUrls = collection;
            Settings.Default.Save();

            DialogResult = DialogResult.OK;
        }

        private void mbCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;

        }

        private void PlayUrl_Load(object sender, EventArgs e) {
            if (Settings.Default.UserUrls != null) {
                foreach (string url in Settings.Default.UserUrls) {
                    mlvLinks.Items.Add(url);
                }
            }
        }

        private void mbAddLink_Click(object sender, EventArgs e) {
            if (!string.IsNullOrWhiteSpace(mtbNewLink.Text)) {
                mlvLinks.Items.Add(mtbNewLink.Text);
            }
        }

        private void tsmiDelete_Click(object sender, EventArgs e) {

            foreach (ListViewItem item in mlvLinks.SelectedItems) {
                if (Settings.Default.UserUrls.Contains(item.Text)) {
                    Settings.Default.UserUrls.Remove(item.Text);
                }
                mlvLinks.Items.Remove(item);
            }
        }
    }
}