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

    class RandomGradient : IHeightMap
    {
        SortedDictionary<Point2D, int> memoizedValues;
        Random random;

        public RandomGradient(int seed)
        {
            memoizedValues = new SortedDictionary<Point2D, int>(new Point2DComparer());
            random = new Random(seed);
        }

        public Rational Height(Point2D p)
        {
            MemoizePointIfRequired(p);
            return memoizedValues[p];
        }

        void MemoizePointIfRequired(Point2D p)
        {
            if (false == memoizedValues.ContainsKey(p))
                memoizedValues.Add(p, random.Next());
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
}
