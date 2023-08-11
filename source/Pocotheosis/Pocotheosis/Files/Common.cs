﻿using System.IO;

namespace Pocotheosis
{
    static partial class Generator
    {
        static bool OutputUpToDate(PocoNamespace ns, string filename)
        {
            var info = new FileInfo(filename);
            return info.Exists && info.LastWriteTimeUtc >= ns.LastWriteTimeUtc;
        }

        static void WriteNamespaceHeader(PocoNamespace ns, TextWriter output)
        {
            var version = typeof(Program).Assembly.GetName().Version;
            output.WriteLine(
@"// ================================================================================
// This file was automatically generated by Pocotheosis version {0}.
// DO NOT EDIT THIS FILE DIRECTLY! Edit the manifest file and regenerate it instead
// ================================================================================

namespace {1}
{{", version, ns.Name);
        }

        static void WriteNamespaceFooter(TextWriter output)
        {
            output.WriteLine("}");
        }
    }
}
