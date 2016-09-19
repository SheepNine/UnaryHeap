using System.Threading;
using System.Windows.Forms;
using System;
using Reversi.Generated;

namespace Reversi.Forms
{
    public partial class ClientForm : Form
    {
        PocoClientEndpoint2 endpoint;
        Role currentRole = Role.None;
        Role activeRole = Role.None;

        public ClientForm(PocoClientEndpoint2 endpoint)
        {
            InitializeComponent();
            this.endpoint = endpoint;
            this.pocoReader.Enabled = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            endpoint.Close();
        }

        public static void Spawn(PocoClientEndpoint2 endpoint)
        {
            new Thread(SpawnThreadMain) { Name = "Spawn Thread" }.Start(endpoint);
        }

        static void SpawnThreadMain(object endpoint)
        {
            Application.Run(new ClientForm((PocoClientEndpoint2)endpoint));
        }

        private void pocoReader_Tick(object sender, EventArgs e)
        {
            if (!endpoint.HasData) return;
            var poco = endpoint.Receive();
            if (poco == null)
            {
                pocoReader.Enabled = false;
            }
            else if (poco is RosterUpdate)
            {
                var rosterUpdatePoco = poco as RosterUpdate;

                whitePlayerLabel.Text = rosterUpdatePoco.PlayerOne;
                blackPlayerLabel.Text = rosterUpdatePoco.PlayerTwo;

                observersListBox.BeginUpdate();
                observersListBox.Items.Clear();
                foreach (var name in rosterUpdatePoco.Observers.Split('|'))
                    observersListBox.Items.Add(name);
                observersListBox.EndUpdate();

                currentRole = (poco as RosterUpdate).YourRole;
            }
            else if (poco is BoardUpdate)
            {
                reversiBoard.UpdateState((poco as BoardUpdate).BoardState);
                activeRole = (poco as BoardUpdate).ActivePlayer;
            }

            reversiBoard.IsActivePlayer = (currentRole != Role.None &&
                currentRole == activeRole);
        }

        private void playerOneButton_Click(object sender, EventArgs e)
        {
            endpoint.Send(new ChangeRole(Role.PlayerOne));
        }

        private void playerTwoButton_Click(object sender, EventArgs e)
        {
            endpoint.Send(new ChangeRole(Role.PlayerTwo));
        }

        private void observeButton_Click(object sender, EventArgs e)
        {
            endpoint.Send(new ChangeRole(Role.Observer));
        }

        private void setNameButton_Click(object sender, EventArgs e)
        {
            endpoint.Send(new SetName(nameTextBox.Text));
        }

        private void reversiBoard_SquareClicked(object sender, SquareClickedEventArgs e)
        {
            endpoint.Send(new PlacePiece(e.X, e.Y));
        }
    }
}
