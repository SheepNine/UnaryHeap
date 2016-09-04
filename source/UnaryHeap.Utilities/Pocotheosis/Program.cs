using System.IO;

namespace Pocotheosis
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataModel = new PocoNamespace("NAMESPAAAACE", new[] {
                new PocoClass("POCO1", new PocoMember[] {
                    new BoolPocoMember("BOO"),
                    new BytePocoMember("AA"),
                    new Int16PocoMember("BB"),
                    new Int32PocoMember("CC"),
                    new Int64PocoMember("DD"),
                    new SBytePocoMember("EE"),
                    new UInt16PocoMember("FF"),
                    new UInt32PocoMember("GG"),
                    new UInt64PocoMember("HH"),
                    new StringPocoMember("II")
                })
            });

            using (var file = File.CreateText("Pocos.cs"))
            {
                dataModel.WriteNamespaceHeader(file);
                foreach (var pocoClass in dataModel.Classes)
                    pocoClass.WriteClassDeclaration(file);
                dataModel.WriteNamespaceFooter(file);
            }

            using (var file = File.CreateText("Pocos_Equatable.cs"))
            {
                dataModel.WriteNamespaceHeader(file);
                foreach (var pocoClass in dataModel.Classes)
                    pocoClass.WriteClassEqualityDeclaration(file);
                dataModel.WriteNamespaceFooter(file);
            }

            using (var file = File.CreateText("Pocos_ToString.cs"))
            {
                dataModel.WriteNamespaceHeader(file);
                foreach (var pocoClass in dataModel.Classes)
                    pocoClass.WriteClassToStringImplementation(file);
                dataModel.WriteNamespaceFooter(file);
            }
        }
    }
}