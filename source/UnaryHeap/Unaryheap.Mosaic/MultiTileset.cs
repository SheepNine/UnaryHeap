using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace UnaryHeap.Mosaic
{
    /// <summary>
    /// Represents an aggregation of multiple ITilesets into a single set.
    /// </summary>
    public class MultiTileset : ITileset
    {
        List<ITileset> children;

        /// <summary>
        /// Constructs a new instance of the MultiTileset class.
        /// </summary>
        /// <param name="tilesets">The tilesets to aggregate.</param>
        /// <exception cref="System.ArgumentNullException">tilesets is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// tilesets is empty, or contains a null value, or the tilesets do not share
        /// a common ImageWidth or TileSize value.
        /// </exception>
        public MultiTileset(params ITileset[] tilesets)
        {
            ArgumentNullException.ThrowIfNull(tilesets);
            if (tilesets.Length == 0)
                throw new ArgumentException("At least one tileset is required", nameof(tilesets));
            if (tilesets.Any(t => t == null))
                throw new ArgumentException("No null values allowed", nameof(tilesets));
            if (!tilesets.All(t => t.ImageWidth == tilesets[0].ImageWidth))
                throw new ArgumentException("Tilesets have mismatched ImageWidth values",
                    nameof(tilesets));
            if (!tilesets.All(t => t.TileSize == tilesets[0].TileSize))
                throw new ArgumentException("Tilesets have mismatched TileSize values",
                    nameof(tilesets));

            children = new List<ITileset>(tilesets);
        }

        /// <summary>
        /// Gets the size in pixels of a single tile in this Tileset.
        /// </summary>
        public int TileSize
        {
            get { return children[0].TileSize; }
        }

        /// <summary>
        /// Gets the number of tiles in this Tileset.
        /// </summary>
        public int NumTiles
        {
            get { return children.Sum(c => c.NumTiles); }
        }

        /// <summary>
        /// Gets the width in pixels of the underlying Tileset image.
        /// </summary>
        public int ImageWidth
        {
            get { return children[0].ImageWidth; }
        }

        /// <summary>
        /// Gets the height in pixels of the underlying Tileset image.
        /// </summary>
        public int ImageHeight
        {
            get { return children.Sum(c => c.ImageHeight); }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Control and its child
        /// controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != children)
                {
                    foreach (var child in children)
                        child.Dispose();
                    children = null;
                }
            }
        }

        /// <summary>
        /// Draws the specified tile at the specified location.
        /// </summary>
        /// <param name="g">The Graphics context to which to draw the tile.</param>
        /// <param name="tileIndex">The index of the tile to be drawn.</param>
        /// <param name="x">The destination coordinates of the upper-left corner
        /// of the tile.</param>
        /// <param name="y">The destination coordinates of the upper-left corner
        /// of the tile.</param>
        /// <param name="scale">The amount by which to scale the tile drawn.</param>
        public void DrawTile(Graphics g, int tileIndex, int x, int y, int scale = 1)
        {
            if (0 > tileIndex || tileIndex >= NumTiles)
                throw new ArgumentOutOfRangeException(nameof(tileIndex));

            int i = 0;
            while (tileIndex >= children[i].NumTiles)
            {
                tileIndex -= children[i].NumTiles;
                i += 1;
            }

            children[i].DrawTile(g, tileIndex, x, y, scale);
        }
    }
}
