using System;
using System.Windows.Forms;

namespace Patchwork
{
    public partial class View : Form
    {
        public View(ViewModel viewModel)
        {
            InitializeComponent();

            editorPanel.PaintContent += viewModel.editorPanel_PaintContent;
        }
    }
}
