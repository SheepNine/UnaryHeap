using System;
using System.Collections.Generic;
using System.Linq;

namespace Disassembler
{
    class Annotations
    {
        private SortedDictionary<int, string> labels = new SortedDictionary<int, string>();
        private SortedDictionary<int, string> inlineComments = new SortedDictionary<int, string>();
        private SortedDictionary<int, string> sectionHeaders = new SortedDictionary<int, string>();
        private SortedDictionary<int, string> variables = new SortedDictionary<int, string>();
        private SortedSet<int> unconditionalBranches = new SortedSet<int>();

        public Annotations()
        {
            RecordLabel(0x9139, "sUpdScrollStrip");
            RecordInlineComment(0x914C, "Screen doesn't scroll if a player is dying/dead, if the game is paused, or a warp is in progress");
            RecordInlineComment(0x9157, "Screen doesn't scroll if the entity types for p1/p2 don't line up??");
            RecordInlineComment(0x9161, "No vertical scroll if in a bonus/pond");
            RecordInlineComment(0x9167, "No vertical scroll on level 11");
            RecordLabel(0x8629, "sDrawSnakePics");
            RecordLabel(0x8010, "tkDrawSnakePics");
            RecordLabel(0x84C2, "cChangeToCrawlMS");
            RecordLabel(0xE23A, "cDrwShipOrFrames");
            RecordLabel(0x801F, "tkDrwShipFrames");
            RecordLabel(0xFE6C, "sDrawEntArrBkDr");
            RecordInlineComment(0xCC04, "On pond 5, all Pibbley eggs hatch as golden type");
            RecordInlineComment(0xB382, "Snake licked a black pibblefish");
            RecordLabel(0xBC9F, "tkPlayLickSound");
            RecordLabel(0xBCE8, "cPlayLickSound");

            RecordLabel(0xF51D, "sFullPpuWipe");
            RecordLabel(0xF523, "sWipePpuRam");
            RecordLabel(0xF527, "sMemSetPpuRam");
            RecordLabel(0xBD5E, "cHaltPlyrAwSFX");

            RecordLabel(0xBB88, "sInteractSnakes");
            RecordLabel(0xBB8F, "sInteractSnakeY");
            RecordLabel(0xBB85, "cInteractSnkDnB");
            RecordLabel(0xBD74, "cInteractSnkDn");
            RecordLabel(0xBC9C, "tkInteractSnkDn");

            // KNOWN SUBROUTINES
            RecordLabel(0x96F8, "ei_snake");
            RecordLabel(0x9E0B, "ei_warpRocket");
            RecordLabel(0x9EA1, "ei_waterJet");
            RecordLabel(0x9F1D, "ei_roller");
            RecordLabel(0xAE8C, "ei_bellFinDoor");
            RecordLabel(0xAF3D, "ei_lid");
            RecordLabel(0xAFCA, "ei_protoPibbley");
            RecordLabel(0xAFEA, "ei_Pibbley");
            RecordLabel(0xB272, "ei_pibblesplat");
            RecordLabel(0xB4A6, "ei_dispPibbley");
            RecordLabel(0xB5C0, "ei_door");
            RecordLabel(0xB65A, "ei_scale");
            RecordLabel(0xB6E5, "ei_snakeDozer");
            RecordLabel(0xB759, "ei_bladez");
            RecordLabel(0xB88E, "ei_splash");
            RecordLabel(0xB8CE, "ei_flag");
            RecordLabel(0xB907, "ei_shark");
            RecordLabel(0xB959, "ei_argLetter");
            RecordLabel(0xB986, "ei_lidSeat");
            RecordLabel(0xB9B3, "ei_spinSeat");
            RecordLabel(0xB9D5, "ei_stillTree");
            RecordLabel(0xB9D9, "ei_seatBubble");
            RecordLabel(0xBA7F, "ei_pCushion");
            RecordLabel(0xBB14, "ei_pcPin");
            RecordLabel(0xBB5D, "ei_bomb");
            RecordLabel(0xBDAF, "ei_explosion");
            RecordLabel(0xBE37, "ei_shrapnel");
            RecordLabel(0xBE8E, "ei_bigfoot");
            RecordLabel(0xC048, "ei_tailFloat");
            RecordLabel(0xC0B5, "ei_tempPwrup");
            RecordLabel(0xC1D7, "ei_bonusCntxt");
            RecordLabel(0xC1F9, "ei_pibDispnsr");
            RecordLabel(0xC634, "ei_staticPwrup");
            RecordLabel(0xC68A, "ei_bouncer");
            RecordLabel(0xC78B, "ei_bellFinFlyr");
            RecordLabel(0xC7C7, "ei_anvil");
            RecordLabel(0xC858, "ei_blackHole");
            RecordLabel(0xC8F6, "ei_null");
            RecordLabel(0xC9B5, "ei_carpet");
            RecordLabel(0xCAEB, "ei_bfSpawn");
            RecordLabel(0xCB2A, "ei_scoreFloat");
            RecordLabel(0xCB48, "ei_pibblechunk");
            RecordLabel(0xCB87, "ei_pibblFeathr");
            RecordLabel(0xCBA6, "ei_breathBbl");
            RecordLabel(0xCBBD, "ei_seaweed");
            RecordLabel(0xCC2C, "ei_pFishEgg");
            RecordLabel(0xCC4B, "ei_pibblefish");
            RecordLabel(0xD302, "ei_snakeSgmnt");
            RecordLabel(0x8DAC, "sUpdateStrtPrs");
            RecordLabel(0xBCFC, "sSpinSnake");

            RecordLabel(0x8064, "sEntW2CTrnsfrm");
            RecordLabel(0x8068, "sEntW2CTrnsfrmX");
            RecordLabel(0x8080, "sEntW2CTrnsfrmY");

            RecordLabel(0xBCFC, "sSpinSnake");
            RecordLabel(0x95D8, "sZeroOutVEnt11");
            RecordLabel(0xCD27, "sStartWarp");
            RecordLabel(0xC88D, "cDoWarpBonusFade");
            RecordLabel(0xBF4E, "sTryCrushSnakeY");
            RecordLabel(0xBF44, "sSlamIntoGround");
            RecordLabel(0xBB72, "cExplodeEntity");
            RecordLabel(0xB875, "cBecomeSplash");
            RecordLabel(0x8025, "cDoECFadeType");
            RecordLabel(0xD5E3, "cDoneTrack");
            RecordLabel(0xD59E, "cDoneStOp");
            RecordLabel(0xD58D, "cDnStOpNewAddrA");
            RecordLabel(0xD590, "cDnStOpNewAddrB");
            RecordLabel(0xD733, "cDoneEffectOp");
            RecordLabel(0xDABC, "tkDoneEffectOp");
            RecordLabel(0xDA63, "sStartVibrado");
            RecordLabel(0xD87B, "cLoadApu");
            RecordLabel(0xD90A, "tkLoadApu");
            RecordLabel(0xD885, "cLoadApuNo4000");
            RecordLabel(0xD8F8, "cSfxUnlockDone");
            RecordLabel(0xB815, "sMvTemplateBy0D");
            RecordLabel(0xB81D, "sMvTemplateByA");
            RecordLabel(0xC64E, "sMvTemplBack4Sq");

            RecordInlineComment(0xBBE5, "Bigfoot-specific code starts here");
            RecordInlineComment(0xBC3E, "Record/mushroom die in one hit");
            RecordInlineComment(0xBC4C, "Code for 38/39/3B metal trees/bells or anything on level 10/11");
            RecordInlineComment(0xBC5A, "Skip next section on level 9");
            RecordUnconditionalBranch(0xBC74);

            RecordLabel(0x85FD, "cCopyright");
            RecordLabel(0x85D1, "cCopyrightDone");
            RecordLabel(0x8605, "cCastOC");
            RecordLabel(0x85D3, "cCastOCDone");

            RecordInlineComment(0xBCFC, "Shoot the snake into the air and start it spinning");
            RecordSectionHeader(0x96F8, "Snake Entity Intelligence");
            RecordSectionHeader(0x9E0B, "Warp rocket Entity Intelligence");
            RecordSectionHeader(0x9EA1, "Water jet Entity Intelligence");
            RecordSectionHeader(0x9F1D, "Metal sphere/snowball/asteriod Entity Intelligence");
            RecordSectionHeader(0xAE8C, "Bell/fin dispenser Entity Intelligence");
            RecordSectionHeader(0xAF3D, "Flipping lid Entity Intelligence");
            RecordSectionHeader(0xAFCA, "Spwaning Pibbley / pibblejogger Entity Intelligence");
            RecordSectionHeader(0xAFEA, "Pibbley Entity Intelligence");
            RecordSectionHeader(0xB272, "Pibblesplat Entity Intelligence");
            RecordSectionHeader(0xB4A6, "Dispensing Pibbley Entity Intelligence");
            RecordSectionHeader(0xB5B1, "Door Entity Intelligence");
            RecordSectionHeader(0xB65A, "Scale Entity Intelligence");
            RecordSectionHeader(0xB6E5, "Snakedozer Entity Intelligence");
            RecordSectionHeader(0xB753, "Bladez Entity Intelligence");
            RecordSectionHeader(0xB88E, "Splash Entity Intelligence");
            RecordSectionHeader(0xB8C5, "Flag Entity Intelligence");
            RecordSectionHeader(0xB907, "Shark Entity Intelligence");
            RecordSectionHeader(0xB959, "ARG letters Entity Intelligence");
            RecordSectionHeader(0xB986, "Crazy seat (from lid) Entity Intelligence");
            RecordSectionHeader(0xB9B3, "Rotating crazy seat Entity Intelligence");
            RecordSectionHeader(0xB9D5, "Stationary metal tree Entity Intelligence");
            RecordSectionHeader(0xB9D9, "Crazy seat / bubble Entity Intelligence");
            RecordSectionHeader(0xBA7F, "Pin cushion Entity Intelligence");
            RecordSectionHeader(0xBB14, "Pin cushion pin Entity Intelligence");
            RecordSectionHeader(0xBB5D, "Bomb Entity Intelligence");
            RecordSectionHeader(0xBDAF, "Explosion Entity Intelligence");
            RecordSectionHeader(0xBE37, "Shrapnel Entity Intelligence");
            RecordSectionHeader(0xBE8E, "BigFoot Entity Intelligence");
            RecordSectionHeader(0xC048, "Lost tail segment Entity Intelligence");
            RecordSectionHeader(0xC0B5, "Temporary Powerup Entity Intelligence");
            RecordSectionHeader(0xC1D7, "Bonus stage context ?? Entity Intelligence");
            RecordSectionHeader(0xC1F9, "Pibbley dispenser Entity Intelligence");
            RecordSectionHeader(0xC634, "Static powerup Entity Intelligence");
            RecordSectionHeader(0xC659, "Record/mushroom/ice cube/Metal tree/Bell Entity Intelligence");
            RecordSectionHeader(0xC78B, "Bell/fin tail in flight Entity Intelligence");
            RecordSectionHeader(0xC7C7, "Anvil Entity Intelligence");
            RecordSectionHeader(0xC858, "Black hole Entity Intelligence");
            RecordSectionHeader(0xC8F6, "Null Entity Intelligence");
            RecordSectionHeader(0xC9B5, "Magic carpet Entity Intelligence");
            RecordSectionHeader(0xCAEB, "BigFoot spawner Entity Intelligence");
            RecordSectionHeader(0xCB2A, "Hovering score Entity Intelligence");
            RecordSectionHeader(0xCB48, "Spit Pibbley chunk Entity Intelligence");
            RecordSectionHeader(0xCB87, "Spit Pibbley feather Entity Intelligence");
            RecordSectionHeader(0xCBA6, "Breath bubbles Entity Intelligence");
            RecordSectionHeader(0xCBBD, "Seaweed Entity Intelligence");
            RecordSectionHeader(0xCC2C, "Pibblefish egg Entity Intelligence");
            RecordSectionHeader(0xCC4B, "Pibblefish Entity Intelligence");
            RecordSectionHeader(0xD302, "Snake tail segment Entity Intelligence");

            RecordLabel(0xE065, "L_E055_ITER");
            RecordLabel(0xB283, "L_PSPLT_ITER");
            RecordLabel(0xCA7B, "L_CRPT_ITER");
            RecordLabel(0xCC59, "L_FISH_ITER");

            RecordInlineComment(0xB011, "Read offset to bonus Pibbley path waypoints");
            RecordInlineComment(0xB2FE, "Read offset to bonus Pibbley path waypoints");

            RecordLabel(0xABE1, "cSendHStripToPPU");
            RecordInlineComment(0xABE1, "Transfers a row of attribute table bytes into the PPU for $2000 and $2400");
            RecordInlineComment(0xAC7B, "Transfers a column of attribute table bytes into the PPU");

            RecordLabel(0xDA06, "cAudLoopShrtFrm");
            RecordLabel(0xD5BE, "tkAudLoopShrtFrm");
            RecordLabel(0xB61B, "cSnakeExiting");
            RecordLabel(0x802C, "tkSnakeExiting");
            RecordLabel(0xFC1B, "tkHideUnusedSprs");
            RecordLabel(0xFF81, "sNMI");
            RecordLabel(0xFFF1, "sRST");
            RecordLabel(0xFF80, "sIRQ_BRK");
            RecordLabel(0x8010, "tk_8629");
            RecordLabel(0x8019, "tkAddPtsToTotal");
            RecordLabel(0x801C, "tkUpdateScrollA");
            RecordLabel(0x8022, "tk_DFA8");
            RecordLabel(0x86AA, "tk_8730");
            RecordLabel(0xC688, "tk_C659");
            RecordLabel(0xE237, "tk_sDrawEcShip");
            RecordLabel(0xCC1C, "tkBuryEntity");
            RecordLabel(0x0653, "sDrawEcShip");
            RecordLabel(0xAFC6, "tk_AI_pibsplat");
            RecordLabel(0x802F, "sNewEntity");
            RecordLabel(0xFCF0, "tk_FE4E");
            RecordLabel(0x8EE8, "tk_cDoneMState");
            RecordLabel(0x8251, "cSystemRestart");
            RecordLabel(0x85C2, "cMainTitles");
            RecordLabel(0xFFAE, "sBusyWait");
            RecordLabel(0xFFB5, "sSetMMC1_CTRL");
            RecordLabel(0xFFC9, "sSetMMC1_CHR0");
            RecordLabel(0xFFDD, "sSetMMC1_CHR1");
            RecordLabel(0x81EA, "sQueueSFX_Pn");
            RecordLabel(0x81EE, "sQueueSFX_P0");
            RecordLabel(0x81F2, "sQueueSFX_P1");
            RecordLabel(0x81FA, "sQueueSFX_NZ");
            RecordLabel(0x81FC, "sQueueSFX");
            RecordLabel(0xC3B7, "sChrRomBlit_5A");
            RecordLabel(0xC3BB, "sChrRomBlit_06");
            RecordLabel(0xC3D7, "sChrRomBlit");
            RecordLabel(0x8242, "sRstPPuCtrlMask");
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
            RecordLabel(0xC350, "sLoadNtRun");
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
            RecordLabel(0xC8AC, "sBackCopyZCoords");
            RecordLabel(0x8128, "cChangeMState");
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
            RecordLabel(0x02C7, "sWipeRam");
            RecordLabel(0x03FF, "tkCfgTally");
            RecordLabel(0xC3B1, "sChrBlitJSR0200");
            RecordLabel(0xC3A7, "sLdPalette_4F40");
            RecordLabel(0xC3A9, "sLdPalette_XX40");
            RecordLabel(0xC3AB, "sLdPalette_XXYY");
            RecordLabel(0x92BB, "tk_000C");
            RecordLabel(0x8295, "cGameStart");
            RecordLabel(0x82BF, "cLevelStart");
            RecordLabel(0x8302, "cPlayStart");
            RecordLabel(0x8322, "cLeaveSubLevel");
            RecordLabel(0x81C3, "sFadeOut");
            RecordLabel(0xFCF3, "sRenderEntity");
            RecordLabel(0xFCDE, "sLkupTongueData");
            RecordLabel(0xFE60, "cDrawEntArr");
            RecordLabel(0x95DE, "sLdSnkFacingInY");
            RecordLabel(0xD54A, "sRdNextAudioOpc");
            RecordLabel(0xD69A, "sAudioMethodA");
            RecordLabel(0xD6FB, "sAudioMethodB");
            RecordLabel(0xD8D1, "sAudioMethodC");
            RecordLabel(0xDA8F, "cSfxOpc_0A_0B");
            RecordLabel(0xDAFC, "cSfxOpc_0E_0F_10");
            RecordLabel(0x9FB7, "sFloorFixedPoint");
            RecordLabel(0x866D, "sGetEntMapCoords");
            RecordLabel(0xC62C, "sSetMapSizeTo40");
            RecordLabel(0xC62E, "sSetMapSizeToX");
            RecordLabel(0x9CF3, "sClampEntCoords");
            RecordLabel(0x9D14, "sClampFixedPoint");
            RecordLabel(0xB3D7, "sGive5KPtsFloat");
            RecordLabel(0xB3D9, "sGiveAPtsFloat");
            RecordLabel(0xB408, "sGive100PtsPerS");
            RecordLabel(0xC165, "sBuryEntity");
            RecordLabel(0xAD37, "cStackPpuBltDone");
            RecordLabel(0x9186, "sSetNoStripMode");
            RecordLabel(0xAFB9, "tkEntitySuicide");
            RecordLabel(0xCB45, "tkEntitySuicideB");
            RecordLabel(0xE00C, "tkEntitySuicideC");
            RecordLabel(0x811D, "sCrLfPpuAddr");
            RecordLabel(0xC15B, "caseWindupKey");
            RecordLabel(0xC177, "caseFishTail");
            RecordLabel(0xC17C, "caseContinue");
            RecordLabel(0xC18C, "caseClock");
            RecordLabel(0xC1A6, "caseOneUp");
            RecordLabel(0xC1B4, "caseDiamond");
            RecordLabel(0xC1BB, "caseTongueXtndr");
            RecordLabel(0x8231, "sClr130170Blks");
            RecordLabel(0xBAC0, "sSpawnYEntsX60");
            RecordLabel(0xBAC2, "sSpawnYEntities");
            RecordLabel(0xBDED, "sSpawn3Shrapnels");
            RecordLabel(0xC10F, "cPwrUpCommonCode");
            RecordLabel(0xE17C, "sReifyTemplate");
            RecordLabel(0xC1EC, "cFadeBackToLvl");
            RecordLabel(0x9631, "tkFadeBackToLvl");
            RecordLabel(0x8DA9, "tkDoneMState");
            RecordLabel(0xBE06, "sDoDeadDropDown");
            RecordLabel(0xC884, "sBoostFrameTempo");
            RecordLabel(0xC476, "sOpenLid");

            RecordLabel(0x9292, "sEntC2STrnsfrmX");
            RecordLabel(0x92A2, "sEntC2STrnsfrmY");

            RecordLabel(0x9D45, "sKillPlayer");
            RecordLabel(0x9D48, "sKlPlyrNoArgTmr");
            RecordLabel(0x8185, "sFindEntSpecial");
            RecordLabel(0xD21A, "sTweakUnkn09");

            RecordLabel(0xBD65, "sHaltPlayerA");
            RecordLabel(0xBD66, "sHaltPlayerY");

            RecordLabel(0xA4DB, "cRenderHStrip");
            RecordLabel(0xAC3E, "sSendStripToPPU");
            RecordLabel(0x9F72, "sSt93LoadMapAddr");
            RecordLabel(0x9F74, "sLoadMapAddr");
            RecordLabel(0xB42A, "sAddPtsToTotal");
            RecordLabel(0xC8B9, "sCmpEntityHeight");
            RecordLabel(0xC552, "sSaveEntXY");
            RecordLabel(0xC55C, "sSaveEntXYLow");
            RecordLabel(0xBDF9, "sSpawn8Shrapnels");

            RecordLabel(0xD1D2, "sAddSgmt");
            RecordLabel(0xD1D5, "sAddSgmtNoSave");

            RecordLabel(0xAC78, "tkStripToPPUDone");
            RecordLabel(0xAD83, "lStripToPPUDone");

            RecordVariable(0x012D, "vStripDestAddrHi");
            RecordVariable(0x012E, "vStripDestAddrLo");
            RecordInlineComment(0xAC49, "Switch to +32 PPU_ADDR per write (vertical strip)");
            RecordInlineComment(0xC5AB, "Technically an invalid parameter for this method; but it is called correctly later");
            RecordInlineComment(0x8D69, "A player has pressed start; pause the game");

            RecordInlineComment(0xC165, "Kill an entity, and wipe out its entity template (if present)");
            RecordLabel(0xC162, "sBuryEntityWPts");
            RecordLabel(0xB3CD, "sBuryWPointsB");
            RecordInlineComment(0xC162, "Grant some points for an entity, and then bury it");
            RecordInlineComment(0x8302, "A level/bonus/pond is starting here");
            RecordInlineComment(0x8322, "A bonus/pond is starting or ending here");
            RecordInlineComment(0x8361, "A level is starting here");
            RecordInlineComment(0x8C27, "If transition high byte address isn't negative (i.e is zero), default to transitioning to playing");

            // Transition functions; called through ($DD)
            RecordLabel(0x849C, "cTTo_EndCredits");
            RecordLabel(0x852C, "cTTo_GameOver");
            RecordLabel(0xC5B5, "cTTo_EnterPond");
            RecordLabel(0xC3BF, "cTTo_EndOfLevel");
            RecordLabel(0xC5F9, "cTTo_LeaevBnsPnd");
            RecordLabel(0xC587, "cTTo_BnsWrpScrn");
            RecordLabel(0x8C31, "cTTo_Play");

            RecordLabel(0x0402, "sSwitchToTally");
            RecordLabel(0x8C01, "cDoFadeTypeYYXX");
            RecordLabel(0x8128, "cChangeMState");
            RecordLabel(0x8C0F, "cMState_Fade");
            RecordLabel(0x8C69, "cMState_Play");
            RecordLabel(0x8DC6, "cMState_DDown");
            RecordLabel(0x84CC, "cMState_Crawl");
            RecordLabel(0x813F, "cDoneMState");
            RecordLabel(0x8142, "cDnMSt_noReadAdr");

            // Places where transition functions are loaded into $DD/$DE
            RecordInlineComment(0x8029, "Transition to end credits");
            RecordInlineComment(0x9004, "Transition to game over");
            RecordInlineComment(0x962E, "Transition to entering pond");
            RecordInlineComment(0x9676, "Transition to end of level");
            RecordInlineComment(0xC1F0, "Transition out of bonus/pond back to level");
            RecordInlineComment(0xC891, "Transition entering warp/bonus");
            RecordInlineComment(0x83AB, "Configure transition to playing");

            RecordInlineComment(0x88BA, "Player landed on a spike");
            RecordInlineComment(0x9A25, "Player fell too far");
            RecordInlineComment(0x927D, "Player fell off the screen (2-player only)");

            RecordInlineComment(0x8A22, "Read map tile type");
            RecordInlineComment(0xA1EA, "Read map tile type");
            RecordInlineComment(0xA631, "Read map tile type");

            RecordInlineComment(0x8796, "Read map tile height");
            RecordInlineComment(0x87A3, "Compare to map tile height");
            RecordInlineComment(0x8A3C, "Read map tile height");
            RecordInlineComment(0xA0C2, "Read map tile height");
            RecordInlineComment(0xA0CE, "Read map tile height");
            RecordInlineComment(0xA1DD, "Read map tile height");
            RecordInlineComment(0xA5B3, "Read map tile height");
            RecordInlineComment(0xA5EF, "Read map tile height");
            RecordInlineComment(0xA6A8, "Read map tile height");
            RecordInlineComment(0xA6B9, "Read map tile height");
            RecordInlineComment(0xA712, "Read map tile height");
            RecordInlineComment(0xA858, "Read map tile height");

            RecordInlineComment(0xA246, "Read BG tile address low byte");
            RecordInlineComment(0xA64B, "Read BG tile address low byte");
            RecordInlineComment(0xA250, "Read BG tile address high byte");
            RecordInlineComment(0xA650, "Read BG tile address high byte");


            RecordInlineComment(0xB408, "$05 is zero when called via this address");
            RecordInlineComment(0xA24C, "$7A flips between 0 and 2 or between 1 and 3 here");

            RecordInlineComment(0xAD8C, "$20 bytes");
            RecordInlineComment(0xAD90, "$1F bytes");
            RecordInlineComment(0xAD94, "$1E bytes");
            RecordInlineComment(0xAD98, "$1D bytes");
            RecordInlineComment(0xAD9C, "$1C bytes");
            RecordInlineComment(0xADA0, "$1B bytes");
            RecordInlineComment(0xADA4, "$1A bytes");
            RecordInlineComment(0xADA8, "$19 bytes");
            RecordInlineComment(0xADAC, "$18 bytes");
            RecordInlineComment(0xADB0, "$17 bytes");
            RecordInlineComment(0xADB4, "$16 bytes");
            RecordInlineComment(0xADB8, "$15 bytes");
            RecordInlineComment(0xADBC, "$14 bytes");
            RecordInlineComment(0xADC0, "$13 bytes");
            RecordInlineComment(0xADC4, "$12 bytes");
            RecordInlineComment(0xADC8, "$11 bytes");
            RecordInlineComment(0xADCC, "$10 bytes");
            RecordInlineComment(0xADD0, "$0F bytes");
            RecordInlineComment(0xADD4, "$0E bytes");
            RecordInlineComment(0xADD8, "$0D bytes");
            RecordInlineComment(0xADDC, "$0C bytes");
            RecordInlineComment(0xADE0, "$0B bytes");
            RecordInlineComment(0xADE4, "$0A bytes");
            RecordInlineComment(0xADE8, "$09 bytes");
            RecordInlineComment(0xADEC, "$08 bytes");
            RecordInlineComment(0xADF0, "$07 bytes");
            RecordInlineComment(0xADF4, "$06 bytes");
            RecordInlineComment(0xADF8, "$05 bytes");
            RecordInlineComment(0xADFC, "$04 bytes");
            RecordInlineComment(0xAE00, "$03 bytes");
            RecordInlineComment(0xAE04, "$02 bytes");
            RecordInlineComment(0xAE08, "$01 byte");

            RecordLabel(0xC160, "cPickUpPowerup");
            RecordInlineComment(0xA1D0, "Start of rendering a single block");

            RecordSectionHeader(0xA196, "Strip rendering subroutine (setup)");
            RecordSectionHeader(0xA1D0, "Strip rendering subroutine (body)");
            RecordSectionHeader(0xA35F, "Strip rendering subroutine (teardown)");
            RecordLabel(0xA197, "sRenderStrip");
            RecordLabel(0xA1A3, "cRenderVStrip");
            RecordLabel(0xA1D0, "lRenderBlock");
            RecordLabel(0xA2D8, "lPshRndWallLedge");
            RecordLabel(0xA2D9, "lRenderWallLedge");
            RecordLabel(0xA301, "lRenderLedge");
            RecordLabel(0xA30E, "lPushAndCliff");
            RecordLabel(0xA30F, "lCliff");
            RecordLabel(0xA31C, "lNullBlock");
            RecordLabel(0xA323, "lPushNBlockDone");
            RecordLabel(0xA324, "lBlockDone");
            RecordLabel(0xA35F, "lColumnDone");
            RecordLabel(0xA365, "lRsFinishing");
            RecordLabel(0xA381, "tkRsDoneA");
            RecordLabel(0xA414, "tkRsDoneB");
            RecordLabel(0xA49B, "lRsDone");
            RecordLabel(0xA384, "lRhsDone");
            RecordLabel(0xA417, "lRvsDone");

            RecordInlineComment(0xA1E3, "$04: (current tile height + 1) * 2");
            RecordInlineComment(0xA1F5, "$07: current tile type");

            RecordLabel(0x9FC6, "sRndStripSetup7C");
            RecordLabel(0x9FD2, "sRndStripSetup");
            RecordLabel(0xA149, "lRndStripStpFin");
            RecordLabel(0xA149, "lRndStripStpFin");
            RecordLabel(0xA0EC, "tkRndStripStpFin");
            RecordLabel(0xA0EF, "lRssCaseA");
            RecordLabel(0xA0E9, "tkRssCaseB");
            RecordLabel(0xA162, "lRssCaseB");
            RecordUnconditionalBranch(0x9FFD);

            RecordLabel(0x93B2, "sUpdateScroll");
            RecordLabel(0x93BA, "sUpdateScrollA");

            // UNKNOWN SUBROUTINES

            var reldelSubs = new[] {
                0xD4F4, 0xD4FF, 0xD501, 0xD513, 0xD525,
                0xD527, 0xD529,
            };
            foreach (var i in Enumerable.Range(0, reldelSubs.Length))
                RecordLabel(reldelSubs[i], string.Format("RELDEL_{0:X4}", reldelSubs[i]));


            var eisubs = new[] {
                0x9DC5, 0xDF1A, 0xC09F, 0xBE06, 0x9C3B, 0xBE23,
                0xE055, 0x86B6, 0x9DFC, 0x8185, 0xD529, 0xD513, 0xE21E,
                0x9DA3, 0x9D54, 0xC476, 0x95EB, 0x9586, 0x9CDE, 0xDF6F,
                0xCD27, 0xB539, 0xC578, 0xDF46, 0xCD4C, 0xBF4E, 0xE161,
                0xAFFA, 0xCF40, 0xDF48, 0xB247, 0xB231, 0x8689, 0xB283,
                0xB315, 0xB1E5, 0xB529, 0xDFA8, 0xB732, 0xBB88, 0xDF55,
                0xBE83, 0xB9A7, 0xC567, 0xBAFC, 0xBB8F, 0xC64E, 0xB815,
                0x9584, 0xBF44, 0xDF9E, 0xBAFF, 0xB527, 0xC884, 0xD527,
                0xD501, 0xCCB5, 0xCC59, 0xC57C, 0xDF9B, 0xD4D2,
                0x95D8, 0xD4E1, 0xD4F4, 0x8A1A, 0x8A51, 0x87E1,
            };
            foreach (var i in Enumerable.Range(0, eisubs.Length))
                RecordLabel(eisubs[i], string.Format("EISUB_{0:X4}", eisubs[i]));

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
                0xBD65, 0xBD66, 0xBD77, 0xBE06,
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
                0xD525, 0xD527, 0xD529, 0xD54A,
                0xD62A, 0xD69A, 0xD6FB, 0xD8A1, 0xD8D1,
                0xD9E6, 0xDA63, 0xDF1A, 0xDF46, 0xDF48,
                0xDF55, 0xDF6F, 0xDF9B, 0xDF9E, 0xDFA8,
                0xE037, 0xE042, 0xE055, 0xE161,
                0xE17C, 0xE209, 0xE21E, 0xE230, 0xF51D,
                0xF523, 0xF527, 0xFA31, 0xFC1B, 0xFCA6,
                0xFCBA, 0xFCDE, 0xFCF3, 0xFE6C,
            };
            foreach (var i in Enumerable.Range(0, unsubs.Length))
                RecordLabel(unsubs[i], string.Format("UNSUB_{0:X4}", unsubs[i]));



