using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.Misc;

namespace Partitioner
{
    class Graph2DBinarySpacePartitioner : BinarySpacePartitioner<GraphEdge, Hyperplane2D>
    {
        Graph2DBinarySpacePartitioner(IPartitioner<GraphEdge, Hyperplane2D> partitioner)
            : base(partitioner)
        {
        }

        public IBspNode<GraphEdge, Hyperplane2D> ConstructBspTree(Graph2D data)
        {
            var edges = new List<GraphEdge>();

            foreach (var edge in data.Edges)
                edges.Add(new GraphEdge(edge.Item1, edge.Item2,
                    data.GetEdgeMetadata(edge.Item1, edge.Item2)));

            return ConstructBspTree(edges);
        }


        protected override bool AreConvex(GraphEdge a, GraphEdge b)
        {
            return a.IsConvexWith(b);
        }

        protected override void Split(GraphEdge edge, Hyperplane2D partitionPlane,
            out GraphEdge frontSurface, out GraphEdge backSurface)
        {
            edge.Split(partitionPlane, out frontSurface, out backSurface);
        }

        public static Graph2DBinarySpacePartitioner WithExhaustivePartitioner()
        {
            return WithExhaustivePartitioner(1, 10);
        }

        public static Graph2DBinarySpacePartitioner WithExhaustivePartitioner(
            int imbalanceWeight, int splitWeight)
        {
            return new Graph2DBinarySpacePartitioner(
                new ExhaustivePartitioner(imbalanceWeight, splitWeight));
        }

        class ExhaustivePartitioner : IPartitioner<GraphEdge, Hyperplane2D>
        {
            int imbalanceWeight;
            int splitWeight;

            public ExhaustivePartitioner(int imbalanceWeight, int splitWeight)
            {
                this.imbalanceWeight = imbalanceWeight;
                this.splitWeight = splitWeight;
            }

            public Hyperplane2D SelectPartitionPlane(IEnumerable<GraphEdge> surfacesToPartition)
            {
                var hyperplanes = surfacesToPartition.Select(s => s.Hyperplane)
                    .Distinct().ToList();

                return hyperplanes.Select(h => ComputeScore(h, surfacesToPartition))
                    .Where(s => s != null).OrderBy(s => GetScore(s)).First().splitter;
            }

            int GetScore(SplitResult splitResult)
            {
                return Math.Abs(splitResult.front - splitResult.back) * imbalanceWeight
                    + splitResult.splits * splitWeight;
            }

            SplitResult ComputeScore(
                Hyperplane2D splitter, IEnumerable<GraphEdge> surfacesToPartition)
            {
                int splits = 0;
                int front = 0;
                int back = 0;

                foreach (var surface in surfacesToPartition)
                {
                    var start = splitter.DetermineHalfspaceOf(surface.Start);
                    var end = splitter.DetermineHalfspaceOf(surface.End);

                    if (start > 0)
                    {
                        if (end > 0)
                            front += 1;
                        else if (end < 0)
                            splits += 1;
                        else // end == 0
                            front += 1;
                    }
                    else if (start < 0)
                    {
                        if (end > 0)
                            splits += 1;
                        else if (end < 0)
                            back += 1;
                        else // end == 0
                            back += 1;
                    }
                    else // start == 0
                    {
                        if (end > 0)
                            front += 1;
                        else if (end < 0)
                            back += 1;
                        else // end == 0
                            if (surface.Hyperplane.Equals(splitter))
                            front += 1;
                        else
                            back += 1;
                    }
                }

                if (splits == 0 && (front == 0 || back == 0))
                    return null;
                else
                    return new SplitResult(splitter, front, back, splits);
            }

            class SplitResult
            {
                public int back;
                public int front;
                public int splits;
                public Hyperplane2D splitter;

                public SplitResult(Hyperplane2D splitter, int front, int back, int splits)
                {
                    this.splitter = splitter;
                    this.front = front;
                    this.back = back;
                    this.splits = splits;
                }
            }
        }
    }
    class GraphEdge
    {
        Point2D start;
        Point2D end;
        Hyperplane2D hyperplane;
        IReadOnlyDictionary<string, string> metadata;

        public GraphEdge(Point2D start, Point2D end,
            IReadOnlyDictionary<string, string> metadata)
            : this(start, end, new Hyperplane2D(start, end), metadata)
        {
        }

        GraphEdge(Point2D start, Point2D end, Hyperplane2D hyperplane,
            IReadOnlyDictionary<string, string> metadata)
        {
            this.start = start;
            this.end = end;
            this.hyperplane = hyperplane;
            this.metadata = metadata;
        }

        public Point2D Start
        {
            get { return start; }
        }

        public Point2D End
        {
            get { return end; }
        }

        public Hyperplane2D Hyperplane
        {
            get { return hyperplane; }
        }

        public IReadOnlyDictionary<string, string> Metadata
        {
            get { return metadata; }
        }

        public void Split(Hyperplane2D splitter,
            out GraphEdge frontSurface, out GraphEdge backSurface)
        {
            var startSpace = splitter.DetermineHalfspaceOf(start);
            var endSpace = splitter.DetermineHalfspaceOf(end);

            if (startSpace > 0)
            {
                if (endSpace > 0)
                {
                    frontSurface = this;
                    backSurface = null;
                }
                else if (endSpace < 0)
                {
                    var middle = splitter.FindIntersection(hyperplane);
                    frontSurface = new GraphEdge(start, middle, hyperplane, metadata);
                    backSurface = new GraphEdge(middle, end, hyperplane, metadata);
                }
                else // endSpace == 0
                {
                    frontSurface = this;
                    backSurface = null;
                }
            }
            else if (startSpace < 0)
            {
                if (endSpace > 0)
                {
                    var middle = splitter.FindIntersection(hyperplane);
                    frontSurface = new GraphEdge(end, middle, hyperplane, metadata);
                    backSurface = new GraphEdge(middle, start, hyperplane, metadata);
                }
                else if (endSpace < 0)
                {
                    frontSurface = null;
                    backSurface = this;
                }
                else // endSpace == 0
                {
                    frontSurface = null;
                    backSurface = this;
                }
            }
            else // startSpace == 0
            {
                if (endSpace > 0)
                {
                    frontSurface = this;
                    backSurface = null;
                }
                else if (endSpace < 0)
                {
                    frontSurface = null;
                    backSurface = this;
                }
                else // endSpace == 0
                {
                    if (hyperplane.Equals(splitter))
                    {
                        frontSurface = this;
                        backSurface = null;
                    }
                    else
                    {
                        frontSurface = null;
                        backSurface = this;
                    }
                }
            }
        }

        public bool IsConvexWith(GraphEdge other)
        {
            return
                this.hyperplane.DetermineHalfspaceOf(other.start) >= 0 &&
                this.hyperplane.DetermineHalfspaceOf(other.end) >= 0 &&
                other.hyperplane.DetermineHalfspaceOf(this.start) >= 0 &&
                other.hyperplane.DetermineHalfspaceOf(this.end) >= 0;
        }
    }
}
