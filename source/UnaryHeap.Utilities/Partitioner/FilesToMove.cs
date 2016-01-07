using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.D2;

namespace Partitioner
{
    static class BinarySpacePartitioner
    {
        public static BspNode ConstructBspTree(IEnumerable<Surface> inputSurfaces)
        {
            var surfaces = inputSurfaces.ToList();
            if (0 == surfaces.Count)
                throw new ArgumentException("No surfaces in input surfaces");

            if (AllConvex(surfaces))
                return BspNode.LeafNode(surfaces);

            var splitter = ChooseSplitter(surfaces);
            List<Surface> frontSurfaces, backSurfaces;
            Partition(surfaces, splitter, out frontSurfaces, out backSurfaces);

            var frontChild = ConstructBspTree(frontSurfaces);
            var backChild = ConstructBspTree(backSurfaces);
            return BspNode.BranchNode(splitter, frontChild, backChild);
        }

        static bool AllConvex(List<Surface> surfaces)
        {
            foreach (var i in Enumerable.Range(0, surfaces.Count))
                foreach (var j in Enumerable.Range(i + 1, surfaces.Count - i - 1))
                    if (false == AreConvex(surfaces[i], surfaces[j]))
                        return false;

            return true;
        }

        static void Partition(List<Surface> surfaces, Hyperplane2D splitter,
            out List<Surface> frontSurfaces, out List<Surface> backSurfaces)
        {
            frontSurfaces = new List<Surface>();
            backSurfaces = new List<Surface>();

            foreach (var surface in surfaces)
            {
                Surface frontSurface, backSurface;
                Split(surface, splitter, out frontSurface, out backSurface);

                if (null != frontSurface)
                    frontSurfaces.Add(frontSurface);
                if (null != backSurface)
                    backSurfaces.Add(backSurface);
            }
        }        

        public class BspNode
        {
            Hyperplane2D splitter;
            BspNode frontChild;
            BspNode backChild;
            List<Surface> surfaces;

            private BspNode() { }

            public static BspNode LeafNode(IEnumerable<Surface> surfaces)
            {
                return new BspNode()
                {
                    splitter = null,
                    frontChild = null,
                    backChild = null,
                    surfaces = surfaces.ToList()
                };
            }

            public static BspNode BranchNode(Hyperplane2D splitter,
                BspNode frontChild, BspNode backChild)
            {
                return new BspNode()
                {
                    splitter = splitter,
                    frontChild = frontChild,
                    backChild = backChild,
                    surfaces = null
                };
            }

            public bool IsLeaf
            {
                get { return surfaces != null; }
            }

            public Hyperplane2D Splitter
            {
                get
                {
                    if (IsLeaf)
                        throw new InvalidOperationException("Leaf nodes have no splitter.");

                    return splitter;
                }
            }

            public BspNode FrontChild
            {
                get
                {
                    if (IsLeaf)
                        throw new InvalidOperationException("Leaf nodes have no children.");

                    return frontChild;
                }
            }

            public BspNode BackChild
            {
                get
                {
                    if (IsLeaf)
                        throw new InvalidOperationException("Leaf nodes have no children.");

                    return backChild;
                }
            }

            public IEnumerable<Surface> Surfaces
            {
                get
                {
                    if (false == IsLeaf)
                        throw new InvalidOperationException("Branch nodes have no surfaces.");

                    return surfaces;
                }
            }

            public int NodeCount
            {
                get
                {
                    if (IsLeaf)
                        return 1;
                    else
                        return 1 + frontChild.NodeCount + backChild.NodeCount;
                }
            }

            public void PreOrder(Action<BspNode> callback)
            {
                if (IsLeaf)
                {
                    callback(this);
                }
                else
                {
                    callback(this);
                    frontChild.PreOrder(callback);
                    backChild.PreOrder(callback);
                }
            }

            public void InOrder(Action<BspNode> callback)
            {
                if (IsLeaf)
                {
                    callback(this);
                }
                else
                {
                    frontChild.InOrder(callback);
                    callback(this);
                    backChild.InOrder(callback);
                }
            }

            public void PostOrder(Action<BspNode> callback)
            {
                if (IsLeaf)
                {
                    callback(this);
                }
                else
                {
                    frontChild.PostOrder(callback);
                    backChild.PostOrder(callback);
                    callback(this);
                }
            }
        }


        // ----------------------------------------------------------------------------------------

        static bool AreConvex(Surface a, Surface b)
        {
            return a.IsConvexWith(b);
        }

        static void Split(Surface surface, Hyperplane2D splitter, out Surface frontSurface, out Surface backSurface)
        {
            surface.Split(splitter, out frontSurface, out backSurface);
        }

        static Hyperplane2D GetPlane(Surface s)
        {
            return s.Hyperplane;
        }
        static Hyperplane2D ChooseSplitter(List<Surface> surfacesToPartition)
        {
            var hyperplanes = surfacesToPartition.Select(s => GetPlane(s))
                .Distinct().ToList();

            return hyperplanes.Select(h => ComputeScore(h, surfacesToPartition))
                .Where(s => s != null).OrderBy(s => s.Score).First().Splitter;
        }

        static SplitterScore ComputeScore(
            Hyperplane2D splitter, List<Surface> surfacesToPartition)
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
                        if (GetPlane(surface).Equals(splitter))
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
