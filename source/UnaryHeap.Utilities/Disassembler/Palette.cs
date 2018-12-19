﻿using System.Drawing;
using System.Linq;

namespace Disassembler
{
    static class Palette
    {
        private static Color[] colors = new[]
        {
            Color.FromArgb( 84,  84,  84),
            Color.FromArgb(  0,  30, 116),
            Color.FromArgb(  8,  16, 144),
            Color.FromArgb( 48,   0, 136),
            Color.FromArgb( 68,   0, 100),
            Color.FromArgb( 92,   0,  48),
            Color.FromArgb( 84,   4,   0),
            Color.FromArgb( 60,  24,   0),
            Color.FromArgb( 32,  42,   0),
            Color.FromArgb(  8,  58,   0),
            Color.FromArgb(  0,  64,   0),
            Color.FromArgb(  0,  60,   0),
            Color.FromArgb(  0,  50,  60),
            Color.FromArgb(  0,   0,   0),
            Color.FromArgb(  0,   0,   0),
            Color.FromArgb(  0,   0,   0),

            Color.FromArgb(152, 150, 152),
            Color.FromArgb(  8,  76, 196),
            Color.FromArgb( 48,  50, 236),
            Color.FromArgb( 92,  30, 228),
            Color.FromArgb(136,  20, 176),
            Color.FromArgb(160,  20, 100),
            Color.FromArgb(152,  34,  32),
            Color.FromArgb(120,  60,   0),
            Color.FromArgb( 84,  90,   0),
            Color.FromArgb( 40, 114,   0),
            Color.FromArgb(  8, 124,   0),
            Color.FromArgb(  0, 118,  40),
            Color.FromArgb(  0, 102, 120),
            Color.FromArgb(  0,   0,   0),
            Color.FromArgb(  0,   0,   0),
            Color.FromArgb(  0,   0,   0),

            Color.FromArgb(236, 238, 236),
            Color.FromArgb( 76, 154, 236),
            Color.FromArgb(120, 124, 236),
            Color.FromArgb(176,  98, 236),
            Color.FromArgb(228,  84, 236),
            Color.FromArgb(236,  88, 180),
            Color.FromArgb(236, 106, 100),
            Color.FromArgb(212, 136,  32),
            Color.FromArgb(160, 170,   0),
            Color.FromArgb(116, 196,   0),
            Color.FromArgb( 76, 208,  32),
            Color.FromArgb( 56, 204, 108),
            Color.FromArgb( 56, 180, 204),
            Color.FromArgb( 60,  60,  60),
            Color.FromArgb(  0,   0,   0),
            Color.FromArgb(  0,   0,   0),

            Color.FromArgb(236, 238, 236),
            Color.FromArgb(168, 204, 236),
            Color.FromArgb(188, 188, 236),
            Color.FromArgb(212, 178, 236),
            Color.FromArgb(236, 174, 236),
            Color.FromArgb(236, 174, 212),
            Color.FromArgb(236, 180, 176),
            Color.FromArgb(228, 196, 144),
            Color.FromArgb(204, 210, 120),
            Color.FromArgb(180, 222, 120),
            Color.FromArgb(168, 226, 144),
            Color.FromArgb(152, 226, 180),
            Color.FromArgb(160, 214, 228),
            Color.FromArgb(160, 162, 160),
            Color.FromArgb(  0,   0,   0),
            Color.FromArgb(  0,   0,   0),
        };

        public static Color ColorForIndex(int index)
        {
            return colors[index];
        }

        public static Color[] ColorsForIndices(byte[] indices)
        {
            var result = new Color[indices.Length];
            foreach (var i in Enumerable.Range(0, indices.Length))
                result[i] = ColorForIndex(indices[i]);
            return result;
        }
    }
}
