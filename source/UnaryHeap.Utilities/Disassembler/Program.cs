using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Disassembler
{
    class Program
    {
        const bool CreateGraphicalOutputs = false;

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
            labels.Record(0xC350, "NAGTRUN");

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

            var comments = new Comments();
            var inlineComments = new InlineComments();

            var fileData = File.ReadAllBytes(args[0]);
            ProduceHackedRom(fileData, AppendSuffix(args[0], " - slow BigFoot start on level 11"), (data) =>
            {
                // Maximize delay between BigFoot steps
                foreach (var i in Enumerable.Range(0, 11))
                    data[PrgRomFileOffset(0xBE78 + i)] = 0xFF;

                HackStartingLevel(data, 11);
            });

            if (CreateGraphicalOutputs.Equals(true))
            {
                var palette = new[]
                {
                    Color.Black,
                    Color.Red,
                    Color.Orange,
                    Color.White
                };
                    for (var pageIndex = 0; pageIndex < 8; pageIndex++)
                    {
                        using (var raster = Pattern.RasterizeChrRomPage(fileData, ChrRomFileOffset(pageIndex, 0), palette))
                            raster.Save(string.Format("ChrRomPage{0}.png", pageIndex), ImageFormat.Png);
                    }


                    DumpArrangement(fileData, ChrRomFileOffset(3, 0x2E6), "SNAKE.arr");
                    DumpArrangement(fileData, ChrRomFileOffset(3, 0x33E), "Rattle.arr");
                    DumpArrangement(fileData, ChrRomFileOffset(3, 0x366), "Roll.arr");
                    DumpArrangement(fileData, ChrRomFileOffset(5, 0x7EE), "Mountain.arr");
                    DumpArrangement(fileData, ChrRomFileOffset(5, 0x7DA), "Moon.arr");

                    var spritePalette = new Color[] {
                    Color.Transparent,
                    Color.FromArgb(0xAA, 0x00, 0x00),
                    Color.FromArgb(0xFF, 0x55, 0x55),
                    Color.FromArgb(0xFF, 0xFF, 0x55),
                };

                DumpSprites(fileData, spritePalette);
                DumpDynamicSprites(fileData, spritePalette);
            }

            using (var disassembler = new OpcodeDisassembler(new MemoryStream(fileData)))
            using (var outputFile = File.CreateText("disassembly.txt"))
            {
                var audioJumpVector = disassembler.ReadJumpVectorHiHiLoLo(PrgRomFileOffset(0xD970), 0x17);
                foreach (var i in Enumerable.Range(0, 0x17))
                    labels.Record(audioJumpVector[i], string.Format("SFX_{0:X2}", i));

                labels.Record(0xAFCA, "AI_PSPN");
                labels.Record(0xAFEA, "AI_PBLY");
                labels.Record(0xB272, "AI_SPLT");
                var aiJumpVector = disassembler.ReadJumpVectorLoHiLoHi(PrgRomFileOffset(0x8B0E), 0x40);
                foreach (var i in Enumerable.Range(0, 0x40))
                    labels.Record(aiJumpVector[i], string.Format("AI_{0:X2}", i));

                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    // PRG ROM
                    disassembler.Disassemble(0x8000, PrgRomFileOffset(0x8000), 0x8000, output, labels, comments, inlineComments, new Range[] {
                        new DescribedRange(0x822B, 0x06, "Template for SFX $2A 'score rollup (pulse)'"),
                        new DescribedRange(0x869A, 0x06, "Unknown data, accessed in triples", 3),
                        new DescribedRange(0x8B0E, 0x80, "AI jump table", 2),
                        new DescribedRange(0x8B8E, 0x40, "Entity control bits(?)"),
                        new UnknownRange(0x8C4D, 0x0F),
                        new DescribedRange(0x8C5C, 0x0B, "Song list by level"),
                        new UnknownRange(0x8C67, 0x2),
                        new UnknownRange(0x9026, 0x0C),
                        new DescribedRange(0x903F, 0x60, "'Drop down' string lists (1 + 6n + $00 format)"),
                        new DescribedRange(0x909F, 0x04, "'PLAY'"),
                        new DescribedRange(0x90A3, 0x02, "'ON'"),
                        new DescribedRange(0x90A5, 0x03, "'ALL'"),
                        new DescribedRange(0x90A8, 0x04, "'GONE'"),
                        new DescribedRange(0x90AC, 0x04, "'TIME'"),
                        new DescribedRange(0x90B0, 0x03, "'OUT'"),
                        new DescribedRange(0x90B3, 0x04, "'GAME'"),
                        new DescribedRange(0x90B7, 0x04, "'OVER'"),
                        new DescribedRange(0x90BB, 0x03, "'#UP'"),
                        new DescribedRange(0x90BE, 0x04, "'LEFT'"),
                        new DescribedRange(0x90C2, 0x04, "'HOLD'"),
                        new UnknownRange(0x94F5, 0x10),
                        new DescribedRange(0x9505, 0x09, "Song -> CHR ROM page lookup"),
                        new UnknownRange(0x950E, 0x07),
                        new UnknownRange(0x9515, 0x0C),
                        new UnknownRange(0x9521, 0x0C),
                        new UnknownRange(0x952D, 0x18),
                        new UnknownRange(0x9545, 0x18),
                        new UnknownRange(0x955D, 0x22),
                        new UnknownRange(0x957F, 0x05),
                        new DescribedRange(0x95F4, 0x05, "Snake pain animation cycle"),
                        new DescribedRange(0x95F9, 0x05, "Snake pain animation attribute cycle"),
                        new UnknownRange(0x9679, 0x16),
                        new DescribedRange(0x96C7, 0x2C, "Pre-recorded input for snakes entering level"),
                        new UnknownRange(0x9D21, 0x24),
                        new SpriteLayoutRange(0x9E57, "6E Fish tail"),
                        new SpriteLayoutRange(0x9E62, "6D Clock"),
                        new SpriteLayoutRange(0x9E6F, "94 Metal tree 2"),
                        new SpriteLayoutRange(0x9E79, "95 Metal tree 3"),
                        new SpriteLayoutRange(0x9E86, "96 Metal tree 4"),
                        new DescribedRange(0x9F09, 0x14, "Water jet animation cycle"),
                        new UnknownRange(0xA49F, 0x1A),
                        new UnknownRange(0xA4B9, 0x1A),
                        new UnknownRange(0xA4D3, 0x08),
                        new UnknownRange(0xA8AD, 0x0C),
                        new UnknownRange(0xA8B9, 0x0C),
                        new UnknownRange(0xA8C5, 0x31C),
                        new UnknownRange(0xAED6, 0x10),
                        new UnknownRange(0xAEE6, 0x08),
                        new UnknownRange(0xAEEE, 0x08),
                        new UnknownRange(0xAEF6, 0x14),
                        new SpriteLayoutRange(0xAF0A, "97 Ice cube 1"),
                        new SpriteLayoutRange(0xAF17, "98 Ice cube 2"),
                        new SpriteLayoutRange(0xAF24, "99 Bubble"),
                        new SpriteLayoutRange(0xAF31, "9A Icy pibbly hole"),
                        new DescribedRange(0xAF71, 0x4, "Lid animation cycle"),
                        new DescribedRange(0xAF75, 0x4, "Lid animation attribute cycle"),
                        new DescribedRange(0xAF79, 0x04, "Offset from $AF7D"),
                        new DescribedRange(0xAF7D, 0x0E, "Bonus 1 pibbly path", 2),
                        new DescribedRange(0xAF8B, 0x10, "Bonus 2 pibbly path", 2),
                        new DescribedRange(0xAF9B, 0x12, "Bonus 3 pibbly path", 2),
                        new DescribedRange(0xAFAD, 0x0C, "Bonus 4 pibbly path", 2),
                        new DescribedRange(0xAFBC, 0x0A, "Nibbly pibbly entity types by level"),
                        new UnknownRange(0xB23F, 0x04),
                        new UnknownRange(0xB243, 0x04),
                        new DescribedRange(0xB450, 0x06, "100"),
                        new DescribedRange(0xB456, 0x06, "200"),
                        new DescribedRange(0xB45C, 0x06, "300"),
                        new DescribedRange(0xB462, 0x06, "750"),
                        new DescribedRange(0xB468, 0x06, "500"),
                        new DescribedRange(0xB46E, 0x06, "150"),
                        new DescribedRange(0xB474, 0x06, "1000"),
                        new DescribedRange(0xB47A, 0x06, "5000"),
                        new DescribedRange(0xB480, 0x06, "1"),
                        new DescribedRange(0xB486, 0x06, "-1"),
                        new DescribedRange(0xB48C, 0x06, "17"),
                        new DescribedRange(0xB492, 0x06, "-17"),
                        new UnknownRange(0xB498, 0x04),
                        new UnknownRange(0xB58D, 0x08),
                        new DescribedRange(0xB595, 0x10, "Pibblejogger animation cycle", 4),
                        new DescribedRange(0xB5A5, 0x08, "Pibblebat animation cycle", 2),
                        new DescribedRange(0xB5AD, 0x4, "Pibblecopter animation cycle"),
                        new UnknownRange(0xB6D2, 0x0A),
                        new DescribedRange(0xB72C, 0x06, "Snakedozer animation cycle"),
                        new DescribedRange(0xB7F4, 0x06, "Bladez animation cycle"),
                        new DescribedRange(0xB860, 0x09, "Metal tree animation cycle"),
                        new DescribedRange(0xB8BC, 0x09, "Splash animation cycle"),
                        new DescribedRange(0xB94E, 0x0B, "Shark animation cycle"),
                        new UnknownRange(0xB983, 0x01),
                        new UnknownRange(0xBA72, 0x0D),
                        new DescribedRange(0xBAB1, 0x0F, "Pin cushion animation cycle"),
                        new DescribedRange(0xBE2F, 0x08, "Explosion animation cycle"),
                        new UnknownRange(0xBE74, 0x04),
                        new DescribedRange(0xBE78, 0x0B, "BigFoot stomp delay by level"),
                        new DescribedRange(0xBFB0, 0x0B, "Index into $BFBB by level"),
                        new DescribedRange(0xBFBB, 0x10, "Level 1 BigFoot Path"),
                        new DescribedRange(0xBFCB, 0x09, "Level 2 BigFoot Path"),
                        new DescribedRange(0xBFD4, 0x09, "Level 3 BigFoot Path"),
                        new DescribedRange(0xBFDD, 0x11, "Level 4 BigFoot Path"),
                        new DescribedRange(0xBFEE, 0x0C, "Level 5 BigFoot Path"),
                        new DescribedRange(0xBFFA, 0x1A, "Level 6 BigFoot Path"),
                        new DescribedRange(0xC014, 0x16, "Level 7 BigFoot Path"),
                        new DescribedRange(0xC02A, 0x06, "Level 9,10 BigFoot Path"),
                        new DescribedRange(0xC030, 0x0E, "Level 11 BigFoot Path"),
                        new UnknownRange(0xC0B1, 0x04),
                        new DescribedRange(0xC1CA, 0x05, "Powerup sprite layouts"),
                        new DescribedRange(0xC1CF, 0x08, "Powerup SFX (indexed from $C162)"),
                        new UnknownRange(0xC2A5, 0x05),
                        new UnknownRange(0xC34C, 0x04),
                        new DescribedRange(0xC35B, 0x06, "RLE-encoded PPU attribute table 00", 2),
                        new DescribedRange(0xC361, 0x06, "RLE-encoded PPU attribute table 06", 2),
                        new DescribedRange(0xC367, 0x16, "RLE-encoded PPU attribute table 0C", 2),
                        new DescribedRange(0xC37D, 0x14, "RLE-encoded PPU attribute table 22", 2),
                        new DescribedRange(0xC391, 0x0C, "RLE-encoded PPU attribute table 36", 2),
                        new DescribedRange(0xC39D, 0x04, "RLE-encoded PPU attribute table 42?", 2),
                        new DescribedRange(0xC3A1, 0x06, "RLE-encoded PPU attribute table 46?", 2),
                        new DescribedRange(0xC410, 0x66, "CHR ROM blit index", 6),
                        new DescribedRange(0xC767, 0x20, "Record hop cycle"),
                        new DescribedRange(0xC894, 0x08, "Sprite layouts for score values"),
                        new UnknownRange(0xC89C, 0x10),
                        new UnknownRange(0xC8E4, 0x12),
                        new UnknownRange(0xCBA2, 0x04),
                        new DescribedRange(0xCCCB, 0x04, "Pibblefish animation cycle"),
                        new UnknownRange(0xCCCF, 0x08),
                        new UnknownRange(0xCD3D, 0x0F),
                        new UnknownRange(0xCE50, 0xF0),
                        new SpriteLayoutRange(0xCF63, "93 Metal tree 1"),
                        new DescribedRange(0xCF6A, 0x80, "Map tile types, non-ice", 8),
                        new DescribedRange(0xCFEA, 0x80, "Map tile types, ice", 8),
                        new DescribedRange(0xD06A, 0x100, "Map tile heights", 16),
                        new SpriteLayoutRange(0xD16A, "8D Magic carpet 1"),
                        new SpriteLayoutRange(0xD17B, "8E Magic carpet 2"),
                        new SpriteLayoutRange(0xD18C, "8F 90 Water jet 1"),
                        new SpriteLayoutRange(0xD19D, "91 Water jet 2"),
                        new SpriteLayoutRange(0xD1B6, "92 Water jet 3"),
                        new SpriteLayoutRange(0xD222, "70 Tongue extension"),
                        new SpriteLayoutRange(0xD22B, "5E Disk/sphere/snowball B"),
                        new SpriteLayoutRange(0xD238, "12 Bell"),
                        new UnknownRange(0xD2AA, 0x0F),
                        new UnknownRange(0xD2E7, 0x18),
                        new UnknownRange(0xD4A5, 0x0A),
                        new UnknownRange(0xD4AF, 0x0A),
                        new DescribedRange(0xD5F3, 0x17, "Audio opcode operand count"),
                        new DescribedRange(0xD60A, 0x17, "Audio opcode read function low byte ($80xx)"),
                        new DescribedRange(0xD621, 0x09, "Song tempos"),
                        new DescribedRange(0xD970, 0x17, "Audio opcode jump vector high byte"),
                        new DescribedRange(0xD987, 0x17, "Audio opcode jump vector low byte"),
                        new DescribedRange(0xDB35, 0x44, "Musical note periods >= 256", 2),
                        new DescribedRange(0xDB79, 0x1B, "Musical note periods < 256"),
                        new DescribedRange(0xDB94, 0x52, "Sound effect address lookup", 2),
                        new DescribedRange(0xDBE6, 0x0D, "SFX $4C Asteriod fall"),
                        new DescribedRange(0xDBF3, 0x08, "SFX $4A Rocket take-off"),
                        new DescribedRange(0xDBFB, 0x15, "SFX $02 Explosion"),
                        new DescribedRange(0xDC10, 0x09, "SFX $04 Pibbly chomp"),
                        new DescribedRange(0xDC19, 0x14, "SFX $06 Pibbly ejection"),
                        new DescribedRange(0xDC2D, 0x07, "SFX $08 Lid opening"),
                        new DescribedRange(0xDC34, 0x0B, "SFX $0A ????"),
                        new DescribedRange(0xDC3F, 0x15, "SFX $0C PLAY ON/1-UP"),
                        new DescribedRange(0xDC54, 0x11, "SFX $44 Tail fin pickup"),
                        new DescribedRange(0xDC65, 0x09, "SFX $0E Control inverter pickup"),
                        new DescribedRange(0xDC6E, 0x0D, "SFX $10 Tongue extender pickup"),
                        new DescribedRange(0xDC7B, 0x23, "SFX $12 Wormhole opening"),
                        new DescribedRange(0xDC9E, 0x10, "SFX $14 Wormhole sucking object"),
                        new DescribedRange(0xDCAE, 0x10, "SFX $16 BigFoot stomp/THUD"),
                        new DescribedRange(0xDCBE, 0x08, "SFX $18 Scale bell ring"),
                        new DescribedRange(0xDCC6, 0x0B, "SFX $2E Jaws (slow)"),
                        new DescribedRange(0xDCD1, 0x0B, "SFX $50 Jaws (fast)"),
                        new DescribedRange(0xDCDC, 0x09, "SFX $1A Exploding enemy 1"),
                        new DescribedRange(0xDCE5, 0x09, "SFX $1C Exploding enemy 2"),
                        new DescribedRange(0xDCEE, 0x09, "SFX $1E Snake OW"),
                        new DescribedRange(0xDCF7, 0x17, "SFX $20 Invincibility diamond pickup"),
                        new DescribedRange(0xDD0E, 0x19, "SFX $22 Snake death spin"),
                        new DescribedRange(0xDD27, 0x0E, "SFX $26 Pibbly countdown (medium)"),
                        new DescribedRange(0xDD35, 0x0E, "SFX $24 Pibbly countdown (low)"),
                        new DescribedRange(0xDD43, 0x0E, "SFX $28 Pibbly countdown (high)"),
                        new DescribedRange(0xDD51, 0x12, "SFX $2C Score rollup (noise)"),
                        new DescribedRange(0xDD63, 0x11, "SFX $30 ARRRGGG-"),
                        new DescribedRange(0xDD74, 0x1A, "SFX $32 Pibbly chunk spit"),
                        new DescribedRange(0xDD8E, 0x10, "SFX $36 Exit door point score"),
                        new DescribedRange(0xDD9E, 0x05, "SFX $38 Bounce/lick enemy"),
                        new DescribedRange(0xDDA3, 0x1A, "SFX $3A Lick foot"),
                        new DescribedRange(0xDDBD, 0x12, "SFX $3C Diving splash"),
                        new DescribedRange(0xDDCF, 0x16, "SFX $3E Water jump jet"),
                        new DescribedRange(0xDDE5, 0x05, "SFX $40 Time extension pickup"),
                        new DescribedRange(0xDDEA, 0x11, "SFX $42 Time running out beep"),
                        new DescribedRange(0xDDFB, 0x0B, "SFX $46 Wind-up key pickup"),
                        new DescribedRange(0xDE06, 0x07, "SFX $48 ???"),
                        new DescribedRange(0xDE0D, 0x05, "SFX $4E Snake gulp"),
                        new SpriteLayoutRange(0xDE12, "89 Shark (teeth)"),
                        new SpriteLayoutRange(0xDE26, "8A Shark (shadow) "),
                        new DescribedRange(0xDE42, 0x90, "Music index", 0x10),
                        new SpriteLayoutRange(0xDED2, "8B Shark (mouth closing)"),
                        new SpriteLayoutRange(0xDEF6, "8C Shark (mouth closed)"),
                        //new SpriteLayoutRange(0xDF1A, "56"), // layout index 56 points at program code
                        new UnknownRange(0xDE12, 0x108),
                        new UnknownRange(0xE00F, 0x28),
                        new DescribedRange(0xE306, 0xA0, "Character tile map", 4),
                        new StringRange(0xE3A6),
                        new StringRange(0xE3AE),
                        new StringRange(0xE3B4),
                        new DescribedRange(0xE3C0, 0x1000, "Snake mountain map", 64),
                        new SpriteLayoutRange(0xF3C0, "57 Mushroom 1"),
                        new SpriteLayoutRange(0xF3CD, "58 Mushroom 2"),
                        new SpriteLayoutRange(0xF3DA, "59 Anvil"),
                        new DescribedRange(0xF3E7, 0x9B, "Sprite layout index (high)"),
                        new DescribedRange(0xF482, 0x9B, "Sprite layout index (low)"),
                        new SpriteLayoutRange(0xF53A, "06 Snake SW"),
                        new SpriteLayoutRange(0xF547, "07 6F Snake smile SW / extra continue"),
                        new SpriteLayoutRange(0xF554, "55 Snake something?"),
                        new SpriteLayoutRange(0xF55D, "08 Snake S"),
                        new SpriteLayoutRange(0xF56A, "09 Snake smile S"),
                        new SpriteLayoutRange(0xF577, "04 Snake W"),
                        new SpriteLayoutRange(0xF584, "05 Snake smile W"),
                        new SpriteLayoutRange(0xF591, "02 Snake NW"),
                        new SpriteLayoutRange(0xF59E, "03 Snake smile NW"),
                        new SpriteLayoutRange(0xF5AB, "00 Snake N"),
                        new SpriteLayoutRange(0xF5B8, "01 18 19 Snake smile N"),
                        new SpriteLayoutRange(0xF5C5, "1A 1B 1C 1D 1E 1F Snake something?"),
                        new SpriteLayoutRange(0xF5D2, "20 21 Snake something?"),
                        new SpriteLayoutRange(0xF5E6, "7D Snake squished"),
                        new SpriteLayoutRange(0xF5F1, "10 Snake swim"),
                        new SpriteLayoutRange(0xF5FE, "0A Pibblyjogger 1"),
                        new SpriteLayoutRange(0xF60B, "0B Pibblyjogger 2"),
                        new SpriteLayoutRange(0xF615, "0C Pibblyjogger 3"),
                        new SpriteLayoutRange(0xF622, "0D Pibblyjogger 4"),
                        new SpriteLayoutRange(0xF62F, "0E Pibblyjogger 5"),
                        new SpriteLayoutRange(0xF639, "0F Pibblyjogger 6"),
                        new SpriteLayoutRange(0xF646, "11 13 Pibball"),
                        new SpriteLayoutRange(0xF64D, "14 Disk/sphere/snowball A"),
                        new SpriteLayoutRange(0xF65A, "15 Lid 1"),
                        new SpriteLayoutRange(0xF66E, "16 Lid 2"),
                        new SpriteLayoutRange(0xF67B, "17 Lid 3"),
                        new SpriteLayoutRange(0xF687, "22 Pibblesplat 1"),
                        new SpriteLayoutRange(0xF693, "23 Pibblesplat 2"),
                        new SpriteLayoutRange(0xF69C, "24 Pibblesplat 3"),
                        new SpriteLayoutRange(0xF6A5, "25 Pibblesplat 4"),
                        new SpriteLayoutRange(0xF6AE, "26 Pibblesplat 5"),
                        new SpriteLayoutRange(0xF6B7, "27 Door closed"),
                        new SpriteLayoutRange(0xF6C8, "28 Door opening"),
                        new SpriteLayoutRange(0xF6DE, "29 Door open"),
                        new SpriteLayoutRange(0xF6F4, "2A Scale marker"),
                        new SpriteLayoutRange(0xF6FB, "2B Scale ringing 1"),
                        new SpriteLayoutRange(0xF713, "2C Scale ringing 2"),
                        new SpriteLayoutRange(0xF72B, "2D Pibblebat 1"),
                        new SpriteLayoutRange(0xF736, "2E Pibblebat 2"),
                        new SpriteLayoutRange(0xF741, "2F Pibblebat 3"),
                        new SpriteLayoutRange(0xF74C, "30 Pibblebat 4"),
                        new UnknownRange(0xF757, 0x02),
                        new SpriteLayoutRange(0xF755, "31 4A Blank"),
                        new SpriteLayoutRange(0xF759, "32 Snakedozer retracted"),
                        new SpriteLayoutRange(0xF765, "33 Snakedozer extending"),
                        new SpriteLayoutRange(0xF772, "34 Snakedozer extended"),
                        new SpriteLayoutRange(0xF781, "35 Bladez emerging"),
                        new SpriteLayoutRange(0xF78C, "36 Bladez extending"),
                        new SpriteLayoutRange(0xF799, "37 Bladez extended"),
                        new SpriteLayoutRange(0xF7AC, "38 Splash 1"),
                        new SpriteLayoutRange(0xF7B5, "39 Splash 2"),
                        new SpriteLayoutRange(0xF7C6, "3A Splash 3"),
                        new SpriteLayoutRange(0xF7CD, "3B Splash 4"),
                        new SpriteLayoutRange(0xF7D4, "3C Splash 5"),
                        new SpriteLayoutRange(0xF7E0, "3D Splash 6"),
                        new SpriteLayoutRange(0xF7EC, "3E Splash 7"),
                        new SpriteLayoutRange(0xF800, "3F 40 41 42 43 44 45 46 47 48 49 4B Warp rocket"),
                        new SpriteLayoutRange(0xF82C, "4C Toilet seat 1"),
                        new SpriteLayoutRange(0xF83B, "4D Toilet seat 2"),
                        new SpriteLayoutRange(0xF848, "4E Toilet seat 3"),
                        new SpriteLayoutRange(0xF857, "4F Toilet seat 4"),
                        new SpriteLayoutRange(0xF864, "50 Pin cushion 1"),
                        new SpriteLayoutRange(0xF86B, "51 Pin cushion 2"),
                        new SpriteLayoutRange(0xF872, "52 Pin cushion 3"),
                        new SpriteLayoutRange(0xF87B, "53 Pin cushion 4"),
                        new SpriteLayoutRange(0xF888, "54 5A Bomb"),
                        new SpriteLayoutRange(0xF898, "5B Explosion 1"),
                        new SpriteLayoutRange(0xF89F, "5C Explosion 2"),
                        new SpriteLayoutRange(0xF8AC, "5D Explosion 3"),
                        new SpriteLayoutRange(0xF8C1, "5F 62 Flag 1"),
                        new SpriteLayoutRange(0xF8CE, "63 Flag 2"),
                        new SpriteLayoutRange(0xF8DB, "64 65 66 67 Bigfoot"),
                        new SpriteLayoutRange(0xF8EE, "6A 6B 6C Poof"),
                        new SpriteLayoutRange(0xF8F5, "68 Snake hurt 1"),
                        new SpriteLayoutRange(0xF902, "69 Snake hurt 2"),
                        new SpriteLayoutRange(0xF90F, "71 Diamond"),
                        new SpriteLayoutRange(0xF91A, "72 Extra life"),
                        new SpriteLayoutRange(0xF924, "73 Reverse"),
                        new SpriteLayoutRange(0xF930, "74 Speed up"),
                        new SpriteLayoutRange(0xF93B, "75 100"),
                        new SpriteLayoutRange(0xF948, "76 200"),
                        new SpriteLayoutRange(0xF955, "77 300"),
                        new SpriteLayoutRange(0xF962, "78 750"),
                        new SpriteLayoutRange(0xF96F, "79 500"),
                        new SpriteLayoutRange(0xF97C, "7A 150"),
                        new SpriteLayoutRange(0xF989, "7B 1000"),
                        new SpriteLayoutRange(0xF999, "7C 5000"),
                        new SpriteLayoutRange(0xF9A9, "7E Pibblefish 1"),
                        new SpriteLayoutRange(0xF9B6, "7F Pibblefish 2"),
                        new SpriteLayoutRange(0xF9C1, "80 Pibblefish 3"),
                        new SpriteLayoutRange(0xF9CC, "81 Pibbleboing 1"),
                        new SpriteLayoutRange(0xF9D6, "82 Pibbleboing 2"),
                        new SpriteLayoutRange(0xF9E0, "83 Pibbleboing 3"),
                        new SpriteLayoutRange(0xF9EA, "84 Pibblecopter 1"),
                        new SpriteLayoutRange(0xF9F4, "85 Pibblecopter 2"),
                        new SpriteLayoutRange(0xF9FE, "86 Pibblecopter 3"),
                        new SpriteLayoutRange(0xFA0A, "87 Portrait frame 1"),
                        new SpriteLayoutRange(0xFA1B, "88 Portrait frame 2"),
                        new UnknownRange(0xFA2F, 0x02),
                        new DescribedRange(0xFC1F, 0x20, "Status bar OAM template"),
                        new UnknownRange(0xFC3F, 0x08),
                        new UnknownRange(0xFC47, 0x08),
                        new UnknownRange(0xFC4F, 0x20),
                        new UnknownRange(0xFC6F, 0x37),
                        new UnknownRange(0xFCD6, 0x08),
                        new UnknownRange(0xFFAC, 0x02),
                        new DescribedRange(0xFFFA, 0x02, "Jump vector (NMI)"),
                        new DescribedRange(0xFFFC, 0x02, "Jump vector (RST)"),
                        new DescribedRange(0xFFFE, 0x02, "Jump vector (IRQ)")
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $06\r\n");
                    // BLIT $06
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(3, 0x2B0), 0xE0, output, labels, comments, inlineComments, new Range[] {
                        //new UnknownRange(0x0236, 0xA8),
                        new BackgroundArrangementRange(0x0236),
                        new BackgroundArrangementRange(0x028E),
                        new BackgroundArrangementRange(0x02B6),
                        new DescribedRange(0x02DE, 0x02, "Chaff")
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $0C\r\n");
                    // BLIT $0C
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(3, 0xDF0), 0x8F, output, labels, comments, inlineComments, new[] {
                        new UnknownRange(0x0230, 0x5F)
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $12\r\n");
                    // BLIT $12
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(3, 0xE7F), 0xD4, output, labels, comments, inlineComments, new Range[] {
                        new DescribedRange(0x0284, 0x50, "Unknown range loaded at $224-$24A above", 8)
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $4E\r\n");
                    // BLIT $4E
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(5, 0xCF0), 0x50, output, labels, comments, inlineComments, new Range[] {
                            new DescribedRange(0x0219, 0x1E, "Unknow range (addressed via $213)", 6),
                            new UnknownRange(0x024F, 0x01)
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $54\r\n");
                    // BLIT $54
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(5, 0x560), 0x40, output, labels, comments, inlineComments, new Range[] {
                        new DescribedRange(0x021A, 0x07, "Number of strings to print"),
                        new StringRange(0x0221),
                        new StringRange(0x0228),
                        new StringRange(0x0231),
                        new StringRange(0x023A),
                        new StringRange(0x023D)
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $5A\r\n");
                    // BLIT $5A
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(2, 0xF07), 0xF9, output, labels, comments, inlineComments, new[] {
                        new UnknownRange(0x0254, 0x23),
                        new UnknownRange(0x02F7, 0x03)
                    });
                }


                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $30\r\n");
                    // BLIT $30
                    disassembler.Disassemble(0x0653, ChrRomFileOffset(5, 0x864), 0x9C, output, labels, comments, inlineComments, new Range[] {
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
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $36\r\n");
                    // BLIT $36
                    disassembler.Disassemble(0x0653, ChrRomFileOffset(1, 0xB60), 0x60, output, labels, comments, inlineComments, new[] {
                        new UnknownRange(0x06A9, 0x0A)
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $18\r\n");
                    // BLIT $18
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(3, 0xF53), 0xAD, output, labels, comments, inlineComments, new Range[] {
                        new UnknownRange(0x0749, 0x12),
                        new DescribedRange(0x075B, 0x12, "PPU ADDR lookup table", 2),
                        new UnknownRange(0x076D, 0x40),
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $1E\r\n");
                    // BLIT $1E
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(3, 0xBA0), 0x100, output, labels, comments, inlineComments, new Range[] {
                        new UnknownRange(0x078E, 0x12),
                        new DescribedRange(0x07C2, 0x30, "Unknown range", 4),
                        new UnknownRange(0x07F2, 0xE)
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $24\r\n");
                    // BLIT $24
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(5, 0x670), 0x100, output, labels, comments, inlineComments, new UnknownRange[] {
                        new UnknownRange(0x7FE, 0x02)
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $42\r\n");
                    // BLIT $42
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(7, 0x4C0), 0x80, output, labels, comments, inlineComments, new Range[] {
                        new DescribedRange(0x0700, 0x06, "Lid contents starting index"),
                        new DescribedRange(0x0706, 0x16, "Level 1 lid contents", 2),
                        new DescribedRange(0x071C, 0x16, "Level 2 lid contents", 2),
                        new DescribedRange(0x0732, 0x1E, "Level 3 lid contents", 2),
                        new DescribedRange(0x0750, 0x1A, "Level 4 lid contents", 2),
                        new DescribedRange(0x076A, 0x14, "Level 5 lid contents", 2),
                        new DescribedRange(0x077E, 0x02, "Level 6 lid contents??", 2),
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $48\r\n");
                    // BLIT $48
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(4, 0xCF0), 0x50, output, labels, comments, inlineComments, new UnknownRange[] {
                        new UnknownRange(0x074C, 0x05)
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $60\r\n");
                    // BLIT $60
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(6, 0x846), 0xB0, output, labels, comments, inlineComments, new UnknownRange[] {
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $2A\r\n");
                    // BLIT $2A
                    disassembler.Disassemble(0x0600, ChrRomFileOffset(5, 0x770), 0xF4, output, labels, comments, inlineComments, new Range[] {
                        new BackgroundArrangementRange(0x66A),
                        new BackgroundArrangementRange(0x67E),
                        new StringRange(0x69A),
                        new StringRange(0x6A7),
                        new StringRange(0x6B4),
                        new StringRange(0x6C3),
                        new StringRange(0x6D3),
                        new StringRange(0x6DC),
                        new StringRange(0x6E2),
                        new UnknownRange(0x06F3, 0x01)
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $3C\r\n");
                    // BLIT $3C
                    disassembler.Disassemble(0x06A0, ChrRomFileOffset(4, 0xB60), 0x60, output, labels, comments, inlineComments, new Range[] {
                        new SpriteLayoutRange(0x06A0, "Spaceship body"),
                        new SpriteLayoutRange(0x06EC, "Spaceship canopy"),
                        new UnknownRange(0x06FD, 0x03)
                    });
                }

                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nBLIT $00\r\n");
                    // BLIT $00
                    disassembler.Disassemble(0x03FF, ChrRomFileOffset(3, 0x390), 0x400, output, labels, comments, inlineComments, new Range[] {
                        new StringRange(0x0409),
                        new StringRange(0x0413),
                        new StringRange(0x041C),
                        new DescribedRange(0x04CA, 0x06, "PPU ADDR low byte"),
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
                }


                labels.ClearLabelsInRAM();
                foreach (var output in new[] { TextWriter.Null, outputFile })
                {
                    output.WriteLine("\r\nChr Rom Page 6\r\n");
                    disassembler.Disassemble(0x08F6, ChrRomFileOffset(6, 0x8F6), 0x6F9, output, labels, comments, inlineComments, new Range[] {
                        new EntityTemplateRange(0x8F6, 0x77 / 0x7, "Level 1 entity data"),
                        new EntityTemplateRange(0x96D, 0x7E / 0x7, "Level 2 entity data"),
                        new EntityTemplateRange(0x9EB, 0xA8 / 0x7, "Level 3 entity data"),
                        new EntityTemplateRange(0xA93, 0x9A / 0x7, "Level 4 entity data"),
                        new EntityTemplateRange(0xB2D, 0xA1 / 0x7, "Level 5 entity data"),
                        new EntityTemplateRange(0xBCE, 0xA8 / 0x7, "Level 6 entity data"),
                        new EntityTemplateRange(0xC76, 0x93 / 0x7, "Level 7 entity data (part 1 of 2)"),
                        new EntityTemplateRange(0xD09, 0x0E / 0x7, "Level 8 entity data"),
                        new EntityTemplateRange(0xD17, 0x9A / 0x7, "Level 9 entity data"),
                        new EntityTemplateRange(0xDB1, 0x70 / 0x7, "Level 10 entity data"),
                        new EntityTemplateRange(0xE21, 0x38 / 0x7, "Level 11 entity data"),
                        new EntityTemplateRange(0xE59, 0x31 / 0x7, "Fish pond 1 Entity data"),
                        new EntityTemplateRange(0xE8A, 0x3F / 0x7, "Fish pond 2 Entity data"),
                        new EntityTemplateRange(0xEC9, 0x3F / 0x7, "Fish pond 3 Entity data"),
                        new EntityTemplateRange(0xF08, 0x46 / 0x7, "Fish pond 4 Entity data"),
                        new EntityTemplateRange(0xF4E, 0x2A / 0x7, "Fish pond 5 Entity data"),
                        new EntityTemplateRange(0xF78, 0x77 / 0x7, "Level 7 entity data (part 2 of 2)"),
                    });
                }
            }

            Process.Start("disassembly.txt");
        }

        private static void HackStartingLevel(byte[] data, int startingLevel)
        {
            data[PrgRomFileOffset(0x82BC)] = (byte)(startingLevel - 2);
        }

        private static string AppendSuffix(string baseFileName, string suffix)
        {
            return Path.Combine(Path.GetDirectoryName(baseFileName),
                Path.ChangeExtension(Path.GetFileNameWithoutExtension(baseFileName) + suffix,
                        Path.GetExtension(baseFileName)));
        }

        private static void ProduceHackedRom(byte[] fileData, string filename, Action<byte[]> hack)
        {
            var fileCopy = new byte[fileData.Length];
            Array.Copy(fileData, 0, fileCopy, 0, fileCopy.Length);
            hack(fileCopy);
            File.WriteAllBytes(filename, fileCopy);
        }

        private static void DumpArrangement(byte[] fileData, int startAddress, string filename)
        {
            using (var reader = new MemoryStream(fileData))
            {
                reader.Seek(startAddress, SeekOrigin.Begin);

                byte destLow = reader.SafeReadByte();
                byte destHi = reader.SafeReadByte();
                byte width = reader.SafeReadByte();
                byte height = reader.SafeReadByte();

                using (var writer = new BinaryWriter(File.Create(filename)))
                {
                    writer.Write((int)width);
                    writer.Write((int)height);
                    for (int i = 0; i < width * height; i++)
                    {
                        writer.Write((int)reader.SafeReadByte());
                    }
                }
            }
        }

        private static void DumpSprites(byte[] fileData, Color[] colors)
        {
            Directory.CreateDirectory("sprites");
            var spriteLayouts = GetSpriteLayouts();
            foreach (var chrPage in Enumerable.Range(0, 8))
            {
                if (!spriteLayouts.ContainsKey(chrPage))
                    continue;

                Directory.CreateDirectory(string.Format("sprites\\{0}", chrPage));

                foreach (var address in spriteLayouts[chrPage])
                {
                    DumpSprite(fileData, address, chrPage,
                        string.Format("sprites\\{1}\\{0:X4}_{1:X2}.png", address + 0x8000 - 0x10, chrPage),
                        colors);
                }
            }
        }

        private static void DumpDynamicSprites(byte[] fileData, Color[] colors)
        {
            Directory.CreateDirectory("sprites\\5");

            DumpSprite(fileData, ChrRomFileOffset(4, 0xB60), 5, "sprites\\5\\06A0_05.png", colors);
            DumpSprite(fileData, ChrRomFileOffset(4, 0xBAC), 5, "sprites\\5\\06EC_05.png", colors);
        }


        private static void DumpSprite(byte[] fileData, int startAddress, int chrPage, string filename, Color[] colors)
        {
            using (var bitmap = SpriteLayout.RasterizeSprite(fileData, startAddress, ChrRomFileOffset(chrPage, 0), colors))
            {
                using (var output = new Bitmap(bitmap.Width * 3, bitmap.Height * 3))
                {
                    using (var g = Graphics.FromImage(output))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                        g.DrawImage(bitmap, 0, 0, output.Width, output.Height);
                    }

                    output.Save(filename, ImageFormat.Png);
                }
            }
        }

        public static Dictionary<int, List<int>> GetSpriteLayouts()
        {
            //new SpriteLayoutRange(0x06A0, "Spaceship body"),
            //new SpriteLayoutRange(0x06EC, "Spaceship canopy"),

            var result = new Dictionary<int, List<int>>();

            result.Add(1, new List<int>()
            {
                //PrgRomFileOffset(0x9E57),
                PrgRomFileOffset(0x9E62),//
                //PrgRomFileOffset(0x9E6F),
                //PrgRomFileOffset(0x9E79),
                //PrgRomFileOffset(0x9E86),
                //PrgRomFileOffset(0xAF0A),
                //PrgRomFileOffset(0xAF17),
                //PrgRomFileOffset(0xAF24),
                //PrgRomFileOffset(0xAF31),
                //PrgRomFileOffset(0xCF63),
                //PrgRomFileOffset(0xD16A),
                //PrgRomFileOffset(0xD17B),
                //PrgRomFileOffset(0xD18C),
                //PrgRomFileOffset(0xD19D),
                //PrgRomFileOffset(0xD1B6),
                PrgRomFileOffset(0xD222),//
                PrgRomFileOffset(0xD22B),//
                //PrgRomFileOffset(0xD238),
                PrgRomFileOffset(0xDE12),//
                PrgRomFileOffset(0xDE26),//
                PrgRomFileOffset(0xDED2),//
                PrgRomFileOffset(0xDEF6),//
                PrgRomFileOffset(0xF3C0),//
                PrgRomFileOffset(0xF3CD),//
                PrgRomFileOffset(0xF3DA),//
                PrgRomFileOffset(0xF53A),//
                PrgRomFileOffset(0xF547),//
                PrgRomFileOffset(0xF554),//
                PrgRomFileOffset(0xF55D),//
                PrgRomFileOffset(0xF56A),//
                PrgRomFileOffset(0xF577),//
                PrgRomFileOffset(0xF584),//
                PrgRomFileOffset(0xF591),//
                PrgRomFileOffset(0xF59E),//
                PrgRomFileOffset(0xF5AB),//
                PrgRomFileOffset(0xF5B8),//
                PrgRomFileOffset(0xF5C5),//
                PrgRomFileOffset(0xF5D2),//
                PrgRomFileOffset(0xF5E6),//
                //PrgRomFileOffset(0xF5F1),
                PrgRomFileOffset(0xF5FE),//
                PrgRomFileOffset(0xF60B),//
                PrgRomFileOffset(0xF615),//
                PrgRomFileOffset(0xF622),//
                PrgRomFileOffset(0xF62F),//
                PrgRomFileOffset(0xF639),//
                PrgRomFileOffset(0xF646),//
                PrgRomFileOffset(0xF64D),//
                PrgRomFileOffset(0xF65A),//
                PrgRomFileOffset(0xF66E),//
                PrgRomFileOffset(0xF67B),//
                PrgRomFileOffset(0xF687),//
                PrgRomFileOffset(0xF693),//
                PrgRomFileOffset(0xF69C),//
                PrgRomFileOffset(0xF6A5),//
                PrgRomFileOffset(0xF6AE),//
                PrgRomFileOffset(0xF6B7),//
                PrgRomFileOffset(0xF6C8),//
                PrgRomFileOffset(0xF6DE),//
                PrgRomFileOffset(0xF6F4),//
                PrgRomFileOffset(0xF6FB),//
                PrgRomFileOffset(0xF713),//
                //PrgRomFileOffset(0xF72B),
                //PrgRomFileOffset(0xF736),
                //PrgRomFileOffset(0xF741),
                //PrgRomFileOffset(0xF74C),
                //PrgRomFileOffset(0xF755),
                //PrgRomFileOffset(0xF759),
                //PrgRomFileOffset(0xF765),
                //PrgRomFileOffset(0xF772),
                PrgRomFileOffset(0xF781),//
                PrgRomFileOffset(0xF78C),//
                PrgRomFileOffset(0xF799),//
                PrgRomFileOffset(0xF7AC),//
                PrgRomFileOffset(0xF7B5),//
                PrgRomFileOffset(0xF7C6),//
                PrgRomFileOffset(0xF7CD),//
                PrgRomFileOffset(0xF7D4),//
                PrgRomFileOffset(0xF7E0),//
                PrgRomFileOffset(0xF7EC),//
                PrgRomFileOffset(0xF800),//
                PrgRomFileOffset(0xF82C),//
                PrgRomFileOffset(0xF83B),//
                PrgRomFileOffset(0xF848),//
                PrgRomFileOffset(0xF857),//
                PrgRomFileOffset(0xF864),//
                PrgRomFileOffset(0xF86B),//
                PrgRomFileOffset(0xF872),//
                PrgRomFileOffset(0xF87B),//
                PrgRomFileOffset(0xF888),//
                PrgRomFileOffset(0xF898),//
                PrgRomFileOffset(0xF89F),//
                PrgRomFileOffset(0xF8AC),//
                //PrgRomFileOffset(0xF8C1),
                //PrgRomFileOffset(0xF8CE),
                PrgRomFileOffset(0xF8DB),//
                PrgRomFileOffset(0xF8EE),//
                PrgRomFileOffset(0xF8F5),//
                PrgRomFileOffset(0xF902),//
                PrgRomFileOffset(0xF90F),//
                PrgRomFileOffset(0xF91A),//
                PrgRomFileOffset(0xF924),//
                PrgRomFileOffset(0xF930),//
                PrgRomFileOffset(0xF93B),//
                PrgRomFileOffset(0xF948),//
                PrgRomFileOffset(0xF955),//
                PrgRomFileOffset(0xF962),//
                PrgRomFileOffset(0xF96F),//
                PrgRomFileOffset(0xF97C),//
                PrgRomFileOffset(0xF989),//
                PrgRomFileOffset(0xF999),//
                //PrgRomFileOffset(0xF9A9),
                //PrgRomFileOffset(0xF9B6),
                //PrgRomFileOffset(0xF9C1),
                PrgRomFileOffset(0xF9CC),//
                PrgRomFileOffset(0xF9D6),//
                PrgRomFileOffset(0xF9E0),//
                //PrgRomFileOffset(0xF9EA),
                //PrgRomFileOffset(0xF9F4),
                //PrgRomFileOffset(0xF9FE),
            });

            result.Add(4, new List<int>()
            {
                PrgRomFileOffset(0x9E57),//
                PrgRomFileOffset(0x9E62),//
                PrgRomFileOffset(0x9E6F),//
                PrgRomFileOffset(0x9E79),//
                PrgRomFileOffset(0x9E86),//
                //PrgRomFileOffset(0xAF0A),
                //PrgRomFileOffset(0xAF17),
                //PrgRomFileOffset(0xAF24),
                //PrgRomFileOffset(0xAF31),
                PrgRomFileOffset(0xCF63),//
                PrgRomFileOffset(0xD16A),//
                PrgRomFileOffset(0xD17B),//
                PrgRomFileOffset(0xD18C),//
                PrgRomFileOffset(0xD19D),//
                PrgRomFileOffset(0xD1B6),//
                PrgRomFileOffset(0xD222),//
                PrgRomFileOffset(0xD22B),//
                PrgRomFileOffset(0xD238),//
                //PrgRomFileOffset(0xDE12),
                //PrgRomFileOffset(0xDE26),
                //PrgRomFileOffset(0xDED2),
                //PrgRomFileOffset(0xDEF6),
                //PrgRomFileOffset(0xF3C0),
                //PrgRomFileOffset(0xF3CD),
                //PrgRomFileOffset(0xF3DA),
                PrgRomFileOffset(0xF53A),//
                PrgRomFileOffset(0xF547),//
                PrgRomFileOffset(0xF554),//
                PrgRomFileOffset(0xF55D),//
                PrgRomFileOffset(0xF56A),//
                PrgRomFileOffset(0xF577),//
                PrgRomFileOffset(0xF584),//
                PrgRomFileOffset(0xF591),//
                PrgRomFileOffset(0xF59E),//
                PrgRomFileOffset(0xF5AB),//
                PrgRomFileOffset(0xF5B8),//
                PrgRomFileOffset(0xF5C5),//
                PrgRomFileOffset(0xF5D2),//
                PrgRomFileOffset(0xF5E6),//
                PrgRomFileOffset(0xF5F1),//
                //PrgRomFileOffset(0xF5FE),
                //PrgRomFileOffset(0xF60B),
                //PrgRomFileOffset(0xF615),
                //PrgRomFileOffset(0xF622),
                //PrgRomFileOffset(0xF62F),
                //PrgRomFileOffset(0xF639),
                PrgRomFileOffset(0xF646),//
                PrgRomFileOffset(0xF64D),//
                PrgRomFileOffset(0xF65A),//
                PrgRomFileOffset(0xF66E),//
                PrgRomFileOffset(0xF67B),//
                //PrgRomFileOffset(0xF687),
                //PrgRomFileOffset(0xF693),
                //PrgRomFileOffset(0xF69C),
                //PrgRomFileOffset(0xF6A5),
                //PrgRomFileOffset(0xF6AE),
                PrgRomFileOffset(0xF6B7),//
                PrgRomFileOffset(0xF6C8),//
                PrgRomFileOffset(0xF6DE),//
                PrgRomFileOffset(0xF6F4),//
                PrgRomFileOffset(0xF6FB),//
                PrgRomFileOffset(0xF713),//
                PrgRomFileOffset(0xF72B),//
                PrgRomFileOffset(0xF736),//
                PrgRomFileOffset(0xF741),//
                PrgRomFileOffset(0xF74C),//
                //PrgRomFileOffset(0xF755),
                PrgRomFileOffset(0xF759),//
                PrgRomFileOffset(0xF765),//
                PrgRomFileOffset(0xF772),//
                //PrgRomFileOffset(0xF781),
                //PrgRomFileOffset(0xF78C),
                //PrgRomFileOffset(0xF799),
                PrgRomFileOffset(0xF7AC),//
                PrgRomFileOffset(0xF7B5),//
                PrgRomFileOffset(0xF7C6),//
                PrgRomFileOffset(0xF7CD),//
                PrgRomFileOffset(0xF7D4),//
                PrgRomFileOffset(0xF7E0),//
                PrgRomFileOffset(0xF7EC),//
                //PrgRomFileOffset(0xF800),
                //PrgRomFileOffset(0xF82C),
                //PrgRomFileOffset(0xF83B),
                //PrgRomFileOffset(0xF848),
                //PrgRomFileOffset(0xF857),
                //PrgRomFileOffset(0xF864),
                //PrgRomFileOffset(0xF86B),
                //PrgRomFileOffset(0xF872),
                //PrgRomFileOffset(0xF87B),
                PrgRomFileOffset(0xF888),//
                PrgRomFileOffset(0xF898),//
                PrgRomFileOffset(0xF89F),//
                PrgRomFileOffset(0xF8AC),//
                //PrgRomFileOffset(0xF8C1),
                //PrgRomFileOffset(0xF8CE),
                PrgRomFileOffset(0xF8DB),//
                PrgRomFileOffset(0xF8EE),//
                PrgRomFileOffset(0xF8F5),//
                PrgRomFileOffset(0xF902),//
                PrgRomFileOffset(0xF90F),//
                PrgRomFileOffset(0xF91A),//
                PrgRomFileOffset(0xF924),//
                PrgRomFileOffset(0xF930),//
                PrgRomFileOffset(0xF93B),//
                PrgRomFileOffset(0xF948),//
                PrgRomFileOffset(0xF955),//
                PrgRomFileOffset(0xF962),//
                PrgRomFileOffset(0xF96F),//
                PrgRomFileOffset(0xF97C),//
                PrgRomFileOffset(0xF989),//
                PrgRomFileOffset(0xF999),//
                PrgRomFileOffset(0xF9A9),//
                PrgRomFileOffset(0xF9B6),//
                PrgRomFileOffset(0xF9C1),//
                //PrgRomFileOffset(0xF9CC),
                //PrgRomFileOffset(0xF9D6),
                //PrgRomFileOffset(0xF9E0),
                //PrgRomFileOffset(0xF9EA),
                //PrgRomFileOffset(0xF9F4),
                //PrgRomFileOffset(0xF9FE),
            });

            result.Add(5, new List<int>()
            {
                //PrgRomFileOffset(0x9E57),
                PrgRomFileOffset(0x9E62),
                //PrgRomFileOffset(0x9E6F),
                //PrgRomFileOffset(0x9E79),
                //PrgRomFileOffset(0x9E86),
                PrgRomFileOffset(0xAF0A),//
                PrgRomFileOffset(0xAF17),//
                PrgRomFileOffset(0xAF24),//
                PrgRomFileOffset(0xAF31),//
                //PrgRomFileOffset(0xCF63),
                //PrgRomFileOffset(0xD16A),
                //PrgRomFileOffset(0xD17B),
                //PrgRomFileOffset(0xD18C),
                //PrgRomFileOffset(0xD19D),
                //PrgRomFileOffset(0xD1B6),
                PrgRomFileOffset(0xD222),//
                PrgRomFileOffset(0xD22B),//
                //PrgRomFileOffset(0xD238),
                //PrgRomFileOffset(0xDE12),
                //PrgRomFileOffset(0xDE26),
                //PrgRomFileOffset(0xDED2),
                //PrgRomFileOffset(0xDEF6),
                //PrgRomFileOffset(0xF3C0),
                //PrgRomFileOffset(0xF3CD),
                //PrgRomFileOffset(0xF3DA),
                PrgRomFileOffset(0xF53A),//
                PrgRomFileOffset(0xF547),//
                PrgRomFileOffset(0xF554),//
                PrgRomFileOffset(0xF55D),//
                PrgRomFileOffset(0xF56A),//
                PrgRomFileOffset(0xF577),//
                PrgRomFileOffset(0xF584),//
                PrgRomFileOffset(0xF591),//
                PrgRomFileOffset(0xF59E),//
                PrgRomFileOffset(0xF5AB),//
                PrgRomFileOffset(0xF5B8),//
                PrgRomFileOffset(0xF5C5),//
                PrgRomFileOffset(0xF5D2),//
                PrgRomFileOffset(0xF5E6),//
                //PrgRomFileOffset(0xF5F1),
                //PrgRomFileOffset(0xF5FE),
                //PrgRomFileOffset(0xF60B),
                //PrgRomFileOffset(0xF615),
                //PrgRomFileOffset(0xF622),
                //PrgRomFileOffset(0xF62F),
                //PrgRomFileOffset(0xF639),
                PrgRomFileOffset(0xF646),//
                PrgRomFileOffset(0xF64D),//
                //PrgRomFileOffset(0xF65A),
                //PrgRomFileOffset(0xF66E),
                //PrgRomFileOffset(0xF67B),
                //PrgRomFileOffset(0xF687),
                //PrgRomFileOffset(0xF693),
                //PrgRomFileOffset(0xF69C),
                //PrgRomFileOffset(0xF6A5),
                //PrgRomFileOffset(0xF6AE),
                PrgRomFileOffset(0xF6B7),//
                PrgRomFileOffset(0xF6C8),//
                PrgRomFileOffset(0xF6DE),//
                PrgRomFileOffset(0xF6F4),//
                PrgRomFileOffset(0xF6FB),//
                PrgRomFileOffset(0xF713),//
                //PrgRomFileOffset(0xF72B),
                //PrgRomFileOffset(0xF736),
                //PrgRomFileOffset(0xF741),
                //PrgRomFileOffset(0xF74C),
                //PrgRomFileOffset(0xF755),
                //PrgRomFileOffset(0xF759),
                //PrgRomFileOffset(0xF765),
                //PrgRomFileOffset(0xF772),
                //PrgRomFileOffset(0xF781),
                //PrgRomFileOffset(0xF78C),
                //PrgRomFileOffset(0xF799),
                //PrgRomFileOffset(0xF7AC),
                //PrgRomFileOffset(0xF7B5),
                //PrgRomFileOffset(0xF7C6),
                //PrgRomFileOffset(0xF7CD),
                //PrgRomFileOffset(0xF7D4),
                //PrgRomFileOffset(0xF7E0),
                //PrgRomFileOffset(0xF7EC),
                //PrgRomFileOffset(0xF800),
                //PrgRomFileOffset(0xF82C),
                //PrgRomFileOffset(0xF83B),
                //PrgRomFileOffset(0xF848),
                //PrgRomFileOffset(0xF857),
                //PrgRomFileOffset(0xF864),
                //PrgRomFileOffset(0xF86B),
                //PrgRomFileOffset(0xF872),
                //PrgRomFileOffset(0xF87B),
                PrgRomFileOffset(0xF888),//
                PrgRomFileOffset(0xF898),//
                PrgRomFileOffset(0xF89F),//
                PrgRomFileOffset(0xF8AC),//
                PrgRomFileOffset(0xF8C1),//
                PrgRomFileOffset(0xF8CE),//
                PrgRomFileOffset(0xF8DB),//
                PrgRomFileOffset(0xF8EE),//
                PrgRomFileOffset(0xF8F5),//
                PrgRomFileOffset(0xF902),//
                PrgRomFileOffset(0xF90F),//
                PrgRomFileOffset(0xF91A),//
                PrgRomFileOffset(0xF924),//
                //PrgRomFileOffset(0xF930),
                PrgRomFileOffset(0xF93B),//
                PrgRomFileOffset(0xF948),//
                PrgRomFileOffset(0xF955),//
                PrgRomFileOffset(0xF962),//
                PrgRomFileOffset(0xF96F),//
                PrgRomFileOffset(0xF97C),//
                PrgRomFileOffset(0xF989),//
                PrgRomFileOffset(0xF999),//
                //PrgRomFileOffset(0xF9A9),
                //PrgRomFileOffset(0xF9B6),
                //PrgRomFileOffset(0xF9C1),
                //PrgRomFileOffset(0xF9CC),
                //PrgRomFileOffset(0xF9D6),
                //PrgRomFileOffset(0xF9E0),
                PrgRomFileOffset(0xF9EA),//
                PrgRomFileOffset(0xF9F4),//
                PrgRomFileOffset(0xF9FE),//
            });

            result.Add(7, new List<int>()
            {
                PrgRomFileOffset(0xFA0A),
                PrgRomFileOffset(0xFA1B)
            });
            return result;
        }
    }
}
