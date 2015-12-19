using System;
using System.Windows.Forms;

namespace GraphPaper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var viewModel = new ViewModel())
                viewModel.Run();
        }
    }
}
