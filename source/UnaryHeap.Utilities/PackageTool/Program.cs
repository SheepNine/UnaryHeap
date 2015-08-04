using System.IO;
using System.IO.Compression;

namespace PackageTool
{
	class Program
	{
		static void Main(string[] args)
		{
			var outputArchive = @"..\..\..\packages\GraphRenderer.zip";

			using (var file = File.Create(outputArchive))
			using (var archive = new ZipArchive(file, ZipArchiveMode.Create))
			{
				PopulateEntry(archive,
					@"..\..\GraphRenderer\Release\GraphRenderer.exe",
					@"GraphRenderer\GraphRenderer.exe");
				PopulateEntry(archive,
					@"..\..\GraphRenderer\Release\Newtonsoft.Json.dll",
					@"GraphRenderer\Newtonsoft.Json.dll");
				PopulateEntry(archive,
					@"..\..\GraphRenderer\Release\UnaryHeap.Utilities.dll",
					@"GraphRenderer\UnaryHeap.Utilities.dll");
			}
		}

		static void PopulateEntry(ZipArchive archive, string entryContents, string entryName)
		{
			var entry = archive.CreateEntry(entryName);

			using (var entryStream = entry.Open())
			using (var input = File.OpenRead(entryContents))
			{
				input.CopyTo(entryStream);
			}
		}
	}
}
