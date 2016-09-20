using System.Threading;
using System.Windows.Forms;
using System;
using Reversi.Generated;

namespace Reversi.Forms
{
    public partial class ClientForm : Form
    {
        PocoClientEndpoint endpoint;
        Role currentRole = Role.None;
        Role activeRole = Role.None;

        public ClientForm(PocoClientEndpoint endpoint)
        {
            InitializeComponent();
            this.endpoint = endpoint;
            this.pocoReader.Enabled = true;
            whitePlayerLabel.Text = string.Empty;
            blackPlayerLabel.Text = string.Empty;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            endpoint.Close();
        }

        public static void Spawn(PocoClientEndpoint endpoint)
        {
            new Thread(SpawnThreadMain) { Name = "Spawn Thread" }.Start(endpoint);
        }

        static void SpawnThreadMain(object endpoint)
        {
            Application.Run(new ClientForm((PocoClientEndpoint)endpoint));
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

                var nameSet = rosterUpdatePoco.YourRole != Role.None;
                playerOneButton.Enabled = nameSet && string.IsNullOrEmpty(rosterUpdatePoco.PlayerOne);
                playerTwoButton.Enabled = nameSet && string.IsNullOrEmpty(rosterUpdatePoco.PlayerTwo);
                observeButton.Enabled = nameSet && rosterUpdatePoco.YourRole != Role.Observer;

                setNameButton.Visible = !nameSet;
                nameTextBox.Visible = !nameSet;

                playerOneButton.Visible = nameSet;
                playerTwoButton.Visible = nameSet;
                observeButton.Visible = nameSet;
                groupBox1.Visible = nameSet;
                reversiBoard.Visible = nameSet;
                label1.Visible = nameSet;
                label2.Visible = nameSet;
                whitePlayerLabel.Visible = nameSet;
                blackPlayerLabel.Visible = nameSet;
            }
            else if (poco is BoardUpdate)
            {
                reversiBoard.UpdateState((poco as BoardUpdate).BoardState);
                activeRole = (poco as BoardUpdate).ActivePlayer;
            }
            else if (poco is InvalidName)
            {
                setNameButton.Enabled = true;
                nameTextBox.Enabled = true;
                nameTextBox.Text = (poco as InvalidName).CurrentName;
            }

            reversiBoard.IsActivePlayer = (currentRole != Role.None &&
                currentRole == activeRole);
        }

        private void playerOneButton_Click(object sender, EventArgs e)
        {
            playerOneButton.Enabled = false;
            playerTwoButton.Enabled = false;
            observeButton.Enabled = false;
            endpoint.Send(new ChangeRole(Role.PlayerOne));
        }

        private void playerTwoButton_Click(object sender, EventArgs e)
        {
            playerOneButton.Enabled = false;
            playerTwoButton.Enabled = false;
            observeButton.Enabled = false;
            endpoint.Send(new ChangeRole(Role.PlayerTwo));
        }

        private void observeButton_Click(object sender, EventArgs e)
        {
            playerOneButton.Enabled = false;
            playerTwoButton.Enabled = false;
            observeButton.Enabled = false;
            endpoint.Send(new ChangeRole(Role.Observer));
        }

        private void setNameButton_Click(object sender, EventArgs e)
        {
            setNameButton.Enabled = false;
            nameTextBox.Enabled = false;
            endpoint.Send(new SetName(nameTextBox.Text));
        }

        private void reversiBoard_SquareClicked(object sender, SquareClickedEventArgs e)
        {
            endpoint.Send(new PlacePiece(e.X, e.Y));
        }
    }
}
