using System.IO;

namespace Pocotheosis
{
    class Program
    {
        static void Main(string[] args)
        {
            PocoNamespace dataModel;
            using (var input = File.OpenText("demo_manifest.xml"))
            {
                dataModel = PocoManifest.Parse(input);
            }

            using (var file = File.CreateText("Pocos.cs"))
            {
                dataModel.WriteNamespaceHeader(file);
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

            using (var file = File.CreateText("Pocos_Equatable.cs"))
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

            using (var file = File.CreateText("Pocos_ToString.cs"))
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

            using (var file = File.CreateText("Pocos_Serialization.cs"))
            {
                dataModel.WriteNamespaceHeader(file);
                foreach (var pocoClass in dataModel.Classes)
                {
                    pocoClass.WriteSerializationImplementation(file);
                    file.WriteLine();
                }
                BoilerplateCode.WriteSerializationHelperClass(file);
                dataModel.WriteNamespaceFooter(file);
            }
        }
    }
}