using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;

namespace PackageTool
{
	static class PackageManifestFile
	{
		const string example =
@"<PackageManifest OutputPath=""..\..\..\packages\GraphRenderer.zip"">
	<Entry ArchivePath=""GraphRenderer\GraphRenderer.exe"" SourceFile=""..\..\GraphRenderer\Release\GraphRenderer.exe""/>
	<Entry ArchivePath=""GraphRenderer\Newtonsoft.Json.dll"" SourceFile=""..\..\GraphRenderer\Release\Newtonsoft.Json.dll""/>
	<Entry ArchivePath=""GraphRenderer\UnaryHeap.Utilities.dll"" SourceFile=""..\..\GraphRenderer\Release\UnaryHeap.Utilities.dll""/>
</PackageManifest>
";
		public static PackageManifest Parse(string filename)
		{
			var doc = new XmlDocument();
			using (var reader = new StringReader(example))
				doc.Load(reader);

			return new PackageManifest(
				ParseOutputPath(doc),
				ParseEntries(doc));
		}

		private static string ParseOutputPath(XmlDocument doc)
		{
			return ((XmlElement)doc.SelectSingleNode("/PackageManifest")).GetAttribute("OutputPath");
		}

		private static IEnumerable<PackageManifestEntry> ParseEntries(XmlDocument doc)
		{
			return doc.SelectNodes("/PackageManifest/Entry").Cast<XmlElement>().Select(entry => ParseEntry(entry));
		}

		private static PackageManifestEntry ParseEntry(XmlElement entry)
		{
			return new PackageManifestEntry(
				entry.GetAttribute("ArchivePath"),
				entry.GetAttribute("SourceFile")
			);
		}
	}
}
