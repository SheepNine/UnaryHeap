using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteEquatableFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                WriteNamespaceHeader(dataModel, file);
                WriteEqualityHelperClass(file, dataModel);
                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    WriteClassEqualityDeclaration(pocoClass, file);
                }
                WriteNamespaceFooter(file);
            }
        }

        static void WriteClassEqualityDeclaration(PocoClass clasz, TextWriter output)
        {
            output.Write("\tpublic partial class ");
            output.Write(clasz.Name);
            output.Write(": global::System.IEquatable<");
            output.Write(clasz.Name);
            output.WriteLine(">");

            output.WriteLine("\t{");

            output.Write("\t\tpublic bool Equals(");
            output.Write(clasz.Name);
            output.WriteLine(" other)");
            output.WriteLine("\t\t{");

            if (clasz.Members.Count() > 0)
            {
                output.WriteLine("\t\t\tif (other == null) return false;");
                var first = true;
                foreach (var member in clasz.Members)
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
                    output.Write(member.GetEqualityTester());
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
            output.Write(clasz.Name);
            output.WriteLine(");");
            output.WriteLine("\t\t}");
            output.WriteLine();

            output.WriteLine("\t\tpublic override int GetHashCode()");
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\tint result = 0;");
            foreach (var member in clasz.Members)
            {
                output.Write("\t\t\tresult = ((result << 19) | (result >> 13)) ^ (");
                output.Write(member.GetHasher());
                output.WriteLine(");");
            }
            output.WriteLine("\t\t\treturn result;");
            output.WriteLine("\t\t}");

            output.WriteLine("\t}");
        }

        static void WriteEqualityHelperClass(TextWriter output,
            PocoNamespace dataModel)
        {
            output.WriteLine(@"    static class EquatableHelper
    {
        public static bool AreEqual(bool a, bool b) { return a == b; }
        public static bool AreEqual(string a, string b) { return string.Equals(a, b); }
        public static bool AreEqual(byte a, byte b) { return a == b; }
        public static bool AreEqual(ushort a, ushort b) { return a == b; }
        public static bool AreEqual(uint a, uint b) { return a == b; }
        public static bool AreEqual(ulong a, ulong b) { return a == b; }
        public static bool AreEqual(sbyte a, sbyte b) { return a == b; }
        public static bool AreEqual(short a, short b) { return a == b; }
        public static bool AreEqual(int a, int b) { return a == b; }
        public static bool AreEqual(long a, long b) { return a == b; }");

            foreach (var enume in dataModel.Enums)
            {
                output.WriteLine(string.Format("        public static bool AreEqual("
                    + "{0} a, {0} b) "
                    + "{{ return a == b; }}", enume.Name));
            }

            foreach (var classe in dataModel.Classes)
            {
                output.WriteLine(string.Format("        public static bool AreEqual("
                    + "{0} a, {0} b) "
                    + "{{ return a.Equals(b); }}", classe.Name));
            }

            output.WriteLine(@"        public static bool ListEquals<T>("
                + @"global::System.Collections.Generic.IList<T> a, "
                + @"global::System.Collections.Generic.IList<T> b, "
                + @"global::System.Func<T, T, bool> comparator)
        {
            if (a.Count != b.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
                if (!comparator(a[i], b[i]))
                    return false;

            return true;
        }

        public static bool DictionaryEquals<TKey, TValue>(
            global::System.Collections.Generic.SortedDictionary<TKey, TValue> a,
            global::System.Collections.Generic.SortedDictionary<TKey, TValue> b,
            global::System.Func<TValue, TValue, bool> valueComparator)
        {
            if (a.Count != b.Count)
                return false;

            foreach (var key in a.Keys)
            {
                if (!b.ContainsKey(key))
                    return false;
                if (!valueComparator(a[key], b[key]))
                    return false;
            }
            return true;
        }
    }");
        }
    }
}
