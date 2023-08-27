using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteBuilderFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using (var file = File.CreateText(outputFileName))
            {
                WriteNamespaceHeader(dataModel, file);

                file.EmitCode(
@"    class BuilderHelper
    {
        public static _nsG_.IList<TBuilder> UnreifyArray<TBase, TBuilder>(
            _nsG_.IEnumerable<TBase> values,
            _nsS_.Func<TBase, TBuilder> unreifier)
        {
            return _nsL_.Enumerable.ToList(
                _nsL_.Enumerable.Select(values, unreifier));
        }

        public static _nsG_.IEnumerable<TBase> ReifyArray<TBase, TBuilder>(
            _nsG_.IEnumerable<TBuilder> values,
            _nsS_.Func<TBuilder, TBase> reifier)
        {
            return _nsL_.Enumerable.Select(values, reifier);
        }

        public static _nsG_.SortedDictionary<TKey, TBuilder>
            UnreifyDictionary<TKey, TBase, TBuilder>(
            _nsG_.IDictionary<TKey, TBase> values,
            _nsS_.Func<TBase, TBuilder> unreifier)
        {
            return new _nsG_.SortedDictionary<TKey, TBuilder>(
                _nsL_.Enumerable.ToDictionary(
                    values, pair => pair.Key, pair => unreifier(pair.Value)));
        }

        public static _nsG_.IDictionary<TKey, TBase>
            ReifyDictionary<TKey, TBuilder, TBase>(
            _nsG_.IDictionary<TKey, TBuilder> values,
            _nsS_.Func<TBuilder, TBase> reifier)
        {
            return _nsL_.Enumerable.ToDictionary(
                values, pair => pair.Key, pair => reifier(pair.Value));
        }
    }"
                );
                foreach (var clasz in dataModel.Classes)
                    WriteBuilderImplementation(clasz, file);

                WriteNamespaceFooter(file);
            }
        }

        static void WriteBuilderImplementation(PocoClass clasz, TextWriter output)
        {
            if (!clasz.Members.Any())
                return;

            var fieldParams = string.Join(", ",
                clasz.Members.Select(m => m.BackingStoreName));
            var builderCtorParams = string.Join(", ",
                clasz.Members.Select(m => $"{m.FormalParameterType} {m.PublicMemberName}"));
            var buildParams = string.Join(", ",
                clasz.Members.Select(m => m.BuilderReifier()));

            output.EmitCode(
$"",
$"    public partial class {clasz.Name}",
$"    {{",
$"        public Builder ToBuilder()",
$"        {{",
$"            return new Builder({fieldParams});",
$"        }}",
$"",
$"        public class Builder",
$"        {{"
            );
            foreach (var member in clasz.Members) output.EmitCode(
$"            {member.BuilderDeclaration()}"
            );
            output.EmitCode(
$"",
$"            public Builder({builderCtorParams})",
$"            {{"
            );
            foreach (var member in clasz.Members
                .Where(m => m.NeedsConstructorCheck)) output.EmitCode(
$"                if (!{member.InputCheck()})",
$"                    throw new _nsS_.ArgumentNullException(nameof({member.PublicMemberName}));",
$""
            );
            foreach (var member in clasz.Members) output.EmitCode(
$"                {member.BuilderAssignment()}"
            );
            output.EmitCode(
$"            }}",
$""
            );
            output.EmitCode(
$"            public {clasz.Name} Build()",
$"            {{",
$"                return new {clasz.Name}({buildParams});",
$"            }}" +
$""
            );
            foreach (var member in clasz.Members)
                member.WriteBuilderPlumbing(output);

            output.EmitCode(
$"        }}",
$"    }}"
            );
        }
    }
}

namespace Pocotheosis.MemberTypes
{
    public partial class PrimitiveType
    {
        public string BuilderDeclaration(string variableName, string privateName)
        {
            return $"private {BuilderTypeName} {privateName};";
        }

        public string BuilderAssignment(string variableName, string privateName)
        {
            return $"{privateName} = {BuilderUnreifier(variableName)};";
        }

        public virtual void WriteBuilderPlumbing(string variableName, string singularName,
            string privateName, TextWriter output)
        {
            output.EmitCode(
$"",
$"            public Builder With{variableName}({TypeName} value)",
$"            {{",
$"                if (!{InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));",
$"                {privateName} = {BuilderUnreifier("value")};",
$"                return this;",
$"            }}",
$"",
$"            public {TypeName} {variableName}",
$"            {{",
$"                get {{ return {BuilderUnreifier(privateName)}; }}",
$"            }}"
            );
        }

        public virtual string BuilderReifier(string privateName)
        {
            return privateName;
        }

        public virtual string BuilderUnreifier(string variableName)
        {
            return variableName;
        }
    }

    public partial class ClassType
    {
        public override void WriteBuilderPlumbing(string variableName, string singularName,
            string privateName, TextWriter output)
        {
            output.EmitCode(
$"",
$"            public Builder With{variableName}({TypeName} value)",
$"            {{",
$"                if (!{InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));",
$"                {privateName} = {BuilderUnreifier("value")};",
$"                return this;",
$"            }}",
$"",
$"            public {BuilderTypeName} {variableName}",
$"            {{",
$"                get {{ return {privateName}; }}",
$"            }}"
            );
        }

