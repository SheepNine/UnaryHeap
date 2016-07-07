#if INCLUDE_WORK_IN_PROGRESS_RETROGRAPHIC

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Misc;

namespace UnaryHeap.Utilities.Retrographic
{
    interface IRetrographicData
    {
        BackgroundControl GetBackgroundControl(int index);
        IReadOnlyList<Mapping> GetBackground(int index);
        IReadOnlyList<Tile> GetBackgroundTilePage(int index);
        IReadOnlyList<Color> GetBackgroundPalette(int index);
        IReadOnlyList<Tile> GetSpriteTilePage(int index);
        IReadOnlyList<Color> GetSpritePalette(int index);
        Sprite GetSprite(int index);
    }

    interface IMutableRetrographicData : IRetrographicData
    {
        void SetBackgroundControl(int layerIndex, BackgroundControl data);
        void SetBackground(int layerIndex, int mappingIndex, Mapping data);
        void SetBackgroundTilePage(int pageIndex, int tileIndex, Tile data);
        void SetBackgroundPalette(int paletteIndex, int colorIndex, Color data);
        void SetSpriteTilePage(int pageIndex, int tileIndex, Tile data);
        void SetSpritePalette(int paletteIndex, int colorIndex, Color data);
        void SetSprite(int spriteIndex, Sprite data);
    }

    class Retrographic : IMutableRetrographicData
    {
        private Tile[][] backgroundPages;
        private Tile[][] spritePages;
        private Color[][] backgroundPalettes;
        private Color[][] spritePalettes;
        private Mapping[][] backgrounds;
        private BackgroundControl[] backgroundControls;
        private Sprite[] sprites;

        public static Retrographic CreateNew()
        {
            using (var input = new MemoryStream(0x12600))
                return Deserialize(input);
        }

        public void Serialize(Stream output)
        {
            foreach (var i in Enumerable.Range(0, 4))
                foreach (var j in Enumerable.Range(0, 256))
                    backgroundPages[i][j].Serialize(output);

            foreach (var i in Enumerable.Range(0, 4))
                foreach (var j in Enumerable.Range(0, 256))
                    spritePages[i][j].Serialize(output);
            
            foreach (var i in Enumerable.Range(0, 8))
                foreach (var j in Enumerable.Range(0, 16))
                    backgroundPalettes[i][j].Serialize(output);

            foreach (var i in Enumerable.Range(0, 8))
                foreach (var j in Enumerable.Range(0, 16))
                    spritePalettes[i][j].Serialize(output);

            foreach (var i in Enumerable.Range(0, 4))
                foreach (var j in Enumerable.Range(0, 1024))
                    backgrounds[i][j].Serialize(output);

            foreach (var i in Enumerable.Range(0, 4))
                backgroundControls[i].Serialize(output);

            foreach (var i in Enumerable.Range(0, 204))
                sprites[i].Serialize(output);
        }

        public static Retrographic Deserialize(Stream input)
        {
            var backgroundPages = new Tile[4][];
            foreach (var i in Enumerable.Range(0, 4))
            {
                backgroundPages[i] = new Tile[256];
                foreach (var j in Enumerable.Range(0, 256))
                    backgroundPages[i][j] = Tile.Deserialize(input);
            }

            var spritePages = new Tile[4][];
            foreach (var i in Enumerable.Range(0, 4))
            {
                spritePages[i] = new Tile[256];
                foreach (var j in Enumerable.Range(0, 256))
                    spritePages[i][j] = Tile.Deserialize(input);
            }

            var backgroundPalettes = new Color[8][];
            foreach (var i in Enumerable.Range(0, 8))
            {
                backgroundPalettes[i] = new Color[16];
                foreach (var j in Enumerable.Range(0, 16))
                    backgroundPalettes[i][j] = Color.Deserialize(input);
            }

            var spritePalettes = new Color[8][];
            foreach (var i in Enumerable.Range(0, 8))
            {
                spritePalettes[i] = new Color[16];
                foreach (var j in Enumerable.Range(0, 16))
                    spritePalettes[i][j] = Color.Deserialize(input);
            }

            var backgrounds = new Mapping[4][];
            foreach (var i in Enumerable.Range(0, 4))
            {
                backgrounds[i] = new Mapping[1024];
                foreach (var j in Enumerable.Range(0, 1024))
                    backgrounds[i][j] = Mapping.Deserialize(input);
            }

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

        public BackgroundControl GetBackgroundControl(int index)
        {
            return backgroundControls[index];
        }

        public IReadOnlyList<Mapping> GetBackground(int index)
        {
            return backgrounds[index];
        }

        public IReadOnlyList<Tile> GetBackgroundTilePage(int index)
        {
            return backgroundPages[index];
        }

        public IReadOnlyList<Color> GetBackgroundPalette(int index)
        {
            return backgroundPalettes[index];
        }

        public IReadOnlyList<Tile> GetSpriteTilePage(int index)
        {
            return spritePages[index];
        }

        public IReadOnlyList<Color> GetSpritePalette(int index)
        {
            return spritePalettes[index];
        }

        public Sprite GetSprite(int index)
        {
            return sprites[index];
        }

        public void SetBackgroundControl(int layerIndex, BackgroundControl data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            backgroundControls[layerIndex] = BackgroundControl.Clone(data);
        }

        public void SetBackground(int layerIndex, int mappingIndex, Mapping data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            backgrounds[layerIndex][mappingIndex] = Mapping.Clone(data);
        }

        public void SetBackgroundTilePage(int pageIndex, int tileIndex, Tile data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            backgroundPages[pageIndex][tileIndex] = Tile.Clone(data);
        }

        public void SetBackgroundPalette(int paletteIndex, int colorIndex, Color data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            backgroundPalettes[paletteIndex][colorIndex] = Color.Clone(data);
        }

        public void SetSpriteTilePage(int pageIndex, int tileIndex, Tile data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            spritePages[pageIndex][tileIndex] = Tile.Clone(data);
        }

        public void SetSpritePalette(int paletteIndex, int colorIndex, Color data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            spritePalettes[paletteIndex][colorIndex] = Color.Clone(data);
        }

        public void SetSprite(int spriteIndex, Sprite data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            sprites[spriteIndex] = Sprite.Clone(data);
        }
    }

