using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace PackageTool
{
    static class PackageManifestFile
    {
        public static PackageManifest Parse(string filename)
        {
            var doc = new XmlDocument();
            doc.Load(filename);

            return new PackageManifest(ParseOutputPath(doc), ParseEntries(doc));
        }

        static string ParseOutputPath(XmlDocument doc)
        {
            var root = (XmlElement)doc.SelectSingleNode("/ArchiveManifest");

            if (false == root.HasAttribute("OutputFileName"))
                throw new InvalidDataException(
                    "Missing 'OutputFileName' attribute on node 'ArchiveManifest'");

            return root.GetAttribute("OutputFileName");
        }

        static IEnumerable<PackageManifestEntry> ParseEntries(XmlDocument doc)
        {
            return doc.SelectNodes("/ArchiveManifest/Entry").Cast<XmlElement>()
                .Select(entry => ParseEntry(entry));
        }

        static PackageManifestEntry ParseEntry(XmlElement entry)
        {
            return new PackageManifestEntry(
                entry.GetAttribute("ArchiveName"),
                entry.GetAttribute("SourceFileName")
            );
        }
    }
}
