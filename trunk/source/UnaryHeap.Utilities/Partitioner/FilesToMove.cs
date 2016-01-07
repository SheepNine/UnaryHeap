using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.D2;

namespace Partitioner
{
    abstract class BinarySpacePartitioner<TSurface>
    {
        public BspNode ConstructBspTree(IEnumerable<TSurface> inputSurfaces)
        {
            var surfaces = inputSurfaces.ToList();
            if (0 == surfaces.Count)
                throw new ArgumentException("No surfaces in input surfaces");

            if (AllConvex(surfaces))
                return BspNode.LeafNode(surfaces);

            var splitter = ChooseSplitter(surfaces);
            List<TSurface> frontSurfaces, backSurfaces;
            Partition(surfaces, splitter, out frontSurfaces, out backSurfaces);

            var frontChild = ConstructBspTree(frontSurfaces);
            var backChild = ConstructBspTree(backSurfaces);
            return BspNode.BranchNode(splitter, frontChild, backChild);
        }

        bool AllConvex(List<TSurface> surfaces)
        {
            foreach (var i in Enumerable.Range(0, surfaces.Count))
                foreach (var j in Enumerable.Range(i + 1, surfaces.Count - i - 1))
                    if (false == AreConvex(surfaces[i], surfaces[j]))
                        return false;

            return true;
        }

        void Partition(List<TSurface> surfaces, Hyperplane2D splitter,
            out List<TSurface> frontSurfaces, out List<TSurface> backSurfaces)
        {
            frontSurfaces = new List<TSurface>();
            backSurfaces = new List<TSurface>();

            foreach (var surface in surfaces)
            {
                TSurface frontSurface, backSurface;
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
            List<TSurface> surfaces;

            private BspNode() { }

            public static BspNode LeafNode(IEnumerable<TSurface> surfaces)
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

            public IEnumerable<TSurface> Surfaces
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

        protected abstract bool AreConvex(TSurface a, TSurface b);

        protected abstract void Split(TSurface surface, Hyperplane2D splitter, out TSurface frontSurface, out TSurface backSurface);

        protected abstract Hyperplane2D GetPlane(TSurface s);

        protected abstract Hyperplane2D ChooseSplitter(List<TSurface> surfacesToPartition);
    }
}
