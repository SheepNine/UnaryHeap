namespace GraphPaper
{
    partial class View
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
            this.wysiwygPanel1 = new UnaryHeap.Utilities.UI.WysiwygPanel();
            this.SuspendLayout();
            // 
            // wysiwygPanel1
            // 
            this.wysiwygPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wysiwygPanel1.Location = new System.Drawing.Point(0, 0);
            this.wysiwygPanel1.Name = "wysiwygPanel1";
            this.wysiwygPanel1.Size = new System.Drawing.Size(284, 261);
            this.wysiwygPanel1.TabIndex = 0;
            this.wysiwygPanel1.Text = "wysiwygPanel1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.wysiwygPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private UnaryHeap.Utilities.UI.WysiwygPanel wysiwygPanel1;
    }
}

