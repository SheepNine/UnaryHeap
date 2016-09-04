using System;
using System.Collections.Generic;
using System.IO;

namespace Pocotheosis
{
    class PocoNamespace
    {
        string name;
        List<PocoClass> classes;

        public PocoNamespace(string name, IEnumerable<PocoClass> classes)
        {
            this.name = name;
            this.classes = new List<PocoClass>(classes);
        }

        public IEnumerable<PocoClass> Classes
        {
            get { return classes; }
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
