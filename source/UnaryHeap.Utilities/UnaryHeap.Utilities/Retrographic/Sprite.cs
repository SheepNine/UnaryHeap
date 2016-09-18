#if INCLUDE_WORK_IN_PROGRESS_RETROGRAPHIC

using System;
using System.IO;

namespace UnaryHeap.Utilities.Retrographic
{
    enum SpriteSize
    {
        OneByOne = 0,
        TwoByTwo = 1,
        FourByFour = 2,
        EightByEight = 3,
        TwoByFour = 4,
        FourByTwo = 5,
        FourByEight = 6,
        EightByFour = 7
    }

    sealed class Sprite
    {
        public sealed class Builder
        {
            int offsetX;
            int offsetY;
            int layer;
            SpriteSize size;
            bool enabled;
            int tile;
            int page;
            bool invertSpriteX;
            bool invertSpriteY;
            bool masked;
            int palette;

            public Builder()
            {
            }

            public Builder(Sprite source)
            {
                offsetX = source.OffsetX;
                offsetY = source.OffsetY;
                layer = source.Layer;
                size = source.size;
                enabled = source.Enabled;
                tile = source.Tile;
                page = source.Page;
                invertSpriteX = source.InvertSpriteX;
                invertSpriteY = source.InvertSpriteY;
                masked = source.Masked;
                palette = source.Palette;
            }

            public Builder WithOffsetX(int value) { offsetX = value; return this; }
            public Builder WithOffsetY(int value) { offsetY = value; return this; }
            public Builder WithLayer(int value) { layer = value; return this; }
            public Builder WithSize(SpriteSize value) { size = value; return this; }
            public Builder WithEnabled(bool value) { enabled = value; return this; }
            public Builder WithTile(int value) { tile = value; return this; }
            public Builder WithPage(int value) { page = value; return this; }
            public Builder WithInverSpriteX(bool value) { invertSpriteX = value; return this; }
            public Builder WithInvertSpriteY(bool value) { invertSpriteY = value; return this; }
            public Builder WithMasked(bool value) { masked = value; return this; }
            public Builder WithPalette(int value) { palette = value; return this; }

            public Sprite Build()
            {
                return new Sprite(offsetX, offsetY, layer, size, enabled,
                    tile, page, invertSpriteX, invertSpriteY, masked, palette);
            }
        }

        public Builder AsBuilder()
        {
            return new Builder(this);
        }
        

        public int Layer { get; private set; }
        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }
        public bool Enabled { get; private set; }

        SpriteSize size;
        public int SizeX
        {
            get
            {
                switch (size)
                {
                    case SpriteSize.OneByOne:
                        return 1;
                    case SpriteSize.TwoByTwo:
                    case SpriteSize.TwoByFour:
                        return 2;
                    case SpriteSize.FourByTwo:
                    case SpriteSize.FourByFour:
                    case SpriteSize.FourByEight:
                        return 4;
                    case SpriteSize.EightByFour:
                    case SpriteSize.EightByEight:
                        return 8;
                }
                throw new InvalidOperationException("size is not valid");
            }
        }

        public int SizeY
        {
            get
            {
                switch (size)
                {
                    case SpriteSize.OneByOne:
                        return 1;
                    case SpriteSize.TwoByTwo:
                    case SpriteSize.FourByTwo:
                        return 2;
                    case SpriteSize.TwoByFour:
                    case SpriteSize.FourByFour:
                    case SpriteSize.EightByFour:
                        return 4;
                    case SpriteSize.FourByEight:
                    case SpriteSize.EightByEight:
                        return 8;
                }
                throw new InvalidOperationException("size is not valid");
            }
        }
        Mapping mapping;
        public int Tile
        {
            get { return mapping.Tile; }
        }

        public int Page
        {
            get { return mapping.Page; }
        }

        public bool InvertSpriteX
        {
            get { return mapping.InvertTileX; }
        }

        public bool InvertSpriteY
        {
            get { return mapping.InvertTileY; }
        }

        public bool Masked
        {
            get { return mapping.Masked; }
        }

        public int Palette
        {
            get { return mapping.Palette; }
        }

