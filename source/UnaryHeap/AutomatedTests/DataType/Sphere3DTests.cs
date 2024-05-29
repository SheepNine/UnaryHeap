using NUnit.Framework;
using System;

namespace UnaryHeap.DataType.Tests
{
    public class Sphere3DTests
    {
        // TODO: Write some tests


        [Test]
        public void SimpleArgumentExceptions()
        {
            var point = new Point3D(1, 2, 3);
            TestUtils.NullChecks(new()
            {
                { typeof(ArgumentNullException), new TestDelegate[] {
                    () => { _ = new Sphere3D(null, 1); },
                    () => { _ = new Sphere3D(Point3D.Origin, null); },
                    () => { Sphere3D.Circumcircle(null, point, point); },
                    () => { Sphere3D.Circumcircle(point, null, point); },
                    () => { Sphere3D.Circumcircle(point, point, null); },
                }}
            });
        }
    }
}
