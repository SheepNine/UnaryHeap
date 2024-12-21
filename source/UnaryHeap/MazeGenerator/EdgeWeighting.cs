using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.DataType;

namespace MazeGenerator
{
    interface IEdgeWeightAssignment
    {
        Rational GetEdgeWeight(
            Point2D l1, Point2D l2, Point2D p1, Point2D p2);
    }

    sealed class BiggestWallEdgeWeightAssignment : IEdgeWeightAssignment
    {
        public Rational GetEdgeWeight(Point2D l1, Point2D l2, Point2D p1, Point2D p2)
        {
            return -Point2D.Quadrance(p1, p2);
        }
    }

    sealed class RandomEdgeWeightAssignment : IEdgeWeightAssignment
    {
        Random random;

        public RandomEdgeWeightAssignment(int? seed = null)
        {
            if (seed.HasValue)
                random = new Random(seed.Value);
            else
                random = new Random();
        }

        public Rational GetEdgeWeight(Point2D l1, Point2D l2, Point2D p1, Point2D p2)
        {
            return random.Next(100);
        }
    }

    sealed class RegionalEdgeWeightingAssignment : IEdgeWeightAssignment
    {
        IRegionAssignment regionAssignment;
        IEdgeWeightAssignment intraregionWeights;

        public RegionalEdgeWeightingAssignment(
            IRegionAssignment regions, IEdgeWeightAssignment weights)
        {
            this.regionAssignment = regions;
            this.intraregionWeights = weights;
        }

        public Rational GetEdgeWeight(Point2D l1, Point2D l2, Point2D p1, Point2D p2)
        {
            if (regionAssignment.GetRegion(l1) != regionAssignment.GetRegion(l2))
                return 100000;
            else
                return intraregionWeights.GetEdgeWeight(l1, l2, p1, p2);
        }
    }

    interface IRegionAssignment
    {
        int GetRegion(Point2D p);
    }

    sealed class VoronoiCellRegionAssignment : IRegionAssignment
    {
        Point2D[] sites;

        public VoronoiCellRegionAssignment(IEnumerable<Point2D> sites)
        {
            this.sites = sites.ToArray();
        }

        public VoronoiCellRegionAssignment(params Point2D[] sites) :
            this((IEnumerable<Point2D>)sites)
        {
        }

        public int GetRegion(Point2D p)
        {
            var bestValue = Point2D.Quadrance(sites[0], p);
            var bestIndex = 0;

            foreach (var i in Enumerable.Range(1, sites.Length - 1))
            {
                var value = Point2D.Quadrance(sites[i], p);

                if (value < bestValue)
                {
                    bestValue = value;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }
    }
}
