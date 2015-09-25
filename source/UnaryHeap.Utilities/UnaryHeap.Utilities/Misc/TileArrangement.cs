using System;
using System.IO;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Write a binary representation of this TileArrangement to the specified
        /// Stream.
        /// </summary>
        /// <param name="destination">The stream to which to write the representation.</param>
        public void Serialize(Stream destination)
        {
            using (var writer = new BinaryWriter(destination, Encoding.Unicode, true))
            {
                writer.Write(tileCountX);
                writer.Write(tileCountY);

                foreach (var y in Enumerable.Range(0, tileCountY))
                    foreach (var x in Enumerable.Range(0, tileCountX))
                        writer.Write(tileIndices[x, y]);
            }
        }

        /// <summary>
        /// Read a binary representation of a TileArrangmenet from the specified Stream.
        /// </summary>
        /// <param name="source">The stream from which to read the representation.</param>
        /// <returns>A copy of the binary representation.</returns>
        public static TileArrangement Deserialize(Stream source)
        {
            using (var reader = new BinaryReader(source, Encoding.Unicode, true))
            {
                var tileCountX = reader.ReadInt32();
                var tileCountY = reader.ReadInt32();

                var result = new TileArrangement(tileCountX, tileCountY);

                foreach (var y in Enumerable.Range(0, tileCountY))
                    foreach (var x in Enumerable.Range(0, tileCountX))
                        result[x, y] = reader.ReadInt32();

                return result;
            }
        }
    }
}
