using System;
using System.IO;

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

            GeneratePocoSourceCode(manifestFileName, outputDirectory);
            return 0;
        }

        private static void GeneratePocoSourceCode(string manifestFileName,
            string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            using (var manifestTextReader = File.OpenText(manifestFileName))
            {
                var dataModel = PocoManifest.Parse(manifestTextReader);
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
            }
        }

        private static void GenerateDefinitionFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                dataModel.WriteNamespaceHeader(file);
                file.WriteLine("\tpublic abstract partial class Poco");
                file.WriteLine("\t{");
                file.WriteLine("\t}");
                file.WriteLine();

                bool first = true;
                foreach (var pocoClass in dataModel.Classes)
                {
                    if (!first)
                        file.WriteLine();
                    first = false;
                    pocoClass.WriteClassDeclaration(file);
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
                bool first = true;
                foreach (var pocoClass in dataModel.Classes)
                {
                    if (!first)
                        file.WriteLine();
                    first = false;
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
                bool first = true;
                foreach (var pocoClass in dataModel.Classes)
                {
                    if (!first)
                        file.WriteLine();
                    first = false;
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
                file.WriteLine("\t}");
                file.WriteLine();

                foreach (var pocoClass in dataModel.Classes)
                {
                    pocoClass.WriteSerializationImplementation(file);
                    file.WriteLine();
                }
                BoilerplateCode.WriteSerializationHelperClass(file);
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
    }
}