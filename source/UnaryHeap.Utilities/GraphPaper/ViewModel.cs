﻿using System;
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

        string CurrentFileName { get; }
        bool IsModified { get; }

        void HookUp(WysiwygPanel editorPanel, GestureInterpreter editorGestures);

        void New();
        void Load();
        void ViewWholeModel();
        void ZoomIn();
        void ZoomOut();
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
            mvTransform.TransformChanged += MvTransform_TransformChanged;

            stateMachine.ModelChanged += StateMachine_ModelChanged;
            stateMachine.ModelReplaced += StateMachine_ModelReplaced;
        }

        private void MvTransform_TransformChanged(object sender, EventArgs e)
        {
            editorPanel.InvalidateContent();
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
                }
            }
        }

        private void EditorGestures_ClickGestured(object sender, ClickGestureEventArgs e)
        {
            if (MouseButtons.Right == e.Button)
            {
                mvTransform.UpdateModelCenter(mvTransform.ModelFromView(
                    new Point2D(e.ClickPoint.X, e.ClickPoint.Y)));
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
