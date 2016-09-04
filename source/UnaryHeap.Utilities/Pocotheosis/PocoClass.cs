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
            var first = true;
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

        public void WriteClassEqualityDeclaration(TextWriter output)
        {
            output.Write("\tpublic partial class ");
            output.Write(name);
            output.Write(": global::System.IEquatable<");
            output.Write(name);
            output.WriteLine(">");

            output.WriteLine("\t{");

            output.Write("\t\tpublic bool Equals(");
            output.Write(name);
            output.WriteLine(" other)");

            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\tif (other == null) return false;");
            var first = true;
            foreach (var member in members)
            {
                if (first)
                {
                    output.Write("\t\t\treturn (");
                }
                else
                {
                    output.WriteLine();
                    output.Write("\t\t\t\t && (");
                }
                first = false;
                member.WriteEqualityComparison(output);
                output.Write(")");
            }
            output.WriteLine(";");

            output.WriteLine("\t\t}");
            output.WriteLine("\t}");
        }

        public void WriteClassToStringImplementation(TextWriter output)
        {
            output.Write("\tpublic partial class ");
            output.WriteLine(name);
            output.WriteLine("\t{");

            output.WriteLine("\t\tpublic override global::System.String ToString()");
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\treturn ToString(global::System.Globalization.CultureInfo.InvariantCulture);");
            output.WriteLine("\t\t}");

            output.WriteLine("\t\tpublic global::System.String ToString(global::System.IFormatProvider format)");
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\tglobal::System.Text.StringBuilder result = new System.Text.StringBuilder();");
            var first = true;
            foreach (var member in members)
            {
                if (!first)
                    output.WriteLine("\t\t\tresult.AppendLine();");
                member.WriteToStringOutput(output);
                first = false;
            }
            output.WriteLine("\t\t\treturn result.ToString();");
            output.WriteLine("\t\t}");

            output.WriteLine("\t}");
        }
    }
}
