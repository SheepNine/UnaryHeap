using System;
using System.Collections.Generic;
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
    public class ViewModel : IDisposable
    {
        const int MinScale = 1;
        const int MaxScale = 5;
        
        Tileset tileset;
        int scale;
        WysiwygPanel editorPanel;
        GestureInterpreter editorGestures;
        WysiwygFeedbackManager editorFeedback;
        WysiwygPanel tilesetPanel;
        GestureInterpreter tilesetGestures;
        WysiwygFeedbackManager tilesetFeedback;
        int activeTileIndex;
        bool gridVisible;
        Point editorOffset;
        Point editorDragOffset;
        ToolStripStatusLabel cursorPositionLabel;
        Bitmap backgroundFill;
        UndoAndRedo undoRedo;
        MruList mruList;

        public event EventHandler<CancelEventArgs> UnsavedChangesBeingDiscarded;
        protected bool OnUnsavedChangedBeingDiscarded()
        {
            if (null == UnsavedChangesBeingDiscarded)
                return false;

            var e = new CancelEventArgs();
            UnsavedChangesBeingDiscarded(this, e);
            return e.Cancel;
        }

        public ViewModel()
        {
            tileset = ProgramData.LoadTileset();
            activeTileIndex = 0;
            editorOffset = new Point(0, 0);
            backgroundFill = CreateBackgroundFill(10);
            undoRedo = new UndoAndRedo();
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
            gridVisible = locker.LoadGridVisibility();
            scale = Math.Max(MinScale, Math.Min(MaxScale, locker.LoadScale()));
            mruList = locker.LoadMruList();
            undoRedo.CurrentFileName = locker.LoadCurrentArrangementFilename();

            if (File.Exists(undoRedo.CurrentFileName))
                OpenArrangement(undoRedo.CurrentFileName);
            else
                NewArrangement();

            Application.Run(new View(this));

            locker.SaveCurrentArrangementFilename(undoRedo.CurrentFileName);
            locker.SaveMruList(mruList);
            locker.SaveScale(scale);
            locker.SaveGridVisibility(gridVisible);
        }

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

            editorFeedback = new WysiwygFeedbackManager(editorPanel);
            tilesetFeedback = new WysiwygFeedbackManager(tilesetPanel);

            ResizeTilesetPanel();
            UpdateTilesetFeedback();
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

            undoRedo.CurrentModel.Render(g, tileset, scale);
            RenderGrid(g, Color.FromArgb(128, Color.Black));

            g.Restore(state);
        }

        public void ExpandRight()
        {
            undoRedo.Do(m => m.ExpandRight());
            editorPanel.InvalidateContent();
        }

        public void ExpandLeft()
        {
            undoRedo.Do(m => m.ExpandLeft());
            editorPanel.InvalidateContent();
        }

        public void ExpandBottom()
        {
            undoRedo.Do(m => m.ExpandBottom());
            editorPanel.InvalidateContent();
        }

        public void ExpandTop()
        {
            undoRedo.Do(m => m.ExpandTop());
            editorPanel.InvalidateContent();
        }

        public void ContractRight()
        {
            if (undoRedo.CurrentModel.TileCountX < 2)
                return;

            undoRedo.Do(m => m.ContractRight());
            editorPanel.InvalidateContent();
        }

        public void ContractLeft()
        {
            if (undoRedo.CurrentModel.TileCountX < 2)
                return;

            undoRedo.Do(m => m.ContractLeft());
            editorPanel.InvalidateContent();
        }

        public void ContractTop()
        {
            if (undoRedo.CurrentModel.TileCountY < 2)
                return;

            undoRedo.Do(m => m.ContractTop());
            editorPanel.InvalidateContent();
        }

        public void ContractBottom()
        {
            if (undoRedo.CurrentModel.TileCountY < 2)
                return;

            undoRedo.Do(m => m.ContractBottom());
            editorPanel.InvalidateContent();
        }

        void PaintBackground(Graphics g, Rectangle rect)
        {
            using (var brush = new TextureBrush(backgroundFill))
                g.FillRectangle(brush, rect);
        }

        private void ApplyEditorOffset(Graphics g)
        {
            var delta = editorOffset;
            delta.Offset(editorDragOffset);
            delta = ClampEditorOffset(delta);

            g.TranslateTransform(delta.X, delta.Y);
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

        private void MruMenuItem_Click(object sender, EventArgs e)
        {
            var filename = (sender as ToolStripMenuItem).Tag as string;

            if (File.Exists(filename))
            {
                OpenArrangement(filename);
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

        private Point ClampEditorOffset(Point offset)
        {
            var size = editorPanel.Size - new Size(
                undoRedo.CurrentModel.TileCountX * tileset.TileSize * scale,
                undoRedo.CurrentModel.TileCountY * tileset.TileSize * scale);

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
                foreach (var y in Enumerable.Range(0, undoRedo.CurrentModel.TileCountY))
                    foreach (var x in Enumerable.Range(0, undoRedo.CurrentModel.TileCountX))
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

                if (tileX >= undoRedo.CurrentModel.TileCountX || tileY >= undoRedo.CurrentModel.TileCountY)
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

            if (tileX >= undoRedo.CurrentModel.TileCountX || tileY >= undoRedo.CurrentModel.TileCountY)
                return;

            if (MouseButtons.Left == e.Button)
            {
                if (e.ModifierKeys == Keys.None)
                {
                    undoRedo.Do(m => m[tileX, tileY] = activeTileIndex);
                    editorPanel.InvalidateContent();
                }
                else if (e.ModifierKeys == Keys.Shift)
                {
                    activeTileIndex = undoRedo.CurrentModel[tileX, tileY];
                    UpdateTilesetFeedback();
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

            activeTileIndex = tileX + tileY * stride;
            UpdateTilesetFeedback();
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

        public void NewArrangement()
        {
            if (undoRedo.IsModified && OnUnsavedChangedBeingDiscarded())
                return;

            undoRedo.NewModel();

            if (null != editorPanel)
                editorPanel.InvalidateContent();
        }

        public void SaveArrangement()
        {
            if (string.IsNullOrEmpty(undoRedo.CurrentFileName))
            {
                MessageBox.Show("No current filename. Save as instead.");
                return;
            }

            undoRedo.SaveAs(undoRedo.CurrentFileName);
            mruList.AddToList(undoRedo.CurrentFileName);
        }

        public void SaveArrangement(string filename)
        {
            undoRedo.SaveAs(filename);
            mruList.AddToList(filename);
        }

        public void OpenArrangement(string filename)
        {
            if (undoRedo.IsModified && OnUnsavedChangedBeingDiscarded())
                return;

            undoRedo.LoadModel(filename);
            mruList.AddToList(filename);

            if (null != editorPanel)
                editorPanel.InvalidateContent();
        }

        public bool CanClose()
        {
            if (undoRedo.IsModified && OnUnsavedChangedBeingDiscarded())
                return false;

            return true;
        }

        public void Export(string filename, ImageFormat format)
        {
            using (var outputBitmap = new Bitmap(
                undoRedo.CurrentModel.TileCountX * tileset.TileSize * scale,
                undoRedo.CurrentModel.TileCountY * tileset.TileSize * scale))
            {
                using (var g = Graphics.FromImage(outputBitmap))
                    undoRedo.CurrentModel.Render(g, tileset, scale);

                outputBitmap.Save(filename, format);
            }
        }

        public void CopyRenderedArrangement()
        {
            using (var outputBitmap = new Bitmap(
                undoRedo.CurrentModel.TileCountX * tileset.TileSize * scale,
                undoRedo.CurrentModel.TileCountY * tileset.TileSize * scale))
            {
                using (var g = Graphics.FromImage(outputBitmap))
                    undoRedo.CurrentModel.Render(g, tileset, scale);

                Clipboard.SetImage(outputBitmap);
            }
        }

        private void ResizeTilesetPanel()
        {
            tilesetPanel.Width = tileset.ImageWidth * scale;
        }

        public void Undo()
        {
            if (false == undoRedo.CanUndo)
                return;

            undoRedo.Undo();
            editorPanel.InvalidateContent();
        }

        public void Redo()
        {
            if (false == undoRedo.CanRedo)
                return;

            undoRedo.Redo();
            editorPanel.InvalidateContent();
        }

        public void ChangeTileset(string newTilesetFilename)
        {
            tileset.Dispose();
            tileset = new Tileset(new Bitmap(newTilesetFilename), 8);

            activeTileIndex = 0;
            tilesetPanel.InvalidateContent();
            editorPanel.InvalidateContent();
            ResizeTilesetPanel();
            UpdateTilesetFeedback();
        }
    }

    class RectFeedback : IWysiwygFeedback
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

        public bool Equals(IWysiwygFeedback other)
        {
            var castOther = other as RectFeedback;

            if (null == castOther)
                return false;

            return this.rect.Equals(castOther.rect);
        }
    }
}
