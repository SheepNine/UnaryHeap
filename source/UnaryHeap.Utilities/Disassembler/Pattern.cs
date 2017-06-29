using System;
using System.Drawing;

namespace Disassembler
{
    class Pattern
    {
        byte[] data;

        private Pattern(byte[] data)
        {
            this.data = data;
        }

        public static Pattern FromChrRom(byte[] chrRom, byte index)
        {
            int start = index * 16;
            byte[] data = new byte[16];
            Array.Copy(chrRom, start, data, 0, 16);
            return new Pattern(data);
        }

        public Bitmap Rasterize(Color[] palette)
        {
            var result = new Bitmap(8, 8);
            for (var y = 0; y < 8; y++)
            {
                var loByte = data[y];
                var hiByte = data[8 + y];

                RasterizeRow(result, y, loByte, hiByte, palette);
            }
            return result;
        }

        private void RasterizeRow(Bitmap bitmap, int y, byte loByte, byte hiByte, Color[] palette)
        {
            for (var x = 0; x < 8; x++)
            {
                var mask = ((byte)0x80 >> x);
                int colorIndex = ((mask & loByte) == 0 ? 0 : 1) | ((mask & hiByte) == 0 ? 0 : 2);
                bitmap.SetPixel(x, y, palette[colorIndex]);
            }
        }

        public static Bitmap RasterizeChrRomPage(byte[] data, int offset, Color[] palette)
        {
            var chrRomData = new byte[0x1000];
            Array.Copy(data, offset, chrRomData, 0, 0x1000);
            return RasterizeChrRomPage(chrRomData, palette);
        }

        public static Bitmap RasterizeChrRomPage(byte[] pageData, Color[] palette)
        {
            var result = new Bitmap(128, 128);
            using (var g = Graphics.FromImage(result))
            {
                for (var patternIndex = 0; patternIndex < 0x100; patternIndex++)
                {
                    var x = patternIndex % 16;
                    var y = patternIndex / 16;

                    var pattern = Pattern.FromChrRom(pageData, (byte)patternIndex);
                    using (var raster = pattern.Rasterize(palette))
                        g.DrawImage(raster, x * 8, y * 8);
                }
            }
            return result;
        }
    }
}
