using System.Collections.Generic;

namespace Disassembler
{
    class Comments
    {
        private Dictionary<int, string> comments = new Dictionary<int, string>()
        {
            { 0x96F8, "Rattle/Roll AI" },
            { 0xD302, "Snake tail segment AI" },
            { 0xAFCA, "Spawning pibbly/pibblejogger AI" },
            { 0xAFEA, "Pibbly/pibblebat/pibbleboing/pibblecopter AI" },
            { 0xB4A6, "Dispensing pibbly AI" },
            { 0xAF3D, "Flipping lid AI" },
            { 0xB272, "Pibblesplat AI" },
            { 0xB5C0, "Door AI" },
            { 0xB65A, "Scale AI" },
            { 0xB6E5, "Snakedozer AI" },
            { 0xB759, "Bladez AI" },
            { 0xB8CE, "Flag AI" },
            { 0xB907, "Shark AI" },
            { 0xB959, "ARG letters AI" },
            { 0xB9D9, "Crazy seat/bubble AI" },
            { 0xBA7F, "Pin cushion AI" },
            { 0xBE37, "Shrapnel AI" },
            { 0xC78B, "Dispensed bell AI" },
            { 0xBE8E, "BigFoot AI" },
            { 0xC048, "Tail segment floating away AI" },
            { 0xC0B5, "Clock/Diamond/1-UP/???/Corkscrew/Fake 1-UP item AI" },
            { 0xC1F9, "Pibbly dispenser AI" },
            { 0xB986, "Crazy seat (from lid) AI" },
            { 0xC858, "Black hole (warp/bonus) AI" },
            { 0xC9B5, "Magic carpet AI" },
            { 0xC1D7, "Bonus stage context AI" },
            { 0xCAEB, "Bigfoot AI" },
            { 0xCB2A, "Hovering score AI" },
            { 0xCB48, "Pibbly chunk (heavy) AI" },
            { 0xCB87, "Pibbly chunk (light) AI" },
            { 0xB9B3, "Rotating crazy seat AI" },
            { 0xAE8C, "Bell & tail dispenser AI" },
            { 0xCBA6, "Breath bubble AI" },
            { 0xCBBD, "Seaweed AI" },
            { 0xCC4B, "Pibblefish AI" },
            { 0xC634, "Powerup AI" },
            { 0xC68A, "Record/mushroom/ice cube, metal tree, bell AI" },
            { 0xC7C7, "Anvil AI" },
            { 0x9EA1, "Water jet AI" },
            { 0xB9D5, "Stationary metal tree (?) AI" },
            { 0x9F1D, "Metal sphere/snowball/asteroid AI" },
            { 0xCC2C, "Pibblyfish egg hatching AI" },
            //{ 0x0700, "Spaceship 1 AI" },
            //{ 0x077C, "Spaceship 2 AI" },
            { 0x9E0B, "Warp rocket  AI" },

            { 0xC3D7, "CHR ROM blitter" },
            { 0xC350, "Nametable attribute RLE run fetcher" },
            { 0x80DB, "Nametable attribute RLE decoder/loader" },
            { 0x80F7, "Palette RAM initializer" },
            { 0x816C, "Sprite hiding methods" },
            { 0xD279, "Controller polling" },
            { 0x8128, "Change the current machine state" },
            { 0x8C0F, "FADE machine state" },
            { 0x8C69, "PLAY machine state" },
            { 0x8DC6, "DROP DOWN machine state" },
            { 0x84CC, "MAIN TITLES machine state" },
            { 0xC154, "Powerup logic" },
            { 0xFCBA, "Code to populate OAM buffer with relative sprites (pibbly being eaten or windup key on head)" },
            { 0xD245, "Kill both players (time out/all gone)" },
            { 0xD26A, "Select SFX $34: Crescendo variant" },
            { 0xFCA6, "A subroutine assisting with tongue drawing somehow" },
            { 0x8B07, "Reset timer to 99 seconds" },
            { 0x8150, "Unknown subroutine" },
            { 0x817A, "Unknown subroutine" },
            { 0x8197, "Unknown subroutine" },
            { 0x81A4, "Unknown subroutine" },
            { 0x81C3, "Unknown subroutine" },
            { 0x81DB, "Unknown subroutine" },
            { 0x8231, "Unknown subroutine" },
            { 0x95DE, "Unknown subroutine" },
            { 0x95EB, "Unknown subroutine" },
            { 0x95FE, "Unknown subroutine" },
            { 0x86B6, "Unknown subroutine" },
            { 0x86A0, "Unknown subroutine" },
            { 0x86AD, "Unknown subroutine" },
            { 0x8BCE, "Unknown subroutine" },
            { 0x86AA, "Thunk" },


            { 0x9CDE, "Unknown subroutine" },
            { 0x9D14, "Unknown subroutine" },
            { 0x9CF3, "Unknown subroutine" },


            { 0x96F3, "Mask controller push state" },
            { 0x8251, "RST handler routine part 2" },
            { 0x8242, "Reset PPU control/mask registers" },
            { 0x8209, "--------" },
            { 0x8295, "--------" },
            { 0x81EA, "SFX enqueueing methods" },
            { 0x9032, "Zero out transient player data for player X" }
        };

