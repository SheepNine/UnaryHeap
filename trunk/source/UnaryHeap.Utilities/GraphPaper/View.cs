using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GraphPaper
{
    partial class View : Form, IView
    {
        IViewModel viewModel;

        public View(IViewModel viewModel)
        {
            this.viewModel = viewModel;
            InitializeComponent();
            viewModel.HookUp(editorPanel, editorGestures);
            viewModel.CurrentFilenameChanged += viewModel_CurrentFilenameChanged;
            viewModel.IsModifiedChanged += viewModel_IsModifiedChanged;
            UpdateDialogText();
        }

        private void viewModel_IsModifiedChanged(object sender, EventArgs e)
        {
            UpdateDialogText();
        }

        private void viewModel_CurrentFilenameChanged(object sender, EventArgs e)
        {
            UpdateDialogText();
        }

        void UpdateDialogText()
        {
            var builder = new StringBuilder();

            if (viewModel.IsModified)
                builder.Append("*");

            if (null != viewModel.CurrentFileName)
            {
                builder.Append(Path.GetFileNameWithoutExtension(viewModel.CurrentFileName));
                builder.Append(" - ");
            }

            builder.Append("GraphPaper");

            Text = builder.ToString();
        }

        private void newToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            viewModel.New();
        }

        private void loadToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            viewModel.Load();
        }

        private void closeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void wholeModelToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            viewModel.ViewWholeModel();
        }
    }

    interface IView
    {

    }
}
