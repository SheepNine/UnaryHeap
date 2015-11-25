using System;
using System.IO;
using System.Text;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class WadFileTests
    {
        [Fact]
        public void UltimateDoom()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Ultimate Doom\base\DOOM.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            var sut = new WadFile(wadFileName);

            AssertHeader(sut, false, 2306);
            AssertLump(sut, 0, "PLAYPAL", 10752);
            AssertLump(sut, 177, "SEGS", 29256);
            AssertLump(sut, 2305, "F_END", 0);
        }

        [Fact]
        public void Doom2()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Doom 2\base\DOOM2.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            var sut = new WadFile(File.ReadAllBytes(wadFileName));

            AssertHeader(sut, false, 2919);
            AssertLump(sut, 0, "PLAYPAL", 10752);
            AssertLump(sut, 177, "SSECTORS", 1684);
            AssertLump(sut, 2918, "F_END", 0);
        }

        [Fact]
        public void FinalDoomPlutonia()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Final Doom\base\PLUTONIA.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            using (var stream = File.OpenRead(wadFileName))
            {
                var sut = new WadFile(stream);

                AssertHeader(sut, false, 2984);
                AssertLump(sut, 0, "MAP01", 0);
                AssertLump(sut, 177, "THINGS", 2880);
                AssertLump(sut, 2983, "F_END", 0);
            }
        }

        [Fact]
        public void FinalDoomEvilution()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Final Doom\base\TNT.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            var sut = new WadFile(wadFileName);

            AssertHeader(sut, false, 3101);
            AssertLump(sut, 0, "MAP01", 0);
            AssertLump(sut, 177, "THINGS", 2600);
            AssertLump(sut, 3100, "F_END", 0);
        }

        [Fact]
        public void MasterLevelsTeeth()
        {
            var wadFileName =
                @"D:\Steam\steamapps\common\Master Levels of Doom\master\wads\TEETH.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            var sut = new WadFile(wadFileName);

            AssertHeader(sut, true, 23);
            AssertLump(sut, 0, "MAP31", 0);
            AssertLump(sut, 9, "REJECT", 10513);
            AssertLump(sut, 22, "TAGDESC", 822);
        }

        [Fact]
        public void EmptyWAD()
        {
            var sut = new WadFile(
                new byte[] {
                    0x49, 0x57, 0x41, 0x44,
                    0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00
                });

            Assert.False(sut.IsPatchWad);
            Assert.Equal(0, sut.LumpCount);
        }

        [Fact]
        public void OneLumpWAD()
        {
            var sut = new WadFile(
                new byte[] {
                    0x49, 0x57, 0x41, 0x44,
                    0x01, 0x00, 0x00, 0x00,
                    0x0C, 0x00, 0x00, 0x00,

                    0x1C, 0x00, 0x00, 0x00,
                    0x04, 0x00, 0x00, 0x00,
                    0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x00,

                    0x01, 0x02, 0x03, 0x00
                });

            Assert.False(sut.IsPatchWad);
            Assert.Equal(1, sut.LumpCount);

            AssertLump(sut, 0, "ABCDEFG", 4);
            AssertLumpContent(sut, 0, new byte[] { 0x01, 0x02, 0x03, 0x00 });
        }

        [Fact]
        public void OneLumpWAD_NonStandardName()
        {
            var sut = new WadFile(
                new byte[] {
                    0x50, 0x57, 0x41, 0x44,
                    0x01, 0x00, 0x00, 0x00,
                    0x0C, 0x00, 0x00, 0x00,

                    0x1C, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
                    0x41, 0x00, 0x43, 0x44, 0x45, 0x46, 0x47, 0x00
                });

            Assert.True(sut.IsPatchWad);
            Assert.Equal(1, sut.LumpCount);

            AssertLump(sut, 0, "A", 0);
            AssertLumpContent(sut, 0, new byte[0]);
        }

        [Fact]
        public void DataVeractiyChecks()
        {
            DataVeracityCheck(
                "WAD files must be at least twelve bytes in size.",
                new byte[] { 0x00, 0x01, 0x02 });
            DataVeracityCheck(
                "Invalid WAD identifier. Valid values are 'IWAD' and 'PWAD'.",
                new byte[] {
                    0x4A, 0x50, 0x45, 0x47,
                    0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00
                });
            DataVeracityCheck(
                "Negative lump count.",
                new byte[] {
                    0x49, 0x57, 0x41, 0x44,
                    0xFF, 0xFF, 0xFF, 0xFF,
                    0x00, 0x00, 0x00, 0x00
                });
            DataVeracityCheck(
                "WAD directory does not lie within file.",
                new byte[] {
                    0x49, 0x57, 0x41, 0x44,
                    0x01, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x20
                });
            DataVeracityCheck(
                "Negative offset at lump 0.",
                new byte[] {
                    0x49, 0x57, 0x41, 0x44,
                    0x01, 0x00, 0x00, 0x00,
                    0x0C, 0x00, 0x00, 0x00,

                    0xFF, 0xFF, 0xFF, 0xFF,
                    0x00, 0x00, 0x00, 0x00,
                    0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x00
                });
            DataVeracityCheck(
                "Negative size at lump 0.",
                new byte[] {
                    0x49, 0x57, 0x41, 0x44,
                    0x01, 0x00, 0x00, 0x00,
                    0x0C, 0x00, 0x00, 0x00,

                    0x00, 0x00, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF,
                    0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x00
                });
            DataVeracityCheck(
                "Lump 0 does not lie within file.",
                new byte[] {
                    0x49, 0x57, 0x41, 0x44,
                    0x01, 0x00, 0x00, 0x00,
                    0x0C, 0x00, 0x00, 0x00,

                    0x1C, 0x00, 0x00, 0x00,
                    0x05, 0x00, 0x00, 0x00,
                    0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x00,

                    0x01, 0x02, 0x03, 0x00
                });
        }

        public void DataVeracityCheck(string expectedMessage, byte[] data)
        {
            Assert.Equal(expectedMessage,
                Assert.Throws<InvalidDataException>(() =>
                    { new WadFile(data); }).Message);
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("data", () =>
                { new WadFile((byte[])null); });
            Assert.Throws<ArgumentNullException>("filename", () =>
                { new WadFile((string)null); });
            Assert.Throws<ArgumentNullException>("source", () =>
                { new WadFile((Stream)null); });
        }

        void AssertHeader(WadFile sut, bool isPatch, int lumpCount)
        {
            Assert.Equal(isPatch, sut.IsPatchWad);
            Assert.Equal(lumpCount, sut.LumpCount);
        }

        void AssertLump(WadFile sut, int index, string name, int size)
        {
            Assert.Equal(name, sut.LumpName(index));
            Assert.Equal(size, sut.LumpDataSize(index));
        }

        void AssertLumpContent(WadFile sut, int index, byte[] expected)
        {
            Assert.Equal(expected, sut.GetLumpData(index));
        }
    }

    public class WadFile
    {
        byte[] data;

        public WadFile(Stream source)
        {
            if (null == source)
                throw new ArgumentNullException("source");

            using (var buffer = new MemoryStream())
            {
                source.CopyTo(buffer);
                Init(buffer.ToArray());
            }
        }

        public WadFile(string filename)
        {
            if (null == filename)
                throw new ArgumentNullException("filename");

            Init(File.ReadAllBytes(filename));
        }

        public WadFile(byte[] data)
        {
            if (null == data)
                throw new ArgumentNullException("data");
            if (data.Length < 12)
                throw new InvalidDataException(
                    "WAD files must be at least twelve bytes in size.");

            Init(data);
        }

        void Init(byte[] source)
        {
            data = new byte[source.Length];
            Array.Copy(source, data, source.Length);

            var identification = Encoding.ASCII.GetString(data, 0, 4);

            if (!identification.Equals("IWAD") && !identification.Equals("PWAD"))
                throw new InvalidDataException(
                    "Invalid WAD identifier. Valid values are 'IWAD' and 'PWAD'.");

            if (0 > LumpCount)
                throw new InvalidDataException("Negative lump count.");
            if (false == CheckDataRange(DirectoryOffset, LumpCount * 16))
                throw new InvalidDataException("WAD directory does not lie within file.");

            for (int i = 0; i < LumpCount; i++)
            {
                if (0 > LumpDataStart(i))
                    throw new InvalidDataException(
                        string.Format("Negative offset at lump {0}.", i));
                if (0 > LumpDataSize(i))
                    throw new InvalidDataException(
                        string.Format("Negative size at lump {0}.", i));

                if (false == CheckDataRange(LumpDataStart(i), LumpDataSize(i)))
                    throw new InvalidDataException(string.Format(
                        "Lump {0} does not lie within file.", i));
            }
        }

        bool CheckDataRange(int offset, int size)
        {
            var start = offset;
            var end = offset + size;

            return start >= 0 && end <= data.Length;
        }

        public bool IsPatchWad
        {
            get { return data[0] == 0x50; }
        }

        public int LumpCount
        {
            get { return ReadLittleEndianInt32(4); }
        }

        int DirectoryOffset
        {
            get { return ReadLittleEndianInt32(8); }
        }

        int ReadLittleEndianInt32(int index)
        {
            return (((((data[index + 3] << 8)
                | data[index + 2]) << 8)
                | data[index + 1]) << 8)
                | data[index + 0];
        }

        public string LumpName(int index)
        {
            int directoryEntryStart = DirectoryOffset + 16 * index;
            var result = Encoding.ASCII.GetString(data, directoryEntryStart + 8, 8);
            var firstNullIndex = result.IndexOf((char)0);

            if (-1 == firstNullIndex)
                return result;
            else
                return result.Substring(0, firstNullIndex);
        }

        int LumpDataStart(int index)
        {
            int directoryEntryStart = DirectoryOffset + 16 * index;
            return ReadLittleEndianInt32(directoryEntryStart);
        }

        public int LumpDataSize(int index)
        {
            int directoryEntryStart = DirectoryOffset + 16 * index;
            return ReadLittleEndianInt32(directoryEntryStart + 4);
        }

        public byte[] GetLumpData(int index)
        {
            return Subset(LumpDataStart(index), LumpDataSize(index));
        }

        byte[] Subset(int offset, int size)
        {
            if (0 == size)
                return new byte[0];

            var result = new byte[size];
            Array.Copy(data, offset, result, 0, size);
            return result;
        }
    }
}
