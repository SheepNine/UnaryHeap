#if INCLUDE_WORK_IN_PROGRESS_RETROGRAPHIC

using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace UnaryHeap.Utilities.Retrographic
{
    class Tile
    {
        // |              31               |   |               1               |               0               |
        // |255|254|253|252|251|250|249|248|...|15 |14 |13 |12 |11 |10 | 9 | 8 | 7 | 6 | 5 | 4 | 3 | 2 | 1 | 0 |
        // |---------------|---------------|   |---------------|---------------|---------------|---------------|
        // |    Pixel62    |    Pixel63    |   |    Pixel 2    |    Pixel 3    |    Pixel 0    |    Pixel 1    |
        // |---------------|---------------|   |---------------|---------------|---------------|---------------|

        //  0  1  2  3  4  5  6  7
        //  8                   15
        // 16                   23
        // 24                   31
        // 32                   39
        // 40                   47
        // 48                   55
        // 56 57 58 59 60 61 62 63

        int[] pixels;

        public Tile(string data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (pixels.Length != RG.NUM_TILE_PIXELS)
                throw new ArgumentException("Expected 64 hexadecimal characters", "data");

            // TODO: optimize (int.Parse is extravagant)
            pixels = data
                .Select(c => int.Parse(new string(c, 1), NumberStyles.HexNumber))
                .ToArray();
        }

        public Tile(int[] pixels)
        {
            if (pixels == null)
                throw new ArgumentNullException("pixels");
            if (pixels.Length != RG.NUM_TILE_PIXELS)
                throw new ArgumentException();
            if (pixels.Any(pixel => pixel < 0 || pixel >= 16))
                throw new ArgumentOutOfRangeException("pixels");

            this.pixels = new int[64];
            Array.Copy(pixels, 0, this.pixels, 0, 64);
        }

        public static Tile Clone(Tile source)
        {
            using (var buffer = new MemoryStream())
            {
                source.Serialize(buffer);
                buffer.Seek(0, SeekOrigin.Begin);
                return Tile.Deserialize(buffer);
            }
        }

        public int this[int x, int y]
        {
            get
            {
                RG.CheckTileCoordinates(x, y);
                return pixels[(y << 3) | x];
            }
        }

        public void Serialize(Stream output)
        {
            var i = 0;
            while (i < RG.NUM_TILE_PIXELS)
            {
                var lowPixel = pixels[i];
                i += 1;
                var highPixel = pixels[i];
                i += 1;

                output.WriteByte((byte)((lowPixel << 4) | highPixel));
            }
        }

        public static Tile Deserialize(Stream input)
        {
            var pixels = new int[RG.NUM_TILE_PIXELS];
            var i = 0;
            while (i < RG.NUM_TILE_PIXELS)
            {
                var pixelPair = input.ReadByte();
                if (-1 == pixelPair)
                    throw new InvalidDataException("End of stream reached");

                pixels[i] = pixelPair >> 4;
                i += 1;
                pixels[i] = pixelPair & 0xF;
                i += 1;
            }
            return new Tile(pixels);
        }
    }
}

#endif