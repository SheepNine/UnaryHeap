﻿using System;
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
        SortedDictionary<Point2D, int> memoizedValues = new SortedDictionary<Point2D, int>(new Point2DComparer());
        Random random;

        public RandomGradient(int seed)
        {
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
}
