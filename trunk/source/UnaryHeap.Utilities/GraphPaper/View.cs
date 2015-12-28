using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using UnaryHeap.Utilities.UI;

namespace GraphPaper
{
    partial class View : Form, IView
    {
        IViewModel viewModel;

        public View(IViewModel viewModel)
        {
            this.viewModel = viewModel;
            InitializeComponent();
            viewModel.CurrentFilenameChanged += viewModel_CurrentFilenameChanged;
            viewModel.IsModifiedChanged += viewModel_IsModifiedChanged;
            viewModel.ContentChanged += ViewModel_ContentChanged;
            viewModel.FeedbackChanged += ViewModel_FeedbackChanged;
            viewModel.SetViewExtents(editorPanel.ClientRectangle);
            UpdateDialogText();
        }

        private void ViewModel_FeedbackChanged(object sender, EventArgs e)
        {
            editorPanel.InvalidateFeedback();
        }

        private void ViewModel_ContentChanged(object sender, EventArgs e)
        {
            editorPanel.InvalidateContent();
        }

        private void viewModel_IsModifiedChanged(object sender, EventArgs e)
        {
            UpdateDialogText();
        }

        private void viewModel_CurrentFilenameChanged(object sender, EventArgs e)
        {
            UpdateDialogText();
        }

        void UpdateDialogText()
        {
            var builder = new StringBuilder();

            if (viewModel.IsModified)
                builder.Append("*");

            if (null != viewModel.CurrentFileName)
            {
                builder.Append(Path.GetFileNameWithoutExtension(viewModel.CurrentFileName));
                builder.Append(" - ");
            }

            builder.Append("GraphPaper");

            Text = builder.ToString();
        }

        private void newToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            viewModel.New();
        }

        private void loadToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            viewModel.Load();
        }

        private void closeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void wholeModelToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            viewModel.ViewWholeModel();
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ZoomIn();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.ZoomOut();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.Redo();
        }

        private void View_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (false == viewModel.CanClose())
                e.Cancel = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.SaveAs();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.SelectAll();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.DeleteSelected();
        }

        private void editorPanel_SizeChanged(object sender, EventArgs e)
        {
            viewModel.SetViewExtents(editorPanel.ClientRectangle);
        }

        void EditorGestures_ClickGestured(object sender, ClickGestureEventArgs e)
        {
            if (Keys.None == e.ModifierKeys && MouseButtons.Left == e.Button)
                viewModel.SelectSingleObject(e.ClickPoint);

            if (Keys.Control == e.ModifierKeys && MouseButtons.Left == e.Button)
                viewModel.ToggleSingleObjectSelection(e.ClickPoint);

            if (Keys.Alt == e.ModifierKeys && MouseButtons.Left == e.Button)
                viewModel.CenterView(e.ClickPoint);

            if (Keys.None == e.ModifierKeys && MouseButtons.Right == e.Button)
                viewModel.AddVertex(e.ClickPoint);
        }

        void EditorGestures_DragGestured(object sender, DragGestureEventArgs e)
        {
            if (Keys.Alt == e.ModifierKeys && MouseButtons.Left == e.Button)
                viewModel.AdjustViewExtents(PackRectangle(e.StartPoint, e.EndPoint));

            if (Keys.None == e.ModifierKeys && MouseButtons.Right == e.Button)
                viewModel.AddEdge(e.StartPoint, e.EndPoint);
        }

        static Rectangle PackRectangle(Point startPoint, Point endPoint)
        {
            return Rectangle.FromLTRB(
                Math.Min(startPoint.X, endPoint.X),
                Math.Min(startPoint.Y, endPoint.Y),
                Math.Max(startPoint.X, endPoint.X),
                Math.Max(startPoint.Y, endPoint.Y));
        }

        private void editorPanel_PaintContent(object sender, PaintEventArgs e)
        {
            viewModel.PaintContent(e.Graphics);
        }

        private void editorPanel_PaintFeedback(object sender, PaintEventArgs e)
        {
            viewModel.PaintFeedback(e.Graphics, e.ClipRectangle);
        }

        private void editorGestures_StateChanged(object sender, EventArgs e)
        {
            switch (editorGestures.CurrentState)
            {
                case GestureState.Idle:
                    viewModel.RemoveFeedback();
                    break;
                case GestureState.Hover:
                    viewModel.PreviewHover(editorGestures.CurrentPosition);
                    break;
                case GestureState.Clicking:
                    viewModel.ShowNoOperationFeedback();
                    break;
                case GestureState.Dragging:
                    if (MouseButtons.Right == editorGestures.ClickButton &&
                        Keys.None == editorGestures.ModifierKeys)
                    {
                        viewModel.PreviewAddEdge(editorGestures.DragStartPosition,
                            editorGestures.CurrentPosition);
                    }
                    else
                    {
                        viewModel.ShowNoOperationFeedback();
                    }
                    break;
            }
        }

        private void increaseGridResolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.IncreaseGridResolution();
        }

        private void decreaseGridResolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewModel.DecreaseGridResolution();
        }
    }

    interface IView
    {

    }
}