            // LOOPS

            var loopBranches = new[] {
                0x80BE, 0x80E7, 0x80EA, 0x8105, 0x8170, 0x81E3, 0x8233,
                0x81CA, 0x82FA, 0x836B, 0x8387, 0x91B5, 0xA827, 0xFB35,
                0x8395, 0x842D, 0xE241, 0x91F5, 0xAD29, 0xFAE7, 0xC6AE,
                0xC402, 0xD643, 0xD6DE, 0xE2D5, 0xA1BB, 0xAE0F, 0xC738,
                0xF530, 0xFBB9, 0xD284, 0x959E, 0xAE42, 0xD580,
                0xFA33, 0xD247, 0xD26C, 0x818A, 0xA2F1, 0xB015, 0xCCD9,
                0x9036, 0xE185, 0xE287, 0x9936, 0xA361, 0xB15F, 0xD2CB,
                0x8155, 0x814B, 0x8219, 0xC659, 0xA381, 0xB1A7,
                0xB41A, 0xB436, 0xBACE, 0x994C, 0xA3E0, 0xB1B6,
                0x8324, 0x83CF, 0x84F1, 0x84F3, 0xA3E3, 0xB2B8,
                0x85EF, 0x85DC, 0x8C86, 0x9A3C, 0xA463, 0xB7AC,
                0x85D3, 0x8632, 0x86D2, 0x8A08, 0xA466, 0xBECD,
                0x8DD3, 0x8DF9, 0x8E3B, 0x9B2B, 0xA53C, 0xBF79,
                0x8E63, 0x910B, 0xABF4, 0x85D1, 0xA57D, 0xC2B5,
                0x891D, 0xFED8, 0x8CD9, 0x9E98, 0xA584, 0xC2CD,
                0x87FA, 0xFD8C, 0xFED9, 0xFF1E, 0xA5E7, 0xC4EF,
                0xFF1D, 0xE281, 0x8F1B, 0xA063, 0xA695, 0xE18C,
                0x8F7E, 0x8FA0, 0x933E, 0xA094, 0xA7D3, 0xFA99,
            };
            foreach (var i in Enumerable.Range(0, loopBranches.Length))
                RecordLabel(loopBranches[i], string.Format("loop_{0:D3}", i));


