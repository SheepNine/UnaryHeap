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
                WriteNamespaceHeader(dataModel, file,
                    new[] { "_nsS_", "_nsI_" });
                WriteToStringHelperClass(file, dataModel);
                foreach (var pocoClass in dataModel.Classes)
                    WriteClassToStringImplementation(pocoClass, file);
                WriteNamespaceFooter(file);
            }
        }

        static void WriteToStringHelperClass(TextWriter output,
            PocoNamespace dataModel)
        {
            output.EmitCode(
@"    public class TextWriterIndenter : _nsS_.IDisposable
    {
        public string IndentString { get; set; }

        bool atStartOfLine = true;
        int indentLevel;
        _nsI_.TextWriter target;

        public TextWriterIndenter(_nsI_.TextWriter target)
        {
            this.target = target;
            IndentString = ""\t"";
        }

        public void Dispose()
        {
            Dispose(true);
            _nsS_.GC.SuppressFinalize(this);
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

        static void WriteClassToStringImplementation(PocoClass clasz, TextWriter output)
        {
            output.EmitCode(
$"",
$"    public partial class {clasz.Name}",
@"    {
        public override string ToString()
        {
            return ToString(_nsS_.Globalization.CultureInfo.InvariantCulture);
        }

        public string ToString(_nsS_.IFormatProvider formatProvider)
        {
            using (var target = new _nsI_.StringWriter(formatProvider))
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
$"        [_nsS_.Diagnostics.CodeAnalysis.SuppressMessage(",
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
$"            target.Write(\"{member.PublicMemberName} = \");",
$"            {member.ToStringOutput()}",
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
    }
}

namespace Pocotheosis.MemberTypes
{
    partial class PrimitiveType
    {
        public virtual string ToStringOutput(string privateName)
        {
            return $"target.Write({privateName});";
        }
    }

    partial class StringType
    {
        public override string ToStringOutput(string variableName)
        {
            return $"target.Write({variableName} == null "
                + $"? \"null\" : $\"'{{{variableName}}}'\");";
        }
    }

    partial class EnumType
    {
        public override string ToStringOutput(string variableName)
        {
            return $"target.Write({variableName}.ToString());";
        }
    }

    partial class ClassType
    {
        public override string ToStringOutput(string privateName)
        {
            if (isNullable)
            {
                return $"if ({privateName} == null) {{ target.Write(\"null\"); }} " +
                    $"else {{ {privateName}.WriteIndented(target); }}";
            }
            else
                return  $"{privateName}.WriteIndented(target);";
        }
    }

    partial class ArrayType
    {
        public string ToStringOutput(string privateName)
        {
            return @"{
                target.Write(""["");
                var separator = """";
                foreach (var iter in " + privateName + @")
                {
                    target.Write(separator);
                    separator = "", "";
                    " + elementType.ToStringOutput("iter") + @"
                }
                target.Write(""]"");
            }";
        }
    }

    partial class DictionaryType
    {
        public string ToStringOutput(string privateName)
        {
            return @"{
                target.Write(""("");
                target.IncreaseIndent();
                var separator = """";
                foreach (var iter in " + privateName + @")
                {
                    target.Write(separator);
                    separator = "","";
                    target.WriteLine();
                    " + keyType.ToStringOutput("iter.Key") + @"
                    target.Write("" -> "");
                    " + valueType.ToStringOutput("iter.Value") + @"
                }
                target.DecreaseIndent();
                if (" + privateName + @".Count > 0)
                    target.WriteLine();
                target.Write("")"");
            }";
        }
    }
}