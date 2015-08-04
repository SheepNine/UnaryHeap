using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace PackageTool
{
	class Program
	{
		static void Main(string[] args)
		{
			Packager.GeneratePackage(Environment.CurrentDirectory,
				new PackageManifest(
				@"..\..\..\packages\GraphRenderer.zip",
				new[] {
					new PackageManifestEntry(@"GraphRenderer\GraphRenderer.exe", @"..\..\GraphRenderer\Release\GraphRenderer.exe"),
					new PackageManifestEntry(@"GraphRenderer\Newtonsoft.Json.dll", @"..\..\GraphRenderer\Release\Newtonsoft.Json.dll"),
					new PackageManifestEntry(@"GraphRenderer\UnaryHeap.Utilities.dll", @"..\..\GraphRenderer\Release\UnaryHeap.Utilities.dll"),
				}));
		}
	}
}
