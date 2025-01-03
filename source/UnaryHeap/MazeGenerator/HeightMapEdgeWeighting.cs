﻿using System;
using UnaryHeap.DataType;

namespace MazeGenerator
{
    sealed class HeightMapEdgeWeightAssignment : IEdgeWeightAssignment
    {
        IHeightMap heightMap;
        bool negateWeights;

        public HeightMapEdgeWeightAssignment(
            IHeightMap heightMap, bool negateWeights)
        {
            this.heightMap = heightMap;
            this.negateWeights = negateWeights;
        }

        public Rational GetEdgeWeight(
            Point2D l1, Point2D l2, Point2D p1, Point2D p2)
        {
            var w1 = heightMap.Height(l1);
            var w2 = heightMap.Height(l2);
            var delta = (w1 - w2).AbsoluteValue;

            if (negateWeights)
                return -delta;
            else
                return delta;
        }
    }

    interface IHeightMap
    {
        Rational Height(Point2D p);
    }

    sealed class XGradient : IHeightMap
    {
        public Rational Height(Point2D p)
        {
            return p.X;
        }
    }

    sealed class YGradient : IHeightMap
    {
        public Rational Height(Point2D p)
        {
            return p.Y;
        }
    }

    sealed class XYSumGradient : IHeightMap
    {
        public Rational Height(Point2D p)
        {
            return p.X + p.Y;
        }
    }

    sealed class XYDifferenceGradient : IHeightMap
    {
        public Rational Height(Point2D p)
        {
            return p.X - p.Y;
        }
    }

    sealed class ManhattanDistanceGradient : IHeightMap
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

    sealed class EuclideanDistanceGradient : IHeightMap
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
