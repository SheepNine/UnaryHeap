using System.IO;

namespace Pocotheosis
{
    static partial class Generator
    {
        static bool OutputUpToDate(PocoNamespace ns, string filename)
        {
            var info = new FileInfo(filename);
            return info.Exists && info.LastWriteTimeUtc >= ns.LastWriteTimeUtc;
        }

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
