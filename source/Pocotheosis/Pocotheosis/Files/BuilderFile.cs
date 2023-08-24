using Pocotheosis.MemberTypes;
using System.Globalization;
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
                clasz.Members.Select(m => "{0} {1}".ICFormat(
                    m.FormalParameterType, m.PublicMemberName)));
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
$"                {member.ConstructorCheck()}"
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
            return string.Format(CultureInfo.InvariantCulture,
                "private {0} {1};",
                BuilderTypeName, privateName);
        }

        public string BuilderAssignment(string variableName, string privateName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = {1};",
                privateName, BuilderUnreifier(variableName));
        }

        public virtual void WriteBuilderPlumbing(string variableName, string singularName,
            string privateName, TextWriter output)
        {
            output.WriteLine("\t\t\tpublic " + TypeName + " " + variableName);
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tget");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\treturn " +
                BuilderReifier(privateName) + ";");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t\tset");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "\t\t\t\t\tif (!ConstructorHelper.CheckValue(value, {0})) " +
                "throw new _nsS_.ArgumentNullException(nameof(value));",
                IsNullable.ToToken()));
            output.WriteLine("\t\t\t\t\t" + privateName +
                " = " + BuilderUnreifier("value") + ";");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t}");
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
            output.WriteLine("\t\t\tpublic Builder With" + variableName +
                "(" + TypeName + " value)");
            output.WriteLine("\t\t\t{");
            output.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "\t\t\t\tif (!ConstructorHelper.CheckValue(value, {0})) " +
                "throw new _nsS_.ArgumentNullException(nameof(value));",
                IsNullable.ToToken()));
            output.WriteLine("\t\t\t\t" + privateName + " = " +
                BuilderUnreifier("value") + ";");
            output.WriteLine("\t\t\t\treturn this;");
            output.WriteLine("\t\t\t}");

            output.WriteLine("\t\t\tpublic " + BuilderTypeName + " " +
                variableName);
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tget");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\treturn " + privateName + ";");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t}");
        }

        public override string BuilderReifier(string privateName)
        {
            if (isNullable)
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} == null ? null : {0}.Build()", privateName);
            else
                return string.Format(CultureInfo.InvariantCulture,
                    "{0}.Build()", privateName);
        }

        public override string BuilderUnreifier(string variableName)
        {
            if (isNullable)
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} == null ? null : {0}.ToBuilder()", variableName);
            else
                return string.Format(CultureInfo.InvariantCulture,
                    "{0}.ToBuilder()", variableName);
        }
    }

    partial class ArrayType
    {
        public virtual string BuilderDeclaration(string variableName, string privateName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private _nsG_.IList<{0}> {1};",
                elementType.BuilderTypeName, privateName);
        }

        public virtual string BuilderAssignment(string variableName, string privateName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = BuilderHelper.UnreifyArray({1}, elem => {2});",
                privateName, variableName,
                elementType.BuilderUnreifier("elem"));
        }

        public void WriteBuilderPlumbing(string variableName, string singularName,
            string privateName, TextWriter output)
        {
            output.WriteLine(
@"            public int Num{0}
            {{
                get {{ return {1}.Count; }}
            }}
            
            public {2} Get{5}(int index)
            {{
                return {1}[index];
            }}
            
            public void Set{5}(int index, {3} value)
            {{
                if (!ConstructorHelper.CheckValue(value, {6}))
                    throw new _nsS_.ArgumentNullException(nameof(value));
                {1}[index] = {4};
            }}
            
            public void Append{5}({3} value)
            {{
                if (!ConstructorHelper.CheckValue(value, {6}))
                    throw new _nsS_.ArgumentNullException(nameof(value));
                {1}.Add({4});
            }}
            
            public void Insert{5}At(int index, {3} value)
            {{
                if (!ConstructorHelper.CheckValue(value, {6}))
                    throw new _nsS_.ArgumentNullException(nameof(value));
                {1}.Insert(index, {4});
            }}
            
            public void Remove{5}At(int index)
            {{
                {1}.RemoveAt(index);
            }}
            
            public void Clear{0}()
            {{
                {1}.Clear();
            }}
            
            public _nsG_.IEnumerable<{2}> {5}Values
            {{
                get {{ return {1}; }}
            }}",
            variableName,
            privateName,
            elementType.BuilderTypeName,
            elementType.TypeName,
            elementType.BuilderUnreifier("value"),
            singularName,
            elementType.IsNullable.ToToken());
        }

        public virtual string BuilderReifier(string variableName)
        {
            return "BuilderHelper.ReifyArray(" + variableName + ", t => " +
                elementType.BuilderReifier("t") + ")";
        }
    }

    partial class DictionaryType
    {
        public virtual string BuilderDeclaration(string variableName, string privateName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private {3}.SortedDictionary<{0}, {1}> {2};",
                keyType.TypeName, valueType.BuilderTypeName, privateName,
                "_nsG_");
        }

        public virtual string BuilderAssignment(string variableName, string privateName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = BuilderHelper.UnreifyDictionary({1}, elem => {2});",
                privateName, variableName,
                valueType.BuilderUnreifier("elem"));
        }

        public void WriteBuilderPlumbing(string variableName, string singularName,
            string privateName, TextWriter output)
        {
            output.WriteLine(
@"            public {4} Get{6}({2} key)
            {{
                return {1}[key];
            }}

            public void Set{6}({2} key, {3} value)
            {{
                if (!ConstructorHelper.CheckValue(key, {7}))
                    throw new _nsS_.ArgumentNullException(nameof(key));
                if (!ConstructorHelper.CheckValue(value, {8}))
                    throw new _nsS_.ArgumentNullException(nameof(value));
                {1}[key] = {5};
            }}

            public void Remove{6}({2} key)
            {{
                {1}.Remove(key);
            }}

            public void Clear{0}()
            {{
                {1}.Clear();
            }}

            public bool Contains{6}Key({2} key)
            {{
                return {1}.ContainsKey(key);
            }}

            public int Count{0}
            {{
                get {{ return {1}.Count; }}
            }}

            public _nsG_.IEnumerable<{2}> {6}Keys
            {{
                get {{ return {1}.Keys; }}
            }}

            public _nsG_.IEnumerable<
                _nsG_.KeyValuePair<{2}, {4}>> {6}Entries
            {{
                get {{ return {1}; }}
            }}",
            variableName,
            privateName,
            keyType.TypeName,
            valueType.TypeName,
            valueType.BuilderTypeName,
            valueType.BuilderUnreifier("value"),
            singularName,
            keyType.IsNullable.ToToken(),
            valueType.IsNullable.ToToken());
        }

        public virtual string BuilderReifier(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "BuilderHelper.ReifyDictionary({0}, t => {1})",
                variableName, valueType.BuilderReifier("t"));
        }
    }
}