using System.Collections.Generic;
using System.IO;

namespace Pocotheosis
{
    class PocoClass
    {
        string name;
        List<PocoMember> members;

        public PocoClass(string name, IEnumerable<PocoMember> members)
        {
            this.name = name;
            this.members = new List<PocoMember>(members);
        }

        public void WriteClassDeclaration(TextWriter output)
        {
            output.WriteLine("\tpublic partial class " + name);
            output.WriteLine("\t{");
            WriteMemberDeclarations(output);
            output.WriteLine();
            WriteConstructor(output);
            output.WriteLine("\t}");
        }

        private void WriteMemberDeclarations(TextWriter output)
        {
            foreach (var member in members)
            {
                output.Write("\t\t");
                member.WriteDeclaration(output);
                output.WriteLine();
            }
        }

        private void WriteConstructor(TextWriter output)
        {
            output.Write("\t\tpublic " + name + "(");
            bool first = true;
            foreach (var member in members)
            {
                if (!first)
                {
                    output.Write(", ");
                }
                first = false;

                member.WriteFormalParameter(output);
            }
            output.WriteLine(")");
            output.WriteLine("\t\t{");
            foreach (var member in members)
            {
                output.Write("\t\t\t");
                member.WriteAssignment(output);
                output.WriteLine();
            }
            output.WriteLine("\t\t}");
        }
    }
}
