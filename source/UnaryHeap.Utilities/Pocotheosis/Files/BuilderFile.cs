using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteBuilderFile(PocoNamespace dataModel,
            string outputFileName)
        {
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
            if (clasz.Members.Count() == 0)
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
                member.WriteBuilderDeclaration(output);

            output.Write("\t\t\tpublic Builder(");
            var first = true;
            foreach (var member in clasz.Members)
            {
                if (!first)
                {
                    output.Write(", ");
                }
                first = false;

                member.WriteFormalParameter(output);
            }
            output.WriteLine(")");
            output.WriteLine("\t\t\t{");
            foreach (var member in clasz.Members)
            {
                output.Write("\t");
                member.WriteConstructorCheck(output);
            }
            foreach (var member in clasz.Members)
            {
                member.WriteBuilderAssignment(output);
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
