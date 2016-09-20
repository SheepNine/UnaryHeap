namespace Reversi.Forms
{
    partial class Launcher
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
            this.hostButton = new System.Windows.Forms.Button();
            this.joinButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.hostPortTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.connectServerTextBox = new System.Windows.Forms.TextBox();
            this.connectPortTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // hostButton
            // 
            this.hostButton.Location = new System.Drawing.Point(179, 17);
            this.hostButton.Name = "hostButton";
            this.hostButton.Size = new System.Drawing.Size(75, 23);
            this.hostButton.TabIndex = 2;
            this.hostButton.Text = "Host";
            this.hostButton.UseVisualStyleBackColor = true;
            this.hostButton.Click += new System.EventHandler(this.hostButton_Click);
            // 
            // joinButton
            // 
            this.joinButton.Location = new System.Drawing.Point(179, 43);
            this.joinButton.Name = "joinButton";
            this.joinButton.Size = new System.Drawing.Size(75, 23);
            this.joinButton.TabIndex = 4;
            this.joinButton.Text = "Join";
            this.joinButton.UseVisualStyleBackColor = true;
            this.joinButton.Click += new System.EventHandler(this.joinButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.hostPortTextBox);
            this.groupBox1.Controls.Add(this.hostButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 47);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Host a New Game";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.connectPortTextBox);
            this.groupBox2.Controls.Add(this.connectServerTextBox);
            this.groupBox2.Controls.Add(this.joinButton);
            this.groupBox2.Location = new System.Drawing.Point(12, 65);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 73);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Join Existing Game";
            // 
            // hostPortTextBox
            // 
            this.hostPortTextBox.Location = new System.Drawing.Point(53, 19);
            this.hostPortTextBox.MaxLength = 5;
            this.hostPortTextBox.Name = "hostPortTextBox";
            this.hostPortTextBox.Size = new System.Drawing.Size(51, 20);
            this.hostPortTextBox.TabIndex = 1;
            this.hostPortTextBox.Text = "7775";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port:";
            // 
            // connectServerTextBox
            // 
            this.connectServerTextBox.Location = new System.Drawing.Point(53, 19);
            this.connectServerTextBox.Name = "connectServerTextBox";
            this.connectServerTextBox.Size = new System.Drawing.Size(201, 20);
            this.connectServerTextBox.TabIndex = 1;
            this.connectServerTextBox.Text = "localhost";
            // 
            // connectPortTextBox
            // 
            this.connectPortTextBox.Location = new System.Drawing.Point(53, 45);
            this.connectPortTextBox.MaxLength = 5;
            this.connectPortTextBox.Name = "connectPortTextBox";
            this.connectPortTextBox.Size = new System.Drawing.Size(51, 20);
            this.connectPortTextBox.TabIndex = 3;
            this.connectPortTextBox.Text = "7775";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Server:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Port:";
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 149);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Launcher";
            this.Text = "Reversi";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button hostButton;
        private System.Windows.Forms.Button joinButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox hostPortTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox connectPortTextBox;
        private System.Windows.Forms.TextBox connectServerTextBox;
    }
}