using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disassembler
{
    class TileAttributeLookup
    {
        byte[] values;

        private TileAttributeLookup(byte[] fileData, int startAddress)
        {
            values = new byte[256];

            var i = startAddress;
            for (int n = 0; n < 256; n++)
            {
                if (fileData[Program.PrgRomFileOffset(i)] <= n)
                    i += 1;

                values[n] = fileData[Program.PrgRomFileOffset(i + 0x1A)];
            }

        }

        public static TileAttributeLookup FirstLookup(byte[] fileData)
        {
            return new TileAttributeLookup(fileData, 0xA49F);
        }

        public static TileAttributeLookup SecondLookup(byte[] fileData)
        {
            return new TileAttributeLookup(fileData, 0xA49F + 6);
        }

        public static TileAttributeLookup ThirdLookup(byte[] fileData)
        {
            return new TileAttributeLookup(fileData, 0xA49F + 18);
        }

        public byte this[int index]
        {
            get { return values[index]; }
        }
    }
}
