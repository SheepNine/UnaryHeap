﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UnaryHeap.Utilities.Misc;
using UnaryHeap.Utilities.UI;

namespace Patchwork
{
    public interface IViewModel
    {
        event EventHandler CurrentFilenameChanged;
        event EventHandler IsModifiedChanged;

        string CurrentFileName { get; }
        bool IsModified { get; }

        void HookUpToView(
            WysiwygPanel editorPanel, GestureInterpreter editorGestures,
            WysiwygPanel tilesetPanel, GestureInterpreter tilesetGestures,
            ToolStripStatusLabel cursorPositionLabel);

        void NewArrangement();
        void OpenArrangement();
        void SyncMruList(ToolStripMenuItem openRecentToolStripMenuItem);
        void SaveArrangement();
        void SaveArrangementAs();
        void Export(string fileName, ImageFormat png);

        void Undo();
        void Redo();
        void CopyRenderedArrangement();

        void ExpandRight();
        void ExpandBottom();
        void ExpandLeft();
        void ExpandTop();

        void ContractRight();
        void ContractBottom();
        void ContractLeft();
        void ContractTop();

        void ZoomIn();
        void ZoomOut();
        void ToggleGridDisplay();
        void ChangeTileset(string fileName, int tileSize);
        void ReloadTileset();

        bool CanClose();
    }

    public class ViewModel : IViewModel, IDisposable
    {
        const int MinScale = 1;
        const int MaxScale = 5;
        
        Tileset tileset;
        string tilesetFilename;
        int scale;
        WysiwygPanel editorPanel;
        GestureInterpreter editorGestures;
        WysiwygFeedbackStrategyContext editorFeedback;
        WysiwygPanel tilesetPanel;
        GestureInterpreter tilesetGestures;
        WysiwygFeedbackStrategyContext tilesetFeedback;
        int activeTileIndex;
        bool gridVisible;
        Point editorOffset;
        Point editorDragOffset;
        ToolStripStatusLabel cursorPositionLabel;
        Bitmap backgroundFill;
        TileArrangementEditorStateMachine stateMachine;
        MruList mruList;

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
            activeTileIndex = 0;
            editorOffset = new Point(0, 0);
            backgroundFill = CreateBackgroundFill(10);
            stateMachine = new TileArrangementEditorStateMachine();
        }

        private void StateMachine_ModelChanged(object sender, EventArgs e)
        {
            editorPanel.InvalidateContent();
        }

        private void StateMachine_ModelReplaced(object sender, EventArgs e)
        {
            editorPanel.InvalidateContent();
        }

        Bitmap CreateBackgroundFill(int squareSize)
        {
            var result = new Bitmap(2 * squareSize, 2 * squareSize);

            using (var g = Graphics.FromImage(result))
            {
                g.Clear(Color.FromArgb(112, 112, 128));
                using (var brush = new SolidBrush(Color.FromArgb(108, 108, 124)))
                {
                    g.FillRectangle(brush, 0, 0, squareSize, squareSize);
                    g.FillRectangle(brush, squareSize, squareSize, squareSize, squareSize);
                }
            }

            return result;
        }

        public void Dispose()
        {
            tileset.Dispose();
            backgroundFill.Dispose();
        }

        public void Run(ISettingsLocker locker)
        {
            tilesetFilename = locker.LoadCurrentTilesetFilename();
            var tileSize = locker.LoadCurrentTilesetTileSize();

            if (false == File.Exists(tilesetFilename))
            {
                tilesetFilename = null;
                tileSize = 8;
                using (var bitmap = CreateInitialTileset())
                    tileset = new Tileset(bitmap, tileSize);
            }
            else
            {
                using (var bitmap = Bitmap.FromFile(tilesetFilename))
                    tileset = new Tileset(bitmap, locker.LoadCurrentTilesetTileSize());
            }

            gridVisible = locker.LoadGridVisibility();
            scale = Math.Max(MinScale, Math.Min(MaxScale, locker.LoadScale()));
            mruList = locker.LoadMruList();
            var startingArrangementFilename = locker.LoadCurrentArrangementFilename();

            if (File.Exists(startingArrangementFilename))
                stateMachine.LoadModel(startingArrangementFilename);
            else
                stateMachine.NewModel(new TileArrangementCreateArgs(45, 30));

            Application.Run(new View(this));

            locker.SaveCurrentArrangementFilename(stateMachine.CurrentFileName);
            locker.SaveCurrentTileset(tilesetFilename, tileset.TileSize);
            locker.SaveMruList(mruList);
            locker.SaveScale(scale);
            locker.SaveGridVisibility(gridVisible);
        }

