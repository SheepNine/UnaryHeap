#if INCLUDE_WORK_IN_PROGRESS_RETROGRAPHIC

using System;
using System.IO;

namespace UnaryHeap.Utilities.Retrographic
{
    class BackgroundControl
    {
        //|               0               |
        //| 7 | 6 | 5 | 4 | 3 | 2 | 1 | 0 |
        //|---|---|-----------|-----------|
        //| V | D |  OFFSETX  |  OFFSETY  |
        //|---|---|-------^---|-----------|
        const int VisibleMask = 0x80;
        const int DrawOverSpritesMask = 0x40;
        const int OffsetXMask = 0x38;
        const int OffsetYMask = 0x07;

        public bool Visible { get; private set; }
        public bool DrawOverSprites { get; private set; }
        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }

        public BackgroundControl(bool visible, bool drawOverSprites, int offsetX, int offsetY)
        {
            if (offsetX < 0 || offsetX > 7)
                throw new ArgumentOutOfRangeException("offsetX");
            if (offsetY < 0 || offsetY > 7)
                throw new ArgumentOutOfRangeException("offsetY");

            Visible = visible;
            DrawOverSprites = drawOverSprites;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public static BackgroundControl Clone(BackgroundControl source)
        {
            using (var buffer = new MemoryStream())
            {
                source.Serialize(buffer);
                buffer.Seek(0, SeekOrigin.Begin);
                return BackgroundControl.Deserialize(buffer);
            }
        }

        public void Serialize(Stream output)
        {
            int encodedByte =
                (Visible ? VisibleMask : 0) |
                (DrawOverSprites ? DrawOverSpritesMask : 0 ) |
                (OffsetX << 3) |
                (OffsetY);

            output.WriteByte((byte)encodedByte);
        }

        public static BackgroundControl Deserialize(Stream input)
        {
            int encodedByte = input.ReadByte();
            if (encodedByte == -1)
                throw new InvalidDataException("End of stream reached");

            return new BackgroundControl(
                (encodedByte & VisibleMask) == VisibleMask,
                (encodedByte & DrawOverSpritesMask) == DrawOverSpritesMask,
                (encodedByte & OffsetXMask) >> 3,
                (encodedByte & OffsetYMask));
        }
    }
}

#endif