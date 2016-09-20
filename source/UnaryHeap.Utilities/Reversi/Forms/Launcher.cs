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
            int port;
            if (int.TryParse(hostPortTextBox.Text, out port) == false)
            {
                MessageBox.Show("The specified port is invalid.", "Reversi");
                return;
            }

            var server = new Server(IPAddress.Any, port);

            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to initialize server: " + ex.Message, "Reversi");
                return;
            }

            ConnectAndCloseLauncher("localhost", port);
        }

        private void joinButton_Click(object sender, EventArgs e)
        {
            int port;
            if (int.TryParse(connectPortTextBox.Text, out port) == false)
            {
                MessageBox.Show("The specified port is invalid.", "Reversi");
                return;
            }

            ConnectAndCloseLauncher(connectServerTextBox.Text, port);
        }

        void ConnectAndCloseLauncher(string hostname, int port)
        {
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(hostname, port);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect: " + ex.Message, "Reversi");
                return;
            }

            using (var evt = new ManualResetEvent(false))
            {
                ClientForm.Spawn(new PocoClientEndpoint(client.GetStream()), evt);
                evt.WaitOne();
            }

            Close();
        }
    }
}
