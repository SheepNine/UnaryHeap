using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.D2;

namespace Partitioner
{
    class BinarySpaceImplementation : BinarySpacePartitioner<Surface, Hyperplane2D>
    {
        BinarySpaceImplementation(IPartitioner partitioner)
            : base(partitioner)
        {
        }


        protected override bool AreConvex(Surface a, Surface b)
        {
            return a.IsConvexWith(b);
        }

        protected override void Split(Surface surface, Hyperplane2D splitter, out Surface frontSurface, out Surface backSurface)
        {
            surface.Split(splitter, out frontSurface, out backSurface);
        }

        public static BinarySpaceImplementation WithExhaustivePartitioner()
        {
            return new BinarySpaceImplementation(new ExhaustivePartitioner());
        }

        class ExhaustivePartitioner : IPartitioner
        {
            public Hyperplane2D SelectPartitionPlane(IEnumerable<Surface> surfacesToPartition)
            {
                var hyperplanes = surfacesToPartition.Select(s => s.Hyperplane)
                    .Distinct().ToList();

                return hyperplanes.Select(h => ComputeScore(h, surfacesToPartition))
                    .Where(s => s != null).OrderBy(s => s.Score).First().Splitter;
            }

            SplitterScore ComputeScore(
                Hyperplane2D splitter, IEnumerable<Surface> surfacesToPartition)
            {
                int splits = 0;
                int front = 0;
                int back = 0;

                foreach (var surface in surfacesToPartition)
                {
                    var start = splitter.DetermineHalfspaceOf(surface.Start);
                    var end = splitter.DetermineHalfspaceOf(surface.End);

                    if (start > 0)
                    {
                        if (end > 0)
                            front += 1;
                        else if (end < 0)
                            splits += 1;
                        else // end == 0
                            front += 1;
                    }
                    else if (start < 0)
                    {
                        if (end > 0)
                            splits += 1;
                        else if (end < 0)
                            back += 1;
                        else // end == 0
                            back += 1;
                    }
                    else // start == 0
                    {
                        if (end > 0)
                            front += 1;
                        else if (end < 0)
                            back += 1;
                        else // end == 0
                            if (surface.Hyperplane.Equals(splitter))
                            front += 1;
                        else
                            back += 1;
                    }
                }

                if (splits == 0 && (front == 0 || back == 0))
                    return null;
                else
                    return new SplitterScore(splitter, front, back, splits);
            }

            class SplitterScore
            {
                private int back;
                private int front;
                private int splits;
                private Hyperplane2D splitter;

                public SplitterScore(Hyperplane2D splitter, int front, int back, int splits)
                {
                    this.splitter = splitter;
                    this.front = front;
                    this.back = back;
                    this.splits = splits;
                }

                public override string ToString()
                {
                    return string.Format("{0} : {1} : {2}", front, splits, back);
                }

                public int Score
                {
                    get { return Math.Abs(back - front) + 10 * splits; }
                }

                public Hyperplane2D Splitter
                {
                    get { return splitter; }
                }
            }
        }
    }
}
