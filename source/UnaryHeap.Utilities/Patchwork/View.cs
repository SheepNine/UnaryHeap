using System;
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

        private void tilesetPanel_SizeChanged(object sender, EventArgs e)
        {
            tilesetPanelBorder.Size = new Size(
                tilesetPanel.Width +
                    (tilesetPanelBorder.Width - tilesetPanelBorder.ClientSize.Width),
                tilesetPanel.Height +
                    (tilesetPanelBorder.Height - tilesetPanelBorder.ClientSize.Height)
                );
        }
    }
}
