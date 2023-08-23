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

                file.WriteLine(@"    class BuilderHelper
    {
        public static global::System.Collections.Generic.IList<TBuilder>
            UnreifyArray<TBase, TBuilder>(
            global::System.Collections.Generic.IEnumerable<TBase> values,
            global::System.Func<TBase, TBuilder> unreifier)
        {
            return global::System.Linq.Enumerable.ToList(
                global::System.Linq.Enumerable.Select(values, unreifier));
        }
        public static global::System.Collections.Generic.IEnumerable<TBase>
            ReifyArray<TBase, TBuilder>(
            global::System.Collections.Generic.IEnumerable<TBuilder> values,
            global::System.Func<TBuilder, TBase> reifier)
        {
            return global::System.Linq.Enumerable.Select(values, reifier);
        }
        public static global::System.Collections.Generic.SortedDictionary<TKey, TBuilder>
            UnreifyDictionary<TKey, TBase, TBuilder>(
            global::System.Collections.Generic.IDictionary<TKey, TBase> values,
            global::System.Func<TBase, TBuilder> unreifier)
        {
            return new global::System.Collections.Generic.SortedDictionary<TKey, TBuilder>(
                global::System.Linq.Enumerable.ToDictionary(
                    values, pair => pair.Key, pair => unreifier(pair.Value)));
        }
        public static global::System.Collections.Generic.IDictionary<TKey, TBase>
            ReifyDictionary<TKey, TBuilder, TBase>(
            global::System.Collections.Generic.IDictionary<TKey, TBuilder> values,
            global::System.Func<TBuilder, TBase> reifier)
        {
            return global::System.Linq.Enumerable.ToDictionary(
                values, pair => pair.Key, pair => reifier(pair.Value));
        }
    }");

                foreach (var clasz in dataModel.Classes)
                    WriteBuilderImplementation(clasz, file);

                WriteNamespaceFooter(file);
            }
        }

        static void WriteBuilderImplementation(PocoClass clasz, TextWriter output)
        {
            if (!clasz.Members.Any())
                return;

            output.WriteLine("\tpublic partial class " + clasz.Name);
            output.WriteLine("\t{");
            output.WriteLine("\t\tpublic Builder ToBuilder()");
            output.WriteLine("\t\t{");
            output.Write("\t\t\treturn new Builder(");
            output.Write(string.Join(", ", clasz.Members.Select(m => m.BackingStoreName())));
            output.WriteLine(");");
            output.WriteLine("\t\t}");

            output.WriteLine("\t\tpublic class Builder");
            output.WriteLine("\t\t{");
            foreach (var member in clasz.Members)
            {
                output.Write("\t\t\t");
                output.WriteLine(member.BuilderDeclaration());
            }

            output.Write("\t\t\tpublic Builder(");
            var first = true;
            foreach (var member in clasz.Members)
            {
                if (!first)
                {
                    output.Write(", ");
                }
                first = false;

                output.Write(member.FormalParameter());
            }
            output.WriteLine(")");
            output.WriteLine("\t\t\t{");
            foreach (var member in clasz.Members)
            {
                output.Write("\t\t\t\t");
                output.WriteLine(member.ConstructorCheck());
            }
            foreach (var member in clasz.Members)
            {
                output.Write("\t\t\t\t");
                output.WriteLine(member.BuilderAssignment());
            }
            output.WriteLine("\t\t\t}");


            output.WriteLine("\t\t\tpublic " + clasz.Name + " Build()");
            output.WriteLine("\t\t\t{");
            output.Write("\t\t\t\t return new " + clasz.Name + "(");
            output.Write(string.Join(", ", clasz.Members.Select(m => m.BuilderReifier())));
            output.WriteLine(");");
            output.WriteLine("\t\t\t}");
            foreach (var member in clasz.Members)
            {
                member.WriteBuilderPlumbing(output);
            }
            output.WriteLine("\t\t}");
            output.WriteLine("\t}");
        }
    }
}

