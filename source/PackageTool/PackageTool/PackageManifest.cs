using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PackageTool
{
    sealed class PackageManifest
    {
        public string OutputFileName { get; private set; }
        public PackageManifestEntry[] Entries { get; private set; }

        public PackageManifest(string outputFileName, IEnumerable<PackageManifestEntry> entries)
        {
            OutputFileName = outputFileName;
            Entries = entries.ToArray();
        }
    }

    sealed class PackageManifestEntry
    {
        public string ArchivePath { get; private set; }
        public string SourceFile { get; private set; }

        public PackageManifestEntry(string archivePath, string sourceFile)
        {
            ArchivePath = archivePath;
            SourceFile = sourceFile;

            if (string.IsNullOrEmpty(ArchivePath))
                throw new InvalidDataException("Entry with blank ArchivePath found");
            if (string.IsNullOrEmpty(SourceFile))
                throw new InvalidDataException("Entry with blank SourceFile found");
        }
    }
}
