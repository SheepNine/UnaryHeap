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

            var settings = new SettingsLocker(Properties.Settings.Default);

            using (var viewModel = new ViewModel())
                viewModel.Run(settings);

            settings.Persist();
        }
    }
}
