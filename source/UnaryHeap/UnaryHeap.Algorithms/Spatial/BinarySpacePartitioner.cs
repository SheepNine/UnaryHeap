using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
        where TSurface : Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>.SurfaceBase
    {

        /// <summary>
        /// Constructs a BSP tree for a set of input surfaces.
        /// </summary>
        /// <param name="strategy">The strategy to use to select a partitioning plane.</param>
        /// <param name="inputSurfaces">The surfaces to partition.</param>
        /// <returns>The root node of the resulting BSP tree.</returns>
        public IBspTree ConstructBspTree(Func<IEnumerable<IBspSurface>, TPlane> strategy,
            IEnumerable<TSurface> inputSurfaces)
        {
            if (null == inputSurfaces)
                throw new ArgumentNullException(nameof(inputSurfaces));

            var surfaces = inputSurfaces.Select(s => new BspSurface()
            {
                Surface = s,
                FrontLeaf = 0,
                BackLeaf = s.IsTwoSided ? 0 : NullNodeIndex
            }).ToList();
            var branchPlanes = new Dictionary<BigInteger, TPlane>();

            if (0 == surfaces.Count)
                throw new ArgumentException("No surfaces to partition.");

            PartitionSurfaces(strategy, surfaces, branchPlanes, 0);
            var result = new BspTree(dimension);
            result.Populate(branchPlanes, surfaces);
            return result;
        }

        private void PartitionSurfaces(Func<IEnumerable<IBspSurface>, TPlane> strategy,
            List<BspSurface> allSurfaces,
            Dictionary<BigInteger, TPlane> branchPlanes, BigInteger index)
        {
            allSurfaces.Sort(BringToFront(index));

            // TODO: optimize this a bit?
            var nodeSurfaceCount = allSurfaces.Count(
                s => s.FrontLeaf == index || s.BackLeaf == index);
            var frontSurfaceCount = allSurfaces.Take(nodeSurfaceCount)
                .Count(s => s.FrontLeaf == index);
            var frontSurfaces = allSurfaces.Take(frontSurfaceCount).ToList();

            if (AllConvex(frontSurfaces))
                return;

            var depth = index.Depth();
            var hintSurface = FindHintSurface(frontSurfaces, depth);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            TPlane partitionPlane;
            if (null != hintSurface)
            {
                partitionPlane = dimension.GetPlane(hintSurface.Surface.Facet);
                allSurfaces.Remove(hintSurface);
                nodeSurfaceCount -= 1; // probably needed
            }
            else
            {
                partitionPlane = strategy(frontSurfaces);
            }
            stopwatch.Stop();
            debug.SplittingPlaneChosen(stopwatch.ElapsedMilliseconds,
                frontSurfaces.Count, depth, partitionPlane);

            if (null == partitionPlane)
                throw new InvalidOperationException("Failed to select partition plane.");

            var frontSurface = false;
            var backSurface = false;
            stopwatch.Restart();
            foreach (var i in Enumerable.Range(0, nodeSurfaceCount))
            {
                var surface = allSurfaces[i];
                var surfacePlane = dimension.GetPlane(surface.Surface.Facet);
                if (surfacePlane.Equals(partitionPlane))
                {
                    frontSurface = true;
                    if (surface.FrontLeaf == index)
                        surface.FrontLeaf = index.FrontChildIndex();
                    if (surface.BackLeaf == index)
                        surface.BackLeaf = index.BackChildIndex();
                }
                else if (surfacePlane.Equals(dimension.GetCoplane(partitionPlane)))
                {
                    backSurface = true;
                    if (surface.FrontLeaf == index)
                        surface.FrontLeaf = index.BackChildIndex();
                    if (surface.BackLeaf == index)
                        surface.BackLeaf = index.FrontChildIndex();
                }
                else
                {
                    surface.Surface.Split(partitionPlane,
                        out TSurface frontPiece, out TSurface backPiece);
                    
                    if (backPiece == null)
                    {
                        frontSurface = true;
                        if (surface.FrontLeaf == index)
                            surface.FrontLeaf = index.FrontChildIndex();
                        if (surface.BackLeaf == index)
                            surface.BackLeaf = index.FrontChildIndex();
                    }
                    else if (frontPiece == null)
                    {
                        backSurface = true;
                        if (surface.FrontLeaf == index)
                            surface.FrontLeaf = index.BackChildIndex();
                        if (surface.BackLeaf == index)
                            surface.BackLeaf = index.BackChildIndex();
                    }
                    else
                    {
                        frontSurface = true;
                        backSurface = true;
                        allSurfaces.Add(new BspSurface()
                        {
                            Surface = backPiece,
                            FrontLeaf = surface.FrontLeaf == index
                                    ? index.BackChildIndex() : surface.FrontLeaf,
                            BackLeaf = surface.BackLeaf == index
                                    ? index.BackChildIndex() : surface.BackLeaf,
                        });

                        surface.Surface = frontPiece;
                        if (surface.FrontLeaf == index)
                            surface.FrontLeaf = index.FrontChildIndex();
                        if (surface.BackLeaf == index)
                            surface.BackLeaf = index.FrontChildIndex();
                    }
                }
            }
            stopwatch.Stop();

            if (!frontSurface || !backSurface)
                throw new InvalidOperationException(
                    "Partition plane selected does not partition surfaces.");

            branchPlanes[index] = partitionPlane;
            PartitionSurfaces(strategy, allSurfaces, branchPlanes, index.FrontChildIndex());
            PartitionSurfaces(strategy, allSurfaces, branchPlanes, index.BackChildIndex());
        }

        static Comparison<BspSurface> BringToFront(BigInteger index)
        {
            return (a, b) =>
            {
                var aFront = a.FrontLeaf == index;
                var bFront = b.FrontLeaf == index;

                if (aFront != bFront)
                    return aFront ? -1 : 1;

                var aBack = a.BackLeaf == index;
                var bBack = b.BackLeaf == index;

                if (aBack != bBack)
                    return aBack ? -1 : 1;

                return 0;
            };
        }

        bool AllConvex(List<BspSurface> surfaces)
        {
            foreach (var i in Enumerable.Range(0, surfaces.Count))
                foreach (var j in Enumerable.Range(i + 1, surfaces.Count - i - 1))
                    if (false == AreConvex(surfaces[i].Surface.Facet, surfaces[j].Surface.Facet))
                        return false;

            return true;
        }

        static BspSurface FindHintSurface(List<BspSurface> surfaces, int depth)
        {
            return surfaces.FirstOrDefault(surface => surface.Surface.IsHintSurface(depth));
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
