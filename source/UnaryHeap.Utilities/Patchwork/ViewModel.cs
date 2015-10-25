using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UnaryHeap.Utilities.Misc;

namespace Patchwork
{
    public class ViewModel
    {
        public void Run()
        {
            // --- Load data ---

            var arrangement = ProgramData.LoadArrangement();
            var tileset = ProgramData.LoadTileset();


            // --- Execute program ---

            Application.Run(new View(this));

            // --- Save data ---

            ProgramData.SaveArrangement(arrangement);
            tileset.Dispose();
        }
    }

    static class ProgramData
    {
        #region Arrangement

        static string DefaultArrangementFile
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "patchwork.arr"); }
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
