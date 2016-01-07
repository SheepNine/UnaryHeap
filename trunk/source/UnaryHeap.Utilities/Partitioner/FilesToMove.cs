using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.D2;

namespace Partitioner
{
    abstract class BinarySpacePartitioner
    {
        public BspNode ConstructBspTree(IEnumerable<Surface> inputSurfaces)
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

        bool AllConvex(List<Surface> surfaces)
        {
            foreach (var i in Enumerable.Range(0, surfaces.Count))
                foreach (var j in Enumerable.Range(i + 1, surfaces.Count - i - 1))
                    if (false == AreConvex(surfaces[i], surfaces[j]))
                        return false;

            return true;
        }

        void Partition(List<Surface> surfaces, Hyperplane2D splitter,
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

        protected abstract bool AreConvex(Surface a, Surface b);

        protected abstract void Split(Surface surface, Hyperplane2D splitter, out Surface frontSurface, out Surface backSurface);

        protected abstract Hyperplane2D GetPlane(Surface s);

        protected abstract Hyperplane2D ChooseSplitter(List<Surface> surfacesToPartition);
    }
}