        public bool HasComment (int address)
        {
            return comments.ContainsKey(address);
        }

        public string GetComment(int address)
        {
            return comments[address];
        }
    }

    class InlineComments
    {
        private Dictionary<int, string> comments = new Dictionary<int, string>()
        {
            { 0xFA55, "Check if there is bonus time to be added" },
            { 0xFA81, "Tick a second off of the timer" },
            { 0xC1A0, "Grant 30 seconds of bonus time" },
            { 0x82A6, "Set starting player lives to 2" },
            { 0x82AD, "Check if two-player mode was chosen; null out their values if so" },
            { 0x82BB, "Set starting level to 0 (-1, since it gets incremented below)" },
            { 0x8EAF, "Reset player lives to 2 (after player continues)" },
            { 0x8442, "Init CHR ROM sprite page by level" },
            { 0xFD1C, "Draw a wind-up key in the snake's head" },
            { 0x828B, "Transfer 'SNAKE' arrangement to PPU" },
            { 0x865B, "Transfer 'Rattle' arrangement to PPU" },
            { 0x8660, "Transfer 'Roll' arrangement to PPU" },
            { 0xFA99, "Read status bar OAM template into OAM staging RAM" },
            { 0xFAA4, "Set tile of sprite for timer low digit" },
            { 0xFAAB, "Set tile of sprite for timer high digit" },
            { 0xFAB2, "Check if timer high digit is zero (less than ten seconds left)" },
            { 0xFA51, "Check whether in normal play mode; skip timer if not" },
            { 0xFA4D, "Skip skipping play mode check if in a pond (allow timer to tick)" },
            { 0xFA59, "Only worry about bonus time every fourth frame" },
            { 0xFA5F, "Do the grant of a second of bonus time" },
            { 0xFA75, "Don't decrement timer if a black hole is open" },
            { 0xFA8D, "Timer is at zero and cannot be decremented" },
            { 0xFAC9, "Set player one life count (or hide sprite if game over)" },
            { 0xFAD7, "Set player two life count (or hide sprite if game over)" },
            { 0xFAE5, "--- next chunk of the method ---" },
            { 0xFBB7, "--- another chunk of the method: $14 loops above completed (for entities?) ---" },
            { 0x82F8, "Delete all entities except 0 and 1 (the snakes)" },
            { 0xCC46, "Transmute into a pibblyfish and tail-call into that AI" },
            { 0x8E63, "Read indices of sprites to hide" },
            { 0x9756, "Mask out left button" },
            { 0x9768, "Mask out right button" },
            { 0x91D4, "Read AI code low byte" },
            { 0x91D9, "Read AI code high byte" },

            { 0x86A4, "Play SFX 'diving splash' rate-limited" },
            { 0x8EA6, "Play SFX 'PLAY ON/1-UP'" },
            { 0x8EA9, "Play SFX 'PLAY ON/1-UP'" },
            { 0x99B8, "Play SFX 'Jaws (slow)' or 'Jaws (fast)'" },
            { 0x9A95, "Play SFX 'Pibbly chomp'" },
            { 0x9AB9, "Play SFX 'Pibbly chunk spit'" },
            { 0x9D4E, "Play SFX 'ARRRGGG'" },
            { 0x9D51, "Play SFX 'ARRRGGG'" },
            { 0x9E14, "Play SFX 'Rocket take-off' rate-limited" },
            { 0x9F05, "Play SFX 'Water jump jet' rate-limited" },
            { 0xB398, "Play SFX 'Snake gulp'" },
            { 0xB4C8, "Play SFX 'Pibbly ejection' rate-limited" },
            { 0xB6B4, "Play SFX 'Scale bell ring' rate-limited" },
            { 0xB6B9, "Play SFX '???'" },
            { 0xBB79, "Play SFX 'Explosion'" },
            { 0xBC01, "Play SFX 'Lick foot' rate-limited" },
            { 0xBC94, "Play SFX 'Exploding enemy 1'" },
            { 0xBC99, "Play SFX 'Exploding enemy 2" },
            { 0xBCEA, "Play SFX 'Bounce/lick enemy'" },
            { 0xBD61, "Play SFX 'Snake OW'" },
            { 0xBD93, "Play SFX 'Snake death spin'" },
            { 0xBDFB, "Play SFX 'Explosion'" },
            { 0xBF40, "Play SFX 'THUD' rate-limited" },
            { 0xC129, "Play SFX for current powerup (see $C1CF)" },
            { 0xC132, "Play SFX for current powerup (see $C1CF)" },
            { 0xC2DB, "Play SFX 'Lid opening''" },
            { 0xC86B, "Play SFX 'Wormhole opening'" },
            { 0xC86E, "Play SFX 'Wormhole opening'" },
            { 0xC984, "Play SFX 'Wormhole sucking up object' rate-limited" },
            { 0xD372, "Play SFX 'Exit door point score' rate-limited" },
            { 0xD375, "Play SFX 'Exit door point score' rate-limited" },
            { 0xD6D2, "Play SFX 'Crescendo'" },
            { 0xD6D5, "Play SFX 'Crescendo'" },
            { 0xFABC, "Play SFX 'Time running out beep'" },

            { 0x8D77, "Configure 'HOLD' drop-down" },
            { 0x8F0D, "Configure 'PLAY ON' drop-down" },
            { 0x8F15, "Configure ??? drop-down" },
            { 0x8F57, "Configure ??? drop-down" },
            { 0xBE1B, "Configure 'ALL GONE' drop-down, or maybe others" },


            { 0xB3C3, "Crescendo SFX setup (5/5 pibbly eaten on bonus)" },
            { 0xC17E, "Crescendo SFX setup (extra continue picked up)" },
            { 0xCD29, "Crescendo SFX setup (warp lid opened)" },
            { 0xC848, "Crescendo SFX setup (anvil stomps scale)" },
            { 0xD49A, "Crescendo SFX setup (pond door opens)" },
            //{ 0x06C1, "Crescendo SFX setup (level x completed / game over)" },

            { 0xC18C, "Clock picked up" },
            { 0xC177, "Fish tail picked up" },
            { 0xC17C, "Extra continue picked up" },
            { 0xC15B, "Windup key picked up" },
            { 0xC1A6, "1-UP picked up" },
            { 0xC1B4, "Diamond picked up" },
            { 0xC154, "Inverter picked up" },
            { 0xC1BB, "Tongue extender picked up" },

            { 0x8462, "Play 'main titles' track" },
            { 0x8D9F, "Play 'game over' track" },
            { 0xFA93, "Play 'time out' track" },


            { 0x873D, "Read 'entity control bits?'" },
            { 0xBCB8, "Read 'entity control bits?'" },
            { 0xC8FF, "Read 'entity control bits?'" },
            { 0xDF5A, "Read 'entity control bits?'" },
            { 0xDFF2, "Read 'entity control bits?'" },
            { 0xFAFC, "Read 'entity control bits?'" },

            { 0xFE76, "Read 'arrangement address high byte" },
            { 0xFE7B, "Read 'arrangement address low byte" },


            //{ 0x0776, "Play SFX" },
            //{ 0x0779, "Play SFX" },
            //{ 0x04DC, "Play SFX" },
            //{ 0x04DF, "Play SFX" },
            //{ 0x05E6, "Play SFX" },
            //{ 0x05E9, "Play SFX" },
            //{ 0x05EE, "Play SFX" }
        };

        public bool HasComment(int address)
        {
            return comments.ContainsKey(address);
        }

        public string GetComment(int address)
        {
            return comments[address];
        }
    }
}
