using System;
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
                    var replacement = x*10 + y;

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
        }
    }
}
