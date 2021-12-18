using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Stamp Quad(int stride)
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
            int[] dX = { 0, 1, 2, 3 };
            int[] dY = { 0, 0, -1, -1 };
            int[] dTile = { 0, 1, 2 - stride, 3 - stride };
            return new Stamp(dX, dY, dTile);
        }

        public static Stamp XEdge(int stride)
        {
            int[] dX = { 0, 1, 2, 3 };
            int[] dY = { 0, 0, 1, 1 };
            int[] dTile = { 0, 1, 2 + stride, 3 + stride };
            return new Stamp(dX, dY, dTile);
        }

        public void Apply(TileArrangement m, int x, int y, int tile)
        {
            for (int i = 0; i < dX.Length; i++)
            {
                int destX = x + dX[i];
                int destY = y + dY[i];
                if (destX < 0 || destY >= m.TileCountX)
                    continue;
                if (destY < 0 || destY >= m.TileCountY)
                    continue;

                m[destX, destY] = tile + dTile[i];
            }
        }
    }
}
