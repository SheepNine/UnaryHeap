using Reversi.Forms;
using System;
using System.Net;
using System.Windows.Forms;

namespace Reversi
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var server = new Server(IPAddress.Any, 7775);
            server.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServerForm());

            server.Stop();
        }
    }
}
