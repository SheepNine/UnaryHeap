using System.IO;

namespace Pocotheosis
{
    static class Extensions
    {
        public static string ToToken(this bool value)
        {
            return value ? "true" : "false";
        }

        public static void EmitCode(this TextWriter output, params string[] lines)
        {
            foreach (var line in lines)
                output.WriteLine(line.Replace("    ", "\t"));
        }

        public static void EmitCodeConditionally(this TextWriter output, bool condition,
            params string[] lines)
        {
            if (condition)
                output.EmitCode(lines);
        }
    }
}