    static class RetrographicRasterizer
    {
        public static RawImage Rasterize(IRetrographicData data)
        {
            var result = new RawImage(248, 248);
            for (int i = 0; i < 4; i++)
                RasterizeLayer(result, data, i);
            return result;
        }

        private static void RasterizeLayer(RawImage result, IRetrographicData data, int layerIndex)
        {
            var backgroundControl = data.GetBackgroundControl(layerIndex);
            
            if (backgroundControl.DrawOverSprites)
                RasterizeSprites(result, data, layerIndex);

            if (backgroundControl.Visible)
                RasterizeBackground(result, data, layerIndex);

            if (backgroundControl.DrawOverSprites == false)
                RasterizeSprites(result, data, layerIndex);
        }

        private static void RasterizeBackground(RawImage result, IRetrographicData data, int layerIndex)
        {
            var backgroundControl = data.GetBackgroundControl(layerIndex);
            var background = data.GetBackground(layerIndex);

            for (int tileY = 0; tileY < 32; tileY++)
                for (int tileX = 0; tileX < 32; tileX++)
                {
                    var destX = tileX * 8 - backgroundControl.OffsetX;
                    var destY = tileY * 8 - backgroundControl.OffsetY;

                    int tileIndex = (tileY << 5) | tileX;
                    var mapping = background[tileIndex];

                    Blit(result, data.GetBackgroundTilePage(mapping.Page), mapping.Tile,
                        data.GetBackgroundPalette(mapping.Palette),
                        destX, destY, 1, 1,
                        mapping.InvertTileX, mapping.InvertTileY, mapping.Masked);
                }
        }

        private static void RasterizeSprites(RawImage result, IRetrographicData data, int layerIndex)
        {
            for (int spriteIndex = 0; spriteIndex < 204; spriteIndex++)
            {
                var sprite = data.GetSprite(spriteIndex);

                if (sprite.Enabled && sprite.Layer == layerIndex)
                    RasterizeSprite(result, data, spriteIndex);
            }
        }

        private static void RasterizeSprite(RawImage result, IRetrographicData data, int spriteIndex)
        {
            var sprite = data.GetSprite(spriteIndex);

            Blit(result, data.GetSpriteTilePage(sprite.Page), sprite.Tile,
                data.GetSpritePalette(sprite.Palette),
                sprite.OffsetX, sprite.OffsetY, sprite.SizeX, sprite.SizeY,
                sprite.InvertSpriteX, sprite.InvertSpriteY, sprite.Masked);
        }

        private static void Blit(RawImage result,
            IReadOnlyList<Tile> tilePage, int tile, IReadOnlyList<Color> palette,
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

        private static void Blit(RawImage result,
            Tile tile, IReadOnlyList<Color> palette,
            int destX, int destY,
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
                for (int i = 0; i < 16; i++)
                {
                    graphic.SetBackgroundPalette(0, i, new Color(true, (byte)(8 * i), (byte)(8 * i), (byte)(8 * i)));
                    graphic.SetBackgroundPalette(1, i, new Color(true, 0, 0, (byte)(16 * i)));
                    graphic.SetBackgroundPalette(2, i, new Color(true, 0, (byte)(16 * i), 0));
                    graphic.SetBackgroundPalette(3, i, new Color(true, 0, (byte)(16 * i), (byte)(16 * i)));
                    graphic.SetBackgroundPalette(4, i, new Color(true, (byte)(16 * i), 0, 0));
                    graphic.SetBackgroundPalette(5, i, new Color(true, (byte)(16 * i), 0, (byte)(16 * i)));
                    graphic.SetBackgroundPalette(6, i, new Color(true, (byte)(16 * i), (byte)(16 * i), 0));
                    graphic.SetBackgroundPalette(7, i, new Color(true, (byte)(16 * i), (byte)(16 * i), (byte)(16 * i)));
                }

                var raster = RetrographicRasterizer.Rasterize(graphic);

                using (var bitmap = raster.MakeBitmap())
                    bitmap.Save("output.png", ImageFormat.Png);
            }
        }
    }
}


#endif