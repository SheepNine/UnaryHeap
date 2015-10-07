using System;
using System.Collections.Generic;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;

namespace MazeGenerator
{
    interface IHeightMap
    {
        Rational Height(Point2D p);
    }

    class XGradient : IHeightMap
    {
        public Rational Height(Point2D p)
        {
            return p.X;
        }
    }

    class YGradient : IHeightMap
    {
        public Rational Height(Point2D p)
        {
            return p.Y;
        }
    }

    class XYSumGradient : IHeightMap
    {
        public Rational Height(Point2D p)
        {
            return p.X + p.Y;
        }
    }

    class XYDifferenceGradient : IHeightMap
    {
        public Rational Height(Point2D p)
        {
            return p.X - p.Y;
        }
    }

    class ManhattanDistanceGradient : IHeightMap
    {
        Point2D origin;

        public ManhattanDistanceGradient(Point2D origin)
        {
            this.origin = origin;
        }

        public Rational Height(Point2D p)
        {
            return Rational.Max(
                (p.X - origin.X).AbsoluteValue,
                (p.Y - origin.Y).AbsoluteValue);
        }
    }

    class EuclideanDistanceGradient : IHeightMap
    {
        Point2D origin;

        public EuclideanDistanceGradient(Point2D origin)
        {
            this.origin = origin;
        }

        public Rational Height(Point2D p)
        {
            var distSquared = (origin.X - p.X).Squared + (origin.Y - p.Y).Squared;
            return (int)Math.Sqrt((double)distSquared);
        }
    }

    interface IEdgeWeightAssignment
    {
        Rational AssignEdgeWeight(
            Point2D l1, Point2D l2, Point2D p1, Point2D p2);
    }

    class HeightMapEdgeWeightAssignment : IEdgeWeightAssignment
    {
        IHeightMap heightMap;

        public HeightMapEdgeWeightAssignment(IHeightMap heightMap)
        {
            this.heightMap = heightMap;
        }
        
        public Rational AssignEdgeWeight(
            Point2D l1, Point2D l2, Point2D p1, Point2D p2)
        {
            var w1 = heightMap.Height(l1);
            var w2 = heightMap.Height(l2);
            var delta = (w1 - w2).AbsoluteValue;

            return delta;
        }
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
