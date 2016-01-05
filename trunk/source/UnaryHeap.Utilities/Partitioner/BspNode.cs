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
        public Hyperplane2D splitter;
        public BspNode frontChild;
        public BspNode backChild;
        public List<Surface> surfaces;

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
    }
}