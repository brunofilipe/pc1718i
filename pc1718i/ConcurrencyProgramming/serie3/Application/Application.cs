using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConcurrencyProgramming.serie3.Application {
    public partial class Application : Form {
        private CancellationTokenSource token;
        private bool IsProcessing;
        public Application() {
            InitializeComponent();
        }

        private void Enter_Click(object sender, EventArgs e) {

        }

        private void Cancel_Click(object sender, EventArgs e) {
            token.Cancel();
            Error("Cancelling...");
        }

        private void Error(string v) {
            throw new NotImplementedException();
        }

        private void Browse_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog()) {
                if (dialog.ShowDialog() == DialogResult.OK) {
                    dir.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
