using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;

namespace MazeGenerator
{
    class MazeListener : IFortunesAlgorithmListener
    {
        Graph2D delaunay;
        Graph2D voronoi;
        SortedDictionary<Point2D, List<Point2D>> voronoiFaces;
        SortedDictionary<Point2D, List<Point2D>> delaunayFaces;
        string outputFilename;
        IComparer<Point2D> pointComparer;
        IHeightMap heightMap;

        public MazeListener(string outputFilename, IHeightMap heightMap)
        {
            this.outputFilename = outputFilename;
            this.heightMap = heightMap;

            pointComparer = new Point2DComparer();
            delaunay = new Graph2D(false);
            voronoi = new Graph2D(false);
            voronoiFaces = new SortedDictionary<Point2D, List<Point2D>>(pointComparer);
            delaunayFaces = new SortedDictionary<Point2D, List<Point2D>>(pointComparer);
        }

        public void EmitDelaunayVertex(Point2D p)
        {
            delaunay.AddVertex(p);
            voronoiFaces.Add(p, new List<Point2D>());
        }

        public void EmitVoronoiVertex(Point2D p)
        {
            voronoi.AddVertex(p);
            delaunayFaces.Add(p, new List<Point2D>());
        }

        public void EmitDualEdges(Point2D d1, Point2D d2, Point2D v1, Point2D v2)
        {
            voronoiFaces[d1].Add(v1);
            voronoiFaces[d2].Add(v1);
            delaunayFaces[v1].Add(d1);
            delaunayFaces[v1].Add(d2);

            if (null == v2)
            {
                voronoiFaces[d1].Add(null);
                voronoiFaces[d2].Add(null);
            }
            else if (false == v1.Equals(v2))
            {
                voronoiFaces[d1].Add(v2);
                voronoiFaces[d2].Add(v2);
                delaunayFaces[v2].Add(d1);
                delaunayFaces[v2].Add(d2);

                voronoi.AddEdge(v1, v2);
                delaunay.AddEdge(d1, d2);
                delaunay.SetEdgeMetadatum(d1, d2, "dual", DualString(v1, v2));
            }
        }

        public void AlgorithmComplete()
        {
            RemoveUnboundedRooms();
            ConnectRooms();
            WriteVoronoiGraph();
        }

        void RemoveUnboundedRooms()
        {
            var unboundedVoronoiFaces = GetUnboundedVoronoiFaces();
            var unnecessaryVoronoiVertices = GetUnnecessaryVoronoiVertices(unboundedVoronoiFaces);

            delaunay.RemoveVertices(unboundedVoronoiFaces);
            voronoi.RemoveVertices(unnecessaryVoronoiVertices);
        }

        private Point2D[] GetUnnecessaryVoronoiVertices(SortedSet<Point2D> unboundedVoronoiFaces)
        {
            return delaunayFaces.Where(
                    df => df.Value.All(dv => unboundedVoronoiFaces.Contains(dv))
                ).Select(df => df.Key)
                .ToArray();
        }

        SortedSet<Point2D> GetUnboundedVoronoiFaces()
        {
            var bounds = Orthotope2D.FromPoints(voronoiFaces.Keys);

            return new SortedSet<Point2D>(
                voronoiFaces.Where(vf =>
                    vf.Value.Any(vv => null == vv || false == bounds.Contains(vv)))
                    .Select(vf => vf.Key),
                pointComparer);
        }

        void ConnectRooms()
        {
            AssignDelaunayGraphWeights();

            var mst = PrimsAlgorithm.FindMinimumSpanningTree(delaunay, delaunay.Vertices.First());

            foreach (var edge in mst.Edges)
            {
                var dual = DualValue(mst.GetEdgeMetadatum(edge.Item1, edge.Item2, "dual"));
                voronoi.RemoveEdge(dual.Item1, dual.Item2);
            }
        }

        void AssignDelaunayGraphWeights()
        {
            foreach (var edge in delaunay.Edges)
            {
                var dual = DualValue(delaunay.GetEdgeMetadatum(edge.Item1, edge.Item2, "dual"));

                if (VoronoiEdgeTooShort(dual))
                    delaunay.SetEdgeMetadatum(edge.Item1, edge.Item2, "weight", "100000");
                else
                    delaunay.SetEdgeMetadatum(edge.Item1, edge.Item2, "weight",
                        HeightDifference(edge.Item1, edge.Item2));
            }
        }

        bool VoronoiEdgeTooShort(Tuple<Point2D, Point2D> voronoiEndpoints)
        {
            return SS(voronoiEndpoints.Item1, voronoiEndpoints.Item2) < 100;
        }

        Rational SS(Point2D p1, Point2D p2)
        {
            return (p1.X - p2.X).Squared + (p1.Y - p2.Y).Squared;
        }

        string HeightDifference(Point2D v1, Point2D v2)
        {
            var w1 = heightMap.Height(v1);
            var w2 = heightMap.Height(v2);
            var delta = (w1 - w2).AbsoluteValue;

            return delta.ToString();
        }

        void WriteVoronoiGraph()
        {
            var settings = new SvgFormatterSettings()
            {
                EdgeThickness = 2,
                OutlineThickness = 0,
                VertexDiameter = 0,
                MajorAxisSize = 1000,
                EdgeColor = "#404040",
                BackgroundColor = "#C0C0C0"
            };

            using (var output = File.CreateText(outputFilename))
                SvgGraph2DFormatter.Generate(voronoi, output, settings);
        }

        static string DualString(Point2D v1, Point2D v2)
        {
            return string.Format("{0},{1};{2},{3}",
                v1.X, v1.Y, v2.X, v2.Y);
        }

        static Tuple<Point2D, Point2D> DualValue(string p)
        {
            var tokens = p.Split(';');
            return Tuple.Create(Point2D.Parse(tokens[0]), Point2D.Parse(tokens[1]));
        }
    }
}
