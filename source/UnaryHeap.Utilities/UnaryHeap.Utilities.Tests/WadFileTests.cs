using NUnit.Framework;
using System;
using System.IO;
using UnaryHeap.Utilities.Doom;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class WadFileTests
    {
        [Test]
        public void UltimateDoom()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Ultimate Doom\base\DOOM.WAD";
            if (!File.Exists(wadFileName))
                Assert.Inconclusive("Could not locate input file.");

            var sut = new WadFile(wadFileName);

            AssertHeader(sut, false, 2306);
            AssertLump(sut, 0, "PLAYPAL", 10752);
            AssertLump(sut, 177, "SEGS", 29256);
            AssertLump(sut, 2305, "F_END", 0);

            Assert.AreEqual(84, sut.FindLumpByName("E1M8"));
            Assert.AreEqual(84, sut.FindLumpByName("E1M8", 0));
            Assert.AreEqual(84, sut.FindLumpByName("E1M8", 84));
            Assert.AreEqual(-1, sut.FindLumpByName("E1M8", 85));
        }

        [Test]
        public void Doom2()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Doom 2\base\DOOM2.WAD";
            if (!File.Exists(wadFileName))
                Assert.Inconclusive("Could not locate input file.");

            var sut = new WadFile(File.ReadAllBytes(wadFileName));

            AssertHeader(sut, false, 2919);
            AssertLump(sut, 0, "PLAYPAL", 10752);
            AssertLump(sut, 177, "SSECTORS", 1684);
            AssertLump(sut, 2918, "F_END", 0);
        }

        [Test]
        public void FinalDoomPlutonia()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Final Doom\base\PLUTONIA.WAD";
            if (!File.Exists(wadFileName))
                Assert.Inconclusive("Could not locate input file.");

            using (var stream = File.OpenRead(wadFileName))
            {
                var sut = new WadFile(stream);

                AssertHeader(sut, false, 2984);
                AssertLump(sut, 0, "MAP01", 0);
                AssertLump(sut, 177, "THINGS", 2880);
                AssertLump(sut, 2983, "F_END", 0);
            }
        }

        [Test]
        public void FinalDoomEvilution()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Final Doom\base\TNT.WAD";
            if (!File.Exists(wadFileName))
                Assert.Inconclusive("Could not locate input file.");

            var sut = new WadFile(wadFileName);

            AssertHeader(sut, false, 3101);
            AssertLump(sut, 0, "MAP01", 0);
            AssertLump(sut, 177, "THINGS", 2600);
            AssertLump(sut, 3100, "F_END", 0);
        }

        [Test]
        public void MasterLevelsTeeth()
        {
            var wadFileName =
                @"D:\Steam\steamapps\common\Master Levels of Doom\master\wads\TEETH.WAD";
            if (!File.Exists(wadFileName))
                Assert.Inconclusive("Could not locate input file.");

            var sut = new WadFile(wadFileName);

            AssertHeader(sut, true, 23);
            AssertLump(sut, 0, "MAP31", 0);
            AssertLump(sut, 9, "REJECT", 10513);
            AssertLump(sut, 22, "TAGDESC", 822);
        }

        [Test]
        public void EmptyWAD()
        {
            var sut = new WadFile(
                new byte[] {
                    0x49, 0x57, 0x41, 0x44,
                    0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00
                });

            Assert.False(sut.IsPatchWad);
            Assert.AreEqual(0, sut.LumpCount);
            Assert.AreEqual(-1, sut.FindLumpByName("NEVERMOR"));
        }

        [Test]
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
            Assert.AreEqual(1, sut.LumpCount);

            AssertLump(sut, 0, "ABCDEFG", 4);
            AssertLumpContent(sut, 0, new byte[] { 0x01, 0x02, 0x03, 0x00 });
        }

        [Test]
        public void RangeChecks()
        {
            var sut = new WadFile(
                new byte[]
                {
                    0x49, 0x57, 0x41, 0x44,
                    0x01, 0x00, 0x00, 0x00,
                    0x0C, 0x00, 0x00, 0x00,

                    0x1C, 0x00, 0x00, 0x00,
                    0x04, 0x00, 0x00, 0x00,
                    0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x00,

                    0x01, 0x02, 0x03, 0x00
                });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.GetLumpName(-1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.GetLumpSize(-1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.GetLumpData(-1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.FindLumpByName("COLLEEN", -1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.GetLumpName(1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.GetLumpSize(1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.GetLumpData(1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.FindLumpByName("COLLEEN", 1); });

            Assert.Throws<ArgumentNullException>(
                () => { sut.FindLumpByName(null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.FindLumpByName(null, 0); });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.FindLumpByName("NINECHARS"); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.FindLumpByName("NINECHARS", 0); });
        }

        [Test]
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
            Assert.AreEqual(1, sut.LumpCount);

            AssertLump(sut, 0, "A", 0);
            AssertLumpContent(sut, 0, new byte[0]);
        }

        [Test]
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
            Assert.AreEqual(expectedMessage,
                Assert.Throws<InvalidDataException>(() =>
                    { new WadFile(data); }).Message);
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(() =>
                { new WadFile((byte[])null); });
            Assert.Throws<ArgumentNullException>(() =>
                { new WadFile((string)null); });
            Assert.Throws<ArgumentNullException>(() =>
                { new WadFile((Stream)null); });
        }

        void AssertHeader(WadFile sut, bool isPatch, int lumpCount)
        {
            Assert.AreEqual(isPatch, sut.IsPatchWad);
            Assert.AreEqual(lumpCount, sut.LumpCount);
        }

        void AssertLump(WadFile sut, int index, string name, int size)
        {
            Assert.AreEqual(name, sut.GetLumpName(index));
            Assert.AreEqual(size, sut.GetLumpSize(index));
        }

        void AssertLumpContent(WadFile sut, int index, byte[] expected)
        {
            Assert.AreEqual(expected, sut.GetLumpData(index));
        }
    }
}
