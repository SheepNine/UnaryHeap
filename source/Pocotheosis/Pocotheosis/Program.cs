using System;
using System.IO;

namespace Pocotheosis
{
    sealed class Program
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

            if (!File.Exists(manifestFileName))
            {
                Console.Error.WriteLine("Manifest file not found");
                return 1;
            }

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
                PocoNamespace dataModel = ManifestParser.Parse(manifestTextReader,
                    new FileInfo(manifestFileName).LastWriteTimeUtc);
                Generator.WriteDefinitionFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_Definition.cs"));
                Generator.WriteEquatableFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_Equatable.cs"));
                Generator.WriteToStringFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_ToString.cs"));
                Generator.WriteSerializationFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_Serialization.cs"));
                Generator.WriteStreamingFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_Streaming.cs"));
                Generator.WriteNetworkingClientFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_NetClient.cs"));
                Generator.WriteNetworkingServerFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_NetServer.cs"));
                Generator.WriteBuilderFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_Builders.cs"));
                Generator.WriteJsonSerializationFile(dataModel,
                    Path.Combine(outputDirectory, "Pocos_JsonSerialization.cs"));
            }
        }
    }
}