using System;
using System.Collections.Generic;
using UnaryHeap.Utilities.Misc;

namespace Patchwork
{
    class Stamp
    {
        int[] dX, dY, dTile;

        private Stamp(int[] dX, int[] dY, int[] dTile)
        {
            this.dX = dX;
            this.dY = dY;
            this.dTile = dTile;
        }

        private const string FOUR_BY_FOUR = "four_by_four";
        public static Stamp FourByFour(int stride)
        {
            int[] dX = new int[16];
            int[] dY = new int[16];
            int[] dTile = new int[16];
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                {
                    int i = x + 4 * y;
                    dX[i] = x;
                    dY[i] = y;
                    dTile[i] = x + stride * y;
                }
            return new Stamp(dX, dY, dTile);
        }

        private const string Y_EDGE = "y_edge";
        public static Stamp YEdge(int stride)
        {
            int[] dX = { 0, -1, -2, -3 };
            int[] dY = { 0, 0, 1, 1 };
            int[] dTile = { 0, -1, -2 + stride, -3 + stride };
            return new Stamp(dX, dY, dTile);
        }

        private const string X_EDGE = "x_edge";
        public static Stamp XEdge(int stride)
        {
            int[] dX = { 0, 1, 2, 3 };
            int[] dY = { 0, 0, 1, 1 };
            int[] dTile = { 0, 1, 2 + stride, 3 + stride };
            return new Stamp(dX, dY, dTile);
        }

        private const string Y_WALL = "y_wall";
        public static Stamp YWall(int stride)
        {
            int[] dX = new int[28];
            int[] dY = new int[28];
            int[] dTile = new int[28];

            for (var i = 0; i < 28; i++)
            {
                var x = i % 4;
                var y = i / 4;
                dX[i] = x - 3;
                dY[i] = y - 5 - (x / 2);
                dTile[i] = dX[i] + dY[i] * stride;
            }
            return new Stamp(dX, dY, dTile);
        }

        private const string X_WALL = "x_wall";
        public static Stamp XWall(int stride)
        {
            int[] dX = new int[28];
            int[] dY = new int[28];
            int[] dTile = new int[28];

            for (var i = 0; i < 28; i++)
            {
                var x = i % 4;
                var y = i / 4;
                dX[i] = x;
                dY[i] = y - 6 + (x / 2);
                dTile[i] = dX[i] + dY[i] * stride;
            }
            return new Stamp(dX, dY, dTile);
        }

        private const string Y_WALL_LOW = "y_wall_low";
        public static Stamp YWallLow(int stride)
        {
            int[] dX = new int[6];
            int[] dY = new int[6];
            int[] dTile = new int[6];

            for (var i = 0; i < 6; i++)
            {
                var x = (i % 4);
                var y = 6 - (i / 4);
                dX[i] = x - 3;
                dY[i] = y - 5 - (x / 2);
                dTile[i] = dX[i] + dY[i] * stride;
            }
            return new Stamp(dX, dY, dTile);
        }

        private const string X_WALL_LOW = "x_wall_low";
        public static Stamp XWallLow(int stride)
        {
            int[] dX = new int[6];
            int[] dY = new int[6];
            int[] dTile = new int[6];

            for (var i = 0; i < 6; i++)
            {
                var x = 3 - i % 4;
                var y = 6 - (i / 4);
                dX[i] = x;
                dY[i] = y - 6 + (x / 2);
                dTile[i] = dX[i] + dY[i] * stride;
            }
            return new Stamp(dX, dY, dTile);
        }

        private const string WALL_SEAM = "wall_seam";
        public static Stamp WallSeam(int stride)
        {
            int[] dX = new int[14];
            int[] dY = new int[14];
            int[] dTile = new int[14];

            for (var i = 0; i < 14; i++)
            {
                var x = i % 2;
                var y = i / 2;
                dX[i] = x;
                dY[i] = y - 6;
                dTile[i] = dX[i] + dY[i] * stride;
            }
            return new Stamp(dX, dY, dTile);
        }

        private const string FOUR_BY_TWO = "four_by_two";
        public static Stamp FourByTwo(int stride)
        {
            int[] dX = new int[8];
            int[] dY = new int[8];
            int[] dTile = new int[8];

            for (var i = 0; i < 8; i++)
            {
                var x = i % 4;
                var y = i / 4;
                dX[i] = x;
                dY[i] = y;
                dTile[i] = x + y * stride;
            }
            return new Stamp(dX, dY, dTile);
        }

        private const string ONE_BY_SIX = "one_by_six";
        public static Stamp OneBySix(int stride)
        {
            int[] dX = new int[6];
            int[] dY = new int[6];
            int[] dTile = new int[6];

            for (var i = 0; i < 6; i++)
            {
                dX[i] = 0;
                dY[i] = -5 + i;
                dTile[i] = dY[i] * stride;
            }
            return new Stamp(dX, dY, dTile);
        }

        private const string TWO_BY_ONE = "two_by_one";

        public static Stamp TwoByOne()
        {
            return new Stamp(
                new int[] { 0, 1 },
                new int[] { 0, 0 },
                new int[] { 0, 1 }
            );
        }

        public void Apply(TileArrangement m, int x, int y, int tile)
        {
            for (int i = 0; i < dX.Length; i++)
            {
                int destX = x + dX[i];
                int destY = y + dY[i];
                int destTile = tile + dTile[i];
                if (destX < 0 || destX >= m.TileCountX)
                    continue;
                if (destY < 0 || destY >= m.TileCountY)
                    continue;
                if (destTile < 0)
                    return;

                m[destX, destY] = destTile;
            }
        }

        public static IEnumerable<string> Names
        {
            get
            {
                return new string[] {
                    TWO_BY_ONE,
                    FOUR_BY_TWO,
                    FOUR_BY_FOUR,
                    ONE_BY_SIX,
                    X_EDGE,
                    X_WALL_LOW,
                    X_WALL,
                    Y_EDGE,
                    Y_WALL_LOW,
                    Y_WALL,
                    WALL_SEAM,
                };
            }
        }

        public static Stamp Get(string stampName, int tileStride)
        {
            switch (stampName)
            {
                case TWO_BY_ONE: return TwoByOne();
                case FOUR_BY_TWO: return FourByTwo(tileStride);
                case FOUR_BY_FOUR: return FourByFour(tileStride);
                case ONE_BY_SIX: return OneBySix(tileStride);
                case X_EDGE: return XEdge(tileStride);
                case X_WALL_LOW: return XWallLow(tileStride);
                case X_WALL: return XWall(tileStride);
                case Y_EDGE: return YEdge(tileStride);
                case Y_WALL_LOW: return YWallLow(tileStride);
                case Y_WALL: return YWall(tileStride);
                case WALL_SEAM: return WallSeam(tileStride);
                default: throw new ArgumentException("Unknown stamp");
            }
        }

        public static string Title(string stampName)
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
        }
    }
}
