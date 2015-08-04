using System.Collections.Generic;
using System.Linq;

namespace PackageTool
{
	class PackageManifest
	{
		public string OutputPath { get; private set; }
		public PackageManifestEntry[] Entries { get; private set; }

		public PackageManifest(string outputPath, IEnumerable<PackageManifestEntry> entries)
		{
			OutputPath = outputPath;
			Entries = entries.ToArray();
		}
	}

	class PackageManifestEntry
	{
		public string ArchivePath { get; private set; }
		public string SourceFile { get; private set; }

		public PackageManifestEntry(string archivePath, string sourceFile)
		{
			ArchivePath = archivePath;
			SourceFile = sourceFile;
		}
	}
}
