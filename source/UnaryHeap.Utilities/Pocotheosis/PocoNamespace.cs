using System;
using System.Collections.Generic;
using System.IO;

namespace Pocotheosis
{
    public class PocoNamespace
    {
        string name;
        List<PocoClass> classes;
        List<PocoEnum> enums;

        public PocoNamespace(string name, IEnumerable<PocoEnum> enums,
            IEnumerable<PocoClass> classes)
        {
            this.name = name;
            this.classes = new List<PocoClass>(classes);
            this.enums = new List<PocoEnum>(enums);
        }

        public IEnumerable<PocoClass> Classes
        {
            get { return classes; }
        }

        public IEnumerable<PocoEnum> Enums
        {
            get { return enums; }
        }

        public void WriteNamespaceHeader(TextWriter output)
        {
            output.WriteLine("namespace " + name);
            output.WriteLine("{");
        }

        public void WriteNamespaceFooter(TextWriter output)
        {
            output.WriteLine("}");
        }
    }
}
