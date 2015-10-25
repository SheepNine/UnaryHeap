namespace Patchwork
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
        /// <param name="disposing">
        /// true if managed resources should be disposed; otherwise, false.</param>
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
            this.editorPanel = new UnaryHeap.Utilities.UI.WysiwygPanel();
            this.SuspendLayout();
            // 
            // wysiwygPanel1
            // 
            this.editorPanel.Location = new System.Drawing.Point(12, 12);
            this.editorPanel.Name = "wysiwygPanel1";
            this.editorPanel.Size = new System.Drawing.Size(260, 238);
            this.editorPanel.TabIndex = 0;
            this.editorPanel.Text = "wysiwygPanel1";
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.editorPanel);
            this.Name = "View";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private UnaryHeap.Utilities.UI.WysiwygPanel editorPanel;
    }
}

