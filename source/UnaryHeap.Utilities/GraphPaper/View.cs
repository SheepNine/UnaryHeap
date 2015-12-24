﻿using System;
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
            viewModel.HookUp(editorPanel, editorGestures);
            viewModel.CurrentFilenameChanged += viewModel_CurrentFilenameChanged;
            viewModel.IsModifiedChanged += viewModel_IsModifiedChanged;
            viewModel.CursorLocationChanged += ViewModel_CursorLocationChanged;
            UpdateDialogText();
        }

        private void ViewModel_CursorLocationChanged(object sender, EventArgs e)
        {
            cursorLocationLabel.Text = viewModel.CursorLocation;
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
    }

    interface IView
    {

    }
}