            // SKIPS

            var skipBranches = new[] {
                0x80B3, 0x8166, 0xB904, 0xB901, 0xB89F, 0xA007, 0xBB26, 0xBDEA, 0x84FE, 0x8E37, 0x8E46, 0x8E5D,
                0x810F, 0x8144, 0x81E1, 0x815E, 0xB8B5, 0xB382, 0x922B, 0x923E, 0x9DC7, 0xA384, 0xA878, 0xA1D7,
                0xB262, 0x8984, 0xFF15, 0x8AEB, 0x8FD0, 0x9223, 0x92AE, 0x949D, 0x9EB8, 0xA3B1, 0xA8A7,
                0xBB46, 0xBDED, 0x8546, 0x877C, 0x8E5B, 0x8E79, 0x92D1, 0x94BF, 0x9ECC, 0xA3D0, 0xB515,
                0xAC58, 0xAE1F, 0xB025, 0xB075, 0xB0CE, 0xB129, 0xB19B, 0xB215, 0xAD83, 0xB2D6, 0xA134,
                0xB266, 0x8993, 0xFF2A, 0x8B01, 0x8FD8, 0x924B, 0x92EE, 0x9740, 0x9F3D, 0xA40F, 0xE0C7,
                0xAC6D, 0xAE34, 0xB034, 0xB08E, 0xB0D1, 0xB13C, 0xB1A0, 0xB218, 0xB292, 0xB311, 0xE0DD,
                0x82B6, 0x82F2, 0x8379, 0x8361, 0x83A6, 0x83DE, 0x942B, 0x975B, 0x9F59, 0xA488, 0xE0E1,
                0xAC78, 0xAE57, 0xB037, 0xB09F, 0xB0EC, 0xB13F, 0xB1A6, 0xB22B, 0xB29A, 0xB4C4, 0xE11D,
                0xB7E3, 0x89FD, 0xFF2D, 0x8B04, 0x8FFD, 0x925B, 0x9445, 0x9763, 0x9F88, 0xA495, 0xE13A,
                0xAC7B, 0xAE5D, 0xB04B, 0xB0A2, 0xB0EF, 0xB16E, 0xB1BB, 0xAD57, 0xB2AA, 0xB4CB, 0xE147,
                0x83F7, 0x8416, 0x841F, 0x8436, 0x8452, 0x8E9E, 0x9455, 0x976D, 0x9F9B, 0xA496, 0xE156,
                0xAD09, 0xAE6E, 0xB066, 0xB0B2, 0xB101, 0xB177, 0xB1C3, 0xAD5A, 0xB2BC, 0xB4FE, 0xE15E,
                0x87F4, 0xFD0A, 0xFF4F, 0x8CA3, 0x9007, 0x9289, 0x9457, 0x9939, 0xA06B, 0xA55D, 0xD31E,
                0xAD30, 0xAE79, 0xB06B, 0xB0B8, 0xB10D, 0xB184, 0xB1D5, 0xAD64, 0xB2D4, 0xB509, 0xD336,
                0x9D1C, 0x9D76, 0x9D86, 0x9D95, 0x9D97, 0x9D99, 0x947A, 0x9947, 0xA078, 0xA56C, 0xD35E,
                0x87FD, 0xFD15, 0xFF59, 0x8CCA, 0x900B, 0x92F9, 0x949A, 0x995D, 0xA07E, 0xA581, 0xD361,
                0x9D9F, 0x9E26, 0x833C, 0x85B2, 0x87DD, 0x8EB6, 0x9487, 0x9965, 0xA092, 0xA58A, 0xD378,
                0x8815, 0xFD2E, 0xFF5A, 0x8CF5, 0x901D, 0x9319, 0x948A, 0x9988, 0xA111, 0xA5A4, 0xD383,
                0xD564, 0xD578, 0xD5EF, 0xD6AA, 0xD6C8, 0xD6D8, 0x94B7, 0x9990, 0xA125, 0xA5BF, 0xD38E,
                0x8824, 0xFD63, 0xFF22, 0x8D03, 0x91D2, 0x9321, 0x94C6, 0x99B8, 0xA127, 0xA5CD, 0xD39A,
                0xD6E6, 0xD6EE, 0xD702, 0x85A7, 0x8EC3, 0x8605, 0x94DF, 0x99DF, 0xA132, 0xA5D4, 0xD39C,
                0xCD6D, 0xCDF6, 0xCC8F, 0xCB4C, 0xCB1C, 0xDA23, 0xD711, 0xD76B, 0xD7C6, 0xD81E, 0xD875,
                0x8840, 0xFD69, 0xE252, 0x8D36, 0x91ED, 0x9324, 0x94E6, 0x99E1, 0xA149, 0xA619, 0xD3B7,
                0xCD9D, 0xCE0C, 0xCC9E, 0xCB62, 0xD5C1, 0xDA49, 0xD73C, 0xD77E, 0xD7C9, 0xD82F, 0xD8D6,
                0xE2F5, 0xB753, 0xB6F4, 0xA037, 0xA043, 0xB3AF, 0x9596, 0x99F5, 0xA162, 0xA628, 0xD3B9,
                0xCDB3, 0xCE23, 0xCBAE, 0xCB36, 0xD5D1, 0xDAFE, 0xD744, 0xD78A, 0xD7CC, 0xD858, 0xD8E2,
                0x8850, 0xFD6F, 0xE25C, 0x8D4A, 0x9283, 0x932D, 0x95B8, 0x99F8, 0xA17C, 0xA63A, 0xD3C3,
                0xCDC9, 0xCE39, 0xCBBA, 0xCAF8, 0xD5DC, 0xDB11, 0xD753, 0xD79E, 0xD7DA, 0xD865, 0xD90D,
                0xBA48, 0xBAC2, 0xBEC4, 0xC06A, 0xC092, 0xC0D8, 0xC213, 0xC268, 0xC2BF, 0xC31B, 0xD3E7,
                0xCDDF, 0xCC63, 0xCC13, 0xCB19, 0xDA0F, 0xDAB7, 0xD754, 0xD7B3, 0xD7FC, 0xD868, 0xD946,
                0xD261, 0xCD14, 0xDF69, 0xDFCF, 0xE003,
                0xB44A, 0xBE02, 0x834C, 0x857C, 0x8E39, 0x8EDD, 0x95CF, 0x99FA, 0xA18C, 0xA649, 0xD3EC,
                0xD263, 0xDF39, 0xDF84, 0xDFD4,
                0x8865, 0xFD84, 0xE271, 0x8D4C, 0x92F3, 0x9417, 0x9665, 0x99FD, 0xA1C5, 0xA65F, 0xD414,
                0xCD00, 0xDF52, 0xDF98, 0xE000,
                0xC57F, 0xC7B0, 0xC7F9, 0xC833, 0xC879, 0xE193, 0xFA4A, 0xFBAC, 0xFB12, 0xE0AE, 0xD41C,
                0xFBA6, 0xFBCA, 0xFBD3, 0xFBE2, 0xFCB5, 0xFF61, 0x9742, 0x9A14, 0xA1C7, 0xA66E, 0xD421,
                0x8868, 0xFDAF, 0xE27C, 0x8D5B, 0x9363, 0x93C5, 0x977F, 0x9A25, 0xA1DC, 0xA676, 0xD433,
                0xBA62, 0xBA98, 0xBEE2, 0xC06C, 0xC0A0, 0xC0F4, 0xC24B, 0xC284, 0xC33C, 0xC519, 0xD454,
                0xFFA6, 0xBE55, 0x853B, 0x8554, 0x87DB, 0x8EE0, 0x97B7, 0x9A28, 0xA1F3, 0xA686, 0xD466,
                0xC5D1, 0xC7BF, 0xC81D, 0xC855, 0xE1F4, 0XFAC9, 0xFB03, 0xE070, 0xE09A, 0xE0BF, 0xD468,
                0xBA67, 0xBAA2, 0xBF00, 0xC075, 0xC0AB, 0xC0FE, 0xC253, 0xC28A, 0xC4DF, 0xC53F, 0xD46A,
                0x888C, 0xFDBA, 0xE27F, 0x8D5D, 0x936F, 0x93E6, 0x97CB, 0x9A50, 0xA204, 0xA694, 0xD474,
                0xB521, 0xB57D, 0xB5E8, 0xB64D, 0xB69F, 0xB6C9, 0xB787, 0xB8C5, 0xB97F, 0xB9DB, 0xD47F,
                0xBB85, 0xBBB2, 0xBBB5, 0xBC33, 0xBBF0, 0xBC04, 0x97EB, 0x9A57, 0xA20A, 0xA69B, 0xD49E,
                0xC5D3, 0xC7E3, 0xC822, 0xC871, 0xE1A1, 0xE202, 0xFBF6, 0xFB0F, 0xE07F, 0xE0C4, 0xD4C3,
                0x8896, 0xFDD9, 0xE2A1, 0x8D80, 0x9378, 0x93F1, 0x97EE, 0x9A63, 0xA220, 0xA6C3, 0xD4B9,
                0xBA6A, 0xBB7F, 0xBF03, 0xC07C, 0xC0C7, 0xC174, 0xC206, 0xC2B3, 0xC4E1, 0xC541, 0xD4BB,
                0xC7A5, 0xC7F7, 0xC82E, 0xC87F, 0xD523, 0xE1EF, 0xFB75, 0xFC16, 0xE088, 0xE0BB, 0xC6C7,
                0xBC1B, 0xBC23, 0xBC4C, 0xBC6F, 0x8C25, 0x8E13, 0x9815, 0x9A66, 0xA222, 0xA6C8, 0xC6C1,
                0x889E, 0xFDE9, 0xE2A9, 0x8DA2, 0x937F, 0x93F7, 0x981A, 0x9A7F, 0xA233, 0xA6D1, 0xC6D3,
                0xBEAC, 0xBEF2, 0xBF1F, 0xC08E, 0xC0CA, 0xC1A0, 0xC1F3, 0xC2C2, 0xC502, 0xC572, 0xC6F3,
                0xB523, 0xB587, 0xB5EB, 0xB66A, 0xB6AD, 0xB6F7, 0xB835, 0xB923, 0xB9A1, 0xB9F8, 0xC6F6,
                0xBC6C, 0xBC83, 0xBC76, 0xBC7E, 0xBC9F, 0xBC9C, 0x981D, 0x9A82, 0xA246, 0xA6ED, 0xC713,
                0x88A0, 0xFE20, 0xE2BE, 0x9382, 0x93FC, 0x9825, 0x9A85, 0xA26C, 0xA701, 0xE091, 0xC732,
                0xBCAE, 0xBCE3, 0xBC85, 0xBCA2, 0xBE2B, 0x90D1, 0x983B, 0x9A98, 0xA292, 0xA704, 0xC740,
                0xB557, 0xB58A, 0xB5F6, 0xB67C, 0xB6BF, 0xB763, 0xB838, 0xB92A, 0xB9C4, 0xBA0F, 0xC744,
                0x88A3, 0xFE46, 0xE2C6, 0x8EEB, 0x93A0, 0x9400, 0x983E, 0x9AAC, 0xA294, 0xA722, 0xC757,
                0xBCF0, 0xBD58, 0xBD17, 0xBD34, 0xBD74, 0xB3CB, 0x984B, 0x9B30, 0xA299, 0xA724, 0xC904,
                0x88A6, 0xFE4B, 0x8A5F, 0x8EF7, 0x93A9, 0x9634, 0x9856, 0x9B32, 0xA2AA, 0xA726, 0xC90B,
                0xB426, 0xBE5E, 0x8349, 0x855A, 0x8BDA, 0x90F4, 0x9873, 0x9B36, 0xA2BA, 0xA72D, 0xC93D,
                0x88C5, 0xFE4E, 0x8A6D, 0x8F11, 0x93AC, 0x9709, 0x988D, 0x9B47, 0xA2C0, 0xA747, 0xC944,
                0xB561, 0xB5DA, 0xB612, 0xB680, 0xB6C7, 0xB772, 0xB844, 0xB93F, 0xB9EE, 0xBA22, 0xC961,
                0xCB96, 0xC676, 0xC66D, 0xB8EC, 0xB70A, 0xB3C9, 0x989C, 0x9B54, 0xA2D5, 0xA754, 0xC968,
                0x88CF, 0xFE60, 0x8A7C, 0x8F17, 0x964A, 0x9756, 0x98C0, 0x9B57, 0xA2D8, 0xA756, 0xC970,
                0xBB3E, 0xBE1B, 0x8393, 0x8572, 0x8C45, 0x9117, 0x98CC, 0x9B96, 0xA2D9, 0xA760, 0xC97F,
                0x88FC, 0xFE91, 0x8A8C, 0x8F2B, 0x965E, 0x98C3, 0x98E9, 0x9BBB, 0xA2F8, 0xA764, 0xC9A2,
                0xD4C4, 0xB74D, 0xB750, 0x9FFF, 0xA05B, 0xB40E, 0x9920, 0x9BFF, 0xA301, 0xA779, 0xC8D4,
                0x8939, 0xFE96, 0x8A8E, 0x8F3A, 0x96C3, 0x990E, 0x992A, 0x9C1E, 0xA30E, 0xA77E, 0xC9EC,
                0xBB2D, 0xBE0D, 0x82E8, 0x859E, 0x8C4A, 0x9183, 0x9B19, 0x9C7C, 0xA30F, 0xA781, 0xCA08,
                0x893C, 0xFEA3, 0x8A90, 0x8F42, 0x9794, 0x9AAF, 0x9B24, 0x9DB3, 0xA35A, 0xA783, 0xCA19,
                0xE176, 0xE1C5, 0xE1EB, 0xE1FF, 0xB385, 0xB434, 0x9B27, 0x9E2E, 0xA35F, 0xA78B, 0xCA26,
                0x8951, 0xFEC9, 0x8A92, 0x8F45, 0x97AC, 0x9AB7, 0x9BA2, 0x9F64, 0xA375, 0xA7B7, 0xCA2F,
                0xB5B1, 0xBB5A, 0x851D, 0x85AF, 0x8A3C, 0xAEAE, 0x9C25, 0xA0E9, 0xA417, 0xA7D0, 0xCA4A,
                0x8959, 0xFEDD, 0x8AA7, 0x8F5C, 0x97AE, 0x9B0A, 0x9CD9, 0xA0EC, 0xA42A, 0xA7E3, 0xCA4F,
                0xFA55, 0xFAB4, 0xFA71, 0xFA75, 0xFAD1, 0xFADF, 0x9E42, 0xA0EF, 0xA431, 0xA7EA, 0xCA52,
                0x8963, 0xFEE5, 0x8ABA, 0x8F66, 0x9B0D, 0x9BAD, 0x9E54, 0xA0F9, 0xA43D, 0xA821, 0xCA74,
                0xFABF, 0xFBB7, 0xBFA2, 0x8593, 0x8A2B, 0xAF56, 0x9EFF, 0xC160, 0xA45E, 0xA83A, 0xCA91,
                0x896B, 0xFEE8, 0x8ACC, 0x8F77, 0x9C2C, 0x9C7E, 0xA0AF, 0xA1A3, 0xA49B, 0xA842, 0xCADB,
                0x85BC, 0x8646, 0x864C, 0x8666, 0x86E2, 0x870A, 0xA0BB, 0xA323, 0xA4E3, 0xA850, 0xCF55,
                0x896F, 0xFF0A, 0x8AD0, 0x8FC0, 0x9C38, 0x9C81, 0xA0C5, 0xA324, 0xA516, 0xA853, 0xCF5D,
                0x8773, 0x8776, 0x89A8, 0x89BD, 0x89F5, 0xAF5E, 0xA0D1, 0xA34D, 0xA518, 0xA867, 0xCF5F,
                0x8982, 0xFF14, 0x8ADE, 0x96BC, 0x9CAB, 0x9CC3, 0xA0D7, 0xA350, 0xA53B, 0xA871, 0xABE1,
                0xD511, 0xD51F, 0xD537, 0xD545, 0xC973, 0xC99C
            };
            foreach (var i in Enumerable.Range(0, skipBranches.Length))
                RecordLabel(skipBranches[i], string.Format("skip_{0:D3}", i));