        public override string BuilderReifier(string privateName)
        {
            if (isNullable)
                return $"{privateName} == null ? null : {privateName}.Build()";
            else
                return $"{privateName}.Build()";
        }

        public override string BuilderUnreifier(string variableName)
        {
            if (isNullable)
                return $"{variableName} == null ? null : {variableName}.ToBuilder()";
            else
                return $"{variableName}.ToBuilder()";
        }
    }

    partial class ArrayType
    {
        public virtual string BuilderDeclaration(string variableName, string privateName)
        {
            return $"private _nsG_.IList<{elementType.BuilderTypeName}> {privateName};";
        }

        public virtual string BuilderAssignment(string variableName, string privateName)
        {
            return $"{privateName} = BuilderHelper.UnreifyArray({variableName}, "
                + $"elem => {elementType.BuilderUnreifier("elem")});";
        }

        public void WriteBuilderPlumbing(string variableName, string singularName,
            string privateName, TextWriter output)
        {
            output.EmitCode(
$"",
$"            public int Num{variableName}",
$"            {{",
$"                get {{ return {privateName}.Count; }}",
$"            }}",
$"",
$"            public {elementType.BuilderTypeName} Get{singularName}(int index)",
$"            {{",
$"                return {privateName}[index];",
$"            }}",
$"",
$"            public void Set{singularName}(int index, {elementType.TypeName} value)",
$"            {{",
$"                if (!{elementType.InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));",
$"",
$"                {privateName}[index] = {elementType.BuilderUnreifier("value")};",
$"            }}",
$"",
$"            public void Append{singularName}({elementType.TypeName} value)",
$"            {{",
$"                if (!{elementType.InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));",
$"",
$"                {privateName}.Add({elementType.BuilderUnreifier("value")});",
$"            }}",
$"",
$"            public void Insert{singularName}At(int index, {elementType.TypeName} value)",
$"            {{",
$"                if (!{elementType.InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));",
$"",
$"                {privateName}.Insert(index, {elementType.BuilderUnreifier("value")});",
$"            }}",
$"",
$"            public void Remove{singularName}At(int index)",
$"            {{",
$"                {privateName}.RemoveAt(index);",
$"            }}",
$"",
$"            public void Clear{variableName}()",
$"            {{",
$"                {privateName}.Clear();",
$"            }}",
$"",
$"            public _nsG_.IEnumerable<{elementType.BuilderTypeName}> {singularName}Values",
$"            {{",
$"                get {{ return {privateName}; }}",
$"            }}"
            );
        }

        public virtual string BuilderReifier(string variableName)
        {
            return $"BuilderHelper.ReifyArray({variableName}, " +
                $"elem => {elementType.BuilderReifier("elem")})";
        }
    }

    partial class DictionaryType
    {
        public virtual string BuilderDeclaration(string variableName, string privateName)
        {
            return $"private _nsG_.SortedDictionary<{keyType.TypeName}, "
                + $"{valueType.BuilderTypeName}> {privateName};";
        }

        public virtual string BuilderAssignment(string variableName, string privateName)
        {
            return $"{privateName} = BuilderHelper.UnreifyDictionary({variableName}, "
                + $"elem => {valueType.BuilderUnreifier("elem")});";
        }

        public void WriteBuilderPlumbing(string variableName, string singularName,
            string privateName, TextWriter output)
        {
            output.EmitCode(
$"",
$"            public {valueType.BuilderTypeName} Get{singularName}({keyType.TypeName} key)",
$"            {{",
$"                return {privateName}[key];",
$"            }}",
$"",
$"            public void Set{singularName}({keyType.TypeName} key, {valueType.TypeName} value)",
$"            {{",
$"                if (!{keyType.InputCheck("key")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(key));",
$"                if (!{valueType.InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));",
$"                {privateName}[key] = {valueType.BuilderUnreifier("value")};",
$"            }}",
$"",
$"            public void Remove{singularName}({keyType.TypeName} key)",
$"            {{",
$"                {privateName}.Remove(key);",
$"            }}",
$"",
$"            public void Clear{variableName}()",
$"            {{",
$"                {privateName}.Clear();",
$"            }}",
$"",
$"            public bool Contains{singularName}Key({keyType.TypeName} key)",
$"            {{",
$"                return {privateName}.ContainsKey(key);",
$"            }}",
$"",
$"            public int Count{variableName}",
$"            {{",
$"                get {{ return {privateName}.Count; }}",
$"            }}",
$"",
$"            public _nsG_.IEnumerable<{keyType.TypeName}> {singularName}Keys",
$"            {{",
$"                get {{ return {privateName}.Keys; }}",
$"            }}",
$"",
$"            public _nsG_.IEnumerable<_nsG_.KeyValuePair<{keyType.TypeName}, "
                    + $"{valueType.BuilderTypeName}>> {singularName}Values",
$"            {{",
$"                get {{ return {privateName}; }}",
$"            }}"
            );
        }

        public virtual string BuilderReifier(string variableName)
        {
            return $"BuilderHelper.ReifyDictionary({variableName}, "
                + $"val => {valueType.BuilderReifier("val")})";
        }
    }
}