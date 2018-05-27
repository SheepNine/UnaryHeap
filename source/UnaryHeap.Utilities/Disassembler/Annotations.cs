using System.Collections.Generic;
using System.Linq;

namespace Disassembler
{
    class Annotations
    {
        private SortedDictionary<int, string> labels = new SortedDictionary<int, string>();
        private SortedDictionary<int, string> inlineComments = new SortedDictionary<int, string>();
        private SortedDictionary<int, string> sectionHeaders = new SortedDictionary<int, string>();

        public Annotations()
        {
            // KNOWN SUBROUTINES

            RecordLabel(0xFF81, "sNMI");
            RecordLabel(0xFFF1, "sRST");
            RecordLabel(0xFF80, "sIRQ_BRK");
            RecordLabel(0x8010, "tk_8629");
            RecordLabel(0x8019, "tk_B42A");
            RecordLabel(0x801C, "tk_93BA");
            RecordLabel(0x801F, "tk_E23A");
            RecordLabel(0x8022, "tk_DFA8");
            RecordLabel(0x86AA, "tk_8730");
            RecordLabel(0xC688, "tk_C659");
            RecordLabel(0xAFC6, "tk_AI_pibsplat");
            RecordLabel(0x802F, "sNewEntity");
            RecordLabel(0xFCF0, "tk_FE4E");
            RecordLabel(0x8251, "RST_PT2");
            RecordLabel(0x85C2, "RST_PT3");
            RecordLabel(0xFFAE, "sBusyWait");
            RecordLabel(0xFFB5, "MP_CTRL");
            RecordLabel(0xFFC9, "MP_B0");
            RecordLabel(0xFFDD, "MP_B1");
            RecordLabel(0x81EA, "sQueueSFX_Pn");
            RecordLabel(0x81EE, "sQueueSFX_P0");
            RecordLabel(0x81F2, "sQueueSFX_P1");
            RecordLabel(0x81FA, "sQueueSFX_NZ");
            RecordLabel(0x81FC, "sQueueSFX");
            RecordLabel(0xC3B7, "sChrRomBlit_5A");
            RecordLabel(0xC3BB, "sChrRomBlit_06");
            RecordLabel(0xC3D7, "sChrRomBlit");
            RecordLabel(0x8242, "RST_PPU");
            RecordLabel(0xE2C9, "sPrtStringRow");
            RecordLabel(0x80B6, "sPrintStrings");
            RecordLabel(0x80BC, "sPrtStrBackDoor");
            RecordLabel(0x816C, "sHideAllSprites");
            RecordLabel(0x816E, "sHideUnusedSprs");
            RecordLabel(0xE209, "sDeleteEntity");
            RecordLabel(0xD279, "sPollControllers");
            RecordLabel(0x8B07, "sSetMaxTimer");
            RecordLabel(0xD2E1, "sWaitForVBlank");
            RecordLabel(0xD62A, "sInitMusic");
            RecordLabel(0x8DB7, "sLoopMusic");
            RecordLabel(0xBF3A, "sRumbleScreen");
            RecordLabel(0xD2C9, "sRandomNumGen");
            RecordLabel(0xC350, "NAGTRUN");
            RecordLabel(0xFCBA, "sRelativeOAM");
            RecordLabel(0xC581, "sEntitySuicide");
            RecordLabel(0xD245, "sKillBothPlyrs");
            RecordLabel(0xD26A, "sLoadCrescendo");
            RecordLabel(0x9032, "sZeroPlayerTData");
            RecordLabel(0x96F3, "sMaskController");
            RecordLabel(0xC040, "sAddATo058B");
            RecordLabel(0xC03E, "sAdd4To058B");
            RecordLabel(0xC9AF, "sSetBlankArngmnt");

            RecordLabel(0x8197, "sCopyZCoords");
            RecordLabel(0x81A4, "sCopyXYZCoords");
            RecordLabel(0x81A7, "sCopyXYCoords");

            RecordLabel(0x8128, "sChangeMState");
            RecordLabel(0x8C01, "sFadeWithSubtype");
            RecordLabel(0x8C0F, "sMState_Fade");
            RecordLabel(0x8C69, "sMState_Play");
            RecordLabel(0x8DC6, "sMState_DDown");
            RecordLabel(0x0402, "sMState_rando");
            RecordLabel(0x84CC, "sMState_MTitles");

            RecordLabel(0xB6DE, "sSetYAMod64Div8");
            RecordLabel(0xB6E0, "sSetYToADiv8");
            RecordLabel(0xB6E1, "sSetYToADiv4");

            RecordLabel(0x80DB, "sLoadNtaXToNt23");
            RecordLabel(0x80DD, "sLoadNtaXToNtA");

            RecordLabel(0x845B, "sChrRomBlit_24");
            RecordLabel(0xBD77, "sDamageSnake");

            RecordLabel(0x95FE, "sSetSnakeLayout");
            RecordLabel(0x89DA, "sBlendAttrs");

            RecordLabel(0x817A, "sFindEntity0");
            RecordLabel(0x817C, "sFindEntityA");
            RecordLabel(0x817E, "sFindEntAFromY");
            RecordLabel(0xB869, "sCreateSplash");

            RecordLabel(0x0600, "sCfgEndCredits");
            RecordLabel(0x0277, "sDynamicLOL");
            RecordLabel(0x02C7, "sDynamicWTF");


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
                RecordLabel(unsubs[i], string.Format("UNSUB_{0:X4}", unsubs[i]));



            // LOOPS

            var loopBranches = new[] {
                0x80BE, 0x80E7, 0x80EA, 0x8105, 0x8170, 0x81E3, 0x8233, 0x81CA, 0x82FA, 0x836B, 0x8387, 0x8395, 0x842D,
                0xC402, 0xD643, 0xD6DE, 0xE2D5, 0xF530, 0xFBB9, 0xD284, 0xFA33, 0xD247, 0xD26C, 0x818A, 0x9036, 0xE185,
                0x8155, 0x814B, 0x8219, 0xC659, 0xB41A, 0xB436, 0xBACE, 0x8324, 0x83CF, 0x84F1, 0x84F3, 0x85EF, 0x85DC,
                0x85D3, 0x8632, 0x86D2, 0x8A08, 0x8DD3, 0x8DF9, 0x8E3B, 0x8E63, 0x910B, 0xABF4
            };
            foreach (var i in Enumerable.Range(0, loopBranches.Length))
                RecordLabel(loopBranches[i], string.Format("loop_{0:D3}", i));


            // SKIPS

            var skipBranches = new[] {
                0x80B3, 0x8166, 0xB904, 0xB901, 0xB89F, 0xA007, 0xBB26, 0xBDEA, 0x84FE, 0x8E37, 0x8E46, 0x8E5D,
                0x810F, 0x8144, 0x81E1, 0x815E, 0xB8B5, 0xB382, 0xB262,
                0xBB46, 0xBDED, 0x8546, 0x877C, 0x8E5B, 0x8E79, 0xB266,
                0x82B6, 0x82F2, 0x8379, 0x8361, 0x83A6, 0x83DE, 0xB7E3,
                0x83F7, 0x8416, 0x841F, 0x8436, 0x8452, 0x8E9E,
                0x9D1C, 0x9D76, 0x9D86, 0x9D95, 0x9D97, 0x9D99,
                0x9D9F, 0x9E26, 0x833C, 0x85B2, 0x87DD, 0x8EB6,
                0xD564, 0xD578, 0xD5EF, 0xD6AA, 0xD6C8, 0xD6D8,
                0xD6E6, 0xD6EE, 0xD702, 0x85A7, 0x8C31, 0x8EC3,
                0xE2F5, 0xB753, 0xB6F4, 0xA037, 0xA043, 0xB3AF,
                0xB44A, 0xBE02, 0x834C, 0x857C, 0x8E39, 0x8EDD,
                0xFBA6, 0xFBCA, 0xFBD3, 0xFBE2, 0xFCB5, 0xFF61,
                0xFFA6, 0xBE55, 0x853B, 0x8554, 0x87DB, 0x8EE0,
                0xBB85, 0xBBB2, 0xBBB5, 0xBC33, 0xBBF0, 0xBC04,
                0xBC1B, 0xBC23, 0xBC4C, 0xBC6F, 0x8C25, 0x8E13,
                0xBC6C, 0xBC83, 0xBC76, 0xBC7E, 0xBC9F, 0xBC9C,
                0xBCAE, 0xBCE3, 0xBC85, 0xBCA2, 0xBE2B, 0x90D1,
                0xBCF0, 0xBD58, 0xBD17, 0xBD34, 0xBD74, 0xB3CB,
                0xB426, 0xBE5E, 0x8349, 0x855A, 0x8BDA, 0x90F4,
                0xCB96, 0xC676, 0xC66D, 0xB8EC, 0xB70A, 0xB3C9,
                0xBB3E, 0xBE1B, 0x8393, 0x8572, 0x8C45, 0x9117,
                0xD4C4, 0xB74D, 0xB750, 0x9FFF, 0xA05B, 0xB40E,
                0xBB2D, 0xBE0D, 0x82E8, 0x859E, 0x8C4A, 0x9183,
                0xE176, 0xE1C5, 0xE1EB, 0xE1FF, 0xB385, 0xB434,
                0xB5B1, 0xBB5A, 0x851D, 0x85AF, 0x8A3C, 0xAEAE,
                0xFA55, 0xFAB4, 0xFA71, 0xFA75, 0xFAD1, 0xFADF,
                0xFABF, 0xFBB7, 0xBFA2, 0x8593, 0x8A2B, 0xAF56,
                0x85BC, 0x8646, 0x864C, 0x8666, 0x86E2, 0x870A,
                0x8773, 0x8776, 0x89A8, 0x89BD, 0x89F5, 0xAF5E,
            };
            foreach (var i in Enumerable.Range(0, skipBranches.Length))
                RecordLabel(skipBranches[i], string.Format("skip_{0:D3}", i));


            // FAR BRANCHES
            var farBranches = new[]
            {
                0x8322, 0x8302, 0x968F, 0xA4DB, 0xABE1, 0xB52B, 0xB53B, 0xB61B,
                0xB7FA, 0xB875, 0xBAC0, 0xBB72, 0xBD5E, 0xBDF9, 0xC8C7
            };
            foreach (var i in Enumerable.Range(0, farBranches.Length))
                RecordLabel(farBranches[i], string.Format("far_{0:D3}", i));


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
                RecordLabel(branchToRTSes[i], string.Format("rts_{0:D2}", i));


            
            RecordInlineComment(0xFA55, "Check if there is bonus time to be added" );
            RecordInlineComment(0xFA81, "Tick a second off of the timer" );
            RecordInlineComment(0xC1A0, "Grant 30 seconds of bonus time" );
            RecordInlineComment(0x82A6, "Set starting player lives to 2" );
            RecordInlineComment(0x82AD, "Check if two-player mode was chosen; null out their values if so" );
            RecordInlineComment(0x82BB, "Set starting level to 0 (-1, since it gets incremented below)" );
            RecordInlineComment(0x8EAF, "Reset player lives to 2 (after player continues)" );
            RecordInlineComment(0x8442, "Init CHR ROM sprite page by level" );
            RecordInlineComment(0xFD1C, "Draw a wind-up key in the snake's head" );
            RecordInlineComment(0x828B, "Call sCopyBgImage: Transfer 'SNAKE' arrangement to PPU" );
            RecordInlineComment(0x865B, "Call sCopyBgImage: Transfer 'Rattle' arrangement to PPU" );
            RecordInlineComment(0x8660, "Call sCopyBgImage: Transfer 'Roll' arrangement to PPU" );
            RecordInlineComment(0xFA99, "Read status bar OAM template into OAM staging RAM" );
            RecordInlineComment(0xFAA4, "Set tile of sprite for timer low digit" );
            RecordInlineComment(0xFAAB, "Set tile of sprite for timer high digit" );
            RecordInlineComment(0xFAB2, "Check if timer high digit is zero (less than ten seconds left)" );
            RecordInlineComment(0xFA51, "Check whether in normal play mode; skip timer if not" );
            RecordInlineComment(0xFA4D, "Skip skipping play mode check if in a pond (allow timer to tick)" );
            RecordInlineComment(0xFA59, "Only worry about bonus time every fourth frame" );
            RecordInlineComment(0xFA5F, "Do the grant of a second of bonus time" );
            RecordInlineComment(0xFA75, "Don't decrement timer if a black hole is open" );
            RecordInlineComment(0xFA8D, "Timer is at zero and cannot be decremented" );
            RecordInlineComment(0xFAC9, "Set player one life count (or hide sprite if game over)" );
            RecordInlineComment(0xFAD7, "Set player two life count (or hide sprite if game over)" );
            RecordInlineComment(0xFAE5, "--- next chunk of the method ---" );
            RecordInlineComment(0xFBB7, "--- another chunk of the method: $14 loops above completed (for entities?) ---" );
            RecordInlineComment(0xB231, "NB: Y is greater than zero" );
            RecordInlineComment(0x82F8, "Delete all entities except 0 and 1 (the snakes)" );
            RecordInlineComment(0xCC46, "Transmute into a pibblyfish and tail-call into that AI" );
            RecordInlineComment(0xBB8F, "NB: Function run twice, with Y=0 and Y=1, unless called remotely" );
            RecordInlineComment(0xBF4E, "NB: Function run twice, with Y=0 and Y=1, unless called remotely" );
            RecordInlineComment(0x8E63, "Read indices of sprites to hide" );
            RecordInlineComment(0x9756, "Mask out left button" );
            RecordInlineComment(0x9768, "Mask out right button" );
            RecordInlineComment(0x91D4, "Read AI code low byte" );
            RecordInlineComment(0x91D9, "Read AI code high byte" );
            RecordInlineComment(0xB8B2, "Entity suicide if reached the end of the animation cycle" );
            RecordInlineComment(0xBF2C, "Populate BigFoot arrangement" );
            RecordInlineComment(0xE209, "Populate blank arrangement" );
            RecordInlineComment(0xCB36, "Populate poof arrangement" );
            RecordInlineComment(0xC7BA, "Populate poof arrangement" );
            RecordInlineComment(0xCAF8, "Populate BigFoot or blank arrangement (by level)" );
            RecordInlineComment(0xC9F4, "Populate one of the two magic carpet arrangements" );
            RecordInlineComment(0xC879, "Populate blank arrangement" );
            RecordInlineComment(0xB753, "Populate blank arrangement" );
            RecordInlineComment(0xC7CC, "Populate blank arrangement" );
            RecordInlineComment(0xC82E, "Populate anvil arrangement" );
            RecordInlineComment(0xC6EF, "Populate one of the two mushroom arrangements" );
            RecordInlineComment(0xB8D6, "Populate one of the two flag arrangements" );
            RecordInlineComment(0xC204, "Populate blank or icy pibbly hole mushroom arrangement (by level)" );
            RecordInlineComment(0x86A4, "Play SFX 'diving splash' rate-limited" );
            RecordInlineComment(0x8EA6, "Play SFX 'PLAY ON/1-UP'" );
            RecordInlineComment(0x8EA9, "Play SFX 'PLAY ON/1-UP'" );
            RecordInlineComment(0x99B8, "Play SFX 'Jaws (slow)' or 'Jaws (fast)'" );
            RecordInlineComment(0x9A95, "Play SFX 'Pibbly chomp'" );
            RecordInlineComment(0x9AB9, "Play SFX 'Pibbly chunk spit'" );
            RecordInlineComment(0x9D4E, "Play SFX 'ARRRGGG'" );
            RecordInlineComment(0x9D51, "Play SFX 'ARRRGGG'" );
            RecordInlineComment(0x9E14, "Play SFX 'Rocket take-off' rate-limited" );
            RecordInlineComment(0x9F05, "Play SFX 'Water jump jet' rate-limited" );
            RecordInlineComment(0xB398, "Play SFX 'Snake gulp'" );
            RecordInlineComment(0xB4C8, "Play SFX 'Pibbly ejection' rate-limited" );
            RecordInlineComment(0xB6B4, "Play SFX 'Scale bell ring' rate-limited" );
            RecordInlineComment(0xB6B9, "Play SFX '???'" );
            RecordInlineComment(0xBB79, "Play SFX 'Explosion'" );
            RecordInlineComment(0xBC01, "Play SFX 'Lick foot' rate-limited" );
            RecordInlineComment(0xBC94, "Play SFX 'Exploding enemy 1'" );
            RecordInlineComment(0xBC99, "Play SFX 'Exploding enemy 2" );
            RecordInlineComment(0xBCEA, "Play SFX 'Bounce/lick enemy'" );
            RecordInlineComment(0xBD61, "Play SFX 'Snake OW'" );
            RecordInlineComment(0xBD93, "Play SFX 'Snake death spin'" );
            RecordInlineComment(0xBDFB, "Play SFX 'Explosion'" );
            RecordInlineComment(0xBF40, "Play SFX 'THUD' rate-limited" );
            RecordInlineComment(0xC129, "Play SFX for current powerup (see $C1CF)" );
            RecordInlineComment(0xC132, "Play SFX for current powerup (see $C1CF)" );
            RecordInlineComment(0xC2DB, "Play SFX 'Lid opening''" );
            RecordInlineComment(0xC86B, "Play SFX 'Wormhole opening'" );
            RecordInlineComment(0xC86E, "Play SFX 'Wormhole opening'" );
            RecordInlineComment(0xC984, "Play SFX 'Wormhole sucking up object' rate-limited" );
            RecordInlineComment(0xD372, "Play SFX 'Exit door point score' rate-limited" );
            RecordInlineComment(0xD375, "Play SFX 'Exit door point score' rate-limited" );
            RecordInlineComment(0xD6D2, "Play SFX 'Crescendo'" );
            RecordInlineComment(0xD6D5, "Play SFX 'Crescendo'" );
            RecordInlineComment(0xFABC, "Play SFX 'Time running out beep'" );
            RecordInlineComment(0x8D77, "Configure 'HOLD' drop-down" );
            RecordInlineComment(0x8F0D, "Configure 'PLAY ON' drop-down" );
            RecordInlineComment(0x8F15, "Configure ??? drop-down" );
            RecordInlineComment(0x8F57, "Configure ??? drop-down" );
            RecordInlineComment(0xBE1B, "Configure 'ALL GONE' drop-down, or maybe others" );
            RecordInlineComment(0xB3C3, "Crescendo SFX setup (5/5 pibbly eaten on bonus)" );
            RecordInlineComment(0xC17E, "Crescendo SFX setup (extra continue picked up)" );
            RecordInlineComment(0xCD29, "Crescendo SFX setup (warp lid opened)" );
            RecordInlineComment(0xC848, "Crescendo SFX setup (anvil stomps scale)" );
            RecordInlineComment(0xD49A, "Crescendo SFX setup (pond door opens)" );
            RecordInlineComment(0xC18C, "Clock picked up" );
            RecordInlineComment(0xC177, "Fish tail picked up" );
            RecordInlineComment(0xC17C, "Extra continue picked up" );
            RecordInlineComment(0xC15B, "Windup key picked up" );
            RecordInlineComment(0xC1A6, "1-UP picked up" );
            RecordInlineComment(0xC1B4, "Diamond picked up" );
            RecordInlineComment(0xC154, "Inverter picked up" );
            RecordInlineComment(0xC1BB, "Tongue extender picked up" );
            RecordInlineComment(0x8462, "Play 'main titles' track" );
            RecordInlineComment(0x8D9F, "Play 'game over' track" );
            RecordInlineComment(0xFA93, "Play 'time out' track" );
            RecordInlineComment(0x873D, "Read 'entity control bits?'" );
            RecordInlineComment(0xBCB8, "Read 'entity control bits?'" );
            RecordInlineComment(0xC8FF, "Read 'entity control bits?'" );
            RecordInlineComment(0xDF5A, "Read 'entity control bits?'" );
            RecordInlineComment(0xDFF2, "Read 'entity control bits?'" );
            RecordInlineComment(0xFAFC, "Read 'entity control bits?'" );
            RecordInlineComment(0xBA99, "Read animation arrangement for this frame" );
            RecordInlineComment(0xCCB6, "Read animation arrangement for this frame" );
            RecordInlineComment(0xB6FD, "Read animation arrangement for this frame" );
            RecordInlineComment(0xFE76, "Read 'arrangement address high byte" );
            RecordInlineComment(0xFE7B, "Read 'arrangement address low byte" );
            RecordInlineComment(0x8C0D, "Unconditional branch" );
            RecordInlineComment(0xB9D7, "Unconditional branch" );
            RecordInlineComment(0xE207, "Unconditional branch" );
            RecordInlineComment(0xCBA0, "Unconditional branch" );
            RecordInlineComment(0x8A18, "Unconditional branch" );
            RecordInlineComment(0xAC3C, "Unconditional branch" );
            RecordInlineComment(0xC688, "One of these two branches will be taken" );
            RecordInlineComment(0x852C, "'Game over' fade subtype" );
            RecordInlineComment(0x8029, "Start fade transition to end credits" );
            RecordInlineComment(0x9004, "Start fade transition to game over" );
            RecordInlineComment(0x962E, "Start fade transition entering pond" );
            RecordInlineComment(0x9676, "Start fade transition to end of level" );
            RecordInlineComment(0xC1F0, "Start fade transition out of bonus/pond back to level" );
            RecordInlineComment(0xC891, "Start fade transition entering warp/bonus" );
            RecordInlineComment(0x8025, "??? how does PC get to this point ???" );
            RecordInlineComment(0x8A0E, "Tail-call this method to another" );
            RecordInlineComment(0x82C4, "Call sDynamicBBQ" );
            RecordInlineComment(0xC5B8, "Call sDynamicBBQ" );

            //{ 0x06C1, "Crescendo SFX setup (level x completed / game over)" },
            //{ 0x0776, "Play SFX" },
            //{ 0x0779, "Play SFX" },
            //{ 0x04DC, "Play SFX" },
            //{ 0x04DF, "Play SFX" },
            //{ 0x05E6, "Play SFX" },
            //{ 0x05E9, "Play SFX" },
            //{ 0x05EE, "Play SFX" }



            RecordSectionHeader(0x8000, "Audio track reads" );
            RecordSectionHeader(0x96F8, "Rattle/Roll AI" );
            RecordSectionHeader(0xD302, "Snake tail segment AI" );
            RecordSectionHeader(0xAFCA, "Spawning pibbly/pibblejogger AI" );
            RecordSectionHeader(0xAFEA, "Pibbly/pibblebat/pibbleboing/pibblecopter AI" );
            RecordSectionHeader(0xB4A6, "Dispensing pibbly AI" );
            RecordSectionHeader(0xAF3D, "Flipping lid AI" );
            RecordSectionHeader(0xB272, "Pibblesplat AI" );
            RecordSectionHeader(0xB5B1, "Door AI" );
            RecordSectionHeader(0xB65A, "Scale AI" );
            RecordSectionHeader(0xB6E5, "Snakedozer AI" );
            RecordSectionHeader(0xB753, "Bladez AI" );
            RecordSectionHeader(0xB907, "Shark AI" );
            RecordSectionHeader(0xB959, "ARG letters AI" );
            RecordSectionHeader(0xB9D9, "Crazy seat/bubble AI" );
            RecordSectionHeader(0xBA7F, "Pin cushion AI" );
            RecordSectionHeader(0xBE37, "Shrapnel AI" );
            RecordSectionHeader(0xC78B, "Dispensed bell AI" );
            RecordSectionHeader(0xBE8E, "BigFoot AI" );
            RecordSectionHeader(0xC048, "Tail segment floating away AI" );
            RecordSectionHeader(0xC0B5, "Clock/Diamond/1-UP/???/Corkscrew/Fake 1-UP item AI" );
            RecordSectionHeader(0xC1F9, "Pibbly dispenser AI" );
            RecordSectionHeader(0xB986, "Crazy seat (from lid) AI" );
            RecordSectionHeader(0xC858, "Black hole (warp/bonus) AI" );
            RecordSectionHeader(0xC9B5, "Magic carpet AI" );
            RecordSectionHeader(0xC1D7, "Bonus stage context AI" );
            RecordSectionHeader(0xCAEB, "Bigfoot Spawner AI" );
            RecordSectionHeader(0xCB2A, "Hovering score AI" );
            RecordSectionHeader(0xCB48, "Pibbly chunk (heavy) AI" );
            RecordSectionHeader(0xCB87, "Pibbly chunk (light) AI" );
            RecordSectionHeader(0xB9B3, "Rotating crazy seat AI" );
            RecordSectionHeader(0xAE8C, "Bell & tail dispenser AI" );
            RecordSectionHeader(0xCBA6, "Breath bubble AI" );
            RecordSectionHeader(0xCBBD, "Seaweed AI" );
            RecordSectionHeader(0xCC4B, "Pibblefish AI" );
            RecordSectionHeader(0xC634, "Powerup AI" );
            RecordSectionHeader(0xC68A, "Record/mushroom/ice cube, metal tree, bell AI" );
            RecordSectionHeader(0xC7C7, "Anvil AI" );
            RecordSectionHeader(0x9EA1, "Water jet AI" );
            RecordSectionHeader(0xB9D5, "Stationary metal tree (?) AI" );
            RecordSectionHeader(0x9F1D, "Metal sphere/snowball/asteroid AI" );
            RecordSectionHeader(0xCC2C, "Pibblyfish egg hatching AI" );
            RecordSectionHeader(0xB88E, "Splash AI" );
            RecordSectionHeader(0x9E0B, "Warp rocket  AI" );
            RecordSectionHeader(0xBB14, "AI $15" );
            RecordSectionHeader(0xBB5D, "AI $16" );
            RecordSectionHeader(0x80B6, "String printing method" );
            RecordSectionHeader(0xC3D7, "CHR ROM blitter" );
            RecordSectionHeader(0xC350, "Nametable attribute RLE run fetcher" );
            RecordSectionHeader(0x80DB, "Nametable attribute RLE decoder/loader" );
            RecordSectionHeader(0x80F7, "Palette RAM initializer" );
            RecordSectionHeader(0x816C, "Sprite hiding methods" );
            RecordSectionHeader(0xD279, "Controller polling" );
            RecordSectionHeader(0x8128, "Change the current machine state" );
            RecordSectionHeader(0x8C69, "PLAY machine state" );
            RecordSectionHeader(0x8DC6, "DROP DOWN machine state" );
            RecordSectionHeader(0x84CC, "MAIN TITLES machine state" );
            RecordSectionHeader(0xC154, "Powerup logic" );
            RecordSectionHeader(0xFCBA, "Code to populate OAM buffer with relative sprites (pibbly being eaten or windup key on head)" );
            RecordSectionHeader(0xD245, "Kill both players (time out/all gone)" );
            RecordSectionHeader(0xD26A, "Select SFX $34: Crescendo variant" );
            RecordSectionHeader(0xFCA6, "A subroutine assisting with tongue drawing somehow" );
            RecordSectionHeader(0x8B07, "Reset timer to 99 seconds" );
            RecordSectionHeader(0xC03E, "Increment $058B for an entity" );
            RecordSectionHeader(0xBF3A, "Initialize screen rumbling variables" );
            RecordSectionHeader(0xE209, "Entity deletion methods" );
            RecordSectionHeader(0xE2C9, "Half-string render method (does one of two rows of tiles)" );
            RecordSectionHeader(0x8150, "Unknown subroutine" );
            RecordSectionHeader(0x817A, "Unknown subroutine (searching for an unused entity to populate?)" );
            RecordSectionHeader(0x8197, "Coordinate copying methods" );
            RecordSectionHeader(0xB848, "Unknown subroutine" );
            RecordSectionHeader(0x81C3, "Unknown subroutine" );
            RecordSectionHeader(0x81DB, "Unknown subroutine" );
            RecordSectionHeader(0x8231, "Unknown subroutine" );
            RecordSectionHeader(0x95DE, "Unknown subroutine" );
            RecordSectionHeader(0x95EB, "Unknown subroutine" );
            RecordSectionHeader(0x95FE, "Method for setting snake entity arrangement/attributes" );
            RecordSectionHeader(0x86B6, "Unknown subroutine" );
            RecordSectionHeader(0x86A0, "Unknown subroutine" );
            RecordSectionHeader(0x86AD, "Unknown subroutine" );
            RecordSectionHeader(0x8BCE, "Unknown subroutine" );
            RecordSectionHeader(0xBAFC, "Unknown subroutine" );
            RecordSectionHeader(0xBB88, "Unknown subroutine" );
            RecordSectionHeader(0xBE23, "Unknown subroutine" );
            RecordSectionHeader(0xBE83, "Unknown subroutine" );
            RecordSectionHeader(0xBF44, "Unknown subroutine" );
            RecordSectionHeader(0xE161, "Unknown subroutine" );
            RecordSectionHeader(0xE17C, "Unknown subroutine" );
            RecordSectionHeader(0xE055, "Unknown subroutine" );
            RecordSectionHeader(0xE037, "Unknown subroutine" );
            RecordSectionHeader(0xD4D2, "Unknown subroutine" );
            RecordSectionHeader(0xDF1A, "Unknown subroutine" );
            RecordSectionHeader(0xD4E1, "Unknown subroutine" );
            RecordSectionHeader(0xD4F4, "Unknown subroutine" );
            RecordSectionHeader(0xD1D2, "Unknown subroutine" );
            RecordSectionHeader(0xCF40, "Unknown subroutine" );
            RecordSectionHeader(0xC64E, "Unknown subroutine" );
            RecordSectionHeader(0xB732, "Unknown subroutine" );
            RecordSectionHeader(0x866D, "Unknown subroutine" );
            RecordSectionHeader(0x8689, "Unknown subroutine" );
            RecordSectionHeader(0x9FB7, "Unknown subroutine" );
            RecordSectionHeader(0x9FC6, "Unknown subroutine" );
            RecordSectionHeader(0x9F9E, "Unknown subroutine" );
            RecordSectionHeader(0xA197, "Unknown subroutine" );
            RecordSectionHeader(0xAE7F, "Unknown subroutine" );
            RecordSectionHeader(0xB231, "Unknown subroutine" );
            RecordSectionHeader(0xB247, "Unknown subroutine" );
            RecordSectionHeader(0xB315, "Unknown subroutine" );
            RecordSectionHeader(0xB9A7, "Unknown subroutine" );
            RecordSectionHeader(0xBD77, "Damage snake method" );
            RecordSectionHeader(0xBDAF, "Explosion AI" );
            RecordSectionHeader(0xBE06, "Unknown subroutine" );
            RecordSectionHeader(0x845B, "Unknown subroutine" );
            RecordSectionHeader(0x8460, "Unknown subroutine" );
            RecordSectionHeader(0x8480, "Unknown subroutine" );
            RecordSectionHeader(0xC592, "Unknown subroutine" );
            RecordSectionHeader(0x8064, "Unknown subroutine" );
            RecordSectionHeader(0x8025, "Unknown subroutine" );
            RecordSectionHeader(0x899A, "Unknown subroutine" );
            RecordSectionHeader(0x89DA, "Method for 'blending in' entity attributes" );
            RecordSectionHeader(0x89E3, "Unknown subroutine" );
            RecordSectionHeader(0x89FD, "Unknown subroutine" );
            RecordSectionHeader(0x8A1A, "Unknown subroutine" );
            RecordSectionHeader(0x8A4E, "Unknown subroutine" );
            RecordSectionHeader(0x90C6, "Unknown subroutine" );
            RecordSectionHeader(0x9139, "Unknown subroutine" );
            RecordSectionHeader(0x9186, "Unknown subroutine" );
            RecordSectionHeader(0x918B, "Unknown subroutine" );
            RecordSectionHeader(0x9292, "Unknown subroutine" );
            RecordSectionHeader(0x92A2, "Unknown subroutine" );
            RecordSectionHeader(0x92BB, "Dynamic thunk" );
            RecordSectionHeader(0x92BE, "Unknown subroutine" );
            RecordSectionHeader(0x93B2, "Unknown subroutine" );
            RecordSectionHeader(0x9432, "Unknown subroutine" );
            RecordSectionHeader(0x9584, "Unknown subroutine" );
            RecordSectionHeader(0x9D45, "Unknown subroutine" );
            RecordSectionHeader(0x9D54, "Unknown subroutine" );
            RecordSectionHeader(0x9DA3, "Unknown subroutine" );
            RecordSectionHeader(0x9DC5, "Unknown subroutine" );
            RecordSectionHeader(0x9E95, "Unknown subroutine" );
            RecordSectionHeader(0xB3D7, "Unknown subroutine" );
            RecordSectionHeader(0xB527, "Unknown subroutine" );
            RecordSectionHeader(0xB7CD, "Unknown subroutine" );
            RecordSectionHeader(0xB869, "Method to spawn a new splash entity" );
            RecordSectionHeader(0xC2AA, "Unknown subroutine" );
            RecordSectionHeader(0xC3A7, "Unknown subroutine" );
            RecordSectionHeader(0xC476, "Unknown subroutine" );
            RecordSectionHeader(0xC552, "Unknown subroutine" );
            RecordSectionHeader(0xC8AC, "Unknown subroutine" );
            RecordSectionHeader(0xF51D, "Unknown subroutine" );
            RecordSectionHeader(0xFA31, "Unknown subroutine" );
            RecordSectionHeader(0xFCDE, "Unknown subroutine" );
            RecordSectionHeader(0x802F, "Entity creation method" );
            RecordSectionHeader(0xB441, "Unknown subroutine (probably modifying player scores though)" );
            RecordSectionHeader(0xCCD7, "Lid-opening method" );
            RecordSectionHeader(0xB6DE, "Utility methods for loading a Y index" );
            RecordSectionHeader(0xCD4C, "Unknown subroutine, many callers" );
            RecordSectionHeader(0xE237, "Unknown fragments" );
            RecordSectionHeader(0x86AA, "Thunk" );
            RecordSectionHeader(0xAFB9, "Thunk" );
            RecordSectionHeader(0xAFC6, "Thunk" );
            RecordSectionHeader(0x8010, "Thunks" );
            RecordSectionHeader(0xD2E1, "Wait for VBlank method" );
            RecordSectionHeader(0x8DB7, "Music looping method (if the track has ended)" );
            RecordSectionHeader(0xD62A, "Init music method" );
            RecordSectionHeader(0xD69A, "Some audio module method???" );
            RecordSectionHeader(0xD6FB, "Some audio module method???" );
            RecordSectionHeader(0xD8A1, "Some audio module method???" );
            RecordSectionHeader(0xD8D1, "Some audio module method???" );
            RecordSectionHeader(0xD99E, "Start of audio opcode methods" );
            RecordSectionHeader(0xFF80, "IRQ/BRK handler" );
            RecordSectionHeader(0xFF81, "NMI handler" );
            RecordSectionHeader(0xFFAE, "Busy wait method" );
            RecordSectionHeader(0xFFF1, "RST handler" );
            RecordSectionHeader(0xFFB5, "Mapper method A" );
            RecordSectionHeader(0xFFC9, "Mapper method B" );
            RecordSectionHeader(0xFFDD, "Mapper method C" );
            RecordSectionHeader(0x9CDE, "Unknown subroutine" );
            RecordSectionHeader(0x9D14, "Unknown subroutine" );
            RecordSectionHeader(0x9CF3, "Unknown subroutine" );
            RecordSectionHeader(0x96F3, "Mask controller push state" );
            RecordSectionHeader(0x8251, "RST handler routine part 2" );
            RecordSectionHeader(0x8242, "Reset PPU control/mask registers" );
            RecordSectionHeader(0x8209, "--------" );
            RecordSectionHeader(0x8295, "--------" );
            RecordSectionHeader(0x8C01, "FADE machine state" );
            RecordSectionHeader(0x9610, "--------" );
            RecordSectionHeader(0xC659, "--------" );
            RecordSectionHeader(0xA4DB, "--------" );
            RecordSectionHeader(0xABE1, "--------" );
            RecordSectionHeader(0xB7FA, "--------" );
            RecordSectionHeader(0xBAC0, "--------" );
            RecordSectionHeader(0xC8C7, "--------" );
            RecordSectionHeader(0x8666, "Fragment belonging to $8689 below" );
            RecordSectionHeader(0x849C, "'End credits' fade subtype" );
            RecordSectionHeader(0xC3BF, "'Level end' fade subtype" );
            RecordSectionHeader(0xC587, "'Warp/bonus' fade subtype" );
            RecordSectionHeader(0xC5B5, "'Entering pond' fade subtype" );
            RecordSectionHeader(0xC5F9, "'Return to play' fade subtype" );
            RecordSectionHeader(0xB8C5, "Flag AI" );
            RecordSectionHeader(0x81EA, "SFX enqueueing methods" );
            RecordSectionHeader(0x9032, "Zero out transient player data for player X" );
            RecordSectionHeader(0xC3B7, "Blit in page $5A" );
            RecordSectionHeader(0xC3BB, "Blit in page $06" );
            //{ 0x0700, "Spaceship 1 AI" },
            //{ 0x077C, "Spaceship 2 AI" },
        }

