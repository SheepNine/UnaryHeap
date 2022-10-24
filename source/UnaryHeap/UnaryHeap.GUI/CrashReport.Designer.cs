namespace UnaryHeap.GUI
{
    partial class CrashReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.exceptionDetails = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.copyDetailsButton = new System.Windows.Forms.Button();
            this.openGitHubButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // exceptionDetails
            // 
            this.exceptionDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exceptionDetails.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exceptionDetails.Location = new System.Drawing.Point(12, 67);
            this.exceptionDetails.Name = "exceptionDetails";
            this.exceptionDetails.ReadOnly = true;
            this.exceptionDetails.Size = new System.Drawing.Size(760, 482);
            this.exceptionDetails.TabIndex = 0;
            this.exceptionDetails.Text = "";
            this.exceptionDetails.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(467, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "The application you are using has experienced an unhandled exception and has been" +
    " shut down.";
            // 
            // copyDetailsButton
            // 
            this.copyDetailsButton.Location = new System.Drawing.Point(12, 38);
            this.copyDetailsButton.Name = "copyDetailsButton";
            this.copyDetailsButton.Size = new System.Drawing.Size(200, 23);
            this.copyDetailsButton.TabIndex = 2;
            this.copyDetailsButton.Text = "Copy Exception Details to Clipboard";
            this.copyDetailsButton.UseVisualStyleBackColor = true;
            this.copyDetailsButton.Click += new System.EventHandler(this.copyDetailsButton_Click);
            // 
            // openGitHubButton
            // 
            this.openGitHubButton.Location = new System.Drawing.Point(218, 38);
            this.openGitHubButton.Name = "openGitHubButton";
            this.openGitHubButton.Size = new System.Drawing.Size(200, 23);
            this.openGitHubButton.TabIndex = 3;
            this.openGitHubButton.Text = "Open GitHub Issue Reporting Page";
            this.openGitHubButton.UseVisualStyleBackColor = true;
            this.openGitHubButton.Click += new System.EventHandler(this.openGitHubButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(554, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Please take a moment to open an issue in the UnaryHeap GitHub project, so that th" +
    "e exception can be investigated.";
            // 
            // CrashReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.copyDetailsButton);
            this.Controls.Add(this.openGitHubButton);
            this.Controls.Add(this.exceptionDetails);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CrashReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UnaryHeap Crash Reporter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox exceptionDetails;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button copyDetailsButton;
        private System.Windows.Forms.Button openGitHubButton;
        private System.Windows.Forms.Label label2;
    }
}