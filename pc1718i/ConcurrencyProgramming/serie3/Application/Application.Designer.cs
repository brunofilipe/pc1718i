namespace ConcurrencyProgramming.serie3.Application {
    partial class Application {
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
            this.dir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.enter = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.displayLabel = new System.Windows.Forms.Label();
            this.filePaths = new System.Windows.Forms.ListView();
            this.Path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.browse = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.number = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.number)).BeginInit();
            this.SuspendLayout();
            // 
            // dir
            // 
            this.dir.Location = new System.Drawing.Point(71, 26);
            this.dir.Name = "dir";
            this.dir.Size = new System.Drawing.Size(141, 20);
            this.dir.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Directory";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Number of Files";
            // 
            // number
            // 
            this.number.Location = new System.Drawing.Point(111, 53);
            this.number.Maximum = new System.Decimal(new int[] {
                                    100,
                                    0,
                                    0,
                                    0});
            this.number.Name = "number";
            this.number.Size = new System.Drawing.Size(120, 20);
            this.number.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 24;
            // 
            // enter
            // 
            this.enter.Location = new System.Drawing.Point(143, 90);
            this.enter.Name = "enter";
            this.enter.Size = new System.Drawing.Size(67, 23);
            this.enter.TabIndex = 6;
            this.enter.Text = "Enter";
            this.enter.UseVisualStyleBackColor = true;
            this.enter.Click += new System.EventHandler(this.Enter_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(69, 90);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(68, 23);
            this.cancel.TabIndex = 7;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // displayLabel
            // 
            this.displayLabel.AutoSize = true;
            this.displayLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.displayLabel.ForeColor = System.Drawing.Color.Chocolate;
            this.displayLabel.Location = new System.Drawing.Point(12, 318);
            this.displayLabel.Name = "displayLabel";
            this.displayLabel.Size = new System.Drawing.Size(163, 15);
            this.displayLabel.TabIndex = 8;
            this.displayLabel.Text = "Error Message";
            this.displayLabel.Visible = false;

            // 
            // filePaths
            // 
            this.filePaths.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Path});
            this.filePaths.Enabled = false;
            this.filePaths.LabelWrap = false;
            this.filePaths.Location = new System.Drawing.Point(6, 119);
            this.filePaths.Name = "filePaths";
            this.filePaths.ShowGroups = false;
            this.filePaths.Size = new System.Drawing.Size(303, 167);
            this.filePaths.TabIndex = 20;
            this.filePaths.UseCompatibleStateImageBehavior = false;
            this.filePaths.View = System.Windows.Forms.View.Details;
            // 
            // Path
            // 
            this.Path.Tag = "paths";
            this.Path.Text = "Path";
            this.Path.Width = 272;
            // 
            // browse
            // 
            this.browse.Location = new System.Drawing.Point(218, 27);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(91, 20);
            this.browse.TabIndex = 21;
            this.browse.Text = "Browse";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(6, 292);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(303, 23);
            this.progressBar.TabIndex = 22;
            this.progressBar.Visible = false;
            // 
            // Application
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(373, 347);
            this.Controls.Add(this.number);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.browse);
            this.Controls.Add(this.filePaths);
            this.Controls.Add(this.displayLabel);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.enter);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Application";
            this.Text = "Search Biggest Files";
            ((System.ComponentModel.ISupportInitialize)(this.number)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.TextBox dir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button enter;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label displayLabel;
        private System.Windows.Forms.ListView filePaths;
        private System.Windows.Forms.ColumnHeader Path;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.NumericUpDown number;
    }
}