        Bitmap CreateInitialTileset()
        {
            Bitmap result = new Bitmap(8, 8);
            using (var g = Graphics.FromImage(result))
            {
                g.Clear(Color.Gray);
                g.DrawLine(Pens.Black, 0, 0, 7, 7);
                g.DrawLine(Pens.Black, 0, 7, 7, 0);
            }
            return result;
        }

        void UndoRedo_CurrentFileNameChanged(object sender, EventArgs e)
        {
            if (false == string.Equals(stateMachine.CurrentFileName, null))
                mruList.AddToList(stateMachine.CurrentFileName);
        }

        void UpdateTilesetFeedback()
        {
            var viewTileSize = tileset.TileSize * scale;
            var stride = Math.Max(1, tilesetPanel.Width / viewTileSize);

            var tileX = activeTileIndex % stride;
            var tileY = activeTileIndex / stride;
            var viewX = tileX * viewTileSize;
            var viewY = tileY * viewTileSize;

            tilesetFeedback.SetFeedback(
                new RectFeedback(viewX, viewY, viewTileSize, viewTileSize));
        }

        void editorPanel_PaintContent(object sender, PaintEventArgs e)
        {
            PaintBackground(e.Graphics, e.ClipRectangle);

            var g = e.Graphics;
            var state = g.Save();
            ApplyEditorOffset(g);

            var delta = editorOffset;
            delta.Offset(editorDragOffset);
            delta = ClampEditorOffset(delta);

            var visibleRect = new Rectangle(
                e.ClipRectangle.Left - delta.X, e.ClipRectangle.Top - delta.Y,
                e.ClipRectangle.Width, e.ClipRectangle.Height);
            stateMachine.CurrentModelState.RenderSubset(g, tileset, scale, visibleRect);
            RenderGrid(g, Color.FromArgb(128, Color.Black));

            g.Restore(state);
        }

        void PaintBackground(Graphics g, Rectangle rect)
        {
            using (var brush = new TextureBrush(backgroundFill))
                g.FillRectangle(brush, rect);
        }

        void ApplyEditorOffset(Graphics g)
        {
            var delta = editorOffset;
            delta.Offset(editorDragOffset);
            delta = ClampEditorOffset(delta);

            g.TranslateTransform(delta.X, delta.Y);
        }

        Point ClampEditorOffset(Point offset)
        {
            var size = editorPanel.Size - new Size(
                stateMachine.CurrentModelState.TileCountX * tileset.TileSize * scale,
                stateMachine.CurrentModelState.TileCountY * tileset.TileSize * scale);

            if (size.Width > 0)
                size.Width = 0;
            if (size.Height > 0)
                size.Height = 0;

            offset.X = Math.Min(0, Math.Max(size.Width, offset.X));
            offset.Y = Math.Min(0, Math.Max(size.Height, offset.Y));

            return offset;
        }

        void RenderGrid(Graphics g, Color c)
        {
            if (false == gridVisible)
                return;

            var viewTileSize = tileset.TileSize * scale;

            using (var pen = new Pen(c))
                foreach (var y in Enumerable.Range(0,
                    stateMachine.CurrentModelState.TileCountY))
                    foreach (var x in Enumerable.Range(0,
                        stateMachine.CurrentModelState.TileCountX))
                        g.DrawRectangle(pen,
                            x * viewTileSize, y * viewTileSize,
                            viewTileSize - 1, viewTileSize - 1);
        }

