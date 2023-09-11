using System.ComponentModel.DataAnnotations;
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

            using var file = File.CreateText(outputFileName);
            WriteNamespaceHeader(dataModel, file,
                new[] { "_nsS_", "_nsG_" });
            WriteEqualityHelperClass(file, dataModel);
            foreach (var pocoClass in dataModel.Classes)
                WriteClassEquality(pocoClass, file);
            WriteNamespaceFooter(file);
        }

        static void WriteEqualityHelperClass(TextWriter output, PocoNamespace dataModel)
        {
            output.EmitCode(
@"    partial class Poco
    {
        protected static bool AreEqual(string a, string b)
        {
            return string.Equals(a, b, _nsS_.StringComparison.Ordinal);
        }

        protected static bool AreEqual(bool a, bool b)
        {
            return a == b;
        }

        protected static bool AreEqual(byte a, byte b)
        {
            return a == b;
        }

        protected static bool AreEqual(ushort a, ushort b)
        {
            return a == b;
        }

        protected static bool AreEqual(uint a, uint b)
        {
            return a == b;
        }

        protected static bool AreEqual(ulong a, ulong b)
        {
            return a == b;
        }

        protected static bool AreEqual(sbyte a, sbyte b)
        {
            return a == b;
        }

        protected static bool AreEqual(short a, short b)
        {
            return a == b;
        }

        protected static bool AreEqual(int a, int b)
        {
            return a == b;
        }

        protected static bool AreEqual(long a, long b)
        {
            return a == b;
        }

        protected static bool AreEqual(IPoco a, IPoco b)
        {
            if (a == null) return b == null;
");
            foreach (var clasz in dataModel.Classes) output.EmitCode(
$"            if (a is {clasz.Name})",
$"                return AreEqual(a as {clasz.Name}, b as {clasz.Name});"
            );
        output.EmitCode(
@"
            throw new _nsS_.InvalidOperationException(""Unrecognized type"");
        }
"
            );
            foreach (var enume in dataModel.Enums) output.EmitCode(
$"",
$"        protected static bool AreEqual({enume.Name} a, {enume.Name} b)",
$"        {{",
$"            return a == b;",
$"        }}"
            );
            foreach (var clasz in dataModel.Classes) output.EmitCode(
$"",
$"        protected static bool AreEqual({clasz.Name} a, {clasz.Name} b)",
$"        {{",
$"            return a == null ? b == null : a.Equals(b);",
$"        }}"
            );

            output.EmitCode(
@"
        protected static bool AreEqual<T>(_nsG_.IList<T> a, _nsG_.IList<T> b,
            _nsS_.Func<T, T, bool> comparator)
        {
            if (a.Count != b.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
                if (!comparator(a[i], b[i]))
                    return false;

            return true;
        }

        protected static bool AreEqual<TKey, TValue>(
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

        public static int GetHashCode<T>(_nsG_.IList<T> list)
        {
            int result = 0;
            foreach (var element in list)
                result = ((result << 19) | (result >> 13))
                    ^ (element == null ? 0x0EADBEEF : element.GetHashCode());
            return result;
        }

        public static int GetHashCode<TKey, TValue>( _nsG_.IDictionary<TKey, TValue> dictionary)
        {
            int result = 0;
            foreach (var iter in dictionary)
            {
                result = ((result << 19) | (result >> 13)) ^ (iter.Key.GetHashCode());
                result = ((result << 19) | (result >> 13))
                    ^ (iter.Value == null ? 0x0EADBEEF : iter.Value.GetHashCode());
            }
            return result;
        }
    }"
            );
        }

        static void WriteClassEquality(PocoClass clasz, TextWriter output)
        {
            output.EmitCode(
$"",
$"    public partial class {clasz.Name} : _nsS_.IEquatable<{clasz.Name}>",
$"    {{"
            );
            WriteClassEqualityContents(clasz, output);
            output.EmitCode(
$"    }}"
            );
        }

        public static void WriteClassEqualityContents(PocoClass clasz, TextWriter output)
        {
            output.EmitCode(
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
$"            int result = {clasz.StreamingId};"
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
$"        }}"
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
            return $"AreEqual(this.{privateName}, other.{privateName})";
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
            return $"AreEqual(this.{privateName}, other.{privateName}, AreEqual)";
        }

        public string GetHasher(string variableName, string privateName)
        {
            return $"GetHashCode({privateName})";
        }
    }

    partial class DictionaryType
    {
        public string GetEqualityTester(string variableName, string privateName)
        {
            return $"AreEqual(this.{privateName}, other.{privateName}, AreEqual)";
        }

        public string GetHasher(string variableName, string privateName)
        {
            return $"GetHashCode({privateName})";
        }
    }
}