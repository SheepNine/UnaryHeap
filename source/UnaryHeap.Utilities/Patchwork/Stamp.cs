using System;
using System.Collections.Generic;
using UnaryHeap.Utilities.Misc;

namespace Patchwork
{
    public class Stamp
    {
        int[] dX, dY, dTileX, dTileY;
        public string ID { get; private set; }
        public string Title { get; private set; }

        private Stamp(int[] dX, int[] dY, int[] dTileX, int[] dTileY, string id, string title)
        {
            this.dX = dX;
            this.dY = dY;
            this.dTileX = dTileX;
            this.dTileY = dTileY;
            ID = id;
            Title = title;
        }

        private const string FOUR_BY_FOUR = "four_by_four";
        public static Stamp FourByFour()
        {
            int[] dX = new int[16];
            int[] dY = new int[16];
            int[] dTileX = new int[16];
            int[] dTileY = new int[16];
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                {
                    int i = x + 4 * y;
                    dX[i] = x;
                    dY[i] = y;
                    dTileX[i] = x;
                    dTileY[i] = y;
                }
            return new Stamp(dX, dY, dTileX, dTileY, "fourbyfour", "4x4");
        }

        private const string Y_EDGE = "y_edge";
        public static Stamp YEdge()
        {
            int[] dX = { 0, -1, -2, -3 };
            int[] dY = { 0, 0, 1, 1 };
            int[] dTileX = { 0, -1, -2, -3 };
            int[] dTileY = { 0, 0, 1, 1 };
            return new Stamp(dX, dY, dTileX, dTileY, "yedge", "Y-Edge");
        }

        private const string X_EDGE = "x_edge";
        public static Stamp XEdge()
        {
            int[] dX = { 0, 1, 2, 3 };
            int[] dY = { 0, 0, 1, 1 };
            int[] dTileX = { 0, 1, 2, 3 };
            int[] dTileY = { 0, 0, 1, 1 };
            return new Stamp(dX, dY, dTileX, dTileY, "xedge", "X-Edge");
        }

        private const string Y_WALL = "y_wall";
        public static Stamp YWall()
        {
            int[] dX = new int[28];
            int[] dY = new int[28];
            int[] dTileX = new int[28];
            int[] dTileY = new int[28];

            for (var i = 0; i < 28; i++)
            {
                var x = i % 4;
                var y = i / 4;
                dX[i] = x - 3;
                dY[i] = y - 5 - (x / 2);
                dTileX[i] = x - 3;
                dTileY[i] = y - 5 - (x / 2);
            }
            return new Stamp(dX, dY, dTileX, dTileY, "ywall", "Y-Wall");
        }

        private const string X_WALL = "x_wall";
        public static Stamp XWall()
        {
            int[] dX = new int[28];
            int[] dY = new int[28];
            int[] dTileX = new int[28];
            int[] dTileY = new int[28];

            for (var i = 0; i < 28; i++)
            {
                var x = i % 4;
                var y = i / 4;
                dX[i] = x;
                dY[i] = y - 6 + (x / 2);
                dTileX[i] = x;
                dTileY[i] = y - 6 + (x / 2);
            }
            return new Stamp(dX, dY, dTileX, dTileY, "xwall", "X-Wall");
        }

        private const string Y_WALL_LOW = "y_wall_low";
        public static Stamp YWallLow()
        {
            int[] dX = new int[6];
            int[] dY = new int[6];
            int[] dTileX = new int[6];
            int[] dTileY = new int[6];

            for (var i = 0; i < 6; i++)
            {
                var x = (i % 4);
                var y = 6 - (i / 4);
                dX[i] = x - 3;
                dY[i] = y - 5 - (x / 2);
                dTileX[i] = x - 3;
                dTileY[i] = y - 5 - (x / 2);
            }
            return new Stamp(dX, dY, dTileX, dTileY, "ywalllow", "Low Y-Wall");
        }

        private const string X_WALL_LOW = "x_wall_low";
        public static Stamp XWallLow()
        {
            int[] dX = new int[6];
            int[] dY = new int[6];
            int[] dTileX = new int[6];
            int[] dTileY = new int[6];

            for (var i = 0; i < 6; i++)
            {
                var x = 3 - i % 4;
                var y = 6 - (i / 4);
                dX[i] = x;
                dY[i] = y - 6 + (x / 2);
                dTileX[i] = x;
                dTileY[i] = y - 6 + (x / 2);
            }
            return new Stamp(dX, dY, dTileX, dTileY, "xwalllow", "Low X-Wall");
        }

