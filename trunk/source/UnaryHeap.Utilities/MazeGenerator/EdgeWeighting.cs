using System;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;

namespace MazeGenerator
{
    interface IEdgeWeightAssignment
    {
        Rational AssignEdgeWeight(
            Point2D l1, Point2D l2, Point2D p1, Point2D p2);
    }

    class BiggestWallEdgeWeightAssignment : IEdgeWeightAssignment
    {
        public Rational AssignEdgeWeight(Point2D l1, Point2D l2, Point2D p1, Point2D p2)
        {
            return -Point2D.Quadrance(p1, p2);
        }
    }

    class RandomEdgeWeightAssignment : IEdgeWeightAssignment
    {
        Random random;

        public RandomEdgeWeightAssignment(int? seed = null)
        {
            if (seed.HasValue)
                random = new Random(seed.Value);
            else
                random = new Random();
        }

        public Rational AssignEdgeWeight(Point2D l1, Point2D l2, Point2D p1, Point2D p2)
        {
            return random.Next(100);
        }
    }
}
