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
    }

    interface IView
    {

    }
}
