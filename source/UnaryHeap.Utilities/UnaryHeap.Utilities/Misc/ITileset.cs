using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace UnaryHeap.Utilities.Misc
{
    /// <summary>
    /// Represents a collection of image tiles.
    /// </summary>
    public interface ITileset : IDisposable
    {
        /// <summary>
        /// Gets the size in pixels of a single tile in this Tileset.
        /// </summary>
        int TileSize { get; }

        /// <summary>
        /// Gets the number of tiles in this Tileset.
        /// </summary>
        int NumTiles { get; }

        /// <summary>
        /// Gets the width in pixels of the underlying Tileset image.
        /// </summary>
        int ImageWidth { get; }

        /// <summary>
        /// Gets the height in pixels of the underlying Tileset image.
        /// </summary>
        int ImageHeight { get; }

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
        void DrawTile(Graphics g, int tileIndex, int x, int y, int scale = 1);
    }
}
