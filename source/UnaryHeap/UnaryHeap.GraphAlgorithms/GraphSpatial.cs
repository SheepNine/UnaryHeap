using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
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
        public static GraphSpatial.IBspTree ConstructBspTree(this Graph2D graph)
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
        public static GraphSpatial.IBspTree ConstructBspTree(this Graph2D graph,
            Func<IEnumerable<GraphSpatial.IBspSurface>, Hyperplane2D> partitioner)
        {
            return GraphSpatial.Instance
                .ConstructBspTree(partitioner, graph.ConvertToGraphSegments());
        }

        /// <summary>
        /// Constructs the portal set for a BSP of graph edges.
        /// </summary>
        /// <param name="root">The root of the BSP tree to portalize.</param>
        /// <param name="portals">
        /// The set of portals connecting the leaves of the BSP tree.</param>
        /// <param name="bspHints">
        /// A collection of facets that can be used to reconstruct the BSP splitting planes
        /// </param>
        public static void Portalize(this GraphSpatial.IBspTree root,
            out IEnumerable<GraphSpatial.Portal> portals,
            out IEnumerable<Tuple<int, Facet2D>> bspHints)
        {
            GraphSpatial.Instance.Portalize(root, out portals, out bspHints);
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
        public static GraphSpatial.IBspTree CullOutside(this GraphSpatial.IBspTree root,
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
                var metadata = data.GetEdgeMetadata(edge.Item1, edge.Item2);

                var line = new GraphLine(edge.Item1, edge.Item2, metadata);
                edges.Add(new GraphSegment(line, 0, 1));
            }

            return edges;
        }
    }

    /// <summary>
    /// Implements the Spatial2D abstract class with customizations for surfaces that
    /// come from Graph objects.
    /// </summary>
    public class GraphSpatial : Spatial2D<GraphSegment>
    {
        /// <summary>
        /// Gets the singleton instance of the GraphSpatial class.
        /// </summary>
        public static readonly GraphSpatial Instance = new();
        private GraphSpatial() : base() { }
    }

    /// <summary>
    /// POCO object containing the data for an initial Graph2D edge.
    /// </summary>
    public class GraphLine
    {
        /// <summary>
        /// Graph metadata key whose presence indicates a two-sided line.
        /// </summary>
        public const string TwoSidedKey = "threeSidedLolJK";

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

        /// <summary>
        /// Whether this graph line is two-sided.
        /// </summary>
        public bool IsTwoSided
        {
            get { return metadata.ContainsKey(TwoSidedKey); }
        }
    }

    /// <summary>
    /// POCO object containing the data for a segment of a GraphLine.
    /// Initially all segments in a BSP are copies of the GraphLines,
    /// but they may be split into sub-segments by the partitioning planes.
    /// </summary>
    public class GraphSegment : GraphSpatial.SurfaceBase
    {
        /// <summary>
        /// Gets the source line for the edge.
        /// </summary>
        public GraphLine Source { get; private set; }

        /// <summary>
        /// Gets a copy of a surface with the front and back sides reversed.
        /// </summary>
        public override GraphSegment Cosurface
        {
            get
            {
                return new GraphSegment(Facet.Cofacet, Source, BackMaterial, FrontMaterial);
            }
        }

        /// <summary>
        /// Contstructs a new instance of the GraphSegment class as
        /// a copy of a given line.
        /// </summary>
        /// <param name="source">The source line.</param>
        /// <param name="frontMaterial">
        /// The material on the front of the surface.
        /// </param>
        /// <param name="backMaterial">
        /// The material on the front of the surface.
        /// </param>
        public GraphSegment(GraphLine source, int frontMaterial, int backMaterial)
            : this(source.Facet, source, frontMaterial, backMaterial)
        {
        }

        /// <summary>
        /// Contstructs a new instance of the GraphSegment class as
        /// a copy of a given line.
        /// </summary>
        /// <param name="facet">The line segment of this graph segment.</param>
        /// <param name="source">The source line.</param>
        /// <param name="frontMaterial">
        /// The material on the front of the surface.
        /// </param>
        /// <param name="backMaterial">
        /// The material on the front of the surface.
        /// </param>
        public GraphSegment(Facet2D facet, GraphLine source, int frontMaterial, int backMaterial)
            : base(facet, frontMaterial, backMaterial)
        {
            Source = source;
        }


        /// <summary>
        /// Checks if this surface is a 'hint surface' used to speed up the first few levels
        /// of BSP partitioning by avoiding an exhaustive search for a balanced plane.
        /// </summary>
        /// <param name="depth">The current depth of the BSP tree.</param>
        /// <returns>True of this surface should be used for a partitioning plane
        /// (and discarded from the final BSP tree), false otherwise.</returns>
        public override bool IsHintSurface(int depth)
        {
            return Source.Metadata.ContainsKey("hint")
                && Source.Metadata["hint"].Equals(
                    depth.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        /// <summary>
        /// Splits a surface into two subsurfaces lying on either side of a
        /// partitioning plane.
        /// If surface lies on the partitioningPlane, it should be considered in the
        /// front halfspace of partitioningPlane if its front halfspace is identical
        /// to that of partitioningPlane. Otherwise, it should be considered in the 
        /// back halfspace of partitioningPlane.
        /// </summary>
        /// <param name="partitioningPlane">The plane used to split surface.</param>
        /// <param name="frontSurface">The subsurface of surface lying in the front
        /// halfspace of partitioningPlane, or null, if surface is entirely in the
        /// back halfspace of partitioningPlane.</param>
        /// <param name="backSurface">The subsurface of surface lying in the back
        /// halfspace of partitioningPlane, or null, if surface is entirely in the
        /// front halfspace of partitioningPlane.</param>
        public override void Split(Hyperplane2D partitioningPlane,
            out GraphSegment frontSurface, out GraphSegment backSurface)
        {
            frontSurface = null;
            backSurface = null;
            Facet.Split(partitioningPlane,
                out Facet2D frontFacet, out Facet2D backFacet);
            if (frontFacet != null)
                frontSurface = new GraphSegment(frontFacet, Source,
                    FrontMaterial, BackMaterial);
            if (backFacet != null)
                backSurface = new GraphSegment(backFacet, Source,
                    FrontMaterial, BackMaterial);
        }

        /// <summary>
        /// Makes a copy of a surface, with the front material replaced.
        /// </summary>
        /// <param name="material">The material to fill in the front.</param>
        /// <returns>The copied surface.</returns>
        public override GraphSegment FillFront(int material)
        {
            return new GraphSegment(Facet, Source, material, BackMaterial);
        }

        /// <summary>
        /// Makes a copy of a surface, with any edges between the given facets and this
        /// surface's facets healed.
        /// </summary>
        /// <param name="facets">The other surfaces that are potentially adjacent
        /// to this surface.</param>
        /// <returns>A new Surface that has no cracks with the input facets.</returns>
        public override GraphSegment HealEdges(List<Facet2D> facets)
        {
            return this; // 2D case cannot have edge cracks
        }

        /// <summary>
        /// Whether this surface is two-sided (i.e. both its front and back halves are
        /// interior spaces.
        /// </summary>
        public override bool IsTwoSided
        {
            get { return Source.IsTwoSided; }
        }
    }
}