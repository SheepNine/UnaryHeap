#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.IO;
using System.Linq;

namespace UnaryHeap.Utilities.Retrographic
{
    class Palette
    {
        const int ColorsPerPalette = 16;

        private Color[] colors;

        public Palette(Color[] colors)
        {
            if (colors == null)
                throw new ArgumentNullException("colors");
            if (colors.Length != ColorsPerPalette)
                throw new ArgumentException();
            if (colors.Any(color => color == null))
                throw new ArgumentNullException();

            this.colors = colors;
        }

        public Color this[int index]
        {
            get
            {
                if (index < 0 || index >= ColorsPerPalette)
                    throw new ArgumentOutOfRangeException("index");
                return colors[index];
            }
        }

        public void Serialize(Stream output)
        {
            foreach (var i in Enumerable.Range(0, ColorsPerPalette))
                colors[i].Serialize(output);
        }

        public static Palette Deserialize(Stream input)
        {
            var colors = new Color[ColorsPerPalette];

            foreach (var i in Enumerable.Range(0, ColorsPerPalette))
                colors[i] = Color.Deserialize(input);

            return new Palette(colors);
        }
    }
}

#endif