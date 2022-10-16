using System;
using UnaryHeap.DataType;

namespace UnaryHeap.Utilities.Misc
{
    #region 1D Mapping

    /// <summary>
    /// Intermediate builder object for a one-dimensional affine mapping.
    /// </summary>
    public interface IAffineMapper1D
    {
        /// <summary>
        /// Specifies the coordinates of the points to be mapped in the destination
        /// coordinate system.
        /// </summary>
        /// <param name="dst1">The output coordinates of the first point.</param>
        /// <param name="dst2">The output coordinates of the second point.</param>
        /// <returns>A Matrix2D that will send src1 to dst1 and src2 to dst2.</returns>
        /// <exception cref="System.ArgumentNullException">dst1 or dst2 are null.</exception>
        Matrix2D Onto(Rational dst1, Rational dst2);
    }

    /// <summary>
    /// Intermediate builder object for a two-dimensional affine mapping.
    /// </summary>
    public interface IAffineMapper2D
    {
        /// <summary>
        /// Specifies the coordinates of the points to be mapped in the destination
        /// coordinate system.
        /// </summary>
        /// <param name="dst1">The output coordinates of the first point.</param>
        /// <param name="dst2">The output coordinates of the second point.</param>
        /// <param name="dst3">The output coordinates of the second point.</param>
        /// <returns>A Matrix2D that will send src1 to dst1,
        /// src2 to dst2 and src3 to dst3.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// dst1, dst2 or dst3 are null.</exception>
        Matrix3D Onto(Point2D dst1, Point2D dst2, Point2D dst3);
    }

    /// <summary>
    /// Utility class for computing linear maps from one coordinate system to another.
    /// </summary>
    public static class AffineMapping
    {
        class AffineMapper1D : IAffineMapper1D
        {
            Matrix2D sourceInverse;

            public AffineMapper1D(Rational src1, Rational src2)
            {
                try
                {
                    sourceInverse = new Matrix2D(src1, src2, 1, 1)
                        .ComputeInverse();
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException(
                        "Source points are linearly dependent; cannot invert.");
                }
            }

            public Matrix2D Onto(Rational dst1, Rational dst2)
            {
                if (null == dst1)
                    throw new ArgumentNullException("dst1");
                if (null == dst2)
                    throw new ArgumentNullException("dst2");

                var dest = new Matrix2D(dst1, dst2, 1, 1);
                return dest * sourceInverse;
            }
        }

        /// <summary>
        /// Specifies the coordinates of the points to be mapped in the source
        /// coordinate system.
        /// </summary>
        /// <param name="src1">The output coordinates of the first point.</param>
        /// <param name="src2">The output coordinates of the second point.</param>
        /// <returns>An IAffineMapper1D object that is used to specify the coordinates
        /// of the input points in the destination coordinate system..</returns>
        /// <exception cref="System.ArgumentNullException">dst1 or dst2 are null.</exception>
        /// <exception cref="System.ArgumentException">The input points are
        /// linearly dependent.</exception>
        public static IAffineMapper1D From(Rational src1, Rational src2)
        {
            if (null == src1)
                throw new ArgumentNullException("src1");
            if (null == src2)
                throw new ArgumentNullException("src2");

            return new AffineMapper1D(src1, src2);
        }


        #endregion


        #region 2D Mapping

        class AffineMapper2D : IAffineMapper2D
        {
            Matrix3D sourceInverse;

            public AffineMapper2D(Point2D src1, Point2D src2, Point2D src3)
            {
                try
                {
                    sourceInverse = new Matrix3D(
                        src1.X, src2.X, src3.X,
                        src1.Y, src2.Y, src3.Y,
                        1, 1, 1)
                        .ComputeInverse();
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException(
                        "Source points are linearly dependent; cannot invert.");
                }
            }

            public Matrix3D Onto(Point2D dst1, Point2D dst2, Point2D dst3)
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
                    1, 1, 1);

                return dest * sourceInverse;
            }
        }

        /// <summary>
        /// Specifies the coordinates of the points to be mapped in the source
        /// coordinate system.
        /// </summary>
        /// <param name="src1">The output coordinates of the first point.</param>
        /// <param name="src2">The output coordinates of the second point.</param>
        /// <param name="src3">The output coordinates of the third point.</param>
        /// <returns>An IAffineMapper2D object that is used to specify the coordinates
        /// of the input points in the destination coordinate system..</returns>
        /// <exception cref="System.ArgumentNullException">
        /// dst1, dst2 or dst3 are null.</exception>
        /// <exception cref="System.ArgumentException">The input points are
        /// linearly dependent.</exception>
        public static IAffineMapper2D From(Point2D src1, Point2D src2, Point2D src3)
        {
            if (null == src1)
                throw new ArgumentNullException("src1");
            if (null == src2)
                throw new ArgumentNullException("src2");
            if (null == src3)
                throw new ArgumentNullException("src3");

            return new AffineMapper2D(src1, src2, src3);
        }

        #endregion
    }
}
