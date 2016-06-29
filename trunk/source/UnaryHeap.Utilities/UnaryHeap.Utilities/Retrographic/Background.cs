#if INCLUDE_WORK_IN_PROGRESS_RETROGRAPHIC

using System;
using System.IO;
using System.Linq;

namespace UnaryHeap.Utilities.Retrographic
{
    class Background
    {
        const int MappingsPerBackground = 1024;

        private Mapping[] mappings;

        public Background(Mapping[] mappings)
        {
            if (mappings == null)
                throw new ArgumentNullException("mappings");
            if (mappings.Length != MappingsPerBackground)
                throw new ArgumentException();
            if (mappings.Any(color => color == null))
                throw new ArgumentNullException();

            this.mappings = mappings;
        }

        public Mapping this[int index]
        {
            get
            {
                if (index < 0 || index >= MappingsPerBackground)
                    throw new ArgumentOutOfRangeException("index");
                return mappings[index];
            }
        }

        public void Serialize(Stream output)
        {
            foreach (var i in Enumerable.Range(0, MappingsPerBackground))
                mappings[i].Serialize(output);
        }

        public static Background Deserialize(Stream input)
        {
            var colors = new Mapping[MappingsPerBackground];

            foreach (var i in Enumerable.Range(0, MappingsPerBackground))
                colors[i] = Mapping.Deserialize(input);

            return new Background(colors);
        }
    }
}

#endif