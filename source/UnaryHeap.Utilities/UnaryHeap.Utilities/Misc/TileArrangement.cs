using System;
using System.Drawing;
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

        /// <summary>
        /// Creates a copy of the current TileArrangment object.
        /// </summary>
        /// <returns>A copy of the current TileArrangement object.</returns>
        public TileArrangement Clone()
        {
            var result = new TileArrangement(tileCountX, tileCountY);

            foreach (var y in Enumerable.Range(0, tileCountY))
                foreach (var x in Enumerable.Range(0, tileCountX))
                    result.tileIndices[x, y] = this.tileIndices[x, y];

            return result;
        }

        /// <summary>
        /// Adds a blank column on the right side of the current TileArrangement object.
        /// </summary>
        public void ExpandRight()
        {
            ExpandRight(1);
        }

        void ExpandRight(int amount)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException("amount");

            var newIndices = new int[tileCountX + amount, tileCountY];

            foreach (var y in Enumerable.Range(0, tileCountY))
                foreach (var x in Enumerable.Range(0, tileCountX))
                    newIndices[x, y] = tileIndices[x, y];

            tileCountX += amount;
            tileIndices = newIndices;
        }

        /// <summary>
        /// Adds a blank column on the left side of the current TileArrangement object.
        /// </summary>
        public void ExpandLeft()
        {
            ExpandLeft(1);
        }

        void ExpandLeft(int amount)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException("amount");

            var newIndices = new int[tileCountX + amount, tileCountY];

            foreach (var y in Enumerable.Range(0, tileCountY))
                foreach (var x in Enumerable.Range(0, tileCountX))
                    newIndices[x + amount, y] = tileIndices[x, y];

            tileCountX += amount;
            tileIndices = newIndices;
        }

        /// <summary>
        /// Adds a blank column on the bottom side of the current TileArrangement object.
        /// </summary>
        public void ExpandBottom()
        {
            ExpandBottom(1);
        }

        void ExpandBottom(int amount)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException("amount");

            var newIndices = new int[tileCountX, tileCountY + amount];

            foreach (var y in Enumerable.Range(0, tileCountY))
                foreach (var x in Enumerable.Range(0, tileCountX))
                    newIndices[x, y] = tileIndices[x, y];

            tileCountY += amount;
            tileIndices = newIndices;
        }

        /// <summary>
        /// Adds a blank column on the top side of the current TileArrangement object.
        /// </summary>
        public void ExpandTop()
        {
            ExpandTop(1);
        }

        void ExpandTop(int amount)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException("amount");

            var newIndices = new int[tileCountX, tileCountY + amount];

            foreach (var y in Enumerable.Range(0, tileCountY))
                foreach (var x in Enumerable.Range(0, tileCountX))
                    newIndices[x, y + amount] = tileIndices[x, y];

            tileCountY += amount;
            tileIndices = newIndices;
        }

        /// <summary>
        /// Removes a column on the right side of the current TileArrangement object.
        /// </summary>
        public void ContractRight()
        {
            ContractRight(1);
        }

        void ContractRight(int amount)
        {
            if (amount < 1 || amount >= TileCountX)
                throw new ArgumentOutOfRangeException("amount");

            var newIndices = new int[tileCountX - amount, tileCountY];

            foreach (var y in Enumerable.Range(0, tileCountY))
                foreach (var x in Enumerable.Range(0, tileCountX - amount))
                    newIndices[x, y] = tileIndices[x, y];

            tileCountX -= amount;
            tileIndices = newIndices;
        }

        /// <summary>
        /// Removes a column on the left side of the current TileArrangement object.
        /// </summary>
        public void ContractLeft()
        {
            ContractLeft(1);
        }

        void ContractLeft(int amount)
        {
            if (amount < 1 || amount >= TileCountX)
                throw new ArgumentOutOfRangeException("amount");

            var newIndices = new int[tileCountX - amount, tileCountY];

            foreach (var y in Enumerable.Range(0, tileCountY))
                foreach (var x in Enumerable.Range(0, tileCountX - amount))
                    newIndices[x, y] = tileIndices[x + amount, y];

            tileCountX -= amount;
            tileIndices = newIndices;
        }

        /// <summary>
        /// Removes a column on the bottom side of the current TileArrangement object.
        /// </summary>
        public void ContractBottom()
        {
            ContractBottom(1);
        }

        void ContractBottom(int amount)
        {
            if (amount < 1 || amount >= TileCountX)
                throw new ArgumentOutOfRangeException("amount");

            var newIndices = new int[tileCountX, tileCountY - amount];

            foreach (var y in Enumerable.Range(0, tileCountY - amount))
                foreach (var x in Enumerable.Range(0, tileCountX))
                    newIndices[x, y] = tileIndices[x, y];

            tileCountY -= amount;
            tileIndices = newIndices;
        }

        /// <summary>
        /// Removes a column on the top side of the current TileArrangement object.
        /// </summary>
        public void ContractTop()
        {
            ContractTop(1);
        }

        void ContractTop(int amount)
        {
            if (amount < 1 || amount >= TileCountX)
                throw new ArgumentOutOfRangeException("amount");

            var newIndices = new int[tileCountX, tileCountY - amount];

            foreach (var y in Enumerable.Range(0, tileCountY - amount))
                foreach (var x in Enumerable.Range(0, tileCountX))
                    newIndices[x, y] = tileIndices[x, y + amount];

            tileCountY -= amount;
            tileIndices = newIndices;
        }

        /// <summary>
        /// Draws the current TileArrangment using the specified TileSet.
        /// </summary>
        /// <param name="g">The graphics context to which to render the
        /// current TileArrangement.</param>
        /// <param name="tileset">The TileSet to use to render the tiles
        /// in the current TileArrangment.</param>
        /// <param name="scale">The amount by which to scale the output image.</param>
        public void Render(Graphics g, Tileset tileset, int scale = 1)
        {
            if (null == g)
                throw new ArgumentNullException("g");
            if (null == tileset)
                throw new ArgumentNullException("tileset");
            if (scale < 1)
                throw new ArgumentOutOfRangeException("scale");

            var wholeBounds = new Rectangle(0, 0,
                scale * tileset.TileSize * tileCountX,
                scale * tileset.TileSize * tileCountY);

            RenderSubset(g, tileset, scale, wholeBounds);
        }

        /// <summary>
        /// Draws the current TileArrangment using the specified TileSet.
        /// </summary>
        /// <param name="g">The graphics context to which to render the
        /// current TileArrangement.</param>
        /// <param name="tileset">The TileSet to use to render the tiles
        /// in the current TileArrangment.</param>
        /// <param name="scale">The amount by which to scale the output image.</param>
        /// <param name="visibleRect">The area (in pixel coordinates) to draw.</param>
        public void RenderSubset(Graphics g, Tileset tileset, int scale, Rectangle visibleRect)
        {
            if (null == g)
                throw new ArgumentNullException("g");
            if (null == tileset)
                throw new ArgumentNullException("tileset");
            if (scale < 1)
                throw new ArgumentOutOfRangeException("scale");

            var size = tileset.TileSize * scale;
            var padding = size - 1;
            var clipRect = new Rectangle(visibleRect.Left - padding, visibleRect.Top - padding,
                visibleRect.Width + padding, visibleRect.Height + padding);

            for (int y = 0; y < tileCountY; y++)
                for (int x = 0; x < tileCountX; x++)
                {
                    var i = tileIndices[x, y];
                    var tileUpperLeft = new Point(x * size, y * size);

                    if (clipRect.Contains(tileUpperLeft) == false)
                        continue;

                    if (i >= tileset.NumTiles)
                        DrawMissingTile(g, size, tileUpperLeft);
                    else
                        tileset.DrawTile(g, tileIndices[x, y],
                            tileUpperLeft.X, tileUpperLeft.Y, scale);
                }
        }

        static void DrawMissingTile(Graphics g, int size, Point origin)
        {
            g.FillRectangle(Brushes.DarkRed,
                new Rectangle(origin, new Size(size, size)));
            g.DrawRectangle(Pens.Red,
                new Rectangle(origin, new Size(size - 1, size - 1)));
            g.DrawLine(Pens.Red,
                origin.X, origin.Y, origin.X + size - 1, origin.Y + size - 1);
            g.DrawLine(Pens.Red,
                origin.X + size - 1, origin.Y, origin.X, origin.Y + size - 1);
        }

        /// <summary>
        /// Switches all occurrence of one tileset for another and vice versa.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void SwapTileIndexes(int from, int to)
        {
            if (from == to)
                return;

            foreach (var y  in Enumerable.Range(0, tileCountY))
                foreach (var x in Enumerable.Range(0, tileCountX))
                {
                    if (tileIndices[x, y] == from)
                    {
                        tileIndices[x, y] = to;
                    }
                    else if (tileIndices[x, y] == to)
                    {
                        tileIndices[x, y] = from;
                    }
                }
        }
    }
}
