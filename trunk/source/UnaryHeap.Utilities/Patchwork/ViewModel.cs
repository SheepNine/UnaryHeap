using System;
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
        TileArrangement arrangement;
        Tileset tileset;
        int scale;
        WysiwygPanel editorPanel;
        GestureInterpreter editorGestures;
        WysiwygPanel tilesetPanel;
        GestureInterpreter tilesetGestures;
        int activeTileIndex;
        bool renderGrid;
        Point editorOffset;
        Point editorDragOffset;
        ToolStripStatusLabel cursorPositionLabel;
        Bitmap backgroundFill;

        public ViewModel()
        {
            arrangement = new TileArrangement(40, 30);
            tileset = ProgramData.LoadTileset();
            scale = 4;
            activeTileIndex = 0;
            renderGrid = false;
            editorOffset = new Point(0, 0);
            backgroundFill = CreateBackgroundFill(10);
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

        public void Run()
        {
            Application.Run(new View(this));
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
            editorPanel.PaintFeedback += editorPanel_PaintFeedback;
            editorGestures.StateChanged += editorGestures_StateChanged;
            editorGestures.ClickGestured += editorGestures_ClickGestured;
            editorGestures.DragGestured += editorGestures_DragGestured;
            tilesetPanel.PaintContent += tilesetPanel_PaintContent;
            tilesetPanel.PaintFeedback += tilesetPanel_PaintFeedback;
            tilesetGestures.ClickGestured += tilesetGestures_ClickGestured;

            ResizeTilesetPanel();
        }

        void editorPanel_PaintFeedback(object sender, PaintEventArgs e)
        {
            var viewTileSize = tileset.TileSize * scale;

            switch (editorGestures.CurrentState)
            {
                case GestureState.Hover:
                    {
                        var tileX = (editorGestures.CurrentPosition.X - editorOffset.X)
                            / viewTileSize;
                        var tileY = (editorGestures.CurrentPosition.Y - editorOffset.Y)
                            / viewTileSize;
                        var viewX = tileX * viewTileSize + editorOffset.X;
                        var viewY = tileY * viewTileSize + editorOffset.Y;

                        e.Graphics.DrawRectangle(Pens.Black,
                            viewX - 1, viewY - 1, viewTileSize + 1, viewTileSize + 1);
                        e.Graphics.DrawRectangle(Pens.White,
                            viewX - 2, viewY - 2, viewTileSize + 3, viewTileSize + 3);
                        e.Graphics.DrawRectangle(Pens.Black,
                            viewX - 3, viewY - 3, viewTileSize + 5, viewTileSize + 5);
                    }
                    break;
                case GestureState.Clicking:
                    {
                        if (MouseButtons.Left == editorGestures.ClickButton)
                        {
                            var tileX = (editorGestures.CurrentPosition.X - editorOffset.X)
                                / viewTileSize;
                            var tileY = (editorGestures.CurrentPosition.Y - editorOffset.Y)
                                / viewTileSize;
                            var viewX = tileX * viewTileSize + editorOffset.X;
                            var viewY = tileY * viewTileSize + editorOffset.Y;

                            if (Keys.None == editorGestures.ModifierKeys)
                            {
                                e.Graphics.DrawRectangle(Pens.Purple,
                                    viewX - 1, viewY - 1, viewTileSize + 1, viewTileSize + 1);
                                tileset.DrawTile(e.Graphics, activeTileIndex, viewX, viewY, scale);
                            }
                            else if (Keys.Shift == editorGestures.ModifierKeys)
                            {
                                e.Graphics.DrawRectangle(Pens.Pink,
                                    viewX - 1, viewY - 1, viewTileSize + 1, viewTileSize + 1);
                            }
                        }
                    }
                    break;
            }
        }

        void editorPanel_PaintContent(object sender, PaintEventArgs e)
        {
            PaintBackground(e.Graphics, e.ClipRectangle);

            var g = e.Graphics;
            var state = g.Save();
            ApplyEditorOffset(g);

            arrangement.Render(g, tileset, scale);
            RenderGrid(g, Color.FromArgb(128, Color.Black));

            g.Restore(state);
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

        private Point ClampEditorOffset(Point offset)
        {
            var size = editorPanel.Size - new Size(
                arrangement.TileCountX * tileset.TileSize * scale,
                arrangement.TileCountY * tileset.TileSize * scale);

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
            var viewTileSize = tileset.TileSize * scale;

            if (renderGrid)
            {
                using (var pen = new Pen(c))
                    foreach (var y in Enumerable.Range(0, arrangement.TileCountY))
                        foreach (var x in Enumerable.Range(0, arrangement.TileCountX))
                            g.DrawRectangle(pen,
                                x * viewTileSize, y * viewTileSize,
                                viewTileSize - 1, viewTileSize - 1);
            }
        }

        void editorGestures_StateChanged(object sender, EventArgs e)
        {
            editorPanel.InvalidateFeedback();

            if (editorGestures.CurrentState == GestureState.Idle)
            {
                cursorPositionLabel.Text = string.Empty;
            }
            else if (editorGestures.CurrentState == GestureState.Hover)
            {
                var viewTileSize = tileset.TileSize * scale;
                var tileX = (editorGestures.CurrentPosition.X - editorOffset.X) / viewTileSize;
                var tileY = (editorGestures.CurrentPosition.Y - editorOffset.Y) / viewTileSize;

                cursorPositionLabel.Text = string.Format("{0}, {1}", tileX, tileY);
            }
            else if (editorGestures.CurrentState == GestureState.Dragging)
            {
                editorDragOffset = new Point(
                    editorGestures.CurrentPosition.X - editorGestures.DragStartPosition.X,
                    editorGestures.CurrentPosition.Y - editorGestures.DragStartPosition.Y);

                editorPanel.InvalidateContent();
            }
        }

        void editorGestures_ClickGestured(object sender, ClickGestureEventArgs e)
        {
            var viewTileSize = tileset.TileSize * scale;
            var tileX = (editorGestures.CurrentPosition.X - editorOffset.X) / viewTileSize;
            var tileY = (editorGestures.CurrentPosition.Y - editorOffset.Y) / viewTileSize;


            if (MouseButtons.Left == e.Button)
            {
                if (e.ModifierKeys == Keys.None)
                {
                    arrangement[tileX, tileY] = activeTileIndex;
                    editorPanel.InvalidateContent();
                }
                else if (e.ModifierKeys == Keys.Shift)
                {
                    activeTileIndex = arrangement[tileX, tileY];
                    tilesetPanel.InvalidateFeedback();
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

        void tilesetPanel_PaintFeedback(object sender, PaintEventArgs e)
        {
            var viewTileSize = tileset.TileSize * scale;
            var stride = Math.Max(1, tilesetPanel.Width / viewTileSize);

            var tileX = activeTileIndex % stride;
            var tileY = activeTileIndex / stride;
            var viewX = tileX * viewTileSize;
            var viewY = tileY * viewTileSize;

            e.Graphics.DrawRectangle(Pens.Black,
                viewX - 1, viewY - 1, viewTileSize + 1, viewTileSize + 1);
            e.Graphics.DrawRectangle(Pens.White,
                viewX - 2, viewY - 2, viewTileSize + 3, viewTileSize + 3);
            e.Graphics.DrawRectangle(Pens.Black,
                viewX - 3, viewY - 3, viewTileSize + 5, viewTileSize + 5);
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

            tilesetPanel.InvalidateFeedback();
        }

        public void ZoomIn()
        {
            scale = Math.Min(5, scale + 1);

            tilesetPanel.InvalidateContent();
            editorPanel.InvalidateContent();
            ResizeTilesetPanel();
        }

        public void ZoomOut()
        {
            scale = Math.Max(1, scale - 1);

            tilesetPanel.InvalidateContent();
            editorPanel.InvalidateContent();
            ResizeTilesetPanel();
        }

        public void ToggleGridDisplay()
        {
            renderGrid ^= true;

            tilesetPanel.InvalidateContent();
            editorPanel.InvalidateContent();
        }

        public void NewArrangement(int tileCountX, int tileCountY)
        {
            arrangement = new TileArrangement(tileCountX, tileCountY);

            editorPanel.InvalidateContent();
        }

        public void SaveArrangement(Stream destination)
        {
            arrangement.Serialize(destination);
        }

        public void OpenArrangement(Stream source)
        {
            arrangement = TileArrangement.Deserialize(source);

            editorPanel.InvalidateContent();
        }

        public void Export(string filename, ImageFormat format)
        {
            using (var outputBitmap = new Bitmap(
                arrangement.TileCountX * tileset.TileSize * scale,
                arrangement.TileCountY * tileset.TileSize * scale))
            {
                using (var g = Graphics.FromImage(outputBitmap))
                    arrangement.Render(g, tileset, scale);

                outputBitmap.Save(filename, format);
            }
        }

        private void ResizeTilesetPanel()
        {
            tilesetPanel.Width = tileset.ImageWidth * scale;
        }
    }
}
