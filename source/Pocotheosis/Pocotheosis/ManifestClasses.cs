using System;
using System.Collections.Generic;

namespace Pocotheosis
{
    public class PocoNamespace
    {
        public string Name { get; private set; }

        readonly List<PocoClass> classes;
        public IEnumerable<PocoClass> Classes { get { return classes; } }

        readonly List<PocoEnumDefinition> enums;
        public IEnumerable<PocoEnumDefinition> Enums { get { return enums; } }

        public DateTime LastWriteTimeUtc { get; private set; }

        public PocoNamespace(string name, DateTime lastWriteTimeUtc,
            IEnumerable<PocoEnumDefinition> enums, IEnumerable<PocoClass> classes)
        {
            Name = name;
            LastWriteTimeUtc = lastWriteTimeUtc;
            this.classes = new List<PocoClass>(classes);
            this.enums = new List<PocoEnumDefinition>(enums);
        }
    }

    public class PocoClass
    {
        public string Name { get; private set; }

        public int StreamingId { get; private set; }

        readonly List<IPocoMember> members;
        public IEnumerable<IPocoMember> Members { get { return members; } }

        internal PocoClass(string name, int id, IEnumerable<IPocoMember> members)
        {
            Name = name;
            StreamingId = id;
            this.members = new List<IPocoMember>(members);
        }
    }

    public class PocoEnumDefinition
    {
        public string Name { get; private set; }

        public bool IsBitField { get; private set; }

        readonly List<PocoEnumerator> enumerators;
        public IReadOnlyList<PocoEnumerator> Enumerators { get { return enumerators; } }

        internal PocoEnumDefinition(string name, bool isBitField,
            IEnumerable<PocoEnumerator> enumerators)
        {
            Name = name;
            IsBitField = isBitField;
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