        private Sprite(int offsetX, int offsetY, int layerIndex,
            SpriteSize size, bool enabled, Mapping mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException("mapping");
            RG.CheckLayerIndex(layerIndex);
            if (Math.Abs(offsetX) >= 256)
                throw new ArgumentOutOfRangeException("offsetX");
            if (Math.Abs(offsetY) >= 256)
                throw new ArgumentOutOfRangeException("offsetY");

            OffsetX = offsetX;
            OffsetY = offsetY;
            Layer = layerIndex;
            this.size = size;
            Enabled = enabled;
            this.mapping = Mapping.Clone(mapping);
        }

        public Sprite(int offsetX, int offsetY, int layer,
            SpriteSize size, bool enabled, int tile, int page,
            bool invertTileX, bool invertTileY, bool masked,
            int palette) :
            this(offsetX, offsetY, layer, size, enabled,
                new Mapping(tile, page, invertTileX, invertTileY, masked, palette))
        {
        }

        public static Sprite Clone(Sprite source)
        {
            using (var buffer = new MemoryStream())
            {
                source.Serialize(buffer);
                buffer.Seek(0, SeekOrigin.Begin);
                return Sprite.Deserialize(buffer);
            }
        }

        #region Serialization
        
        // |               4               |
        // |39 |38 |37 |36 |35 |34 |33 |32 |
        // |-------|---|---|---|-----------|
        // | LAYER |RXO|RYO| E |   SIZE    |
        // |-------|---|---|---|-----------|   

        // |               3               |               2               |
        // |31 |30 |29 |28 |27 |26 |25 |24 |23 |22 |21 |20 |19 |18 |17 |16 |
        // |-------------------------------|-------------------------------|
        // |            OFFSETX            |            OFFSETY            |
        // |---------------^---------------|---------------^---------------|

        // |               1               |               0               |
        // |15 |14 |13 |12 |11 |10 | 9 | 8 | 7 | 6 | 5 | 4 | 3 | 2 | 1 | 0 |
        // |-------------------------------|-------|---|---|---|-----------|
        // |              TILE             |  PAGE |ITX|ITY| M |  PALETTE  |
        // |---------------^---------------|-------|---|---|---|-----------|

        // 0 0 0 1x1
        // 0 0 1 2x2
        // 0 1 0 4x4
        // 0 1 1 8x8
        // 1 0 0 2x4
        // 1 0 1 4x2
        // 1 1 0 4x8
        // 1 1 1 8x4

        const int LayerMask = 0xC00000;
        const int ReverseOffsetXMask = 0x200000;
        const int ReverseOffsetYMask = 0x100000;
        const int EnabledMask = 0x080000;
        const int SizeMask = 0x070000;
        const int OffsetXMask = 0x00FF00;
        const int OffsetYMask = 0x0000FF;

        public void Serialize(Stream output)
        {
            mapping.Serialize(output);

            int encodedBytes =
                (Layer << 22) |
                (OffsetX < 0 ? ReverseOffsetXMask : 0) |
                (OffsetY < 0 ? ReverseOffsetYMask : 0) |
                (Enabled ? EnabledMask : 0) |
                (((int)size) << 16) |
                (Math.Abs(OffsetX) << 8) |
                (Math.Abs(OffsetY));

            output.WriteByte((byte)(encodedBytes));
            output.WriteByte((byte)(encodedBytes >> 8));
            output.WriteByte((byte)(encodedBytes >> 16));
        }

        public static Sprite Deserialize(Stream input)
        {
            var mapping = Mapping.Deserialize(input);

            int byte1 = input.ReadByte();
            if (byte1 == -1)
                throw new InvalidDataException("End of stream reached");
            int byte2 = input.ReadByte();
            if (byte2 == -1)
                throw new InvalidDataException("End of stream reached");
            int byte3 = input.ReadByte();
            if (byte3 == -1)
                throw new InvalidDataException("End of stream reached");

            int encodedBytes = (byte3 << 16) | (byte2 << 8) | byte1;

            return new Sprite(
                ((encodedBytes & ReverseOffsetXMask) == ReverseOffsetXMask ? -1 : 1) *
                ((encodedBytes & OffsetXMask) >> 8),
                ((encodedBytes & ReverseOffsetYMask) == ReverseOffsetYMask ? -1 : 1) *
                ((encodedBytes & OffsetYMask)),
                ((encodedBytes & LayerMask) >> 22),
                (SpriteSize)((encodedBytes & SizeMask) >> 16),
                (encodedBytes & EnabledMask) == EnabledMask,
                mapping);
        }

        #endregion
    }
}

#endif