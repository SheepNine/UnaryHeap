﻿using System;
using System.Drawing;
using System.IO;
using UnaryHeap.Utilities.Misc;

namespace Patchwork
{
    static class ProgramData
    {
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
