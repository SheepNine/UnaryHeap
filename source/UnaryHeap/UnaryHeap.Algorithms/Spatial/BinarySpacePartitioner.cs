using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
        where TSurface : Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>.SurfaceBase
    {
        /// <summary>
        /// Interface defining a strategy for partitioning sets of surfaces.
        /// </summary>
        public interface IPartitionStrategy
        {
            /// <summary>
            /// Selects a partitioning plane to be used to partition a set of surfaces.
            /// </summary>
            /// <param name="surfacesToPartition">The set of surfaces to partition.</param>
            /// <returns>The selected plane.</returns>
            TPlane SelectPartitionPlane(IEnumerable<TSurface> surfacesToPartition);
        }

        /// <summary>
        /// Constructs a BSP tree for a set of input surfaces.
        /// </summary>
        /// <param name="strategy">The strategy to use to select a partitioning plane.</param>
        /// <param name="inputSurfaces">The surfaces to partition.</param>
        /// <returns>The root node of the resulting BSP tree.</returns>
        public IBspTree ConstructBspTree(IPartitionStrategy strategy,
            IEnumerable<TSurface> inputSurfaces)
        {
            if (null == inputSurfaces)
                throw new ArgumentNullException(nameof(inputSurfaces));

            var surfaces = inputSurfaces.ToList();

            if (0 == surfaces.Count)
                throw new ArgumentException("No surfaces to partition.");

            var result = new BspTree(dimension);
            ConstructBspNode(strategy, surfaces, result, 0);
            return result;
        }

        void ConstructBspNode(IPartitionStrategy partitioner,
            List<TSurface> surfaces, BspTree tree, int index)
        {
            if (AllConvex(surfaces))
            {
                tree.AddLeaf(index, surfaces);
                return;
            }

            var depth = index.Depth();
            var hintSurface = FindHintSurface(surfaces, depth);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            TPlane partitionPlane;
            if (null != hintSurface)
            {
                partitionPlane = dimension.GetPlane(hintSurface.Facet);
                surfaces.Remove(hintSurface);
            }
            else
            {
                partitionPlane = partitioner.SelectPartitionPlane(surfaces);
            }
            stopwatch.Stop();
            debug.SplittingPlaneChosen(stopwatch.ElapsedMilliseconds,
                surfaces, depth, partitionPlane);

            if (null == partitionPlane)
                throw new InvalidOperationException("Failed to select partition plane.");

            stopwatch.Restart();
            Partition(surfaces, partitionPlane, out List<TSurface> frontSurfaces,
                out List<TSurface> backSurfaces);
            stopwatch.Stop();

            debug.PartitionOccurred(stopwatch.ElapsedMilliseconds, surfaces, depth,
                partitionPlane, frontSurfaces, backSurfaces);

            if (0 == frontSurfaces.Count || 0 == backSurfaces.Count)
                throw new InvalidOperationException(
                    "Partition plane selected does not partition surfaces.");

            tree.AddBranch(index, partitionPlane);
            ConstructBspNode(partitioner, frontSurfaces, tree, index.FrontChildIndex());
            ConstructBspNode(partitioner, backSurfaces, tree, index.BackChildIndex());
        }

        bool AllConvex(List<TSurface> surfaces)
        {
            foreach (var i in Enumerable.Range(0, surfaces.Count))
                foreach (var j in Enumerable.Range(i + 1, surfaces.Count - i - 1))
                    if (false == AreConvex(surfaces[i].Facet, surfaces[j].Facet))
                        return false;

            return true;
        }

        static TSurface FindHintSurface(List<TSurface> surfaces, int depth)
        {
            return surfaces.FirstOrDefault(surface => surface.IsHintSurface(depth));
        }

        static void Partition(List<TSurface> surfaces, TPlane partitionPlane,
            out List<TSurface> frontSurfaces, out List<TSurface> backSurfaces)
        {
            frontSurfaces = new List<TSurface>();
            backSurfaces = new List<TSurface>();

            foreach (var surface in surfaces)
            {
                surface.Split(partitionPlane, out TSurface frontSurface,
                    out TSurface backSurface);

                if (null != frontSurface)
                    frontSurfaces.Add(frontSurface);
                if (null != backSurface)
                    backSurfaces.Add(backSurface);
            }
        }

        /// <summary>
        /// Checks whether two surfaces are mutually convex (that is, neither one is
        /// behind the other). Surfaces which are convex do not need to be partitioned.
        /// </summary>
        /// <param name="a">The first surface to check.</param>
        /// <param name="b">The second surface to check.</param>
        /// <returns>True, if a is in the front halfspace of b and vice versa;
        /// false otherwise.</returns>
        protected bool AreConvex(TFacet a, TFacet b)
        {
            if (null == a)
                throw new ArgumentNullException(nameof(a));
            if (null == b)
                throw new ArgumentNullException(nameof(b));

            dimension.ClassifySurface(a, dimension.GetPlane(b), out int aMin, out _);
            dimension.ClassifySurface(b, dimension.GetPlane(a), out int bMin, out _);

            return aMin >= 0 && bMin >= 0;
        }
    }
}
