using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnaryHeap.Utilities.D2;

namespace Partitioner
{
    class BspNode
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

        public IEnumerable<Surface> NonPassageWalls
        {
            get { return surfaces.Where(surface => false == surface.IsPassage); }
        }

        public string RoomName
        {
            get
            {
                return NonPassageWalls
                    .Select(surface => surface.RoomName)
                    .Distinct()
                    .SingleOrDefault();
            }
        }
    }
}