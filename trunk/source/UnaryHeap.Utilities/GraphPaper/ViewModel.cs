using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using System.Linq;

namespace GraphPaper
{
    interface IViewModel
    {
        event EventHandler CurrentFilenameChanged;
        event EventHandler IsModifiedChanged;
        event EventHandler ContentChanged;
        event EventHandler FeedbackChanged;

        string CurrentFileName { get; }
        bool IsModified { get; }


        void New();
        void Load();
        void Save();
        void SaveAs();
        void ViewWholeModel();
        void ZoomIn();
        void ZoomOut();
        void IncreaseGridResolution();
        void DecreaseGridResolution();
        void Undo();
        void Redo();
        bool CanClose();
        void SelectAll();
        void SelectNone();
        void DeleteSelected();
        void SelectSingleObject(Point clickPoint);
        void ToggleSingleObjectSelection(Point clickPoint);
        void PreviewAddEdge(Point startPoint, Point currentPoint);
        void AddEdge(Point startVertex, Point endVertex);
        void AddVertex(Point vertex);
        void AdjustViewExtents(Rectangle modelExtents);
        void CenterView(Point centerPoint);
        void SetViewExtents(Rectangle extents);

        void PaintContent(Graphics g);
        void PaintFeedback(Graphics g, Rectangle clipRectangle);

        void ShowNoOperationFeedback();
        void RemoveFeedback();
        void PreviewHover(Point p);
        void PreviewSelectSingleObject(Point p);
        void PreviewToggleSingleObjectSelection(Point p);
        void PreviewAdjustViewExtents(Rectangle rectangle);
        void PreviewCenterView(Point p);
        void PreviewAddVertex(Point p);
        void SelectObjectsInArea(Rectangle rectangle);
        void PreviewSelectObjectsInArea(Rectangle rectangle);
        void AppendObjectsInAreaToSelection(Rectangle rectangle);
        void PreviewAppendObjectsInAreaToSelection(Rectangle rectangle);
        void MoveSelected(Point start, Point end);
        void PreviewMoveSelected(Point start, Point current);
        void SplitEdge(Point p);
        void PreviewSplitEdge(Point p);
    }

    class ViewModel : IDisposable, IViewModel
    {
        GraphEditorStateMachine stateMachine;
        ModelViewTransform mvTransform;
        GridSnapper gridSnapper;
        GraphObjectSelection selection;
        IFeedback feedback = new NullFeedback();

        public event EventHandler FeedbackChanged;
        protected void OnFeedbackChanged()
        {
            if (null != FeedbackChanged)
                FeedbackChanged(this, EventArgs.Empty);
        }

        public event EventHandler ContentChanged;
        protected void OnContentChanged()
        {
            if (null != ContentChanged)
                ContentChanged(this, EventArgs.Empty);
        }

        public ViewModel()
        {
            stateMachine = new GraphEditorStateMachine();
            stateMachine.ModelReplaced += (sender, e) => { ViewWholeModel(); };
            stateMachine.ModelChanged += (sender, e) => { OnContentChanged(); };

            gridSnapper = new GridSnapper();

            selection = new GraphObjectSelection();
            selection.SelectionChanged += (sender, e) => { OnContentChanged(); };

            mvTransform = new ModelViewTransform();
            mvTransform.TransformChanged += (sender, e) => { OnContentChanged(); };
        }

        public void Dispose()
        {
        }

        public void Run()
        {
            View view = new View(this);
            stateMachine.NewModel(new Graph2DCreateArgs(true));
            ViewWholeModel();
            Application.Run(view);
        }

        public void PaintContent(Graphics g)
        {
            using (var screen = new Screen(g, mvTransform))
            {
                g.Clear(GraphPaperColors.Paper);
                screen.RenderGrid(gridSnapper.GridSize);
                screen.Render(stateMachine.CurrentModelState, selection);
            }
        }

        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------

        public void PreviewMoveSelected(Point start, Point current)
        {
            __ClearFeedback(); // TODO: implement me
        }

        public void PreviewSelectSingleObject(Point p)
        {
            __ClearFeedback(); // TODO: implement me
        }

        public void PreviewToggleSingleObjectSelection(Point p)
        {
            __ClearFeedback(); // TODO: implement me
        }

        public void PreviewSplitEdge(Point p)
        {
            __ClearFeedback(); // TODO: implement me
        }

        public void PreviewAdjustViewExtents(Rectangle rectangle)
        {
            var rect = mvTransform.ModelFromView(rectangle);
            __SetFeedback(new AdjustViewExtentsFeedback(rect));
        }

        public void PreviewCenterView(Point p)
        {
            var point = mvTransform.ModelFromView(p);
            __SetFeedback(new CenterViewFeedback(point));
        }

