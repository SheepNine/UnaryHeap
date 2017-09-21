using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Pocotheosis
{
    public class PocoClass
    {
        public string Name { get { return name; } }
        string name;
        public IEnumerable<string> Routes { get { return routes; } }
        string[] routes;
        int id;
        List<IPocoMember> members;

        internal PocoClass(string name, int id, string[] routes, IEnumerable<IPocoMember> members)
        {
            this.name = name;
            this.id = id;
            this.routes = routes;
            this.members = new List<IPocoMember>(members);
        }

        public void WriteClassDeclaration(TextWriter output)
        {
            output.Write("\tpublic partial class ");
            output.Write(name);
            output.WriteLine(" : Poco");
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
                member.WriteConstructorCheck(output);

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

            if (members.Count > 0)
            {
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
            }
            else
            {
                output.WriteLine("\t\t\treturn other != null;");
            }
            output.WriteLine("\t\t}");
            output.WriteLine();

            output.WriteLine("\t\tpublic override bool Equals(object other)");
            output.WriteLine("\t\t{");
            output.Write("\t\t\treturn Equals(other as ");
            output.Write(name);
            output.WriteLine(");");
            output.WriteLine("\t\t}");
            output.WriteLine();

            output.WriteLine("\t\tpublic override int GetHashCode()");
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\tint result = 0;");
            foreach (var member in members)
            {
                output.Write("\t\t\tresult = ((result << 19) | (result >> 13)) ^ (");
                member.WriteHash(output);
                output.WriteLine(");");
            }
            output.WriteLine("\t\t\treturn result;");
            output.WriteLine("\t\t}");

            output.WriteLine("\t}");
        }

        public void WriteClassToStringImplementation(TextWriter output)
        {
            output.Write("\tpublic partial class ");
            output.WriteLine(name);
            output.WriteLine("\t{");

            output.WriteLine("\t\tpublic override string ToString()");
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\treturn ToString(" +
                "global::System.Globalization.CultureInfo.InvariantCulture);");
            output.WriteLine("\t\t}");

            output.WriteLine("\t\tpublic string ToString(" + 
                "global::System.IFormatProvider format)");
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\tglobal::System.Text.StringBuilder result = " +
                "new System.Text.StringBuilder();");
            output.WriteLine("\t\t\tresult.Append(\"" + name + "\");");
            foreach (var member in members)
            {
                member.WriteToStringOutput(output);
                output.WriteLine();
            }
            output.WriteLine("\t\t\treturn result.ToString();");
            output.WriteLine("\t\t}");

            output.WriteLine("\t}");
        }

        public void WriteSerializationImplementation(TextWriter output)
        {
            output.Write("\tpublic partial class ");
            output.WriteLine(name);
            output.WriteLine("\t{");

            output.WriteLine("\t\tpublic override void Serialize(" +
                "global::System.IO.Stream output)");
            output.WriteLine("\t\t{");
            foreach (var member in members)
            {
                output.Write("\t\t\t");
                member.WriteSerialization(output);
                output.WriteLine();
            }
            output.WriteLine("\t\t}");
            output.WriteLine();
            output.Write("\t\tpublic static ");
            output.Write(name);
            output.WriteLine(" Deserialize(global::System.IO.Stream input)");
            output.WriteLine("\t\t{");
            foreach (var member in members)
            {
                output.Write("\t\t\t");
                member.WriteDeserialization(output);
                output.WriteLine();
            }
            output.Write("\t\t\treturn new ");
            output.Write(name);
            output.Write("(");
            output.Write(string.Join(", ", members.Select(member => member.name)));
            output.WriteLine(");");
            output.WriteLine("\t\t}");

            output.WriteLine("\t}");
        }

        public void WriteClassStreamingImplementation(StreamWriter output)
        {
            output.Write("\tpublic partial class ");
            output.WriteLine(name);
            output.WriteLine("\t{");
            output.Write("\t\tpublic const int Identifier = ");
            output.Write(id.ToString(CultureInfo.InvariantCulture));
            output.WriteLine(";");
            output.WriteLine();
            output.WriteLine("\t\tprotected override int getIdentifier()");
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\treturn Identifier;");
            output.WriteLine("\t\t}");
            output.WriteLine("\t}");
        }

        internal void WriteRoutingDelcaration(TextWriter output)
        {
            output.Write("\t\tvoid ");
            output.Write(name);
            output.Write("(");
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
            output.WriteLine(");");
        }

        internal void WriteRoutingImplementation(TextWriter output)
        {
            if (routes.Length == 0)
                return;

            output.Write("\tpublic partial class " + name + ": ");
            output.WriteLine(string.Join(", ", routes.Select(
                r => { return "I" + r + "RoutedPoco"; })));
            output.WriteLine("\t{");
            foreach(var route in routes)
            {
                output.WriteLine("\t\tpublic void RouteTo(I" + route +
                    "Destination destination)");
                output.WriteLine("\t\t{");
                output.WriteLine("\t\t\tdestination." + name + "(" +
                    string.Join(", ", members.Select(m => m.name)) + ");");
                output.WriteLine("\t\t}");
            }
            output.WriteLine("\t}");
        }

        // TODO: Builder classes for POCOs
    }
}
