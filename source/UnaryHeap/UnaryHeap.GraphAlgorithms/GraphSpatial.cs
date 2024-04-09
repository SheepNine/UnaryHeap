﻿using System;
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
        public static GraphSpatial.BspNode ConstructBspTree(this Graph2D graph)
        {
            return ConstructBspTree(graph,
                GraphSpatial.Instance.ExhaustivePartitionStrategy(1, 10));
        }

        /// <summary>
        /// Constructs a BSP tree for a set of graph edges.
        /// </summary>
        /// <param name="graph">The graph to partition.</param>
        /// <param name="partitioner">The partitioner to use to construct the tree.</param>
        /// <returns>The root node of the resulting BSP tree.</returns>
        public static GraphSpatial.BspNode ConstructBspTree(this Graph2D graph,
            GraphSpatial.IPartitionStrategy partitioner)
        {
            return GraphSpatial.Instance
                .ConstructBspTree(partitioner, graph.ConvertToGraphSegments());
        }

        /// <summary>
        /// Constructs the portal set for a BSP of graph edges.
        /// </summary>
        /// <param name="root">The root of the BSP tree to portalize.</param>
        /// <returns>Portals between leaves of the BSP tree.</returns>
        public static IEnumerable<GraphSpatial.Portal> Portalize(this GraphSpatial.BspNode root)
        {
            return GraphSpatial.Instance.Portalize(root);
        }

        /// <summary>
        /// Cull leaves of a BSP tree which are not interior spaces.
        /// </summary>
        /// <param name="root">The BSP tree to cull.</param>
        /// <param name="portals">Portals between leaf nodes in the tree.</param>
        /// <param name="interiorPoints">Locations in the tree which are considered interior.
        /// </param>
        /// <returns>A new BSP with only leaves which are interior, or are connected
        /// to interior spaces.</returns>
        public static GraphSpatial.BspNode CullOutside(this GraphSpatial.BspNode root,
            IEnumerable<GraphSpatial.Portal> portals,
            IEnumerable<Point2D> interiorPoints)
        {
            return GraphSpatial.Instance.CullOutside(root, portals, interiorPoints);
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

    class GraphSpatial : Spatial2D<GraphSegment>
    {
        public static readonly GraphSpatial Instance = new();
        private GraphSpatial() : base(new GraphDimension()) { }

        class GraphDimension : Dimension
        {
            public override void Split(GraphSegment surface, Hyperplane2D partitioningPlane,
                out GraphSegment frontSurface, out GraphSegment backSurface)
            {
                if (null == surface)
                    throw new ArgumentNullException(nameof(surface));
                if (null == partitioningPlane)
                    throw new ArgumentNullException(nameof(partitioningPlane));

                frontSurface = null;
                backSurface = null;
                surface.Facet.Split(partitioningPlane,
                    out Facet2D frontFacet, out Facet2D backFacet);
                if (frontFacet != null)
                    frontSurface = new GraphSegment(frontFacet, surface.Source);
                if (backFacet != null)
                    backSurface = new GraphSegment(backFacet, surface.Source);
            }

            public override bool IsHintSurface(GraphSegment surface, int depth)
            {
                if (surface == null)
                    throw new ArgumentNullException(nameof(surface));

                return surface.Source.Metadata.ContainsKey("hint")
                    && surface.Source.Metadata["hint"].Equals(
                        depth.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
            }

            public override Facet2D GetFacet(GraphSegment surface)
            {
                return surface.Facet;
            }

            public override Orthotope2D CalculateBounds(IEnumerable<GraphSegment> surfaces)
            {
                return Orthotope2D.FromPoints(surfaces.Select(surface => surface.Facet.Start)
                    .Concat(surfaces.Select(surface => surface.Facet.End)));
            }
        }
    }

    /// <summary>
    /// POCO object containing the data for an initial Graph2D edge.
    /// </summary>
    public class GraphLine
    {
        readonly Facet2D facet;
        readonly IReadOnlyDictionary<string, string> metadata;

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
        readonly Facet2D facet;
        readonly GraphLine source;

        /// <summary>
        /// Contstructs a new instance of the GraphSegment class as
        /// a copy of a given line.
        /// </summary>
        /// <param name="source">The source line.</param>
        public GraphSegment(GraphLine source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
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
            this.source = source ?? throw new ArgumentNullException(nameof(source));
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