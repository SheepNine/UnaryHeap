namespace Reversi.Forms
{
    partial class ServerForm
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
            this.spawnClientButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // spawnClientButton
            // 
            this.spawnClientButton.Location = new System.Drawing.Point(12, 12);
            this.spawnClientButton.Name = "spawnClientButton";
            this.spawnClientButton.Size = new System.Drawing.Size(212, 23);
            this.spawnClientButton.TabIndex = 0;
            this.spawnClientButton.Text = "Spawn Client";
            this.spawnClientButton.UseVisualStyleBackColor = true;
            this.spawnClientButton.Click += new System.EventHandler(this.spawnClientButton_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 47);
            this.Controls.Add(this.spawnClientButton);
            this.Name = "ServerForm";
            this.Text = "ServerForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button spawnClientButton;
    }
}