#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnaryHeap.Utilities.Retrographic
{
    class Mapping
    {
        // |               1               |               0               |
        // |15 |14 |13 |12 |11 |10 | 9 | 8 | 7 | 6 | 5 | 4 | 3 | 2 | 1 | 0 |
        // |-------------------------------|-------|---|---|---|-----------|
        // |              TILE             |  PAGE |ITX|ITY| M |  PALETTE  |
        // |---------------^---------------|-------|---|---|---|-----------|
        const int TileMask = 0xFF00;
        const int PageMask = 0x00C0;
        const int InvertTileXMask = 0x0020;
        const int InvertTileYMask = 0x0010;
        const int MaskedMask = 0x0008;
        const int PaletteMask = 0x0007;

        public int Tile { get; private set; }
        public int Page { get; private set; }
        public bool InvertTileX { get; private set; }
        public bool InvertTileY { get; private set; }
        public bool Masked { get; private set; }
        public int Palette { get; private set; }

        public Mapping(int tile, int page, bool invertTileX, bool invertTileY,
            bool masked, int palette)
        {
            if (tile < 0 || tile >= 256)
                throw new ArgumentOutOfRangeException("tile");
            if (page < 0 || page >= 4)
                throw new ArgumentOutOfRangeException("page");
            if (palette < 0 || palette >= 8)
                throw new ArgumentOutOfRangeException("palette");

            Tile = tile;
            Page = page;
            InvertTileX = invertTileX;
            InvertTileY = invertTileY;
            Masked = masked;
            Palette = palette;
        }

        public void Serialize(Stream output)
        {
            int encodedBytes =
                (Tile << 8) |
                (Page << 6) |
                (InvertTileX ? InvertTileXMask : 0) |
                (InvertTileY ? InvertTileYMask : 0) |
                (Masked ? MaskedMask : 0) |
                (Palette);

            output.WriteByte((byte)(encodedBytes));
            output.WriteByte((byte)(encodedBytes >> 8));
        }

        public static Mapping Deserialize(Stream input)
        {
            int byte1 = input.ReadByte();
            if (byte1 == -1)
                throw new InvalidDataException("End of stream reached");
            int byte2 = input.ReadByte();
            if (byte2 == -1)
                throw new InvalidDataException("End of stream reached");

            int encodedBytes = (byte2 << 8) | byte1;
            return new Mapping(
                (encodedBytes & TileMask) >> 8,
                (encodedBytes & PageMask) >> 6,
                (encodedBytes & InvertTileXMask) == InvertTileXMask,
                (encodedBytes & InvertTileYMask) == InvertTileYMask,
                (encodedBytes & MaskedMask) == MaskedMask,
                (encodedBytes & PaletteMask)
            );
        }
    }
}

#endif