using System;
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

            viewModel.UnsavedChangesBeingDiscarded += viewModel_UnsavedChangesBeingDiscarded;
        }

        void viewModel_UnsavedChangesBeingDiscarded(object sender, CancelEventArgs e)
        {
            e.Cancel = (DialogResult.No == MessageBox.Show(
                "Discard unsaved changes?",
                string.Empty,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2));

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
            viewModel.NewArrangement(64, 64);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                DefaultExt = "arr",
                Filter = "Tile Arrangement Files (*.arr)|*.arr",
                FilterIndex = 0,
                Multiselect = false,
                RestoreDirectory = true,
                Title = "Open File"
            };

            using (dialog)
                if (DialogResult.OK == dialog.ShowDialog())
                    using (var inputStream = dialog.OpenFile())
                        viewModel.OpenArrangement(inputStream);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "Tile Arrangement Files (*.arr)|*.arr",
                FilterIndex = 0,
                Title = "Save File As",
                OverwritePrompt = true,
                AutoUpgradeEnabled = true,
                CheckPathExists = true,
                CreatePrompt = false,
                DefaultExt = "arr",
                RestoreDirectory = true,
            };

            using (dialog)
                if (DialogResult.OK == dialog.ShowDialog())
                    using (var outputStream = dialog.OpenFile())
                        viewModel.SaveArrangement(outputStream);
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
    }
}
