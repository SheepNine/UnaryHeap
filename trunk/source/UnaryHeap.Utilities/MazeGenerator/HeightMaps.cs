using System;
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
}
