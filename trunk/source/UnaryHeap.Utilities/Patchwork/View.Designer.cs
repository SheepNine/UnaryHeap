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
            this.openRecentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyRenderedArrangementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contractRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contractLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleGridDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeTilesetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.cursorPositionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.editorPanelBorder = new System.Windows.Forms.Panel();
            this.tilesetPanelBorder = new System.Windows.Forms.Panel();
            this.contractBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contractTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tilesetPanel.SizeChanged += new System.EventHandler(this.tilesetPanel_SizeChanged);
            // 
            // tilesetGestures
            // 
            this.tilesetGestures.Target = this.tilesetPanel;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
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
            this.openRecentToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exportToPNGToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
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
            // openRecentToolStripMenuItem
            // 
            this.openRecentToolStripMenuItem.Name = "openRecentToolStripMenuItem";
            this.openRecentToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.openRecentToolStripMenuItem.Text = "Open Recent";
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
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.copyRenderedArrangementToolStripMenuItem,
            this.expandRightToolStripMenuItem,
            this.expandBottomToolStripMenuItem,
            this.expandLeftToolStripMenuItem,
            this.expandTopToolStripMenuItem,
            this.contractRightToolStripMenuItem,
            this.contractBottomToolStripMenuItem,
            this.contractLeftToolStripMenuItem,
            this.contractTopToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // copyRenderedArrangementToolStripMenuItem
            // 
            this.copyRenderedArrangementToolStripMenuItem.Name = "copyRenderedArrangementToolStripMenuItem";
            this.copyRenderedArrangementToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.copyRenderedArrangementToolStripMenuItem.Text = "Copy Rendered Arrangement";
            this.copyRenderedArrangementToolStripMenuItem.Click += new System.EventHandler(this.copyRenderedArrangementToolStripMenuItem_Click);
            // 
            // expandRightToolStripMenuItem
            // 
            this.expandRightToolStripMenuItem.Name = "expandRightToolStripMenuItem";
            this.expandRightToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Right)));
            this.expandRightToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.expandRightToolStripMenuItem.Text = "Expand Right";
            this.expandRightToolStripMenuItem.Click += new System.EventHandler(this.expandRightToolStripMenuItem_Click);
            // 
            // expandBottomToolStripMenuItem
            // 
            this.expandBottomToolStripMenuItem.Name = "expandBottomToolStripMenuItem";
            this.expandBottomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.expandBottomToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.expandBottomToolStripMenuItem.Text = "Expand Bottom";
            this.expandBottomToolStripMenuItem.Click += new System.EventHandler(this.expandBottomToolStripMenuItem_Click);
            // 
            // expandLeftToolStripMenuItem
            // 
            this.expandLeftToolStripMenuItem.Name = "expandLeftToolStripMenuItem";
            this.expandLeftToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Left)));
            this.expandLeftToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.expandLeftToolStripMenuItem.Text = "Expand Left";
            this.expandLeftToolStripMenuItem.Click += new System.EventHandler(this.expandLeftToolStripMenuItem_Click);
            // 
            // expandTopToolStripMenuItem
            // 
            this.expandTopToolStripMenuItem.Name = "expandTopToolStripMenuItem";
            this.expandTopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.expandTopToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.expandTopToolStripMenuItem.Text = "Expand Top";
            this.expandTopToolStripMenuItem.Click += new System.EventHandler(this.expandTopToolStripMenuItem_Click);
            // 
            // contractRightToolStripMenuItem
            // 
            this.contractRightToolStripMenuItem.Name = "contractRightToolStripMenuItem";
            this.contractRightToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Right)));
            this.contractRightToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.contractRightToolStripMenuItem.Text = "Contract Right";
            this.contractRightToolStripMenuItem.Click += new System.EventHandler(this.contractRightToolStripMenuItem_Click);
            // 
            // contractLeftToolStripMenuItem
            // 
            this.contractLeftToolStripMenuItem.Name = "contractLeftToolStripMenuItem";
            this.contractLeftToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Left)));
            this.contractLeftToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.contractLeftToolStripMenuItem.Text = "Contract Left";
            this.contractLeftToolStripMenuItem.Click += new System.EventHandler(this.contractLeftToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.toggleGridDisplayToolStripMenuItem,
            this.changeTilesetToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemplus)));
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.zoomInToolStripMenuItem.Text = "Zoom In";
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.zoomInToolStripMenuItem_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemMinus)));
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom Out";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.zoomOutToolStripMenuItem_Click);
            // 
            // toggleGridDisplayToolStripMenuItem
            // 
            this.toggleGridDisplayToolStripMenuItem.Name = "toggleGridDisplayToolStripMenuItem";
            this.toggleGridDisplayToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.toggleGridDisplayToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.toggleGridDisplayToolStripMenuItem.Text = "Toggle Grid Display";
            this.toggleGridDisplayToolStripMenuItem.Click += new System.EventHandler(this.toggleGridDisplayToolStripMenuItem_Click);
            // 
            // changeTilesetToolStripMenuItem
            // 
            this.changeTilesetToolStripMenuItem.Name = "changeTilesetToolStripMenuItem";
            this.changeTilesetToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.changeTilesetToolStripMenuItem.Text = "Change Tileset...";
            this.changeTilesetToolStripMenuItem.Click += new System.EventHandler(this.changeTilesetToolStripMenuItem_Click);
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
            // contractBottomToolStripMenuItem
            // 
            this.contractBottomToolStripMenuItem.Name = "contractBottomToolStripMenuItem";
            this.contractBottomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Down)));
            this.contractBottomToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.contractBottomToolStripMenuItem.Text = "Contract Bottom";
            this.contractBottomToolStripMenuItem.Click += new System.EventHandler(this.contractBottomToolStripMenuItem_Click);
            // 
            // contractTopToolStripMenuItem
            // 
            this.contractTopToolStripMenuItem.Name = "contractTopToolStripMenuItem";
            this.contractTopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Up)));
            this.contractTopToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.contractTopToolStripMenuItem.Text = "Contract Top";
            this.contractTopToolStripMenuItem.Click += new System.EventHandler(this.contractTopToolStripMenuItem_Click);
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
            this.Text = "Patchwork";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.View_FormClosing);
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
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyRenderedArrangementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeTilesetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRecentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandRightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandBottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandLeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contractRightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contractLeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contractBottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contractTopToolStripMenuItem;
    }
}

