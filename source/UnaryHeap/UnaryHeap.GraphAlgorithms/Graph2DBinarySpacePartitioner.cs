using System;
using System.Collections.Generic;
using System.Globalization;
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
            return ConstructBspTree(graph, new Graph2DExhaustivePartitioner(1, 10));
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
                .ConstructBspTree(graph.ConvertToGraphSegments());
        }

        static List<GraphSegment> ConvertToGraphSegments(this Graph2D data)
        {
            var edges = new List<GraphSegment>();

            foreach (var edge in data.Edges)
            {
                var line = new GraphLine(edge.Item1, edge.Item2,
                    data.GetEdgeMetadata(edge.Item1, edge.Item2));
                edges.Add(new GraphSegment(line));
            }

            return edges;
        }
    }

    class Graph2DBinarySpacePartitioner : BinarySpacePartitioner<GraphSegment, Hyperplane2D>
    {
        public Graph2DBinarySpacePartitioner(IPartitioner<GraphSegment, Hyperplane2D> partitioner)
            : base(partitioner)
        {
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

        protected override bool IsHintSurface(GraphSegment surface, int depth)
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            return surface.Source.Metadata.ContainsKey("hint")
                && surface.Source.Metadata["hint"].Equals(
                    depth.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }
    }

    class Graph2DExhaustivePartitioner : ExhaustivePartitioner<GraphSegment, Hyperplane2D>
    {
        public Graph2DExhaustivePartitioner(int imbalanceWeight, int splitWeight)
            : base(imbalanceWeight, splitWeight)
        {
        }

        public override void ClassifySurface(GraphSegment segment, Hyperplane2D plane,
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

        public override Hyperplane2D GetPlane(GraphSegment surface)
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            return surface.Hyperplane;
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