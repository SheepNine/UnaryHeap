using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Misc;
using System.Globalization;
using System.Drawing.Imaging;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class RawImageTests
    {
        [Test]
        public void Accessors()
        {
            var sut = new RawImage(4, 3);
            Assert.AreEqual(4, sut.Width);
            Assert.AreEqual(3, sut.Height);
        }

        [Test]
        public void GetSetBlendPixel()
        {
            var sut = new RawImage(1, 1);
            AssertPixel(sut, 0, 0, 0, 0, 0);
            sut.SetPixel(0, 0, 20, 40, 60);
            AssertPixel(sut, 0, 0, 20, 40, 60);
            sut.BlendPixel(0, 0, 10, 60, 100);
            AssertPixel(sut, 0, 0, 15, 50, 80);
        }

        [Test]
        public void PixelIndices()
        {
            const int Width = 5;
            const int Height = 7;
            var sut = new RawImage(Width, Height);

            foreach (var x in Enumerable.Range(0, Width))
                foreach (var y in Enumerable.Range(0, Height))
                {
                    AssertPixel(sut, x, y, 0, 0, 0);
                    sut.SetPixel(x, y, (byte)(x * y), (byte)(x * y + 2), (byte)(x * y + 4));
                }

            foreach (var x in Enumerable.Range(0, Width))
                foreach (var y in Enumerable.Range(0, Height))
                    AssertPixel(sut, x, y, (byte)(x * y), (byte)(x * y + 2), (byte)(x * y + 4));
        }

        [Test]
        public void Serialize()
        {
            var sut = new RawImage(3, 2);
            foreach (var x in Enumerable.Range(0, 3))
                foreach (var y in Enumerable.Range(0, 2))
                    sut.SetPixel(x, y,
                        (byte)(16 * x + y), (byte)(16 * x + y + 1), (byte)(16 * x + y + 2));

            using (var buffer = new MemoryStream())
            {
                sut.Serialize(buffer);
                CollectionAssert.AreEqual(DecodeHexStream(
                    "03000000 02000000 000102 101112 202122 010203 111213 212223"),
                    buffer.ToArray());
            }
        }

        [Test]
        public void Deserialize()
        {
            RawImage sut;
            using (var buffer = new MemoryStream(DecodeHexStream(
                "02000000 03000000 000000 100010 001001 101011 002002 102012")))
                sut = RawImage.Deserialize(buffer);

            Assert.AreEqual(2, sut.Width);
            Assert.AreEqual(3, sut.Height);

            foreach (var x in Enumerable.Range(0, 2))
                foreach (var y in Enumerable.Range(0, 3))
                    AssertPixel(sut, x, y, (byte)(x * 16), (byte)(y * 16), (byte)(x * 16 + y));
        }

        [Test]
        public void MakeImage()
        {
            var sut = new RawImage(256, 256);
            foreach (var x in Enumerable.Range(0, 256))
                foreach (var y in Enumerable.Range(0, 256))
                    sut.SetPixel(x, y, (byte)(x & y), (byte)(x | y), (byte)(x ^ y));

            Directory.CreateDirectory(@"data\RawImageTests");
            using (var bitmap = sut.MakeBitmap())
                bitmap.Save(@"data\RawImageTests\MakeImageActual.png", ImageFormat.Png);

            TilesetTests.ImageCompare(
                @"data\RawImageTests\MakeImageExpected.png",
                @"data\RawImageTests\MakeImageActual.png");
        }

        private byte[] DecodeHexStream(string data)
        {
            data = data.Replace(" ", String.Empty);
            if (data.Length % 2 != 0)
                throw new ArgumentException();

            var result = new byte[data.Length / 2];

            foreach (var i in Enumerable.Range(0, result.Length))
                result[i] = byte.Parse(data.Substring(i * 2, 2), NumberStyles.HexNumber);

            return result;
        }

        private void AssertPixel(RawImage sut, int x, int y,
            byte expectedR, byte expectedG, byte expectedB)
        {
            byte actualR, actualG, actualB;
            sut.GetPixel(x, y, out actualR, out actualG, out actualB);
            Assert.AreEqual(expectedR, actualR, "R mismatch at (" + x + ", " + y + ")");
            Assert.AreEqual(expectedG, actualG, "G mismatch at (" + x + ", " + y + ")");
            Assert.AreEqual(expectedB, actualB, "B mismatch at (" + x + ", " + y + ")");
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new RawImage(0, 100));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new RawImage(100, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new RawImage(-1, 100));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new RawImage(100, -1));

            const int Width = 10;
            const int Height = 20;
            var sut = new RawImage(Width, Height);
            byte r, g, b;

            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.SetPixel(-1, Height / 2, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.SetPixel(Width, Height / 2, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.SetPixel(Width / 2, -1, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.SetPixel(Width / 2, Height, 0, 0, 0));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.GetPixel(-1, Height / 2, out r, out g, out b));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.GetPixel(Width, Height / 2, out r, out g, out b));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.GetPixel(Width / 2, -1, out r, out g, out b));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.GetPixel(Width / 2, Height, out r, out g, out b));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.BlendPixel(-1, Height / 2, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.BlendPixel(Width, Height / 2, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.BlendPixel(Width / 2, -1, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.BlendPixel(Width / 2, Height, 0, 0, 0));

            Assert.Throws<ArgumentNullException>(
                () => sut.Serialize(null));
            Assert.Throws<ArgumentNullException>(
                () => RawImage.Deserialize(null));
        }
    }
}
