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
            /// Called when binary space partitioning partitions a set of surfaces.
            /// </summary>
            /// <param name="partitionPlane">The splitting plane.</param>
            /// <param name="frontSurfaces">
            /// The resulting surfaces on the front of the plane.</param>
            /// <param name="backSurfaces">
            /// The resulting surfaces on the back of the plane.</param>
            void PartitionOccurred(TPlane partitionPlane,
                List<TSurface> frontSurfaces, List<TSurface> backSurfaces);
        }

        readonly IDebug debug;
    }
}
