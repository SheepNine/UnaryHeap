using System.IO;

namespace Pocotheosis
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataModel = new PocoNamespace("NAMESPAAAACE", new[] {
                new PocoClass("POCO1", new PocoMember[] {
                    new Int32PocoMember("VAR1"),
                    new Int64PocoMember("VAR2"),
                })
            });

            using (var file = File.CreateText("Pocos.cs"))
            {
                dataModel.WriteNamespaceHeader(file);
                foreach (var pocoClass in dataModel.Classes)
                    pocoClass.WriteClassDeclaration(file);
                dataModel.WriteNamespaceFooter(file);
            }
        }
    }
}
