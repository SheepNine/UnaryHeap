using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using UnaryHeap.Utilities.Misc;
using UnaryHeap.Utilities.UI;

namespace Patchwork
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length == 5)
            {
                // TODO: harden this
                ConvertArrangementToPng(args[0], args[1], int.Parse(args[2]),
                    args[3], int.Parse(args[4]));
                return 0;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var settings = new SettingsLocker(Properties.Settings.Default);

            using (var viewModel = new ViewModel())
                viewModel.Run(settings);

            settings.Persist();
            return 0;
        }

        private static void ConvertArrangementToPng(string arrangementFilename,
            string tilesetFilename, int tilesetTileSize, string outputFilename, int outputScale)
        {
            TileArrangementEditorStateMachine stateMachine =
                    new TileArrangementEditorStateMachine();
            stateMachine.LoadModel(arrangementFilename);

            Tileset tileset;
            using (var image = Bitmap.FromFile(tilesetFilename))
                tileset = new Tileset(image, tilesetTileSize);

            using (var outputBitmap = new Bitmap(
                stateMachine.CurrentModelState.TileCountX * tileset.TileSize * outputScale,
                stateMachine.CurrentModelState.TileCountY * tileset.TileSize * outputScale))
            {
                using (var g = Graphics.FromImage(outputBitmap))
                    stateMachine.CurrentModelState.Render(g, tileset, outputScale);

                outputBitmap.Save(outputFilename, ImageFormat.Png);
            }
        }
    }
}