        void editorGestures_StateChanged(object sender, EventArgs e)
        {
            if (editorGestures.CurrentState == GestureState.Idle)
            {
                editorFeedback.ClearFeedback();
                cursorPositionLabel.Text = string.Empty;
            }
            else if (editorGestures.CurrentState == GestureState.Hover)
            {
                var viewTileSize = tileset.TileSize * scale;
                var tileX = (editorGestures.CurrentPosition.X - editorOffset.X) / viewTileSize;
                var tileY = (editorGestures.CurrentPosition.Y - editorOffset.Y) / viewTileSize;

                if (tileX >= stateMachine.CurrentModelState.TileCountX ||
                    tileY >= stateMachine.CurrentModelState.TileCountY)
                {
                    cursorPositionLabel.Text = string.Empty;
                    editorFeedback.ClearFeedback();
                }
                else
                {
                    cursorPositionLabel.Text = string.Format("{0}, {1}", tileX, tileY);
                    editorFeedback.SetFeedback(new RectFeedback(
                        tileX * tileset.TileSize * scale + editorOffset.X,
                        tileY * tileset.TileSize * scale + editorOffset.Y,
                        tileset.TileSize * scale,
                        tileset.TileSize * scale));
                }
            }
            else if (editorGestures.CurrentState == GestureState.Clicking)
            {
                editorFeedback.ClearFeedback();
            }
            else if (editorGestures.CurrentState == GestureState.Dragging)
            {
                editorDragOffset = new Point(
                    editorGestures.CurrentPosition.X - editorGestures.DragStartPosition.X,
                    editorGestures.CurrentPosition.Y - editorGestures.DragStartPosition.Y);

                editorFeedback.ClearFeedback();
                editorPanel.InvalidateContent();
            }
        }

        void editorGestures_ClickGestured(object sender, ClickGestureEventArgs e)
        {
            var viewTileSize = tileset.TileSize * scale;
            var tileX = (editorGestures.CurrentPosition.X - editorOffset.X) / viewTileSize;
            var tileY = (editorGestures.CurrentPosition.Y - editorOffset.Y) / viewTileSize;

            if (tileX >= stateMachine.CurrentModelState.TileCountX ||
                tileY >= stateMachine.CurrentModelState.TileCountY)
                return;

            if (MouseButtons.Left == e.Button)
            {
                if (e.ModifierKeys == Keys.None)
                {
                    stateMachine.Do(m => m[tileX, tileY] = activeTileIndex);
                }
                else if (e.ModifierKeys == Keys.Shift)
                {
                    activeTileIndex = stateMachine.CurrentModelState[tileX, tileY];
                    UpdateTilesetFeedback();
                }
            }
            if (MouseButtons.Right == e.Button)
            {
                if (e.ModifierKeys == Keys.None)
                {
                    stateMachine.Do(m =>
                    {
                        var isRightEdge = tileX + 1 == m.TileCountX;

                        m[tileX, tileY] = activeTileIndex;
                        if (isRightEdge == false)
                            m[tileX + 1, tileY] = activeTileIndex + 1;
                    });
                }
                else if (e.ModifierKeys == Keys.Shift)
                {
                    stateMachine.Do(m =>
                    {
                        var isRightEdge = tileX + 1 == m.TileCountX;
                        var isBottomEdge = tileY + 1 == m.TileCountY;
                        var stride = Math.Max(1, tilesetPanel.Width / viewTileSize);

                        m[tileX, tileY] = activeTileIndex;
                        if (!isRightEdge)
                            m[tileX + 1, tileY] = activeTileIndex + 1;
                        if (!isBottomEdge)
                            m[tileX, tileY+1] = activeTileIndex + stride;
                        if (!isBottomEdge && !isRightEdge)
                            m[tileX + 1, tileY+1] = activeTileIndex + 1 + stride;
                    });

                }
            }
        }

        void editorGestures_DragGestured(object sender, DragGestureEventArgs e)
        {
            var deltaX = e.EndPoint.X - e.StartPoint.X;
            var deltaY = e.EndPoint.Y - e.StartPoint.Y;

            editorOffset = ClampEditorOffset(
                new Point(editorOffset.X + deltaX, editorOffset.Y + deltaY));
            editorDragOffset = new Point(0, 0);

            editorPanel.InvalidateContent();
        }

        void tilesetPanel_PaintContent(object sender, PaintEventArgs e)
        {
            var viewTileSize = tileset.TileSize * scale;
            var stride = Math.Max(1, tilesetPanel.Width / viewTileSize);

            PaintBackground(e.Graphics, e.ClipRectangle);
            for (int i = 0; i < tileset.NumTiles; i++)
            {
                var tileX = i % stride;
                var tileY = i / stride;
                var viewX = tileX * viewTileSize;
                var viewY = tileY * viewTileSize;

                tileset.DrawTile(e.Graphics, i, viewX, viewY, scale);
            }
        }

