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
        event EventHandler CurrentFilenameChanged;
        event EventHandler IsModifiedChanged;

        string CurrentFileName { get; }
        bool IsModified { get; }

        void HookUp(WysiwygPanel editorPanel, GestureInterpreter editorGestures);

        void New();
        void Load();
        void ViewWholeModel();
    }

    class ViewModel : IDisposable, IViewModel
    {
        GraphEditorStateMachine stateMachine;
        WysiwygPanel editorPanel;
        GestureInterpreter editorGestures;
        ModelViewTransform mvTransform;

        public event EventHandler CurrentFilenameChanged
        {
            add { stateMachine.CurrentFileNameChanged += value; }
            remove { stateMachine.CurrentFileNameChanged -= value; }
        }

        public event EventHandler IsModifiedChanged
        {
            add { stateMachine.IsModifiedChanged += value; }
            remove { stateMachine.IsModifiedChanged -= value; }
        }

        public ViewModel()
        {
            stateMachine = new GraphEditorStateMachine();
        }

        public void Dispose()
        {
        }

        public void HookUp(WysiwygPanel editorPanel, GestureInterpreter editorGestures)
        {
            this.editorPanel = editorPanel;
            editorPanel.PaintContent += EditorPanel_PaintContent;
            editorPanel.Resize += EditorPanel_Resize;

            this.editorGestures = editorGestures;
            editorGestures.ClickGestured += EditorGestures_ClickGestured;
            editorGestures.DragGestured += EditorGestures_DragGestured;

            mvTransform = new ModelViewTransform(
                editorPanel.ClientRectangle,
                stateMachine.CurrentModelState.Extents);

            stateMachine.ModelChanged += StateMachine_ModelChanged;
        }

        private void EditorGestures_DragGestured(object sender, DragGestureEventArgs e)
        {
            if (MouseButtons.Right == e.Button)
            {
                var derp = Orthotope2D.FromPoints(new[] {
                        mvTransform.ModelFromView(new Point2D(e.StartPoint.X, e.StartPoint.Y)),
                        mvTransform.ModelFromView(new Point2D(e.EndPoint.X, e.EndPoint.Y))
                });

                if (0 != derp.X.Size && 0 != derp.Y.Size)
                {
                    mvTransform.UpdateModelRange(derp, 1);
                    editorPanel.InvalidateContent();
                }
            }
        }

        private void EditorGestures_ClickGestured(object sender, ClickGestureEventArgs e)
        {
            if (MouseButtons.Right == e.Button)
            {
                mvTransform.UpdateModelCenter(mvTransform.ModelFromView(
                    new Point2D(e.ClickPoint.X, e.ClickPoint.Y)));

                editorPanel.InvalidateContent();
            }
        }

        private void StateMachine_ModelChanged(object sender, EventArgs e)
        {
            editorPanel.InvalidateContent();
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
            e.Graphics.Clear(GraphPaperColors.Paper);
            var gstate = e.Graphics.Save();
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            var screen = new Screen(e.Graphics, mvTransform);
            screen.RenderGrid(editorPanel.ClientRectangle);
            screen.Render(stateMachine.CurrentModelState);
            e.Graphics.Restore(gstate);

        }

        public void New()
        {
            stateMachine.NewModel();
        }

        public void Load()
        {
            stateMachine.LoadModel();
        }

        public void ViewWholeModel()
        {
            mvTransform.UpdateModelRange(
                stateMachine.CurrentModelState.Extents, new Rational(11, 10));
            editorPanel.InvalidateContent();
        }

        public string CurrentFileName
        {
            get { return stateMachine.CurrentFileName; }
        }

        public bool IsModified
        {
            get { return stateMachine.IsModelModified; }
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
            this.modelCenter = new Point2D(newExtents.X.Midpoint, newExtents.Y.Midpoint);
            this.modelHeight = newExtents.GetScaled(padding).Y.Size;

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
            return Transform(modelCoords, modelToView);
        }

        public Point2D ModelFromView(Point2D viewCoords)
        {
            return Transform(viewCoords, viewToModel);
        }

        static Point2D Transform(Point2D p, Matrix3D m)
        {
            var affineCoords = new Point3D(p.X, p.Y, 1);
            var affineResult = m * affineCoords;
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

        public void RenderGrid(Rectangle viewExtents)
        {
            var min = mvTransform.ModelFromView(
                new Point2D(viewExtents.Left, viewExtents.Bottom));
            var max = mvTransform.ModelFromView(
                new Point2D(viewExtents.Right, viewExtents.Top));

            using (var pen = new Pen(GraphPaperColors.GridLines))
            {
                for (var x = min.X.Ceiling; x <= max.X.Floor; x += 1)
                    DrawLine(pen,
                        mvTransform.ViewFromModel(new Point2D(x, min.Y)),
                        mvTransform.ViewFromModel(new Point2D(x, max.Y)));

                for (var y = min.Y.Ceiling; y <= max.Y.Floor; y += 1)
                    DrawLine(pen,
                        mvTransform.ViewFromModel(new Point2D(min.X, y)),
                        mvTransform.ViewFromModel(new Point2D(max.X, y)));
            }
        }

        public void Render(ReadOnlyGraph2D graph)
        {
            using (var brush = new SolidBrush(GraphPaperColors.BluePen))
                using (var pen = new Pen(brush, 3.0f))
                    foreach (var edge in graph.Edges)
                        DrawLine(pen,
                            mvTransform.ViewFromModel(edge.Item1),
                            mvTransform.ViewFromModel(edge.Item2));

            using (var brush = new SolidBrush(GraphPaperColors.RedPen))
                foreach (var vertex in graph.Vertices)
                    DrawPoint(brush, mvTransform.ViewFromModel(vertex));
        }            

        private void DrawPoint(Brush b, Point2D point2D)
        {
            var radius = 4.0f;

            var x = (float)point2D.X;
            var y = (float)point2D.Y;
            g.FillEllipse(b, x - radius, y - radius, radius * 2, radius * 2);
        }

        private void DrawLine(Pen p, Point2D start, Point2D end)
        {
            g.DrawLine(p, (float)start.X, (float)start.Y, (float)end.X, (float)end.Y);
        }
    }

    class GraphPaperColors
    {
        public static Color Paper { get { return Color.FromArgb(255, 250, 240); } }
        public static Color GridLines { get { return Color.FromArgb(100, 200, 255); } }
        public static Color BluePen { get { return Color.FromArgb(30, 30, 160); } }
        public static Color RedPen { get { return Color.FromArgb(220, 20, 20); } }
    }
}
