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
            { 0x16, "Player buttons pushed [0,1]" },
            { 0x18, "Player buttons held [0,1]" },
            { 0xAA, "The current level" },
            { 0xB7, "One or two players (flag in bit zero; other bits ignored" },
            { 0xBD, "The current mode (0: normal stage, 1: bonus stage, 2: pond stage, -1: returning to normal stage)" },
            { 0xDF, "Somehow controls crescendo sound effect duration remaining" },
            { 0xCE, "Timer tens digit" },
            { 0xCF, "Timer ones digit" },
            { 0xD0, "Timer frames until next tick down" },
            { 0xF7, "Bonus time granted by a clock, yet to be accumulated into timer" },
            { 0xFB, "Player continues remaining [0,1]" },
            // 0x100 - 0x1FF is normally the stack, but sometimes the stack pointer is decremented
            // a bit and part of the stack is used as additional RAM
            { 0x03DF, "Player lives renamining [0,1]" }
        };
    }
}
