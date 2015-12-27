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
        void Undo();
        void Redo();
        bool CanClose();
        void SelectAll();
        void DeleteSelected();
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
                screen.RenderGrid(new Rational(1, 2));
                screen.Render(stateMachine.CurrentModelState);
                screen.Render(selection);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------
        public void ShowNoOperationFeedback()
        {
            __ClearFeedback();
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

            if (startVertex.Equals(endVertex))
                __ClearFeedback();
            else
                __SetFeedback(new AddEdgeFeedback(startVertex, endVertex));
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

        //--------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------

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
            foreach (var vertex in stateMachine.CurrentModelState.Vertices)
                selection.SelectVertex(vertex);

        }

        public void DeleteSelected()
        {
            stateMachine.Do(graph =>
            {
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
    }
}
