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
            this.components = new System.ComponentModel.Container();
            this.spawnClientButton = new System.Windows.Forms.Button();
            this.outputTextBox = new System.Windows.Forms.RichTextBox();
            this.pocoReader = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // spawnClientButton
            // 
            this.spawnClientButton.Location = new System.Drawing.Point(12, 12);
            this.spawnClientButton.Name = "spawnClientButton";
            this.spawnClientButton.Size = new System.Drawing.Size(108, 23);
            this.spawnClientButton.TabIndex = 0;
            this.spawnClientButton.Text = "Spawn Client";
            this.spawnClientButton.UseVisualStyleBackColor = true;
            this.spawnClientButton.Click += new System.EventHandler(this.spawnClientButton_Click);
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputTextBox.Location = new System.Drawing.Point(12, 41);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(554, 208);
            this.outputTextBox.TabIndex = 2;
            this.outputTextBox.Text = "";
            // 
            // pocoReader
            // 
            this.pocoReader.Tick += new System.EventHandler(this.pocoReader_Tick);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 261);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.spawnClientButton);
            this.Name = "ServerForm";
            this.Text = "ServerForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button spawnClientButton;
        private System.Windows.Forms.RichTextBox outputTextBox;
        private System.Windows.Forms.Timer pocoReader;
    }
}