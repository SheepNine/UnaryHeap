using NUnit.Framework;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Facet2DTests
    {
        [Test]
        public void Split()
        {
            var start = new Point2D(1, 2);
            var end = new Point2D(7, 5);
            var mid = new Point2D(5, 4);

            var sut = new Facet2D(new Hyperplane2D(start, end), start, end);

            // Coplanar
            sut.Split(new Hyperplane2D(start, end), out Facet2D front, out Facet2D back);
            Assert.IsNull(back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(end, front.End);

            // Counterplanar
            sut.Split(new Hyperplane2D(end, start), out front, out back);
            Assert.IsNull(front);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(end, back.End);

            // Edge touch
            sut.Split(new Hyperplane2D(start, new Point2D(start.X, start.Y - 1)),
                out front, out back);
            Assert.IsNull(back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(end, front.End);

            sut.Split(new Hyperplane2D(start, new Point2D(start.X, start.Y + 1)),
                out front, out back);
            Assert.IsNull(front);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(end, back.End);

            sut.Split(new Hyperplane2D(end, new Point2D(end.X, end.Y + 1)),
                out front, out back);
            Assert.IsNull(back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(end, front.End);

            sut.Split(new Hyperplane2D(end, new Point2D(end.X, end.Y - 1)),
                out front, out back);
            Assert.IsNull(front);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(end, back.End);

            // Actual split
            sut.Split(new Hyperplane2D(mid, new Point2D(mid.X, mid.Y + 1)),
                out front, out back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(mid, front.End);
            Assert.AreEqual(mid, back.Start);
            Assert.AreEqual(end, back.End);

            sut.Split(new Hyperplane2D(mid, new Point2D(mid.X, mid.Y - 1)),
                out front, out back);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(mid, back.End);
            Assert.AreEqual(mid, front.Start);
            Assert.AreEqual(end, front.End);
        }
    }
}
