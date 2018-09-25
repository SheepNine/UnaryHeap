using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pocotheosis
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length > 2)
            {
                Console.Error.WriteLine(
                    "Usage: pocotheosis [input manifest file] [output directory]");
                return 1;
            }

            var manifestFileName = Path.GetFullPath(
                Path.Combine(Environment.CurrentDirectory, "manifest.xml"));
            var outputDirectory = Path.GetFullPath(
                Path.Combine(Environment.CurrentDirectory, "pocos"));

            if (args.Length > 0)
                manifestFileName = Path.GetFullPath(args[0]);
            if (args.Length > 1)
                outputDirectory = Path.GetFullPath(args[1]);

            try
            {
                GeneratePocoSourceCode(manifestFileName, outputDirectory);
                return 0;
            }
            catch (InvalidDataException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }

        private static void GeneratePocoSourceCode(string manifestFileName,
            string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            using (var manifestTextReader = File.OpenText(manifestFileName))
            {
                PocoNamespace dataModel = PocoManifest.Parse(manifestTextReader);
                GenerateDefinitionFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_Definition.cs"));
                GenerateEquatableFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_Equatable.cs"));
                GenerateToStringFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_ToString.cs"));
                GenerateSerializationFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_Serialization.cs"));
                GenerateStreamFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_Streaming.cs"));
                GenerateNetworkingClientFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_NetClient.cs"));
                GenerateNetworkingServerFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_NetServer.cs"));
                GenerateBuilderFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_Builders.cs"));
            }
        }

        private static void GenerateDefinitionFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                dataModel.WriteNamespaceHeader(file);
                BoilerplateCode.WriteConstructorHelperClass(file, dataModel);

                file.WriteLine("\tpublic abstract partial class Poco");
                file.WriteLine("\t{");
                file.WriteLine("\t}");

                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    pocoClass.WriteClassDeclaration(file);
                }

                foreach (var pocoEnum in dataModel.Enums)
                {
                    file.WriteLine();
                    pocoEnum.WriteEnumDeclaration(file);
                }
                dataModel.WriteNamespaceFooter(file);
            }
        }

        private static void GenerateEquatableFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                dataModel.WriteNamespaceHeader(file);
                BoilerplateCode.WriteEqualityHelperClass(file, dataModel);
                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    pocoClass.WriteClassEqualityDeclaration(file);
                }
                dataModel.WriteNamespaceFooter(file);
            }
        }

        private static void GenerateToStringFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                dataModel.WriteNamespaceHeader(file);
                BoilerplateCode.WriteToStringHelperClass(file, dataModel);
                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    pocoClass.WriteClassToStringImplementation(file);
                }
                dataModel.WriteNamespaceFooter(file);
            }
        }

        private static void GenerateSerializationFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                dataModel.WriteNamespaceHeader(file);
                file.WriteLine("\tpublic abstract partial class Poco");
                file.WriteLine("\t{");
                file.WriteLine("\t\tpublic abstract void Serialize(" +
                    "global::System.IO.Stream output);");
                file.WriteLine();
                file.WriteLine("\t\tpublic global::System.Guid Md5Checksum");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\tget");
                file.WriteLine("\t\t\t{");
                file.WriteLine("\t\t\t\tvar buffer = new global::System.IO.MemoryStream();");
                file.WriteLine("\t\t\t\tSerialize(buffer);");
                file.WriteLine("\t\t\t\tusing (var md5 = " +
                    "global::System.Security.Cryptography.MD5.Create())");
                file.WriteLine("\t\t\t\t\treturn new global::System.Guid(" +
                    "md5.ComputeHash(buffer.ToArray()));");
                file.WriteLine("\t\t\t\t}");
                file.WriteLine("\t\t\t}");
                file.WriteLine("\t}");
                file.WriteLine();

                foreach (var pocoClass in dataModel.Classes)
                {
                    pocoClass.WriteSerializationImplementation(file);
                    file.WriteLine();
                }
                BoilerplateCode.WriteSerializationHelperClass(file, dataModel);
                dataModel.WriteNamespaceFooter(file);
            }
        }

        private static void GenerateStreamFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                dataModel.WriteNamespaceHeader(file);
                BoilerplateCode.WriteStreamingCommonClasses(file);
                file.WriteLine();
                BoilerplateCode.WriteStreamingBaseClass(file, dataModel);

                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    pocoClass.WriteClassStreamingImplementation(file);
                }

                dataModel.WriteNamespaceFooter(file);
            }
        }

        private static void GenerateNetworkingClientFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                dataModel.WriteNamespaceHeader(file);
                BoilerplateCode.WriteNetworkingClientClasses(file);
                dataModel.WriteNamespaceFooter(file);
            }
        }

        private static void GenerateNetworkingServerFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                dataModel.WriteNamespaceHeader(file);
                BoilerplateCode.WriteNetworkingServerClasses(file);
                dataModel.WriteNamespaceFooter(file);
            }
        }

        private static void GenerateBuilderFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                dataModel.WriteNamespaceHeader(file);

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
                {
                    clasz.WriteBuilderImplementation(file);
                }
                dataModel.WriteNamespaceFooter(file);
            }
        }
    }
}