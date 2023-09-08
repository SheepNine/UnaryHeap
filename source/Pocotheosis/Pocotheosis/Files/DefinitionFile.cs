using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteDefinitionFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using var file = File.CreateText(outputFileName);
            WriteNamespaceHeader(dataModel, file,
                new[] { "_nsS_", "_nsC_", "_nsG_", "_nsL_" });
            WriteConstructorHelperClass(file, dataModel);
            foreach (var pocoClass in dataModel.Classes)
                WriteClassDeclaration(pocoClass, file);
            foreach (var pocoEnum in dataModel.Enums)
                WriteEnumDeclaration(pocoEnum, file);
            WriteNamespaceFooter(file);
        }

        static void WriteConstructorHelperClass(TextWriter output, PocoNamespace dataModel)
        {
            output.EmitCode(
$"    public interface IPoco {{ }}",
$"",
$"    public abstract partial class Poco : IPoco",
$"    {{",
$"        protected static bool CheckValue(string value, bool allowNull)",
$"        {{",
$"            return allowNull || value != null;",
$"        }}"
            );
            foreach (var classe in dataModel.Classes) output.EmitCode(
$"",
$"        protected static bool CheckValue({classe.Name} value, bool allowNull)",
$"        {{",
$"            return allowNull || value != null;",
$"        }}"
            );
            output.EmitCode(
@"
        protected static bool CheckValue<T>(T _, bool _2)
        {
            return true;
        }

        protected static bool CheckValue<T>(_nsG_.IEnumerable<T> memberValues,
            _nsS_.Func<T, bool, bool> memberChecker, bool memberIsNullable)
        {
            return memberValues != null && _nsL_.Enumerable.All(memberValues,
                (m) => memberChecker(m, memberIsNullable));
        }

        protected static bool CheckValue<TKey, TValue>(
            _nsG_.IDictionary<TKey, TValue> memberValues,
            _nsS_.Func<TValue, bool, bool> valueChecker, bool valueIsNullable)
        {
            return memberValues != null && _nsL_.Enumerable.All(memberValues,
                (kv) => valueChecker(kv.Value, valueIsNullable));
        }

        protected class ListWrapper<T> : _nsG_.IReadOnlyList<T>
        {
            private readonly _nsG_.IList<T> wrappedObject;

            public ListWrapper(_nsG_.IList<T> wrappedObject)
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

            public _nsG_.IEnumerator<T> GetEnumerator()
            {
                return wrappedObject.GetEnumerator();
            }

            _nsC_.IEnumerator _nsC_.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        protected class WrapperDictionary<TKey, TValue> : _nsG_.IReadOnlyDictionary<TKey, TValue>
        {
            private readonly _nsG_.IDictionary <TKey, TValue> wrappedObject;

            public WrapperDictionary(_nsG_.IDictionary<TKey, TValue> wrappedObject)
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

            public _nsG_.IEnumerable<TKey> Keys
            {
                get { return wrappedObject.Keys; }
            }

            public _nsG_.IEnumerable<TValue> Values
            {
                get { return wrappedObject.Values; }
            }

            public bool ContainsKey(TKey key)
            {
                return wrappedObject.ContainsKey(key);
            }

            public _nsG_.IEnumerator<_nsG_.KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return wrappedObject.GetEnumerator();
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                return wrappedObject.TryGetValue(key, out value);
            }

            _nsC_.IEnumerator _nsC_.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }"
            );
        }

        static void WriteClassDeclaration(PocoClass clasz, TextWriter output)
        {
            output.EmitCode(
$"",
$"    public partial class {clasz.Name} : Poco",
$"    {{"
            );
            WriteClassDeclarationContents(clasz, output);
            output.EmitCode(
$"    }}"
            );
        }

        public static void WriteClassDeclarationContents(PocoClass clasz, TextWriter output)
        {
            var paramList = string.Join(", ", clasz.Members.Select(
                m => $"{m.FormalParameterType} {m.PublicMemberName}"));

            foreach (var member in clasz.Members)
            {
                output.EmitCode(
$"        {member.PublicMemberDeclaration()}",
$"        {member.BackingStoreDeclaration()}",
$""
                );
            }
            output.EmitCode(
$"        public {clasz.Name} ({paramList})",
$"        {{"
            );
            foreach (var member in clasz.Members
                .Where(m => m.NeedsConstructorCheck)) output.EmitCode(
$"            if (!{member.InputCheck()})",
$"                throw new _nsS_.ArgumentNullException(nameof({member.PublicMemberName}));",
$""
            );
            foreach (var member in clasz.Members) output.EmitCode(
                member.Assignment()
            );
            output.EmitCode(
$"        }}");
        }

        static void WriteEnumDeclaration(PocoEnumDefinition enume, StreamWriter output)
        {
            output.WriteLine();
            output.EmitCodeConditionally(enume.IsBitField,
$"    [_nsS_.Flags]"
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
    }
}

namespace Pocotheosis.MemberTypes
{
    partial class PrimitiveType
    {
        public  string PublicMemberDeclaration(string variableName, string privateName)
        {
            return $"public {TypeName} {variableName} {{ get {{ return {privateName}; }} }}";
        }

        public string BackingStoreDeclaration(string variableName, string privateName)
        {
            return $"readonly {TypeName} {privateName};";
        }

        public string[] Assignment(string variableName, string privateName)
        {
            return new[]
            {
$"            {privateName} = {variableName};"
            };
        }
    }

    partial class ArrayType
    {
        public string PublicMemberDeclaration(string variableName, string privateName)
        {
            return $"public _nsG_.IReadOnlyList<{elementType.TypeName}> {variableName} "
                + "{ get; private set; }";
        }

        public string BackingStoreDeclaration(string variableName, string privateName)
        {
            return $"readonly _nsG_.IList<{elementType.TypeName}> {privateName};";
        }

        public string[] Assignment(string variableName, string privateName)
        {
            return new[]
            {
$"            this.{privateName} = "
+ $"_nsL_.Enumerable.ToArray({variableName});",
$"            this.{variableName} = "
+ $"new ListWrapper<{elementType.TypeName}>({privateName});"
            };
        }
    }

    partial class DictionaryType
    {
        public string PublicMemberDeclaration(string variableName, string privateName)
        {
            return $"public _nsG_.IReadOnlyDictionary<{keyType.TypeName}, {valueType.TypeName}>"
                + $" {variableName} {{ get; private set; }}";
        }

        public string BackingStoreDeclaration(string variableName, string privateName)
        {
            return $"readonly _nsG_.SortedDictionary<{keyType.TypeName}, {valueType.TypeName}> "
                + $"{privateName};";
        }

        public string[] Assignment(string variableName, string privateName)
        {
            return new[]
            {
$"            this.{privateName} = new _nsG_.SortedDictionary<"
+ $"{keyType.TypeName}, {valueType.TypeName}>({variableName});",
$"            this.{variableName} = new WrapperDictionary<"
+ $"{keyType.TypeName}, {valueType.TypeName}>({privateName});",
            };
        }
    }
}
