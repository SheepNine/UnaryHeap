using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
        where TSurface : Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>.SurfaceBase
    {
        class Portalization
        {
            readonly IDimension dimension;
            List<Portal> portals;
            public IEnumerable<Portal> Portals { get { return portals; } }
            List<Tuple<int, TFacet>> bspHints;
            public IEnumerable<Tuple<int, TFacet>> BspHints { get { return bspHints; } }

            public Portalization(IDimension dimension, IEnumerable<Portal> initialPortals)
            {
                this.dimension = dimension;
                portals = initialPortals.ToList();
                bspHints = new List<Tuple<int, TFacet>>();
            }

            public void SplitCell(BigInteger cell, TPlane splittingPlane,
                BigInteger newFrontCell, BigInteger newBackCell)
            {
                var cellPortals = new List<Portal>();
                var otherPortals = new List<Portal>();
                foreach (var portal in portals)
                {
                    if (portal.Front == cell)
                        cellPortals.Add(portal);
                    else if (portal.Back == cell)
                        cellPortals.Add(new Portal(
                            dimension.GetCofacet(portal.Facet), portal.Back, portal.Front));
                    else
                        otherPortals.Add(portal);
                }

                var splitCellFronts = new List<Portal>();
                var splitCellBacks = new List<Portal>();

                cellPortals.ForEach(portal =>
                {
                    dimension.Split(portal.Facet, splittingPlane,
                        out TFacet frontFacet, out TFacet backFacet);
                    if (frontFacet != null)
                    {
                        splitCellFronts.Add(new Portal(frontFacet,
                            portal.Front == cell ? newFrontCell : portal.Front,
                            portal.Back == cell ? newFrontCell : portal.Back
                        ));
                    }
                    if (backFacet != null)
                    {
                        splitCellBacks.Add(new Portal(backFacet,
                            portal.Front == cell ? newBackCell : portal.Front,
                            portal.Back == cell ? newBackCell : portal.Back
                        ));
                    }
                });

                portals = otherPortals
                    .Concat(splitCellFronts)
                    .Concat(splitCellBacks)
                    .ToList();

                if (splitCellFronts.Count > 0 && splitCellBacks.Count > 0)
                {
                    var splitFacet = dimension.Facetize(splittingPlane);
                    foreach (var portal in cellPortals)
                    {
                        if (portal.Front == cell)
                            dimension.Split(splitFacet, dimension.GetPlane(portal.Facet),
                                out splitFacet, out _);
                        else
                            dimension.Split(splitFacet, dimension.GetPlane(portal.Facet),
                                out _, out splitFacet);

                        if (splitFacet == null)
                            throw new InvalidOperationException("Split portal was clipped away "
                                + "by cell portals; should not happen");
                    }
                    portals.Add(new Portal(splitFacet, newFrontCell, newBackCell));
                    if (newFrontCell != cell)
                        bspHints.Add(Tuple.Create(cell.Depth(), splitFacet));
                }
            }
        }

        Portalization MakePortalization(IBspTree tree)
        {
            var bounds = dimension.Expand(tree.CalculateBoundingBox());
            var boundsFacets = dimension.MakeFacets(bounds);

            return new Portalization(dimension,
                boundsFacets.Select(facet => new Portal(facet, 0, NullNodeIndex)));
        }

        /// <summary>
        /// Calculate the set of portals between BSP leaves.
        /// </summary>
        /// <param name="tree">The BSP tree to portalize.</param>
        /// <param name="portals">
        /// The set of portals connecting the leaves of the BSP tree.</param>
        /// <param name="bspHints">
        /// A collection of facets that can be used to reconstruct the BSP splitting planes
        /// </param>
        public void Portalize(IBspTree tree, out IEnumerable<Portal> portals,
            out IEnumerable<Tuple<int, TFacet>> bspHints)
        {
            var portalization = MakePortalization(tree);

            tree.PreOrderTraverse((nodeIndex) =>
            {
                if (tree.IsLeaf(nodeIndex))
                {
                    var clipPlanes = tree.Surfaces(nodeIndex)
                        .Where(s => !s.Surface.IsTwoSided)
                        .Select(s => dimension.GetPlane(s.Surface.Facet))
                        .Distinct();
                    foreach (var plane in clipPlanes)
                        portalization.SplitCell(nodeIndex, plane, nodeIndex, NullNodeIndex);
                }
                else
                {
                    portalization.SplitCell(nodeIndex, tree.PartitionPlane(nodeIndex),
                        nodeIndex.FrontChildIndex(), nodeIndex.BackChildIndex());
                }
            });

            portals = portalization.Portals.Where(
                p => p.Front != NullNodeIndex && p.Back != NullNodeIndex);
            bspHints = portalization.BspHints;
        }

        /// <summary>
        /// Represents a portal facet between two BSP leaves.
        /// </summary>
        public class Portal
        {
            /// <summary>
            /// The facet defining the portal.
            /// </summary>
            public TFacet Facet { get; private set; }

            /// <summary>
            /// The BSP leaf on the front side of the facet.
            /// </summary>
            public BigInteger Front { get; private set; }

            /// <summary>
            /// The BSP leaf on the back side of the facet.
            /// </summary>
            public BigInteger Back { get; private set; }

            /// <summary>
            /// Initializes a new instance of the Portal class.
            /// </summary>
            /// <param name="facet">The facet defining the portal.</param>
            /// <param name="front">The BSP leaf on the front side of the facet.</param>
            /// <param name="back">The BSP leaf on the back side of the facet.</param>
            public Portal(TFacet facet, BigInteger front, BigInteger back)
            {
                if (facet == null)
                    throw new ArgumentNullException(nameof(facet));
                Facet = facet;
                Front = front;
                Back = back;
            }
        }
    }
}
