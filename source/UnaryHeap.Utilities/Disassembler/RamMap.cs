using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disassembler
{
    class RamMap
    {
        private static Dictionary<int, string> ramUsage = new Dictionary<int, string>()
        {
            { 0x02, "A counter that increments each frame" },
            { 0x16, "Player buttons pushed [0,1]" },
            { 0x18, "Player buttons held [0,1]" },
            { 0xAA, "The current level" },
            { 0xB7, "One or two players (flag in bit zero; other bits ignored" },
            { 0xB8, "Counter while black hole is open (counts from $FF down to $60)" },
            { 0xBD, "The current mode (0: normal stage, 1: bonus stage, 2: pond stage, -1: returning to normal stage)" },
            { 0xDF, "Somehow controls crescendo sound effect duration remaining" },
            { 0xC5, "The currently occupied pond (1-5) or no pond (0)" },
            { 0xCE, "Timer tens digit" },
            { 0xCF, "Timer ones digit" },
            { 0xD0, "Timer frames until next tick down" },
            { 0xE2, "CHR ROM page to be used for sprites" },
            { 0xF7, "Bonus time granted by a clock, yet to be accumulated into timer" },
            { 0xFB, "Player continues remaining [0,1]" },
            // 0x100 - 0x1FF is normally the stack, but sometimes the stack pointer is decremented
            // a bit and part of the stack is used as additional RAM
            { 0x03DF, "Player lives renamining [0,1]" },

            // Transient player data
            { 0x03FF, "Player something [0,1]" },
            { 0x0401, "Player something [0,1]" },
            { 0x0403, "Player something [0,1]" },
            { 0x0405, "Player something [0,1]" },
            { 0x0407, "Player speedup time [0,1]" },
            { 0x0409, "Player something [0,1]" },
            { 0x040B, "Player inverted time [0,1]" },
            { 0x040D, "Player invincibility time [0,1]" },
            { 0x040F, "Player something [0,1]" },
            { 0x0411, "Player something [0,1]" },
            { 0x0413, "Player something [0,1]" },
            { 0x0415, "Player something [0,1]" },
            { 0x0417, "Player something [0,1]" },
            { 0x0419, "Player something [0,1]" },
            { 0x041B, "Player something [0,1]" },
            { 0x041D, "Player something [0,1]" },
            { 0x041F, "Player something [0,1]" },
            { 0x0421, "Player something [0,1]" },
            { 0x0423, "Player something [0,1]" },
            { 0x0425, "Player something [0,1]" },
            { 0x0427, "Player something [0,1]" },
            { 0x0429, "Player something [0,1]" },
            { 0x042B, "Player something [0,1]" },

            { 0x0485, "Player pibbly on tongue (84/85/86: red/blue/gold pibbly, C4/C5/C6 about to be swallowed, 01-38 chewing and spitting)" },
            { 0x0499, "Player tongue length [0,1] (0, 2, 4 or 6)" }
        };
    }
}
