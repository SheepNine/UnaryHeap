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
				PackageManifestFile.Parse("notusedyet"));
		}
	}
}
