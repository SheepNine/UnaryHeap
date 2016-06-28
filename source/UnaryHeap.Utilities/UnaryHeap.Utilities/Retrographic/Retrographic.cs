#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnaryHeap.Utilities.Misc;

namespace UnaryHeap.Utilities.Retrographic
{
    class Retrographic
    {
        private TilePage[] backgroundPages;
        private TilePage[] spritePages;
        private Palette[] backgroundPalettes;
        private Palette[] spritePalettes;
        private Background[] backgrounds;
        private BackgroundControl[] backgroundControls;
        private Sprite[] sprites;

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

            var backgroundControls = new BackgroundControl[4];
            foreach (var i in Enumerable.Range(0, 4))
                backgroundControls[i] = BackgroundControl.Deserialize(input);

            var sprites = new Sprite[204];
            foreach (var i in Enumerable.Range(0, 204))
                sprites[i] = Sprite.Deserialize(input);

            return new Retrographic()
            {
                backgroundPages = backgroundPages,
                spritePages = spritePages,
                backgroundPalettes = backgroundPalettes,
                spritePalettes = spritePalettes,
                backgrounds = backgrounds,
                backgroundControls = backgroundControls,
                sprites = sprites
            };
        }

        public RawImage Rasterize()
        {
            var result = new RawImage(248, 248);
            for (int i = 0; i < 4; i++)
                RasterizeLayer(result, i);
            return result;
        }

        private void RasterizeLayer(RawImage result, int layerIndex)
        {
            var backgroundControl = backgroundControls[layerIndex];
            
            if (backgroundControl.DrawOverSprites)
                RasterizeSprites(result, layerIndex);

            if (backgroundControl.Visible)
                RasterizeBackground(result, layerIndex);

            if (backgroundControl.DrawOverSprites == false)
                RasterizeSprites(result, layerIndex);
        }

        private void RasterizeBackground(RawImage result, int layerIndex)
        {
            var backgroundControl = backgroundControls[layerIndex];
            var background = backgrounds[layerIndex];

            for (int tileY = 0; tileY < 32; tileY++)
                for (int tileX = 0; tileX < 32; tileX++)
                {
                    var destX = tileX * 8 - backgroundControl.OffsetX;
                    var destY = tileY * 8 - backgroundControl.OffsetY;

                    int tileIndex = (tileY << 5) | tileX;
                    var mapping = background[tileIndex];

                    Blit(result, backgroundPages[mapping.Page], mapping.Tile,
                        backgroundPalettes[mapping.Palette],
                        destX, destY, 1, 1,
                        mapping.InvertTileX, mapping.InvertTileY, mapping.Masked);
                }
        }

        private void RasterizeSprites(RawImage result, int layerIndex)
        {
            for (int spriteIndex = 0; spriteIndex < 204; spriteIndex++)
            {
                var sprite = sprites[spriteIndex];

                if (sprite.Enabled && sprite.Layer == layerIndex)
                    RasterizeSprite(result, spriteIndex);
            }
        }

        private void RasterizeSprite(RawImage result, int spriteIndex)
        {
            var sprite = sprites[spriteIndex];

            Blit(result, spritePages[sprite.Page], sprite.Tile,
                spritePalettes[sprite.Palette],
                sprite.OffsetX, sprite.OffsetY, sprite.SizeX, sprite.SizeY,
                sprite.InvertSpriteX, sprite.InvertSpriteY, sprite.Masked);
        }

        private static void Blit(RawImage result, TilePage tilePage, int tile, Palette palette,
            int destX, int destY, int tilesX, int tilesY,
            bool invertTileX, bool invertTileY, bool masked)
        {
            for (var tileY = 0; tileY < tilesY; tileY++)
                for (var tileX = 0; tileX < tilesX; tileX++)
                {
                    Blit(result, tilePage[(tile + tileX + (tileY << 4)) & 0xFF], palette,
                        invertTileX ? (destX + 8 * (tilesX - tileX - 1)) : destX + 8 * tileX,
                        invertTileY ? (destY + 8 * (tilesY - tileY - 1)) : destY + 8 * tileY,
                        invertTileX, invertTileY, masked);
                }
        }

        private static void Blit(RawImage result, Tile tile, Palette palette, int destX, int destY,
            bool invertTileX, bool invertTileY, bool masked)
        {
            for (var y = 0; y < 8; y++)
                for (var x = 0; x < 8; x++)
                {
                    var colorIndex = tile[x, y];
                    if (masked && colorIndex == 0)
                        continue;

                    var color = palette[colorIndex];

                    var simX = invertTileX ? destX + 7 - x : destX + x;
                    var simY = invertTileY ? destY + 7 - y : destY + y;

                    if (simX < 0 || simX >= 248) continue;
                    if (simY < 0 || simY >= 248) continue;

                    if (color.Transparent)
                        result.BlendPixel(simX, simY, color.R, color.G, color.B);
                    else
                        result.SetPixel(simX, simY, color.R, color.G, color.B);
                }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ProofOfConcept
    {
        /// <summary>
        /// 
        /// </summary>
        public static void ProveConcept()
        {
            var buffer = new byte[0x12600];
            new Random(19830630).NextBytes(buffer);

            Retrographic graphic;
            using (var stream = new MemoryStream(buffer))
            {
                graphic = Retrographic.Deserialize(stream);

                var raster = graphic.Rasterize();

                using (var bitmap = raster.MakeBitmap())
                    bitmap.Save("output.png", ImageFormat.Png);
            }
        }
    }
}


#endif