        public void PreviewAddVertex(Point p)
        {
            var point = gridSnapper.Snap(mvTransform.ModelFromView(p));
            __SetFeedback(new AddVertexFeedback(point));
        }

        public void ShowNoOperationFeedback()
        {
            __SetFeedback(new UnsupportedFeedback());
        }

        public void RemoveFeedback()
        {
            __ClearFeedback();
        }

        public void PreviewHover(Point p)
        {
            var point = gridSnapper.Snap(mvTransform.ModelFromView(p));
            __SetFeedback(new HoverFeedback(point));
        }

        public void PreviewAddEdge(Point startPoint, Point currentPoint)
        {
            var startVertex = gridSnapper.Snap(mvTransform.ModelFromView(startPoint));
            var endVertex = gridSnapper.Snap(mvTransform.ModelFromView(currentPoint));

            __SetFeedback(new AddEdgeFeedback(startVertex, endVertex));
        }

        public void PreviewAppendObjectsInAreaToSelection(Rectangle rectangle)
        {
            __SetFeedback(new AppendSelectionFeedback(mvTransform.ModelFromView(rectangle)));
        }

        public void PreviewSelectObjectsInArea(Rectangle rectangle)
        {
            __SetFeedback(new SelectObjectsFeedback(mvTransform.ModelFromView(rectangle)));
        }

        void __ClearFeedback()
        {
            __SetFeedback(new NullFeedback());
        }

        void __SetFeedback(IFeedback newFeedback)
        {
            if (feedback.Equals(newFeedback))
                return;

            feedback = newFeedback;
            OnFeedbackChanged();
        }

        public void PaintFeedback(Graphics g, Rectangle clipRectangle)
        {
            using (var screen = new Screen(g, mvTransform))
                feedback.Render(screen);
        }

        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------

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

        public string CurrentFileName
        {
            get { return stateMachine.CurrentFileName; }
        }

        public bool IsModified
        {
            get { return stateMachine.IsModelModified; }
        }

        public void New()
        {
            stateMachine.NewModel(null);
        }

        public void Load()
        {
            stateMachine.LoadModel();
        }

        public void Save()
        {
            stateMachine.Save();
        }

        public void SaveAs()
        {
            stateMachine.SaveAs();
        }

        public void ViewWholeModel()
        {
            mvTransform.UpdateModelRange(
                stateMachine.CurrentModelState.Extents, new Rational(11, 10));
        }

        public void ZoomIn()
        {
            mvTransform.ZoomIn();
            __ClearFeedback();
        }

        public void ZoomOut()
        {
            mvTransform.ZoomOut();
            __ClearFeedback();
        }

        public void IncreaseGridResolution()
        {
            gridSnapper.GridSize /= 2;
            __ClearFeedback();
            OnContentChanged();
        }

        public void DecreaseGridResolution()
        {
            gridSnapper.GridSize *= 2;
            __ClearFeedback();
            OnContentChanged();
        }

        public void Undo()
        {
            if (false == stateMachine.CanUndo)
                return;

            stateMachine.Undo();
            selection.ClearSelection();
        }

        public void Redo()
        {
            if (false == stateMachine.CanRedo)
                return;

            stateMachine.Redo();
            selection.ClearSelection();
        }

        public bool CanClose()
        {
            return stateMachine.CanClose();
        }

        public void SelectAll()
        {
            selection.SelectAll(stateMachine.CurrentModelState);
        }

        public void SelectNone()
        {
            selection.ClearSelection();
        }

        public void SelectSingleObject(Point clickPoint)
        {
            var modelPoint = mvTransform.ModelFromView(clickPoint);
            selection.SelectNearestObject(
                stateMachine.CurrentModelState, modelPoint, SelectionQuadranceCutoff);
        }

        public void ToggleSingleObjectSelection(Point clickPoint)
        {
            var modelPoint = mvTransform.ModelFromView(clickPoint);
            selection.ToggleSelectionOfNearestObject(
                stateMachine.CurrentModelState, modelPoint, SelectionQuadranceCutoff);
        }

        Rational SelectionQuadranceCutoff
        {
            get
            {
                const int SelectionCutoffInPixels = 24;
                return mvTransform.Quadrance(
                    new Point(0, 0), new Point(SelectionCutoffInPixels, 0));
            }
        }

        public void DeleteSelected()
        {
            stateMachine.Do(graph =>
            {
                foreach (var edge in selection.Edges)
                    graph.RemoveEdge(edge.Item1, edge.Item2);
                foreach (var vertex in selection.Vertices)
                    graph.RemoveVertex(vertex);
            });
            selection.ClearSelection();
        }

