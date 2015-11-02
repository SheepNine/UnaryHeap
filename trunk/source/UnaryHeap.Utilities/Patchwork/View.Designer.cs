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
            this.editorGestures = new UnaryHeap.Utilities.UI.GestureInterpreter();
            this.tilesetPanel = new UnaryHeap.Utilities.UI.WysiwygPanel();
            this.tilesetGestures = new UnaryHeap.Utilities.UI.GestureInterpreter();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleGridDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.cursorPositionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.editorPanelBorder = new System.Windows.Forms.Panel();
            this.tilesetPanelBorder = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.editorPanelBorder.SuspendLayout();
            this.tilesetPanelBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // editorPanel
            // 
            this.editorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorPanel.Location = new System.Drawing.Point(0, 0);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Size = new System.Drawing.Size(697, 685);
            this.editorPanel.TabIndex = 0;
            this.editorPanel.Text = "wysiwygPanel1";
            // 
            // editorGestures
            // 
            this.editorGestures.Target = this.editorPanel;
            // 
            // tilesetPanel
            // 
            this.tilesetPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilesetPanel.Location = new System.Drawing.Point(0, 0);
            this.tilesetPanel.Name = "tilesetPanel";
            this.tilesetPanel.Size = new System.Drawing.Size(196, 685);
            this.tilesetPanel.TabIndex = 1;
            // 
            // tilesetGestures
            // 
            this.tilesetGestures.Target = this.tilesetPanel;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(901, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exportToPNGToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.loadToolStripMenuItem.Text = "Open...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exportToPNGToolStripMenuItem
            // 
            this.exportToPNGToolStripMenuItem.Name = "exportToPNGToolStripMenuItem";
            this.exportToPNGToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.exportToPNGToolStripMenuItem.Text = "Export to PNG...";
            this.exportToPNGToolStripMenuItem.Click += new System.EventHandler(this.exportToPNGToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.toggleGridDisplayToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.zoomInToolStripMenuItem.Text = "Zoom In";
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.zoomInToolStripMenuItem_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom Out";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.zoomOutToolStripMenuItem_Click);
            // 
            // toggleGridDisplayToolStripMenuItem
            // 
            this.toggleGridDisplayToolStripMenuItem.Name = "toggleGridDisplayToolStripMenuItem";
            this.toggleGridDisplayToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.toggleGridDisplayToolStripMenuItem.Text = "Toggle Grid Display";
            this.toggleGridDisplayToolStripMenuItem.Click += new System.EventHandler(this.toggleGridDisplayToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cursorPositionLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 713);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(901, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // cursorPositionLabel
            // 
            this.cursorPositionLabel.Name = "cursorPositionLabel";
            this.cursorPositionLabel.Size = new System.Drawing.Size(94, 17);
            this.cursorPositionLabel.Text = "<not initialized>";
            // 
            // editorPanelBorder
            // 
            this.editorPanelBorder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.editorPanelBorder.Controls.Add(this.editorPanel);
            this.editorPanelBorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorPanelBorder.Location = new System.Drawing.Point(200, 24);
            this.editorPanelBorder.Name = "editorPanelBorder";
            this.editorPanelBorder.Size = new System.Drawing.Size(701, 689);
            this.editorPanelBorder.TabIndex = 4;
            // 
            // tilesetPanelBorder
            // 
            this.tilesetPanelBorder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tilesetPanelBorder.Controls.Add(this.tilesetPanel);
            this.tilesetPanelBorder.Dock = System.Windows.Forms.DockStyle.Left;
            this.tilesetPanelBorder.Location = new System.Drawing.Point(0, 24);
            this.tilesetPanelBorder.Name = "tilesetPanelBorder";
            this.tilesetPanelBorder.Size = new System.Drawing.Size(200, 689);
            this.tilesetPanelBorder.TabIndex = 5;
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 735);
            this.Controls.Add(this.editorPanelBorder);
            this.Controls.Add(this.tilesetPanelBorder);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "View";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.editorPanelBorder.ResumeLayout(false);
            this.tilesetPanelBorder.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UnaryHeap.Utilities.UI.WysiwygPanel editorPanel;
        private UnaryHeap.Utilities.UI.GestureInterpreter editorGestures;
        private UnaryHeap.Utilities.UI.WysiwygPanel tilesetPanel;
        private UnaryHeap.Utilities.UI.GestureInterpreter tilesetGestures;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleGridDisplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToPNGToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel cursorPositionLabel;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.Panel editorPanelBorder;
        private System.Windows.Forms.Panel tilesetPanelBorder;
    }
}