        void tilesetGestures_ClickGestured(object sender, ClickGestureEventArgs e)
        {
            if (0 > e.ClickPoint.X || tileset.ImageWidth * scale <= e.ClickPoint.X ||
                0 > e.ClickPoint.Y || tileset.ImageHeight * scale <= e.ClickPoint.Y)
                return;

            var viewTileSize = tileset.TileSize * scale;
            var tileX = tilesetGestures.CurrentPosition.X / viewTileSize;
            var tileY = tilesetGestures.CurrentPosition.Y / viewTileSize;
            var stride = Math.Max(1, tilesetPanel.Width / viewTileSize);
            var clickedTileIndex = tileX + tileY * stride;


            if (e.Button == MouseButtons.Left)
            {
                activeTileIndex = tileX + tileY * stride;
                UpdateTilesetFeedback();
            }
            else if (e.Button == MouseButtons.Right)
            {
                stateMachine.Do(m => m.SwapTileIndexes(activeTileIndex, clickedTileIndex));
            }
        }

        void ResizeTilesetPanel()
        {
            tilesetPanel.Width = tileset.ImageWidth * scale;
        }


        public string CurrentFileName
        {
            get { return stateMachine.CurrentFileName; }
        }

        public bool IsModified
        {
            get { return stateMachine.IsModelModified; }
        }


        #region IViewModel Implementation

        public void HookUpToView(
            WysiwygPanel editorPanel, GestureInterpreter editorGestures,
            WysiwygPanel tilesetPanel, GestureInterpreter tilesetGestures,
            ToolStripStatusLabel cursorPositionLabel)
        {
            this.editorPanel = editorPanel;
            this.editorGestures = editorGestures;
            this.tilesetPanel = tilesetPanel;
            this.tilesetGestures = tilesetGestures;
            this.cursorPositionLabel = cursorPositionLabel;

            editorPanel.PaintContent += editorPanel_PaintContent;
            editorGestures.StateChanged += editorGestures_StateChanged;
            editorGestures.ClickGestured += editorGestures_ClickGestured;
            editorGestures.DragGestured += editorGestures_DragGestured;
            tilesetPanel.PaintContent += tilesetPanel_PaintContent;
            tilesetGestures.ClickGestured += tilesetGestures_ClickGestured;

            editorFeedback = new WysiwygFeedbackStrategyContext(editorPanel);
            tilesetFeedback = new WysiwygFeedbackStrategyContext(tilesetPanel);

            ResizeTilesetPanel();
            UpdateTilesetFeedback();

            stateMachine.ModelChanged += StateMachine_ModelChanged;
            stateMachine.ModelReplaced += StateMachine_ModelReplaced;
            stateMachine.CurrentFileNameChanged += UndoRedo_CurrentFileNameChanged;
        }

        public void NewArrangement()
        {
            stateMachine.NewModel(null);
        }

        public void OpenArrangement()
        {
            stateMachine.LoadModel();
        }

        public void SyncMruList(ToolStripMenuItem openRecentToolStripMenuItem)
        {
            openRecentToolStripMenuItem.DropDownItems.Clear();

            if (0 == mruList.Count)
            {
                openRecentToolStripMenuItem.Enabled = false;
            }
            else
            {
                openRecentToolStripMenuItem.Enabled = true;

                for (int i = 0; i < mruList.Count; i++)
                {
                    var menuItem = new ToolStripMenuItem()
                    {
                        Text = Path.GetFileNameWithoutExtension(mruList[i]),
                        ToolTipText = mruList[i],
                        Tag = mruList[i]
                    };

                    menuItem.Click += MruMenuItem_Click;
                    openRecentToolStripMenuItem.DropDownItems.Add(menuItem);
                }
            }
        }

