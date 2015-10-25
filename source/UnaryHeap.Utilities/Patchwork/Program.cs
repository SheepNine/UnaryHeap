using System;
using System.Windows.Forms;

namespace Patchwork
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

            ProgramData.CreateDefaultArrangement();

            new ViewModel().Run();
        }
    }
}
