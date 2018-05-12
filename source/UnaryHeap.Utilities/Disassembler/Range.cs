using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Disassembler
{
    interface Range
    {
        int Start { get; }

        int Consume(Stream source, TextWriter output);
    }

    class UnknownRange : Range
    {
        public int Start { get; private set; }
        int length;

        public UnknownRange(int start, int length)
        {
            Start = start;
            this.length = length;
        }

        public int Consume(Stream source, TextWriter output)
        {
            source.Seek(this.length, SeekOrigin.Current);
            output.WriteLine("{1:X4} Skipped {0} unknown bytes", this.length, Start);
            return this.length;
        }
    }

    class StringRange : Range
    {
        static Dictionary<byte, string> characterMap;

        static StringRange()
        {
            characterMap = new Dictionary<byte, string>()
            {
                { 0x36, "_" },
                { 0x37, "0" },
                { 0x38, "1" },
                { 0x39, "2" },
                { 0x3A, "3" },
                { 0x3B, "4" },
                { 0x3C, "5" },
                { 0x3D, "6" },
                { 0x3E, "7" },
                { 0x3F, "8" },
                { 0x40, "9" },
                { 0x41, "A" },
                { 0x42, "B" },
                { 0x43, "C" },
                { 0x44, "D" },
                { 0x45, "E" },
                { 0x46, "F" },
                { 0x47, "G" },
                { 0x48, "H" },
                { 0x49, "I" },
                { 0x4A, "J" },
                { 0x4B, "K" },
                { 0x4C, "L" },
                { 0x4D, "M" },
                { 0x4E, "N" },
                { 0x4F, "O" },
                { 0x50, "P" },
                { 0x51, "Q" },
                { 0x52, "R" },
                { 0x53, "S" },
                { 0x54, "T" },
                { 0x55, "U" },
                { 0x56, "V" },
                { 0x57, "W" },
                { 0x58, "X" },
                { 0x59, "Y" },
                { 0x5A, "Z" },
                { 0x5B, "-I" },
                { 0x5C, "." },
                { 0x5D, "(tm)" },
            };
        }

        public int Start { get; private set; }

        public StringRange(int start)
        {
            Start = start;
        }

        public int Consume(Stream source, TextWriter output)
        {
            var addrHi = source.SafeReadByte();
            var addrLo = source.SafeReadByte();
            List<byte> chars = new List<byte>();
            var result = 2;
            while (true)
            {
                var chr = source.SafeReadByte();
                chars.Add(chr);
                result += 1;
                if (chr > 0x7F)
                    break;
            }
            output.Write("{0:X4} Text string (written to {1:X2}{2:X2}):", Start, addrHi, addrLo);
            foreach (var chr in chars)
                output.Write(" {0:X2}", chr);
            output.Write(" '");
            foreach (var chr in chars)
                output.Write(characterMap[(byte)(chr & 0x7F)]);
            output.WriteLine("'");
            return result;
        }
    }

    class DescribedRange : Range
    {
        public int Start { get; private set; }
        int length;
        string description;
        int stride;

        public DescribedRange(int start, int length, string description)
            :this (start, length, description, Int32.MaxValue)
        {
        }

        public DescribedRange(int start, int length, string description, int stride)
        {
            Start = start;
            this.length = length;
            this.description = description;
            this.stride = stride;
        }

        public int Consume(Stream source, TextWriter output)
        {
            output.Write("{0:X4} {1}:", Start, description);
            for (int i = 0; i < this.length; i++)
            {
                if (i % stride == 0)
                    output.Write(Environment.NewLine + "\t");
                output.Write("{0:X2} ", source.SafeReadByte());
            }
            output.WriteLine();
            return this.length;
        }
    }

    class EntityTemplateRange : Range
    {
        private static string[] types = new[]
        {
            "UNKNOWN",// 00
            "UNKNOWN",// 01
            "UNKNOWN",// 02
            "UNKNOWN",// 03
            "UNKNOWN",// 04
            "UNKNOWN",// 05
            "UNKNOWN",// 06
            "UNKNOWN",// 07
            "UNKNOWN",// 08
            "UNKNOWN",// 09
            "Door",// 0A
            "Scale",// 0B
            "UNKNOWN",// 0C
            "Snakedozer",// 0D
            "Bladez",// 0E
            "UNKNOWN",// 0F

            "Flag (?)",// 10
            "UNKNOWN",// 11
            "UNKNOWN",// 12
            "Crazy seat/bubble",// 13
            "Pin cushion",// 14
            "UNKNOWN",// 15
            "UNKNOWN",// 16
            "UNKNOWN",// 17
            "UNKNOWN",// 18
            "UNKNOWN",// 19
            "UNKNOWN",// 1A
            "UNKNOWN",// 1B
            "UNKNOWN",// 1C
            "UNKNOWN",// 1D
            "UNKNOWN",// 1E
            "UNKNOWN",// 1F

            "UNKNOWN",// 20
            "Pibbly dispenser",// 21
            "UNKNOWN",// 22
            "UNKNOWN",// 23
            "UNKNOWN",// 24
            "Magic carpet",// 25
            "Jaws (?)",// 26
            "Bigfoot",// 27
            "UNKNOWN",// 28
            "UNKNOWN",// 29
            "UNKNOWN",// 2A
            "UNKNOWN",// 2B
            "Rotating crazy seat",// 2C
            "Bell & fishtail dispenser",// 2D
            "UNKNOWN",// 2E
            "Seaweed",// 2F

            "UNKNOWN",// 30
            "UNKNOWN",// 31
            "UNKNOWN",// 32
            "UNKNOWN",// 33
            "Powerup",// 34
            "Record/mushroom/ice cube",// 35
            "Anvil",// 36
            "Water jet",// 37
            "Stationary metal tree (?)",// 38
            "Metal tree",// 39
            "Metal sphere/snowball/asteriod",// 3A
            "Bell",// 3B
            "UNKNOWN",// 3C
            "Spaceship part 1 (?)",// 3D
            "Spaceship part 2 (?)",// 3E
            "Warp rocket",// 3F
        };

        public int Start { get; private set; }
        int numEntities;
        string description;

        public EntityTemplateRange(int start, int numEntities, string description)
        {
            Start = start;
            this.numEntities = numEntities;
            this.description = description;
        }

        public int Consume(Stream source, TextWriter output)
        {
            output.WriteLine("{0:X4} {1}:", Start, description);
            for (int i  = 0; i < numEntities; i++)
            {
                var bytes = new List<byte>();
                for (int u = 0; u < 7; u++)
                    bytes.Add(source.SafeReadByte());

                output.Write("      ");
                foreach (var bite in bytes)
                    output.Write("{0:X2} ", bite);

                output.Write("    x=");
                output.Write("{0:F4}", (((bytes[1] & 0xE0) << 3) | bytes[2]) / 16.0);
                output.Write(" y=");
                output.Write("{0:F4}", (((bytes[1] & 0x1C) << 6) | bytes[3]) / 16.0);
                output.Write(" type=");
                output.Write(types[bytes[0]]);

                output.WriteLine();
            }
            return numEntities * 7;
        }
    }

    class BackgroundArrangementRange : Range
    {
        public int Start { get; private set; }

        public BackgroundArrangementRange(int start)
        {
            Start = start;
        }

        public int Consume(Stream source, TextWriter output)
        {
            var addrHi = source.SafeReadByte();
            var addrLo = source.SafeReadByte();
            output.WriteLine("{0:X4} Background Arrangement (written to {1:X2}{2:X2}):", Start, addrHi, addrLo);
            var width = source.SafeReadByte();
            var height = source.SafeReadByte();
            for (int row = 0; row < height; row++)
            {
                output.Write("     ");
                for (int col = 0; col < width; col++)
                    output.Write("{0:X2} ", source.SafeReadByte());
                output.WriteLine();
            }
            return width * height + 4;
        }
    }

    class SpriteLayoutRange : Range
    {
        public int Start { get; private set; }
        private string description;

        public SpriteLayoutRange(int start, string description)
        {
            Start = start;
            this.description = description;
        }

        public int Consume(Stream source, TextWriter output)
        {
            var result = 0;
            var control0 = source.SafeReadByte();
            var control1 = source.SafeReadByte();
            var control2 = source.SafeReadByte();
            var control3 = source.SafeReadByte();
            result += 4;

            output.Write("{0:X4} Sprite layout '{1}': ", Start, description);
            output.Write("{0:X2} ", control0);
            output.Write("{0:X2} ", control1);
            output.Write("{0:X2} ", control2);
            output.Write("{0:X2}: ", control3);

            var count = (control3 & 0x7F);
            var sequentialIndices = ((control3 & 0x80) == 0x80);
            var distinctTileAttrs = (control2 & 0xFF) == 0xFF;

            var bytesPerChunk = 2;
            if (distinctTileAttrs)
                bytesPerChunk += 1;
            if (!sequentialIndices)
                bytesPerChunk += 1;

            var chunkDataSize = count * bytesPerChunk + (sequentialIndices ? 1 : 0);

            for (var i = 0; i < count; i++)
            {
                output.Write("|");
                if (i != 0 && sequentialIndices)
                    output.Write("-- ");
                else
                    output.Write("{0:X2} ", source.SafeReadByte());

                output.Write("{0:X2} ", source.SafeReadByte());
                output.Write("{0:X2} ", source.SafeReadByte());

                if (distinctTileAttrs)
                    output.Write("{0:X2}", source.SafeReadByte());
                else
                    output.Write("--");
            }
            output.WriteLine("|");

            return chunkDataSize + 4;
        }
    }
}
