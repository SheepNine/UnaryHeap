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
}
