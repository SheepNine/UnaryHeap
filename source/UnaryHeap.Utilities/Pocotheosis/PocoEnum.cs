using System.Collections.Generic;

namespace Pocotheosis
{
    public class PocoEnum
    {
        public string Name { get; private set; }
        List<PocoEnumerator> enumerators;
        public IEnumerable<PocoEnumerator> Enumerators { get { return enumerators; } }

        internal PocoEnum(string name, IEnumerable<PocoEnumerator> enumerators)
        {
            Name = name;
            this.enumerators = new List<PocoEnumerator>(enumerators);
        }
    }

    public class PocoEnumerator
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
