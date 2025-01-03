﻿using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteBuilderFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using var file = File.CreateText(outputFileName);
            WriteNamespaceHeader(dataModel, file, ["_nsS_", "_nsG_", "_nsL_"]);
            WriteBuilderHelperClass(file);
            foreach (var clasz in dataModel.Classes)
                WriteBuilderImplementation(clasz, file);
            WriteNamespaceFooter(file);
        }

        private static void WriteBuilderHelperClass(StreamWriter file)
        {
            file.EmitCode(
@"    partial class Poco
    {
        protected static _nsG_.IList<TBuilder> Unbuild<TBase, TBuilder>(
            _nsG_.IEnumerable<TBase> values, _nsS_.Func<TBase, TBuilder> unbuilder)
        {
            return _nsL_.Enumerable.ToList( _nsL_.Enumerable.Select(values, unbuilder));
        }

        protected static _nsG_.IEnumerable<TBase> Build<TBase, TBuilder>(
            _nsG_.IEnumerable<TBuilder> values, _nsS_.Func<TBuilder, TBase> builder)
        {
            return _nsL_.Enumerable.Select(values, builder);
        }

        protected static _nsG_.SortedDictionary<TKey, TBuilder> Unbuild<TKey, TBase, TBuilder>(
            _nsG_.IDictionary<TKey, TBase> values, _nsS_.Func<TBase, TBuilder> unbuilder)
        {
            return new _nsG_.SortedDictionary<TKey, TBuilder>(
                _nsL_.Enumerable.ToDictionary(
                    values, pair => pair.Key, pair => unbuilder(pair.Value)));
        }

        protected static _nsG_.IDictionary<TKey, TBase> Build<TKey, TBase, TBuilder>(
            _nsG_.IDictionary<TKey, TBuilder> values, _nsS_.Func<TBuilder, TBase> builder)
        {
            return _nsL_.Enumerable.ToDictionary(
                values, pair => pair.Key, pair => builder(pair.Value));
        }
    }"
            );
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
$"            {{"
            );
            output.EmitCodeConditionally(NeedsConstructorCheck,
$"                if (!{InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));",
$""
            );
            output.EmitCode(
$"                {privateName} = {BuilderUnreifier("value")};",
$"                return this;",
$"            }}",
$"",
$"            public {TypeName} {variableName}",
$"            {{",
$"                get {{ return {BuilderUnreifier(privateName)}; }}",
$"                set {{ With{variableName}({BuilderReifier("value")}); }}",
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
$"            {{"
            );
            output.EmitCodeConditionally(NeedsConstructorCheck,
$"                if (!{InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));",
$""
            );
            output.EmitCode(
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
                return $"{privateName}?.Build()";
            else
                return $"{privateName}.Build()";
        }

        public override string BuilderUnreifier(string variableName)
        {
            if (isNullable)
                return $"{variableName}?.ToBuilder()";
            else
                return $"{variableName}.ToBuilder()";
        }
    }

    partial class ArrayType
    {
        public string BuilderDeclaration(string variableName, string privateName)
        {
            return $"private _nsG_.IList<{elementType.BuilderTypeName}> {privateName};";
        }

        public string BuilderAssignment(string variableName, string privateName)
        {
            return $"{privateName} = Unbuild({variableName}, "
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
$"            public Builder With{variableName}(",
$"                    _nsG_.IEnumerable<{elementType.TypeName}> values)",
$"            {{",
$"                if (!{InputCheck("values")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(values));",
$"",
$"                {BuilderAssignment("values", privateName)}",
$"                return this;",
$"            }}",
$"",
$"            public void Set{singularName}(int index, {elementType.TypeName} value)",
$"            {{"
            );
            output.EmitCodeConditionally(elementType.NeedsConstructorCheck,
$"                if (!{elementType.InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));",
$""
            );
            output.EmitCode(
$"                {privateName}[index] = {elementType.BuilderUnreifier("value")};",
$"            }}",
$"",
$"            public void Append{singularName}({elementType.TypeName} value)",
$"            {{"
            );
            output.EmitCodeConditionally(elementType.NeedsConstructorCheck,
$"                if (!{elementType.InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));",
$""
            );
            output.EmitCode(
$"                {privateName}.Add({elementType.BuilderUnreifier("value")});",
$"            }}",
$"",
$"            public void Insert{singularName}At(int index, {elementType.TypeName} value)",
$"            {{");
            output.EmitCodeConditionally(elementType.NeedsConstructorCheck,
$"                if (!{elementType.InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));",
$""
            );
            output.EmitCode(
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

        public string BuilderReifier(string variableName)
        {
            return $"Build<{elementType.TypeName}, {elementType.BuilderTypeName}>"
                + $"({variableName}, elem => {elementType.BuilderReifier("elem")})";
        }
    }

    partial class DictionaryType
    {
        public string BuilderDeclaration(string variableName, string privateName)
        {
            return $"readonly _nsG_.SortedDictionary<{keyType.TypeName}, "
                + $"{valueType.BuilderTypeName}> {privateName};";
        }

        public string BuilderAssignment(string variableName, string privateName)
        {
            return $"{privateName} = Unbuild({variableName}, "
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
$"            {{"
            );
            output.EmitCodeConditionally(keyType.NeedsConstructorCheck,
$"                if (!{keyType.InputCheck("key")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(key));"
            );
            output.EmitCodeConditionally(valueType.NeedsConstructorCheck,
$"                if (!{valueType.InputCheck("value")})",
$"                    throw new _nsS_.ArgumentNullException(nameof(value));"
            );
            output.EmitCode(
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

        public string BuilderReifier(string variableName)
        {
            return $"Build<{keyType.TypeName}, {valueType.TypeName}, "
                + $"{valueType.BuilderTypeName}>({variableName}, "
                + $"val => {valueType.BuilderReifier("val")})";
        }
    }
}