using System;
using System.Drawing;
using System.IO;
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

        public ViewModel()
        {
            arrangement = ProgramData.LoadArrangement();
            tileset = ProgramData.LoadTileset();
            scale = 3;
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

        public void HookUpToView(WysiwygPanel editorPanel, GestureInterpreter editorGestures)
        {
            this.editorPanel = editorPanel;
            this.editorGestures = editorGestures;

            editorPanel.PaintContent += editorPanel_PaintContent;
            editorPanel.PaintFeedback += editorPanel_PaintFeedback;
            editorGestures.StateChanged += editorGestures_StateChanged;
        }

        void editorPanel_PaintFeedback(object sender, PaintEventArgs e)
        {
            var viewTileSize = tileset.TileSize * scale;

            switch (editorGestures.CurrentState)
            {
                case GestureState.Hover:
                    {
                        var tileX = editorGestures.CurrentPosition.X / viewTileSize;
                        var tileY = editorGestures.CurrentPosition.Y / viewTileSize;
                        var viewX = tileX * viewTileSize;
                        var viewY = tileY * viewTileSize;

                        e.Graphics.DrawRectangle(Pens.Black, viewX - 1, viewY - 1, viewTileSize + 1, viewTileSize + 1);
                        e.Graphics.DrawRectangle(Pens.White, viewX - 2, viewY - 2, viewTileSize + 3, viewTileSize + 3);
                        e.Graphics.DrawRectangle(Pens.Black, viewX - 3, viewY - 3, viewTileSize + 5, viewTileSize + 5);
                    }
                    break;
            }
        }

        void editorPanel_PaintContent(object sender, PaintEventArgs e)
        {
            arrangement.Render(e.Graphics, tileset, scale);
        }

        void editorGestures_StateChanged(object sender, EventArgs e)
        {
            editorPanel.InvalidateFeedback();
        }
    }

    static class ProgramData
    {
        #region Arrangement

        static string DefaultArrangementFile
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "patchwork.arr");
            }
        }

        public static void CreateDefaultArrangement()
        {
            if (File.Exists(DefaultArrangementFile))
                return;

            SaveArrangement(new TileArrangement(64, 48));
        }

        public static void SaveArrangement(TileArrangement defaultContents)
        {
            using (var file = File.Create(DefaultArrangementFile))
                defaultContents.Serialize(file);
        }

        public static TileArrangement LoadArrangement()
        {
            using (var file = File.Open(DefaultArrangementFile, FileMode.Open))
                return TileArrangement.Deserialize(file);
        }

        #endregion


        #region Tileset

        static string DefaultTileImageFile
        {
            get { return "tileset_template_1x.png"; }
        }

        public static Tileset LoadTileset()
        {
            return new Tileset(new Bitmap(DefaultTileImageFile), 8);
        }

        #endregion
    }
}
