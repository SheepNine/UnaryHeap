using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteToStringFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using (var file = File.CreateText(outputFileName))
            {
                WriteNamespaceHeader(dataModel, file);
                WriteToStringHelperClass(file, dataModel);
                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    WriteClassToStringImplementation(pocoClass, file);
                }
                WriteNamespaceFooter(file);
            }
        }

        static void WriteClassToStringImplementation(PocoClass clasz, TextWriter output)
        {
            output.EmitCode(
$"    public partial class {clasz.Name}",
@"    {
        public override string ToString()
        {
            return ToString(global::System.Globalization.CultureInfo.InvariantCulture);
        }

        public string ToString(global::System.IFormatProvider formatProvider)
        {
            using (global::System.IO.StringWriter target =
                    new global::System.IO.StringWriter(formatProvider))
            {
                using (var textWriter = new TextWriterIndenter(target))
                {
                    WriteIndented(textWriter);
                    textWriter.Flush();
                }
                return target.ToString();
            }
        }
"
            );
            output.EmitCodeConditionally(!clasz.Members.Any(),
$"        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(",
$"            \"Performance\", \"CA1822:Mark members as static\")]"
            );
            output.EmitCode(
$"        public void WriteIndented(TextWriterIndenter target)",
$"        {{"
            );
            if (!clasz.Members.Any()) output.EmitCodeConditionally(!clasz.Members.Any(),
$"            target.Write(\"{{ }}\");"
            );
            else
            {
                output.EmitCodeConditionally(clasz.Members.Any(),
$"            target.WriteLine(\"{{\");",
$"            target.IncreaseIndent();"
                );
                foreach (var member in clasz.Members) output.EmitCode(
$"            target.Write(\"{member.PublicMemberName()} = \");",
$"            {member.ToStringer()}",
$"            target.WriteLine();"
                );
                output.EmitCode(
$"            target.DecreaseIndent();",
$"            target.Write(\"}}\");"
                );
            }
            output.EmitCode(
$"        }}",
$"    }}"
            );
        }

        static void WriteToStringHelperClass(TextWriter output,
            PocoNamespace dataModel)
        {
            output.EmitCode(
@"    public class TextWriterIndenter : global::System.IDisposable
    {
        public string IndentString { get; set; }

        bool atStartOfLine = true;
        int indentLevel;
        global::System.IO.TextWriter target;

        public TextWriterIndenter(global::System.IO.TextWriter target)
        {
            this.target = target;
            IndentString = ""\t"";
        }

        public void Dispose()
        {
            Dispose(true);
            global::System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (target != null)
                {
                    target.Dispose();
                    target = null;
                }
            }
        }

        public void Flush()
        {
            target.Flush();
        }

        public void IncreaseIndent()
        {
            if (indentLevel < 1024)
            indentLevel += 1;
        }

        public void DecreaseIndent()
        {
            if (indentLevel > 0)
                indentLevel -= 1;
        }

        void WriteIndentIfRequired()
        {
            if (!atStartOfLine) return;

            for (var i = 0; i < indentLevel; i++)
                target.Write(IndentString);

            atStartOfLine = false;
        }

        public void WriteLine()
        {
            WriteLine("""");
        }

        public void WriteLine(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                WriteIndentIfRequired();
                target.Write(value);
            }

            target.WriteLine();
            atStartOfLine = true;
        }

        public void Write(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                WriteIndentIfRequired();
                target.Write(value);
            }
        }

        public void Write(bool value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        public void Write(byte value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        public void Write(sbyte value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        public void Write(ushort value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        public void Write(short value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        public void Write(uint value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        public void Write(int value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        public void Write(ulong value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        public void Write(long value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }
    }");
        }
    }
}
