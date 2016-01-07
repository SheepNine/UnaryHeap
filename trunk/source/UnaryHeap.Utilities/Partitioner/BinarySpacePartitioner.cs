using System;
using System.Collections.Generic;
using System.Linq;

namespace Partitioner
{
    public abstract class BinarySpacePartitioner<TSurface, TPlane>
        where TPlane : class
    {
        IPartitioner partitioner;
        public BinarySpacePartitioner(IPartitioner partitioner)
        {
            this.partitioner = partitioner;
        }

        public IBspNode ConstructBspTree(IEnumerable<TSurface> inputSurfaces)
        {
            if (null == inputSurfaces)
                throw new ArgumentNullException("inputSurfaces");

            var surfaces = inputSurfaces.ToList();

            if (0 == surfaces.Count)
                throw new ArgumentException("No surfaces to partition.");

            return ConstructBspNode(surfaces);
        }

        BspNode ConstructBspNode(List<TSurface> surfaces)
        {
            if (AllConvex(surfaces))
                return BspNode.LeafNode(surfaces);

            var partitionPlane = partitioner.SelectPartitionPlane(surfaces);

            if (null == partitionPlane)
                throw new ApplicationException("Failed to select partition plane.");

            List<TSurface> frontSurfaces, backSurfaces;
            Partition(surfaces, partitionPlane, out frontSurfaces, out backSurfaces);

            if (0 == frontSurfaces.Count || 0 == backSurfaces.Count)
                throw new ApplicationException(
                    "Partition plane selected does not partition surfaces.");

            var frontChild = ConstructBspNode(frontSurfaces);
            var backChild = ConstructBspNode(backSurfaces);
            return BspNode.BranchNode(partitionPlane, frontChild, backChild);
        }

        bool AllConvex(List<TSurface> surfaces)
        {
            foreach (var i in Enumerable.Range(0, surfaces.Count))
                foreach (var j in Enumerable.Range(i + 1, surfaces.Count - i - 1))
                    if (false == AreConvex(surfaces[i], surfaces[j]))
                        return false;

            return true;
        }

        void Partition(List<TSurface> surfaces, TPlane partitionPlane,
            out List<TSurface> frontSurfaces, out List<TSurface> backSurfaces)
        {
            frontSurfaces = new List<TSurface>();
            backSurfaces = new List<TSurface>();

            foreach (var surface in surfaces)
            {
                TSurface frontSurface, backSurface;
                Split(surface, partitionPlane, out frontSurface, out backSurface);

                if (null != frontSurface)
                    frontSurfaces.Add(frontSurface);
                if (null != backSurface)
                    backSurfaces.Add(backSurface);
            }
        }

        protected abstract bool AreConvex(TSurface a, TSurface b);

        protected abstract void Split(TSurface surface, TPlane splitter,
            out TSurface frontSurface, out TSurface backSurface);

        public interface IPartitioner
        {
            TPlane SelectPartitionPlane(IEnumerable<TSurface> surfacesToPartition);
        }

        public interface IBspNode
        {
            bool IsLeaf { get; }

            TPlane PartitionPlane { get; }

            IBspNode FrontChild { get; }

            IBspNode BackChild { get; }

            IEnumerable<TSurface> Surfaces { get; }

            int NodeCount { get; }

            void PreOrder(Action<IBspNode> callback);

            void InOrder(Action<IBspNode> callback);

            void PostOrder(Action<IBspNode> callback);
        }


        class BspNode : IBspNode
        {
            TPlane partitionPlane;
            BspNode frontChild;
            BspNode backChild;
            List<TSurface> surfaces;

            private BspNode() { }

            public static BspNode LeafNode(IEnumerable<TSurface> surfaces)
            {
                return new BspNode()
                {
                    partitionPlane = null,
                    frontChild = null,
                    backChild = null,
                    surfaces = surfaces.ToList()
                };
            }

            public static BspNode BranchNode(TPlane splitter,
                BspNode frontChild, BspNode backChild)
            {
                return new BspNode()
                {
                    partitionPlane = splitter,
                    frontChild = frontChild,
                    backChild = backChild,
                    surfaces = null
                };
            }

            public bool IsLeaf
            {
                get { return surfaces != null; }
            }

            public TPlane PartitionPlane
            {
                get
                {
                    if (IsLeaf)
                        throw new InvalidOperationException(
                            "Leaf nodes have no partition plane.");

                    return partitionPlane;
                }
            }

            public IBspNode FrontChild
            {
                get
                {
                    if (IsLeaf)
                        throw new InvalidOperationException("Leaf nodes have no children.");

                    return frontChild;
                }
            }

            public IBspNode BackChild
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

            public void PreOrder(Action<IBspNode> callback)
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

            public void InOrder(Action<IBspNode> callback)
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

            public void PostOrder(Action<IBspNode> callback)
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
    }
}
