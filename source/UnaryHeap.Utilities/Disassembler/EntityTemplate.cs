using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disassembler
{
    class EntityTemplate
    {
        public int Type { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public byte RenderAttributes { get; private set; }
        public byte ControlAttributes { get; private set; }
        public byte ODValue { get; private set; }


        public EntityTemplate(byte[] data, int offset)
        {
            var bytes = new List<byte>();
            for (int u = 0; u < 7; u++)
                bytes.Add(data[offset + u]);

            Type = bytes[0];
            X = (((bytes[1] & 0xE0) << 3) | bytes[2]) / 16.0;
            Y = (((bytes[1] & 0x1C) << 6) | bytes[3]) / 16.0;
            Z = (((bytes[1] & 0x03) << 9) | ((bytes[5] & 0x80) << 1) | bytes[4]) / 16.0;
            int attrs = bytes[5] & 0x7F;
            RenderAttributes = (byte)(attrs & 0x63);
            ControlAttributes = (byte)(attrs & 0x9C);
            ODValue = bytes[6];
        }

        public string Description
        {
            get { return string.Format("{0:X2}\r\n{1:X2}|{2:X2}\r\n{3:X2}",
                Type, RenderAttributes, ControlAttributes, ODValue); }
        }
    }
}
