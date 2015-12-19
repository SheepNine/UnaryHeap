using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.D3;
using UnaryHeap.Utilities.Misc;
using UnaryHeap.Utilities.UI;

namespace GraphPaper
{
    interface IViewModel
    {
        void HookUp(WysiwygPanel editorPanel);
    }

    class ViewModel : IDisposable, IViewModel
    {
        GraphEditorStateMachine stateMachine;
        WysiwygPanel editorPanel;
        ModelViewTransform mvTransform;

        public ViewModel()
        {
            stateMachine = new GraphEditorStateMachine();
        }

        public void Dispose()
        {
        }

        public void HookUp(WysiwygPanel editorPanel)
        {
            this.editorPanel = editorPanel;
            editorPanel.PaintContent += EditorPanel_PaintContent;
            editorPanel.Resize += EditorPanel_Resize;

            mvTransform = new ModelViewTransform(
                editorPanel.ClientRectangle,
                stateMachine.CurrentModelState.Extents);
        }

        private void EditorPanel_Resize(object sender, EventArgs e)
        {
            mvTransform.UpdateViewport(editorPanel.ClientRectangle);
            editorPanel.InvalidateContent();
        }

        public void Run()
        {
            stateMachine.NewModel();

            View view = new View(this);
            Application.Run(view);
        }

        private void EditorPanel_PaintContent(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            var gstate = e.Graphics.Save();
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            new Screen(e.Graphics, mvTransform).Render(stateMachine.CurrentModelState);
            e.Graphics.Restore(gstate);

        }
    }

    class ModelViewTransform
    {
        Point2D modelCenter;
        Rational modelHeight;
        Rectangle viewExtents;

        Matrix3D modelToView;
        Matrix3D viewToModel;

        public ModelViewTransform(Rectangle viewExtents, Orthotope2D modelRange)
        {
            this.modelCenter = new Point2D(modelRange.X.Midpoint, modelRange.Y.Midpoint);
            this.modelHeight = modelRange.GetScaled(new Rational(11, 10)).Y.Size;
            this.viewExtents = viewExtents;

            InitMatrices();
        }

        public void UpdateViewport(Rectangle newViewExtents)
        {
            modelHeight = modelHeight * newViewExtents.Height / viewExtents.Height;
            viewExtents = newViewExtents;

            InitMatrices();
        }

        void InitMatrices()
        {
            var modelWidth = modelHeight * viewExtents.Width / viewExtents.Height;


            modelToView = AffineMapping.From(
                new Point2D(modelCenter.X - modelWidth / 2, modelCenter.Y + modelHeight / 2),
                new Point2D(modelCenter.X + modelWidth / 2, modelCenter.Y + modelHeight / 2),
                new Point2D(modelCenter.X - modelWidth / 2, modelCenter.Y - modelHeight / 2))
                .Onto(
                new Point2D(0, 0),
                new Point2D(viewExtents.Width, 0),
                new Point2D(0, viewExtents.Height)
                );

            viewToModel = modelToView.ComputeInverse();
        }

        public Point2D ViewFromModel(Point2D modelCoords)
        {
            var affineCoords = new Point3D(modelCoords.X, modelCoords.Y, 1);
            var affineResult = modelToView * affineCoords;
            return new Point2D(affineResult.X / affineResult.Z, affineResult.Y / affineResult.Z);
        }
    }

    class Screen
    {
        Graphics g;
        ModelViewTransform mvTransform;

        public Screen(Graphics g, ModelViewTransform mvTransform)
        {
            this.g = g;
            this.mvTransform = mvTransform;
        }

        public void Render(ReadOnlyGraph2D graph)
        {
            foreach (var vertex in graph.Vertices)
            {
                DrawPoint(mvTransform.ViewFromModel(vertex));
            }
        }

        private void DrawPoint(Point2D point2D)
        {
            var x = (float)point2D.X;
            var y = (float)point2D.Y;
            g.FillEllipse(Brushes.Red, x - 2, y - 2, 4, 4);
        }
    }
}
