using Reversi.Generated;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Reversi.Forms
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();
        }

        private void hostButton_Click(object sender, EventArgs e)
        {
            new Server(IPAddress.Any, 7775).Start();
            ConnectAndCloseLauncher("localhost", 7775);
        }

        private void joinButton_Click(object sender, EventArgs e)
        {
            ConnectAndCloseLauncher("localhost", 7775);
        }

        void ConnectAndCloseLauncher(string hostname, int port)
        {
            TcpClient client = new TcpClient();
            client.Connect(hostname, port);

            using (var evt = new ManualResetEvent(false))
            {
                ClientForm.Spawn(new PocoClientEndpoint(client.GetStream()), evt);
                evt.WaitOne();
            }

            Close();
        }
    }
}
