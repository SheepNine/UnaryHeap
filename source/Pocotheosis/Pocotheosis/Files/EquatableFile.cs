using System.Globalization;
using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteEquatableFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

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

            if (clasz.Members.Any())
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
                    output.Write(member.EqualityTester());
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

            output.WriteLine("\t\tpublic override bool Equals(object obj)");
            output.WriteLine("\t\t{");
            output.Write("\t\t\treturn Equals(obj as ");
            output.Write(clasz.Name);
            output.WriteLine(");");
            output.WriteLine("\t\t}");
            output.WriteLine();

            output.WriteLine("\t\tpublic override int GetHashCode()");
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\tint result = Identifier;");
            foreach (var member in clasz.Members)
            {
                output.Write("\t\t\tresult = ((result << 19) | (result >> 13)) ^ (");
                output.Write(member.Hasher());
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
        public static bool AreEqual(string a, string b)
        {
            return string.Equals(a, b, global::System.StringComparison.Ordinal);
        }
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
                output.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "        public static bool AreEqual("
                    + "{0} a, {0} b) "
                    + "{{ return a == b; }}", enume.Name));
            }

            foreach (var classe in dataModel.Classes)
            {
                output.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "        public static bool AreEqual("
                    + "{0} a, {0} b) "
                    + "{{ return a == null ? b == null : a.Equals(b); }}", classe.Name));
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

namespace Pocotheosis.MemberTypes
{
    partial class PrimitiveType
    {
        public string GetEqualityTester(string variableName, string privateName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "EquatableHelper.AreEqual(this.{0}, other.{0})",
                privateName);
        }

        public string GetHasher(string variableName, string privateName)
        {
            if (IsNullable)
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} == null ? 0x0EADBEEF : {0}.GetHashCode()",
                    privateName);
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0}.GetHashCode()",
                    privateName);
            }
        }
    }

    partial class ArrayType
    {
        public string GetEqualityTester(string variableName, string privateName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "EquatableHelper.ListEquals(this.{0}, other.{0}, EquatableHelper.AreEqual)",
                privateName);
        }

        public string GetHasher(string variableName, string privateName)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                "HashHelper.GetListHashCode({0})",
                privateName);
        }
    }

    partial class DictionaryType
    {
        public string GetEqualityTester(string variableName, string privateName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "EquatableHelper.DictionaryEquals(this.{0}, other.{0}, EquatableHelper.AreEqual)",
                privateName);
        }

        public string GetHasher(string variableName, string privateName)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                "HashHelper.GetDictionaryHashCode({0})",
                privateName);
        }
    }
}