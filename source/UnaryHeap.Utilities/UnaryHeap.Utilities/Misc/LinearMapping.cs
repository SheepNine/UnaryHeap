using System;
using UnaryHeap.DataType;

namespace UnaryHeap.Utilities.Misc
{
    #region 2D Mapping

    /// <summary>
    /// Intermediate builder object for a two-dimensional linear mapping.
    /// </summary>
    public interface ILinearMapper2D
    {
        /// <summary>
        /// Specifies the coordinates of the points to be mapped in the destination
        /// coordinate system.
        /// </summary>
        /// <param name="dst1">The output coordinates of the first point.</param>
        /// <param name="dst2">The output coordinates of the second point.</param>
        /// <returns>A Matrix2D that will send src1 to dst1 and src2 to dst2.</returns>
        /// <exception cref="System.ArgumentNullException">dst1 or dst2 are null.</exception>
        Matrix2D Onto(Point2D dst1, Point2D dst2);
    }

    /// <summary>
    /// Intermediate builder object for a three-dimensional linear mapping.
    /// </summary>
    public interface ILinearMapper3D
    {
        /// <summary>
        /// Specifies the coordinates of the points to be mapped in the destination
        /// coordinate system.
        /// </summary>
        /// <param name="dst1">The output coordinates of the first point.</param>
        /// <param name="dst2">The output coordinates of the second point.</param>
        /// <param name="dst3">The output coordinates of the third point.</param>
        /// <returns>A Matrix2D that will send src1 to dst1,
        /// src2 to dst2 and src3 to dst3.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// dst1, dst2, or dst3 are null.</exception>
        Matrix3D Onto(Point3D dst1, Point3D dst2, Point3D dst3);
    }

    /// <summary>
    /// Utility class for computing linear maps from one coordinate system to another.
    /// </summary>
    public static class LinearMapping
    {
        class LinearMapper2D : ILinearMapper2D
        {
            Matrix2D sourceInverse;

            public LinearMapper2D(Point2D src1, Point2D src2)
            {
                try
                {
                    sourceInverse = new Matrix2D(src1.X, src2.X, src1.Y, src2.Y)
                        .ComputeInverse();
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException(
                        "Source points are linearly dependent; cannot invert.");
                }
            }

            public Matrix2D Onto(Point2D dst1, Point2D dst2)
            {
                if (null == dst1)
                    throw new ArgumentNullException("dst1");
                if (null == dst2)
                    throw new ArgumentNullException("dst2");

                var dest = new Matrix2D(dst1.X, dst2.X, dst1.Y, dst2.Y);
                return dest * sourceInverse;
            }
        }

        /// <summary>
        /// Specifies the coordinates of the points to be mapped in the source
        /// coordinate system.
        /// </summary>
        /// <param name="src1">The output coordinates of the first point.</param>
        /// <param name="src2">The output coordinates of the second point.</param>
        /// <returns>An ILinearMapper2D object that is used to specify the coordinates
        /// of the input points in the destination coordinate system..</returns>
        /// <exception cref="System.ArgumentNullException">dst1 or dst2 are null.</exception>
        /// <exception cref="System.ArgumentException">The input points are
        /// linearly dependent.</exception>
        public static ILinearMapper2D From(Point2D src1, Point2D src2)
        {
            if (null == src1)
                throw new ArgumentNullException("src1");
            if (null == src2)
                throw new ArgumentNullException("src2");

            return new LinearMapper2D(src1, src2);
        }

        #endregion


        #region 3D Mapping

        class LinearMapper3D : ILinearMapper3D
        {
            Matrix3D sourceInverse;

            public LinearMapper3D(Point3D src1, Point3D src2, Point3D src3)
            {
                try
                {
                    sourceInverse = new Matrix3D(
                        src1.X, src2.X, src3.X,
                        src1.Y, src2.Y, src3.Y,
                        src1.Z, src2.Z, src3.Z)
                        .ComputeInverse();
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException(
                        "Source points are linearly dependent; cannot invert.");
                }
            }

            public Matrix3D Onto(Point3D dst1, Point3D dst2, Point3D dst3)
            {
                if (null == dst1)
                    throw new ArgumentNullException("dst1");
                if (null == dst2)
                    throw new ArgumentNullException("dst2");
                if (null == dst3)
                    throw new ArgumentNullException("dst3");

                var dest = new Matrix3D(
                    dst1.X, dst2.X, dst3.X,
                    dst1.Y, dst2.Y, dst3.Y,
                    dst1.Z, dst2.Z, dst3.Z);

                return dest * sourceInverse;
            }
        }

        /// <summary>
        /// Specifies the coordinates of the points to be mapped in the source
        /// coordinate system.
        /// </summary>
        /// <param name="src1">The output coordinates of the first point.</param>
        /// <param name="src2">The output coordinates of the second point.</param>
        /// <param name="src3">The output coordinates of the second point.</param>
        /// <returns>An ILinearMapper3D object that is used to specify the coordinates
        /// of the input points in the destination coordinate system..</returns>
        /// <exception cref="System.ArgumentNullException">
        /// dst1, dst2 or dst3 are null.</exception>
        /// <exception cref="System.ArgumentException">The input points are
        /// linearly dependent.</exception>
        public static ILinearMapper3D From(Point3D src1, Point3D src2, Point3D src3)
        {
            if (null == src1)
                throw new ArgumentNullException("src1");
            if (null == src2)
                throw new ArgumentNullException("src2");
            if (null == src3)
                throw new ArgumentNullException("src3");

            return new LinearMapper3D(src1, src2, src3);
        }

        #endregion
    }
}
