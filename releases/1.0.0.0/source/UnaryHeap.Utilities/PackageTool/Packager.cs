using System.IO;
using System.IO.Compression;

namespace PackageTool
{
    static class Packager
    {
        public static void GeneratePackage(string relativeRoot, PackageManifest manifest)
        {
            var outputFileName = Path.GetFullPath(Path.Combine(relativeRoot, manifest.OutputFileName));
            Directory.CreateDirectory(Path.GetDirectoryName(outputFileName));

            using (var file = File.Create(outputFileName))
            using (var archive = new ZipArchive(file, ZipArchiveMode.Create))
                foreach (var entry in manifest.Entries)
                    PopulateEntry(archive, entry.ArchivePath,
                        Path.GetFullPath(Path.Combine(relativeRoot, entry.SourceFile)));
        }

        static void PopulateEntry(ZipArchive archive, string entryName, string contentsFileName)
        {
            var entry = archive.CreateEntry(entryName);

            using (var entryStream = entry.Open())
            using (var input = File.OpenRead(contentsFileName))
                input.CopyTo(entryStream);
        }
    }
}
