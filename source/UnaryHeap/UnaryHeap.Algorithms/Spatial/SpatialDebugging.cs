using System;
using System.Collections.Generic;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
    {
        /// <summary>
        /// Provides hooks for geometric operations in order to analyze results.
        /// </summary>
        public interface IDebug
        {
            /// <summary>
            /// Called when binary space partitioning had to choose a splitting plan for a
            /// branch node.
            /// </summary>
            /// <param name="elapsedMilliseconds">How long the selection took.</param>
            /// <param name="surfaces">The input set of surfaces requiring partitioning.</param>
            /// <param name="depth">The depth of the node in the tree.</param>
            /// <param name="partitionPlane">The plane chosen</param>
            void SplittingPlaneChosen(long elapsedMilliseconds, List<TSurface> surfaces,
                int depth, TPlane partitionPlane);

            /// <summary>
            /// Called when binary space partitioning partitions a set of surfaces.
            /// </summary>
            /// <param name="elapsedTimeMs">How long the partitioning took.</param>
            /// <param name="surfacesToPartition">
            /// The input set of surfaces requiring partitioning.</param>
            /// <param name="partitionPlane">The splitting plane.</param>
            /// <param name="frontSurfaces">
            /// The resulting surfaces on the front of the plane.</param>
            /// <param name="depth">The depth of the node in the tree.</param>
            /// <param name="backSurfaces">
            /// The resulting surfaces on the back of the plane.</param>
            void PartitionOccurred(long elapsedTimeMs, List<TSurface> surfacesToPartition,
                int depth, TPlane partitionPlane,
                List<TSurface> frontSurfaces, List<TSurface> backSurfaces);
        }

        readonly IDebug debug;
    }
}