        public void RecordAnonymousLabel(int address)
        {
            RecordLabel(address, string.Format("UL_{0:X4}", address));
        }

        public void RecordLabel(int address, string label)
        {
            if (!labels.ContainsKey(address))
                labels.Add(address, label);
        }

        public string GetLabel(int address)
        {
            if (labels.ContainsKey(address))
                return labels[address];
            else
                return null;
        }

        public void RecordInlineComment(int address, string text)
        {
            inlineComments.Add(address, text);
        }

        public bool HasInlineComment(int address)
        {
            return inlineComments.ContainsKey(address);
        }

        public string GetInlineComment(int address)
        {
            return inlineComments[address];
        }

        public void RecordSectionHeader(int address, string text)
        {
            sectionHeaders.Add(address, text);
        }

        public bool HasSectionHeader(int address)
        {
            return sectionHeaders.ContainsKey(address);
        }

        public string GetSectionHeader(int address)
        {
            return sectionHeaders[address];
        }

        public void ClearRAM()
        {
            foreach (int i in labels.Keys.ToArray())
            {
                if (i < 0x1000)
                    labels.Remove(i);
            }
            foreach (int i in inlineComments.Keys.ToArray())
            {
                if (i < 0x1000)
                    inlineComments.Remove(i);
            }
            foreach (int i in sectionHeaders.Keys.ToArray())
            {
                if (i < 0x1000)
                    sectionHeaders.Remove(i);
            }
        }
    }
}
