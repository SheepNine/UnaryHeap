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

            using var file = File.CreateText(outputFileName);
            WriteNamespaceHeader(dataModel, file,
                ["_nsS_", "_nsI_", "_nsG_", "_nsGl_"]);
            WriteToStringHelperClass(file, dataModel);
            foreach (var pocoClass in dataModel.Classes)
                WriteClassToString(pocoClass, file);
            WriteNamespaceFooter(file);
        }

        static void WriteToStringHelperClass(TextWriter output,
            PocoNamespace dataModel)
        {
            output.EmitCode(
@"    partial class Poco
    {
        public class TextWriterIndenter : _nsS_.IDisposable
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
        }

        protected static string ToString<T>(T poco, _nsS_.Action<T, TextWriterIndenter> action)
        {
            using (var target = new _nsI_.StringWriter(_nsGl_.CultureInfo.InvariantCulture))
            {
                using (var textWriter = new TextWriterIndenter(target))
                {
                    action(poco, textWriter);
                    textWriter.Flush();
                }
                return target.ToString();
            }
        }

        protected static void WriteIndented<T>(_nsG_.IEnumerable<T> data,
                _nsS_.Action<T, TextWriterIndenter> elementWriter, TextWriterIndenter target)
        {
            target.Write(""["");
            var separator = string.Empty;
            foreach (var datum in data)
            {
                target.Write(separator);
                separator = "", "";
                elementWriter(datum, target);
            }
            target.Write(""]"");
        }

        protected static void WriteIndented<TKey, TValue>(
                _nsG_.IReadOnlyDictionary<TKey, TValue> data,
                _nsS_.Action<TKey, TextWriterIndenter> keyWriter,
                _nsS_.Action<TValue, TextWriterIndenter> valueWriter,
                TextWriterIndenter target)
        {
            target.Write(""("");
            target.IncreaseIndent();
            var separator = string.Empty;
            foreach (var iter in data)
            {
                target.Write(separator);
                separator = "","";
                target.WriteLine();
                keyWriter(iter.Key, target);
                target.Write("" -> "");
                valueWriter(iter.Value, target);
            }
            target.DecreaseIndent();
            if (data.Count > 0)
                target.WriteLine();
            target.Write("")"");
        }

        protected static void WriteIndented(string value, TextWriterIndenter target)
        {
            if (value == null)
                target.Write(""null"");
            else
                target.Write($""'{value}'"");
        }

        protected static void WriteIndented(IPoco value, TextWriterIndenter target)
        {
            if (value == null) target.Write(""null"");");
            foreach (var clasz in dataModel.Classes) output.EmitCode(
$"            if (value is {clasz.Name}) WriteIndented(value as {clasz.Name}, target);"
            );
            output.EmitCode(
@"        }
"
            );

            foreach (var primitive in new[] {
                "bool", "byte", "short", "int", "long", "sbyte", "ushort", "uint", "ulong" }
            ) output.EmitCode(
$"",
$"        protected static void WriteIndented({primitive} value, TextWriterIndenter target)",
$"        {{",
$"            target.Write(value);",
$"        }}"
            );

            foreach (var enume in dataModel.Enums) output.EmitCode(
$"",
$"        protected static void WriteIndented({enume.Name} value, TextWriterIndenter target)",
$"        {{",
$"            target.Write(value.ToString());",
$"        }}"
            );

            foreach (var clasz in dataModel.Classes)
            {
                output.EmitCode(
$"",
$"        protected static void WriteIndented({clasz.Name} poco, TextWriterIndenter target)",
$"        {{",
$"            if (poco == null)",
$"            {{",
$"                target.Write(\"null\");",
$"                return;",
$"            }}",
$""
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
$"            {member.ToStringOutput()};",
$"            target.WriteLine();"
                    );
                    output.EmitCode(
$"            target.DecreaseIndent();",
$"            target.Write(\"}}\");"
                    );
                }
                output.EmitCode(
$"        }}"
                );
            }
            output.EmitCode(
"    }"
            );
        }

        static void WriteClassToString(PocoClass clasz, TextWriter output)
        {
            output.EmitCode(
$"",
$"    public partial class {clasz.Name}",
$"    {{"
            );
            WriteClassToStringContents(output);
            output.EmitCode(
$"    }}"
            );
        }

        public static void WriteClassToStringContents(TextWriter output)
        {
            output.EmitCode(
$"        public override string ToString()",
$"        {{",
$"            return ToString(this, WriteIndented);",
$"        }}"
            );
        }
    }
}

namespace Pocotheosis.MemberTypes
{
    partial class PrimitiveType
    {
        public virtual string ToStringOutput(string variableName)
        {
            return $"WriteIndented({variableName}, target)";
        }
    }

    partial class ArrayType
    {
        public string ToStringOutput(string variableName)
        {
            return $"WriteIndented({variableName}, WriteIndented, target)";
        }
    }

    partial class DictionaryType
    {
        public string ToStringOutput(string variableName)
        {
            return $"WriteIndented({variableName}, WriteIndented, WriteIndented, target)";
        }
    }
}