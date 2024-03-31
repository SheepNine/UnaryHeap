using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.DataType;

namespace UnaryHeap.Graph
{
    /// <summary>
    /// Contains extension methods for the Graph2D class.
    /// </summary>
    public static partial class GraphAlgorithmExtensions
    {
        /// <summary>
        /// Constructs a BSP tree for a set of graph edges.
        /// </summary>
        /// <param name="graph">The graph to partition.</param>
        /// <returns>The root node of the resulting BSP tree.</returns>
        public static IBspNode<GraphSegment, Hyperplane2D> ConstructBspTree(this Graph2D graph)
        {
            return Graph2DBinarySpacePartitioner.WithExhaustivePartitioner()
                .ConstructBspTree(graph);
        }

        /// <summary>
        /// Constructs a BSP tree for a set of graph edges.
        /// </summary>
        /// <param name="graph">The graph to partition.</param>
        /// <param name="partitioner">The partitioner to use to construct the tree.</param>
        /// <returns>The root node of the resulting BSP tree.</returns>
        public static IBspNode<GraphSegment, Hyperplane2D> ConstructBspTree(this Graph2D graph,
            IPartitioner<GraphSegment, Hyperplane2D> partitioner)
        {
            return new Graph2DBinarySpacePartitioner(partitioner)
                .ConstructBspTree(graph);
        }

        /// <summary>
        /// Picks an optimal partitioning plane based on a computed score.
        /// </summary>
        /// <param name="surfacesToPartition">The surfaces to partition.</param>
        /// <param name="imbalanceWeight">The weight to give the imbalance in the
        /// count between surfaces on the front and back of a given hyperplane.</param>
        /// <param name="splitWeight">The weight to give to the number of surfaces
        /// that are split by a given hyperplane.</param>
        /// <returns>The hyperplane from the surface to partition that minimizes
        /// the overall parition score.</returns>
        public static Hyperplane2D SearchExhaustivelyForPartitionPlane(
                IEnumerable<GraphSegment> surfacesToPartition,
                int imbalanceWeight, int splitWeight)
        {
            return SearchExhaustivelyForPartitionPlane(
                surfacesToPartition, (s) => true, imbalanceWeight, splitWeight);
        }

        /// <summary>
        /// Picks an optimal partitioning plane based on a computed score,
        /// giving preference to one group of planes over another.
        /// </summary>
        /// <param name="surfacesToPartition">The surfaces to partition.</param>
        /// <param name="isPreferredPartitionSegument">
        /// A function that determines whether a surface is preferred for being
        /// used as a splitting plane.
        /// </param>
        /// <param name="imbalanceWeight">The weight to give the imbalance in the
        /// count between surfaces on the front and back of a given hyperplane.</param>
        /// <param name="splitWeight">The weight to give to the number of surfaces
        /// that are split by a given hyperplane.</param>
        /// <returns>The hyperplane from the surface to partition that minimizes
        /// the overall parition score.</returns>
        public static Hyperplane2D SearchExhaustivelyForPartitionPlane(
                IEnumerable<GraphSegment> surfacesToPartition,
                Func<GraphSegment, bool> isPreferredPartitionSegument,
                int imbalanceWeight, int splitWeight)
        {
            var choices = surfacesToPartition
                .Where(s => isPreferredPartitionSegument(s))
                .Select(s => s.Hyperplane)
                .Distinct()
                .Select(h => ComputeScore(h, surfacesToPartition))
                .Where(score => score != null)
                .ToList();

            if (choices.Count == 0)
            {
                choices = surfacesToPartition
                   .Select(s => s.Hyperplane)
                   .Distinct()
                   .Select(h => ComputeScore(h, surfacesToPartition))
                   .Where(score => score != null)
                   .ToList();
            }

            return choices
                .OrderBy(score => GetScore(score, imbalanceWeight, splitWeight))
                .First()
                .splitter;
        }

