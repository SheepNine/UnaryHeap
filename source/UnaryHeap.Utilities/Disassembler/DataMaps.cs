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
}
