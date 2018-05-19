using System.Collections.Generic;

namespace Disassembler
{
    class Comments
    {
        private Dictionary<int, string> comments = new Dictionary<int, string>()
        {
            { 0xB5C0, "Door AI" },
            { 0xB65A, "Scale AI" },
            { 0xB6E5, "Snakedozer AI" },
            { 0xB759, "Bladez AI" },
            { 0xB8CE, "Flag AI" },
            { 0xB9D9, "Crazy seat / bubble AI" },
            { 0xBA7F, "Pin cushion AI" },
            { 0xC1F9, "Pibbly dispenser AI" },
            { 0xC9B5, "Magic carpet AI" },
            { 0xC1D7, "Jaws AI (?)" },
            { 0xCAEB, "Bigfoot AI" },
            { 0xB9B3, "Rotating crazy seat AI" },
            { 0xAE8C, "Bell & tail dispenser AI" },
            { 0xCBBD, "Seaweed AI" },
            { 0xC634, "Powerup AI" },
            { 0xC68A, "Record / mushroom / ice cube, metal tree, bell AIs" },
            { 0xC7C7, "Anvil AI" },
            { 0x9EA1, "Water jet AI" },
            { 0xB9D5, "Stationary metal tree AI (?)" },
            { 0x9F1D, "Metal sphere / snowball / asteriod AI" },
            { 0x9E0B, "Warp rocket AI" },
            { 0xC3D7, "CHR ROM blitter" },
            { 0xAFEA, "Pibbly bat/plain/boing/copter AI (0C,31,32,33)" },
            { 0xAFCA, "Spawning pibbly AI and pibblyjogger (06)" },
            { 0xC350, "Nametable attribute RLE run fetcher" },
            { 0x80DB, "Nametable attribute RLE decoder/loader" },
            { 0x80F7, "Palette RAM initializer" },
            { 0xBE8E, "BigFoot AI (type $1A)" }
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
