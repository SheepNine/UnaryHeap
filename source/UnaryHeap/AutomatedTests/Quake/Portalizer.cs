using System;
using System.Collections.Generic;
using UnaryHeap.Algorithms;
using UnaryHeap.DataType;

namespace Quake
{
    abstract class Portalizer<TSurface, TPlane, TFacet, TBounds>
    {
        public void Portalize(IBspNode<TSurface, TPlane> root)
        {
            var bounds = CalculateBoundingBox(root);

        }

        private TBounds CalculateBoundingBox(IBspNode<TSurface, TPlane> root)
        {
            if (root.IsLeaf)
            {
                return UnionBounds(
                    CalculateBoundingBox(root.FrontChild),
                    CalculateBoundingBox(root.BackChild)
                );
            } else
            {
                return CalculateBounds(root.Surfaces);
            }
        }

        protected abstract TBounds CalculateBounds(IEnumerable<TSurface> surfaces);
        protected abstract TBounds UnionBounds(TBounds a, TBounds b);

        class Portal
        {
            public TFacet Facet { get; private set; }
            public IBspNode<TSurface, TFacet> Front { get; private set; }
            public IBspNode<TSurface, TFacet> Back { get; private set; }

            public Portal(TFacet facet, IBspNode<TSurface, TFacet> front,
                IBspNode<TSurface, TFacet> back)
            {
                Facet = facet;
                Front = front;
                Back = back;
            }

            public void Split(Func<TFacet, Tuple<TFacet, TFacet>> splitFunc,
                out Portal front, out Portal back)
            {
                var splitFacets = splitFunc(Facet);
                if (splitFacets.Item1 == null)
                    front = null;
                else
                    front = new Portal(splitFacets.Item1, Front, Back);

                if (splitFacets.Item2 == null)
                    back = null;
                else
                    back = new Portal(splitFacets.Item2, Front, Back);
            }
        }
    }

    class QuakePortalizer: Portalizer<QuakeSurface, Hyperplane3D, Facet3D, Orthotope3D>
    {
        protected override Orthotope3D CalculateBounds(IEnumerable<QuakeSurface> surfaces)
        {
            return null;
        }

        protected override Orthotope3D UnionBounds(Orthotope3D a, Orthotope3D b)
        {
            return null;
        }
    }
}
