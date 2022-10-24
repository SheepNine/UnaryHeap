#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Drawing;

namespace UnaryHeap.Utilities.Misc
{
    /// <summary>
    /// Provides utility methods for working with NES ROMs.
    /// </summary>
    public static class NesCartridge
    {
        /// <summary>
        /// Produces a bitmap representation of a CHR ROM page.
        /// </summary>
        /// <param name="chrRom">The CHR ROM data.</param>
        /// <param name="colors">The colors used to draw the tiles.</param>
        /// <returns>The rasterized bitmap.</returns>
        /// <exception cref="ArgumentNullException">chrRom or colors is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">chrRom.Length is not 8192,
        /// or colors.Length is not 4.</exception>
        public static Bitmap RasterizeNesChrRom(byte[] chrRom, Color[] colors)
        {
            if (chrRom == null)
                throw new ArgumentNullException("chrRom");
            if (chrRom.Length != 8 * 1024)
                throw new ArgumentOutOfRangeException(
                    "chrRom", "CHR ROM must be 8KB in size");
            if (colors == null)
                throw new ArgumentNullException("colors");
            if (colors.Length != 4)
                throw new ArgumentOutOfRangeException(
                    "colors", "Color array must be four elements in size");

            var result = new Bitmap(128, 256);

            for (int tileIndex = 0; tileIndex < 512; tileIndex++)
                RasterizeTile(chrRom, tileIndex, result, colors);

            return result;
        }

        static void RasterizeTile(byte[] chrRom, int tile, Bitmap target, Color[] colors)
        {
            var tileData = new byte[16];
            Array.Copy(chrRom, 16 * tile, tileData, 0, 16);

            var tileX = tile % 16;
            var tileY = tile / 16;
            RasterizeTile(tileData, 8 * tileX, 8 * tileY, target, colors);
        }

        static void RasterizeTile(byte[] tileData, int tileX, int tileY,
            Bitmap target, Color[] colors)
        {
            for (int y = 0; y < 8; y++)
                RasterizeTileRow(tileData[y], tileData[y + 8], tileX, tileY + y, target, colors);
        }

        static void RasterizeTileRow(byte lowBits, byte highBits, int rowX, int rowY,
            Bitmap target, Color[] colors)
        {
            for (int x = 0; x < 8; x++)
            {
                var mask = (1 << (7 - x));
                var pixel = ((lowBits & mask) == mask ? 1 : 0) |
                    ((highBits & mask) == mask ? 2 : 0);
                target.SetPixel(rowX + x, rowY, colors[pixel]);
            }
        }
    }
}

#endif