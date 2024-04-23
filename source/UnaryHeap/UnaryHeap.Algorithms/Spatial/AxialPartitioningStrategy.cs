using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
    {
        /// <summary>
        /// Provides a partitioning strategy which strongly favours axial splitting planes
        /// that divide the surfaces into approximately equally-sized halfspaces.
        /// </summary>
        /// <returns>The partitioner.</returns>
        public IPartitionStrategy AxialPartitionStrategy()
        {
            return new AxialPartitioner(dimension);
        }

        class AxialPartitioner : IPartitionStrategy
        {
            readonly IDimension dimension;

            public AxialPartitioner(IDimension dimension)
            {
                this.dimension = dimension;
            }

            public TPlane SelectPartitionPlane(IEnumerable<TSurface> surfacesToPartition)
            {
                var options = surfacesToPartition
                    .Select(s => dimension.GetPlane(dimension.GetFacet(s)))
                    .Distinct()
                    .ToList();

                var axialOptions = new List<TPlane>();
                var nonAxialOptions = new List<TPlane>();

                foreach (var option in options)
                    (dimension.IsAxial(option) ? axialOptions : nonAxialOptions).Add(option);

                if (axialOptions.Count > 1)
                {
                    // More than one candidate axial plane
                    // find the one that is closest to the middle of the surfaces
                    var center = dimension.FindCenterPoint(
                        dimension.CalculateBounds(surfacesToPartition));

                    axialOptions = axialOptions
                        .OrderBy(p => dimension.DeterminatePoint(center, p))
                        .ToList();
                }

                foreach (var option in axialOptions.Concat(nonAxialOptions))
                {
                    var hasFront = false;
                    var hasBack = false;
                    foreach (var surface in surfacesToPartition)
                    {
                        dimension.ClassifySurface(dimension.GetFacet(surface),
                            option, out int minDeterminant, out int maxDeterminant);

                        if (minDeterminant == -1)
                            hasBack = true;
                        if (maxDeterminant == 1)
                            hasFront = true;

                        if (hasFront && hasBack)
                            return option;
                    }
                }

                throw new InvalidOperationException(
                    "BUG HERE; should have found a partitioning plane that would work");
            }
        }
    }
}
