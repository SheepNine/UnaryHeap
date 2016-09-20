using Reversi.Generated;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Reversi.Forms
{
    public partial class ServerForm : Form
    {
        PocoServerEndpoint endpoint = new PocoServerEndpoint();
        List<Guid> connectionIds = new List<Guid>();
        ServerLogic logic = new ServerLogic();

        public ServerForm()
        {
            InitializeComponent();
            pocoReader.Enabled = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            endpoint.DisconnectAll();
        }

        private void spawnClientButton_Click(object sender, EventArgs e)
        {
            NetworkStream a, b;
            LocalConnection.CreateTcpLoopback(out a, out b);

            endpoint.AddConnection(Guid.NewGuid(), a);
            ClientForm.Spawn(new PocoClientEndpoint(b));
        }

        private void pocoReader_Tick(object sender, EventArgs e)
        {
            while (endpoint.HasData)
            {
                var twople = endpoint.Receive();
                var connectionId = twople.Item1;
                var poco = twople.Item2;

                if (poco is ConnectionAdded)
                    connectionIds.Add(connectionId);
                if (poco is ConnectionLost)
                    connectionIds.Remove(connectionId);

                outputTextBox.AppendText(string.Format("\t[{0}]", connectionId));
                outputTextBox.AppendText(Environment.NewLine);
                outputTextBox.AppendText(poco.ToString());
                outputTextBox.AppendText(Environment.NewLine);
                outputTextBox.Select(outputTextBox.TextLength, 0);
                outputTextBox.ScrollToCaret();

                logic.Process(connectionId, poco, (a, b) => endpoint.Send(a, b));
            }
        }

        private void dropClientsButton_Click(object sender, EventArgs e)
        {
            endpoint.DisconnectAll();
        }
    }
}
