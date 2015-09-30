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
        IComparer<Point2D> pointComparer;

        public MazeListener()
        {
            pointComparer = new Point2DComparer();
            delaunay = new Graph2D(false);
            voronoi = new Graph2D(false);
            voronoiFaces = new SortedDictionary<Point2D, List<Point2D>>(pointComparer);
            delaunayFaces = new SortedDictionary<Point2D, List<Point2D>>(pointComparer);
        }

        public Graph2D LogicalGraph
        {
            get { return delaunay; }
        }

        public Graph2D PhysicalGraph
        {
            get { return voronoi; }
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

        static string DualString(Point2D v1, Point2D v2)
        {
            return string.Format("{0},{1};{2},{3}",
                v1.X, v1.Y, v2.X, v2.Y);
        }
    }
}
