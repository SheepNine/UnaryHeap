#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnaryHeap.Utilities.Retrographic
{
    class Retrographic
    {
        public static Retrographic Deserialize(Stream input)
        {
            var backgroundPages = new TilePage[4];
            foreach (var i in Enumerable.Range(0, 4))
                backgroundPages[i] = TilePage.Deserialize(input);

            var spritePages = new TilePage[4];
            foreach (var i in Enumerable.Range(0, 4))
                spritePages[i] = TilePage.Deserialize(input);

            var backgroundPalettes = new Palette[8];
            foreach (var paletteIndex in Enumerable.Range(0, 8))
                backgroundPalettes[paletteIndex] = Palette.Deserialize(input);

            var spritePalettes = new Palette[8];
            foreach (var paletteIndex in Enumerable.Range(0, 8))
                spritePalettes[paletteIndex] = Palette.Deserialize(input);

            var backgrounds = new Background[4];
            foreach (var i in Enumerable.Range(0, 4))
                backgrounds[i] = Background.Deserialize(input);

            var layerControls = new BackgroundControl[4];
            foreach (var i in Enumerable.Range(0, 4))
                layerControls[i] = BackgroundControl.Deserialize(input);

            var sprites = new Sprite[204];
            foreach (var i in Enumerable.Range(0, 204))
                sprites[i] = Sprite.Deserialize(input);

            return null;
        }
    }
}

#endif