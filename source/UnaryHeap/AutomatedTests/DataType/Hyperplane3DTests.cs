﻿using NUnit.Framework;
using System;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Hyperplane3DTests
    {
        private static void CheckPointConstructor(
            Point3D A, Point3D B, Point3D C, Hyperplane3D front)
        {
            Assert.AreEqual(0, front.DetermineHalfspaceOf(A));
            Assert.AreEqual(0, front.DetermineHalfspaceOf(B));
            Assert.AreEqual(0, front.DetermineHalfspaceOf(C));
            Assert.AreEqual(front, new Hyperplane3D(A, B, C));
            Assert.AreEqual(front, new Hyperplane3D(B, C, A));
            Assert.AreEqual(front, new Hyperplane3D(C, A, B));
            Assert.AreEqual(front.Coplane, new Hyperplane3D(A, C, B));
            Assert.AreEqual(front.Coplane, new Hyperplane3D(B, A, C));
            Assert.AreEqual(front.Coplane, new Hyperplane3D(C, B, A));
        }

        [Test]
        public void XPlane()
        {
            var plane = new Hyperplane3D(1, 0, 0, 0);
            CheckPointConstructor(Point3D.Origin, new Point3D(0, 1, 0), new Point3D(0, 0, 1),
                plane);
        }

        [Test]
        public void YPlane()
        {
            var plane = new Hyperplane3D(0, 1, 0, 0);
            CheckPointConstructor(Point3D.Origin, new Point3D(0, 0, 1), new Point3D(1, 0, 0),
                plane);
        }

        [Test]
        public void ZPlane()
        {
            var plane = new Hyperplane3D(0, 0, 1, 0);
            CheckPointConstructor(Point3D.Origin, new Point3D(1, 0, 0), new Point3D(0, 1, 0),
                plane);
        }
    }
}
