namespace Reversi.Forms
{
    partial class ClientForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientForm));
            this.pocoReader = new System.Windows.Forms.Timer(this.components);
            this.playerOneButton = new System.Windows.Forms.Button();
            this.playerTwoButton = new System.Windows.Forms.Button();
            this.observeButton = new System.Windows.Forms.Button();
            this.setNameButton = new System.Windows.Forms.Button();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.whitePlayerLabel = new System.Windows.Forms.Label();
            this.blackPlayerLabel = new System.Windows.Forms.Label();
            this.observersListBox = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.reversiBoard = new Reversi.Forms.ReversiBoardControl();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pocoReader
            // 
            this.pocoReader.Tick += new System.EventHandler(this.pocoReader_Tick);
            // 
            // playerOneButton
            // 
            this.playerOneButton.Enabled = false;
            this.playerOneButton.Location = new System.Drawing.Point(12, 41);
            this.playerOneButton.Name = "playerOneButton";
            this.playerOneButton.Size = new System.Drawing.Size(75, 23);
            this.playerOneButton.TabIndex = 1;
            this.playerOneButton.Text = "Be White";
            this.playerOneButton.UseVisualStyleBackColor = true;
            this.playerOneButton.Visible = false;
            this.playerOneButton.Click += new System.EventHandler(this.playerOneButton_Click);
            // 
            // playerTwoButton
            // 
            this.playerTwoButton.Enabled = false;
            this.playerTwoButton.Location = new System.Drawing.Point(93, 41);
            this.playerTwoButton.Name = "playerTwoButton";
            this.playerTwoButton.Size = new System.Drawing.Size(75, 23);
            this.playerTwoButton.TabIndex = 2;
            this.playerTwoButton.Text = "Be Black";
            this.playerTwoButton.UseVisualStyleBackColor = true;
            this.playerTwoButton.Visible = false;
            this.playerTwoButton.Click += new System.EventHandler(this.playerTwoButton_Click);
            // 
            // observeButton
            // 
            this.observeButton.Enabled = false;
            this.observeButton.Location = new System.Drawing.Point(174, 41);
            this.observeButton.Name = "observeButton";
            this.observeButton.Size = new System.Drawing.Size(75, 23);
            this.observeButton.TabIndex = 3;
            this.observeButton.Text = "Observe";
            this.observeButton.UseVisualStyleBackColor = true;
            this.observeButton.Visible = false;
            this.observeButton.Click += new System.EventHandler(this.observeButton_Click);
            // 
            // setNameButton
            // 
            this.setNameButton.Location = new System.Drawing.Point(12, 12);
            this.setNameButton.Name = "setNameButton";
            this.setNameButton.Size = new System.Drawing.Size(75, 23);
            this.setNameButton.TabIndex = 4;
            this.setNameButton.Text = "Set Name:";
            this.setNameButton.UseVisualStyleBackColor = true;
            this.setNameButton.Click += new System.EventHandler(this.setNameButton_Click);
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.Location = new System.Drawing.Point(93, 12);
            this.nameTextBox.MaxLength = 16;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(193, 20);
            this.nameTextBox.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 25);
            this.label1.TabIndex = 7;
            this.label1.Text = "White:";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 25);
            this.label2.TabIndex = 8;
            this.label2.Text = "Black:";
            this.label2.Visible = false;
            // 
            // whitePlayerLabel
            // 
            this.whitePlayerLabel.AutoSize = true;
            this.whitePlayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.whitePlayerLabel.Location = new System.Drawing.Point(91, 67);
            this.whitePlayerLabel.Name = "whitePlayerLabel";
            this.whitePlayerLabel.Size = new System.Drawing.Size(195, 25);
            this.whitePlayerLabel.TabIndex = 9;
            this.whitePlayerLabel.Text = "NAMENAMENAME";
            this.whitePlayerLabel.Visible = false;
            // 
            // blackPlayerLabel
            // 
            this.blackPlayerLabel.AutoSize = true;
            this.blackPlayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blackPlayerLabel.Location = new System.Drawing.Point(91, 92);
            this.blackPlayerLabel.Name = "blackPlayerLabel";
            this.blackPlayerLabel.Size = new System.Drawing.Size(195, 25);
            this.blackPlayerLabel.TabIndex = 10;
            this.blackPlayerLabel.Text = "NAMENAMENAME";
            this.blackPlayerLabel.Visible = false;
            // 
            // observersListBox
            // 
            this.observersListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.observersListBox.FormattingEnabled = true;
            this.observersListBox.IntegralHeight = false;
            this.observersListBox.Location = new System.Drawing.Point(3, 16);
            this.observersListBox.Name = "observersListBox";
            this.observersListBox.Size = new System.Drawing.Size(114, 81);
            this.observersListBox.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.observersListBox);
            this.groupBox1.Location = new System.Drawing.Point(292, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(120, 100);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Observers:";
            this.groupBox1.Visible = false;
            // 
            // reversiBoard
            // 
            this.reversiBoard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reversiBoard.HoveredSquare = null;
            this.reversiBoard.IsActivePlayer = false;
            this.reversiBoard.Location = new System.Drawing.Point(12, 120);
            this.reversiBoard.Name = "reversiBoard";
            this.reversiBoard.Size = new System.Drawing.Size(400, 400);
            this.reversiBoard.TabIndex = 6;
            this.reversiBoard.Text = "reversiBoard1";
            this.reversiBoard.Visible = false;
            this.reversiBoard.SquareClicked += new System.EventHandler<Reversi.Forms.SquareClickedEventArgs>(this.reversiBoard_SquareClicked);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 532);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.blackPlayerLabel);
            this.Controls.Add(this.whitePlayerLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.reversiBoard);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.setNameButton);
            this.Controls.Add(this.observeButton);
            this.Controls.Add(this.playerTwoButton);
            this.Controls.Add(this.playerOneButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClientForm";
            this.Text = "Reversi";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer pocoReader;
        private System.Windows.Forms.Button playerOneButton;
        private System.Windows.Forms.Button playerTwoButton;
        private System.Windows.Forms.Button observeButton;
        private System.Windows.Forms.Button setNameButton;
        private System.Windows.Forms.TextBox nameTextBox;
        private ReversiBoardControl reversiBoard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label whitePlayerLabel;
        private System.Windows.Forms.Label blackPlayerLabel;
        private System.Windows.Forms.ListBox observersListBox;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}