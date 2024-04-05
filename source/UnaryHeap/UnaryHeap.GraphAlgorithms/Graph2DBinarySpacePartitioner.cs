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
            return ConstructBspTree(graph,
                new ExhaustivePartitioner2D<GraphSegment>(GraphDimension.Instance, 1, 10));
        }

        /// <summary>
        /// Constructs a BSP tree for a set of graph edges.
        /// </summary>
        /// <param name="graph">The graph to partition.</param>
        /// <param name="partitioner">The partitioner to use to construct the tree.</param>
        /// <returns>The root node of the resulting BSP tree.</returns>
        public static IBspNode<GraphSegment, Hyperplane2D> ConstructBspTree(this Graph2D graph,
            IPartitioner<GraphSegment, Hyperplane2D, Orthotope2D, Facet2D> partitioner)
        {
            return new BinarySpacePartitioner2D<GraphSegment>(
                GraphDimension.Instance, partitioner)
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

    /// <summary>
    /// TODO
    /// </summary>
    public class GraphDimension : Dimension2D<GraphSegment>
    {
        public static readonly GraphDimension Instance = new GraphDimension();
        private GraphDimension() { }

        /// <summary>
        /// Splits a surface into two subsurfaces lying on either side of a
        /// partitioning plane.
        /// If surface lies on the partitioningPlane, it should be considered in the
        /// front halfspace of partitioningPlane if its front halfspace is identical
        /// to that of partitioningPlane. Otherwise, it should be considered in the 
        /// back halfspace of partitioningPlane.
        /// </summary>
        /// <param name="surface">The surface to split.</param>
        /// <param name="partitioningPlane">The plane used to split surface.</param>
        /// <param name="frontSurface">The subsurface of surface lying in the front
        /// halfspace of partitioningPlane, or null, if surface is entirely in the
        /// back halfspace of partitioningPlane.</param>
        /// <param name="backSurface">The subsurface of surface lying in the back
        /// halfspace of partitioningPlane, or null, if surface is entirely in the
        /// front halfspace of partitioningPlane.</param>
        public override void Split(GraphSegment surface, Hyperplane2D partitioningPlane,
            out GraphSegment frontSurface, out GraphSegment backSurface)
        {
            if (null == surface)
                throw new ArgumentNullException(nameof(surface));
            if (null == partitioningPlane)
                throw new ArgumentNullException(nameof(partitioningPlane));

            frontSurface = null;
            backSurface = null;
            surface.Facet.Split(partitioningPlane, out Facet2D frontFacet, out Facet2D backFacet);
            if (frontFacet != null)
                frontSurface = new GraphSegment(frontFacet, surface.Source);
            if (backFacet != null)
                backSurface = new GraphSegment(backFacet, surface.Source);
        }

        /// <summary>
        /// Checks if a surface is a 'hint surface' used to speed up the first few levels
        /// of BSP partitioning by avoiding an exhaustive search for a balanced plane.
        /// </summary>
        /// <param name="surface">The surface to check.</param>
        /// <param name="depth">The current depth of the BSP tree.</param>
        /// <returns>True of this surface should be used for a partitioning plane
        /// (and discarded from the final BSP tree), false otherwise.</returns>
        public override bool IsHintSurface(GraphSegment surface, int depth)
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            return surface.Source.Metadata.ContainsKey("hint")
                && surface.Source.Metadata["hint"].Equals(
                    depth.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the min and max determinant for a surface against a plane.
        /// If the surface is coincident with the plane, min=max=1.
        /// If the surface is coincident with the coplane, min=max=-1.
        /// Otherwise, this gives the range of determinants of the surface against the plane.
        /// </summary>
        /// <param name="surface">The surface to classify.</param>
        /// <param name="plane">The plane to classify against.</param>
        /// <param name="minDeterminant">
        /// The smallest determinant among the surface's points.</param>
        /// <param name="maxDeterminant">
        /// The greatest determinant among the surface's points.
        /// </param>
        public override void ClassifySurface(GraphSegment surface, Hyperplane2D plane,
            out int minDeterminant, out int maxDeterminant)
        {
            if (surface.Facet.Plane == plane)
            {
                minDeterminant = 1;
                maxDeterminant = 1;
                return;
            }
            if (surface.Facet.Plane == plane.Coplane)
            {
                minDeterminant = -1;
                maxDeterminant = -1;
                return;
            }
            var d1 = plane.DetermineHalfspaceOf(surface.Facet.Start);
            var d2 = plane.DetermineHalfspaceOf(surface.Facet.End);

            minDeterminant = Math.Min(d1, d2);
            maxDeterminant = Math.Max(d1, d2);
        }

        /// <summary>
        /// Gets the plane of a surface.
        /// </summary>
        /// <param name="surface">The surface from which to get the plane.</param>
        /// <returns>The plane of the surface.</returns>
        public override Hyperplane2D GetPlane(GraphSegment surface)
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            return surface.Facet.Plane;
        }

        public override Orthotope2D CalculateBounds(IEnumerable<GraphSegment> surfaces)
        {
            return Orthotope2D.FromPoints(surfaces.Select(surface => surface.Facet.Start)
                .Concat(surfaces.Select(surface => surface.Facet.End)));
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