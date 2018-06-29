using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Disassembler
{
    interface Range
    {
        int Start { get; }

        int Consume(Stream source, IDisassemblerOutput output, Annotations labels, string category);
    }

    class UnknownRange : DescribedRange
    {
        public UnknownRange(int start, int length)
            : base(start, length, string.Format("{0} bytes, purpose unknown", length))
        {
        }
    }

    class StringRange : Range
    {
        static Dictionary<byte, string> characterMap;

        static StringRange()
        {
            characterMap = new Dictionary<byte, string>()
            {
                { 0x36, " " },
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

        public int Consume(Stream source, IDisassemblerOutput output, Annotations labels, string category)
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
            output.WriteSectionHeader(string.Format("String data \'{0}\'", string.Join("", chars.Select(c => characterMap[(byte)(c & 0x7F)]))));
            output.WriteRawData((ushort)Start, new[] { addrHi, addrLo }, labels, category);
            output.WriteRawData(null, chars, labels, category);

            return result;
        }
    }

    class DescribedRange : Range
    {
        public int Start { get; private set; }
        int length;
        string description;
        int? stride;
        bool binary;

        public DescribedRange(int start, int length, string description)
            : this (start, length, description, null)
        {
        }

        public DescribedRange(int start, int length, string description, int? stride)
            : this(start, length, description, stride, false)
        {
        }

        public DescribedRange(int start, int length, string description, int? stride, bool binary)
        {
            Start = start;
            this.length = length;
            this.description = description;
            this.stride = stride;
            this.binary = binary;
        }

        public int Consume(Stream source, IDisassemblerOutput output, Annotations labels, string category)
        {
            output.WriteSectionHeader(description);
            ushort? start = (ushort)Start;

            var data = new List<byte>();
            foreach (var i in Enumerable.Range(0, length))
                data.Add(source.SafeReadByte());

            if (stride.HasValue)
            {
                int offset = 0;
                while (offset < length)
                {
                    output.WriteRawData(start, data.GetRange(offset, Math.Min(stride.Value, data.Count - offset)), labels, category);
                    offset += stride.Value;
                    start = null;
                }
            }
            else
            {
                output.WriteRawData(start, data, labels, category);
            }

            return length;
        }

        private string getBit(byte b, int bit)
        {
            int mask = 0x1 << bit;
            return (b & mask) == mask ? "1" : "0";
        }
    }

    class LidManifestRange : Range
    {
        private static string[] types = {
            "XXXXX", // 0, AI $16
            "Red pibbly", // 1, AI $06
            "XXXXX", // 2, AI $1F
            "Corkscrew", // 3, AI $20
            "Crazy seat", // 4, AI $22
            "Clock", // 5, AI $1C
            "Bonus", // 6, AI $23
            "1-UP", // 7, AI $1E
            "BigFoot", // 8, AI $1A
            "Blue pibbly", // 9, AI $28
            "Gold pibbly", // A, AI $29
            "Diamond", // B, AI $1D
            "Warp", // C, AI $23
            "Fake 1-UP", // D, AI $2B
        };

        public int Start { get; private set; }
        int numLids;
        int level;

        public LidManifestRange(int start, int numLids, int level)
        {
            Start = start;
            this.numLids = numLids;
            this.level = level;
        }

        public int Consume(Stream source, IDisassemblerOutput output, Annotations labels, string category)
        {
            output.WriteSectionHeader("Lid contents for level " + level);
            ushort? start = (ushort)Start;

            for (int i = 0; i < numLids; i++)
            {
                var byte0 = source.SafeReadByte();
                var byte1 = source.SafeReadByte();

                var x = ((byte0 & 0xF0) >> 4) | ((byte0 & 0x0F) << 4);
                var y = (byte1 & 0xF0) >> 4;
                var type = (byte1 & 0x0F);
                string info = string.Format("{0} at (${1:X2},$-{2:X1})", types[type], x, y);

                output.WriteRawData(start, new byte[] { byte0, byte1 }, labels, category);
                start = null;
            }

            return numLids * 2;
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
            "Pibbly spawn",// 06, also pibblejogger
            "UNKNOWN",// 07
            "UNKNOWN",// 08
            "Pibblesplat",// 09
            "Door",// 0A
            "Scale",// 0B
            "Pibblebat",// 0C
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
            "Bonus stage context",// 26
            "Bigfoot spawner",// 27
            "UNKNOWN",// 28
            "UNKNOWN",// 29
            "UNKNOWN",// 2A
            "UNKNOWN",// 2B
            "Rotating crazy seat",// 2C
            "Bell & fishtail dispenser",// 2D
            "UNKNOWN",// 2E
            "Seaweed",// 2F

            "Pibblefish",// 30
            "Nibbly pibbly",// 31
            "Pibboing",// 32
            "Pibblecopter",// 33
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

        public int Consume(Stream source, IDisassemblerOutput output, Annotations labels, string category)
        {
            output.WriteSectionHeader(description);
            ushort? start = (ushort)Start;

            foreach (var i in Enumerable.Range(0, numEntities))
            {
                var bytes = new List<byte>();
                for (int u = 0; u < 7; u++)
                    bytes.Add(source.SafeReadByte());

                output.WriteRawData(start, bytes, labels, category);
                start = null;

                // TODO: restore this
                /*output.Write("    x=");
                output.Write("{0:F4}", (((bytes[1] & 0xE0) << 3) | bytes[2]) / 16.0);
                output.Write(" y=");
                output.Write("{0:F4}", (((bytes[1] & 0x1C) << 6) | bytes[3]) / 16.0);
                output.Write(" type=");
                output.Write(types[bytes[0]]);
                output.WriteLine();*/
            }
            return numEntities * 7;
        }
    }

    class BackgroundArrangementRange : Range
    {
        public int Start { get; private set; }
        string description;

        public BackgroundArrangementRange(int start, string description)
        {
            Start = start;
            this.description = description;
        }

        public int Consume(Stream source, IDisassemblerOutput output, Annotations labels, string category)
        {
            output.WriteSectionHeader(string.Format("Background arrangement '{0}'", description));
            var addrHi = source.SafeReadByte();
            var addrLo = source.SafeReadByte();
            var width = source.SafeReadByte();
            var height = source.SafeReadByte();

            output.WriteRawData((ushort)Start, new[] { addrHi, addrLo }, labels, category);
            output.WriteRawData(null, new[] { width, height }, labels, category);

            foreach (var i in Enumerable.Range(0, height))
            {
                var data = new List<byte>();
                foreach (var j in Enumerable.Range(0, width))
                    data.Add(source.SafeReadByte());
                output.WriteRawData(null, data, labels, category);
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

        public int Consume(Stream source, IDisassemblerOutput output, Annotations labels, string category)
        {
            var result = 0;
            var control0 = source.SafeReadByte();
            var control1 = source.SafeReadByte();
            var control2 = source.SafeReadByte();
            var control3 = source.SafeReadByte();
            result += 4;

            output.WriteSectionHeader(string.Format("Composite '{0}'", description));

            /*output.Write(" {0:X2}", control0);
            output.Write(" {0:X2}", control1);
            output.Write(" {0:X2}", control2);
            output.WriteLine(" {0:X2}", control3);*/

            var count = (control3 & 0x7F);
            var sequentialIndices = ((control3 & 0x80) == 0x80);
            var distinctTileAttrs = (control2 & 0xFF) == 0xFF;

            var bytesPerChunk = 2;
            if (distinctTileAttrs)
                bytesPerChunk += 1;
            if (!sequentialIndices)
                bytesPerChunk += 1;

            var chunkDataSize = count * bytesPerChunk + (sequentialIndices ? 1 : 0);

            output.WriteRawData((ushort)Start, new[] { control0, control1, control2, control3 }, labels, category);

            var data = new List<byte>();
            foreach (var i in Enumerable.Range(0, chunkDataSize))
                data.Add(source.SafeReadByte());

            output.WriteRawData(null, data, labels, category);

            // TODO: restore this
            /*for (var i = 0; i < count; i++)
            {
                output.Write("                      .DATA");
                if (i != 0 && sequentialIndices)
                    output.Write("   ");
                else
                    output.Write(" {0:X2}", source.SafeReadByte());

                output.Write(" {0:X2}", source.SafeReadByte());
                output.Write(" {0:X2}", source.SafeReadByte());

                if (distinctTileAttrs)
                    output.Write(" {0:X2}", source.SafeReadByte());

                output.WriteLine();
            }*/

            return chunkDataSize + 4;
        }
    }
}