        public void AddEdge(Point startPoint, Point endPoint)
        {
            var startVertex = gridSnapper.Snap(mvTransform.ModelFromView(startPoint));
            var endVertex = gridSnapper.Snap(mvTransform.ModelFromView(endPoint));

            if (startVertex.Equals(endVertex))
                return;

            if (stateMachine.CurrentModelState.HasVertex(startVertex) &&
                    stateMachine.CurrentModelState.HasVertex(endVertex) &&
                    stateMachine.CurrentModelState.HasEdge(startVertex, endVertex))
                return;

            stateMachine.Do(graph =>
            {
                if (!graph.HasVertex(startVertex))
                    graph.AddVertex(startVertex);
                if (!graph.HasVertex(endVertex))
                    graph.AddVertex(endVertex);

                graph.AddEdge(startVertex, endVertex);
            });
        }


        public void AddVertex(Point point)
        {
            var vertex = gridSnapper.Snap(mvTransform.ModelFromView(point));

            if (stateMachine.CurrentModelState.HasVertex(vertex))
                return;

            stateMachine.Do(graph => { graph.AddVertex(vertex); });
        }

        public void AdjustViewExtents(Rectangle viewExtents)
        {
            if (0 == viewExtents.Width || 0 == viewExtents.Height)
                return;

            mvTransform.UpdateModelRange(mvTransform.ModelFromView(viewExtents), 1);
        }

        public void CenterView(Point centerPoint)
        {
            mvTransform.UpdateModelCenter(mvTransform.ModelFromView(centerPoint));
        }

        public void SetViewExtents(Rectangle extents)
        {
            mvTransform.UpdateViewport(extents);
        }

        public void SelectObjectsInArea(Rectangle rectangle)
        {
            selection.SelectObjectsInArea(stateMachine.CurrentModelState,
                mvTransform.ModelFromView(rectangle));
        }

        public void AppendObjectsInAreaToSelection(Rectangle rectangle)
        {
            selection.AppendObjectsInAreaToSelection(stateMachine.CurrentModelState, 
                mvTransform.ModelFromView(rectangle));
        }

        public void MoveSelected(Point start, Point end)
        {
            var modelStart = gridSnapper.Snap(mvTransform.ModelFromView(start));
            var modelEnd = gridSnapper.Snap(mvTransform.ModelFromView(end));

            var dX = modelEnd.X - modelStart.X;
            var dY = modelEnd.Y - modelStart.Y;

            var selectedVertices = new SortedSet<Point2D>(new Point2DComparer());
            foreach (var vertex in selection.Vertices)
                selectedVertices.Add(vertex);
            foreach (var edge in selection.Edges)
            {
                selectedVertices.Add(edge.Item1);
                selectedVertices.Add(edge.Item2);
            }

            var destinationVertices = new SortedSet<Point2D>(new Point2DComparer());
            foreach (var vertex in selectedVertices)
                destinationVertices.Add(new Point2D(vertex.X + dX, vertex.Y + dY));

            var unselectedVertices = new SortedSet<Point2D>(new Point2DComparer());
            foreach (var vertex in stateMachine.CurrentModelState.Vertices)
                unselectedVertices.Add(vertex);
            unselectedVertices.ExceptWith(selectedVertices);

            unselectedVertices.IntersectWith(destinationVertices);

            if (unselectedVertices.Count > 0)
                return;

            // --- Sort points along the translation vector so a point is not moved
            // --- on top of another point being moved

            var moveOrder = selectedVertices
                .OrderByDescending(p => p.X * dX + p.Y * dY)
                .ToList();

            // --- Checks complete and data wrangled; make the change ---

            stateMachine.Do(g =>
            {
                foreach (var vertex in moveOrder)
                    g.MoveVertex(vertex, new Point2D(vertex.X + dX, vertex.Y + dY));
            });

            selection.TranslateValues(dX, dY);
        }

        public void SplitEdge(Point p)
        {
            var edge = stateMachine.CurrentModelState.FindNearestEdge(
                mvTransform.ModelFromView(p), SelectionQuadranceCutoff);

            if (null == edge)
                return;

            var midpoint = new Point2D(
                (edge.Item1.X + edge.Item2.X) / 2,
                (edge.Item1.Y + edge.Item2.Y) / 2);

            if (stateMachine.CurrentModelState.HasVertex(midpoint))
                return;

            stateMachine.Do(g =>
            {
                g.RemoveEdge(edge.Item1, edge.Item2);
                g.AddVertex(midpoint);
                g.AddEdge(edge.Item1, midpoint);
                g.AddEdge(midpoint, edge.Item2);
            });

            selection.ClearSelection();
        }
    }
}
