using NUnit.Framework;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Point3DComparerTests
    {
        [Test]
        public void ComparePoints()
        {
            var sortedPoints = new[]
            {
                null,
                new Point3D(0, 0, 0),
                new Point3D(0, 0, 1),
                new Point3D(0, 1, 0),
                new Point3D(1, 0, 0)
            };

            for (var i = 0; i < sortedPoints.Length; i++)
            {
                Assert.AreEqual(0, Point3DComparer.Instance.Compare(sortedPoints[i], sortedPoints[i]));

                for (var j = i + 1; j < sortedPoints.Length; j++)
                {
                    Assert.AreEqual(-1, Point3DComparer.Instance.Compare(sortedPoints[i], sortedPoints[j]));
                    Assert.AreEqual(1, Point3DComparer.Instance.Compare(sortedPoints[j], sortedPoints[i]));
                }
            }
        }
    }
}
