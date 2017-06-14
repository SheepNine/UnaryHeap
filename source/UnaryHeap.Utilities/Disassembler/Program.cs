using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disassembler
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var disassembler = new OpcodeDisassembler(File.OpenRead(args[0])))
                disassembler.Disassemble(0x8000, 0x10, 0x800F, Console.Out, new[] {
                        new Range(0x822B, 0x06),
                        new Range(0x869A, 0x06),
                        new Range(0x8B0E, 0xC0),
                        new Range(0x8C4D, 0x1C),
                        new Range(0x9026, 0x0C),
                        new Range(0x903F, 0x87),
                        new Range(0x94F5, 0x8F),
                        new Range(0x95F4, 0x0A),
                        new Range(0x9679, 0x16),
                        new Range(0x96C7, 0x2C),
                        new Range(0x9D21, 0x24),
                        new Range(0x9E57, 0x3E),
                        new Range(0x9F09, 0x14),
                        new Range(0xA49F, 0x3C),
                        new Range(0xA8AD, 0x334),
                        new Range(0xAEE6, 0x57),
                        new Range(0xAF71, 0x58),
                        new Range(0xB23F, 0x08),
                        new Range(0xB450, 0x4C),
                        new Range(0xB58D, 0x24),
                        new Range(0xB6D2, 0x0A),
                        new Range(0xB72C, 0x06),
                        new Range(0xB7F4, 0x06),
                        new Range(0xB860, 0x09),
                        new Range(0xB8BC, 0x09),
                        new Range(0xB94E, 0x0B),
                        new Range(0xB983, 0x01),
                        new Range(0xBA72, 0x0D),
                        new Range(0xBAB1, 0x0F),
                        new Range(0xBE2F, 0x08),
                        new Range(0xBE74, 0x0F),
                        new Range(0xBFB0, 0x8E),
                        new Range(0xC0B1, 0x04),
                        new Range(0xC1CA, 0x0D),
                        new Range(0xC2A5, 0x05),
                        new Range(0xC34C, 0x04),
                        new Range(0xC35B, 0x4E),
                        new Range(0xC410, 0x66),
                        new Range(0xC767, 0x20),
                        new Range(0xC894, 0x18),
                        new Range(0xC8E4, 0x12),
                        new Range(0xCBA2, 0x04),
                        new Range(0xCCCB, 0x0E),
                        new Range(0xCD3D, 0x0F),
                        new Range(0xCE50, 0xF0),
                        new Range(0xCF63, 0x26F),
                        new Range(0xD222, 0x23),
                        new Range(0xD2AA, 0x0F),
                        new Range(0xD2E7, 0x18),
                        new Range(0xD4A5, 0x14),
                        new Range(0xD5F3, 0x37),
                        new Range(0xD970, 0x2E),
                        new Range(0xDB35, 0x3E5),
                        new Range(0xE00F, 0x46),
                        new Range(0xE306, 0xBA),
                        new Range(0xE3C0, 0x1000),
                        new Range(0xF3C0, 0x15D),
                        new Range(0xF53A, 0x4F5),
                        new Range(0xFA2F, 0x02),
                        new Range(0xFC1F, 0x87),
                        new Range(0xFCD6, 0x08),
                        new Range(0xFFAC, 0x02),
                        new Range(0xFFFA, 0x06)
                    });
        }
    }
}
