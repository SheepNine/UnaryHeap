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

    static class RG
    {
        public const int NUM_PAGES = 4;
        public const int NUM_TILES = 256;
        public const int NUM_LAYERS = 4;
        public const int NUM_SPRITES = 204;
        public const int NUM_PALETTES = 8;
        public const int NUM_COLORS = 16;
        public const int NUM_TILE_PIXELS = TILE_DIMENSION * TILE_DIMENSION;
        public const int NUM_BACKGROUND_MAPPINGS = BACKGROUND_DIMENSION * BACKGROUND_DIMENSION;
        public const int TILE_DIMENSION = 8;
        public const int BACKGROUND_DIMENSION = 32;
        public const int IMAGE_DIMENSION = 248;
        public const int FILE_SIZE = 0x12600;

        public static void CheckPageIndex(int pageIndex)
        {
            if (OutOfRange(pageIndex, NUM_PAGES))
                throw new ArgumentOutOfRangeException("pageIndex");
        }

        public static void CheckTileIndex(int tileIndex)
        {
            if (OutOfRange(tileIndex, NUM_TILES))
                throw new ArgumentOutOfRangeException("tileIndex");
        }

        public static void CheckLayerIndex(int layerIndex)
        {
            if (OutOfRange(layerIndex, NUM_LAYERS))
                throw new ArgumentOutOfRangeException("layerIndex");
        }

        public static void CheckTileCoordinates(int x, int y)
        {
            if (OutOfRange(x, TILE_DIMENSION))
                throw new ArgumentOutOfRangeException("x");
            if (OutOfRange(y, TILE_DIMENSION))
                throw new ArgumentOutOfRangeException("y");
        }

        public static void CheckSpriteIndex(int spriteIndex)
        {
            if (OutOfRange(spriteIndex, NUM_SPRITES))
                throw new ArgumentOutOfRangeException("spriteIndex");
        }

        public static void CheckPaletteIndex(int paletteIndex)
        {
            if (OutOfRange(paletteIndex, NUM_PALETTES))
                throw new ArgumentOutOfRangeException("paletteIndex");
        }

        public static void CheckColorIndex(int colorIndex)
        {
            if (OutOfRange(colorIndex, NUM_COLORS))
                throw new ArgumentOutOfRangeException("colorIndex");
        }

        private static bool OutOfRange(int pageIndex, int numValues)
        {
            return pageIndex < 0 || pageIndex >= numValues;
        }

        public static IEnumerable<int> PageIndices()
        {
            return Enumerable.Range(0, NUM_PAGES);
        }
        public static IEnumerable<int> TileIndices()
        {
            return Enumerable.Range(0, NUM_TILES);
        }
        public static IEnumerable<int> LayerIndices()
        {
            return Enumerable.Range(0, NUM_LAYERS);
        }
        public static IEnumerable<int> SpriteIndices()
        {
            return Enumerable.Range(0, NUM_SPRITES);
        }
        public static IEnumerable<int> PaletteIndices()
        {
            return Enumerable.Range(0, NUM_PALETTES);
        }
        public static IEnumerable<int> ColorIndices()
        {
            return Enumerable.Range(0, NUM_COLORS);
        }
        public static IEnumerable<int> TileCoordinates()
        {
            return Enumerable.Range(0, TILE_DIMENSION);
        }
        public static IEnumerable<int> BackgroundCoordinates()
        {
            return Enumerable.Range(0, BACKGROUND_DIMENSION);
        }
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
            using (var input = new MemoryStream(RG.FILE_SIZE))
                return Deserialize(input);
        }

        public void Serialize(Stream output)
        {
            foreach (var pageIndex in RG.PageIndices())
                foreach (var tileIndex in RG.TileIndices())
                    backgroundPages[pageIndex][tileIndex].Serialize(output);

            foreach (var pageIndex in RG.PageIndices())
                foreach (var tileIndex in RG.TileIndices())
                    spritePages[pageIndex][tileIndex].Serialize(output);
            
            foreach (var paletteIndex in RG.PaletteIndices())
                foreach (var colorIndex in RG.ColorIndices())
                    backgroundPalettes[paletteIndex][colorIndex].Serialize(output);

            foreach (var paletteIndex in RG.PaletteIndices())
                foreach (var colorIndex in RG.ColorIndices())
                    spritePalettes[paletteIndex][colorIndex].Serialize(output);

            foreach (var layerIndex in RG.LayerIndices())
                foreach (var j in Enumerable.Range(0, 1024))
                    backgrounds[layerIndex][j].Serialize(output);

            foreach (var layerIndex in RG.LayerIndices())
                backgroundControls[layerIndex].Serialize(output);

            foreach (var spriteIndex in RG.SpriteIndices())
                sprites[spriteIndex].Serialize(output);
        }

        public static Retrographic Deserialize(Stream input)
        {
            var backgroundPages = new Tile[RG.NUM_PAGES][];
            foreach (var pageIndex in RG.PageIndices())
            {
                backgroundPages[pageIndex] = new Tile[RG.NUM_TILES];
                foreach (var tileIndex in RG.TileIndices())
                    backgroundPages[pageIndex][tileIndex] = Tile.Deserialize(input);
            }

            var spritePages = new Tile[RG.NUM_PAGES][];
            foreach (var pageIndex in RG.PageIndices())
            {
                spritePages[pageIndex] = new Tile[RG.NUM_TILES];
                foreach (var tileIndex in RG.TileIndices())
                    spritePages[pageIndex][tileIndex] = Tile.Deserialize(input);
            }

            var backgroundPalettes = new Color[RG.NUM_PALETTES][];
            foreach (var paletteIndex in RG.PaletteIndices())
            {
                backgroundPalettes[paletteIndex] = new Color[RG.NUM_COLORS];
                foreach (var colorIndex in RG.ColorIndices())
                    backgroundPalettes[paletteIndex][colorIndex] = Color.Deserialize(input);
            }

            var spritePalettes = new Color[RG.NUM_PALETTES][];
            foreach (var paletteIndex in RG.PaletteIndices())
            {
                spritePalettes[paletteIndex] = new Color[RG.NUM_COLORS];
                foreach (var colorIndex in RG.ColorIndices())
                    spritePalettes[paletteIndex][colorIndex] = Color.Deserialize(input);
            }

            var backgrounds = new Mapping[RG.NUM_LAYERS][];
            foreach (var layerIndex in RG.LayerIndices())
            {
                backgrounds[layerIndex] = new Mapping[1024];
                foreach (var j in Enumerable.Range(0, 1024))
                    backgrounds[layerIndex][j] = Mapping.Deserialize(input);
            }

            var backgroundControls = new BackgroundControl[RG.NUM_LAYERS];
            foreach (var layerIndex in RG.LayerIndices())
                backgroundControls[layerIndex] = BackgroundControl.Deserialize(input);

            var sprites = new Sprite[RG.NUM_SPRITES];
            foreach (var spriteIndex in RG.SpriteIndices())
                sprites[spriteIndex] = Sprite.Deserialize(input);

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

        public BackgroundControl GetBackgroundControl(int layerIndex)
        {
            RG.CheckLayerIndex(layerIndex);
            return backgroundControls[layerIndex];
        }

        public IReadOnlyList<Mapping> GetBackground(int layerIndex)
        {
            RG.CheckLayerIndex(layerIndex);
            return backgrounds[layerIndex];
        }

        public IReadOnlyList<Tile> GetBackgroundTilePage(int pageIndex)
        {
            RG.CheckPageIndex(pageIndex);
            return backgroundPages[pageIndex];
        }

        public IReadOnlyList<Color> GetBackgroundPalette(int paletteIndex)
        {
            RG.CheckPaletteIndex(paletteIndex);
            return backgroundPalettes[paletteIndex];
        }

        public IReadOnlyList<Tile> GetSpriteTilePage(int pageIndex)
        {
            RG.CheckPageIndex(pageIndex);
            return spritePages[pageIndex];
        }

        public IReadOnlyList<Color> GetSpritePalette(int paletteIndex)
        {
            RG.CheckPaletteIndex(paletteIndex);
            return spritePalettes[paletteIndex];
        }

        public Sprite GetSprite(int spriteIndex)
        {
            RG.CheckSpriteIndex(spriteIndex);
            return sprites[spriteIndex];
        }

        public void SetBackgroundControl(int layerIndex, BackgroundControl data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            RG.CheckLayerIndex(layerIndex);
            backgroundControls[layerIndex] = BackgroundControl.Clone(data);
        }

        public void SetBackground(int layerIndex, int mappingIndex, Mapping data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            RG.CheckLayerIndex(layerIndex);
            // check mapping index!
            backgrounds[layerIndex][mappingIndex] = Mapping.Clone(data);
        }

        public void SetBackgroundTilePage(int pageIndex, int tileIndex, Tile data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            RG.CheckPageIndex(pageIndex);
            RG.CheckTileIndex(tileIndex);
            backgroundPages[pageIndex][tileIndex] = Tile.Clone(data);
        }

        public void SetBackgroundPalette(int paletteIndex, int colorIndex, Color data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            RG.CheckPaletteIndex(paletteIndex);
            RG.CheckColorIndex(colorIndex);
            backgroundPalettes[paletteIndex][colorIndex] = Color.Clone(data);
        }

        public void SetSpriteTilePage(int pageIndex, int tileIndex, Tile data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            RG.CheckPageIndex(pageIndex);
            RG.CheckTileIndex(tileIndex);
            spritePages[pageIndex][tileIndex] = Tile.Clone(data);
        }

        public void SetSpritePalette(int paletteIndex, int colorIndex, Color data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            RG.CheckPaletteIndex(paletteIndex);
            RG.CheckColorIndex(colorIndex);
            spritePalettes[paletteIndex][colorIndex] = Color.Clone(data);
        }

        public void SetSprite(int spriteIndex, Sprite data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            RG.CheckSpriteIndex(spriteIndex);
            sprites[spriteIndex] = Sprite.Clone(data);
        }
    }

    static class RetrographicRasterizer
    {
        public static RawImage Rasterize(IRetrographicData data)
        {
            var result = new RawImage(RG.IMAGE_DIMENSION, RG.IMAGE_DIMENSION);
            foreach (var layerIndex in RG.LayerIndices())
                RasterizeLayer(result, data, layerIndex);
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

            foreach (var tileY in RG.BackgroundCoordinates())
                foreach (var tileX in RG.BackgroundCoordinates())
                {
                    var destX = RG.TILE_DIMENSION * tileX - backgroundControl.OffsetX;
                    var destY = RG.TILE_DIMENSION * tileY - backgroundControl.OffsetY;

                    int tileIndex = RG.BACKGROUND_DIMENSION * tileY + tileX;
                    var mapping = background[tileIndex];

                    Blit(result, data.GetBackgroundTilePage(mapping.Page), mapping.Tile,
                        data.GetBackgroundPalette(mapping.Palette),
                        destX, destY, 1, 1,
                        mapping.InvertTileX, mapping.InvertTileY, mapping.Masked);
                }
        }

        private static void RasterizeSprites(RawImage result, IRetrographicData data, int layerIndex)
        {
            foreach (var spriteIndex in RG.SpriteIndices())
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
            foreach (var tileY in Enumerable.Range(0, tilesY))
                foreach (var tileX in Enumerable.Range(0, tilesX))
                {
                    Blit(result, tilePage[(tile + tileX + (tileY << 4)) & 0xFF], palette,
                        destX + (invertTileX ? RG.TILE_DIMENSION * (tilesX - 1 - tileX) : RG.TILE_DIMENSION * tileX),
                        destY + (invertTileY ? RG.TILE_DIMENSION * (tilesY - 1 - tileY) : RG.TILE_DIMENSION * tileY),
                        invertTileX, invertTileY, masked);
                }
        }

        private static void Blit(RawImage result,
            Tile tile, IReadOnlyList<Color> palette,
            int destX, int destY,
            bool invertTileX, bool invertTileY, bool masked)
        {
            foreach (var y in RG.TileCoordinates())
                foreach (var x in RG.TileCoordinates())
                {
                    var colorIndex = tile[x, y];
                    if (masked && colorIndex == 0)
                        continue;

                    var color = palette[colorIndex];

                    var simX = destX + (invertTileX ? RG.TILE_DIMENSION - 1 - x : x);
                    var simY = destY + (invertTileY ? RG.TILE_DIMENSION - 1 - y : y);

                    if (simX < 0 || simX >= RG.IMAGE_DIMENSION) continue;
                    if (simY < 0 || simY >= RG.IMAGE_DIMENSION) continue;

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
            var buffer = new byte[RG.FILE_SIZE];
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