namespace Pocotheosis.MemberTypes
{
    public partial class PrimitiveType
    {
        public string BuilderDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private {0} {1};",
                BuilderTypeName, BackingStoreName(variableName));
        }

        public string BuilderAssignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = {1};",
                BackingStoreName(variableName), BuilderUnreifier(TempVarName(variableName)));
        }

        public virtual void WriteBuilderPlumbing(string variableName, string singularName,
            TextWriter output)
        {
            output.WriteLine("\t\t\t// --- " + variableName + " ---");

            output.WriteLine("\t\t\tpublic " + TypeName + " " + PublicMemberName(variableName));
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tget");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\treturn " +
                BuilderReifier(BackingStoreName(variableName)) + ";");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t\tset");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "\t\t\t\t\tif (!ConstructorHelper.CheckValue(value, {0})) " +
                "throw new global::System.ArgumentNullException(nameof(value));",
                IsNullable.ToToken()));
            output.WriteLine("\t\t\t\t\t" + BackingStoreName(variableName) +
                " = " + BuilderUnreifier("value") + ";");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t}");
        }

        public virtual string BuilderReifier(string variableName)
        {
            return variableName;
        }

        public virtual string BuilderUnreifier(string variableName)
        {
            return variableName;
        }
    }

    public partial class ClassType
    {
        public override void WriteBuilderPlumbing(string variableName, string singularName,
            TextWriter output)
        {
            output.WriteLine("\t\t\t// --- " + variableName + " ---");

            output.WriteLine("\t\t\tpublic Builder With" + PublicMemberName(variableName) +
                "(" + TypeName + " value)");
            output.WriteLine("\t\t\t{");
            output.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "\t\t\t\tif (!ConstructorHelper.CheckValue(value, {0})) " +
                "throw new global::System.ArgumentNullException(nameof(value));",
                IsNullable.ToToken()));
            output.WriteLine("\t\t\t\t" + BackingStoreName(variableName) + " = " +
                BuilderUnreifier("value") + ";");
            output.WriteLine("\t\t\t\treturn this;");
            output.WriteLine("\t\t\t}");

            output.WriteLine("\t\t\tpublic " + BuilderTypeName + " " +
                PublicMemberName(variableName));
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tget");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\treturn " + BackingStoreName(variableName) + ";");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t}");
        }

        public override string BuilderReifier(string variableName)
        {
            if (isNullable)
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} == null ? null : {0}.Build()", variableName);
            else
                return string.Format(CultureInfo.InvariantCulture,
                    "{0}.Build()", variableName);
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
        public virtual string BuilderDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private global::System.Collections.Generic.IList<{0}> {1};",
                elementType.BuilderTypeName, BackingStoreName(variableName));
        }

        public virtual string BuilderAssignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = BuilderHelper.UnreifyArray({1}, t => {2});",
                BackingStoreName(variableName), TempVarName(variableName),
                elementType.BuilderUnreifier("t"));
        }

        public void WriteBuilderPlumbing(string variableName, string singularName,
            TextWriter output)
        {
            output.WriteLine(@"            //{0}
            public int Num{0}
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
                    throw new global::System.ArgumentNullException(nameof(value));
                {1}[index] = {4};
            }}
            
            public void Append{5}({3} value)
            {{
                if (!ConstructorHelper.CheckValue(value, {6}))
                    throw new global::System.ArgumentNullException(nameof(value));
                {1}.Add({4});
            }}
            
            public void Insert{5}At(int index, {3} value)
            {{
                if (!ConstructorHelper.CheckValue(value, {6}))
                    throw new global::System.ArgumentNullException(nameof(value));
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
            
            public global::System.Collections.Generic.IEnumerable<{2}> {5}Values
            {{
                get {{ return {1}; }}
            }}",
            PublicMemberName(variableName),
            BackingStoreName(variableName),
            elementType.BuilderTypeName,
            elementType.TypeName,
            elementType.BuilderUnreifier("value"),
            PublicMemberName(singularName),
            elementType.IsNullable.ToToken());
        }
    }

    partial class DictionaryType
    {
        public virtual string BuilderDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private {3}.SortedDictionary<{0}, {1}> {2};",
                keyType.TypeName, valueType.BuilderTypeName, BackingStoreName(variableName),
                "global::System.Collections.Generic");
        }

        public virtual string BuilderAssignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = BuilderHelper.UnreifyDictionary({1}, t => {2});",
                BackingStoreName(variableName), TempVarName(variableName),
                valueType.BuilderUnreifier("t"));
        }

        public void WriteBuilderPlumbing(string variableName, string singularName,
            TextWriter output)
        {
            output.WriteLine(@"            // {0}
            public {4} Get{6}({2} key)
            {{
                return {1}[key];
            }}

            public void Set{6}({2} key, {3} value)
            {{
                if (!ConstructorHelper.CheckValue(key, {7}))
                    throw new global::System.ArgumentNullException(nameof(key));
                if (!ConstructorHelper.CheckValue(value, {8}))
                    throw new global::System.ArgumentNullException(nameof(value));
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

            public global::System.Collections.Generic.IEnumerable<{2}> {6}Keys
            {{
                get {{ return {1}.Keys; }}
            }}

            public global::System.Collections.Generic.IEnumerable<
                global::System.Collections.Generic.KeyValuePair<{2}, {4}>> {6}Entries
            {{
                get {{ return {1}; }}
            }}",
            PublicMemberName(variableName),
            BackingStoreName(variableName),
            keyType.TypeName,
            valueType.TypeName,
            valueType.BuilderTypeName,
            valueType.BuilderUnreifier("value"),
            PublicMemberName(singularName),
            keyType.IsNullable.ToToken(),
            valueType.IsNullable.ToToken());
        }
    }
}