using System.IO;

namespace Pocotheosis
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataModel = new PocoNamespace("NAMESPAAAACE", new[] {
                new PocoClass()
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
