using System;
using System.Collections.Generic;
using System.Linq;

namespace Reversi
{
    public enum Player
    {
        PlayerOne = 1,
        PlayerTwo = 2,
    }

    class GameLogic
    {
        static class TraceSet
        {
            static List<Trace> traces;
            static TraceSet()
            {
                traces = new List<Trace>();
                foreach (var x in Enumerable.Range(0, 8))
                    foreach (var y in Enumerable.Range(0, 8))
                        traces.AddRange(Trace.CreateAllForDirection(x, y));
                traces = traces.Where(t => t.Length >= 3).ToList();
            }

            public static bool HasAnyValidMovesFor(Player?[,] board, Player player)
            {
                return traces.Any(t => t.IsFlippableBy(board, player));
            }
        }

        struct Trace
        {
            int startX, startY;
            int dX, dY;
            int length;
            public int Length { get { return length; } }

            public Trace(int startX, int startY, int dX, int dY)
            {
                if (Math.Abs(dX) > 1 || Math.Abs(dY) > 1 || dX == 0 && dY == 0)
                    throw new ArgumentException();

                this.startX = startX;
                this.startY = startY;
                this.dX = dX;
                this.dY = dY;
                length = GetLength(startX, startY, dX, dY);
            }

            static int GetLength(int startX, int startY, int dX, int dY)
            {
                var result = 8;

                if (dX == 1)
                    result = Math.Min(result, 8 - startX);
                if (dX == -1)
                    result = Math.Min(result, 1 + startX);
                if (dY == 1)
                    result = Math.Min(result, 8 - startY);
                if (dY == -1)
                    result = Math.Min(result, 1 + startY);

                return result;
            }

            public Player? Get(Player?[,] board, int index)
            {
                return board[startX + index * dX, startY + index * dY];
            }

            public void Set(Player?[,] board, int index, Player? value)
            {
                board[startX + index * dX, startY + index * dY] = value;
            }

            public bool IsFlippableBy(Player?[,] board, Player player)
            {
                if (length < 3)
                    return false;
                if (Get(board, 0).HasValue)
                    return false;
                if (!(Get(board, 1).HasValue && Get(board, 1) != player))
                    return false;
                for (int i = 2; i < length; i++)
                {
                    if (!Get(board, i).HasValue)
                        return false;
                    if (Get(board, i).Value == player)
                        return true;
                }
                return false;
            }

            public void Flip(Player?[,] board, Player player)
            {
                for (int i = 1; i < length; i++)
                {
                    if (Get(board, i).Value == player)
                        break;
                    else
                        Set(board, 1, player);
                }
            }

            public static List<Trace> CreateAllForDirection(int x, int y)
            {
                var result = new List<Trace>();
                result.Add(new Trace(x, y, 1, -1));
                result.Add(new Trace(x, y, 1, 0));
                result.Add(new Trace(x, y, 1, 1));
                result.Add(new Trace(x, y, 0, 1));
                result.Add(new Trace(x, y, -1, 1));
                result.Add(new Trace(x, y, -1, 0));
                result.Add(new Trace(x, y, -1, -1));
                result.Add(new Trace(x, y, 0, -1));
                return result;
            }
        }


        private Player?[,] board;
        public Player ActivePlayer { get; private set; }
        public bool GameOver { get; private set; }

        public Player? this[int x, int y]
        {
            get { return board[x, y]; }
        }


        public GameLogic()
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            board = new Player?[8, 8];
            board[3, 3] = Player.PlayerOne;
            board[3, 4] = Player.PlayerTwo;
            board[4, 4] = Player.PlayerOne;
            board[4, 3] = Player.PlayerTwo;
            ActivePlayer = Player.PlayerOne;
            GameOver = false;
        }

        public void PlacePiece(int x, int y)
        {
            if (GameOver)
                return;

            var traces = GetFlippableTracesForCurrentPlayer(x, y);
            if (traces.Count == 0)
                return;

            board[x, y] = ActivePlayer;
            foreach (var trace in traces)
                trace.Flip(board, ActivePlayer);

            var inactivePlayer = ActivePlayer == Player.PlayerOne ? Player.PlayerTwo : Player.PlayerOne;
            if (TraceSet.HasAnyValidMovesFor(board, inactivePlayer))
                ActivePlayer = inactivePlayer;
            else if (!TraceSet.HasAnyValidMovesFor(board, ActivePlayer))
                GameOver = true;
        }

        List<Trace> GetFlippableTracesForCurrentPlayer(int x, int y)
        {
            return Trace.CreateAllForDirection(x, y)
                .Where(trace => trace.IsFlippableBy(board, ActivePlayer))
                .ToList();
        }
    }
}