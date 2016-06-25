#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.IO;

namespace UnaryHeap.Utilities.Retrographic
{
    class Color
    {
        // |               1               |               0               |
        // |15 |14 |13 |12 |11 |10 | 9 | 8 | 7 | 6 | 5 | 4 | 3 | 2 | 1 | 0 |
        // |---|-------------------|-------------------|-------------------|
        // | T |        RED        |       GREEN       |       BLUE        |
        // |---|-----------^-------|-------^-----------|---^---------------|
        const int TransparentMask = 0x8000;
        const int RedMask = 0x7C00;
        const int GreenMask = 0x03E0;
        const int BlueMask = 0x001F;

        const int UnusedColorBits = 0x07;

        public bool Transparent { get; private set; }
        public byte R { get; private set; }
        public byte G { get; private set; }
        public byte B { get; private set; }

        public Color(bool transparent, byte r, byte g, byte b)
        {
            if ((r & UnusedColorBits) != 0)
                throw new ArgumentOutOfRangeException("r");
            if ((g & UnusedColorBits) != 0)
                throw new ArgumentOutOfRangeException("g");
            if ((b & UnusedColorBits) != 0)
                throw new ArgumentOutOfRangeException("b");

            Transparent = transparent;
            R = r;
            G = g;
            B = b;
        }

        public void Serialize(Stream output)
        {
            int encodedBytes =
                (Transparent ? TransparentMask : 0) |
                (R << 7) |
                (G << 2) |
                (B >> 3);

            output.WriteByte((byte)(encodedBytes));
            output.WriteByte((byte)(encodedBytes >> 8));
        }

        public static Color Deserialize(Stream input)
        {
            int byte1 = input.ReadByte();
            if (byte1 == -1)
                throw new InvalidDataException("End of stream reached");
            int byte2 = input.ReadByte();
            if (byte2 == -1)
                throw new InvalidDataException("End of stream reached");

            int encodedBytes = (byte2 << 8) | byte1;

            return new Color(
                (encodedBytes & TransparentMask) == TransparentMask,
                (byte)((encodedBytes & RedMask) >> 7),
                (byte)((encodedBytes & GreenMask) >> 2),
                (byte)((encodedBytes & BlueMask) << 3)
            );
        }
    }
}

#endif