        static int GetScore(SplitResult splitResult, int imbalanceWeight, int splitWeight)
        {
            return Math.Abs(splitResult.front - splitResult.back) * imbalanceWeight
                + splitResult.splits * splitWeight;
        }

        static SplitResult ComputeScore(Hyperplane2D splitter,
            IEnumerable<GraphSegment> surfacesToPartition)
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

    class Graph2DBinarySpacePartitioner : BinarySpacePartitioner<GraphSegment, Hyperplane2D>
    {
        public Graph2DBinarySpacePartitioner(IPartitioner<GraphSegment, Hyperplane2D> partitioner)
            : base(partitioner)
        {
        }

        public IBspNode<GraphSegment, Hyperplane2D> ConstructBspTree(Graph2D data)
        {
            var edges = new List<GraphSegment>();

            foreach (var edge in data.Edges)
            {
                var line = new GraphLine(edge.Item1, edge.Item2,
                    data.GetEdgeMetadata(edge.Item1, edge.Item2));
                edges.Add(new GraphSegment(line));
            }

            return ConstructBspTree(edges);
        }

        protected override Hyperplane2D GetPlane(GraphSegment surface)
        {
            return surface.Hyperplane;
        }

        protected override void ClassifySurface(GraphSegment segment, Hyperplane2D plane,
            out int minDeterminant, out int maxDeterminant)
        {
            if (segment.Hyperplane == plane)
            {
                minDeterminant = 1;
                maxDeterminant = 1;
                return;
            }
            if (segment.Hyperplane == plane.Coplane)
            {
                minDeterminant = -1;
                maxDeterminant = -1;
                return;
            }
            var d1 = plane.DetermineHalfspaceOf(segment.Start);
            var d2 = plane.DetermineHalfspaceOf(segment.End);

            minDeterminant = Math.Min(d1, d2);
            maxDeterminant = Math.Max(d1, d2);
        }

