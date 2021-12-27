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

        public static Stamp Quad(int stride)
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

        public static Stamp YEdge(int stride)
        {
            int[] dX = { 0, -1, -2, -3 };
            int[] dY = { 0, 0, 1, 1 };
            int[] dTile = { 0, -1, -2 + stride, -3 + stride };
            return new Stamp(dX, dY, dTile);
        }

        public static Stamp XEdge(int stride)
        {
            int[] dX = { 0, 1, 2, 3 };
            int[] dY = { 0, 0, 1, 1 };
            int[] dTile = { 0, 1, 2 + stride, 3 + stride };
            return new Stamp(dX, dY, dTile);
        }

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

        public static Stamp LowYWall(int stride)
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


        public static Stamp LowXWall(int stride)
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
    }
}
