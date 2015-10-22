using System.Collections.Generic;
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
            return ((XmlElement)doc.SelectSingleNode("/ArchiveManifest"))
                .GetAttribute("OutputFileName");
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
