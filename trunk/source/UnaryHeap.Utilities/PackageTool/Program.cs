using System.IO;

namespace PackageTool
{
	class Program
	{
		static void Main(string[] args)
		{
			var manifestFile = Path.GetFullPath(@"..\..\..\..\packages\GraphRenderer.xml");

			Packager.GeneratePackage(Path.GetDirectoryName(manifestFile), PackageManifestFile.Parse(manifestFile));
		}
	}
}
