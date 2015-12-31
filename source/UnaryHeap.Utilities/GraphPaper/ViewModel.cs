using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using UnaryHeap.Utilities.Core;

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
        public void PreviewSelectSingleObject(Point p)
        {
            __ClearFeedback(); // TODO: implement me
        }

        public void PreviewToggleSingleObjectSelection(Point p)
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
        }

        public void Redo()
        {
            if (false == stateMachine.CanRedo)
                return;

            stateMachine.Redo();
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
    }
}