            // FAR BRANCHES
            var farBranches = new[]
            {
                0x8322, 0x8302, 0x968F, 0xA4DB, 0xB52B, 0xB53B, 0xB61B,
                0xB7FA, 0xBAC0, 0xBB72, 0xBD5E, 0xBDF9, 0xC8C7, 0xC5BF,
                0x8025, 0x8209, 0x84C2, 0x85FD, 0x8730, 0x88C2, 0xE23A, 0x8D84,
                0xBA37, 0xBCE8, 0xC10F, 0xC1EC, 0xC54F, 0xD95E,
                0x8D88, 0x93BA, 0x9610, 0x96A7, 0x9F74, 0xA1D0, 0xA31C, 0xA365,
                0xA57B, 0xA5C5, 0xA5EA, 0xA602, 0xA62C, 0xA7B1, 0xB4CE, 0xB9E8,
                0xBA26, 0xBA31, 0xC88D, 0xD370, 0xCC1F, 0xD94F,
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
                0xFB0C, 0xE0DA,
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
            RecordInlineComment(0xCC46, "Transmute into a Pibbleyfish and tail-call into that AI" );
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
            RecordInlineComment(0xC204, "Populate blank or icy Pibbley hole mushroom arrangement (by level)" );


            RecordInlineComment(0xBB79, "Play noise SFX $02 'Explosion'" );
            RecordInlineComment(0xBDFB, "Play noise SFX $02 'Explosion'" );
            RecordInlineComment(0x9A95, "Play noise SFX $04 'Pibbley chomp'" );
            RecordInlineComment(0xB4C8, "Repeat noise SFX $06 'Pibbley ejection'" );
            RecordInlineComment(0xC2DB, "Play pulse SFX $08 'Lid opening''" );
            // 0A is UNUSED
            RecordInlineComment(0x8EA6, "Play pulse SFX $0C 'PLAY ON/1-UP'" );
            RecordInlineComment(0x8EA9, "Play pulse SFX $0C 'PLAY ON/1-UP'" );
            RecordInlineComment(0xC129, "Play pulse SFX for current powerup (see $C1CF: 0C 0E 10 20 40 44 46)" );
            RecordInlineComment(0xC132, "Play pulse SFX for current powerup (see $C1CF: 0C 0E 10 20 40 44 46)" );
            // ------------------------------ pulse --- $0E at 0xC129
            // ------------------------------ pulse --- $10 at 0xC129
            RecordInlineComment(0xC86B, "Play pulse SFX $12 'Wormhole opening'" );
            RecordInlineComment(0xC86E, "Play pulse SFX $12 'Wormhole opening'" );
            RecordInlineComment(0xC984, "Repeat noise SFX $14 'Wormhole sucking up object'" );
            RecordInlineComment(0xBF40, "Play noise SFX $16 'THUD' rate-limited" );
            RecordInlineComment(0xB6B4, "Repeat pulse SFX $18 'Scale bell ring'" );
            RecordInlineComment(0xB6B9, "Repeat pulse SFX $18 'Scale bell ring' (except it's buggy)" );
            RecordInlineComment(0xBC94, "Play pulse SFX $1A 'Exploding enemy pulse'" );
            RecordInlineComment(0xBC99, "Play noise SFX $1C 'Exploding enemy noise" );
            RecordInlineComment(0xBD61, "Play pulse SFX $1E 'Snake OW'" );
            // ------------------------------ pulse --- $20 at 0xC129
            RecordInlineComment(0xBD93, "Play pulse SFX $22 'Snake death spin'" );
            // ------------------------------ pulse --- $24 on blit page $00
            // ------------------------------ pulse --- $26 on blit page $00
            // ------------------------------ pulse --- $28 on blit page $00
            // ------------------------------ pulse --- $2A on blit page $00
            // ------------------------------ noise --- $2C on blit page $00
            RecordInlineComment(0x99B8, "Play pulse SFX $2E 'Jaws (slow)' or $50 'Jaws (fast)'" );
            RecordInlineComment(0x9D4E, "Play pulse SFX $30 'ARRRGGG'" );
            RecordInlineComment(0x9D51, "Play pulse SFX $30 'ARRRGGG'" );
            RecordInlineComment(0x9AB9, "Play noise SFX $32 'Pibbley chunk spit'" );
            RecordInlineComment(0xD6D2, "Play pulse SFX $34 'Crescendo'" );
            RecordInlineComment(0xD6D5, "Play pulse SFX $34 'Crescendo'" );
            RecordInlineComment(0xD372, "Repeat pulse SFX $36 'Exit door point score'" );
            RecordInlineComment(0xD375, "Repeat pulse SFX $36 'Exit door point score'" );
            RecordInlineComment(0xBCEA, "Play pulse SFX $38 'Bounce/lick enemy'" );
            RecordInlineComment(0xBC01, "Repeat noise SFX $3A 'Lick foot'" );
            RecordInlineComment(0x86A4, "Repeat noise SFX $3C 'diving splash'" );
            RecordInlineComment(0x9F05, "Repeat noise SFX $3E 'Water jump jet'" );
            // ------------------------------ pulse --- $40 at 0xC129
            RecordInlineComment(0xFABC, "Play pulse SFX $42 'Time running out beep'" );
            // ------------------------------ pulse --- $44 at 0xC129
            // ------------------------------ pulse --- $46 at 0xC129
            // ------------------------------ pulse --- $48 on blit page $60
            RecordInlineComment(0x9E14, "Repeat noise SFX $4A 'Rocket take-off'" );
            // ------------------------------ pulse --- $4C on blit page $60
            RecordInlineComment(0xB398, "Play pulse SFX $4E 'Snake gulp'" );
            // ------------------------------ pulse --- $50 at 0x99B8


            RecordInlineComment(0x8D77, "Configure 'HOLD' drop-down" );
            RecordInlineComment(0x8F0D, "Configure 'PLAY ON' drop-down" );
            RecordInlineComment(0x8F15, "Configure 'GAME OVER' drop-down again??" );
            RecordInlineComment(0x8F57, "Configure ??? drop-down" );
            RecordInlineComment(0xBE1B, "Configure 'GAME OVER', 'SCORE/LEFT', 'TIME OUT' or 'ALL GONE' drop-down depending on fate" );
            RecordInlineComment(0xB3C3, "Crescendo SFX setup (5/5 Pibbley eaten on bonus)" );
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
            RecordInlineComment(0xFE76, "Read entity arrangement data address high byte" );
            RecordInlineComment(0xFE7B, "Read entity arrangement data address low byte" );
            RecordInlineComment(0x96B7, "Load 'smushed snake' arrangement");
            RecordUnconditionalBranch(0x8C0D);
            RecordUnconditionalBranch(0xB9D7);
            RecordUnconditionalBranch(0xE207);
            RecordUnconditionalBranch(0xCBA0);
            RecordUnconditionalBranch(0x8A18);
            RecordUnconditionalBranch(0xAC3C);
            RecordUnconditionalBranch(0x813D);
            RecordUnconditionalBranch(0xC5BD);
            RecordUnconditionalBranch(0x9420);
            RecordUnconditionalBranch(0x8627);
            RecordUnconditionalBranch(0x8771);
            RecordUnconditionalBranch(0xA202);
            RecordUnconditionalBranch(0xA1C3);
            RecordInlineComment(0xFF5D, "One of these two branches will be taken");
            RecordInlineComment(0xC688, "One of these two branches will be taken" );
            RecordInlineComment(0x852C, "'Game over' fade subtype" );
            RecordInlineComment(0x8025, "??? how does PC get to this point ???" );
            RecordInlineComment(0x8A0E, "Tail-call this method to another" );
            RecordInlineComment(0x82C4, "Call sLdEntTemplates");
            RecordInlineComment(0xC5B8, "Call sLdEntTemplates");
            RecordInlineComment(0x843F, "Call sInitLevel");
            RecordInlineComment(0xC58C, "Call sPrtBonusWarpMsg");
            RecordInlineComment(0xC5FB, "Call sReturnSnakes");
            RecordInlineComment(0x843A, "Call sLdPalette(00,10)");
            RecordInlineComment(0x8475, "Call sLdPalette(20,40)");
            RecordInlineComment(0x849F, "Call sLdPalette(4F,40)");
            RecordInlineComment(0xC3CC, "Call sLdPalette(4F,40)");
            RecordInlineComment(0x8316, "Call sDynamicPage48");
            RecordInlineComment(0x83BB, "Call sDynamicPage1E");
            RecordInlineComment(0x855C, "Call sDynamicPage24");
            RecordInlineComment(0xC5E0, "Call sDecodeRleMap");
            RecordInlineComment(0x91DE, "Invoke AI method");
            RecordInlineComment(0xD5AF, "Read audio opcode address into jump vector");
            RecordInlineComment(0xD5BB, "Jump to audio opcode instructions");
            RecordInlineComment(0x92BB, "Thunk to AI instructions");
            RecordInlineComment(0xAD34, "Jump to pulling stack and pushing it into PPU_ADDR");
            RecordInlineComment(0xD5CE, "Jump to audio opcode read function");
            RecordInlineComment(0xD96D, "Jump to audio opcode instructions");
            RecordInlineComment(0x825A, "Configure vertical mirroring");
            RecordInlineComment(0x829A, "Configure vertical mirroring");
            RecordInlineComment(0x8304, "Configure vertical mirroring");
            RecordInlineComment(0x846A, "Configure horizontal mirroring");
            RecordInlineComment(0x8148, "Enable NMI interrupt during vblank");
            RecordInlineComment(0x8244, "-NMI, PPU master, 8x8 sprites, $1000 BG, $0000 sprites, PPU_ADDR increment by 1, $2000 base address");
            RecordInlineComment(0x814B, "Spin, generating entropy");
            RecordInlineComment(0x85DA, "Load new background palette based on $90 (either #$20 or #$30)");
            RecordInlineComment(0x8458, "Change to TRANSITION machine state");
            RecordInlineComment(0x84C9, "Change to CRAWL machine state");
            RecordInlineComment(0x8C3D, "Change to PLAY machine state");
            RecordInlineComment(0x8D80, "Change to DROPDOWN machine state");
            RecordInlineComment(0x9016, "Change to TRANSITION machine state");
            RecordInlineComment(0xC3D4, "Change to TALLY machine state");
            RecordInlineComment(0xFD67, "Rendering snake tongue at this point");
            RecordInlineComment(0xFE58, "Rendering Pibbley stuck to snake tongue at this point");

            RecordInlineComment(0x850D, "Check if a player has pressed any button/DPAD and if so go to player select screen");
            RecordInlineComment(0x8587, "Check if a player has pressed up/down/select and if so toggle the number of players");
            RecordInlineComment(0x8593, "Check if a player has pressed start and if so, start the game!");
            RecordInlineComment(0x8ED2, "Check if a game-overed player has pressed a button and if so, continue playing");
            RecordInlineComment(0x9702, "Check if a slide-immune player has pressed a button and if so, remove the immunity");
            RecordInlineComment(0x9B57, "Check if a player is holding the tongue button (B)");
            RecordInlineComment(0x9B68, "Check if a player just pressed the tongue button (B)");
            RecordInlineComment(0x9BFF, "Check if a player just pressed the jump button (A)...");
            RecordInlineComment(0x9C03, "...they did!");

            RecordInlineComment(0x971F, "A player is entering the level; ignore what their controller is doing...");
            RecordInlineComment(0x9740, "... and override it with the pre-recorded input");
            RecordInlineComment(0xB6B7, "This is probably a bug: should be #$19, not $19");

            RecordInlineComment(0x8ED4, "This is probably a bug or a ROM dump error: should both be 'press' variants, not 'hold'");
            RecordInlineComment(0x8E9E, "Check if dead player has pressed a non-DPAD button and if so, use a continue");
            RecordInlineComment(0xFE2D, "EAT: Attach Pibbley to tongue");
            RecordInlineComment(0x94B9, "EAT: Attach Pibbley to mouth");
            RecordInlineComment(0x94DA, "EAT: Begin chewing");
            RecordInlineComment(0x9A85, "EAT: Increment chewing counter");
            RecordInlineComment(0x9B24, "EAT: Swallow Pibbley; mouth is now clear");
            RecordInlineComment(0xB38A, "Tongue touched a Pibbley!");
            RecordInlineComment(0xFE26, "Check for Pibbley/tongue contact");
            RecordInlineComment(0xFE48, "Clear Pibbley contact register");
            RecordInlineComment(0xFD03, "Check if this entity is one of the snakes");
            RecordInlineComment(0xFD0A, "Snake update/rendering code here");
            RecordInlineComment(0xFE60, "Snake update/rendering completed or skipeed; time to produce OAM for the entity's layout");
            RecordInlineComment(0xB513, "Spawn a bomb instead of a Pibbley");
            RecordInlineComment(0xBC04, "BigFoot slain - grant points");
            RecordInlineComment(0xB648, "Snake exits level - grant points");
            RecordInlineComment(0xC852, "Anvil slams scale - grant points");
            RecordInlineComment(0x885A, "Snake jumped into pond - grant points");
            RecordInlineComment(0xBBFA, "BigFoot licked - grant points");
            RecordInlineComment(0xD36D, "Snake segment exits level - grant points");
            RecordInlineComment(0xBCE1, "If branch taken: jumped on and crushed an enemy, 750 points");
            RecordInlineComment(0x97A9, "Snake has just been swallowed by a shark");
            RecordInlineComment(0xBD8E, "Snake has just been bumped with no tail left");
            RecordInlineComment(0xBFA4, "Snake has just smushed by a BigFoot, anvil, etc");

            RecordInlineComment(0xB637, "Snake has bumpted an open exit door");
            RecordInlineComment(0x8CA3, "Update BG palette for falling water");
            RecordInlineComment(0xBECD, "Load the next waypoint in the path");

            RecordInlineComment(0x91BC, "Check if warp in progress; if so, only black hole/hovering scores/doors/scales AI function");

            RecordInlineComment(0xC8FB, "If you get here, a different entity type was overridden due to a warp-in-progress");

            RecordInlineComment(0x9F74, "Last part of subroutine at 9FC6");
            RecordInlineComment(0xA149, "Subroutine is nearly complete; clean up and farjump to the last section");
            RecordInlineComment(0xA080, "Init loop counter to 5");
            RecordInlineComment(0xA094, "Loop body start");
            RecordInlineComment(0xA142, "Loop body end");
            
            RecordInlineComment(0xA4F0, "Render a horizontal row of background");

            RecordInlineComment(0x9F84, "This is always #$07...");
            RecordUnconditionalBranch(0x9F86);

            RecordInlineComment(0x8370, "Grant one tail segment at level start");
            RecordInlineComment(0x8FC5, "Grant one tail segment when respawning");
            RecordInlineComment(0x9D99, "Grant one tail segment for eating a Pibbley");

            RecordInlineComment(0x93B2, "NB: Y is zero or two here");
            RecordInlineComment(0xDFFD, "Entity hit water and is being removed");

            RecordInlineComment(0x97C3, "Spawn an ARG letter entity");
            RecordInlineComment(0x995D, "Spawn a shark entity");
            RecordInlineComment(0x9AC7, "Spawn a Pibbley chunk");
            RecordInlineComment(0xAE9A, "Spawn a flying bell/fin");
            RecordInlineComment(0xB3DD, "Spawn a floating score");
            RecordInlineComment(0xB86F, "Spawn a splash");
            RecordInlineComment(0xBACE, "Spawn an entity from $07");
            RecordInlineComment(0xC219, "Spawn a dispensing Pibbley");
            RecordInlineComment(0xC482, "Spawn a flipping lid?");
            RecordInlineComment(0xC51E, "Spawn lid contents?");
            RecordInlineComment(0xCBD3, "Spawn a pibblefish egg");
            RecordInlineComment(0xD1D5, "Spawn a tail segment");
            RecordInlineComment(0xBAAE, "Spawn three pin cushion pins");
            RecordInlineComment(0xBE02, "Spawn shrapnels");
            RecordInlineComment(0xAEAC, "Bell layout");
            RecordInlineComment(0xAEA6, "Fishtail layout");
            RecordUnconditionalBranch(0xBDF7);

            RecordInlineComment(0x9771, "Convert controller ordinals to screen ordinals");
            RecordInlineComment(0x8CE4, "Invert controller inputs");

            RecordUnconditionalBranch(0xC159);
            RecordUnconditionalBranch(0xC17A);
            RecordUnconditionalBranch(0xC18A);
            RecordUnconditionalBranch(0xC1A4);
            RecordUnconditionalBranch(0xC1B2);
            RecordUnconditionalBranch(0xC1B9);
            RecordUnconditionalBranch(0xC1C7);

            RecordInlineComment(0xD99E, "Opcode 00: end of track");
            RecordInlineComment(0xD9AB, "Opcode 02: SFX unlock repeat");
            RecordInlineComment(0xD9C9, "Opcode 03: APU update");
            RecordInlineComment(0xDA0B, "Opcode 04: start loop");
            RecordInlineComment(0xDA32, "Opcode 05: end of loop");
            RecordInlineComment(0xD9BE, "Opcode 06: set length override");
            RecordInlineComment(0xD9B5, "Opcode 07: clear length override");
            RecordInlineComment(0xDA5D, "Opcode 08: start vibrado");
            RecordInlineComment(0xDA8F, "Opcode 0B: pitch down slide");
            RecordInlineComment(0xDAAE, "Opcode 0C: decrescendo ignore");
            RecordInlineComment(0xDB0A, "Opcode 11: adjust tempo");
            RecordInlineComment(0xDB0F, "Opcode 12: absolute pitch shift");
            RecordInlineComment(0xDB1C, "Opcode 13: relative pitch shift");
            RecordInlineComment(0xDB22, "Opcode 14: cross-channel pitch shift");
            RecordInlineComment(0xDAC6, "Opcode 16: decrescendo start");
            RecordUnconditionalBranch(0xDAC4);
            RecordUnconditionalBranch(0xDA09);
            RecordUnconditionalBranch(0xDA8D);



            //{ 0x06C1, "Crescendo SFX setup (level x completed / game over)" },
            //{ 0x0776, "Play SFX" },
            //{ 0x0779, "Play SFX" },
            //{ 0x04DC, "Play SFX" },
            //{ 0x04DF, "Play SFX" },
            //{ 0x05E6, "Play SFX" },
            //{ 0x05E9, "Play SFX" },
            //{ 0x05EE, "Play SFX" }



            RecordSectionHeader(0x8000, "Audio track reads" );

            RecordSectionHeader(0x80B6, "String printing method" );
            RecordSectionHeader(0xC3D7, "CHR ROM blitter" );
            RecordSectionHeader(0xC350, "Nametable attribute RLE run fetcher" );
            RecordSectionHeader(0x80DB, "Nametable attribute RLE decoder/loader" );
            RecordSectionHeader(0x80F7, "Palette RAM initializer" );
            RecordSectionHeader(0x816C, "Sprite hiding methods" );
            RecordSectionHeader(0xD279, "Controller polling" );
            RecordSectionHeader(0x8128, "Change the current machine state" );
            RecordSectionHeader(0x8C69, "PLAY machine state" );
            RecordSectionHeader(0x8DC6, "DROPDOWN machine state" );
            RecordSectionHeader(0x84CC, "CRAWL machine state" );
            RecordSectionHeader(0xC154, "Powerup logic" );
            RecordSectionHeader(0xFCBA, "Code to populate OAM buffer with relative sprites (Pibbley being eaten or windup key on head)" );
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
            RecordSectionHeader(0x81C3, "Fade out routine: returns Z set if the fade out is complete" );
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
            RecordSectionHeader(0xBB85, "Method to interact with the snakes (be jumped on/licked or bump into)" );
            RecordSectionHeader(0xBE23, "Unknown subroutine" );
            RecordSectionHeader(0xBE83, "Unknown subroutine" );
            RecordSectionHeader(0xBF44, "Unknown subroutine" );
            RecordSectionHeader(0xE161, "Unknown subroutine" );
            RecordSectionHeader(0xE17C, "Method to check if a template object is on screen and if so, to build an entity for it" );
            RecordSectionHeader(0xE055, "Subroutine related to converting controller inputs into motions");
            RecordSectionHeader(0xE037, "Unknown subroutine" );
            RecordSectionHeader(0xD4D2, "Unknown subroutine (used by snake tail segments)" );
            RecordSectionHeader(0xDF1A, "Unknown subroutine" );
            RecordSectionHeader(0xD4E1, "Unknown subroutine (used by snake tail segments)" );
            RecordSectionHeader(0xD4F4, "Unknown subroutine" );
            RecordSectionHeader(0xD1D2, "Unknown subroutine (creates a tail segment?)" );
            RecordSectionHeader(0xCF40, "Unknown subroutine" );
            RecordSectionHeader(0xC64E, "Unknown subroutine" );
            RecordSectionHeader(0xB732, "Unknown subroutine" );
            RecordSectionHeader(0xC567, "Unknown subroutine");
            RecordSectionHeader(0x866D, "Load entity map tile coordinates into $77/Y (x coord) and $78/A (y coord)");
            RecordSectionHeader(0x9F72, "Load map data address from coordinates ($77, A)");
            RecordSectionHeader(0x8689, "Unknown subroutine" );
            RecordSectionHeader(0x9FB7, "Convert fixed point (high nybble in $04, low byte in A) to nearest whole number" );
            RecordSectionHeader(0x9FC6, "Unknown subroutine" );
            RecordSectionHeader(0x9F9E, "Unknown subroutine (component of subroutine at 9FC6)" );
            RecordSectionHeader(0xAE7F, "Unknown subroutine" );
            RecordSectionHeader(0xB231, "Unknown subroutine (component of Pibbley AI)" );
            RecordSectionHeader(0xB247, "Unknown subroutine" );
            RecordSectionHeader(0xB315, "Unknown subroutine" );
            RecordSectionHeader(0xB9A7, "Unknown subroutine" );
            RecordSectionHeader(0xBD77, "Damage snake method" );
            RecordSectionHeader(0xBE06, "Unknown subroutine" );
            RecordSectionHeader(0x845B, "ChrRomBlit 24" );
            RecordSectionHeader(0x8460, "Unknown subroutine" );
            RecordSectionHeader(0x8480, "Unknown subroutine" );
            RecordSectionHeader(0xC592, "Unknown subroutine" );
            RecordSectionHeader(0x8064, "Convert entity world coordinates to screen coordinates" +
                ":If called from $8064, Y should be zero or two" + 
                ":Screen X = World X + World Y" +
                ":Screen Y = (WorldX - World Y) / 2 - World Z" +
                ":Purpose of entity variables 9/A/C not known" +
                ":Value returned in $04 and $05" );
            RecordSectionHeader(0x8022, "Spaceship entity intelligence helper code" );
            RecordSectionHeader(0x899A, "Unknown subroutine (called when an entity is above water?)" );
            RecordSectionHeader(0x89DA, "Method for 'blending in' entity attributes" );
            RecordSectionHeader(0x89E3, "Unknown subroutine" );
            RecordSectionHeader(0x89FD, "Unknown subroutine" );
            RecordSectionHeader(0x8A1A, "Unknown subroutine" );
            RecordSectionHeader(0x8A4E, "Unknown subroutine" );
            RecordSectionHeader(0x90C6, "Unknown subroutine (produces sprites for drop down state?)" );
            RecordSectionHeader(0x9139, "Unknown subroutine" );
            RecordSectionHeader(0x9186, "Utility to disable strip rendering on next frame" );
            RecordSectionHeader(0x918B, "Unknown subroutine" );
            RecordSectionHeader(0x9292, "Camera to screen transform" );
            RecordSectionHeader(0x92A2, "Camera to screen transform" );
            RecordSectionHeader(0x92BB, "Dynamic thunk" );
            RecordSectionHeader(0x92BE, "Unknown subroutine" );
            RecordSectionHeader(0x93B2, "Unknown subroutine" );
            RecordSectionHeader(0x9432, "Unknown subroutine" );
            RecordSectionHeader(0x9584, "Unknown subroutine" );
            RecordSectionHeader(0x9D45, "Unknown subroutine" );
            RecordSectionHeader(0x9D54, "Unknown subroutine (something about a Pibbley being eaten)" );
            RecordSectionHeader(0x9DA3, "Unknown subroutine" );
            RecordSectionHeader(0x9DC5, "Unknown subroutine" );
            RecordSectionHeader(0x9E95, "Unknown subroutine" );
            RecordSectionHeader(0xD2C0, "Unknown subroutine");
            RecordSectionHeader(0xB3D7, "Methods to grant points" );
            RecordSectionHeader(0xB527, "Unknown subroutine" );
            RecordSectionHeader(0xB7CD, "Unknown subroutine" );
            RecordSectionHeader(0xB869, "Method to spawn a new splash entity" );
            RecordSectionHeader(0xC2AA, "Unknown subroutine" );
            RecordSectionHeader(0xC3A7, "Unknown subroutine" );
            RecordSectionHeader(0xC476, "Method to open a lid" );
            RecordSectionHeader(0xB79D, "Unknown subroutine");
            RecordSectionHeader(0xC552, "Copy entity coordinates int $86-$89" );
            RecordSectionHeader(0xC8AC, "Copy entity Z coordinates, but opposite direction to sCopyZCoords" );
            RecordSectionHeader(0xF51D, "Unknown subroutine" );
            RecordSectionHeader(0xFA31, "Unknown subroutine" );
            RecordSectionHeader(0xC2EF, "Unknown subroutine");
            RecordSectionHeader(0xC8B9, "Compare entity Z coordinates");
            RecordSectionHeader(0x8DAC, "Unknown subroutine");
            RecordSectionHeader(0x9422, "Unknown subroutine");
            RecordSectionHeader(0xD2C9, "Random number generating method");
            RecordSectionHeader(0xFCDE, "Lookup an index based on a snake's sprite arrangement and hflip flag" );
            RecordSectionHeader(0xFCF0, "Entity->OAM rendering method");
            RecordSectionHeader(0x802F, "Entity creation method" );
            RecordSectionHeader(0xB441, "Unknown subroutine (probably modifying player scores though)" );
            RecordSectionHeader(0xCCD7, "Lid-opening method" );
            RecordSectionHeader(0xB6DE, "Utility methods for loading a Y index" );
            RecordSectionHeader(0xCD4C, "Unknown subroutine, many callers" );
            RecordSectionHeader(0xE237, "End credits spaceship and snake frame rendering" );
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
            RecordSectionHeader(0xFFB5, "Method to set MMC1 control register" );
            RecordSectionHeader(0xFFC9, "Method to set MMC1 CHR bank 0 register" );
            RecordSectionHeader(0xFFDD, "Method to set MMC1 CHR bank 1 register" );
            RecordSectionHeader(0x9CDE, "Unknown subroutine" );
            RecordSectionHeader(0x9D14, "Method to clamp a fixed-point value (in Y/A) to the range 0-16" );
            RecordSectionHeader(0x9CF3, "Clip an entity's coordinates to [0-16) so it stays in a pond");
            RecordSectionHeader(0x96F3, "Mask controller push state" );
            RecordSectionHeader(0x8251, "RST handler routine part 2" );
            RecordSectionHeader(0x8242, "Reset PPU control/mask registers" );
            RecordSectionHeader(0x8209, "--------" );
            RecordSectionHeader(0x8295, "--------" );
            RecordSectionHeader(0x8C01, "TRANSITION machine state" );
            RecordSectionHeader(0x9610, "--------" );
            RecordSectionHeader(0xA4DB, "--------" );
            RecordSectionHeader(0xABE1, "Method to transfer a strip from the stack into the PPU" );
            RecordSectionHeader(0xB7FA, "--------" );
            RecordSectionHeader(0xBAC0, "--------" );
            RecordSectionHeader(0xC8C7, "--------" );
            RecordSectionHeader(0x968F, "More code for chunk, above");
            RecordSectionHeader(0x9634, "--------");
            RecordSectionHeader(0x964A, "--------");
            RecordSectionHeader(0xAE0F, "--------");
            RecordSectionHeader(0xD4B9, "--------");
            RecordSectionHeader(0x8666, "Fragment belonging to $8689 below" );
            RecordSectionHeader(0x849C, "'End credits' fade subtype" );
            RecordSectionHeader(0xC3BF, "'Level end' fade subtype" );
            RecordSectionHeader(0xC587, "'Warp/bonus' fade subtype" );
            RecordSectionHeader(0xC5B5, "'Entering pond' fade subtype" );
            RecordSectionHeader(0xC5F9, "'Return to play' fade subtype" );
            RecordSectionHeader(0x81EA, "SFX enqueueing methods" );
            RecordSectionHeader(0x9032, "Zero out transient player data for player X" );
            RecordSectionHeader(0xC3B7, "Blit in page $5A" );
            RecordSectionHeader(0xC3BB, "Blit in page $06" );
            RecordSectionHeader(0xD54A, "Music track opcode reading method");
            RecordSectionHeader(0xC62C, "Map size setting methods");
            RecordSectionHeader(0xAD8C, "Stack-to-PPU blitter");
            RecordVariable(0x02, "vFrameCounter");
            RecordVariable(0x12, "vRandomNumber");
            RecordVariable(0x16, "vCntrl_holdP");
            RecordVariable(0x17, "vCntrl_holdP2");
            RecordVariable(0x18, "vCntrl_pressP");
            RecordVariable(0x19, "vCntrl_pressP2");
            RecordVariable(0x74, "vStackPtrBackup");
            RecordVariable(0x8C, "vCrawlTallyMode");
            RecordVariable(0xA4, "vRendStripMode");
            RecordVariable(0xAA, "vCurrentLvl");
            RecordVariable(0xAB, "vCurrTemplate");
            RecordVariable(0xAC, "vNumTemplates");
            RecordVariable(0xAF, "vDropDownCounter");
            RecordVariable(0xB0, "vDropDownText");
            RecordVariable(0xB2, "vDropDownPlyr");
            RecordVariable(0xB5, "vActivePibbleyCnt");
            RecordVariable(0xB7, "v2PlyrModeInBit0");
            RecordVariable(0xB8, "vWarpClock");
            RecordVariable(0xBB, "vMapSize");
            RecordVariable(0xBC, "vMapMaxTile");
            RecordVariable(0xBD, "vPlayMode");
            RecordVariable(0xC5, "vCurrentPond");
            RecordVariable(0xCE, "vTimerTens");
            RecordVariable(0xCF, "vTimerOnes");
            RecordVariable(0xD0, "vTimerMs");
            RecordVariable(0xDA, "vStartPressed");
            RecordVariable(0xDC, "vTimerEntrance");
            RecordVariable(0xE0, "vDiving");
            RecordVariable(0xE2, "vSpriteChrRomPg");
            RecordVariable(0xFA, "vIsLevel91011");
            RecordVariable(0xFB, "vPly_continues");
            RecordVariable(0x03DF, "vPly_lives");
            RecordVariable(0x03E0, "vPly_lives2");
            RecordVariable(0xF9, "vLvl7ClockHack");
            RecordVariable(0xC2, "vRumbleTimer");
            RecordVariable(0xC3, "vRumbleYOffset");

            RecordVariable(0xCC, "vMinXScrollHi");
            RecordVariable(0xCD, "vMinXScrollLo");
            RecordVariable(0xCA, "vMaxXScrollHi");
            RecordVariable(0xCB, "vMaxXScrollLo");
            RecordVariable(0x6D, "vCurrXScrollHi");
            RecordVariable(0x6E, "vCurrXScrollLo");
            RecordVariable(0x6F, "vCurrYScrollHi");
            RecordVariable(0x70, "vCurrYScrollLo");

            RecordVariable(0x7C, "vXScrollDelta");
            RecordVariable(0x7E, "vYScrollDelta");

            RecordInlineComment(0xE17C, "Method called once per frame; $AB increments from #$00 by #$07 and loops upon reaching vNumTemplates");
            RecordInlineComment(0xE191, "One of the two branches is taken\r\n");
            RecordInlineComment(0xE19F, "One of the two branches is taken\r\n");
            RecordInlineComment(0xE1E9, "One of the two branches is taken\r\n");
            RecordInlineComment(0xE1BB, "Check if entity template is on the screen?");
            RecordInlineComment(0xE1C0, "Check if entity template is on the screen?");
            RecordInlineComment(0xE1CA, "An entity template has been brought onto the screen. Spawn an entity from it");
            RecordInlineComment(0xE1F4, "Done processing the template, increment counter for the next pass");

            RecordVariable(0x03E1, "vPly_score"); //3E1 - 3E6: p1 score, 3E7 - 3EC: p2 score
            RecordVariable(0x03FF, "vPly_lidAirTime" );
            RecordVariable(0x0401, "vPly_slideProof" );
            RecordVariable(0x0403, "vPly_owSpinTime" );
            //RecordVariable(0x0405, "Player something [0,1]" );
            RecordVariable(0x0407, "vPly_spdUpTime");
            RecordVariable(0x0409, "vPly_dethClock");
            RecordVariable(0x040A, "vPly_dethClock2");
            RecordVariable(0x040B, "vPly_invertTime");
            RecordVariable(0x040D, "vPly_invisTime");
            RecordVariable(0x040F, "vPly_argClock");
            RecordVariable(0x0410, "vPly_argClock2");
            //RecordVariable(0x0411, "Player something [0,1]" );
            //RecordVariable(0x0413, "Player something [0,1]" );
            //RecordVariable(0x0415, "Player something [0,1]" );
            //RecordVariable(0x0417, "Player something [0,1]" );
            //RecordVariable(0x0419, "Player something [0,1]" );
            //RecordVariable(0x041B, "Player something [0,1]" );
            //RecordVariable(0x041D, "Player something [0,1]" );
            RecordVariable(0x041F, "vPly_tngExtendedLen" );
            //RecordVariable(0x0421, "Player something [0,1]" );
            //RecordVariable(0x0423, "Player something [0,1]" );
            //RecordVariable(0x0425, "Player something [0,1]" );
            //RecordVariable(0x0427, "Player something [0,1]" );
            //RecordVariable(0x0429, "Player something [0,1]" );
            //RecordVariable(0x042B, "Player something [0,1]" );
            RecordVariable(0x0485, "vPly_tonguePibbley");
            RecordVariable(0x048D, "vPly_exitedTime");
            RecordVariable(0x048E, "vPly_exitedTimeP2");
            RecordVariable(0x0493, "vPly_lickedPibbley");
            RecordVariable(0x0495, "vPly_hasFTail");
            RecordVariable(0x0499, "vPly_tngLength");

            RecordVariable(0x03B7, "vEnt_screenX");
            RecordVariable(0x03CB, "vEnt_screenY");
            RecordVariable(0x049B, "vEnt_type");
            RecordVariable(0x049C, "vEnt_type2");
            RecordVariable(0x04AF, "vEnt_arrangement");
            RecordVariable(0x04C3, "vEnt_xHigh");
            RecordVariable(0x04D7, "vEnt_xLow");
            RecordVariable(0x04EB, "vEnt_yHigh");
            RecordVariable(0x04FF, "vEnt_yLow");
            RecordVariable(0x0513, "vEnt_zHigh");
            RecordVariable(0x0527, "vEnt_zLow");
            RecordVariable(0x053B, "vEnt_unkn_09");
            RecordVariable(0x054F, "vEnt_unkn_0A");
            RecordVariable(0x0563, "vEnt_unkn_0B");
            RecordVariable(0x0577, "vEnt_unkn_0C");
            RecordVariable(0x058B, "vEnt_unkn_0D");
            RecordVariable(0x059F, "vEnt_unkn_0E");
            RecordVariable(0x05B3, "vEnt_oamAttrs");
            RecordVariable(0x05C7, "vEnt_unkn_10");
            RecordVariable(0x05DB, "vEnt_unkn_11");
            RecordVariable(0x05EF, "vEnt_unkn_12");
            RecordVariable(0x0603, "vEnt_unkn_13");
            RecordVariable(0x0617, "vEnt_unkn_14");
            RecordVariable(0x062B, "vEnt_unkn_15");
            RecordVariable(0x063F, "vEnt_unkn_16");
            RecordVariable(0x0640, "vEnt_unkn_16_2");

            RecordVariable(0x0653, "vTmpl_type");
            RecordVariable(0x0654, "vTmpl_posHi");
            RecordVariable(0x0655, "vTmpl_xLo");
            RecordVariable(0x0656, "vTmpl_yLo");
            RecordVariable(0x0657, "vTmpl_zLo");
            RecordVariable(0x0658, "vTmpl_oamAttrs");
            RecordVariable(0x0659, "vTmpl_ent0D");

            RecordVariable(0x2000, "ppu_ctrl");
            RecordVariable(0x2001, "ppu_mask");
            RecordVariable(0x2002, "ppu_status");
            RecordVariable(0x2003, "oam_addr");
            RecordVariable(0x2004, "oam_data");
            RecordVariable(0x2005, "ppu_scroll");
            RecordVariable(0x2006, "ppu_addr");
            RecordVariable(0x2007, "ppu_data");
            RecordVariable(0x4014, "oam_dma");

            RecordVariable(0x0301, "vAudio_BG_ctrl");
            RecordVariable(0x0302, "vAudio_BG_cSteps");
            RecordVariable(0x0303, "vAudio_BG_addrLo");
            RecordVariable(0x0304, "vAudio_BG_addrHi");

            RecordVariable(0x0311, "vAudio_SFX_tune");
            RecordVariable(0x0312, "vAudio_SFX_ntCtrLen");
            RecordVariable(0x0313, "vAudio_SFX_addrLo");
            RecordVariable(0x0314, "vAudio_SFX_addrHi");

            RecordVariable(0x0321, "vAudio_BG_4000hi");
            RecordVariable(0x0322, "vAudio_BG_4001");
            RecordVariable(0x0323, "vAudio_BG_4002");
            RecordVariable(0x0324, "vAudio_BG_4003");

            RecordVariable(0x0331, "vAudio_SFX_4000");
            RecordVariable(0x0332, "vAudio_SFX_4001");
            RecordVariable(0x0333, "vAudio_SFX_4002");
            RecordVariable(0x0334, "vAudio_SFX_4003");

            RecordVariable(0x0341, "vAudio_BG_subLo");
            RecordVariable(0x0342, "vAudio_BG_subHi");
            RecordVariable(0x0343, "vAudio_BG_cSub");
            RecordVariable(0x0344, "vAudio_VB_fDel");

            RecordVariable(0x0351, "vAudio_BG_lnOvr");
            RecordVariable(0x0352, "vAudio_BG_4003lo");
            RecordVariable(0x0353, "vAudio_CR_curDly");
            RecordVariable(0x0354, "vAudio_BG_pShft");

            RecordVariable(0x0361, "vAudio_VB_nSteps");
            RecordVariable(0x0362, "vAudio_VB_cSteps");
            RecordVariable(0x0363, "vAudio_VB_nCalls");
            RecordVariable(0x0364, "vAudio_VB_cCalls");

            RecordVariable(0x0371, "vAudio_CR_vCpy");
            RecordVariable(0x0373, "vAudio_CR_curFDly");
            RecordVariable(0x0374, "vAndio_SL_01");

            RecordVariable(0x0381, "vAndio_SL_02");
            RecordVariable(0x0382, "vAndio_SL_03");
            RecordVariable(0x0383, "vAndio_SL_04");
            RecordVariable(0x0384, "vAndio_SL_05");

            RecordVariable(0x0391, "vAudio_CR_tVolume");
            RecordVariable(0x0392, "vAudio_CR_baseDly");
            RecordVariable(0x0393, "vAudio_CR_nCtrl");
            RecordVariable(0x0394, "vAudio_CR_baseFDly");

            RecordVariable(0x03A1, "vAndio_SL_06");
            RecordVariable(0x03A2, "vAudio_BG_tempo");
            RecordVariable(0x03A3, "vAudio_BG_nBytes");
            

            RecordVariable(0x1C, "vTempoForFrame");
            RecordVariable(0x1D, "vTempoForSong");

            RecordVariable(0xFF, "vMTCtrlrMask");

            RecordUnconditionalBranch(0xA0AD);
            RecordUnconditionalBranch(0x9361);
            RecordUnconditionalBranch(0x97AC);
            RecordUnconditionalBranch(0x8F0F);
            RecordInlineComment(0x9414, "Need to render a strip; Y=0 for vertical strip/hscroll, Y=2 for horizontal strip/vscroll");

            RecordInlineComment(0xA266, "Pull off previous tile-to-space cap");
            RecordInlineComment(0xA272, "Pull off previous tile-to-space cap");
            RecordInlineComment(0xA2D0, "Maybe replace zero tile with exended shadow");
            RecordInlineComment(0xA2D3, "Ocean extended shadow");
            RecordInlineComment(0xA2D8, "Wall lower edge");
            RecordInlineComment(0xA2F7, "Wall quad upper tile");
            RecordInlineComment(0xA2FE, "Wall quad lower tile");
            RecordInlineComment(0xA305, "Wall topper lower tile");
            RecordInlineComment(0xA30A, "Wall topper upper tile");
            RecordInlineComment(0xA30E, "Zero-delta connector");
            RecordInlineComment(0xA323, "Tile-to-space cap");
            RecordInlineComment(0xA361, "Off the map: fill remaining tiles with zeroes");
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

        public bool IsUnconditionalBranch(int address)
        {
            return unconditionalBranches.Contains(address);
        }

        public void RecordUnconditionalBranch(int address)
        {
            unconditionalBranches.Add(address);
        }

        public bool HasInlineComment(int address)
        {
            return inlineComments.ContainsKey(address) || unconditionalBranches.Contains(address);
        }

        public string GetInlineComment(int address)
        {
            if (inlineComments.ContainsKey(address))
                return inlineComments[address];
            else if (unconditionalBranches.Contains(address))
                return "Unconditional branch";
            else
                throw new InvalidOperationException();
        }

        public void RecordSectionHeader(int address, string text)
        {
            if (sectionHeaders.ContainsKey(address))
                throw new InvalidOperationException(string.Format(
                    "A section header is already recorded for {0:X4}" , address));

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

        public void RecordVariable(int address, string name)
        {
            variables.Add(address, name);
        }

        public bool HasVariable(int address)
        {
            return variables.ContainsKey(address);
        }

        public string GetVariable(int address)
        {
            return variables[address];
        }
    }
}
