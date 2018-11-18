using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Disassembler
{
    // What are all the calls to sFindEntityA used to seek?
    // What do the AI flags control?
    // Create a hack to skip straight to player select
    // 
    class Program
    {
        const bool CreateGraphicalOutputs = true;

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

            /*
            Blit patterns:

            Startup
            5A 30 24 0C 06

            Main Titles Screen
            5A 0C : 5A 0C : 5A 06 0C

            Level Start
            5A 48 42 1E (3C 60)* 0C 12  *(level 11 only)

            Level End / Game Over Screen
            0C 06 00 0C 24

            Bonus / Warp Screen
            24 0C 06 54 0C

            Bonus Start
            18 48 1E 0C 12

            Pond Entry
            5A 18 48 1E 0C 12

            Pond exit / Bonus end
            4E 1E 0C 12
            
            End credits
            24 0C 06 0C 06 2A 36 24
            */


            var fileData = File.ReadAllBytes(args[0]);

            if ("a".Equals("a"))
            {
                byte[] snakeMountain = new byte[4096];
                Array.Copy(fileData, PrgRomFileOffset(0xE3C0), snakeMountain, 0, 4096);
                DumpMap(fileData, snakeMountain, "snake_mountain.png", 64);

                DumpMap(fileData, DecompressBonusWarpMap(fileData, ChrRomFileOffset(6, 0x1DA)), "bonus_1.png", 16);
                DumpMap(fileData, DecompressBonusWarpMap(fileData, ChrRomFileOffset(6, 0x22A)), "bonus_2.png", 16);
                DumpMap(fileData, DecompressBonusWarpMap(fileData, ChrRomFileOffset(6, 0x29E)), "bonus_3.png", 16);
                DumpMap(fileData, DecompressBonusWarpMap(fileData, ChrRomFileOffset(6, 0x30E)), "bonus_4.png", 16);
                DumpMap(fileData, DecompressBonusWarpMap(fileData, ChrRomFileOffset(6, 0x678)), "pond_1.png", 16);
                DumpMap(fileData, DecompressBonusWarpMap(fileData, ChrRomFileOffset(6, 0x6FE)), "pond_2.png", 16);
                DumpMap(fileData, DecompressBonusWarpMap(fileData, ChrRomFileOffset(6, 0x774)), "pond_3.png", 16);
                DumpMap(fileData, DecompressBonusWarpMap(fileData, ChrRomFileOffset(6, 0x7DC)), "pond_4_5.png", 16);
            }



            ProduceHackedRom(fileData, AppendSuffix(args[0], " - slow BigFoot start on level 11"), (data) =>
            {
                DisableBigfootHealthRegeneration(data);
                HackStartingLevel(data, 11);
                HackQuickGameStart(data);
            });

            for (int i = 2; i < 11; i++)
            {
                ProduceHackedRom(fileData, AppendSuffix(args[0], " - start on level " + i), (data) =>
                {
                    DisableBigfootHealthRegeneration(data);
                    HackStartingLevel(data, i);
                    HackQuickGameStart(data);
                });
            }

            ProduceHackedRom(fileData, AppendSuffix(args[0], " - no bombs"), (data) =>
            {
                HackDisableBombPibblies(data);
                HackQuickGameStart(data);

                // Psychedellic backgrounds
                //foreach (var i in Enumerable.Range(0, 26))
                //    data[PrgRomFileOffset(0xA4B9 + i)] = 0x1B;
            });

            ProduceHackedRom(fileData, AppendSuffix(args[0], " - start on level 10 no bombs"), (data) =>
            {
                HackQuickGameStart(data);
                HackStartingLevel(data, 10);
                HackDisableBombPibblies(data);
            });

            if (CreateGraphicalOutputs.Equals(true))
            {
                var palette = new[]
                {
                    Color.FromArgb(40,2,196),
                    Color.FromArgb(0, 0, 0),
                    Color.FromArgb(0, 91, 0),
                    Color.FromArgb(0, 160, 0)
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

                Directory.CreateDirectory("palettes");
                DumpPalette(fileData, ChrRomFileOffset(3, 0xE20), @"palettes\PlayBgPalette_00.png");
                DumpPalette(fileData, ChrRomFileOffset(3, 0xE30), @"palettes\PlaySpritePalette_10.png");
                DumpPalette(fileData, ChrRomFileOffset(3, 0xE40), @"palettes\TitleBgPalette_CopyrightAndPressStart_20.png");
                DumpPalette(fileData, ChrRomFileOffset(3, 0xE50), @"palettes\TitleBgPalette_Starring_30.png");
                DumpPalette(fileData, ChrRomFileOffset(3, 0xE60), @"palettes\TitleSpritePalette_Sparkles_40.png");
                DumpPalette(fileData, ChrRomFileOffset(3, 0xE6F), @"palettes\InterstitialBgPalette_4F.png");
            }

            Annotations annotations = new Annotations();

            using (var disassembler = new OpcodeDisassembler(new MemoryStream(fileData)))
            using (var outputFile = File.CreateText("disassembly.txt"))
            {
                var audioJumpVector = disassembler.ReadJumpVectorHiHiLoLo(PrgRomFileOffset(0xD970), 0x17);
                foreach (var i in Enumerable.Range(0, 0x17))
                    annotations.RecordLabel(audioJumpVector[i], string.Format("cSfxOpc_{0:X2}", i));

                var outputs = new IDisassemblerOutput[] { new NullDisassemblerOutput(), new TextDisassemblerOutput(outputFile) };

                foreach (var output in outputs)
                {
                    DisassemblePrgRom(annotations, disassembler, output);
                }


                annotations.ClearRAM();
                annotations.RecordLabel(0x0200, "sCopyBgImage");
                annotations.RecordLabel(0x0219, "loop_06_01");
                annotations.RecordLabel(0x021B, "loop_06_02");
                annotations.RecordLabel(0x022B, "loop_06_03");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $06:Common to non-playing segments");
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(3, 0x2B0), 0xE0, output, annotations, new Range[] {
                        //new UnknownRange(0x0236, 0xA8),
                        new BackgroundArrangementRange(0x0236, "SNAKE"),
                        new BackgroundArrangementRange(0x028E, "Rattle"),
                        new BackgroundArrangementRange(0x02B6, "Roll"),
                        new DescribedRange(0x02DE, 0x02, "Chaff")
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0200, "sLdPalette");
                annotations.RecordLabel(0x0203, "loop_0C_01");
                annotations.RecordLabel(0x020A, "loop_0C_02");
                annotations.RecordLabel(0x021C, "loop_0C_03");
                annotations.RecordLabel(0x022F, "rts_0C_01");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $0C:Always loaded");
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(3, 0xDF0), 0x8F, output, annotations, new[] {
                        new DescribedRange(0x0230, 0x5F, "Maybe palette data?", 0x10)
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0200, "sInitLevel");
                annotations.RecordLabel(0x021A, "skip_12_01");
                annotations.RecordLabel(0x0224, "skip_12_02");
                annotations.RecordLabel(0x024C, "skip_12_03");
                annotations.RecordLabel(0x025C, "skip_12_04");
                annotations.RecordLabel(0x0260, "skip_12_05");
                annotations.RecordInlineComment(0x0204, "For levels other than the first, adjust the BG palette");
                annotations.RecordInlineComment(0x020C, "Dark yellow on level 11");
                annotations.RecordInlineComment(0x0212, "Gray on level 9,10");
                annotations.RecordInlineComment(0x0218, "Dark teal otherwise");
                annotations.RecordInlineComment(0x0224, "Tweak BG palette 0 colors 2,3");
                annotations.RecordInlineComment(0x022E, "Tweak BG palette 1 colors 2,3");
                annotations.RecordInlineComment(0x0238, "Tweak BG palette 2 colors 2,3");
                annotations.RecordInlineComment(0x0242, "Tweak sprite palette 3 colors 2,3");
                annotations.RecordInlineComment(0x021A, "Set palette 0, color 1");
                annotations.RecordInlineComment(0x021C, "Set palette 2, color 1");
                annotations.RecordInlineComment(0x0222, "Set palette 1, color 1, sometimes");
                annotations.RecordInlineComment(0x0252, "For levels 9-11, CHR ROM page 3 for background, black sky");
                annotations.RecordInlineComment(0x0254, "For levels 1-4, CHR ROM page 0 for background, lighter blue sky");
                annotations.RecordInlineComment(0x025A, "For levels 5-8, CHR ROM page 0 for background, dark blue sky");
                annotations.RecordInlineComment(0x268, "Save snake X,Y,Z coordinates to $67-$6C for some reason");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $12:Loaded while playing");
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(3, 0xE7F), 0xD4, output, annotations, new Range[] {
                        new DescribedRange(0x0284, 0x50, "Palette data highest two colors BG 0 BG 1 BG 2 sprite 3", 8)
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0200, "sReturnSnakes");
                annotations.RecordLabel(0x0237, "skip_4E_01");
                annotations.RecordLabel(0x020D, "skip_4E_02");
                annotations.RecordLabel(0x023E, "loop_4E_01");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $4E:Pond exiting/leaving bonus");
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(5, 0xCF0), 0x50, output, annotations, new Range[] {
                            new DescribedRange(0x0219, 0x1E, "Pond exit positions", 6),
                            new UnknownRange(0x024F, 0x01)
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0200, "sPrtBonusWarpMsg");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $54:Only bouns/warp screen");
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(5, 0x560), 0x40, output, annotations, new Range[] {
                        new DescribedRange(0x021A, 0x07, "Number of strings to print"),
                        new StringRange(0x0221),
                        new StringRange(0x0228),
                        new StringRange(0x0231),
                        new StringRange(0x023A),
                        new StringRange(0x023D)
                    }, "FADE");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0200, "sLdEntTemplates");
                annotations.RecordLabel(0x0277, "sDynamicLOL");
                annotations.RecordLabel(0x02C7, "sWipeRam");
                annotations.RecordLabel(0x020C, "skip_5A_01");
                annotations.RecordLabel(0x02AF, "skip_5A_02");
                annotations.RecordLabel(0x0230, "loop_5A_01");
                annotations.RecordLabel(0x0289, "loop_5A_02");
                annotations.RecordLabel(0x02CA, "loop_5A_03");
                annotations.RecordLabel(0x02E2, "loop_5A_04");
                annotations.RecordLabel(0x02AE, "rts_5A_01");
                annotations.RecordLabel(0x0653, "sPrtPlyrSelect");
                annotations.RecordInlineComment(0x02BF, "Load new background palette based on $20");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $5A:Startup, main titles, level/pond start");
                    disassembler.Disassemble(0x0200, ChrRomFileOffset(2, 0xF07), 0xF9, output, annotations, new[] {
                        new DescribedRange(0x0254, 0x23, "Entity template addresses in CHR ROM page 6", 2),
                        new DescribedRange(0x02BB, 0x0C, "Decompiles as code but looks unreachable"),
                        new UnknownRange(0x02F7, 0x03)
                    }, "UNKN");
                }

                annotations.ClearRAM();
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("CHR ROM page 6");
                    disassembler.Disassemble(0x08F6, ChrRomFileOffset(6, 0x8F6), 0x6F9, output, annotations, new Range[] {
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
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0653, "sPrtPlyrSelect");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $30:Only during startup");
                    disassembler.Disassemble(0x0653, ChrRomFileOffset(5, 0x864), 0x9C, output, annotations, new Range[] {
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
                        new DescribedRange(0x06EC, 0x03, "Chaff")
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0691, "skip_36_01");
                annotations.RecordLabel(0x069A, "skip_36_02");
                annotations.RecordLabel(0x06A8, "rts_36_01");
                annotations.RecordLabel(0x0653, "sDrawEcShip");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $36:Only during end credits");
                    disassembler.Disassemble(0x0653, ChrRomFileOffset(1, 0xB60), 0x60, output, annotations, new[] {
                        new UnknownRange(0x06A9, 0x0A)
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0700, "sDecodeRleMap");
                annotations.RecordLabel(0x0713, "skip_18_01");
                annotations.RecordLabel(0x0715, "skip_18_02");
                annotations.RecordLabel(0x072E, "loop_18_01");
                annotations.RecordLabel(0x073C, "loop_18_02");
                annotations.RecordLabel(0x0748, "rts_18_01");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $18:Bonus start and pond entry");
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(3, 0xF53), 0xAD, output, annotations, new Range[] {
                        new DescribedRange(0x0749, 0x12, "Data for bonus/ponds (loaded at $C5E5 after map decoded)", 0x02),
                        new DescribedRange(0x075B, 0x12, "PPU ADDR lookup table", 2),
                        //new UnknownRange(0x076D, 0x40),
                        new UnknownRange(0x076D, 0x1),
                        new DescribedRange(0x076E, 0x3F, "Looks like a copy of code at $FAC9, probably chaff"),
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0700, "sDynamicPage1E");
                annotations.RecordLabel(0x07A0, "UNSUB_1E_07A0");
                annotations.RecordSectionHeader(0x07A0, "------");
                annotations.RecordLabel(0x075B, "loop_1E_01");
                annotations.RecordLabel(0x07A8, "skip_1E_01");
                annotations.RecordLabel(0x07AD, "skip_1E_02");
                annotations.RecordLabel(0x070B, "skip_1E_03");
                annotations.RecordLabel(0x0789, "skip_1E_04");
                annotations.RecordLabel(0x0751, "skip_1E_05");
                annotations.RecordLabel(0x0749, "skip_1E_06");
                annotations.RecordLabel(0x0747, "skip_1E_07");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $1E:Loaded while playing:Replaced by $60 on level 11");
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(3, 0xBA0), 0x100, output, annotations, new Range[] {
                        new UnknownRange(0x078E, 0x12),
                        new DescribedRange(0x07C2, 0x2C, "Min/max level horizontal ranges by level", 4),
                        new DescribedRange(0x07EE, 0x04, "Min/max horizontal range of bonus/warps", 4),
                        new UnknownRange(0x07F2, 0xE)
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0700, "sDynamicPage24");
                annotations.RecordLabel(0x070B, "sStarFieldB");
                annotations.RecordLabel(0x070D, "loop_24_01");
                annotations.RecordLabel(0x078D, "loop_24_02");
                annotations.RecordLabel(0x071B, "skip_24_01");
                annotations.RecordLabel(0x0739, "skip_24_02");
                annotations.RecordLabel(0x0753, "skip_24_03");
                annotations.RecordLabel(0x0768, "skip_24_04");
                annotations.RecordLabel(0x0774, "skip_24_05");
                annotations.RecordLabel(0x0776, "skip_24_06");
                annotations.RecordLabel(0x077C, "skip_24_07");
                annotations.RecordLabel(0x0788, "skip_24_08");
                annotations.RecordLabel(0x078B, "skip_24_09");
                annotations.RecordLabel(0x079A, "skip_24_10");
                annotations.RecordLabel(0x07AF, "skip_24_11");
                annotations.RecordLabel(0x07CE, "skip_24_12");
                annotations.RecordLabel(0x07D2, "skip_24_13");
                annotations.RecordLabel(0x07DC, "skip_24_14");
                annotations.RecordLabel(0x07E5, "skip_24_15");
                annotations.RecordLabel(0x07F1, "skip_24_16");
                annotations.RecordLabel(0x07FD, "rts_24_01");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $24:Startup/level end/game over/warp/bonus/end credits");
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(5, 0x670), 0x100, output, annotations, new Range[] {
                        new DescribedRange(0x7FE, 0x02, "Chaff")
                    }, "SPARKLE");
                }

                annotations.ClearRAM();
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $42:Only during level start");
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(7, 0x4C0), 0x80, output, annotations, new Range[] {
                        new DescribedRange(0x0700, 0x06, "Lid contents starting index"),
                        new LidManifestRange(0x0706, 11, 1),
                        new LidManifestRange(0x071C, 11, 2),
                        new LidManifestRange(0x0732, 15, 3),
                        new LidManifestRange(0x0750, 13, 4),
                        new LidManifestRange(0x076A, 10, 5),
                        new LidManifestRange(0x077E, 1, 6),
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0700, "sDynamicPage48");
                annotations.RecordLabel(0x071A, "skip_48_01");
                annotations.RecordLabel(0x072E, "skip_48_02");
                annotations.RecordLabel(0x0745, "skip_48_03");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $48:Level/bonus/pond start");
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(4, 0xCF0), 0x50, output, annotations, new Range[] {
                        new DescribedRange(0x074C, 0x05, "Chaff")
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordSectionHeader(0x0700, "Spaceship Body AI");
                annotations.RecordLabel(0x0700, "ei_sbody");
                annotations.RecordSectionHeader(0x077C, "Spaceship Canopy AI");
                annotations.RecordLabel(0x077C, "ei_glass");
                annotations.RecordLabel(0x0709, "UNSUB_60_0709");
                annotations.RecordLabel(0x076E, "UNSUB_60_076E");
                annotations.RecordLabel(0x076C, "UNSUB_60_076C");
                annotations.RecordLabel(0x079E, "far_60_01");
                annotations.RecordLabel(0x0768, "far_60_02");
                annotations.RecordLabel(0x0721, "rts_60_01");
                annotations.RecordLabel(0x07A5, "skip_60_01");
                annotations.RecordLabel(0x078F, "skip_60_02");
                annotations.RecordLabel(0x07AB, "skip_60_03");
                annotations.RecordLabel(0x0715, "skip_60_04");
                annotations.RecordLabel(0x0722, "skip_60_05");
                annotations.RecordLabel(0x0779, "skip_60_06");
                annotations.RecordLabel(0x074A, "skip_60_07");
                annotations.RecordLabel(0x075E, "skip_60_08");
                annotations.RecordInlineComment(0x0776, "Play pulse SFX $48 'Spaceship' or SFX $4C 'Asteriod fall'");
                annotations.RecordInlineComment(0x0779, "Play pulse SFX $48 'Spaceship' or SFX $4C 'Asteriod fall'");
                annotations.RecordUnconditionalBranch(0x07A3);
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $60:Only on level 11");
                    disassembler.Disassemble(0x0700, ChrRomFileOffset(6, 0x846), 0xB0, output, annotations, new UnknownRange[] { }, "EI_SHIP");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x0600, "sCfgEndCredits");
                annotations.RecordLabel(0x0624, "loop_2A_01");
                annotations.RecordInlineComment(0x0622, "Copy arrangements into RAM for sCopyBgImage");
                annotations.RecordInlineComment(0x062F, "Call sCopyBgImage: Transfer 'Moon' arrangement to PPU");
                annotations.RecordInlineComment(0x0634, "Call sCopyBgImage: Transfer 'Moon' arrangement to PPU");
                annotations.RecordInlineComment(0x063D, "Print 'hippety hop' paragraph");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $2A:Only during end credits");
                    disassembler.Disassemble(0x0600, ChrRomFileOffset(5, 0x770), 0xF4, output, annotations, new Range[] {
                        new BackgroundArrangementRange(0x66A, "Moon"),
                        new BackgroundArrangementRange(0x67E, "Mountain"),
                        new StringRange(0x69A),
                        new StringRange(0x6A7),
                        new StringRange(0x6B4),
                        new StringRange(0x6C3),
                        new StringRange(0x6D3),
                        new StringRange(0x6DC),
                        new StringRange(0x6E2),
                        new DescribedRange(0x06F3, 0x01, "Chaff")
                    }, "UNKN");
                }

                annotations.ClearRAM();
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $3C:Only on level 11");
                    disassembler.Disassemble(0x06A0, ChrRomFileOffset(4, 0xB60), 0x60, output, annotations, new Range[] {
                        new SpriteLayoutRange(0x06A0, "Spaceship body"),
                        new SpriteLayoutRange(0x06EC, "Spaceship canopy"),
                        new DescribedRange(0x06FD, 0x03, "Chaff")
                    }, "UNKN");
                }

                annotations.ClearRAM();
                annotations.RecordLabel(0x03FF, "tkCfgTally");
                annotations.RecordLabel(0x0402, "sSwitchToTally");
                annotations.RecordLabel(0x04E2, "cMState_Tally");
                annotations.RecordSectionHeader(0x06A5, "Method to set initial configuration of tally machine state");
                annotations.RecordLabel(0x06A5, "sCfgTally");
                annotations.RecordLabel(0x0425, "far_00_01");
                annotations.RecordLabel(0x0743, "skip_00_01");
                annotations.RecordLabel(0x0708, "skip_00_02");
                annotations.RecordLabel(0x06DD, "skip_00_03");
                annotations.RecordLabel(0x06E2, "skip_00_04");
                annotations.RecordLabel(0x0715, "skip_00_05");
                annotations.RecordLabel(0x04FE, "skip_00_06");
                annotations.RecordLabel(0x04F7, "skip_00_07");
                annotations.RecordLabel(0x0517, "skip_00_08");
                annotations.RecordLabel(0x052F, "skip_00_09");
                annotations.RecordLabel(0x054B, "skip_00_10");
                annotations.RecordLabel(0x055D, "skip_00_11");
                annotations.RecordLabel(0x0566, "skip_00_12");
                annotations.RecordLabel(0x0581, "skip_00_13");
                annotations.RecordLabel(0x056F, "skip_00_14");
                annotations.RecordLabel(0x0571, "skip_00_15");
                annotations.RecordLabel(0x058F, "skip_00_16");
                annotations.RecordLabel(0x05A3, "skip_00_17");
                annotations.RecordLabel(0x05AA, "skip_00_18");
                annotations.RecordLabel(0x05C4, "skip_00_19");
                annotations.RecordLabel(0x05D6, "skip_00_20");
                annotations.RecordLabel(0x05E4, "skip_00_21");
                annotations.RecordLabel(0x05F3, "skip_00_22");
                annotations.RecordLabel(0x05FA, "skip_00_23");
                annotations.RecordLabel(0x0611, "skip_00_24");
                annotations.RecordLabel(0x0614, "skip_00_25");
                annotations.RecordLabel(0x0616, "skip_00_26");
                annotations.RecordLabel(0x061F, "skip_00_27");
                annotations.RecordLabel(0x0622, "skip_00_28");
                annotations.RecordLabel(0x062E, "skip_00_29");
                annotations.RecordLabel(0x0630, "skip_00_30");
                annotations.RecordLabel(0x064A, "skip_00_31");
                annotations.RecordLabel(0x0672, "skip_00_32");
                annotations.RecordLabel(0x067F, "skip_00_33");
                annotations.RecordLabel(0x0687, "skip_00_34");
                annotations.RecordLabel(0x043A, "skip_00_35");
                annotations.RecordLabel(0x0442, "skip_00_36");
                annotations.RecordLabel(0x044F, "skip_00_37");
                annotations.RecordLabel(0x04DF, "skip_00_38");
                annotations.RecordLabel(0x04AF, "skip_00_39");
                annotations.RecordLabel(0x04AB, "skip_00_40");
                annotations.RecordLabel(0x04C6, "skip_00_41");
                annotations.RecordLabel(0x0464, "skip_00_42");
                annotations.RecordLabel(0x047F, "skip_00_43");
                annotations.RecordLabel(0x048C, "skip_00_44");
                annotations.RecordLabel(0x048F, "skip_00_45");
                annotations.RecordLabel(0x04A7, "skip_00_46");
                annotations.RecordLabel(0x0452, "skip_00_47");
                annotations.RecordLabel(0x0696, "tkDoneMStateB");
                annotations.RecordLabel(0x075F, "loop_00_01");
                annotations.RecordLabel(0x06AA, "loop_00_02");
                annotations.RecordLabel(0x073C, "loop_00_03");
                annotations.RecordLabel(0x0591, "loop_00_04");
                annotations.RecordLabel(0x064C, "loop_00_05");
                annotations.RecordLabel(0x042C, "loop_00_06");
                annotations.RecordLabel(0x0451, "rts_00_01");
                annotations.RecordLabel(0x04C9, "rts_00_02");
                annotations.RecordLabel(0x04AE, "rts_00_03");
                annotations.RecordLabel(0x0456, "00_0452_ITER");
                annotations.RecordLabel(0x04D0, "UNSUB_00_04D0");
                annotations.RecordLabel(0x070B, "sStarFieldB");
                annotations.RecordInlineComment(0x0581, "This method is actually in blit block $24");
                annotations.RecordUnconditionalBranch(0x05F8);
                annotations.RecordInlineComment(0x06F2, "Print 'level 00' and '000000's");
                annotations.RecordInlineComment(0x0705, "Print current level's exclamation");
                annotations.RecordInlineComment(0x0719, "Print either 'completed' or 'game over' and 'final score'");
                annotations.RecordInlineComment(0x075F, "Load 'score rollup' SFX into RAM");
                annotations.RecordInlineComment(0x0631, "Load destination address into strings at $0413");
                annotations.RecordInlineComment(0x064C, "Load player score and rollup buffer into strings at $0413");
                annotations.RecordInlineComment(0x0676, "Check if either player has pressed a button, and if so, restart the game");
                annotations.RecordInlineComment(0x06D2, "Overwrite '00' with actual level number in string at $0409");
                annotations.RecordSectionHeader(0x04E2, "TALLY machine state");
                annotations.RecordInlineComment(0x04DC, "Repeat pulse SFX $24/$26/$28 'Pibbley count low/medium/high'");
                annotations.RecordInlineComment(0x04DF, "Repeat pulse SFX $24/$26/$28 'Pibbley count low/medium/high'");
                annotations.RecordInlineComment(0x05E6, "Repeat pulse SFX $2A 'Score roll-up pulse'");
                annotations.RecordInlineComment(0x05E9, "Repeat pulse SFX $2A 'Score roll-up pulse'");
                annotations.RecordInlineComment(0x05EE, "Repeat noise SFX $2C 'Score roll-up noise'");
                foreach (var output in outputs)
                {
                    output.WriteSectionHeader("BLIT $00:Only on level end/game over screen");
                    disassembler.Disassemble(0x03FF, ChrRomFileOffset(3, 0x390), 0x400, output, annotations, new Range[] {
                        new StringRange(0x0409),
                        new StringRange(0x0413),
                        new StringRange(0x041C),
                        new DescribedRange(0x04CA, 0x06, "PPU ADDR low byte"),
                        new DescribedRange(0x0699, 0x04, "Destination addresses for strings at $0413", 0x02),
                        new DescribedRange(0x069D, 0x04, "Offsets to ones digit of score RAM"),
                        new DescribedRange(0x06A1, 0x04, "Offsets/3 to base address $B450 (Score -1 and 1)"),
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
                    }, "UNKN");
                }
            }
                    
            DumpStrip(fileData, PrgRomFileOffset(0xA8E3), 0x1C, "Tile 0 Grass A.arr");
            DumpStrip(fileData, PrgRomFileOffset(0xA919), 0x24, "Tile 1 Grass B.arr");
            DumpStrip(fileData, PrgRomFileOffset(0xA957), 0x0C, "Tile 2 Lid A.arr");
            DumpStrip(fileData, PrgRomFileOffset(0xA97D), 0x38, "Tile 3 Lid B.arr");
            DumpStrip(fileData, PrgRomFileOffset(0xA9CF), 0x24, "Tile 4 Dispenser and Moon.arr");
            DumpStrip(fileData, PrgRomFileOffset(0xAA0D), 0x54, "Tile 5 Stone and Ice.arr");
            DumpStrip(fileData, PrgRomFileOffset(0xAA7B), 0x0C, "Tile 6 Spikes A.arr");
            DumpStrip(fileData, PrgRomFileOffset(0xAAA1), 0x58, "Tile 7 Spikes B.arr");
            DumpStrip(fileData, PrgRomFileOffset(0xAB13), 0x1C, "Tile 8 Scale.arr");
            DumpStrip(fileData, PrgRomFileOffset(0xAB49), 0x0C, "Tile 9 Sloped Ice A.arr");
            DumpStrip(fileData, PrgRomFileOffset(0xABBD), 0x24, "Tile A Water.arr");
            DumpStrip(fileData, PrgRomFileOffset(0xAB6F), 0x34, "Tile B Sloped Ice B.arr");

            Process.Start("disassembly.txt");
        }

        private static void DisableBigfootHealthRegeneration(byte[] data)
        {
            data[PrgRomFileOffset(0x0BE97)] = 0xEA;
            data[PrgRomFileOffset(0x0BE98)] = 0xEA;
            data[PrgRomFileOffset(0x0BE99)] = 0xEA;
        }

        private static byte[] DecompressBonusWarpMap(byte[] fileData, int startOffset)
        {
            var result = new byte[256];
            var index = 0;

            while (index < 256)
            {
                var tile = fileData[startOffset++];
                var count = fileData[startOffset++];

                while (count > 0)
                {
                    result[index++] = tile;
                    count -= 1;
                    if (index == 256) break;
                }
            }
            return result;
        }

        private static void DumpMap(byte[] fileData, byte[] mapData, string filename, int tileSize)
        {
            int width = mapData.Length == 4096 ? 64 : 16;
            int height = mapData.Length == 4096 ? 64 : 16;

            var output = new Bitmap(tileSize * width, tileSize * height);
            using (var g = Graphics.FromImage(output))
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        DumpSnakeMountainMapSquare(fileData, mapData, g, x, y, tileSize);
                    }
                }

                if (width == 64) // brittle!
                {
                    for (int offset = 0x08F6; offset < 0x0E59; offset += 7)
                    {
                        var template = new EntityTemplate(fileData, ChrRomFileOffset(6, offset));
                        DumpEntityTemplate(template, g, tileSize);
                    }
                }
            }
            output.Save(filename, ImageFormat.Png);
        }

        private static void DumpEntityTemplate(EntityTemplate template, Graphics g, int tileSize)
        {
            var state = g.Save();
            g.TranslateTransform((int)(tileSize * template.X), (int)(tileSize * template.Y));
            {
                g.FillRectangle(Brushes.Orange, -1, -1, 2, 2);
                using (var font = new Font(FontFamily.GenericMonospace, tileSize / 8.0f))
                    g.DrawString(template.Description, font, Brushes.Orange, 2, 0);
            }
            g.Restore(state);
        }

        private static void DumpSnakeMountainMapSquare(byte[] fileData, byte[] mapData, Graphics g, int x, int y, int tileSize)
        {
            var tileColors = new Color[0xC];
            int width = mapData.Length == 4096 ? 64 : 16;
            int height = mapData.Length == 4096 ? 64 : 16;

            tileColors[0x0] = Color.FromArgb(0x00, 0x55, 0x00); // Grass/ocean/darkness
            tileColors[0x1] = Color.FromArgb(0x00, 0x77, 0x00); // Grass/ocean/darkness (dark)
            tileColors[0x2] = Color.FromArgb(0x55, 0x00, 0x55); // Lid
            tileColors[0x3] = Color.FromArgb(0x77, 0x00, 0x77); // Lid (dark)
            tileColors[0x4] = Color.FromArgb(0xAA, 0xAA, 0x00); // Scale / moon cheese
            tileColors[0x5] = Color.FromArgb(0x55, 0x55, 0x55); // Stone/ice
            tileColors[0x6] = Color.FromArgb(0xAA, 0x00, 0x00); // Spikes
            tileColors[0x7] = Color.FromArgb(0x00, 0x00, 0xAA); // Spikes 2
            tileColors[0x8] = Color.FromArgb(0xFF, 0xFF, 0x55); // Dispenser
            tileColors[0x9] = Color.FromArgb(0x55, 0x77, 0x55); // Sloped ice A
            tileColors[0xA] = Color.FromArgb(0x55, 0x55, 0xFF); // Water            
            tileColors[0xB] = Color.FromArgb(0x55, 0x55, 0x77); // Sloped ice B

            Pen lightPen = new Pen(Color.FromArgb(0x80, Color.White));
            Pen darkPen = new Pen(Color.FromArgb(0x80, Color.Black));

            var state = g.Save();
            g.TranslateTransform(tileSize * x, tileSize * y);
            {
                var isPeak = x <= 28 && y >= 36;
                var mapOffset = y * width + x;
                var tileIndex = mapData[mapOffset];
                var tileHeight = (int)fileData[PrgRomFileOffset(0xD069 + tileIndex)];

                var tileType = fileData[PrgRomFileOffset((isPeak ? 0xCFEA : 0xCF6A) + tileIndex / 2)];
                if (tileIndex % 2 == 0)
                    tileType = (byte)(tileType & 0x0F);
                else
                    tileType = (byte)(tileType >> 4);

                if (tileType == 0x0 && tileHeight == 0x0)
                {
                    tileType = 0xA;
                }

                if (tileType < 4 && ((x ^ y) & 1) == 0)
                {
                    tileType += 1;
                }

                using (var brush = new SolidBrush(tileColors[tileType]))
                    g.FillRectangle(brush, 0, 0, tileSize, tileSize);

                if (x > 0)
                {
                    var otherTileIndex = mapData[mapOffset - 1];
                    var heightDelta = (int)fileData[PrgRomFileOffset(0xD069 + tileIndex)]
                        - (int)fileData[PrgRomFileOffset(0xD069 + otherTileIndex)];

                    if (tileType == 0xB && isPeak && heightDelta == -1)
                        heightDelta = 0;

                    if (heightDelta > 0)
                        g.DrawLine(lightPen, 0, 0, 0, tileSize-1);
                    if (heightDelta < 0)
                        g.DrawLine(darkPen, 0, 0, 0, tileSize-1);
                }
                if (x +1 < width)
                {
                    var otherTileIndex = mapData[mapOffset + 1];
                    var heightDelta = (int)fileData[PrgRomFileOffset(0xD069 + tileIndex)]
                        - (int)fileData[PrgRomFileOffset(0xD069 + otherTileIndex)];

                    if (tileType == 0xB && isPeak && heightDelta == 1)
                        heightDelta = 0;

                    if (heightDelta > 0)
                        g.DrawLine(lightPen, tileSize - 1, 0, tileSize - 1, tileSize - 1);
                    if (heightDelta < 0)
                        g.DrawLine(darkPen, tileSize - 1, 0, tileSize - 1, tileSize - 1);
                }

                if (y > 0)
                {
                    var otherTileIndex = mapData[mapOffset - height];
                    var heightDelta = (int)fileData[PrgRomFileOffset(0xD069 + tileIndex)]
                        - (int)fileData[PrgRomFileOffset(0xD069 + otherTileIndex)];
                    
                    if (tileType == 0x9 && isPeak && heightDelta == 1)
                        heightDelta = 0;

                    if (heightDelta > 0)
                        g.DrawLine(lightPen, 0, 0, tileSize - 1, 0);
                    if (heightDelta < 0)
                        g.DrawLine(darkPen, 0, 0, tileSize - 1, 0);
                }
                if (y + 1 < height)
                {
                    var otherTileIndex = mapData[mapOffset + height];
                    var heightDelta = (int)fileData[PrgRomFileOffset(0xD069 + tileIndex)]
                        - (int)fileData[PrgRomFileOffset(0xD069 + otherTileIndex)];

                    if (tileType == 0x9 && isPeak && heightDelta == -1)
                        heightDelta = 0;

                    if (heightDelta > 0)
                        g.DrawLine(lightPen, 0, tileSize - 1, tileSize - 1, tileSize - 1);
                    if (heightDelta < 0)
                        g.DrawLine(darkPen, 0, tileSize - 1, tileSize - 1, tileSize - 1);
                }

                g.FillRectangle(Brushes.Black,  0,  0, 1, 1);
                g.FillRectangle(Brushes.Black,  0, tileSize - 1, 1, 1);
                g.FillRectangle(Brushes.Black, tileSize - 1,  0, 1, 1);
                g.FillRectangle(Brushes.Black, tileSize - 1, tileSize - 1, 1, 1);
            }
            g.Restore(state);
        }

        private static void DisassemblePrgRom(Annotations annotations, OpcodeDisassembler disassembler, IDisassemblerOutput output)
        {
            var ranges = new Range[] {
                    new DescribedRange(0x822B, 0x06, "Data for SFX $2A 'score rollup (pulse)'"),
                    new DescribedRange(0x869A, 0x06, "Unknown data, accessed in triples", 3),
                    new DescribedRange(0x8B0E, 0x80, "AI jump table", 2),
                    new DescribedRange(0x8B8E, 0x40, "Entity control bits(?)", 0x01, true),
                    new DescribedRange(0x8C4D, 0x0F, "Rotates controller ordinals one-eighth counter-clockwise (1->5->4->6->2->A->8->9->1, 0->0)"),
                    new DescribedRange(0x8C5C, 0x0B, "Song list by level"),
                    new UnknownRange(0x8C67, 0x2),
                    new DescribedRange(0x9026, 0x0C, "Controls for player lives remaining drop down's score flicker effect", 2),
                    new DropDownStringListing(0x903F, "Drop down 00 string listing (GAME OVER)"),
                    new DropDownStringListing(0x904D, "Drop down 0E string listing (#UP ###### # LEFT)"),
                    new DropDownStringListing(0x9067, "Drop down 28 string listing (HOLD)"),
                    new DropDownStringListing(0x906F, "Drop down 30 string listing (TIME OUT)"),
                    new DropDownStringListing(0x907D, "Drop down 3E string listing (ALL GONE)"),
                    new DropDownStringListing(0x908B, "Drop down 4C string listing (PLAY ON #)"),
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
                    new DescribedRange(0x94F5, 0x10, "Snake arrangement by facing angle (bit 8 set = HFlip)"),
                    new DescribedRange(0x9505, 0x09, "Song -> CHR ROM page lookup"),
                    new UnknownRange(0x950E, 0x07),
                    new UnknownRange(0x9515, 0x0C),
                    new UnknownRange(0x9521, 0x0C),
                    new UnknownRange(0x952D, 0x18),
                    new DescribedRange(0x9545, 0x18, "Offsets into data at $FC4F by snake layout/hflip flag ($0A-$11 not used)", 0x08),
                    new DescribedRange(0x955D, 0x22, "Relative offsets for pibblies being eaten and wind-up keys", 2),
                    new UnknownRange(0x957F, 0x05),
                    new DescribedRange(0x95F4, 0x05, "Snake pain animation cycle"),
                    new DescribedRange(0x95F9, 0x05, "Snake pain animation attribute cycle"),
                    new DescribedRange(0x9679, 0xB, "Level minimum Z before death, high nybble"),
                    new DescribedRange(0x9684, 0xB, "Level minimum Z before death, low byte"),
                    new DescribedRange(0x96C7, 0x2C, "Pre-recorded input for snakes entering level"),
                    new UnknownRange(0x9D21, 0x02),
                    new DescribedRange(0x9D23, 0x0A, "Pibbley chunk AI by level"),
                    new UnknownRange(0x9D2D, 0x0A),
                    new DescribedRange(0x9D37, 0x0A, "Pibbley chunk sprite by level"),
                    new UnknownRange(0x9D41, 0x04),
                    new SpriteLayoutRange(0x9E57, "6E Fish tail"),
                    new SpriteLayoutRange(0x9E62, "6D Clock"),
                    new SpriteLayoutRange(0x9E6F, "94 Metal tree 2"),
                    new SpriteLayoutRange(0x9E79, "95 Metal tree 3"),
                    new SpriteLayoutRange(0x9E86, "96 Metal tree 4"),
                    new DescribedRange(0x9F09, 0x14, "Water jet animation cycle"),
                    new DescribedRange(0x9F6D, 0x05, "Unknown; vEnt0E for rollers; base address $9F69"),
                    new DescribedRange(0xA49F, 0x1A, "Background attribute table indices (3 runs terminated by #$FF)"),
                    new DescribedRange(0xA4B9, 0x1A, "Background attribute table values"),
                    new DescribedRange(0xA4D3, 0x08, "Unknown", 2),
                    new DescribedRange(0xA8AD, 0x0C, "Low byte address into data block below"),
                    new DescribedRange(0xA8B9, 0x0C, "High byte address into data block below"),
                    new DescribedRange(0xA8C5, 0x04, "Probably chaff"),
                    new DescribedRange(0xA8C9, 0x1A, "Map tile type 0 - Grass A control", 0x0D),
                    new DescribedRange(0xA8E3, 0x1C, "Map tile type 0 - Grass A tiles", 0x04),
                    new DescribedRange(0xA8FF, 0x1A, "Map tile type 1 - Grass B control", 0x0D),
                    new DescribedRange(0xA919, 0x24, "Map tile type 1 - Grass B tiles", 0x04),
                    new DescribedRange(0xA93D, 0x1A, "Map tile type 2 - Lid A control", 0x0D),
                    new DescribedRange(0xA957, 0x0C, "Map tile type 2 - Lid A tiles", 0x04),
                    new DescribedRange(0xA963, 0x1A, "Map tile type 3 - Lid B control", 0x0D),
                    new DescribedRange(0xA97D, 0x38, "Map tile type 3 - Lid B tiles", 0x04),
                    new DescribedRange(0xA9B5, 0x1A, "Map tile type 4 - Dispenser/moon control", 0x0D),
                    new DescribedRange(0xA9CF, 0x24, "Map tile type 4 - Dispenser/moon tiles", 0x04),
                    new DescribedRange(0xA9F3, 0x1A, "Map tile type 5 - Stone/ice control", 0x0D),
                    new DescribedRange(0xAA0D, 0x54, "Map tile type 5 - Stone/ice tiles", 0x04),
                    new DescribedRange(0xAA61, 0x1A, "Map tile type 6 - Spikes A control", 0x0D),
                    new DescribedRange(0xAA7B, 0x0C, "Map tile type 6 - Spikes A tiles", 0x04),
                    new DescribedRange(0xAA87, 0x1A, "Map tile type 7 - Spikes B control", 0x0D),
                    new DescribedRange(0xAAA1, 0x58, "Map tile type 7 - Spikes B tiles", 0x04),
                    new DescribedRange(0xAAF9, 0x1A, "Map tile type 8 - Scale control", 0x0D),
                    new DescribedRange(0xAB13, 0x1C, "Map tile type 8 - Scale tiles", 0x04),
                    new DescribedRange(0xAB2F, 0x1A, "Map tile type 9 - Sloped ice A control", 0x0D),
                    new DescribedRange(0xAB49, 0x0C, "Map tile type 9 - Sloped ice A tiles", 0x04),
                    new DescribedRange(0xAB55, 0x1A, "Map tile type B - Sloped ice B control", 0x0D),
                    new DescribedRange(0xAB6F, 0x34, "Map tile type B - Sloped ice B tiles", 0x04),
                    new DescribedRange(0xABA3, 0x1A, "Map tile type A - Water control", 0x0D),
                    new DescribedRange(0xABBD, 0x24, "Map tile type A - Water tiles", 0x04),
                    new UnknownRange(0xAB55, 0x8C),
                    new DescribedRange(0xAED6, 0x20, "Data for bell/tail dispenser", 0x02),
                    new DescribedRange(0xAEF6, 0x9, "Falling water palettes", 0x3),
                    new DescribedRange(0xAEFF, 0xB, "Drop down sprite palette (for text); base address $AEEE", 4),
                    new SpriteLayoutRange(0xAF0A, "97 Ice cube 1"),
                    new SpriteLayoutRange(0xAF17, "98 Ice cube 2"),
                    new SpriteLayoutRange(0xAF24, "99 Bubble"),
                    new SpriteLayoutRange(0xAF31, "9A Icy Pibbley hole"),
                    new DescribedRange(0xAF71, 0x4, "Lid animation cycle"),
                    new DescribedRange(0xAF75, 0x4, "Lid animation attribute cycle"),
                    new DescribedRange(0xAF79, 0x04, "Offset from $AF7D"),
                    new DescribedRange(0xAF7D, 0x0E, "Bonus 1 Pibbley path", 2),
                    new DescribedRange(0xAF8B, 0x10, "Bonus 2 Pibbley path", 2),
                    new DescribedRange(0xAF9B, 0x12, "Bonus 3 Pibbley path", 2),
                    new DescribedRange(0xAFAD, 0x0C, "Bonus 4 Pibbley path", 2),
                    new DescribedRange(0xAFBC, 0x0A, "Nibbley Pibbley entity types by level"),
                    new DescribedRange(0xB23F, 0x04, "Unknown; base address $B23E"),
                    new DescribedRange(0xB243, 0x04, "Unknown; base address $B242"),
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
                    new DescribedRange(0xB498, 0x04, "Pibbley score layout by colour"),
                    new DescribedRange(0xB49C, 0x0A, "Looks like code, but isn't referenced?"),
                    new DescribedRange(0xB58D, 0x08, "Pibblejogger animation cycle index by facing angle"),
                    new DescribedRange(0xB595, 0x10, "Pibblejogger animation cycle", 4),
                    new DescribedRange(0xB5A5, 0x08, "Pibblebat animation cycle", 2),
                    new DescribedRange(0xB5AD, 0x4, "Pibblecopter animation cycle"),
                    new UnknownRange(0xB6D2, 0x0C),
                    new DescribedRange(0xB72C, 0x06, "Snakedozer animation cycle"),
                    new DescribedRange(0xB7F4, 0x06, "Bladez animation cycle"),
                    new UnknownRange(0xB798, 0x05),
                    new DescribedRange(0xB860, 0x09, "Metal tree animation cycle"),
                    new DescribedRange(0xB8BC, 0x09, "Splash animation cycle"),
                    new DescribedRange(0xB94E, 0x0B, "Shark animation cycle"),
                    new UnknownRange(0xB983, 0x03),
                    new DescribedRange(0xBA72, 0x04, "Crazy seat arrangement by facing angle"),
                    new DescribedRange(0xBA76, 0x04, "Crazy seat arrangement attribute by facing angle"),
                    new UnknownRange(0xBA7A, 0x05),
                    new DescribedRange(0xBAB1, 0x0F, "Pin cushion animation cycle"),
                    new DescribedRange(0xBE2F, 0x08, "Explosion animation cycle"),
                    new DescribedRange(0xBE74, 0x04, "Shrapnel attribute blending mask"),
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
                    new DescribedRange(0xC0B1, 0x04, "Tail segment floating away animation cycle"),
                    new DescribedRange(0xC1CA, 0x05, "Powerup sprite layouts"),
                    new DescribedRange(0xC1CF, 0x08, "Powerup SFX (indexed from $C162)"),
                    new UnknownRange(0xC2A5, 0x05),
                    new UnknownRange(0xC34C, 0x04),
                    new DescribedRange(0xC35B, 0x06, "RLE-encoded PPU attribute table 00", 2),
                    new DescribedRange(0xC361, 0x06, "RLE-encoded PPU attribute table 06", 2),
                    new DescribedRange(0xC367, 0x16, "RLE-encoded PPU attribute table 0C", 2),
                    new DescribedRange(0xC37D, 0x14, "RLE-encoded PPU attribute table 22", 2),
                    new DescribedRange(0xC391, 0x0C, "RLE-encoded PPU attribute table 36", 2),
                    new DescribedRange(0xC39D, 0x04, "RLE-encoded PPU attribute table 42", 2),
                    new DescribedRange(0xC3A1, 0x06, "RLE-encoded PPU attribute table 46", 2),
                    new DescribedRange(0xC410, 0x66, "CHR ROM blit index", 6),
                    new DescribedRange(0xC767, 0x20, "Record hop cycle"),
                    new UnknownRange(0xC787, 0x04),
                    new DescribedRange(0xC894, 0x08, "Sprite layouts for score values"),
                    new DescribedRange(0xC89C, 0x08, "Unknown (addressed via C827 + 75 through 7C)"),
                    new DescribedRange(0xC8A4, 0x08, "Unknown (addressed via C82F + 75 through 7C)"),
                    new DescribedRange(0xC8E4, 0xA, "Unknown (addressed via C8DE)"),
                    new DescribedRange(0xC8EE, 0x8, "Unknown (addressed via C8E8)"),
                    new UnknownRange(0xCBA2, 0x04),
                    new DescribedRange(0xCCCB, 0x04, "Pibblefish animation cycle"),
                    new DescribedRange(0xCCCF, 0x08, "Unkn 10/11 for pibblefish", 0x02),
                    new DescribedRange(0xCD3D, 0x0F, "Lid contents by type"),
                    new DescribedRange(0xCE50, 0xF0, "Unknown; addressed via CE3F/CE40", 0x10),
                    new SpriteLayoutRange(0xCF63, "93 Metal tree 1"),
                    new DescribedRange(0xCF6A, 0x80, "Map tile types, non-ice", 8),
                    new DescribedRange(0xCFEA, 0x7F, "Map tile types, ice", 8),
                    new DescribedRange(0xD069, 0x100, "Map tile heights", 16),
                    new DescribedRange(0xD169, 0x01, "Chaff"),
                    new SpriteLayoutRange(0xD16A, "8D Magic carpet 1"),
                    new SpriteLayoutRange(0xD17B, "8E Magic carpet 2"),
                    new SpriteLayoutRange(0xD18C, "8F 90 Water jet 1"),
                    new SpriteLayoutRange(0xD19D, "91 Water jet 2"),
                    new SpriteLayoutRange(0xD1B6, "92 Water jet 3"),
                    new SpriteLayoutRange(0xD222, "70 Tongue extension"),
                    new SpriteLayoutRange(0xD22B, "5E Disk/sphere/snowball B"),
                    new SpriteLayoutRange(0xD238, "12 Bell"),
                    new DescribedRange(0xD2AA, 0x0F, "Map from controller state to inverted controller state"),
                    new DescribedRange(0xD2B9, 0x07, "Looks like code, but unreachable?"),
                    new DescribedRange(0xD2E7, 0x18, "Somehow controls snake tail rendering (tile indices but access unknown)", 0x03),
                    new UnknownRange(0xD2FF, 0x03),
                    new DescribedRange(0xD4A5, 0x0A, "Tail length for flashing segment by level"),
                    new DescribedRange(0xD4AF, 0x0A, "Scale marker offset by level (compare to previous data range)"),
                    new UnknownRange(0xD4DE, 0x03),
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
                    new DescribedRange(0xDC10, 0x09, "SFX $04 Pibbley chomp"),
                    new DescribedRange(0xDC19, 0x14, "SFX $06 Pibbley ejection"),
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
                    new DescribedRange(0xDD27, 0x0E, "SFX $26 Pibbley countdown (medium)"),
                    new DescribedRange(0xDD35, 0x0E, "SFX $24 Pibbley countdown (low)"),
                    new DescribedRange(0xDD43, 0x0E, "SFX $28 Pibbley countdown (high)"),
                    new DescribedRange(0xDD51, 0x12, "SFX $2C Score rollup (noise)"),
                    new DescribedRange(0xDD63, 0x11, "SFX $30 ARRRGGG-"),
                    new DescribedRange(0xDD74, 0x0B, "SFX $32 Pibbley chunk spit"),
                    new DescribedRange(0xDD7F, 0x05, "Data for SFX $34 Crescendo (variant A)"),
                    new DescribedRange(0xDD84, 0x05, "Data for SFX $34 Crescendo (variant B)"),
                    new DescribedRange(0xDD89, 0x05, "Data for SFX $34 Crescendo (variant C)"),
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
                    new DescribedRange(0xE00F, 0x28, "Unknown data (pairs of opposite-direction deltas?)", 0x04),
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
                    new SpriteLayoutRange(0xF5FE, "0A Pibblejogger 1"),
                    new SpriteLayoutRange(0xF60B, "0B Pibblejogger 2"),
                    new SpriteLayoutRange(0xF615, "0C Pibblejogger 3"),
                    new SpriteLayoutRange(0xF622, "0D Pibblejogger 4"),
                    new SpriteLayoutRange(0xF62F, "0E Pibblejogger 5"),
                    new SpriteLayoutRange(0xF639, "0F Pibblejogger 6"),
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
                    new DescribedRange(0xFC1F, 0x20, "Status bar OAM template", 4),
                    new UnknownRange(0xFC3F, 0x08),
                    new UnknownRange(0xFC47, 0x08),
                    new DescribedRange(0xFC4F, 0x20, "Involved in snake tongue somehow", 2),
                    new DescribedRange(0xFC6F, 0x30, "Snake tongue draw data (dX, dY, tongue tile, attrs, tongue tip, attrs)", 6),
                    new DescribedRange(0xFC9F, 0x07, "Tongue something something (accessed at FC9E +1, +3, +5, +7)", 2), // Uses carry bit of successive adds of the constant to do something
                    new DescribedRange(0xFCD6, 0x04, "Spinning wind-up key animation cycle"),
                    new DescribedRange(0xFCDA, 0x04, "Spinning wind-up key animation attribute cycle"),
                    new UnknownRange(0xFFAC, 0x02),
                    new DescribedRange(0xFFFA, 0x02, "Jump vector (NMI)"),
                    new DescribedRange(0xFFFC, 0x02, "Jump vector (RST)"),
                    new DescribedRange(0xFFFE, 0x02, "Jump vector (IRQ)")
            };

            var blocks = new[]
            {
                // --- Sorted ---
                new DisassemblyBlock(0xCF6A, 0xD169, "MAP"),
                new DisassemblyBlock(0xE3C0, 0xF3C0, "MAP"),
                // CHR ROM blitter
                new DisassemblyBlock(0x845B, 0x8460, "BLITTER"),
                new DisassemblyBlock(0xC3B7, 0xC3BF, "BLITTER"),
                new DisassemblyBlock(0xC3D7, 0xC476, "BLITTER"),
                // String printing
                new DisassemblyBlock(0x80B6, 0x80DB, "STRING"),
                new DisassemblyBlock(0xE2C9, 0xE306, "STRING"),
                new DisassemblyBlock(0xE306, 0xE3C0, "STRING"),
                // Audio
                new DisassemblyBlock(0xD54A, 0xDBE6, "AUDIO"),
                new DisassemblyBlock(0x8DB7, 0x8DC6, "AUDIO"),
                new DisassemblyBlock(0x81EA, 0x8209, "AUDIO"),
                new DisassemblyBlock(0x8000, 0x8010, "AUDIO"),
                new DisassemblyBlock(0x9505, 0x950E, "AUDIO"),
                new DisassemblyBlock(0xDE42, 0xDED2, "AUDIO"),
                new DisassemblyBlock(0x8C5C, 0x8C67, "AUDIO"),
                new DisassemblyBlock(0xDBE6, 0xDE12, "AUDIO"),
                new DisassemblyBlock(0x822B, 0x8231, "AUDIO"),
                new DisassemblyBlock(0xD26A, 0xD279, "AUDIO"),
                // Sprite composites and rendering
                new DisassemblyBlock(0x9545, 0x957F, "RENDER"),
                new DisassemblyBlock(0xFC4F, 0xFF61, "RENDER"),
                new DisassemblyBlock(0xF3E7, 0xF51D, "RENDER"),
                new DisassemblyBlock(0x9E57, 0x9E95, "RENDER"),
                new DisassemblyBlock(0xAF0A, 0xAF3D, "RENDER"),
                new DisassemblyBlock(0xCF63, 0xCF6A, "RENDER"),
                new DisassemblyBlock(0xD16A, 0xD1D2, "RENDER"),
                new DisassemblyBlock(0xD222, 0xD245, "RENDER"),
                new DisassemblyBlock(0xDE12, 0xDE42, "RENDER"),
                new DisassemblyBlock(0xDED2, 0xDF1A, "RENDER"),
                new DisassemblyBlock(0xF3C0, 0xF3E7, "RENDER"),
                new DisassemblyBlock(0xF53A, 0xF757, "RENDER"),
                new DisassemblyBlock(0xF759, 0xFA2F, "RENDER"),
                // Interrupt handlers
                new DisassemblyBlock(0xFFFA, 0x10000, "HANDLER"),
                new DisassemblyBlock(0xFF81, 0xFFAC, "HANDLER"),
                new DisassemblyBlock(0xFF61, 0xFF81, "HANDLER"),
                new DisassemblyBlock(0xFFF1, 0xFFFA, "HANDLER"),
                new DisassemblyBlock(0x8251, 0x8295, "HANDLER"),
                new DisassemblyBlock(0xD2E1, 0xD2E7, "HANDLER"),
                // Machine states                
                new DisassemblyBlock(0x80DB, 0x80F7, "MSTATE"),
                new DisassemblyBlock(0xC350, 0xC3A7, "MSTATE"),
                new DisassemblyBlock(0x8128, 0x8150, "MSTATE"),
                new DisassemblyBlock(0x80F7, 0x8128, "MSTATE"),
                new DisassemblyBlock(0x8150, 0x817A, "MSTATE"),
                new DisassemblyBlock(0x81C3, 0x81DB, "MSTATE"),
                new DisassemblyBlock(0xE17C, 0xE209, "MSTATE"),
                new DisassemblyBlock(0x9292, 0x92BB, "MSTATE"),
                new DisassemblyBlock(0x8064, 0x80B6, "MSTATE"),
                new DisassemblyBlock(0x8231, 0x8242, "MSTATE"),
                new DisassemblyBlock(0xD279, 0xD2AA, "MSTATE"),
                new DisassemblyBlock(0x8B07, 0x8B0E, "MSTATE"),
                new DisassemblyBlock(0x8BCE, 0x8C01, "MSTATE"),
                new DisassemblyBlock(0xAE0F, 0xAE8C, "MSTATE"),
                new DisassemblyBlock(0x8DAC, 0x8DB7, "MSTATE"),
                new DisassemblyBlock(0x9032, 0x903F, "MSTATE"),
                new DisassemblyBlock(0x9E95, 0x9EA1, "MSTATE"),
                new DisassemblyBlock(0xF51D, 0xF53A, "MSTATE"),
                new DisassemblyBlock(0xE237, 0xE2C9, "SPARKLE"),
                new DisassemblyBlock(0x84CC, 0x8666, "MSTATEC"),
                new DisassemblyBlock(0xFFAE, 0xFFB5, "MSTATEC"),
                new DisassemblyBlock(0x8C01, 0x8C4D, "MSTATEF"),
                new DisassemblyBlock(0x8C69, 0x8DAC, "MSTATEP"),
                new DisassemblyBlock(0x918B, 0x9292, "MSTATEP"),
                new DisassemblyBlock(0x92BB, 0x92BE, "MSTATEP"),
                new DisassemblyBlock(0x8B0E, 0x8B8E, "MSTATEP"),
                new DisassemblyBlock(0xFA31, 0xFC4F, "MSTATEP"),
                new DisassemblyBlock(0xAEF6, 0xAEFF, "MSTATEP"),
                new DisassemblyBlock(0xD2AA, 0xD2B9, "MSTATEP"),
                new DisassemblyBlock(0x9139, 0x918B, "MSTATEP"),
                new DisassemblyBlock(0x92BE, 0x93B2, "MSTATEP"),
                new DisassemblyBlock(0x9422, 0x9432, "MSTATEP"),
                new DisassemblyBlock(0x93B2, 0x9422, "MSTATEP"),
                new DisassemblyBlock(0x9432, 0x94F5, "MSTATEP"),
                new DisassemblyBlock(0xD245, 0xD26A, "MSTATEP"),
                new DisassemblyBlock(0x8DC6, 0x9032, "MSTATED"),
                new DisassemblyBlock(0x903F, 0x9139, "MSTATED"),
                new DisassemblyBlock(0xAEFF, 0xAF0A, "MSTATED"),

                new DisassemblyBlock(0x849C, 0x84CC, "FADE"),
                new DisassemblyBlock(0xC3BF, 0xC3D7, "FADE"),
                new DisassemblyBlock(0xC587, 0xC592, "FADE"),
                new DisassemblyBlock(0xC5B5, 0xC62C, "FADE"),
                new DisassemblyBlock(0xC592, 0xC5B5, "FADE"),
                new DisassemblyBlock(0x8460, 0x8480, "FADE"),
                new DisassemblyBlock(0xC62C, 0xC634, "FADE"),

                new DisassemblyBlock(0x9F9E, 0x9FB7, "BACKGD"),
                new DisassemblyBlock(0x9FC6, 0xAE0F, "BACKGD"),

                new DisassemblyBlock(0xC2AA, 0xC350, "LID"),
                new DisassemblyBlock(0xC476, 0xC552, "LID"),
                new DisassemblyBlock(0xCCD7, 0xCD4C, "LID"),

                new DisassemblyBlock(0x9610, 0x9CDE, "EI_SNEK"),
                new DisassemblyBlock(0x9D54, 0x9DA3, "EI_SNEK"),
                new DisassemblyBlock(0xE055, 0xE161, "EI_SNEK"),
                new DisassemblyBlock(0x9DA3, 0x9E0B, "EI_SNEK"),
                new DisassemblyBlock(0x95F4, 0x95FE, "EI_SNEK"),
                new DisassemblyBlock(0x8C4D, 0x8C5C, "EI_SNEK"),
                new DisassemblyBlock(0x9D21, 0x9D2D, "EI_SNEK"),
                new DisassemblyBlock(0x9D2D, 0x9D45, "EI_SNEK"),
                new DisassemblyBlock(0xD302, 0xD4AF, "EI_TAIL"),
                new DisassemblyBlock(0xD4B9, 0xD4DE, "EI_TAIL"),
                new DisassemblyBlock(0xD4E1, 0xD4F4, "EI_TAIL"),
                new DisassemblyBlock(0xD2E7, 0xD2FF, "EI_TAIL"),
                new DisassemblyBlock(0x9E0B, 0x9E57, "EI_RCKT"),
                new DisassemblyBlock(0x9EA1, 0x9F1D, "EI_WJET"),
                new DisassemblyBlock(0x9F1D, 0x9F72, "EI_ROLR"),
                new DisassemblyBlock(0xAE8C, 0xAEF6, "EI_BDSP"),
                new DisassemblyBlock(0xAF3D, 0xAF79, "EI_FLID"),
                new DisassemblyBlock(0xC659, 0xC78B, "EI_BNCR"),
                new DisassemblyBlock(0xB272, 0xB315, "EI_PBLY"),
                new DisassemblyBlock(0xAF79, 0xB247, "EI_PBLY"),
                new DisassemblyBlock(0xB58D, 0xB5B1, "EI_PBLY"),
                new DisassemblyBlock(0xB247, 0xB272, "EI_PBLY"),
                new DisassemblyBlock(0xB315, 0xB3D7, "EI_PBLY"),
                new DisassemblyBlock(0xB4A6, 0xB527, "EI_PDSP"),
                new DisassemblyBlock(0xBB14, 0xBB5D, "EI_PIN"),
                new DisassemblyBlock(0xBA7F, 0xBAC0, "EI_CSHN"),
                new DisassemblyBlock(0xBB5D, 0xBB85, "EI_BOMB"),
                new DisassemblyBlock(0xB6E5, 0xB732, "EI_DOZR"),
                new DisassemblyBlock(0xB5B1, 0xB65A, "EI_DOOR"),
                new DisassemblyBlock(0xB65A, 0xB6DE, "EI_SCAL"),
                new DisassemblyBlock(0xB869, 0xB8C5, "EI_RIPL"),
                new DisassemblyBlock(0xBE8E, 0xBF3A, "EI_FOOT"),
                new DisassemblyBlock(0xBFB0, 0xC03E, "EI_FOOT"),
                new DisassemblyBlock(0xBE78, 0xBE83, "EI_FOOT"),
                new DisassemblyBlock(0xBDAF, 0xBDED, "EI_BOOM"),
                new DisassemblyBlock(0xBE2F, 0xBE37, "EI_BOOM"),
                new DisassemblyBlock(0xBE37, 0xBE78, "EI_FLAK"),
                new DisassemblyBlock(0xC1D7, 0xC2AA, "EI_PDSP"),
                new DisassemblyBlock(0xC634, 0xC64E, "EI_ITEM"),
                new DisassemblyBlock(0xC0B5, 0xC1D7, "EI_ITEM"),
                new DisassemblyBlock(0xB8C5, 0xB907, "EI_FLAG"),
                new DisassemblyBlock(0xB907, 0xB959, "EI_JAWS"),
                new DisassemblyBlock(0xB959, 0xB983, "EI_ARGH"),
                new DisassemblyBlock(0xB753, 0xB798, "EI_BLAD"),
                new DisassemblyBlock(0xB7F4, 0xB7FA, "EI_BLAD"),
                new DisassemblyBlock(0xCB48, 0xCBA6, "EI_SPIT"),
                new DisassemblyBlock(0xCBA6, 0xCBBD, "EI_BRTH"),
                new DisassemblyBlock(0xCB2A, 0xCB48, "EI_SKOR"),
                new DisassemblyBlock(0xC9B5, 0xCAEB, "EI_CRPT"),
                new DisassemblyBlock(0xCBBD, 0xCC2C, "EI_WEED"),
                new DisassemblyBlock(0xCC2C, 0xCC4B, "EI_FEGG"),
                new DisassemblyBlock(0xCC4B, 0xCCD7, "EI_FISH"),
                new DisassemblyBlock(0xC8C7, 0xC9B5, "EI_NULL"),
                new DisassemblyBlock(0xC894, 0xC89C, "EI_NULL"),
                new DisassemblyBlock(0xC8B9, 0xC8C7, "EI_NULL"),
                new DisassemblyBlock(0xC7C7, 0xC858, "EI_ANVL"),
                new DisassemblyBlock(0xCAEB, 0xCB2A, "EI_TKLR"),
                new DisassemblyBlock(0xC78B, 0xC7C7, "EI_BDSP"),
                new DisassemblyBlock(0xC858, 0xC894, "EI_WARP"),
                new DisassemblyBlock(0xC048, 0xC0B5, "EI_TFLT"),
                new DisassemblyBlock(0x8022, 0x802F, "EI_SHIP"),
                new DisassemblyBlock(0xB9D5, 0xB9D9, "EI_SEAT"),
                new DisassemblyBlock(0xB860, 0xB869, "EI_SEAT"),
                new DisassemblyBlock(0xB983, 0xB986, "EI_SEAT"),
                new DisassemblyBlock(0xB986, 0xB9A7, "EI_SEAT"),
                new DisassemblyBlock(0xB9B3, 0xB9D5, "EI_SEAT"),
                new DisassemblyBlock(0xB9D9, 0xBA7A, "EI_SEAT"),
                new DisassemblyBlock(0xB7FA, 0xB815, "EI_SEAT"),
                new DisassemblyBlock(0xB815, 0xB860, "EI_COMN"),
                new DisassemblyBlock(0x8B8E, 0x8BCE, "EI_COMN"),
                new DisassemblyBlock(0xBB85, 0xBD77, "EI_COMN"),
                new DisassemblyBlock(0x9584, 0x95DE, "EI_COMN"),
                new DisassemblyBlock(0xC64E, 0xC659, "EI_COMN"),
                new DisassemblyBlock(0xB732, 0xB753, "EI_COMN"),
                new DisassemblyBlock(0xB527, 0xB58D, "EI_COMN"),
                new DisassemblyBlock(0x86B6, 0x899A, "EI_COMN"),
                new DisassemblyBlock(0x869A, 0x86A0, "EI_COMN"),
                new DisassemblyBlock(0x8689, 0x869A, "EI_COMN"),
                new DisassemblyBlock(0x8666, 0x866D, "EI_COMN"),
                new DisassemblyBlock(0x89E3, 0x89FD, "EI_COMN"),
                new DisassemblyBlock(0x86AD, 0x86B6, "EI_COMN"),
                new DisassemblyBlock(0x8A4E, 0x8B07, "EI_COMN"),
                new DisassemblyBlock(0x89FD, 0x8A4E, "EI_COMN"),
                new DisassemblyBlock(0x86A0, 0x86AD, "EI_COMN"),
                new DisassemblyBlock(0x899A, 0x89E3, "EI_COMN"),
                new DisassemblyBlock(0xDF1A, 0xE00F, "EI_COMN"),
                new DisassemblyBlock(0xD4F4, 0xD54A, "EI_COMN"),
                new DisassemblyBlock(0xE161, 0xE17C, "EI_COMN"),
                new DisassemblyBlock(0xE00F, 0xE055, "EI_COMN"),
                new DisassemblyBlock(0xCF40, 0xCF63, "EI_COMN"),
                new DisassemblyBlock(0xCD4C, 0xCF40, "EI_COMN"),
                new DisassemblyBlock(0xC567, 0xC587, "EI_COMN"),
                new DisassemblyBlock(0xBAFC, 0xBB14, "EI_COMN"),
                new DisassemblyBlock(0xBE23, 0xBE2F, "EI_COMN"),
                new DisassemblyBlock(0xBE83, 0xBE8E, "EI_COMN"),
                new DisassemblyBlock(0x8197, 0x81C3, "EI_COMN"),
                new DisassemblyBlock(0x817A, 0x8197, "EI_COMN"),
                new DisassemblyBlock(0xB498, 0xB49C, "EI_COMN"),
                new DisassemblyBlock(0xE209, 0xE237, "EI_COMN"),
                new DisassemblyBlock(0x9CF3, 0x9D21, "EI_COMN"),
                new DisassemblyBlock(0x9F72, 0x9F9E, "EI_COMN"),
                new DisassemblyBlock(0x866D, 0x8689, "EI_COMN"),
                new DisassemblyBlock(0x9FB7, 0x9FC6, "EI_COMN"),
                new DisassemblyBlock(0xB3D7, 0xB498, "EI_COMN"),
                new DisassemblyBlock(0xC89C, 0xC8B9, "EI_COMN"),
                new DisassemblyBlock(0xB6DE, 0xB6E5, "EI_COMN"),
                new DisassemblyBlock(0xB9A7, 0xB9B3, "EI_COMN"),
                new DisassemblyBlock(0xC03E, 0xC048, "EI_COMN"),
                new DisassemblyBlock(0xC552, 0xC567, "EI_COMN"),
                new DisassemblyBlock(0x8295, 0x845B, "INIT"),
                new DisassemblyBlock(0xB79D, 0xB7CD, "INIT"),
                new DisassemblyBlock(0xB7CD, 0xB7F4, "INIT"),
                new DisassemblyBlock(0xFFB5, 0xFFF1, "UTIL"),

                // --- Unsorted ---
                new DisassemblyBlock(0x8010, 0x8022, "UNKN"),
                new DisassemblyBlock(0x802F, 0x8064, "UNKN"),
                new DisassemblyBlock(0x81DB, 0x81EA, "UNKN"),
                new DisassemblyBlock(0x8209, 0x822B, "UNKN"),
                new DisassemblyBlock(0x8242, 0x8251, "UNKN"),
                new DisassemblyBlock(0x8480, 0x849C, "UNKN"),
                new DisassemblyBlock(0x8C67, 0x8C69, "UNKN"),
                new DisassemblyBlock(0x94F5, 0x9505, "UNKN"),
                new DisassemblyBlock(0x950E, 0x9545, "UNKN"),
                new DisassemblyBlock(0x957F, 0x9584, "UNKN"),
                new DisassemblyBlock(0x95DE, 0x95F4, "UNKN"),
                new DisassemblyBlock(0x95FE, 0x9610, "UNKN"),
                new DisassemblyBlock(0x9CDE, 0x9CF3, "UNKN"),
                new DisassemblyBlock(0x9D45, 0x9D54, "UNKN"),
                new DisassemblyBlock(0xB49C, 0xB4A6, "UNKN"),
                new DisassemblyBlock(0xB798, 0xB79D, "UNKN"),
                new DisassemblyBlock(0xBAC0, 0xBAFC, "UNKN"),
                new DisassemblyBlock(0xBD77, 0xBDAF, "UNKN"),
                new DisassemblyBlock(0xBDED, 0xBE23, "UNKN"),
                new DisassemblyBlock(0xBF3A, 0xBFB0, "UNKN"),
                new DisassemblyBlock(0xC3A7, 0xC3B7, "UNKN"),
                new DisassemblyBlock(0xD1D2, 0xD222, "UNKN"),
                new DisassemblyBlock(0xD2C0, 0xD2E1, "UNKN"),
                new DisassemblyBlock(0xD2FF, 0xD302, "UNKN"),
                new DisassemblyBlock(0xD4AF, 0xD4B9, "UNKN"),
                // --- Salty garbage ---
                new DisassemblyBlock(0xBA7A, 0xBA7F, "CHAFF"),
                new DisassemblyBlock(0xD169, 0xD16A, "CHAFF"),
                new DisassemblyBlock(0xD2B9, 0xD2C0, "CHAFF"),
                new DisassemblyBlock(0xD4DE, 0xD4E1, "CHAFF"),
                new DisassemblyBlock(0xF757, 0xF759, "CHAFF"),
                new DisassemblyBlock(0xFA2F, 0xFA31, "CHAFF"),
                new DisassemblyBlock(0xFFAC, 0xFFAE, "CHAFF"),
            };

            Array.Sort(blocks);

            GetStats(blocks);

            foreach (var block in blocks)
                disassembler.Disassemble(block.Start, PrgRomFileOffset(block.Start), block.Length, output, annotations, ranges, block.Category);
        }

        private static void GetStats(DisassemblyBlock[] blocks)
        {
            SortedDictionary<string, int> counts = new SortedDictionary<string, int>();
            var total = 0;

            foreach (var block in blocks)
            {
                total += block.Length;
                if (!counts.ContainsKey(block.Category))
                    counts.Add(block.Category, 0);
                counts[block.Category] += block.Length;
            }

            foreach (var category in counts.Keys)
            {
                var percent = counts[category] * 100.0 / total;
                Debug.Print(category + "\t" + counts[category] + "b\t(" + percent.ToString("F2") + "%)");
            }
            Debug.Print("TOTAL:\t" + total + "b");
        }

        class DisassemblyBlock : IComparable<DisassemblyBlock>
        {
            public ushort Start { get; private set; }
            public int Length { get; private set; }
            public string Category { get; private set; }

            public DisassemblyBlock(ushort start, int end, string category)
            {
                Start = start;
                Length = end - start;
                Category = category;
            }

            public int CompareTo(DisassemblyBlock other)
            {
                return this.Start.CompareTo(other.Start);
            }
        }

        private static void HackDisableBombPibblies(byte[] data)
        {
            data[PrgRomFileOffset(0xB514)] = 0x06;
        }

        private static void HackStartingLevel(byte[] data, int startingLevel)
        {
            data[PrgRomFileOffset(0x82BC)] = (byte)(startingLevel - 2);
        }

        private static void HackQuickGameStart(byte[] data)
        {
            data[PrgRomFileOffset(0x8511)] = 0xEA;
            data[PrgRomFileOffset(0x8512)] = 0xEA;
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

        private static void DumpStrip(byte[] fileData, int startAddress, int numBytes, string filename)
        {
            var width = 4;
            var height = numBytes / width;
            if (numBytes % width != 0)
            {
                height += 1;
            }

            using (var writer = new BinaryWriter(File.Create(filename)))
            {
                writer.Write((int)width);
                writer.Write((int)height);
                if (numBytes % width != 0)
                {
                    writer.Write((int)0);
                    writer.Write((int)0);
                }

                for (int i = 0; i < numBytes; i++)
                {
                    writer.Write((int)(uint)fileData[startAddress + i]);
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

        private static void DumpPalette(byte[] fileData, int startAddress, string filename)
        {
            using (var bitmap = new Bitmap(64, 64))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    DumpSwatch(g, 0x0, fileData[startAddress + 0x0]);
                    DumpSwatch(g, 0x1, fileData[startAddress + 0x1]);
                    DumpSwatch(g, 0x2, fileData[startAddress + 0x2]);
                    DumpSwatch(g, 0x3, fileData[startAddress + 0x3]);
                    DumpSwatch(g, 0x4, fileData[startAddress + 0x0]);
                    DumpSwatch(g, 0x5, fileData[startAddress + 0x5]);
                    DumpSwatch(g, 0x6, fileData[startAddress + 0x6]);
                    DumpSwatch(g, 0x7, fileData[startAddress + 0x7]);
                    DumpSwatch(g, 0x8, fileData[startAddress + 0x0]);
                    DumpSwatch(g, 0x9, fileData[startAddress + 0x9]);
                    DumpSwatch(g, 0xA, fileData[startAddress + 0xA]);
                    DumpSwatch(g, 0xB, fileData[startAddress + 0xB]);
                    DumpSwatch(g, 0xC, fileData[startAddress + 0x0]);
                    DumpSwatch(g, 0xD, fileData[startAddress + 0xD]);
                    DumpSwatch(g, 0xE, fileData[startAddress + 0xE]);
                    DumpSwatch(g, 0xF, fileData[startAddress + 0xF]);
                }
                bitmap.Save(filename, ImageFormat.Png);
            }
        }

        private static void DumpSwatch(Graphics g, int swatchIndex, byte color)
        {
            using (var brush = new SolidBrush(Palette.ColorForIndex(color)))
                g.FillRectangle(brush, (swatchIndex % 4) * 16, (swatchIndex / 4) * 16, 16, 16);
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
