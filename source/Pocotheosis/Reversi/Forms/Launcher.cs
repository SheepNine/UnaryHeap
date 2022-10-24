using Reversi.Generated;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Reversi.Forms
{
    public partial class Launcher : Form
    {
        IServer server;

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

            server = Server.Create(IPAddress.Any, port, ServerLogicFactory.Instance);

            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to initialize server: " + ex.Message, "Reversi");
                server.Dispose();
                server = null;
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

        void ClientFormShown()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ClientFormShown));
            }
            else
            {
                Close();
            }
        }

        void ClientFormClosed()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ClientFormClosed));
            }
            else
            {
                if (server != null)
                {
                    server.RequestShutdown();
                    server.WaitUntilServerShutdownComplete();
                    server.Dispose();
                    server = null;
                }
            }
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

            ClientForm.Spawn(new PocoClientEndpoint(
                client.GetStream()), ClientFormShown, ClientFormClosed);
        }
    }
}
