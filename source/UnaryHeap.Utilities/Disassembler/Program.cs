using System;
using System.IO;
using System.Linq;

namespace Disassembler
{
    class Program
    {
        static int PrgRomFileOffset(int prgRomAddress)
        {
            return prgRomAddress + 0x10 - 0x8000;
        }

        static int ChrRomFileOffset(int chrRomPage, int address)
        {
            return 0x8010 + 0x1000 * chrRomPage + address;
        }

        static void Main(string[] args)
        {
            LabelSet labels = new LabelSet();

            // KNOWN SUBROUTINES

            labels.Record(0xFF81, "NMI");
            labels.Record(0xFFF1, "RST");
            labels.Record(0xFF80, "IRQ_BRK");
            labels.Record(0x8010, "tk_8629");
            labels.Record(0x8019, "tk_B42A");
            labels.Record(0x801C, "tk_93BA");
            labels.Record(0x801F, "tk_E23A");
            labels.Record(0x8022, "tk_DFA8");
            labels.Record(0x802F, "NEW_ENT");
            labels.Record(0xFCF0, "tk_FE4E");
            labels.Record(0x8251, "RST_PT2");
            labels.Record(0x85C2, "RST_PT3");
            labels.Record(0xFFAE, "BSYWAIT");
            labels.Record(0xFFB5, "MP_CTRL");
            labels.Record(0xFFC9, "MP_B0");
            labels.Record(0xFFDD, "MP_B1");
            labels.Record(0x81EA, "QSFX_Pn");
            labels.Record(0x81EE, "QSFX_P0");
            labels.Record(0x81F2, "QSFX_P1");
            labels.Record(0x81FA, "QSFX_NZ");
            labels.Record(0x81FC, "QSFX");
            labels.Record(0xC3B7, "CRBLT5A");
            labels.Record(0xC3BB, "CRBLT06");
            labels.Record(0xC3D7, "CROMBLT");
            labels.Record(0x8242, "RST_PPU");
            labels.Record(0xE2C9, "HLFSTR");
            labels.Record(0x80B6, "PRTSTRS");
            labels.Record(0x80BC, "PTSTRSB");
            labels.Record(0x816C, "HDALSPR");
            labels.Record(0x816E, "HDRMSPR");
            labels.Record(0xE209, "DEL_ENT");
            labels.Record(0xD279, "RDCTRLR");
            labels.Record(0x8B07, "MAXTIMR");
            labels.Record(0xD2E1, "WAITVBL");
            labels.Record(0xD62A, "INITBGM");
            labels.Record(0x8DB7, "LOOPBGM");
            labels.Record(0xBF3A, "RUMBLE");
            labels.Record(0xD2C9, "RANDOM");

            labels.Record(0x8128, "CHNG_ST");
            labels.Record(0x8C0F, "ST_FADE");
            labels.Record(0x8C69, "ST_PLAY");
            labels.Record(0x8DC6, "ST_DDWN");
            labels.Record(0x84CC, "ST_MTTL");


            // UNKNOWN SUBROUTINES

            var unsubs = new[] {
                0x8013, 0x802F, 0x8064, 0x8068, 0x8080,
                0x80DB, 0x80DD, 0x80F7, 0x811D, 0x8150,
                0x817A, 0x817C, 0x817E, 0x8185, 0x8197,
                0x81A4, 0x81A7, 0x81C3, 0x81DB, 0x81DF,
                0x81EA, 0x8231, 0x845B, 0x8460, 0x8465,
                0x8468, 0x8480, 0x8629, 0x866D, 0x8689,
                0x86A0, 0x86AD, 0x86B6, 0x87E1, 0x899A,
                0x89DA, 0x89E3, 0x8A00, 0x8A1A, 0x8A3B,
                0x8A4E, 0x8A51, 0x8B07, 0x8BCE, 0x8BE1,
                0x8DAC, 0x8DB7, 0x9032, 0x90C6, 0x9139,
                0x9186, 0x918B, 0x9292, 0x92A2, 0x92BB,
                0x92BE, 0x93B2, 0x9422, 0x9432, 0x9584,
                0x9586, 0x95D8, 0x95DE, 0x95EB, 0x95FE,
                0x96F3, 0x9C3B, 0x9CDE, 0x9CF3, 0x9D14,
                0x9D45, 0x9D48, 0x9D54, 0x9DA3, 0x9DC5,
                0x9DFC, 0x9E95, 0x9F72, 0x9F9E, 0x9FB7,
                0x9FC6, 0x9FD2, 0xA197, 0xAC3E, 0xAE17,
                0xAE7F, 0xAFFA, 0xB1E5, 0xB231, 0xB247,
                0xB283, 0xB315, 0xB3CD, 0xB3D7, 0xB3D9,
                0xB408, 0xB441, 0xB527, 0xB529, 0xB539,
                0xB6DE, 0xB6E0, 0xB6E1, 0xB732, 0xB79D,
                0xB7CD, 0xB815, 0xB81D, 0xB848, 0xB869,
                0xB9A7, 0xBAFC, 0xBAFF, 0xBB88, 0xBB8F,
                0xBCFC, 0xBD65, 0xBD66, 0xBD77, 0xBE06,
                0xBE23, 0xBE83, 0xBF3A, 0xBF44, 0xBF4E,
                0xC03E, 0xC040, 0xC09F, 0xC162, 0xC165,
                0xC2AA, 0xC2EF, 0xC350, 0xC3A7, 0xC3A9,
                0xC3AB, 0xC3B1, 0xC476, 0xC480, 0xC552,
                0xC55C, 0xC567, 0xC578, 0xC57C, 0xC581,
                0xC592, 0xC62C, 0xC62E, 0xC64E, 0xC884,
                0xC8AC, 0xC8B9, 0xC9AF, 0xCA7B, 0xCC59,
                0xCCB5, 0xCCD7, 0xCD27, 0xCD4C, 0xCF40,
                0xD1D2, 0xD1D5, 0xD21A, 0xD245, 0xD26A,
                0xD279, 0xD2C0, 0xD2C9, 0xD2E1, 0xD4D2,
                0xD4E1, 0xD4F4, 0xD4FF, 0xD501, 0xD513,
                0xD523, 0xD525, 0xD527, 0xD529, 0xD54A,
                0xD62A, 0xD69A, 0xD6FB, 0xD8A1, 0xD8D1,
                0xD9E6, 0xDA63, 0xDF1A, 0xDF46, 0xDF48,
                0xDF55, 0xDF6F, 0xDF9B, 0xDF9E, 0xDFA8,
                0xE037, 0xE042, 0xE055, 0xE065, 0xE161,
                0xE17C, 0xE209, 0xE21E, 0xE230, 0xF51D,
                0xF523, 0xF527, 0xFA31, 0xFC1B, 0xFCA6,
                0xFCBA, 0xFCDE, 0xFCF3, 0xFE6C, 
            };
            foreach (var i in Enumerable.Range(0, unsubs.Length))
                labels.Record(unsubs[i], string.Format("SB_{0:X4}", unsubs[i]));



            // LOOPS

            var loopBranches = new[] {
                0x80BE, 0x80E7, 0x80EA, 0x8105, 0x8170, 0x81E3, 0x8233, 0x81CA, 0x82FA, 0x836B, 0x8387, 0x8395, 0x842D,
                0xC402, 0xD643, 0xD6DE, 0xE2D5, 0xF530, 0xFBB9, 0xD284
            };
            foreach (var i in Enumerable.Range(0, loopBranches.Length))
                labels.Record(loopBranches[i], string.Format("lp_{0:D3}", i));


            // SKIPS

            var skipBranches = new[] {
                0x80B3, 
                0x810F, 0x8144, 0x81E1,
                0x82B6, 0x82F2, 0x8379, 0x8361, 0x83A6, 0x83DE, 0x83F7, 0x8416, 0x841F, 0x8436, 0x8452,
                0x9D1C, 0x9D76, 0x9D86, 0x9D95, 0x9D97, 0x9D99, 0x9D9F,
                0xD564, 0xD578, 0xD5EF, 0xD6AA, 0xD6C8, 0xD6D8, 0xD6E6, 0xD6EE, 0xD702,
                0xE2F5, 
                0xFBA6, 0xFBCA, 0xFBD3, 0xFBE2, 0xFCB5, 0xFF61, 0xFFA6,
                0xBB85, 0xBBB2, 0xBBB5, 0xBC33, 0xBBF0, 0xBC04, 0xBC1B, 0xBC23, 0xBC4C, 0xBC6F,
                0xBC6C, 0xBC83, 0xBC76, 0xBC7E, 0xBC9F, 0xBC9C, 0xBCAE, 0xBCE3, 0xBC85, 0xBCA2,
                0xBCF0, 0xBD58, 0xBD17, 0xBD34, 0xBD74
            };
            foreach (var i in Enumerable.Range(0, skipBranches.Length))
                labels.Record(skipBranches[i], string.Format("sk_{0:D3}", i));


            // RETURNS

            var branchToRTSes = new[] {
                0x80F6, 0x816B, 0x8196, 0x81DA, 0x8208,
                0x89BC, 0x89FC, 0x8DB6, 0x9185, 0x9291, 0x9416, 0x9431,
                0x94F4, 0x9649, 0x9D1B, 0x9E0A, 0x9F08, 0x9F6C, 0xA196,
                0xAED5, 0xAFC9, 0xB23E, 0xB271, 0xB314, 0xB659, 0xB72B,
                0xB797, 0xB7CC, 0xB834, 0xB847, 0xB88D, 0xB94D, 0xBAFB,
                0xBB13, 0xBB87, 0xBDAE, 0xBE05, 0xBE2E, 0xBE8D, 0xBF39,
                0xC0F3, 0xC1C9, 0xC2EE, 0xC586, 0xC7C6, 0xC88C, 0xC8C6,
                0xCAEA, 0xCB29, 0xCB86, 0xCC1B, 0xCD3C, 0xCD9C, 0xCDC8,
                0xCDF5, 0xCE22, 0xCE4F, 0xD789, 0xD8A0, 0xD9F6, 0xE00B,
                0xE139, 0xE305, 0xFC1E, 0xFCD5, 0xFCEF, 0xFEA2, 0xFF1C,
            };
            foreach (var i in Enumerable.Range(0, branchToRTSes.Length))
                labels.Record(branchToRTSes[i], string.Format("rts_{0:D2}", i));

            using (var disassembler = new OpcodeDisassembler(File.OpenRead(args[0])))
            {
                var audioJumpVector = disassembler.ReadJumpVectorHiHiLoLo(PrgRomFileOffset(0xD970), 0x17);
                foreach (var i in Enumerable.Range(0, 0x17))
                    labels.Record(audioJumpVector[i], string.Format("SFX_{0:X2}", i));

                var aiJumpVector = disassembler.ReadJumpVectorLoHiLoHi(PrgRomFileOffset(0x8B0E), 0x40);
                foreach (var i in Enumerable.Range(0, 0x40))
                    labels.Record(aiJumpVector[i], string.Format("AI_{0:X2}", i));

                foreach (var output in new[] { TextWriter.Null, Console.Out }) {
                    // PRG ROM
                    disassembler.Disassemble(0x8000, PrgRomFileOffset(0x8000), 0x8000, output, labels, new Range[] {
                        new UnknownRange(0x822B, 0x06),
                        new UnknownRange(0x869A, 0x06),
                        new UnknownRange(0x8B0E, 0xC0),
                        new UnknownRange(0x8C4D, 0x1C),
                        new UnknownRange(0x9026, 0x0C),
                        new UnknownRange(0x903F, 0x87),
                        new UnknownRange(0x94F5, 0x8F),
                        new UnknownRange(0x95F4, 0x0A),
                        new UnknownRange(0x9679, 0x16),
                        new UnknownRange(0x96C7, 0x2C),
                        new UnknownRange(0x9D21, 0x24),
                        new UnknownRange(0x9E57, 0x3E),
                        new UnknownRange(0x9F09, 0x14),
                        new UnknownRange(0xA49F, 0x3C),
                        new UnknownRange(0xA8AD, 0x334),
                        new UnknownRange(0xAEE6, 0x57),
                        new UnknownRange(0xAF71, 0x58),
                        new UnknownRange(0xB23F, 0x08),
                        new UnknownRange(0xB450, 0x4C),
                        new UnknownRange(0xB58D, 0x24),
                        new UnknownRange(0xB6D2, 0x0A),
                        new UnknownRange(0xB72C, 0x06),
                        new UnknownRange(0xB7F4, 0x06),
                        new UnknownRange(0xB860, 0x09),
                        new UnknownRange(0xB8BC, 0x09),
                        new UnknownRange(0xB94E, 0x0B),
                        new UnknownRange(0xB983, 0x01),
                        new UnknownRange(0xBA72, 0x0D),
                        new UnknownRange(0xBAB1, 0x0F),
                        new UnknownRange(0xBE2F, 0x08),
                        new UnknownRange(0xBE74, 0x0F),
                        new UnknownRange(0xBFB0, 0x8E),
                        new UnknownRange(0xC0B1, 0x04),
                        new UnknownRange(0xC1CA, 0x0D),
                        new UnknownRange(0xC2A5, 0x05),
                        new UnknownRange(0xC34C, 0x04),
                        new UnknownRange(0xC35B, 0x4E),
                        new UnknownRange(0xC410, 0x66),
                        new UnknownRange(0xC767, 0x20),
                        new UnknownRange(0xC894, 0x18),
                        new UnknownRange(0xC8E4, 0x12),
                        new UnknownRange(0xCBA2, 0x04),
                        new UnknownRange(0xCCCB, 0x0E),
                        new UnknownRange(0xCD3D, 0x0F),
                        new UnknownRange(0xCE50, 0xF0),
                        new UnknownRange(0xCF63, 0x26F),
                        new UnknownRange(0xD222, 0x23),
                        new UnknownRange(0xD2AA, 0x0F),
                        new UnknownRange(0xD2E7, 0x18),
                        new UnknownRange(0xD4A5, 0x14),
                        new UnknownRange(0xD5F3, 0x37),
                        new UnknownRange(0xD970, 0x2E),
                        new UnknownRange(0xDB35, 0x3E5),
                        new UnknownRange(0xE00F, 0x46),
                        new UnknownRange(0xE306, 0xA0),
                        new StringRange(0xE3A6),
                        new StringRange(0xE3AE),
                        new StringRange(0xE3B4),
                        new UnknownRange(0xE3C0, 0x1000),
                        new UnknownRange(0xF3C0, 0x15D),
                        new UnknownRange(0xF53A, 0x4F5),
                        new UnknownRange(0xFA2F, 0x02),
                        new UnknownRange(0xFC1F, 0x87),
                        new UnknownRange(0xFCD6, 0x08),
                        new UnknownRange(0xFFAC, 0x02),
                        new UnknownRange(0xFFFA, 0x06)
                    });

                    output.WriteLine("\r\nBLIT $00\r\n");
                    // BLIT $00
                    disassembler.Disassemble(0x03FF, ChrRomFileOffset(3, 0x390), 0x400, output, labels, new Range[] {
                        new StringRange(0x0409),
                        new StringRange(0x0413),
                        new StringRange(0x041C),
                        new UnknownRange(0x04CD, 0x06),
                        new UnknownRange(0x0699, 0x0C),
                        new StringRange(0x076A),
                        new StringRange(0x0775),
                        new StringRange(0x0782),
                        new DescribedRange(0x78D, 0x0A, "Offsets from $0797"),
                        new StringRange(0x0797),
                        new StringRange(0x07A2),
                        new StringRange(0x07AD),
                        new StringRange(0x07B6),
                        new StringRange(0x07C0),
                        new StringRange(0x07C7),
                        new StringRange(0x07D4),
                        new StringRange(0x07E0),
                        new StringRange(0x07EC),
                        new StringRange(0x07F1),
                        new UnknownRange(0x07FA, 0x05)
                    });
                    output.WriteLine("\r\nBLIT $06\r\n");
                    // BLIT $06
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(3, 0x2B0), 0x100, output, labels, new[] {
                        new UnknownRange(0x0236, 0xA8),
                        new UnknownRange(0x02EA, 0x16)
                    });
                    output.WriteLine("\r\nBLIT $0C\r\n");
                    // BLIT $0C
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(3, 0xDF0), 0x8F, output, labels, new[] {
                        new UnknownRange(0x0230, 0x5F)
                    });
                    output.WriteLine("\r\nBLIT $12\r\n");
                    // BLIT $12
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(3, 0xE7F), 0xD4, output, labels, new UnknownRange[] {
                        new UnknownRange(0x0284, 0x50)
                    });
                    output.WriteLine("\r\nBLIT $18\r\n");
                    // BLIT $18
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(3, 0xF53), 0xAD, output, labels, new[] {
                        new UnknownRange(0x0749, 0x64)
                    });
                    output.WriteLine("\r\nBLIT $1E\r\n");
                    // BLIT $1E
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(3, 0xBA0), 0x100, output, labels, new[] {
                        new UnknownRange(0x078E, 0x12),
                        new UnknownRange(0x07C2, 0x3E)
                    });
                    output.WriteLine("\r\nBLIT $24\r\n");
                    // BLIT $24
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(5, 0x670), 0x100, output, labels, new UnknownRange[] {
                        new UnknownRange(0x7FE, 0x02)
                    });
                    output.WriteLine("\r\nBLIT $2A\r\n");
                    // BLIT $2A
                    disassembler.Disassemble(0x0600, ChrRomFileOffset(5, 0x770), 0xF4, output, labels, new Range[] {
                        new UnknownRange(0x66A, 0x30),
                        new StringRange(0x69A),
                        new StringRange(0x6A7),
                        new StringRange(0x6B4),
                        new StringRange(0x6C3),
                        new StringRange(0x6D3),
                        new StringRange(0x6DC),
                        new StringRange(0x6E2),
                        new UnknownRange(0x06F3, 0x01)
                    });
                    output.WriteLine("\r\nBLIT $30\r\n");
                    // BLIT $30
                    disassembler.Disassemble(0x0653, ChrRomFileOffset(5, 0x864), 0x9C, output, labels, new Range[] {
                        new StringRange(0x065C),
                        new StringRange(0x0666),
                        new StringRange(0x0670),
                        new StringRange(0x0680),
                        new StringRange(0x0690),
                        new StringRange(0x069B),
                        new StringRange(0x06A5),
                        new StringRange(0x06A9),
                        new StringRange(0x06B3),
                        new StringRange(0x06B7),
                        new StringRange(0x06C9),
                        new StringRange(0x06D3),
                        new StringRange(0x06DE),
                        new UnknownRange(0x06EC, 0x03)
                    });
                    output.WriteLine("\r\nBLIT $36\r\n");
                    // BLIT $36
                    disassembler.Disassemble(0x0653, ChrRomFileOffset(1, 0xB60), 0x60, output, labels, new[] {
                        new UnknownRange(0x06A9, 0x0A)
                    });
                    output.WriteLine("\r\nBLIT $3C\r\n");
                    // BLIT $3C
                    disassembler.Disassemble(0x06A0, ChrRomFileOffset(4, 0xB60), 0x60, output, labels, new UnknownRange[] {
                        new UnknownRange(0x06A0, 0x60)
                    });
                    output.WriteLine("\r\nBLIT $42\r\n");
                    // BLIT $42
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(7, 0x4C0), 0x80, output, labels, new UnknownRange[] {
                        new UnknownRange(0x0700, 0x80)
                    });
                    output.WriteLine("\r\nBLIT $48\r\n");
                    // BLIT $48
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(4, 0xCF0), 0x50, output, labels, new UnknownRange[] {
                        new UnknownRange(0x074C, 0x05)
                    });
                    output.WriteLine("\r\nBLIT $4E\r\n");
                    // BLIT $4E
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(5, 0xCF0), 0x50, output, labels, new UnknownRange[] {
                            new UnknownRange(0x0219, 0x1E),
                            new UnknownRange(0x024F, 0x01)
                    });
                    output.WriteLine("\r\nBLIT $54\r\n");
                    // BLIT $54
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(5, 0x560), 0x40, output, labels, new Range[] {
                        new DescribedRange(0x021A, 0x07, "Number of strings to print"),
                        new StringRange(0x0221),
                        new StringRange(0x0228),
                        new StringRange(0x0231),
                        new StringRange(0x023A),
                        new StringRange(0x023D)
                    });
                    output.WriteLine("\r\nBLIT $5A\r\n");
                    // BLIT $5A
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(2, 0xF07), 0xF9, output, labels, new[] {
                        new UnknownRange(0x0254, 0x23),
                        new UnknownRange(0x02F7, 0x03)
                    });
                    output.WriteLine("\r\nBLIT $60\r\n");
                    // BLIT $60
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(6, 0x846), 0xB0, output, labels, new UnknownRange[] {
                    });
                }
            }
        }
    }
}
