using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnaryHeap.Algorithms;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class FortunesAlgorithmTests
    {
        [Fact]
        public void FlatBottomTriangle()
        {
            TestFortunesAlgorithmListener.RunTest(
                new[] {
                    new Point2D(-1, 0),
                    new Point2D(1, 0),
                    new Point2D(0, 2)
                },
@"--- Delaunay Vertices (3) ---
0,2
1,0
-1,0
--- Voronoi Vertices (1) ---
0,3/4
--- Edges (3) ---
1,0 -> 0,2, ray from 0,3/4
-1,0 -> 0,2, ray from 0,3/4
-1,0 -> 1,0, ray from 0,3/4
");
        }

        [Fact]
        public void FlatTopTriangle()
        {
            TestFortunesAlgorithmListener.RunTest(
                new[] {
                    new Point2D(-1, 0),
                    new Point2D(1, 0),
                    new Point2D(0, -2)
                },
@"--- Delaunay Vertices (3) ---
0,-2
1,0
-1,0
--- Voronoi Vertices (1) ---
0,-3/4
--- Edges (3) ---
0,-2 -> 1,0, ray from 0,-3/4
0,-2 -> -1,0, ray from 0,-3/4
-1,0 -> 1,0, ray from 0,-3/4
");
        }

        [Fact]
        public void Square()
        {
            TestFortunesAlgorithmListener.RunTest(
                new[] {
                    new Point2D(01, 01),
                    new Point2D(01, -1),
                    new Point2D(-1, -1),
                    new Point2D(-1, 01),
                },
@"--- Delaunay Vertices (4) ---
1,1
1,-1
-1,1
-1,-1
--- Voronoi Vertices (1) ---
0,0
--- Edges (5) ---
1,-1 -> -1,1, crossing through 0,0
1,-1 -> 1,1, ray from 0,0
-1,1 -> 1,1, ray from 0,0
-1,-1 -> 1,-1, ray from 0,0
-1,-1 -> -1,1, ray from 0,0
");
        }

        [Fact]
        public void Diamond()
        {
            TestFortunesAlgorithmListener.RunTest(
                new[] {
                    new Point2D(00, -1),
                    new Point2D(00, 01),
                    new Point2D(-1, 00),
                    new Point2D(01, 00),
                },
@"--- Delaunay Vertices (4) ---
0,1
0,-1
1,0
-1,0
--- Voronoi Vertices (1) ---
0,0
--- Edges (5) ---
0,-1 -> 1,0, ray from 0,0
0,-1 -> -1,0, ray from 0,0
1,0 -> 0,1, ray from 0,0
-1,0 -> 0,1, ray from 0,0
-1,0 -> 1,0, crossing through 0,0
");
        }

        [Fact]
        public void FlatTopFan()
        {
            TestFortunesAlgorithmListener.RunTest(
                new[] {
                    new Point2D(-3, 00),
                    new Point2D(-2, 00),
                    new Point2D(-1, 00),
                    new Point2D(00, 00),
                    new Point2D(01, 00),
                    new Point2D(02, 00),
                    new Point2D(03, 00),
                    new Point2D(00, -5),
                },
@"--- Delaunay Vertices (8) ---
0,0
0,-5
1,0
-1,0
2,0
-2,0
3,0
-3,0
--- Voronoi Vertices (6) ---
1/2,-5/2
-1/2,-5/2
3/2,-27/10
-3/2,-27/10
5/2,-31/10
-5/2,-31/10
--- Edges (13) ---
0,0 -> 1,0, ray from 1/2,-5/2
0,-5 -> 0,0, dual -1/2,-5/2 -> 1/2,-5/2
0,-5 -> 1,0, dual 3/2,-27/10 -> 1/2,-5/2
0,-5 -> -1,0, dual -3/2,-27/10 -> -1/2,-5/2
0,-5 -> 2,0, dual 5/2,-31/10 -> 3/2,-27/10
0,-5 -> -2,0, dual -5/2,-31/10 -> -3/2,-27/10
0,-5 -> 3,0, ray from 5/2,-31/10
0,-5 -> -3,0, ray from -5/2,-31/10
-1,0 -> 0,0, ray from -1/2,-5/2
1,0 -> 2,0, ray from 3/2,-27/10
-2,0 -> -1,0, ray from -3/2,-27/10
2,0 -> 3,0, ray from 5/2,-31/10
-3,0 -> -2,0, ray from -5/2,-31/10
");
        }

        [Fact]
        public void FlatBottomFan()
        {
            TestFortunesAlgorithmListener.RunTest(
                new[] {
                    new Point2D(-3, 0),
                    new Point2D(-2, 0),
                    new Point2D(-1, 0),
                    new Point2D(00, 0),
                    new Point2D(01, 0),
                    new Point2D(02, 0),
                    new Point2D(03, 0),
                    new Point2D(00, 5),
                },
@"--- Delaunay Vertices (8) ---
0,0
0,5
1,0
-1,0
2,0
-2,0
3,0
-3,0
--- Voronoi Vertices (6) ---
1/2,5/2
-1/2,5/2
3/2,27/10
-3/2,27/10
5/2,31/10
-5/2,31/10
--- Edges (13) ---
0,0 -> 0,5, dual -1/2,5/2 -> 1/2,5/2
0,0 -> 1,0, ray from 1/2,5/2
-1,0 -> 0,0, ray from -1/2,5/2
1,0 -> 0,5, dual 1/2,5/2 -> 3/2,27/10
-1,0 -> 0,5, dual -1/2,5/2 -> -3/2,27/10
1,0 -> 2,0, ray from 3/2,27/10
2,0 -> 0,5, dual 3/2,27/10 -> 5/2,31/10
-2,0 -> 0,5, dual -3/2,27/10 -> -5/2,31/10
-2,0 -> -1,0, ray from -3/2,27/10
2,0 -> 3,0, ray from 5/2,31/10
3,0 -> 0,5, ray from 5/2,31/10
-3,0 -> 0,5, ray from -5/2,31/10
-3,0 -> -2,0, ray from -5/2,31/10
");
        }

        [Fact]
        public void Wheel()
        {
            TestFortunesAlgorithmListener.RunTest(
                new[] {
                    new Point2D(05, 00),
                    new Point2D(04, 03),
                    new Point2D(03, 04),
                    new Point2D(00, 05),
                    new Point2D(-3, 04),
                    new Point2D(-4, 03),
                    new Point2D(-5, 00),
                    new Point2D(-4, -3),
                    new Point2D(-3, -4),
                    new Point2D(00, -5),
                    new Point2D(03, -4),
                    new Point2D(04, -3),
                    Point2D.Origin,
                },
@"--- Delaunay Vertices (13) ---
0,0
0,5
0,-5
3,4
3,-4
-3,4
-3,-4
4,3
4,-3
-4,3
-4,-3
5,0
-5,0
--- Voronoi Vertices (12) ---
25/14,25/14
25/14,-25/14
-25/14,25/14
-25/14,-25/14
5/2,5/6
5/2,-5/6
-5/2,5/6
-5/2,-5/6
5/6,5/2
5/6,-5/2
-5/6,5/2
-5/6,-5/2
--- Edges (24) ---
0,0 -> 0,5, dual -5/6,5/2 -> 5/6,5/2
0,0 -> 3,4, dual 25/14,25/14 -> 5/6,5/2
0,0 -> -3,4, dual -25/14,25/14 -> -5/6,5/2
0,0 -> 4,3, dual 5/2,5/6 -> 25/14,25/14
0,0 -> -4,3, dual -5/2,5/6 -> -25/14,25/14
0,0 -> 5,0, dual 5/2,-5/6 -> 5/2,5/6
0,-5 -> 0,0, dual -5/6,-5/2 -> 5/6,-5/2
0,-5 -> 3,-4, ray from 5/6,-5/2
0,-5 -> -3,-4, ray from -5/6,-5/2
3,-4 -> 0,0, dual 5/6,-5/2 -> 25/14,-25/14
-3,-4 -> 0,0, dual -5/6,-5/2 -> -25/14,-25/14
3,4 -> 0,5, ray from 5/6,5/2
-3,4 -> 0,5, ray from -5/6,5/2
3,-4 -> 4,-3, ray from 25/14,-25/14
-3,-4 -> -4,-3, ray from -25/14,-25/14
4,-3 -> 0,0, dual 25/14,-25/14 -> 5/2,-5/6
-4,-3 -> 0,0, dual -25/14,-25/14 -> -5/2,-5/6
4,3 -> 3,4, ray from 25/14,25/14
-4,3 -> -3,4, ray from -25/14,25/14
4,-3 -> 5,0, ray from 5/2,-5/6
-4,-3 -> -5,0, ray from -5/2,-5/6
-5,0 -> 0,0, dual -5/2,-5/6 -> -5/2,5/6
5,0 -> 4,3, ray from 5/2,5/6
-5,0 -> -4,3, ray from -5/2,5/6
");
        }

        [Fact]
        public void Circle()
        {
            TestFortunesAlgorithmListener.RunTest(
                new[] {
                    new Point2D(05, 00),
                    new Point2D(04, 03),
                    new Point2D(03, 04),
                    new Point2D(00, 05),
                    new Point2D(-3, 04),
                    new Point2D(-4, 03),
                    new Point2D(-5, 00),
                    new Point2D(-4, -3),
                    new Point2D(-3, -4),
                    new Point2D(00, -5),
                    new Point2D(03, -4),
                    new Point2D(04, -3),
                },
@"--- Delaunay Vertices (12) ---
0,5
0,-5
3,4
3,-4
-3,4
-3,-4
4,3
4,-3
-4,3
-4,-3
5,0
-5,0
--- Voronoi Vertices (1) ---
0,0
--- Edges (21) ---
0,-5 -> 3,-4, ray from 0,0
0,-5 -> -3,-4, ray from 0,0
3,-4 -> 0,5, crossing through 0,0
3,4 -> 0,5, ray from 0,0
-3,4 -> 0,5, ray from 0,0
3,-4 -> 3,4, crossing through 0,0
3,-4 -> -3,4, crossing through 0,0
-3,-4 -> 3,-4, crossing through 0,0
3,-4 -> 4,3, crossing through 0,0
3,-4 -> -4,3, crossing through 0,0
3,-4 -> -4,-3, crossing through 0,0
3,-4 -> 4,-3, ray from 0,0
-3,-4 -> -4,-3, ray from 0,0
3,-4 -> 5,0, crossing through 0,0
3,-4 -> -5,0, crossing through 0,0
4,3 -> 3,4, ray from 0,0
-4,3 -> -3,4, ray from 0,0
4,-3 -> 5,0, ray from 0,0
-4,-3 -> -5,0, ray from 0,0
5,0 -> 4,3, ray from 0,0
-5,0 -> -4,3, ray from 0,0
");
        }

        [Fact]
        public void Lattice()
        {
            TestFortunesAlgorithmListener.RunTest(new[] {
                new Point2D(0, 0),
                new Point2D(2, 0),
                new Point2D(4, 0),
                new Point2D(0, 2),
                new Point2D(2, 2),
                new Point2D(4, 2),
                new Point2D(0, 4),
                new Point2D(2, 4),
                new Point2D(4, 4),
            },
@"--- Delaunay Vertices (9) ---
0,0
0,2
0,4
2,0
2,2
2,4
4,0
4,2
4,4
--- Voronoi Vertices (4) ---
1,1
1,3
3,1
3,3
--- Edges (16) ---
0,0 -> 0,2, ray from 1,1
0,0 -> 2,0, ray from 1,1
0,2 -> 0,4, ray from 1,3
0,2 -> 2,2, dual 1,1 -> 1,3
0,4 -> 2,4, ray from 1,3
2,0 -> 0,2, crossing through 1,1
2,0 -> 2,2, dual 1,1 -> 3,1
2,0 -> 4,0, ray from 3,1
2,2 -> 0,4, crossing through 1,3
2,2 -> 2,4, dual 1,3 -> 3,3
2,2 -> 4,2, dual 3,1 -> 3,3
2,4 -> 4,4, ray from 3,3
4,0 -> 2,2, crossing through 3,1
4,0 -> 4,2, ray from 3,1
4,2 -> 2,4, crossing through 3,3
4,2 -> 4,4, ray from 3,3
");
        }

        [Fact]
        public void InputWithDuplicatePoints()
        {
            var listener = new NullFortunesAlgorithmListener();
            var sites = FortunesAlgorithm.GenerateRandomPoints(10, 19830630);

            for (int dupIndex = 0; dupIndex < sites.Length; dupIndex++)
            {
                var input = sites.ToList();
                input.Add(sites[dupIndex]);

                Assert.StartsWith("Enumerable contains one or more duplicate points.",
                    Assert.Throws<ArgumentException>("sites",
                    () => { FortunesAlgorithm.Execute(input, listener); }).Message);
            }
        }

        [Fact]
        public void ColinearPointsOnHorizontalLine()
        {
            var listener = new NullFortunesAlgorithmListener();
            var sites = Enumerable.Range(0, 10).Select(i => new Point2D(i, 0)).ToArray();

            for (int i = 3; i < sites.Length; i++)
            {
                Assert.StartsWith("Input sites are colinear.",
                    Assert.Throws<ArgumentException>("sites",
                        () => { FortunesAlgorithm.Execute(sites.Take(i), listener); }).Message);
            }
        }

        [Fact]
        public void ColinearPointsOnDiagonalLine()
        {
            var listener = new NullFortunesAlgorithmListener();
            var sites = Enumerable.Range(0, 10).Select(i => new Point2D(i, i)).ToArray();

            for (int i = 3; i < sites.Length; i++)
            {
                Assert.StartsWith("Input sites are colinear.",
                    Assert.Throws<ArgumentException>("sites",
                        () => { FortunesAlgorithm.Execute(sites.Take(i), listener); }).Message);
            }
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            var listener = new NullFortunesAlgorithmListener();

            // --- null arary ---
            Assert.Throws<ArgumentNullException>("sites",
                () => { FortunesAlgorithm.Execute(null, listener); });
            // --- null listener ---
            Assert.Throws<ArgumentNullException>("listener",
                () => { FortunesAlgorithm.Execute(Enumerable.Empty<Point2D>(), null); });
            // --- empty array ---
            Assert.Throws<ArgumentException>("sites",
                () => { FortunesAlgorithm.Execute(Enumerable.Empty<Point2D>(), listener); });
            // --- null point in array ---
            var sitesContainingNull = new[] { new Point2D(0, 0), null, new Point2D(1, 1) };
            Assert.Throws<ArgumentNullException>("sites",
                () => { FortunesAlgorithm.Execute(sitesContainingNull, listener); });

            // --- one or two points ---
            var twoSites = new[] { new Point2D(0, 0), new Point2D(1, 1) };
            Assert.Throws<ArgumentException>("sites",
                () => { FortunesAlgorithm.Execute(twoSites.Take(1), listener); });
            Assert.Throws<ArgumentException>("sites",
                () => { FortunesAlgorithm.Execute(twoSites.Take(2), listener); });
        }
    }

    class NullFortunesAlgorithmListener : IFortunesAlgorithmListener
    {
        public void EmitDelaunayEdge(Point2D p1, Point2D p2) { }
        public void EmitDelaunayVertex(Point2D p) { }
        public void EmitVoronoiVertex(Point2D p) { }
        public void EmitDualEdges(Point2D site1, Point2D site2, Point2D p1, Point2D p2) { }
        public void EmitVoronoiRay(Point2D p, Point2D site1, Point2D site2) { }
    }

    class TestFortunesAlgorithmListener : IFortunesAlgorithmListener
    {
        SortedSet<string> delaunayVertices = new SortedSet<string>();
        SortedSet<string> voronoiVertices = new SortedSet<string>();
        SortedSet<string> edges = new SortedSet<string>();
        IComparer<Point2D> pointComparer = new Point2DComparer();

        public void EmitDelaunayVertex(Point2D p)
        {
            var text = p.ToString();

            if (delaunayVertices.Contains(text))
                throw new ArgumentException("Duplicate Delaunay vertex emitted.");

            delaunayVertices.Add(text);
        }

        public void EmitVoronoiVertex(Point2D p)
        {
            var text = p.ToString();

            if (voronoiVertices.Contains(text))
                throw new ArgumentException("Duplicate Voronoi vertex emitted.");

            voronoiVertices.Add(text);
        }

        public void EmitDualEdges(Point2D d1, Point2D d2, Point2D v1, Point2D v2)
        {
            Order(ref d1, ref d2);

            string text;

            if (null == v2)
            {
                text = string.Format("{0} -> {1}, ray from {2}", d1, d2, v1);
            }
            else if (v1.Equals(v2))
            {
                text = string.Format("{0} -> {1}, crossing through {2}", d1, d2, v1);
            }
            else
            {
                Order(ref v1, ref v2);
                text = string.Format("{0} -> {1}, dual {2} -> {3}", d1, d2, v1, v2);
            }

            if (edges.Contains(text))
                throw new ArgumentException("Duplicate Voronoi edge emitted");

            edges.Add(text);
        }

        string GetActualLog()
        {
            var result = new StringBuilder();

            result.AppendLine(string.Format("--- Delaunay Vertices ({0}) ---", delaunayVertices.Count));
            foreach (var delaunayVertex in delaunayVertices)
                result.AppendLine(delaunayVertex);

            result.AppendLine(string.Format("--- Voronoi Vertices ({0}) ---", voronoiVertices.Count));
            foreach (var voronoiVertex in voronoiVertices)
                result.AppendLine(voronoiVertex);

            result.AppendLine(string.Format("--- Edges ({0}) ---", edges.Count));
            foreach (var edge in edges)
                result.AppendLine(edge);

            return result.ToString();
        }

        public static void RunTest(Point2D[] sites, string expectedLog)
        {
            var listener = new TestFortunesAlgorithmListener();
            FortunesAlgorithm.Execute(sites, listener);

            Assert.Equal(expectedLog, listener.GetActualLog());
        }

        void Order(ref Point2D p1, ref Point2D p2)
        {
            if (1 == pointComparer.Compare(p1, p2))
            {
                var temp = p1;
                p1 = p2;
                p2 = temp;
            }
        }
    }
}
