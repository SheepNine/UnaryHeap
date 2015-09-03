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
--- Delaunay Edges (3) ---
1,0 -> 0,2
-1,0 -> 0,2
-1,0 -> 1,0
--- Voronoi Vertices (1) ---
0,3/4
--- Voronoi Edges (0) ---
--- Voronoi Rays (3) ---
0,3/4, through 1,0 -> 0,2
0,3/4, through -1,0 -> 0,2
0,3/4, through -1,0 -> 1,0
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
--- Delaunay Edges (3) ---
0,-2 -> 1,0
0,-2 -> -1,0
-1,0 -> 1,0
--- Voronoi Vertices (1) ---
0,-3/4
--- Voronoi Edges (0) ---
--- Voronoi Rays (3) ---
0,-3/4, through 0,-2 -> 1,0
0,-3/4, through 0,-2 -> -1,0
0,-3/4, through -1,0 -> 1,0
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
--- Delaunay Edges (5) ---
1,-1 -> 1,1
1,-1 -> -1,1
-1,1 -> 1,1
-1,-1 -> 1,-1
-1,-1 -> -1,1
--- Voronoi Vertices (1) ---
0,0
--- Voronoi Edges (1) ---
0,0, on 1,-1 -> -1,1
--- Voronoi Rays (4) ---
0,0, through 1,-1 -> 1,1
0,0, through -1,1 -> 1,1
0,0, through -1,-1 -> 1,-1
0,0, through -1,-1 -> -1,1
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
--- Delaunay Edges (5) ---
0,-1 -> 1,0
0,-1 -> -1,0
1,0 -> 0,1
-1,0 -> 0,1
-1,0 -> 1,0
--- Voronoi Vertices (1) ---
0,0
--- Voronoi Edges (1) ---
0,0, on -1,0 -> 1,0
--- Voronoi Rays (4) ---
0,0, through 0,-1 -> 1,0
0,0, through 0,-1 -> -1,0
0,0, through 1,0 -> 0,1
0,0, through -1,0 -> 0,1
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
--- Delaunay Edges (13) ---
0,0 -> 1,0
0,-5 -> 0,0
0,-5 -> 1,0
0,-5 -> -1,0
0,-5 -> 2,0
0,-5 -> -2,0
0,-5 -> 3,0
0,-5 -> -3,0
-1,0 -> 0,0
1,0 -> 2,0
-2,0 -> -1,0
2,0 -> 3,0
-3,0 -> -2,0
--- Voronoi Vertices (6) ---
1/2,-5/2
-1/2,-5/2
3/2,-27/10
-3/2,-27/10
5/2,-31/10
-5/2,-31/10
--- Voronoi Edges (5) ---
-1/2,-5/2 -> 1/2,-5/2, through 0,-5 -> 0,0
3/2,-27/10 -> 1/2,-5/2, through 0,-5 -> 1,0
-3/2,-27/10 -> -1/2,-5/2, through 0,-5 -> -1,0
5/2,-31/10 -> 3/2,-27/10, through 0,-5 -> 2,0
-5/2,-31/10 -> -3/2,-27/10, through 0,-5 -> -2,0
--- Voronoi Rays (8) ---
1/2,-5/2, through 0,0 -> 1,0
-1/2,-5/2, through -1,0 -> 0,0
3/2,-27/10, through 1,0 -> 2,0
-3/2,-27/10, through -2,0 -> -1,0
5/2,-31/10, through 0,-5 -> 3,0
-5/2,-31/10, through 0,-5 -> -3,0
5/2,-31/10, through 2,0 -> 3,0
-5/2,-31/10, through -3,0 -> -2,0
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
--- Delaunay Edges (13) ---
0,0 -> 0,5
0,0 -> 1,0
-1,0 -> 0,0
1,0 -> 0,5
-1,0 -> 0,5
1,0 -> 2,0
2,0 -> 0,5
-2,0 -> 0,5
-2,0 -> -1,0
2,0 -> 3,0
3,0 -> 0,5
-3,0 -> 0,5
-3,0 -> -2,0
--- Voronoi Vertices (6) ---
1/2,5/2
-1/2,5/2
3/2,27/10
-3/2,27/10
5/2,31/10
-5/2,31/10
--- Voronoi Edges (5) ---
-1/2,5/2 -> 1/2,5/2, through 0,0 -> 0,5
1/2,5/2 -> 3/2,27/10, through 1,0 -> 0,5
-1/2,5/2 -> -3/2,27/10, through -1,0 -> 0,5
3/2,27/10 -> 5/2,31/10, through 2,0 -> 0,5
-3/2,27/10 -> -5/2,31/10, through -2,0 -> 0,5
--- Voronoi Rays (8) ---
1/2,5/2, through 0,0 -> 1,0
-1/2,5/2, through -1,0 -> 0,0
3/2,27/10, through 1,0 -> 2,0
-3/2,27/10, through -2,0 -> -1,0
5/2,31/10, through 2,0 -> 3,0
5/2,31/10, through 3,0 -> 0,5
-5/2,31/10, through -3,0 -> 0,5
-5/2,31/10, through -3,0 -> -2,0
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
--- Delaunay Edges (24) ---
0,0 -> 0,5
0,0 -> 3,4
0,0 -> -3,4
0,0 -> 4,3
0,0 -> -4,3
0,0 -> 5,0
0,-5 -> 0,0
0,-5 -> 3,-4
0,-5 -> -3,-4
3,-4 -> 0,0
-3,-4 -> 0,0
3,4 -> 0,5
-3,4 -> 0,5
3,-4 -> 4,-3
-3,-4 -> -4,-3
4,-3 -> 0,0
-4,-3 -> 0,0
4,3 -> 3,4
-4,3 -> -3,4
4,-3 -> 5,0
-4,-3 -> -5,0
-5,0 -> 0,0
5,0 -> 4,3
-5,0 -> -4,3
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
--- Voronoi Edges (12) ---
25/14,-25/14 -> 5/2,-5/6, through 4,-3 -> 0,0
-25/14,-25/14 -> -5/2,-5/6, through -4,-3 -> 0,0
25/14,25/14 -> 5/6,5/2, through 0,0 -> 3,4
-25/14,25/14 -> -5/6,5/2, through 0,0 -> -3,4
5/2,5/6 -> 25/14,25/14, through 0,0 -> 4,3
-5/2,5/6 -> -25/14,25/14, through 0,0 -> -4,3
5/2,-5/6 -> 5/2,5/6, through 0,0 -> 5,0
-5/2,-5/6 -> -5/2,5/6, through -5,0 -> 0,0
5/6,-5/2 -> 25/14,-25/14, through 3,-4 -> 0,0
-5/6,-5/2 -> -25/14,-25/14, through -3,-4 -> 0,0
-5/6,5/2 -> 5/6,5/2, through 0,0 -> 0,5
-5/6,-5/2 -> 5/6,-5/2, through 0,-5 -> 0,0
--- Voronoi Rays (12) ---
25/14,-25/14, through 3,-4 -> 4,-3
-25/14,-25/14, through -3,-4 -> -4,-3
25/14,25/14, through 4,3 -> 3,4
-25/14,25/14, through -4,3 -> -3,4
5/2,-5/6, through 4,-3 -> 5,0
-5/2,-5/6, through -4,-3 -> -5,0
5/2,5/6, through 5,0 -> 4,3
-5/2,5/6, through -5,0 -> -4,3
5/6,-5/2, through 0,-5 -> 3,-4
-5/6,-5/2, through 0,-5 -> -3,-4
5/6,5/2, through 3,4 -> 0,5
-5/6,5/2, through -3,4 -> 0,5
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
--- Delaunay Edges (21) ---
0,-5 -> 3,-4
0,-5 -> -3,-4
3,4 -> 0,5
3,-4 -> 0,5
-3,4 -> 0,5
3,-4 -> 3,4
3,-4 -> -3,4
-3,-4 -> 3,-4
3,-4 -> 4,3
3,-4 -> 4,-3
3,-4 -> -4,3
3,-4 -> -4,-3
-3,-4 -> -4,-3
3,-4 -> 5,0
3,-4 -> -5,0
4,3 -> 3,4
-4,3 -> -3,4
4,-3 -> 5,0
-4,-3 -> -5,0
5,0 -> 4,3
-5,0 -> -4,3
--- Voronoi Vertices (1) ---
0,0
--- Voronoi Edges (9) ---
0,0, on 3,-4 -> 0,5
0,0, on 3,-4 -> 3,4
0,0, on 3,-4 -> -3,4
0,0, on -3,-4 -> 3,-4
0,0, on 3,-4 -> 4,3
0,0, on 3,-4 -> -4,3
0,0, on 3,-4 -> -4,-3
0,0, on 3,-4 -> 5,0
0,0, on 3,-4 -> -5,0
--- Voronoi Rays (12) ---
0,0, through 0,-5 -> 3,-4
0,0, through 0,-5 -> -3,-4
0,0, through 3,4 -> 0,5
0,0, through -3,4 -> 0,5
0,0, through 3,-4 -> 4,-3
0,0, through -3,-4 -> -4,-3
0,0, through 4,3 -> 3,4
0,0, through -4,3 -> -3,4
0,0, through 4,-3 -> 5,0
0,0, through -4,-3 -> -5,0
0,0, through 5,0 -> 4,3
0,0, through -5,0 -> -4,3
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
--- Delaunay Edges (16) ---
0,0 -> 0,2
0,0 -> 2,0
0,2 -> 0,4
0,2 -> 2,2
0,4 -> 2,4
2,0 -> 0,2
2,0 -> 2,2
2,0 -> 4,0
2,2 -> 0,4
2,2 -> 2,4
2,2 -> 4,2
2,4 -> 4,4
4,0 -> 2,2
4,0 -> 4,2
4,2 -> 2,4
4,2 -> 4,4
--- Voronoi Vertices (4) ---
1,1
1,3
3,1
3,3
--- Voronoi Edges (8) ---
1,1 -> 1,3, through 0,2 -> 2,2
1,1 -> 3,1, through 2,0 -> 2,2
1,1, on 2,0 -> 0,2
1,3 -> 3,3, through 2,2 -> 2,4
1,3, on 2,2 -> 0,4
3,1 -> 3,3, through 2,2 -> 4,2
3,1, on 4,0 -> 2,2
3,3, on 4,2 -> 2,4
--- Voronoi Rays (8) ---
1,1, through 0,0 -> 0,2
1,1, through 0,0 -> 2,0
1,3, through 0,2 -> 0,4
1,3, through 0,4 -> 2,4
3,1, through 2,0 -> 4,0
3,1, through 4,0 -> 4,2
3,3, through 2,4 -> 4,4
3,3, through 4,2 -> 4,4
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
        public void EmitVoronoiEdge(Point2D p1, Point2D p2, Point2D site1, Point2D site2) { }
        public void EmitVoronoiRay(Point2D p, Point2D site1, Point2D site2) { }
    }

    class TestFortunesAlgorithmListener : IFortunesAlgorithmListener
    {
        SortedSet<string> delaunayEdges = new SortedSet<string>();
        SortedSet<string> delaunayVertices = new SortedSet<string>();
        SortedSet<string> voronoiEdges = new SortedSet<string>();
        SortedSet<string> voronoiVertices = new SortedSet<string>();
        SortedSet<string> voronoiRays = new SortedSet<string>();
        IComparer<Point2D> pointComparer = new Point2DComparer();

        public void EmitDelaunayEdge(Point2D p1, Point2D p2)
        {
            Order(ref p1, ref p2);

            var text = string.Format("{0} -> {1}", p1, p2);

            if (delaunayEdges.Contains(text))
                throw new ArgumentException("Duplicate Delaunay edge emitted.");

            delaunayEdges.Add(text);
        }

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

        public void EmitVoronoiEdge(Point2D p1, Point2D p2, Point2D site1, Point2D site2)
        {
            Order(ref p1, ref p2);
            Order(ref site1, ref site2);

            string text;

            if (p1.Equals(p2))
                text = string.Format("{0}, on {1} -> {2}", p1, site1, site2);
            else
                text = string.Format("{0} -> {1}, through {2} -> {3}", p1, p2, site1, site2);

            if (voronoiEdges.Contains(text))
                throw new ArgumentException("Duplicate Voronoi edge emitted");

            voronoiEdges.Add(text);
        }

        public void EmitVoronoiRay(Point2D p, Point2D site1, Point2D site2)
        {
            Order(ref site1, ref site2);

            var text = string.Format("{0}, through {1} -> {2}", p, site1, site2);

            if (voronoiRays.Contains(text))
                throw new ArgumentException("Duplicate Voronoi edge emitted");

            voronoiRays.Add(text);
        }

        string GetActualLog()
        {
            var result = new StringBuilder();

            result.AppendLine(string.Format("--- Delaunay Vertices ({0}) ---", delaunayVertices.Count));
            foreach (var delaunayVertex in delaunayVertices)
                result.AppendLine(delaunayVertex);

            result.AppendLine(string.Format("--- Delaunay Edges ({0}) ---", delaunayEdges.Count));
            foreach (var delaunayEdge in delaunayEdges)
                result.AppendLine(delaunayEdge);

            result.AppendLine(string.Format("--- Voronoi Vertices ({0}) ---", voronoiVertices.Count));
            foreach (var voronoiVertex in voronoiVertices)
                result.AppendLine(voronoiVertex);

            result.AppendLine(string.Format("--- Voronoi Edges ({0}) ---", voronoiEdges.Count));
            foreach (var voronoiEdge in voronoiEdges)
                result.AppendLine(voronoiEdge);

            result.AppendLine(string.Format("--- Voronoi Rays ({0}) ---", voronoiRays.Count));
            foreach (var voronoiRay in voronoiRays)
                result.AppendLine(voronoiRay);

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
