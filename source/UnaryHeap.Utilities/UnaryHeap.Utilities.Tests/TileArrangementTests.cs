using System;
using System.Drawing;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Misc;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class TileArrangementTests
    {
        [Fact]
        public void ConstructorAccessorMutator()
        {
            var sut = new TileArrangement(4, 3);

            Assert.Equal(4, sut.TileCountX);
            Assert.Equal(3, sut.TileCountY);

            foreach (var x in Enumerable.Range(0, 4))
                foreach (var y in Enumerable.Range(0, 3))
                {
                    var replacement = x * 10 + y;

                    Assert.Equal(0, sut[x, y]);
                    sut[x, y] = replacement;
                    Assert.Equal(replacement, sut[x, y]);
                }
        }

        [Fact]
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

            Assert.Equal(4, sut2.TileCountX);
            Assert.Equal(3, sut2.TileCountY);

            foreach (var x in Enumerable.Range(0, 4))
                foreach (var y in Enumerable.Range(0, 3))
                    Assert.Equal(x * 10 + y, sut2[x, y]);

            Assert.Equal(new byte[] {
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

        [Fact]
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
                    using (var tileset = new Tileset(tilesetBitmap, 8))
                        sut.Render(g, tileset);
                }

                output.Save(@"data\TileArrangementTests\actual.png");
            }

            TilesetTests.ImageCompare(
                @"data\TileArrangementTests\expected.png",
                @"data\TileArrangementTests\actual.png");
        }

        [Fact]
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
                    using (var tileset = new Tileset(tilesetBitmap, 8))
                        sut.Render(g, tileset, 2);
                }

                output.Save(@"data\TileArrangementTests\actual2x.png");
            }

            TilesetTests.ImageCompare(
                @"data\TileArrangementTests\expected2x.png",
                @"data\TileArrangementTests\actual2x.png");
        }

        [Fact]
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
                    Assert.Equal(original[x, y], duplicate[x, y]);

                    duplicate[x, y] = 10;

                    Assert.Equal(x + 3 * y, original[x, y]);
                    Assert.Equal(10, duplicate[x, y]);
                }
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentOutOfRangeException>("tileCountX",
                () => { new TileArrangement(0, 1); });
            Assert.Throws<ArgumentOutOfRangeException>("tileCountX",
                () => { new TileArrangement(-1, 1); });

            Assert.Throws<ArgumentOutOfRangeException>("tileCountY",
                () => { new TileArrangement(1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>("tileCountY",
                () => { new TileArrangement(1, -1); });

            var sut = new TileArrangement(3, 4);

            Assert.Throws<ArgumentOutOfRangeException>("x",
                () => { var i = sut[-1, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>("y",
                () => { var i = sut[0, -1]; });
            Assert.Throws<ArgumentOutOfRangeException>("x",
                () => { var i = sut[3, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>("y",
                () => { var i = sut[0, 4]; });

            Assert.Throws<ArgumentOutOfRangeException>("x",
                () => { sut[-1, 0] = 0; });
            Assert.Throws<ArgumentOutOfRangeException>("y",
                () => { sut[0, -1] = 0; });
            Assert.Throws<ArgumentOutOfRangeException>("x",
                () => { sut[3, 0] = 0; });
            Assert.Throws<ArgumentOutOfRangeException>("y",
                () => { sut[0, 4] = 0; });

            using (var bitmap = new Bitmap(10, 10))
            {
                var tileset = new Tileset(bitmap, 10);

                Assert.Throws<ArgumentNullException>("g",
                    () => { sut.Render(null, tileset); });

                using (var g = Graphics.FromImage(bitmap))
                {
                    Assert.Throws<ArgumentNullException>("tileset",
                        () => { sut.Render(g, null); });
                    Assert.Throws<ArgumentOutOfRangeException>("scale",
                        () => { sut.Render(g, tileset, 0); });
                }
            }

        }
    }
}
