using Reversi.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Reversi
{
    public class ServerLogicFactory : IServerLogicFactory
    {
        private static ServerLogicFactory instance = new ServerLogicFactory();
        public static IServerLogicFactory Instance
        {
            get { return instance; }
        }

        public IServerLogic Create(IServerLogicCallbacks callbacks)
        {
            return new ServerLogic(callbacks);
        }
    }

    public class ServerLogic : IServerLogic
    {
        SortedDictionary<Guid, string> names = new SortedDictionary<Guid, string>();
        Guid playerOne;
        Guid playerTwo;
        List<Guid> observers = new List<Guid>();
        GameLogic logic = new GameLogic();
        IServerLogicCallbacks callbacks;

        public ServerLogic(IServerLogicCallbacks callbacks)
        {
            this.callbacks = callbacks;
        }

        public void Shutdown()
        {
        }

        public void Process(Guid id, Poco poco)
        {
            if (poco is ConnectionAdded)
            {
                observers.Add(id);
                names.Add(id, null);
                callbacks.Send(MakeRosterState(id), id);
                callbacks.Send(MakeBoardState(), id);
            }
            else if (poco is ConnectionLost)
            {
                if (playerOne.Equals(id))
                    playerOne = Guid.Empty;
                if (playerTwo.Equals(id))
                    playerTwo = Guid.Empty;
                observers.Remove(id);
                names.Remove(id);
                PushRosterState();
            }
            else if (poco is SetName)
            {
                var setNamePoco = (SetName)poco;

                if (nameIsValid(setNamePoco.Name))
                {
                    names[id] = setNamePoco.Name;
                    PushRosterState();
                }
                else
                {
                    callbacks.Send(new InvalidName(names[id] ?? string.Empty), id);
                }
            }
            else if (poco is ChangeRole)
            {
                if (names[id] == null)
                    return;

                var changeRosterPoco = (ChangeRole)poco;

                if (changeRosterPoco.NewRole == Role.Observer)
                {
                    if (playerOne.Equals(id))
                    {
                        playerOne = Guid.Empty;
                        observers.Add(id);
                        PushRosterState();
                    }
                    else if (playerTwo.Equals(id))
                    {
                        playerTwo = Guid.Empty;
                        observers.Add(id);
                        PushRosterState();
                    }
                }
                else if (changeRosterPoco.NewRole == Role.PlayerOne &&
                    playerOne.Equals(Guid.Empty))
                {
                    playerOne = id;
                    observers.Remove(id);
                    if (playerTwo.Equals(id))
                        playerTwo = Guid.Empty;
                    PushRosterState();
                }
                else if (changeRosterPoco.NewRole == Role.PlayerTwo &&
                    playerTwo.Equals(Guid.Empty))
                {
                    playerTwo = id;
                    observers.Remove(id);
                    if (playerOne.Equals(id))
                        playerOne = Guid.Empty;
                    PushRosterState();
                }
            }
            else if (poco is PlacePiece)
            {
                var placePiecePoco = poco as PlacePiece;

                if (playerOne.Equals(id) && logic.ActivePlayer == Player.PlayerOne ||
                    playerTwo.Equals(id) && logic.ActivePlayer == Player.PlayerTwo)
                {
                    logic.PlacePiece(placePiecePoco.X, placePiecePoco.Y);
                    PushBoardState();
                }
            }
        }

        private bool nameIsValid(string name)
        {
            return name.Length > 0 &&
                name.Length <= 16 &&
                Regex.IsMatch(name, "^[a-zA-Z_0-9]*$");
        }

        void PushRosterState()
        {
            if (!playerOne.Equals(Guid.Empty))
                callbacks.Send(MakeRosterState(playerOne), playerOne);
            if (!playerTwo.Equals(Guid.Empty))
                callbacks.Send(MakeRosterState(playerTwo), playerTwo);
            foreach (var observer in observers)
                callbacks.Send(MakeRosterState(observer), observer);
        }

        Poco MakeRosterState(Guid observerId)
        {
            var playerOneName = playerOne.Equals(Guid.Empty) ? "" : names[playerOne];
            var playerTwoName = playerTwo.Equals(Guid.Empty) ? "" : names[playerTwo];
            var observerNames = string.Join("|",
                observers.Select(o => names[o]).Where(s => s != null));

            var role = Role.Observer;
            if (names[observerId] == null)
                role = Role.None;
            else if (observerId.Equals(playerOne))
                role = Role.PlayerOne;
            else if (observerId.Equals(playerTwo))
                role = Role.PlayerTwo;

            return new RosterUpdate(playerOneName, playerTwoName, observerNames, role);
        }

        void PushBoardState()
        {
            var poco = MakeBoardState();

            if (!playerOne.Equals(Guid.Empty))
                callbacks.Send(poco, playerOne);
            if (!playerTwo.Equals(Guid.Empty))
                callbacks.Send(poco, playerTwo);
            foreach (var observer in observers)
                callbacks.Send(poco, observer);
        }

        private BoardUpdate MakeBoardState()
        {
            return new BoardUpdate(logic.GetState(),
                logic.GameOver ? Role.None : (Role)(int)logic.ActivePlayer);
        }
    }
}
