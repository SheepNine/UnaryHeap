using System;
using System.IO;

namespace PackageTool
{
    class Program
    {
        static int Main(string[] args)
        {
            if (1 != args.Length)
            {
                Console.Error.WriteLine("Incorrect usage.");
                return 1;
            }
            else
            {
                try
                {
                    Execute(Path.GetFullPath(args[0]));
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("FAILURE: " + ex.Message);
                    return 1;
                }
            }
        }

        static void Execute(string manifestFile)
        {
            Packager.GeneratePackage(Path.GetDirectoryName(manifestFile),
                PackageManifestFile.Parse(manifestFile));
        }
    }
}