        protected override void Split(GraphSegment edge, Hyperplane2D partitionPlane,
            out GraphSegment frontSurface, out GraphSegment backSurface)
        {
            if (null == edge)
                throw new ArgumentNullException(nameof(edge));
            if (null == partitionPlane)
                throw new ArgumentNullException(nameof(partitionPlane));

            var startSpace = partitionPlane.DetermineHalfspaceOf(edge.Start);
            var endSpace = partitionPlane.DetermineHalfspaceOf(edge.End);

            if (startSpace > 0)
            {
                if (endSpace > 0)
                {
                    frontSurface = edge;
                    backSurface = null;
                }
                else if (endSpace < 0)
                {
                    var middle = partitionPlane.FindIntersection(edge.Hyperplane);
                    frontSurface = new GraphSegment(edge.Start, middle, edge.Source);
                    backSurface = new GraphSegment(middle, edge.End, edge.Source);
                }
                else // endSpace == 0
                {
                    frontSurface = edge;
                    backSurface = null;
                }
            }
            else if (startSpace < 0)
            {
                if (endSpace > 0)
                {
                    var middle = partitionPlane.FindIntersection(edge.Hyperplane);
                    frontSurface = new GraphSegment(edge.End, middle, edge.Source);
                    backSurface = new GraphSegment(middle, edge.Start, edge.Source);
                }
                else if (endSpace < 0)
                {
                    frontSurface = null;
                    backSurface = edge;
                }
                else // endSpace == 0
                {
                    frontSurface = null;
                    backSurface = edge;
                }
            }
            else // startSpace == 0
            {
                if (endSpace > 0)
                {
                    frontSurface = edge;
                    backSurface = null;
                }
                else if (endSpace < 0)
                {
                    frontSurface = null;
                    backSurface = edge;
                }
                else // endSpace == 0
                {
                    if (edge.Hyperplane.Equals(partitionPlane))
                    {
                        frontSurface = edge;
                        backSurface = null;
                    }
                    else
                    {
                        frontSurface = null;
                        backSurface = edge;
                    }
                }
            }
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

        protected override bool IsHintSurface(GraphSegment surface, int depth)
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            return surface.Source.Metadata.ContainsKey("hint")
                && surface.Source.Metadata["hint"].Equals(
                    depth.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        class ExhaustivePartitioner : IPartitioner<GraphSegment, Hyperplane2D>
        {
            int imbalanceWeight;
            int splitWeight;

            public ExhaustivePartitioner(int imbalanceWeight, int splitWeight)
            {
                this.imbalanceWeight = imbalanceWeight;
                this.splitWeight = splitWeight;
            }

            public Hyperplane2D SelectPartitionPlane(
                IEnumerable<GraphSegment> surfacesToPartition)
            {
                return GraphAlgorithmExtensions.SearchExhaustivelyForPartitionPlane(
                    surfacesToPartition, imbalanceWeight, splitWeight);
            }

            public Hyperplane2D GetPlane(GraphSegment surface)
            {
                if (surface == null)
                    throw new ArgumentNullException(nameof(surface));

                return surface.Hyperplane;
            }
        }
    }

    /// <summary>
    /// POCO object containing the data for an initial Graph2D edge.
    /// </summary>
    public class GraphLine
    {
        Point2D start;
        Point2D end;
        Hyperplane2D hyperplane;
        IReadOnlyDictionary<string, string> metadata;

        /// <summary>
        /// Contstructs a new instance of the GraphEdge class.
        /// </summary>
        /// <param name="start">The edge start point.</param>
        /// <param name="end">The edge end point.</param>
        /// <param name="metadata">The metadata for the edge.</param>
        public GraphLine(Point2D start, Point2D end,
            IReadOnlyDictionary<string, string> metadata)
        {
            this.start = start;
            this.end = end;
            this.hyperplane = new Hyperplane2D(start, end);
            this.metadata = metadata;
        }

        /// <summary>
        /// Gets the edge start point.
        /// </summary>
        public Point2D Start
        {
            get { return start; }
        }

        /// <summary>
        /// Gets the edge end point.
        /// </summary>
        public Point2D End
        {
            get { return end; }
        }

        /// <summary>
        /// Gets the Hyperplane2D containing the edge.
        /// </summary>
        public Hyperplane2D Hyperplane
        {
            get { return hyperplane; }
        }

        /// <summary>
        /// Gets the metadata for the edge.
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata
        {
            get { return metadata; }
        }
    }

    /// <summary>
    /// POCO object containing the data for a segment of a GraphLine.
    /// Initially all segments in a BSP are copies of the GraphLines,
    /// but they may be split into sub-segments by the partitioning planes.
    /// </summary>
    public class GraphSegment
    {
        Point2D start;
        Point2D end;
        GraphLine source;

        /// <summary>
        /// Contstructs a new instance of the GraphSegment class as
        /// a copy of a given line.
        /// </summary>
        /// <param name="source">The source line.</param>
        public GraphSegment(GraphLine source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            this.start = source.Start;
            this.end = source.End;
            this.source = source;
        }

        /// <summary>
        /// Contstructs a new instance of the GraphSegment class as
        /// a copy of a given line.
        /// </summary>
        /// <param name="start">The start vertex of the segment.</param>
        /// <param name="end">The end vertex of the segment.</param>
        /// <param name="source">The source line.</param>
        public GraphSegment(Point2D start, Point2D end, GraphLine source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            this.start = start;
            this.end = end;
            this.source = source;
        }

        /// <summary>
        /// Gets the segment start point.
        /// </summary>
        public Point2D Start
        {
            get { return start; }
        }

        /// <summary>
        /// Gets the segment end point.
        /// </summary>
        public Point2D End
        {
            get { return end; }
        }

        /// <summary>
        /// Gets the Hyperplane2D containing the segment.
        /// </summary>
        public Hyperplane2D Hyperplane
        {
            get { return source.Hyperplane; }
        }

        /// <summary>
        /// Gets the source line for the edge.
        /// </summary>
        public GraphLine Source
        {
            get { return source; }
        }
    }
}