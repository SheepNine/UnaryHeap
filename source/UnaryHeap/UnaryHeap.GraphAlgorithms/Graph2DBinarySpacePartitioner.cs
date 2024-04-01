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

            frontSurface = null;
            backSurface = null;
            edge.Facet.Split(partitionPlane, out Facet2D frontFacet, out Facet2D backFacet);
            if (frontFacet != null)
                frontSurface = new GraphSegment(frontFacet, edge.Source);
            if (backFacet != null)
                backSurface = new GraphSegment(backFacet, edge.Source);
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
            if (segment.Facet.Plane == plane)
            {
                minDeterminant = 1;
                maxDeterminant = 1;
                return;
            }
            if (segment.Facet.Plane == plane.Coplane)
            {
                minDeterminant = -1;
                maxDeterminant = -1;
                return;
            }
            var d1 = plane.DetermineHalfspaceOf(segment.Facet.Start);
            var d2 = plane.DetermineHalfspaceOf(segment.Facet.End);

            minDeterminant = Math.Min(d1, d2);
            maxDeterminant = Math.Max(d1, d2);
        }

        public override Hyperplane2D GetPlane(GraphSegment surface)
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            return surface.Facet.Plane;
        }
    }

    /// <summary>
    /// POCO object containing the data for an initial Graph2D edge.
    /// </summary>
    public class GraphLine
    {
        Facet2D facet;
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
            facet = new Facet2D(new Hyperplane2D(start, end), start, end);
            this.metadata = metadata;
        }

        /// <summary>
        /// Gets the line's segment.
        /// </summary>
        public Facet2D Facet
        {
            get { return facet; }
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
        Facet2D facet;
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

            this.source = source;
            this.facet = source.Facet;
        }

        /// <summary>
        /// Contstructs a new instance of the GraphSegment class as
        /// a copy of a given line.
        /// </summary>
        /// <param name="facet">The line segment of this graph segment.</param>
        /// <param name="source">The source line.</param>
        public GraphSegment(Facet2D facet, GraphLine source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            this.source = source;
            this.facet = facet;
        }

        /// <summary>
        /// The line segment of this graph segment.
        /// </summary>
        public Facet2D Facet
        {
            get { return facet; }
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