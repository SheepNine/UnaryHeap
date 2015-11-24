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

            AssertHeader(sut, false, 2306, 12371396);
            AssertLump(sut, 0, "PLAYPAL", 12, 10752);
            AssertLump(sut, 177, "SEGS", 1694408, 29256);
            AssertLump(sut, 2305, "F_END", 0, 0);
        }

        [Fact]
        public void Doom2()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Doom 2\base\DOOM2.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            var sut = new WadFile(File.ReadAllBytes(wadFileName));

            AssertHeader(sut, false, 2919, 14557880);
            AssertLump(sut, 0, "PLAYPAL", 12, 10752);
            AssertLump(sut, 177, "SSECTORS", 1590780, 1684);
            AssertLump(sut, 2918, "F_END", 0, 0);
        }

        [Fact]
        public void FinalDoomPlutonia()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Final Doom\base\PLUTONIA.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            using (var stream = File.OpenRead(wadFileName))
            {
                var sut = new WadFile(stream);

                AssertHeader(sut, false, 2984, 17373080);
                AssertLump(sut, 0, "MAP01", 32, 0);
                AssertLump(sut, 177, "THINGS", 1930348, 2880);
                AssertLump(sut, 2983, "F_END", 17373080, 0);
            }
        }

        [Fact]
        public void FinalDoomEvilution()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Final Doom\base\TNT.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            var sut = new WadFile(wadFileName);

            AssertHeader(sut, false, 3101, 18146120);
            AssertLump(sut, 0, "MAP01", 32, 0);
            AssertLump(sut, 177, "THINGS", 2223820, 2600);
            AssertLump(sut, 3100, "F_END", 18146120, 0);
        }

        [Fact]
        public void MasterLevelsTeeth()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Master Levels of Doom\master\wads\TEETH.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            var sut = new WadFile(wadFileName);

            AssertHeader(sut, true, 23, 190163);
            AssertLump(sut, 0, "MAP31", 12, 0);
            AssertLump(sut, 9, "REJECT", 156340, 10513);
            AssertLump(sut, 22, "TAGDESC", 189341, 822);
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

        void AssertHeader(WadFile sut, bool isPatch, int lumpCount, int directoryOffset)
        {
            Assert.Equal(isPatch, sut.IsPatchWad);
            Assert.Equal(lumpCount, sut.LumpCount);
            Assert.Equal(directoryOffset, sut.DirectoryOffset);
        }

        private void AssertLump(WadFile sut, int index, string name, int start, int size)
        {
            Assert.Equal(name, sut.LumpName(index));
            Assert.Equal(start, sut.LumpDataStart(index));
            Assert.Equal(size, sut.LumpDataSize(index));
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
            //if (data.Length < 12)
            //    throw new InvalidDataException("WAD files must contain at least twelve bytes.");

            Init(data);
        }

        void Init(byte[] source)
        {
            data = new byte[source.Length];
            Array.Copy(source, data, source.Length);

            //var identification = Encoding.ASCII.GetString(data, 0, 4);
            //
            //if (!identification.Equals("IWAD") && !identification.Equals("PWAD"))
            //    throw new InvalidDataException("Invalid WAD data: identification incorrect.");
        }

        public bool IsPatchWad
        {
            get { return data[0] == 0x50; }
        }

        public int LumpCount
        {
            get { return ReadLittleEndianInt32(4); }
        }

        public int DirectoryOffset
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
            return Encoding.ASCII.GetString(data, directoryEntryStart + 8, 8).TrimEnd((char)0);
        }

        public int LumpDataStart(int index)
        {
            int directoryEntryStart = DirectoryOffset + 16 * index;
            return ReadLittleEndianInt32(directoryEntryStart);
        }

        public int LumpDataSize(int index)
        {
            int directoryEntryStart = DirectoryOffset + 16 * index;
            return ReadLittleEndianInt32(directoryEntryStart + 4);
        }
    }
}
