using System;
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
            viewModel.HookUpToView(editorPanel, editorGestures, tilesetPanel, tilesetGestures);
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
    }
}
