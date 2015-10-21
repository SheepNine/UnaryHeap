using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace UnaryHeap.Utilities.Misc
{
    /// <summary>
    /// Represents a collection of tiles as subsets of a single tileset image,
    /// allowing drawing of an individual tile.
    /// </summary>
    public class Tileset : IDisposable
    {
        Image tileImages;
        int tileSize;

        /// <summary>
        /// Constructs a new instance of the Tileset class.
        /// </summary>
        /// <param name="tileImages">The image containing the individual tiles.</param>
        /// <param name="tileSize">The size of an individual tile.</param>
        /// <exception cref="System.ArgumentNullException">tileImages is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// tileSize is less than one, or tileImage's width or height are not
        /// multiples of tileSize.
        /// </exception>
        public Tileset(Image tileImages, int tileSize)
        {
            if (null == tileImages)
                throw new ArgumentNullException("tileImages");
            if (1 > tileSize)
                throw new ArgumentOutOfRangeException(
                    "tileSize", "tileSize is less than one.");

            if (0 != tileImages.Width % tileSize)
                throw new ArgumentException(
                    "tileImages width is not a multiple of tileSize.", "tileSize");
            if (0 != tileImages.Height % tileSize)
                throw new ArgumentException(
                    "tileImages height is not a multiple of tileSize.", "tileSize");

            this.tileImages = tileImages;
            this.tileSize = tileSize;
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
                if (null != tileImages)
                {
                    tileImages.Dispose();
                    tileImages = null;
                }
            }
        }

        /// <summary>
        /// Gets the number of tiles in this Tileset.
        /// </summary>
        public int NumTiles
        {
            get { return (tileImages.Width / tileSize) * (tileImages.Height / tileSize); }
        }

        /// <summary>
        /// Gets the size in pixels of a single tile in this Tileset.
        /// </summary>
        public int TileSize
        {
            get { return tileSize; }
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
            if (null == g)
                throw new ArgumentNullException("g");
            if (0 > tileIndex || tileIndex >= NumTiles)
                throw new ArgumentOutOfRangeException("tileIndex");
            if (scale < 1)
                throw new ArgumentOutOfRangeException("scale");

            var gState = g.Save();
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var step = tileImages.Width / tileSize;
            var tileX = tileIndex % step;
            var tileY = tileIndex / step;

            g.DrawImage(tileImages,
                new Rectangle(x, y, tileSize * scale, tileSize * scale),
                new Rectangle(tileX * tileSize, tileY * tileSize, tileSize, tileSize),
                GraphicsUnit.Pixel);

            g.Restore(gState);
        }
    }
}
