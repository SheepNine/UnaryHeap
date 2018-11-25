using System;

namespace Disassembler
{
    class HeightMap
    {
        private byte[] data;

        public HeightMap(byte[] rom, int address)
        {
            data = new byte[256];
            Array.Copy(rom, address, data, 0, data.Length);
        }

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= 256)
                    throw new ArgumentOutOfRangeException("index");

                return data[index];
            }
        }
    }

    class TerrainMap
    {
        private byte[] data;

        public TerrainMap(byte[] rom, int address)
        {
            data = new byte[128];
            Array.Copy(rom, address, data, 0, data.Length);
        }

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= 256)
                    throw new ArgumentOutOfRangeException("index");

                var datum = data[index / 2];

                if (index % 2 == 0)
                    return (byte)(datum & 0x0F);
                else
                    return (byte)(datum >> 4);
            }
        }
    }

    class IndexMap
    {
        private byte[] data;
        private int size;

        public IndexMap(byte[] rom, int address, int size)
        {
            this.size = size;
            data = new byte[size * size];
            Array.Copy(rom, address, data, 0, data.Length);
        }

        public byte this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= size)
                    throw new ArgumentOutOfRangeException("x");
                if (y < 0 || y >= size)
                    throw new ArgumentOutOfRangeException("y");

                return data[x + size * y];
            }
        }

        public int Size
        {
            get { return size; }
        }
    }

    class TileSlice
    {
        byte[] data;
        public TileSlice(byte[] rom, int address)
        {
            data = new byte[4];
            Array.Copy(rom, address, data, 0, data.Length);
        }

        public byte this[int index]
        {
            get { return data[index]; }
        }
    }

    class BackgroundTileMap
    {
        public TileSlice Cliff { get; private set; }
        public TileSlice LedgeHigh { get; private set; }
        public TileSlice LedgeLow { get; private set; }
        public TileSlice ClipHigh { get; private set; }
        public TileSlice ClipLow { get; private set; }

        public TileSlice WallHigh { get; private set; }
        public TileSlice WallLow { get; private set; }

        public TileSlice Shore { get; private set; }

        public TileSlice[] Trims { get; private set; }
        public TileSlice[] Flats { get; private set; }

        public BackgroundTileMap(byte[] rom, int indexAddress)
        {
            Cliff = new TileSlice(rom, indexAddress + 0x1A);
            LedgeHigh = new TileSlice(rom, indexAddress + 0x1E);
            LedgeLow = new TileSlice(rom, indexAddress + 0x22);
            ClipHigh = new TileSlice(rom, indexAddress + 0x26);
            ClipLow = new TileSlice(rom, indexAddress + 0x2A);

            WallHigh = new TileSlice(rom, indexAddress + rom[indexAddress] + 4);
            WallLow = new TileSlice(rom, indexAddress + rom[indexAddress]);

            Shore = new TileSlice(rom, indexAddress + rom[indexAddress + 13]);

            Trims = new TileSlice[12];
            Flats = new TileSlice[12];

            for (int i = 1; i < 13; i++)
            {
                Flats[i - 1] = new TileSlice(rom, indexAddress + rom[indexAddress + i]);
                Trims[i - 1] = new TileSlice(rom, indexAddress + rom[indexAddress + i + 13]);
            }
        }
    }
}
