using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.UI;

namespace GraphPaper
{
    interface IViewModel
    {
        event EventHandler CurrentFilenameChanged;
        event EventHandler IsModifiedChanged;
        event EventHandler CursorLocationChanged;
        event EventHandler ContentChanged;

        string CurrentFileName { get; }
        bool IsModified { get; }
        string CursorLocation { get; }

        void HookUp(WysiwygPanel editorPanel);

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

        void ShowNoOperationFeedback();
        void RemoveFeedback();
        void PreviewHover(Point p);
    }

    class ViewModel : IDisposable, IViewModel
    {
        GraphEditorStateMachine stateMachine;
        WysiwygFeedbackStrategyContext editorFeedback;
        ModelViewTransform mvTransform;
        GridSnapper gridSnapper;
        GraphObjectSelection selection;

        public event EventHandler ContentChanged;
        protected void OnContentChanged()
        {
            if (null != ContentChanged)
                ContentChanged(this, EventArgs.Empty);
        }

        public event EventHandler CursorLocationChanged;
        protected void OnCursorLocationChanged()
        {
            if (null != CursorLocationChanged)
                CursorLocationChanged(this, EventArgs.Empty);
        }

        public string CursorLocation { get; private set; }

        public ViewModel()
        {
            stateMachine = new GraphEditorStateMachine();
            stateMachine.ModelReplaced += (sender, e) => { ViewWholeModel(); };
            stateMachine.ModelChanged += (sender, e) => { OnContentChanged(); };

            gridSnapper = new GridSnapper();

            selection = new GraphObjectSelection();
            selection.SelectionChanged += (sender, e) => { OnContentChanged(); };

            CursorLocation = string.Empty;
        }

        public void Dispose()
        {
        }

        public void HookUp(WysiwygPanel editorPanel)
        {
            mvTransform = new ModelViewTransform();
            mvTransform.TransformChanged += (sender, e) => { OnContentChanged(); };

            editorFeedback = new WysiwygFeedbackStrategyContext(editorPanel);
        }

        private void StateMachine_ModelReplaced(object sender, EventArgs e)
        {
            ViewWholeModel();
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
            g.Clear(GraphPaperColors.Paper);
            var gstate = g.Save();
            g.SmoothingMode = SmoothingMode.HighQuality;
            var screen = new Screen(mvTransform);
            screen.RenderGrid(g, new Rational(1, 2));
            screen.Render(g, stateMachine.CurrentModelState);
            screen.Render(g, selection);
            g.Restore(gstate);

        }

        //--------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------
        public void ShowNoOperationFeedback()
        {
            editorFeedback.ClearFeedback();
            OnCursorLocationChanged();
        }

        public void RemoveFeedback()
        {
            editorFeedback.ClearFeedback();
            OnCursorLocationChanged();
        }

        public void PreviewHover(Point p)
        {
            var point = gridSnapper.Snap(mvTransform.ModelFromView(p));
            CursorLocation = string.Format("({0}, {1})", (double)point.X, (double)point.Y);
            editorFeedback.SetFeedback(new HoverFeedback(point, mvTransform));

            OnCursorLocationChanged();
        }

        public void PreviewAddEdge(Point startPoint, Point currentPoint)
        {
            var startVertex = gridSnapper.Snap(mvTransform.ModelFromView(startPoint));
            var endVertex = gridSnapper.Snap(mvTransform.ModelFromView(currentPoint));

            if (startVertex.Equals(endVertex))
                editorFeedback.ClearFeedback();
            else
                editorFeedback.SetFeedback(new AddEdgeFeedback(
                    startVertex, endVertex, mvTransform));

            OnCursorLocationChanged();
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
            editorFeedback.ClearFeedback();
        }

        public void ZoomOut()
        {
            mvTransform.ZoomOut();
            editorFeedback.ClearFeedback();
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
