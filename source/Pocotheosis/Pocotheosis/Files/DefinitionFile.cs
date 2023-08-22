using System.Globalization;
using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void EmitCode(this TextWriter output, params string[] lines)
        {
            foreach (var line in lines)
                output.WriteLine(line.Replace("    ", "\t"));
        }

        public static void EmitCodeConditionally(this TextWriter output, bool condition,
            params string[] lines)
        {
            if (condition)
                output.EmitCode(lines);
        }

        public static void WriteDefinitionFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using (var file = File.CreateText(outputFileName))
            {
                WriteNamespaceHeader(dataModel, file);
                WriteConstructorHelperClass(file, dataModel);

                file.EmitCode(
$"",
$"    public abstract partial class Poco",
$"    {{",
$"    }}"
                );
                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    WriteClassDeclaration(pocoClass, file);
                }
                foreach (var pocoEnum in dataModel.Enums)
                {
                    file.WriteLine();
                    WriteEnumDeclaration(pocoEnum, file);
                }
                WriteNamespaceFooter(file);
            }
        }

        static void WriteClassDeclaration(PocoClass clasz, TextWriter output)
        {
            output.EmitCode(
$"    public partial class {clasz.Name} : Poco",
$"    {{"
            );
            foreach (var member in clasz.Members)
            {
                output.EmitCode(
$"        {member.PublicMemberDeclaration()}",
$"        {member.BackingStoreDeclaration()}"
                );
            }
            var paramList = string.Join(", ", clasz.Members.Select(m => m.FormalParameter()));
            output.EmitCode(
$"",
$"        public {clasz.Name} ({paramList})",
$"        {{"
            );
            foreach (var member in clasz.Members
                .Where(m => m.NeedsConstructorCheck)) output.EmitCode(
$"            {member.ConstructorCheck()}"
            );
            foreach (var member in clasz.Members) output.EmitCode(
$"            {member.Assignment()}"
            );
            output.EmitCode(
$"        }}",
$"    }}"
            );
        }

        static void WriteEnumDeclaration(PocoEnumDefinition enume, StreamWriter output)
        {
            if (enume.IsBitField)
                output.EmitCode(
$"    [global::System.Flags]"
                );
            output.EmitCode(
$"    public enum {enume.Name}",
$"    {{"
            );
            foreach (var enumerator in enume.Enumerators) output.EmitCode(
$"        {enumerator.Name} = {enumerator.Value},"
            );
            output.EmitCode(
$"    }}"
            );
        }

        static void WriteConstructorHelperClass(TextWriter output, PocoNamespace dataModel)
        {
            output.EmitCode(
$"    static class ConstructorHelper",
$"    {{",
$"        public static bool CheckValue(string value, bool allowNull)",
$"        {{",
$"            return allowNull || value != null;",
$"        }}"
            );
            foreach (var classe in dataModel.Classes) output.EmitCode(
$"",
$"        public static bool CheckValue({classe.Name} value, bool allowNull)",
$"        {{",
$"            return allowNull || value != null;",
$"        }}"
            );
            foreach (var TPrimitive in new[] {
                    "bool", "byte", "short", "int", "long",
                    "sbyte", "ushort", "uint", "ulong" }) output.EmitCode(
$"",
$"        public static bool CheckValue({TPrimitive} _, bool allowNull)",
$"        {{",
$"            return !allowNull;",
$"        }}"
            );
            foreach (var enume in dataModel.Enums) output.EmitCode(
$"",
$"        public static bool CheckValue({enume.Name} _, bool allowNull)",
$"        {{",
$"            return !allowNull;",
$"        }}"
            );
            output.EmitCode(
@"
        public static bool CheckArrayValue<T>(
            global::System.Collections.Generic.IEnumerable<T> memberValues,
            global::System.Func<T, bool, bool> memberChecker,
            bool memberIsNullable)
        {
            return memberValues != null && global::System.Linq.Enumerable.All(memberValues,
                (m) => memberChecker(m, memberIsNullable));
        }

        public static bool CheckDictionaryValue<TKey, TValue>(
            global::System.Collections.Generic.IDictionary<TKey, TValue> memberValues,
            global::System.Func<TKey, bool, bool> keyChecker,
            global::System.Func<TValue, bool, bool> valueChecker,
            bool valueIsNullable)
        {
            return memberValues != null && global::System.Linq.Enumerable.All(memberValues,
                (kv) => keyChecker(kv.Key, false) && valueChecker(kv.Value, valueIsNullable));
        }
    }

    class ListWrapper<T> : global::System.Collections.Generic.IReadOnlyList<T>
    {
        private readonly global::System.Collections.Generic.IList<T> wrappedObject;

        public ListWrapper(global::System.Collections.Generic.IList<T> wrappedObject)
        {
            this.wrappedObject = wrappedObject;
        }

        public T this[int index]
        {
            get { return wrappedObject[index]; }
        }

        public int Count
        {
            get { return wrappedObject.Count; }
        }

        public global::System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return wrappedObject.GetEnumerator();
        }

        global::System.Collections.IEnumerator
            global::System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class WrapperDictionary<TKey, TValue>
        : global::System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>
    {
        private readonly global::System.Collections.Generic.IDictionary
                <TKey, TValue> wrappedObject;

        public WrapperDictionary(
            global::System.Collections.Generic.IDictionary<TKey, TValue> wrappedObject)
        {
            this.wrappedObject = wrappedObject;
        }

        public TValue this[TKey key]
        {
            get { return wrappedObject[key]; }
        }

        public int Count
        {
            get { return wrappedObject.Count; }
        }

        public global::System.Collections.Generic.IEnumerable<TKey> Keys
        {
            get { return wrappedObject.Keys; }
        }

        public global::System.Collections.Generic.IEnumerable<TValue> Values
        {
            get { return wrappedObject.Values; }
        }

        public bool ContainsKey(TKey key)
        {
            return wrappedObject.ContainsKey(key);
        }

        public global::System.Collections.Generic.IEnumerator<
            global::System.Collections.Generic.KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return wrappedObject.GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return wrappedObject.TryGetValue(key, out value);
        }

        global::System.Collections.IEnumerator
            global::System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }"
            );
        }
    }
}

namespace Pocotheosis.MemberTypes
{
    partial class PrimitiveType
    {
        public string PublicMemberName(string variableName)
        {
            return variableName;
        }

        public string BackingStoreName(string variableName)
        {
            return "__" + variableName;
        }

        public virtual string Assignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = {1};",
                BackingStoreName(variableName), TempVarName(variableName));
        }
    }

    partial class ArrayType
    {
        public string PublicMemberName(string variableName)
        {
            return variableName;
        }

        public string BackingStoreName(string variableName)
        {
            return "__" + variableName;
        }

        public string Assignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "this.{0} = global::System.Linq.Enumerable.ToArray({1}); " +
                "this.{2} = new ListWrapper<{3}>({0});",
                BackingStoreName(variableName), TempVarName(variableName),
                PublicMemberName(variableName), elementType.TypeName);
        }
    }

    partial class DictionaryType
    {
        public string PublicMemberName(string variableName)
        {
            return variableName;
        }

        public string BackingStoreName(string variableName)
        {
            return "__" + variableName;
        }

        public string Assignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "this.{0} = new {5}.SortedDictionary<{1}, {2}>({3}); " +
                "this.{4} = new WrapperDictionary<{1}, {2}>({0});",
                BackingStoreName(variableName), keyType.TypeName,
                valueType.TypeName, TempVarName(variableName),
                PublicMemberName(variableName),
                "global::System.Collections.Generic");
        }
    }
}
