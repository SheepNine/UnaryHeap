using System;
using System.Drawing;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.D3;
using UnaryHeap.Utilities.Misc;

namespace GraphPaper
{
    class ModelViewTransform
    {
        public event EventHandler TransformChanged;
        protected void OnTransformChanged()
        {
            if (null != TransformChanged)
                TransformChanged(this, EventArgs.Empty);
        }

        Point2D modelCenter;
        Rational modelHeight;
        Rectangle viewExtents;

        Matrix3D modelToView;
        Matrix3D viewToModel;

        public ModelViewTransform(Rectangle viewExtents, Orthotope2D modelRange)
        {
            this.modelCenter = modelRange.Center;
            this.viewExtents = viewExtents;
            this.modelHeight = FitRange(modelRange, new Rational(11, 10));

            InitMatrices();
        }

        public void UpdateModelCenter(Point2D newCenter)
        {
            this.modelCenter = newCenter;

            InitMatrices();
        }

        public void UpdateViewport(Rectangle newViewExtents)
        {
            modelHeight = modelHeight * newViewExtents.Height / viewExtents.Height;
            viewExtents = newViewExtents;

            InitMatrices();
        }

        public void UpdateModelRange(Orthotope2D newExtents, Rational padding)
        {
            this.modelCenter = newExtents.Center;
            this.modelHeight = FitRange(newExtents, padding);

            InitMatrices();
        }

        public void ZoomIn()
        {
            this.modelHeight *= new Rational(2, 3);
            InitMatrices();
        }

        public void ZoomOut()
        {
            this.modelHeight *= new Rational(3, 2);
            InitMatrices();
        }

        Rational FitRange(Orthotope2D modelRange, Rational padding)
        {
            var y = modelRange.Y.Size;
            var x = modelRange.X.Size * viewExtents.Height / viewExtents.Width;

            return Rational.Max(x, y) * padding;
        }

        void InitMatrices()
        {
            var modelWidth = modelHeight * viewExtents.Width / viewExtents.Height;


            modelToView = AffineMapping.From(
                new Point2D(modelCenter.X - modelWidth / 2, modelCenter.Y + modelHeight / 2),
                new Point2D(modelCenter.X + modelWidth / 2, modelCenter.Y + modelHeight / 2),
                new Point2D(modelCenter.X - modelWidth / 2, modelCenter.Y - modelHeight / 2))
                .Onto(
                new Point2D(viewExtents.Left, viewExtents.Top),
                new Point2D(viewExtents.Right, viewExtents.Top),
                new Point2D(viewExtents.Left, viewExtents.Bottom)
                );

            viewToModel = modelToView.ComputeInverse();

            OnTransformChanged();
        }

        public Orthotope2D ModelFromView(Rectangle rect)
        {
            return Orthotope2D.FromPoints(new[] {
                ModelFromView(new Point2D(rect.Left, rect.Top)),
                ModelFromView(new Point2D(rect.Right, rect.Bottom))
            });
        }

        public Point2D ViewFromModel(Point2D modelCoords)
        {
            return Transform(modelCoords.X, modelCoords.Y, modelToView);
        }

        public Point2D ModelFromView(Point2D viewCoords)
        {
            return Transform(viewCoords.X, viewCoords.Y, viewToModel);
        }

        public Point2D ModelFromView(Point viewCoords)
        {
            return Transform(viewCoords.X, viewCoords.Y, viewToModel);
        }

        static Point2D Transform(Rational x, Rational y, Matrix3D m)
        {
            var affineCoords = new Point3D(x, y, 1);
            var affineResult = m * affineCoords;
            return new Point2D(
                affineResult.X / affineResult.Z,
                affineResult.Y / affineResult.Z);
        }
    }
}
