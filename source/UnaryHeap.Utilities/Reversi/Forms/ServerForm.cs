using Reversi.Generated;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Reversi.Forms
{
    public partial class ServerForm : Form
    {
        public ServerForm()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void spawnClientButton_Click(object sender, EventArgs e)
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, 7775);

            using (var evt = new ManualResetEvent(false))
            {
                ClientForm.Spawn(new PocoClientEndpoint(client.GetStream()), evt);
                evt.WaitOne();
            }
        }
    }
}
