using System;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;

namespace MazeGenerator
{
    class HeightMapEdgeWeightAssignment : IEdgeWeightAssignment
    {
        IHeightMap heightMap;

        public HeightMapEdgeWeightAssignment(IHeightMap heightMap)
        {
            this.heightMap = heightMap;
        }

        public Rational GetEdgeWeight(
            Point2D l1, Point2D l2, Point2D p1, Point2D p2)
        {
            var w1 = heightMap.Height(l1);
            var w2 = heightMap.Height(l2);
            var delta = (w1 - w2).AbsoluteValue;

            return delta;
        }
    }

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
            var distSquared = Point2D.Quadrance(origin, p);
            return (int)Math.Sqrt((double)distSquared);
        }
    }
}
