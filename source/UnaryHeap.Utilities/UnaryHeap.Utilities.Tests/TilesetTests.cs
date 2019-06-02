using System;
using System.Drawing;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Misc;
using NUnit.Framework;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class TilesetTests
    {
        [Test]
        public void Constructor()
        {
            var image = new Bitmap(20, 30);

            using (var sut = new Tileset(image, 10))
            {
                Assert.AreEqual(6, sut.NumTiles);
                Assert.AreEqual(10, sut.TileSize);
                Assert.AreEqual(20, sut.ImageWidth);
                Assert.AreEqual(30, sut.ImageHeight);
            }
        }

        [Test]
        public void DrawTile()
        {
            var tiles = new Bitmap(@"data\TilesetTests\tileset.png");

            using (var sut = new Tileset(tiles, 8))
            {
                // Ensure that sut has its own private copy of tiles
                tiles.Dispose();

                using (var output = new Bitmap(80, 130))
                {
                    using (var g = Graphics.FromImage(output))
                    {
                        g.Clear(Color.CornflowerBlue);
                        foreach (var i in Enumerable.Range(0, 12))
                            sut.DrawTile(g, i, 5 * i, 10 * i);
                    }

                    output.Save(@"data\TilesetTests\actual.png");
                }
            }

            ImageCompare(
                @"data\TilesetTests\expected.png", @"data\TilesetTests\actual.png");
        }

        public static void ImageCompare(string expectedFilename, string actualFilename)
        {
            expectedFilename = Path.GetFullPath(expectedFilename);
            actualFilename = Path.GetFullPath(actualFilename);

            Assert.True(File.Exists(expectedFilename),
                "Expected image file '" + expectedFilename + "' does not exist");
            Assert.True(File.Exists(actualFilename),
                "Actual image file '" + actualFilename + "' does not exist");

            using (var expected = new Bitmap(expectedFilename))
            using (var actual = new Bitmap(actualFilename))
            {
                Assert.AreEqual(expected.Size, actual.Size,
                    "Image file '" + actualFilename + "' size incorrect");

                foreach (var y in Enumerable.Range(0, expected.Height))
                    foreach (var x in Enumerable.Range(0, expected.Width))
                        Assert.AreEqual(expected.GetPixel(x, y), actual.GetPixel(x, y),
                                "Image file '" + actualFilename + "' has incorrect pixel at" +
                                x + ", " + y);
            }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            using (var img = new Bitmap(30, 20))
            using (var imgo = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(imgo))
            {

                Assert.Throws<ArgumentNullException>(
                    () => { new Tileset(null, 2); });
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => { new Tileset(img, 0); });
                Assert.Throws<ArgumentException>(
                    () => { new Tileset(img, 3); });
                Assert.Throws<ArgumentException>(
                    () => { new Tileset(img, 4); });

                var sut = new Tileset(img, 10);

                Assert.Throws<ArgumentNullException>(
                    () => { sut.DrawTile(null, 0, 0, 0); });
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => { sut.DrawTile(g, -1, 0, 0); });
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => { sut.DrawTile(g, 6, 0, 0); });

                Assert.Throws<ArgumentOutOfRangeException>(
                    () => { sut.DrawTile(g, 0, 0, 0, 0); });
            }
        }
    }
}
