﻿using System;
using System.Drawing;
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

        public ViewModel()
        {
            arrangement = ProgramData.LoadArrangement();
            tileset = ProgramData.LoadTileset();
            scale = 4;
            activeTileIndex = 0;
            renderGrid = false;
            editorOffset = new Point(0, 0);
        }

        public void Dispose()
        {
            ProgramData.SaveArrangement(arrangement);
            tileset.Dispose();
        }

        public void Run()
        {
            Application.Run(new View(this));
        }

        public void HookUpToView(
            WysiwygPanel editorPanel, GestureInterpreter editorGestures,
            WysiwygPanel tilesetPanel, GestureInterpreter tilesetGestures)
        {
            this.editorPanel = editorPanel;
            this.editorGestures = editorGestures;
            this.tilesetPanel = tilesetPanel;
            this.tilesetGestures = tilesetGestures;

            editorPanel.PaintContent += editorPanel_PaintContent;
            editorPanel.PaintFeedback += editorPanel_PaintFeedback;
            editorGestures.StateChanged += editorGestures_StateChanged;
            editorGestures.ClickGestured += editorGestures_ClickGestured;
            editorGestures.DragGestured += editorGestures_DragGestured;
            tilesetPanel.PaintContent += tilesetPanel_PaintContent;
            tilesetPanel.PaintFeedback += tilesetPanel_PaintFeedback;
            tilesetGestures.ClickGestured += tilesetGestures_ClickGestured;
        }

        void editorPanel_PaintFeedback(object sender, PaintEventArgs e)
        {
            var viewTileSize = tileset.TileSize * scale;

            switch (editorGestures.CurrentState)
            {
                case GestureState.Hover:
                    {
                        var tileX = (editorGestures.CurrentPosition.X - editorOffset.X) / viewTileSize;
                        var tileY = (editorGestures.CurrentPosition.Y - editorOffset.Y) / viewTileSize;
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
                        var tileX = (editorGestures.CurrentPosition.X - editorOffset.X) / viewTileSize;
                        var tileY = (editorGestures.CurrentPosition.Y - editorOffset.Y) / viewTileSize;
                        var viewX = tileX * viewTileSize + editorOffset.X;
                        var viewY = tileY * viewTileSize + editorOffset.Y;

                        e.Graphics.DrawRectangle(Pens.Purple,
                            viewX - 1, viewY - 1, viewTileSize + 1, viewTileSize + 1);
                        tileset.DrawTile(e.Graphics, activeTileIndex, viewX, viewY, scale);
                    }
                    break;
            }
        }

        void editorPanel_PaintContent(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            var state = g.Save();
            ApplyEditorOffset(g);

            g.Clear(Color.HotPink);
            arrangement.Render(g, tileset, scale);
            RenderGrid(g, Color.FromArgb(128, Color.Black));

            g.Restore(state);
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

            if (editorGestures.CurrentState == GestureState.Dragging)
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

            arrangement[tileX, tileY] = activeTileIndex;

            editorPanel.InvalidateContent();
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

            e.Graphics.Clear(Color.HotPink);
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
        }

        public void ZoomOut()
        {
            scale = Math.Max(1, scale - 1);

            tilesetPanel.InvalidateContent();
            editorPanel.InvalidateContent();
        }

        public void ToggleGridDisplay()
        {
            renderGrid ^= true;

            tilesetPanel.InvalidateContent();
            editorPanel.InvalidateContent();
        }
    }
}