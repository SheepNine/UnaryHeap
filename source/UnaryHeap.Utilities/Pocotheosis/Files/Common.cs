using System.IO;

namespace Pocotheosis
{
    static partial class Generator
    {
        static void WriteNamespaceHeader(PocoNamespace ns, TextWriter output)
        {
            output.WriteLine("namespace " + ns.Name);
            output.WriteLine("{");
        }

        static void WriteNamespaceFooter(TextWriter output)
        {
            output.WriteLine("}");
        }
    }
}
