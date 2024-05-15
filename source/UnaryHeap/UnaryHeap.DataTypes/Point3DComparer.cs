using System.Collections.Generic;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Defines a class that can be used to order Point3D objects.
    /// </summary>
    public class Point3DComparer : IComparer<Point3D>
    {
        /// <summary>
        /// The singleton instance of Point3DComparer.
        /// </summary>
        public static readonly IComparer<Point3D> Instance = new Point3DComparer();

        private Point3DComparer() { }

        /// <summary>
        /// Compares two Point2D objects and returns a value indicating whether one is less than,
        /// equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A negative integer, if x is less than y. Zero, if x equals y.
        /// A positive integer, if x is greater than y.</returns>
        public int Compare(Point3D x, Point3D y)
        {
            if (null == x && null == y)
                return 0;
            if (null == x)
                return -1;
            if (null == y)
                return 1;

            var result = x.X.CompareTo(y.X);
            if (result == 0)
            {
                result = x.Y.CompareTo(y.Y);
                if (result == 0)
                {
                    result = x.Z.CompareTo(y.Z);
                }
            }
            return result;
        }
    }
}
