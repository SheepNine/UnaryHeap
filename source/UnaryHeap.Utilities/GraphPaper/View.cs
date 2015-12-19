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
            viewModel.HookUp(wysiwygPanel1);
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
