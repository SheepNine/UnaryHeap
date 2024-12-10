using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
        where TSurface : Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>.SurfaceBase
    {

        /// <summary>
        /// Event raised during outside culling each time new leaves are marked as interior.
        /// </summary>
        public event EventHandler<InsideFilledEventArgs> InsideFilled;

        /// <summary>
        /// Raises the InsideFilled event.
        /// </summary>
        /// <param name="interiorPoint">The origin point of the fill.</param>
        /// <param name="interiorLeaves">The current list of interior leaves.</param>
        /// <param name="leafCount">The number of leaves in the tree.</param>
        protected void OnInsideFilled(TPoint interiorPoint, HashSet<BigInteger> interiorLeaves,
            int leafCount)
        {
            InsideFilled?.Invoke(this, new InsideFilledEventArgs(
                interiorPoint, interiorLeaves, leafCount));
        }

        /// <summary>
        /// Contains debugging information about the progress of an outside cull operation.
        /// </summary>
        public class InsideFilledEventArgs: EventArgs
        {
            /// <summary>
            /// The origin point of the fill.
            /// </summary>
            public TPoint InteriorPoint { get; private set; }

            /// <summary>
            /// The current list of interior leaves.
            /// </summary>
            public ImmutableHashSet<BigInteger> InteriorLeaves { get; private set; }

            /// <summary>
            /// The number of leaves in the tree.
            /// </summary>
            public int LeafCount { get; private set; }

            /// <summary>
            /// Initializes a new instance of the InsideFilledEventArgs class.
            /// </summary>
            /// <param name="interiorPoint">The origin point of the fill.</param>
            /// <param name="interiorLeaves">The current list of interior leaves.</param>
            /// <param name="leafCount">The number of leaves in the tree.</param>
            public InsideFilledEventArgs(TPoint interiorPoint, HashSet<BigInteger> interiorLeaves,
                int leafCount)
            {
                InteriorPoint = interiorPoint;
                InteriorLeaves = interiorLeaves.ToImmutableHashSet();
                LeafCount = leafCount; 
            }
        }

        /// <summary>
        /// Event raised during binary space paritioning each time a node needs to be parittioned.
        /// </summary>
        public event EventHandler<SplittingPlaneChosenEventArgs> SplittingPlaneChosen;

        /// <summary>
        /// Raises the SplittingPlaneChosen event.
        /// </summary>
        /// <param name="elapsedMs">The amount of time it took to choose
        /// a partitioning plane.</param>
        /// <param name="surfaceCount">The number of surfaces that
        /// required partitioning.</param>
        /// <param name="depth">The depth of the current node being
        /// considered in the BSP tree.</param>
        /// <param name="chosenPlane">The plane chosen to partition the surfaces.</param>
        protected void OnSplittingPlaneChosen(long elapsedMs, int surfaceCount,
                int depth, TPlane chosenPlane)
        {
            SplittingPlaneChosen?.Invoke(this, new SplittingPlaneChosenEventArgs(
                elapsedMs, surfaceCount, depth, chosenPlane));
        }

        /// <summary>
        /// Contains diagnostic information about the performance of BSP partitioning selection.
        /// </summary>
        public class SplittingPlaneChosenEventArgs : EventArgs
        {
            /// <summary>
            /// The amount of time it took to choose a partitioning plane.
            /// </summary>
            public long ElapsedMs { get; private set; }

            /// <summary>
            /// The number of surfaces that required partitioning.
            /// </summary>
            public int SurfaceCount { get; private set; }

            /// <summary>
            /// The depth of the current node being considered in the BSP tree.
            /// </summary>
            public int Depth { get; private set; }

            /// <summary>
            /// The plane chosen to partition the surfaces.
            /// </summary>
            public TPlane ChosenPlane { get; private set; }

            /// <summary>
            /// Initializes a new instance of the SplittingPlaneChosenEventArgs class. 
            /// </summary>
            /// <param name="elapsedMs">The amount of time it took to choose
            /// a partitioning plane.</param>
            /// <param name="surfaceCount">The number of surfaces that
            /// required partitioning.</param>
            /// <param name="depth">The depth of the current node being
            /// considered in the BSP tree.</param>
            /// <param name="chosenPlane">The plane chosen to partition the surfaces.</param>
            public SplittingPlaneChosenEventArgs(long elapsedMs, int surfaceCount,
                int depth, TPlane chosenPlane)
            {
                ElapsedMs = elapsedMs;
                SurfaceCount = surfaceCount;
                Depth = depth;
                ChosenPlane = chosenPlane;
            }
        }
    }
}
