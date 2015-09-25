using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnaryHeap.Utilities.Misc
{
    /// <summary>
    /// Represents a 2D arrangement of image tiles from a TileSet.
    /// </summary>
    public class TileArrangement
    {
        int tileCountX;
        int tileCountY;
        int[,] tileIndices;

        /// <summary>
        /// Initializes a new instance of the TileArrangmenet class.
        /// </summary>
        /// <param name="tileCountX">The number of tiles along the X-axis.</param>
        /// <param name="tileCountY">The number of tiles along the Y-axis.</param>
        public TileArrangement(int tileCountX, int tileCountY)
        {
            if (0 >= tileCountX)
                throw new ArgumentOutOfRangeException("tileCountX");
            if (0 >= tileCountY)
                throw new ArgumentOutOfRangeException("tileCountY");

            this.tileCountX = tileCountX;
            this.tileCountY = tileCountY;
            this.tileIndices = new int[tileCountX, tileCountY];
        }

        /// <summary>
        /// Gets the number of tiles along the X-axis.
        /// </summary>
        public int TileCountX
        {
            get { return tileCountX; }
        }

        /// <summary>
        /// Gets the number of tiles along the Y-axis.
        /// </summary>
        public int TileCountY
        {
            get { return tileCountY; }
        }

        /// <summary>
        /// Gets or sets the index of the tile at the specified X,Y location.
        /// Tile (0,0) is located in the upper-left corner of the arrangement.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <returns>The index of the tile at the specified location.</returns>
        public int this[int x, int y]
        {
            get
            {
                RangeCheck(x, y);
                return tileIndices[x, y];
            }
            set
            {
                RangeCheck(x, y);
                tileIndices[x, y] = value;
            }
        }

        void RangeCheck(int x, int y)
        {
            if (0 > x || x >= tileCountX)
                throw new ArgumentOutOfRangeException("x");
            if (0 > y || y >= tileCountY)
                throw new ArgumentOutOfRangeException("y");
        }
    }
}
