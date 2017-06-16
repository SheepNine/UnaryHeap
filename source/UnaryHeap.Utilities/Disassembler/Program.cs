using System;
using System.IO;
using System.Linq;

namespace Disassembler
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var disassembler = new OpcodeDisassembler(File.OpenRead(args[0])))
            {
                // PRG ROM
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

                // BLIT $00
                disassembler.Disassemble(0x03FF, 0xB3A0, 0xB79F, Console.Out, new [] {
                    new Range(0x0409, 0x1C),
                    new Range(0x04CD, 0x06),
                    new Range(0x0699, 0x0C),
                    new Range(0x076A, 0x90),
                    new Range(0x07FA, 0x05)
                });
                // BLIT $06
                disassembler.Disassemble(0x0200, 0xB2C0, 0xB3BF, Console.Out, new [] {
                    new Range(0x0236, 0xA8),
                    new Range(0x02EA, 0x16)
                });
                // BLIT $0C
                disassembler.Disassemble(0x0200, 0xBE00, 0xBE8E, Console.Out, new [] {
                    new Range(0x0230, 0x5F)
                });
                // BLIT $12
                disassembler.Disassemble(0x0200, 0xBE8F, 0xBF62, Console.Out, new Range[] {
                    new Range(0x0284, 0x50)
                });
                // BLIT $18
                disassembler.Disassemble(0x0700, 0xBF63, 0xC00F, Console.Out, new [] {
                    new Range(0x0749, 0x64)
                });
                // BLIT $1E
                disassembler.Disassemble(0x0700, 0xBBB0, 0xBCAF, Console.Out, new [] {
                    new Range(0x078E, 0x12),
                    new Range(0x07C2, 0x3E)
                });
                // BLIT $24
                disassembler.Disassemble(0x0700, 0xD680, 0xD77F, Console.Out, new Range[] {
                    new Range(0x7FE, 0x02)
                });
                // BLIT $2A
                disassembler.Disassemble(0x0600, 0xD780, 0xD873, Console.Out, new Range[] {
                    new Range(0x66A, 0x8A)
                });
                // BLIT $30
                disassembler.Disassemble(0x0653, 0x0D874, 0x0D910, Console.Out, new Range[] {
                    new Range(0x065C, 0x90)
                });
                // BLIT $36
                disassembler.Disassemble(0x0653, 0x9B70, 0x9BCF, Console.Out, new [] {
                    new Range(0x06A9, 0x0A)
                });
                // BLIT $3C
                disassembler.Disassemble(0x06A0, 0xCB70, 0xCBCF, Console.Out, new Range[] {
                    new Range(0x06A0, 0x60)
                });
                // BLIT $42
                disassembler.Disassemble(0x0700, 0xF4D0, 0xF54F, Console.Out, new Range[] {
                    new Range(0x0700, 0x80)
                });
                // BLIT $48
                disassembler.Disassemble(0x0700, 0xCD00, 0xCD4F, Console.Out, new Range[] {
                    new Range(0x074C, 0x05)
                });
                // BLIT $4E
                disassembler.Disassemble(0x0200, 0xDD00, 0xDD4F, Console.Out, new Range[] {
                        new Range(0x0219, 0x1E),
                        new Range(0x024F, 0x01)
                });
                // BLIT $54
                disassembler.Disassemble(0x0200, 0xD570, 0xD5AF, Console.Out, new [] {
                    new Range(0x021A, 0x07),
                    new Range(0x0221, 0x19),
                    new Range(0x023A, 0x03),
                    new Range(0x023D, 0x03)
                });
                // BLIT $5A
                disassembler.Disassemble(0x0200, 0xAF17, 0xB00F, Console.Out, new [] {
                    new Range(0x0254, 0x23),
                    new Range(0x02F7, 0x03)
                });
                // BLIT $60
                disassembler.Disassemble(0x0700, 0xE856, 0xE905, Console.Out, new Range[] {
                });
            }
        }
    }
}
