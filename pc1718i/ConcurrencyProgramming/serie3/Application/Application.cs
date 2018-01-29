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
using ConcurrencyProgramming.serie3.FileSearcher;

namespace ConcurrencyProgramming.serie3.Application {
    public partial class Application : Form {
        private readonly SynchronizationContext _synchronizationContext;
        private CancellationTokenSource _token;
        private bool _isProcessing;
        private readonly IProgress<ResultContainer> _progress;

        public Application() {
            InitializeComponent();
            _synchronizationContext = SynchronizationContext.Current;
            _progress = new Progress<ResultContainer>(UpdateProgressBar);
        }

        private async void Enter_Click(object sender, EventArgs e) {
            if (_isProcessing) {
                DisplayError("Search already in progress..");
                return;
            }
            if (string.IsNullOrEmpty(dir.Text)) {
                DisplayError("Missing parameters..");
                return;
            }
            _token = new CancellationTokenSource();
            _isProcessing = true;
            ResetResults();
            await Search.ParallelGetBiggestFilesAsync(dir.Text, (int) number.Value, _token.Token, _progress)
                .ContinueWith(r => {
                    _isProcessing = false;
                    if (r.IsCanceled) {
                        DisplayError("Cancelled");
                    }
                    else if (r.IsFaulted) {
                        DisplayError("An error occurred! -> " + r.Exception);
                    }
                    else {
                        DisplayResults(r.Result);
                    }
                });
        }

        private void Cancel_Click(object sender, EventArgs e) {
            if (_isProcessing) {
                _token.Cancel();
                DisplayError("Cancelling...");
            }
        }

        private void Browse_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog()) {
                if (dialog.ShowDialog() == DialogResult.OK) {
                    dir.Text = dialog.SelectedPath;
                }
            }
        }

        private void ResetResults() {
            _synchronizationContext.Post(state => {
                displayLabel.Visible = false;
                progressBar.Visible = false;
                progressBar.Value = 0;
                filePaths.Enabled = false;
                filePaths.Items.Clear();
            }, null);
        }

        private async void DisplayError(string m, int time = 3000) {
            _synchronizationContext.Post(message => {
                displayLabel.ForeColor = Color.Red;
                displayLabel.Text = (string)message;
                displayLabel.Visible = true;
                progressBar.Visible = false;
                progressBar.Value = 0;
            }, m);
            if (time != Timeout.Infinite) {
                await Task.Factory.StartNew(() => {
                    Thread.Sleep(time);
                    _synchronizationContext.Send(state => { displayLabel.Visible = false; }, null);
                });
            }
        }

        private void UpdateProgressBar(ResultContainer resultContainer) {
            _synchronizationContext.Post(state => {
                ResultContainer result = state as ResultContainer;
                if (result == null)
                    throw new ArgumentException();

                progressBar.Visible = true;
                progressBar.Maximum = result.FilesCount + 1;
                progressBar.Value += 1;

            }, resultContainer);
        }

        private void DisplayResults(ResultContainer resultContainer) {
            _synchronizationContext.Post(state => {
                ResultContainer result = state as ResultContainer;
                if (result == null)
                    throw new ArgumentException();

                progressBar.Visible = true;
                progressBar.Maximum = result.FilesCount+1;
                progressBar.Value += 1;

                filePaths.Enabled = true;
                /*
                List<string> files = result.GetFiles();
                if (filePaths.Items.Count == 0) {
                    foreach (string file in files) {
                        filePaths.Items.Add(new ListViewItem(file));
                    }
                    return;
                }
                foreach (ListViewItem item in filePaths.Items) {
                    if (!files.Contains(item.Text)) {
                        foreach (string file in files) {
                            ListViewItem newItem = new ListViewItem(file);
                            if (!filePaths.Items.Contains(newItem)) {
                                filePaths.Items.Remove(item);
                                filePaths.Items.Add(newItem);
                                break;
                            }
                        }
                        break;
                    }
                        filePaths.Items.Remove(item);
                }*/
                /*
                foreach (string s in files) {
                    if(filePaths.Items.)
                }

                filePaths.Items.Clear();*/
                foreach (string file in result.GetFiles()) {
                    filePaths.Items.Add(new ListViewItem(file.Substring(dir.Text.Length)));
                }
            }, resultContainer);
        }
    }
}