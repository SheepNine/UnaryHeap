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

        string CurrentFileName { get; }
        bool IsModified { get; }
        string CursorLocation { get; }

        void HookUp(WysiwygPanel editorPanel, GestureInterpreter editorGestures);

        void New();
        void Load();
        void Save();
        void SaveAs();
        void ViewWholeModel();
        void ZoomIn();
        void ZoomOut();
        void Redo();
        void Undo();
        bool CanClose();
        void SelectAll();
        void DeleteSelected();
        void AddEdge(Point2D startVertex, Point2D endVertex);
        void AddVertex(Point2D vertex);
        void AdjustViewExtents(Orthotope2D modelExtents);
        void CenterView(Point2D modelCenterPoint);
        void SetViewExtents(Rectangle extents);
    }

    class ViewModel : IDisposable, IViewModel
    {
        GraphEditorStateMachine stateMachine;
        WysiwygPanel editorPanel;
        GestureInterpreter editorGestures;
        WysiwygFeedbackStrategyContext editorFeedback;
        ModelViewTransform mvTransform;
        GridSnapper gridSnapper;
        GraphObjectSelection selection;

        public event EventHandler CursorLocationChanged;
        protected void OnCursorLocationChanged()
        {
            if (null != CursorLocationChanged)
                CursorLocationChanged(this, EventArgs.Empty);
        }

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

        public string CursorLocation { get; private set; }

        public ViewModel()
        {
            stateMachine = new GraphEditorStateMachine();
            gridSnapper = new GridSnapper();
            selection = new GraphObjectSelection();
            CursorLocation = string.Empty;
        }

        public void Dispose()
        {
        }

        public void HookUp(WysiwygPanel editorPanel, GestureInterpreter editorGestures)
        {
            this.editorPanel = editorPanel;
            editorPanel.PaintContent += EditorPanel_PaintContent;

            this.editorGestures = editorGestures;
            editorGestures.StateChanged += EditorGestures_StateChanged;
            editorGestures.ClickGestured += EditorGestures_ClickGestured;
            editorGestures.DragGestured += EditorGestures_DragGestured;

            mvTransform = new ModelViewTransform(
                editorPanel.ClientRectangle,
                stateMachine.CurrentModelState.Extents);
            mvTransform.TransformChanged += MvTransform_TransformChanged;

            stateMachine.ModelChanged += StateMachine_ModelChanged;
            stateMachine.ModelReplaced += StateMachine_ModelReplaced;

            editorFeedback = new WysiwygFeedbackStrategyContext(editorPanel);

            selection.SelectionChanged += Selection_SelectionChanged;
        }

        private void Selection_SelectionChanged(object sender, EventArgs e)
        {
            editorPanel.InvalidateContent();
        }

        private void EditorGestures_StateChanged(object sender, EventArgs e)
        {
            switch (editorGestures.CurrentState)
            {
                case GestureState.Idle:
                    editorFeedback.ClearFeedback();
                    break;
                case GestureState.Hover:
                case GestureState.Clicking:
                    var point = gridSnapper.Snap(mvTransform.ModelFromView(
                        editorGestures.CurrentPosition));
                    CursorLocation = string.Format("({0}, {1})",
                        (double)point.X, (double)point.Y);
                    editorFeedback.SetFeedback(new ModelPointFeedback(point, mvTransform));
                    break;
                case GestureState.Dragging:
                    var start = gridSnapper.Snap(
                        mvTransform.ModelFromView(editorGestures.DragStartPosition));
                    var end = gridSnapper.Snap(
                        mvTransform.ModelFromView(editorGestures.CurrentPosition));

                    if (start.Equals(end))
                        editorFeedback.ClearFeedback();
                    else
                        editorFeedback.SetFeedback(
                            new CreateEdgeFeedback(start, end, mvTransform));
                    break;
            }

            OnCursorLocationChanged();
        }

        private void MvTransform_TransformChanged(object sender, EventArgs e)
        {
            editorPanel.InvalidateContent();
        }

        private void EditorGestures_DragGestured(object sender, DragGestureEventArgs e)
        {
            if (Keys.Alt == e.ModifierKeys && MouseButtons.Left == e.Button)
                AdjustViewExtents(mvTransform.ModelFromView(e.StartPoint, e.EndPoint));

            if (Keys.None == e.ModifierKeys && MouseButtons.Right == e.Button)
                AddEdge(gridSnapper.Snap(mvTransform.ModelFromView(e.StartPoint)),
                    gridSnapper.Snap(mvTransform.ModelFromView(e.EndPoint)));
        }

        public void AdjustViewExtents(Orthotope2D modelExtents)
        {
            if (0 == modelExtents.X.Size || 0 == modelExtents.Y.Size)
                return;

            mvTransform.UpdateModelRange(modelExtents, 1);
        }

        public void CenterView(Point2D modelPoint)
        {
            mvTransform.UpdateModelCenter(modelPoint);
        }

        public void AddEdge(Point2D startVertex, Point2D endVertex)
        {
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

        private void EditorGestures_ClickGestured(object sender, ClickGestureEventArgs e)
        {
            if (Keys.Alt == e.ModifierKeys && MouseButtons.Left == e.Button)
                CenterView(mvTransform.ModelFromView(e.ClickPoint));

            if (Keys.None == e.ModifierKeys && MouseButtons.Right == e.Button)
                AddVertex(gridSnapper.Snap(mvTransform.ModelFromView(e.ClickPoint)));
        }


        public void AddVertex(Point2D vertex)
        {
            if (stateMachine.CurrentModelState.HasVertex(vertex))
                return;

            stateMachine.Do(graph => { graph.AddVertex(vertex); });
        }

        private void StateMachine_ModelChanged(object sender, EventArgs e)
        {
            editorPanel.InvalidateContent();
        }

        private void StateMachine_ModelReplaced(object sender, EventArgs e)
        {
            ViewWholeModel();
        }

        public void SetViewExtents(Rectangle extents)
        {
            mvTransform.UpdateViewport(extents);
        }

        public void Run()
        {
            stateMachine.NewModel(new Graph2DCreateArgs(true));

            View view = new View(this);
            Application.Run(view);
        }

        private void EditorPanel_PaintContent(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(GraphPaperColors.Paper);
            var gstate = e.Graphics.Save();
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            var screen = new Screen(mvTransform);
            screen.RenderGrid(e.Graphics, editorPanel.ClientRectangle, new Rational(1, 2));
            screen.Render(e.Graphics, stateMachine.CurrentModelState);
            screen.Render(e.Graphics, selection);
            e.Graphics.Restore(gstate);

        }

        public void New()
        {
            stateMachine.NewModel(null);
        }

        public void Load()
        {
            stateMachine.LoadModel();
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

        public void Save()
        {
            stateMachine.Save();
        }

        public void SaveAs()
        {
            stateMachine.SaveAs();
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

        public string CurrentFileName
        {
            get { return stateMachine.CurrentFileName; }
        }

        public bool IsModified
        {
            get { return stateMachine.IsModelModified; }
        }
    }
}
