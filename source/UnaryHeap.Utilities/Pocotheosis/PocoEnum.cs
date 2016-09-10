using System;
using System.Collections.Generic;
using System.IO;

namespace Pocotheosis
{
    class PocoEnum
    {
        public string Name { get; private set; }
        List<PocoEnumerator> enumerators;

        public PocoEnum(string name, IEnumerable<PocoEnumerator> enumerators)
        {
            Name = name;
            this.enumerators = new List<PocoEnumerator>(enumerators);
        }

        public void WriteEnumDeclaration(StreamWriter file)
        {
            file.WriteLine("\tpublic enum " + Name);
            file.WriteLine("\t{");
            foreach (var enumerator in enumerators)
                file.WriteLine(string.Format("\t\t{0} = {1},",
                    enumerator.Name, enumerator.Value));
            file.WriteLine("\t}");
        }
    }

    class PocoEnumerator
    {
        public string Name { get; private set; }
        public int Value { get; private set; }

        public PocoEnumerator(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}
