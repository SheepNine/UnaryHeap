using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disassembler
{
    class SpriteLayout
    {
        class Chunk
        {
            public byte TileIndex { get; private set; }
            public byte XOffset { get; private set; }
            public byte YOffset { get; private set; }
            public byte Attributes { get; private set; }

            public bool HFlip
            {
                get { return (Attributes & 0x40) == 0x40; }
            }

            public bool VFlip
            {
                get { return (Attributes & 0x80) == 0x80; }
            }

            public Chunk(byte tile, byte x, byte y, byte attrs)
            {
                TileIndex = tile;
                XOffset = x;
                YOffset = y;
                Attributes = attrs;
            }

            public override string ToString()
            {
                return String.Format("{0:X2} {1:X2} {2:X2} {3:X2}", TileIndex, XOffset, YOffset, Attributes);
            }
        }

        public static Bitmap RasterizeSprite(byte[] data, int layoutOffset, int chrPageOffset, Color[] colors)
        {
            var chrPageData = new byte[0x1000];
            Array.Copy(data, chrPageOffset, chrPageData, 0, 0x1000);

            var readHead = layoutOffset;

            var control0 = data[layoutOffset++];
            var control1 = data[layoutOffset++];
            var control2 = data[layoutOffset++];
            var control3 = data[layoutOffset++];

            var count = (control3 & 0x7F);
            var sequentialIndices = ((control3 & 0x80) == 0x80);
            var distinctTileAttrs = (control2 & 0xFF) == 0xFF;

            var chunks = new List<Chunk>();

            var firstTileIndex = data[layoutOffset];
            if (sequentialIndices)
            {
                layoutOffset += 1;
            }

            for (var i = 0; i < count; i++)
            {
                byte tileIndex;
                if (sequentialIndices)
                    tileIndex = (byte)(firstTileIndex + i);
                else
                    tileIndex = data[layoutOffset++];

                var xOffset = data[layoutOffset++];
                var yOffset = data[layoutOffset++];

                byte attributes;
                if (!distinctTileAttrs)
                    attributes = control2;
                else
                    attributes = data[layoutOffset++];

                chunks.Add(new Chunk(tileIndex, xOffset, yOffset, attributes));
            }

            if (count == 0)
            {
                Bitmap bitmap = new Bitmap(1, 1);
                bitmap.SetPixel(0, 0, Color.Black);
                return bitmap;
            }

            var imageWidth = 8 + chunks.Max(chunk => chunk.XOffset);
            var imageHeight = 8 + chunks.Max(chunk => chunk.YOffset);

            var result = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(result))
                g.Clear(Color.FromArgb(0x55, 0x55, 0x55));

            foreach (var chunk in chunks)
            {
                var pattern = Pattern.FromChrRom(chrPageData, chunk.TileIndex);
                pattern.Rasterize(colors, result, chunk.XOffset, imageHeight - 8 - chunk.YOffset, chunk.HFlip, chunk.VFlip);
            }
            return result;
        }
    }
}
