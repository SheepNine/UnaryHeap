using System;
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
        void ViewWholeModel();
        void ZoomIn();
        void ZoomOut();
        void Redo();
        void Undo();
    }

    class ViewModel : IDisposable, IViewModel
    {
        GraphEditorStateMachine stateMachine;
        WysiwygPanel editorPanel;
        GestureInterpreter editorGestures;
        ModelViewTransform mvTransform;
        GridSnapper gridSnapper;

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
            CursorLocation = string.Empty;
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
            editorGestures.StateChanged += EditorGestures_StateChanged;
            editorGestures.ClickGestured += EditorGestures_ClickGestured;
            editorGestures.DragGestured += EditorGestures_DragGestured;

            mvTransform = new ModelViewTransform(
                editorPanel.ClientRectangle,
                stateMachine.CurrentModelState.Extents);
            mvTransform.TransformChanged += MvTransform_TransformChanged;

            stateMachine.ModelChanged += StateMachine_ModelChanged;
            stateMachine.ModelReplaced += StateMachine_ModelReplaced;
        }

        private void EditorGestures_StateChanged(object sender, EventArgs e)
        {
            switch (editorGestures.CurrentState)
            {
                case GestureState.Idle:
                    break;
                case GestureState.Hover:
                case GestureState.Clicking:
                    var point = gridSnapper.Snap(mvTransform.ModelFromView(
                        editorGestures.CurrentPosition));
                    CursorLocation = string.Format("({0}, {1})",
                        (double)point.X, (double)point.Y);
                    break;
                case GestureState.Dragging:
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
            if (Keys.Alt == e.ModifierKeys)
            {
                if (MouseButtons.Left == e.Button)
                {
                    var derp = Orthotope2D.FromPoints(new[] {
                        mvTransform.ModelFromView(e.StartPoint),
                        mvTransform.ModelFromView(e.EndPoint)
                });

                    if (0 != derp.X.Size && 0 != derp.Y.Size)
                    {
                        mvTransform.UpdateModelRange(derp, 1);
                    }
                }
            }
            if (Keys.None == e.ModifierKeys)
            {

            }
        }

        private void EditorGestures_ClickGestured(object sender, ClickGestureEventArgs e)
        {
            if (Keys.Alt == e.ModifierKeys)
            {
                if (MouseButtons.Left == e.Button)
                    mvTransform.UpdateModelCenter(mvTransform.ModelFromView(e.ClickPoint));
            }
            if (Keys.None == e.ModifierKeys)
            {
                if (MouseButtons.Right == e.Button)
                {
                    var vert = gridSnapper.Snap(mvTransform.ModelFromView(e.ClickPoint));

                    if (false == stateMachine.CurrentModelState.HasVertex(vert))
                        stateMachine.Do(graph => {
                            graph.AddVertex(vert);
                        });
                }
            }
        }

        private void StateMachine_ModelChanged(object sender, EventArgs e)
        {
            editorPanel.InvalidateContent();
        }

        private void StateMachine_ModelReplaced(object sender, EventArgs e)
        {
            ViewWholeModel();
        }

        private void EditorPanel_Resize(object sender, EventArgs e)
        {
            mvTransform.UpdateViewport(editorPanel.ClientRectangle);
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
        }

        public void ZoomIn()
        {
            mvTransform.ZoomIn();
        }

        public void ZoomOut()
        {
            mvTransform.ZoomOut();
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
