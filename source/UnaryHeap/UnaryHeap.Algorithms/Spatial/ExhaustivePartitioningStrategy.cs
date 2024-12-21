using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
        where TSurface : Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>.SurfaceBase
    {
        /// <summary>
        /// Provides a partitioning strategy which evaluates all available splitting planes.
        /// </summary>
        /// <param name="imbalanceWeight">How many points to deduct for imbalance between
        /// the number of surfaces on the front and back halves of a splitting plane.</param>
        /// <param name="splitWeight">How many points to deduct for each surface that is split
        /// by a splitting plane.</param>
        /// <returns>A partitioner with the given settings.</returns>
        public Func<IEnumerable<IBspSurface>, TPlane> ExhaustivePartitionStrategy(
            int imbalanceWeight, int splitWeight)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(imbalanceWeight);
            ArgumentOutOfRangeException.ThrowIfNegative(splitWeight);
            if (imbalanceWeight == 0 && splitWeight == 0)
                throw new ArgumentException("Both weights cannot be zero");

            return surfaces =>
                surfaces
                .Select(s => dimension.GetPlane(s.Surface.Facet))
                .Distinct()
                .Select(h => ComputeSplitResult(h, surfaces))
                .Where(splitResult => splitResult != null)
                .OrderBy(splitResult =>
                    splitResult.ComputeScore(imbalanceWeight, splitWeight))
                .First()
                .splitter;
        }

        SplitResult ComputeSplitResult(TPlane splitter,
            IEnumerable<IBspSurface> surfacesToPartition)
        {
            int splits = 0;
            int front = 0;
            int back = 0;

            foreach (var surface in surfacesToPartition)
            {
                dimension.ClassifySurface(surface.Surface.Facet, splitter,
                    out int minDeterminant, out int maxDeterminant);

                if (maxDeterminant > 0)
                    front += 1;
                if (minDeterminant < 0)
                    back += 1;
                if (maxDeterminant > 0 && minDeterminant < 0)
                    splits += 1;
            }

            if (splits == 0 && (front == 0 || back == 0))
                return null;
            else
                return new SplitResult(splitter, front, back, splits);
        }

        sealed class SplitResult
        {
            public int back;
            public int front;
            public int splits;
            public TPlane splitter;

            public SplitResult(TPlane splitter, int front, int back, int splits)
            {
                this.splitter = splitter;
                this.front = front;
                this.back = back;
                this.splits = splits;
            }

            public int ComputeScore(int imbalanceWeight, int splitWeight)
            {
                return Math.Abs(front - back) * imbalanceWeight + splits * splitWeight;
            }
        }
    }
}
