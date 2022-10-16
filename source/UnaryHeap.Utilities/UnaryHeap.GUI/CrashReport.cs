using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace UnaryHeap.GUI
{
    partial class CrashReport : Form
    {
        public CrashReport(string stackTrace)
        {
            InitializeComponent();
            exceptionDetails.Text = stackTrace;
        }

        private void copyDetailsButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(exceptionDetails.Text);
        }

        private void openGitHubButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/SheepNine/UnaryHeap/issues");
        }
    }
}
