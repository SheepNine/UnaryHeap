using NUnit.Framework;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace UnaryHeap.Mosaic.Tests
{
    [TestFixture]
    public class TileArrangementTests
    {
        [Test]
        public void ConstructorAccessorMutator()
        {
            var sut = new TileArrangement(4, 3);

            Assert.AreEqual(4, sut.TileCountX);
            Assert.AreEqual(3, sut.TileCountY);

            foreach (var x in Enumerable.Range(0, 4))
                foreach (var y in Enumerable.Range(0, 3))
                {
                    var replacement = x * 10 + y;

                    Assert.AreEqual(0, sut[x, y]);
                    sut[x, y] = replacement;
                    Assert.AreEqual(replacement, sut[x, y]);
                }
        }

        [Test]
        public void Serialization()
        {
            var sut = new TileArrangement(4, 3);

            foreach (var x in Enumerable.Range(0, 4))
                foreach (var y in Enumerable.Range(0, 3))
                    sut[x, y] = x * 10 + y;

            var buffer = new MemoryStream();
            sut.Serialize(buffer);
            buffer.Seek(0, SeekOrigin.Begin);
            var sut2 = TileArrangement.Deserialize(buffer);

            Assert.AreEqual(4, sut2.TileCountX);
            Assert.AreEqual(3, sut2.TileCountY);

            foreach (var x in Enumerable.Range(0, 4))
                foreach (var y in Enumerable.Range(0, 3))
                    Assert.AreEqual(x * 10 + y, sut2[x, y]);

            Assert.AreEqual(new byte[] {
                4, 0, 0, 0,
                3, 0, 0, 0,

                0, 0, 0, 0,
                10, 0, 0, 0,
                20, 0, 0, 0,
                30, 0, 0, 0,

                1, 0, 0, 0,
                11, 0, 0, 0,
                21, 0, 0, 0,
                31, 0, 0, 0,

                2, 0, 0, 0,
                12, 0, 0, 0,
                22, 0, 0, 0,
                32, 0, 0, 0,
            }, buffer.ToArray());
        }

        [Test]
        public void Render()
        {
            var sut = new TileArrangement(13, 6);

            foreach (var y in Enumerable.Range(0, 6))
                foreach (var x in Enumerable.Range(0, 13))
                    sut[x, y] = (x > y) ? x : 12 - y;

            using (var output = new Bitmap(13 * 8, 6 * 8))
            {
                using (var g = Graphics.FromImage(output))
                {
                    g.Clear(Color.CornflowerBlue);

                    var tilesetBitmap = new Bitmap(@"data\TilesetTests\tileset.png");
                    using (var tileset = new ImageTileset(tilesetBitmap, 8))
                        sut.Render(g, tileset);
                }

                output.Save(@"data\TileArrangementTests\actual.png");
            }

            ImageTilesetTests.ImageCompare(
                @"data\TileArrangementTests\expected.png",
                @"data\TileArrangementTests\actual.png");
        }

        [Test]
        public void RenderScaled()
        {
            var sut = new TileArrangement(13, 6);

            foreach (var y in Enumerable.Range(0, 6))
                foreach (var x in Enumerable.Range(0, 13))
                    sut[x, y] = (x > y) ? x : 12 - y;

            using (var output = new Bitmap(13 * 8 * 2, 6 * 8 * 2))
            {
                using (var g = Graphics.FromImage(output))
                {
                    g.Clear(Color.CornflowerBlue);

                    var tilesetBitmap = new Bitmap(@"data\TilesetTests\tileset.png");
                    using (var tileset = new ImageTileset(tilesetBitmap, 8))
                        sut.Render(g, tileset, 2);
                }

                output.Save(@"data\TileArrangementTests\actual2x.png");
            }

            ImageTilesetTests.ImageCompare(
                @"data\TileArrangementTests\expected2x.png",
                @"data\TileArrangementTests\actual2x.png");
        }

        [Test]
        public void Clone()
        {
            var original = new TileArrangement(3, 2);

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 3))
                    original[x, y] = x + 3 * y;

            var duplicate = original.Clone();

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 3))
                {
                    Assert.AreEqual(original[x, y], duplicate[x, y]);

                    duplicate[x, y] = 10;

                    Assert.AreEqual(x + 3 * y, original[x, y]);
                    Assert.AreEqual(10, duplicate[x, y]);
                }
        }

        [Test]
        public void ExpandRight()
        {
            var sut = new TileArrangement(3, 2);

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 3))
                    sut[x, y] = x + 3 * y;

            sut.ExpandRight();

            Assert.AreEqual(4, sut.TileCountX);
            Assert.AreEqual(2, sut.TileCountY);

            foreach (var y in Enumerable.Range(0, 2))
            {
                foreach (var x in Enumerable.Range(0, 3))
                    Assert.AreEqual(x + 3 * y, sut[x, y]);

                Assert.AreEqual(0, sut[3, y]);
            }
        }

        [Test]
        public void ExpandLeft()
        {
            var sut = new TileArrangement(3, 2);

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 3))
                    sut[x, y] = x + 3 * y;

            sut.ExpandLeft();

            Assert.AreEqual(4, sut.TileCountX);
            Assert.AreEqual(2, sut.TileCountY);

            foreach (var y in Enumerable.Range(0, 2))
            {
                Assert.AreEqual(0, sut[0, y]);

                foreach (var x in Enumerable.Range(0, 3))
                    Assert.AreEqual(x + 3 * y, sut[x + 1, y]);
            }
        }

        [Test]
        public void ExpandBottom()
        {
            var sut = new TileArrangement(3, 2);

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 3))
                    sut[x, y] = x + 3 * y;

            sut.ExpandBottom();

            Assert.AreEqual(3, sut.TileCountX);
            Assert.AreEqual(3, sut.TileCountY);

            foreach (var x in Enumerable.Range(0, 3))
            {
                foreach (var y in Enumerable.Range(0, 2))
                    Assert.AreEqual(x + 3 * y, sut[x, y]);

                Assert.AreEqual(0, sut[x, 2]);
            }
        }

        [Test]
        public void ExpandTop()
        {
            var sut = new TileArrangement(3, 2);

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 3))
                    sut[x, y] = x + 3 * y;

            sut.ExpandTop();

            Assert.AreEqual(3, sut.TileCountX);
            Assert.AreEqual(3, sut.TileCountY);

            foreach (var x in Enumerable.Range(0, 3))
            {
                Assert.AreEqual(0, sut[x, 0]);

                foreach (var y in Enumerable.Range(0, 2))
                    Assert.AreEqual(x + 3 * y, sut[x, y + 1]);
            }
        }

        [Test]
        public void ContractRight()
        {
            var sut = new TileArrangement(3, 2);

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 3))
                    sut[x, y] = x + 3 * y;

            sut.ContractRight();

            Assert.AreEqual(2, sut.TileCountX);
            Assert.AreEqual(2, sut.TileCountY);

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 2))
                    Assert.AreEqual(x + 3 * y, sut[x, y]);
        }

        [Test]
        public void ContractLeft()
        {
            var sut = new TileArrangement(3, 2);

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 3))
                    sut[x, y] = x + 3 * y;

            sut.ContractLeft();

            Assert.AreEqual(2, sut.TileCountX);
            Assert.AreEqual(2, sut.TileCountY);

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 2))
                    Assert.AreEqual((x + 1) + 3 * y, sut[x, y]);
        }

        [Test]
        public void ContractBottom()
        {
            var sut = new TileArrangement(3, 2);

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 3))
                    sut[x, y] = x + 3 * y;

            sut.ContractBottom();

            Assert.AreEqual(3, sut.TileCountX);
            Assert.AreEqual(1, sut.TileCountY);
            
            foreach (var x in Enumerable.Range(0, 3))
                Assert.AreEqual(x, sut[x, 0]);
        }

        [Test]
        public void ContractTop()
        {
            var sut = new TileArrangement(3, 2);

            foreach (var y in Enumerable.Range(0, 2))
                foreach (var x in Enumerable.Range(0, 3))
                    sut[x, y] = x + 3 * y;

            sut.ContractTop();

            Assert.AreEqual(3, sut.TileCountX);
            Assert.AreEqual(1, sut.TileCountY);

            foreach (var x in Enumerable.Range(0, 3))
                Assert.AreEqual(x + 3, sut[x, 0]);
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { new TileArrangement(0, 1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { new TileArrangement(-1, 1); });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { new TileArrangement(1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { new TileArrangement(1, -1); });

            var sut = new TileArrangement(3, 4);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var i = sut[-1, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var i = sut[0, -1]; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var i = sut[3, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var i = sut[0, 4]; });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut[-1, 0] = 0; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut[0, -1] = 0; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut[3, 0] = 0; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut[0, 4] = 0; });

            using (var bitmap = new Bitmap(10, 10))
            {
                var tileset = new ImageTileset(bitmap, 10);

                Assert.Throws<ArgumentNullException>(
                    () => { sut.Render(null, tileset); });

                using (var g = Graphics.FromImage(bitmap))
                {
                    Assert.Throws<ArgumentNullException>(
                        () => { sut.Render(g, null); });
                    Assert.Throws<ArgumentOutOfRangeException>(
                        () => { sut.Render(g, tileset, 0); });
                }
            }

            sut = new TileArrangement(1, 1);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.ContractBottom(); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.ContractTop(); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.ContractLeft(); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.ContractRight(); });
        }
    }
}