        private const string WALL_SEAM = "wall_seam";
        public static Stamp WallSeam()
        {
            int[] dX = new int[14];
            int[] dY = new int[14];
            int[] dTileX = new int[14];
            int[] dTileY = new int[14];

            for (var i = 0; i < 14; i++)
            {
                var x = i % 2;
                var y = i / 2;
                dX[i] = x;
                dY[i] = y - 6;
                dTileX[i] = dX[i];
                dTileY[i] = dY[i];
            }
            return new Stamp(dX, dY, dTileX, dTileY, "wallseam", "Wall Seam");
        }

        private const string FOUR_BY_TWO = "four_by_two";
        public static Stamp FourByTwo()
        {
            int[] dX = new int[8];
            int[] dY = new int[8];
            int[] dTileX = new int[8];
            int[] dTileY = new int[8];

            for (var i = 0; i < 8; i++)
            {
                var x = i % 4;
                var y = i / 4;
                dX[i] = x;
                dY[i] = y;
                dTileX[i] = x;
                dTileY[i] = y;
            }
            return new Stamp(dX, dY, dTileX, dTileY, "fourbytwo", "4x2");
        }

        private const string ONE_BY_SIX = "one_by_six";
        public static Stamp OneBySix()
        {
            int[] dX = new int[6];
            int[] dY = new int[6];
            int[] dTileX = new int[6];
            int[] dTileY = new int[6];

            for (var i = 0; i < 6; i++)
            {
                dX[i] = 0;
                dY[i] = -5 + i;
                dTileX[i] = 0;
                dTileY[i] = dY[i];
            }
            return new Stamp(dX, dY, dTileX, dTileY, "onebysix", "1x6");
        }

        private const string TWO_BY_ONE = "two_by_one";

        public static Stamp TwoByOne()
        {
            return new Stamp(
                new int[] { 0, 1 },
                new int[] { 0, 0 },
                new int[] { 0, 1 },
                new int[] { 0, 0 },
                "twobyone",
                "2x1"
            );
        }

        public void Apply(TileArrangement m, int x, int y, int tile, int tileStride)
        {
            for (int i = 0; i < dX.Length; i++)
            {
                int destX = x + dX[i];
                int destY = y + dY[i];
                int destTile = tile + dTileX[i] + dTileY[i] * tileStride;
                if (destX < 0 || destX >= m.TileCountX)
                    continue;
                if (destY < 0 || destY >= m.TileCountY)
                    continue;
                if (destTile < 0)
                    return;

                m[destX, destY] = destTile;
            }
        }

        private static Stamp[] _stamps =
        {
            TwoByOne(),
            FourByTwo(),
            FourByFour(),
            OneBySix(),
            XEdge(),
            XWallLow(),
            XWall(),
            YEdge(),
            YWallLow(),
            YWall(),
            WallSeam(),
        };

        public static IEnumerable<Stamp> Stamps
        {
            get { return _stamps; }
        }

        /*public static Stamp Get(string stampName)
        {
            switch (stampName)
            {
                case TWO_BY_ONE: return TwoByOne();
                case FOUR_BY_TWO: return FourByTwo();
                case FOUR_BY_FOUR: return FourByFour();
                case ONE_BY_SIX: return OneBySix();
                case X_EDGE: return XEdge();
                case X_WALL_LOW: return XWallLow();
                case X_WALL: return XWall();
                case Y_EDGE: return YEdge();
                case Y_WALL_LOW: return YWallLow();
                case Y_WALL: return YWall();
                case WALL_SEAM: return WallSeam();
                default: throw new ArgumentException("Unknown stamp");
            }
        }/*

        /*public static string Title(string stampName)
        {
            switch (stampName)
            {
                case TWO_BY_ONE: return "2x1";
                case FOUR_BY_TWO: return "4x2";
                case FOUR_BY_FOUR: return "4x4";
                case ONE_BY_SIX: return "1x6";
                case X_EDGE: return "X-Edge";
                case X_WALL_LOW: return "Low X-Wall";
                case X_WALL: return "X-Wall";
                case Y_EDGE: return "Y-Edge";
                case Y_WALL_LOW: return "Low Y-Wall";
                case Y_WALL: return "Y-Wall";
                case WALL_SEAM: return "Wall Seam";
                default: throw new ArgumentException("Unknown stamp");
            }
        }*/
    }
}
