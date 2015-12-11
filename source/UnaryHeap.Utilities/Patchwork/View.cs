﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Patchwork
{
    public partial class View : Form
    {
        ViewModel viewModel;

        public View(ViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            viewModel.HookUpToView(
                editorPanel, editorGestures,
                tilesetPanel, tilesetGestures,
                cursorPositionLabel);
        }

        private void toggleGridDisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ToggleGridDisplay();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ZoomOut();
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ZoomIn();
        }

        private void exportToPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "Portable Network Graphics Files (*.png)|*.png",
                FilterIndex = 0,
                Title = "Select Filename for Export",
                OverwritePrompt = true,
                AutoUpgradeEnabled = true,
                CheckPathExists = true,
                CreatePrompt = false,
                DefaultExt = "png",
                RestoreDirectory = true,
            };

            using (dialog)
                if (DialogResult.OK == dialog.ShowDialog())
                    viewModel.Export(dialog.FileName, ImageFormat.Png);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.NewArrangement();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dest = Prompts.RequestFilenameToLoad();

            if (dest != null)
                viewModel.OpenArrangement(dest);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.SaveArrangement();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dest = Prompts.RequestFilenameToSaveAs();

            if (dest != null)
                viewModel.SaveArrangement(dest);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.Redo();
        }

        private void tilesetPanel_SizeChanged(object sender, EventArgs e)
        {
            tilesetPanelBorder.Size = new Size(
                tilesetPanel.Width +
                    (tilesetPanelBorder.Width - tilesetPanelBorder.ClientSize.Width),
                tilesetPanel.Height +
                    (tilesetPanelBorder.Height - tilesetPanelBorder.ClientSize.Height)
                );
        }

        private void View_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (false == viewModel.CanClose())
                e.Cancel = true;
        }

        private void copyRenderedArrangementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.CopyRenderedArrangement();
        }

        private void changeTilesetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                Filter = "Image Files " +
"(*.gif;*.jpg;*.jpe*;*.png;*.bmp;*.dib;*.tif;*.wmf;*.ras;*.eps;*.pcx;*.pcd;*.tga)" +
"|*.gif;*.jpg;*.jpe*;*.png;*.bmp;*.dib;*.tif;*.wmf;*.ras;*.eps;*.pcx;*.pcd;*.tga",
                FilterIndex = 0,
                Multiselect = false,
                RestoreDirectory = true,
                Title = "Select Tileset Image"
            };

            using (dialog)
                if (DialogResult.OK == dialog.ShowDialog())
                    viewModel.ChangeTileset(dialog.FileName);
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            viewModel.SyncMruList(openRecentToolStripMenuItem);
        }

        private void expandRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ExpandRight();
        }

        private void expandBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ExpandBottom();
        }

        private void expandLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ExpandLeft();
        }

        private void expandTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ExpandTop();
        }

        private void contractRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ContractRight();
        }

        private void contractLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ContractLeft();
        }

        private void contractBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ContractBottom();
        }

        private void contractTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ContractTop();
        }
    }
}