        void MruMenuItem_Click(object sender, EventArgs e)
        {
            var filename = (sender as ToolStripMenuItem).Tag as string;

            if (File.Exists(filename))
            {
                stateMachine.LoadModel(filename);
            }
            else
            {
                if (DialogResult.Yes == MessageBox.Show(
                    "File not found. Remove from MRU list?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1))
                {
                    mruList.RemoveFromList(filename);
                }
            }
        }

        public void SaveArrangement()
        {
            stateMachine.Save();
        }

        public void SaveArrangementAs()
        {
            stateMachine.SaveAs();
        }

        public void Export(string filename, ImageFormat format)
        {
            using (var outputBitmap = new Bitmap(
                stateMachine.CurrentModelState.TileCountX * tileset.TileSize * scale,
                stateMachine.CurrentModelState.TileCountY * tileset.TileSize * scale))
            {
                using (var g = Graphics.FromImage(outputBitmap))
                    stateMachine.CurrentModelState.Render(g, tileset, scale);

                outputBitmap.Save(filename, format);
            }
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

        public void CopyRenderedArrangement()
        {
            using (var outputBitmap = new Bitmap(
                stateMachine.CurrentModelState.TileCountX * tileset.TileSize * scale,
                stateMachine.CurrentModelState.TileCountY * tileset.TileSize * scale))
            {
                using (var g = Graphics.FromImage(outputBitmap))
                    stateMachine.CurrentModelState.Render(g, tileset, scale);

                Clipboard.SetImage(outputBitmap);
            }
        }

        public void ExpandRight()
        {
            stateMachine.Do(m => m.ExpandRight());
        }

        public void ExpandBottom()
        {
            stateMachine.Do(m => m.ExpandBottom());
        }

        public void ExpandLeft()
        {
            stateMachine.Do(m => m.ExpandLeft());
        }

        public void ExpandTop()
        {
            stateMachine.Do(m => m.ExpandTop());
        }

        public void ContractRight()
        {
            if (stateMachine.CurrentModelState.TileCountX < 2)
                return;

            stateMachine.Do(m => m.ContractRight());
        }

        public void ContractBottom()
        {
            if (stateMachine.CurrentModelState.TileCountY < 2)
                return;

            stateMachine.Do(m => m.ContractBottom());
        }

        public void ContractLeft()
        {
            if (stateMachine.CurrentModelState.TileCountX < 2)
                return;

            stateMachine.Do(m => m.ContractLeft());
        }

        public void ContractTop()
        {
            if (stateMachine.CurrentModelState.TileCountY < 2)
                return;

            stateMachine.Do(m => m.ContractTop());
        }

        public void ZoomIn()
        {
            scale = Math.Min(MaxScale, scale + 1);

            tilesetPanel.InvalidateContent();
            editorPanel.InvalidateContent();
            ResizeTilesetPanel();
            UpdateTilesetFeedback();
        }

        public void ZoomOut()
        {
            scale = Math.Max(MinScale, scale - 1);

            tilesetPanel.InvalidateContent();
            editorPanel.InvalidateContent();
            ResizeTilesetPanel();
            UpdateTilesetFeedback();
        }

        public void ToggleGridDisplay()
        {
            gridVisible ^= true;

            tilesetPanel.InvalidateContent();
            editorPanel.InvalidateContent();
        }

        public void ChangeTileset(string newTilesetFilename, int tileSize)
        {
            tileset.Dispose();
            using (var bitmap = new Bitmap(newTilesetFilename))
                tileset = new Tileset(bitmap, tileSize);
            tilesetFilename = newTilesetFilename;

            activeTileIndex = 0;
            tilesetPanel.InvalidateContent();
            editorPanel.InvalidateContent();
            ResizeTilesetPanel();
            UpdateTilesetFeedback();
        }

        public void ReloadTileset()
        {
            using (var bitmap = new Bitmap(tilesetFilename))
                tileset = new Tileset(bitmap, tileset.TileSize);
            
            tilesetPanel.InvalidateContent();
            editorPanel.InvalidateContent();
            ResizeTilesetPanel();
            UpdateTilesetFeedback();
        }

        public bool CanClose()
        {
            return stateMachine.CanClose();
        }

        #endregion
    }

    class RectFeedback : IWysiwygFeedbackStrategy
    {
        Rectangle rect;

        public RectFeedback(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
        }

        public void Render(Graphics g, Rectangle clipRectangle)
        {
            g.DrawRectangle(Pens.Black,
                rect.X - 1, rect.Y - 1, rect.Width + 1, rect.Height + 1);
            g.DrawRectangle(Pens.White,
                rect.X - 2, rect.Y - 2, rect.Width + 3, rect.Height + 3);
            g.DrawRectangle(Pens.Black,
                rect.X - 3, rect.Y - 3, rect.Width + 5, rect.Height + 5);
        }

        public bool Equals(IWysiwygFeedbackStrategy other)
        {
            var castOther = other as RectFeedback;

            if (null == castOther)
                return false;

            return this.rect.Equals(castOther.rect);
        }
    }
}
