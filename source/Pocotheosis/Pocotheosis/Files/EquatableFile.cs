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
                WriteNamespaceHeader(dataModel, file,
                    new[] { "_nsS_", "_nsG_" });
                WriteEqualityHelperClass(file, dataModel);
                foreach (var pocoClass in dataModel.Classes)
                    WriteClassEqualityDeclaration(pocoClass, file);
                WriteNamespaceFooter(file);
            }
        }

        static void WriteEqualityHelperClass(TextWriter output, PocoNamespace dataModel)
        {
            output.EmitCode(
@"    static class EquatableHelper
    {
        public static bool AreEqual(string a, string b)
        {
            return string.Equals(a, b, _nsS_.StringComparison.Ordinal);
        }

        public static bool AreEqual(bool a, bool b)
        {
            return a == b;
        }

        public static bool AreEqual(byte a, byte b)
        {
            return a == b;
        }

        public static bool AreEqual(ushort a, ushort b)
        {
            return a == b;
        }

        public static bool AreEqual(uint a, uint b)
        {
            return a == b;
        }

        public static bool AreEqual(ulong a, ulong b)
        {
            return a == b;
        }

        public static bool AreEqual(sbyte a, sbyte b)
        {
            return a == b;
        }

        public static bool AreEqual(short a, short b)
        {
            return a == b;
        }

        public static bool AreEqual(int a, int b)
        {
            return a == b;
        }

        public static bool AreEqual(long a, long b)
        {
            return a == b;
        }"
            );
            foreach (var enume in dataModel.Enums) output.EmitCode(
$"",
$"        public static bool AreEqual({enume.Name} a, {enume.Name} b)",
$"        {{",
$"            return a == b;",
$"        }}"
            );
            foreach (var clasz in dataModel.Classes) output.EmitCode(
$"",
$"        public static bool AreEqual({clasz.Name} a, {clasz.Name} b)",
$"        {{",
$"            return a == null ? b == null : a.Equals(b);",
$"        }}"
            );

            output.EmitCode(
@"
        public static bool ListEquals<T>(_nsG_.IList<T> a, _nsG_.IList<T> b,
            _nsS_.Func<T, T, bool> comparator)
        {
            if (a.Count != b.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
                if (!comparator(a[i], b[i]))
                    return false;

            return true;
        }

        public static bool DictionaryEquals<TKey, TValue>(
            _nsG_.SortedDictionary<TKey, TValue> a,
            _nsG_.SortedDictionary<TKey, TValue> b,
            _nsS_.Func<TValue, TValue, bool> valueComparator)
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
    }"
            );
        }

        static void WriteClassEqualityDeclaration(PocoClass clasz, TextWriter output)
        {
            output.EmitCode(
$"",
$"    public partial class {clasz.Name} : _nsS_.IEquatable<{clasz.Name}>",
$"    {{",
$"        public bool Equals({clasz.Name} other)",
$"        {{"
            );
            if (clasz.Members.Any())
            {
                output.EmitCode(
$"            return (other != null"
                );
                foreach (var member in clasz.Members) output.EmitCode(
$"                && {member.EqualityTester()}"
                );
                output.EmitCode(
$"            );"
                );
            }
            else
            {
                output.EmitCode(
$"            return other != null;"
                );
            }
            output.EmitCode(
$"        }}",
$"",
$"        public override bool Equals(object obj)",
$"        {{",
$"            return Equals(obj as {clasz.Name});",
$"        }}",
$"",
$"        public override int GetHashCode()",
$"        {{"
            );
            if (clasz.Members.Any())
            {
                output.EmitCode(
$"            int result = Identifier;"
                );
                foreach (var member in clasz.Members) output.EmitCode(
$"            result = ((result << 19) | (result >> 13)) ^ ({member.Hasher()});"
                );
                output.EmitCode(
$"            return result;"
                );
            }
            else
            {
                output.EmitCode(
$"            return 42;"
                );
            }
            output.EmitCode(
$"        }}",
$"    }}"
            );
        }
    }
}

namespace Pocotheosis.MemberTypes
{
    partial class PrimitiveType
    {
        public string GetEqualityTester(string variableName, string privateName)
        {
            return $"EquatableHelper.AreEqual(this.{privateName}, other.{privateName})";
        }

        public string GetHasher(string variableName, string privateName)
        {
            if (IsNullable)
                return $"{privateName} == null ? 0x0EADBEEF : {privateName}.GetHashCode()";
            else
                return $"{privateName}.GetHashCode()";
        }
    }

    partial class ArrayType
    {
        public string GetEqualityTester(string variableName, string privateName)
        {
            return $"EquatableHelper.ListEquals(this.{privateName}, other.{privateName}, "
                + "EquatableHelper.AreEqual)";
        }

        public string GetHasher(string variableName, string privateName)
        {
            return $"HashHelper.GetListHashCode({privateName})";
        }
    }

    partial class DictionaryType
    {
        public string GetEqualityTester(string variableName, string privateName)
        {
            return $"EquatableHelper.DictionaryEquals(this.{privateName}, other.{privateName}, "
                + "EquatableHelper.AreEqual)";
        }

        public string GetHasher(string variableName, string privateName)
        {
            return $"HashHelper.GetDictionaryHashCode({privateName})";
        }
    }
}