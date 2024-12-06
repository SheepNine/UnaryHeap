using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qtwols
{
    /// <summary>
    /// Represents a WAD2 file from a PAK file (e.g. gfx.wad).
    /// </summary>
    public class Wad2File
    {
        class Wad2Entry
        {
            public int Offset { get; private set; }
            public int DiskSize { get; private set; }
            public int FullSize { get; private set; } // when uncompressed
            public byte Type { get; private set; }
            public byte Compression { get; private set; }
            public byte Pad1 { get; private set; }
            public byte Pad2 { get; private set; }
            public string Name { get; private set; }

            public Wad2Entry(int offset, int diskSize, int fullSize, byte type,
                byte compression, byte pad1, byte pad2, string name)
            {
                Offset = offset;
                DiskSize = diskSize;
                FullSize = fullSize;
                Type = type;
                Compression = compression;
                Pad1 = pad1;
                Pad2 = pad2;
                Name = name;
            }

            public override string ToString()
            {
                return $"{Name}, {DiskSize}/{FullSize}, Type {Type}, "
                    + $"Compression {Compression}, Offset {Offset}";
            }
        }

        readonly Stream data;
        readonly List<Wad2Entry> manifes = new();

        /// <summary>
        /// Initializes a new instance of the Wad2File class.
        /// </summary>
        /// <param name="dataStream">The stream from which to read data.</param>
        public Wad2File(Stream dataStream)
        {
            data = dataStream;
            data.Seek(0, SeekOrigin.Begin);
            var magic = ReadString(4);
            if (!magic.Equals("WAD2", StringComparison.Ordinal))
                throw new InvalidDataException("Stream is not a WAD2 file");
            var manifestCount = ReadLeInt32();
            var manifestOffset = ReadLeInt32();
            data.Seek(manifestOffset, SeekOrigin.Begin);

            foreach (var _ in Enumerable.Range(0, manifestCount))
            {
                var lumpOffset = ReadLeInt32();
                var lumpDiskSize = ReadLeInt32();
                var lumpFullSize = ReadLeInt32();
                var lumpType = ReadByte();
                var lumpCompression = ReadByte();
                var pad1 = ReadByte();
                var pad2 = ReadByte();
                var lumpName = ReadString(16);

                manifes.Add(new Wad2Entry(lumpOffset, lumpDiskSize, lumpFullSize,
                    lumpType, lumpCompression, pad1, pad2, lumpName));
            }
        }

        private string ReadString(int size)
        {
            return Encoding.ASCII.GetString(ReadBytes(size)).TrimEnd('\0');
        }

        private int ReadLeInt32()
        {
            var buffer = ReadBytes(4);
            return BitConverter.ToInt32(buffer, 0);
        }

        private byte[] ReadBytes(int size)
        {
            var buffer = new byte[size];
            var bytesRead = data.Read(buffer, 0, size);
            if (bytesRead != size)
                throw new InvalidDataException("Failed to read string");
            return buffer;
        }

        private byte ReadByte()
        {
            var result = data.ReadByte();
            if (result == -1)
                throw new InvalidDataException("Expected a byte; found none");
            return (byte)result;
        }
    }
}
