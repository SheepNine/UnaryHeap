﻿using System.Collections.Generic;

namespace Pocotheosis
{
    public class PocoNamespace
    {
        public string Name { get; private set; }
        List<PocoClass> classes;
        public IEnumerable<PocoClass> Classes { get { return classes; } }
        List<PocoEnumDefinition> enums;
        public IEnumerable<PocoEnumDefinition> Enums { get { return enums; } }

        public PocoNamespace(string name, IEnumerable<PocoEnumDefinition> enums,
            IEnumerable<PocoClass> classes)
        {
            Name = name;
            this.classes = new List<PocoClass>(classes);
            this.enums = new List<PocoEnumDefinition>(enums);
        }
    }

    public class PocoClass
    {
        public string Name { get; private set; }
        public int StreamingId { get; private set; }
        List<IPocoMember> members;
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
        List<PocoEnumerator> enumerators;
        public IEnumerable<PocoEnumerator> Enumerators { get { return enumerators; } }

        internal PocoEnumDefinition(string name, IEnumerable<PocoEnumerator> enumerators)
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
