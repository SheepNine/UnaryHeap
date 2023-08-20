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
            output.Write("\tpublic partial class ");
            output.WriteLine(clasz.Name);
            output.WriteLine("\t{");

            output.WriteLine(@"        public override string ToString()
        {
            return ToString(global::System.Globalization.CultureInfo.InvariantCulture);
        }

        public string ToString(global::System.IFormatProvider formatProvider)
        {
            global::System.IO.StringWriter target = " +
                @"new global::System.IO.StringWriter(formatProvider);
            WriteIndented(new TextWriterIndenter(target));
            return target.ToString();
        }");

            if (!clasz.Members.Any())
                output.WriteLine("#pragma warning disable CA1822 // Mark members as static");

            output.WriteLine("\t\tpublic void WriteIndented(TextWriterIndenter target)");

            if (!clasz.Members.Any())
                output.WriteLine("#pragma warning restore CA1822");

            output.WriteLine("\t{");
            if (!clasz.Members.Any())
            {
                output.WriteLine("\t\t\ttarget.Write(\"{ }\");");
            }
            else
            {
                output.WriteLine(@"            target.WriteLine(""{"");
            target.IncreaseIndent();");

                foreach (var member in clasz.Members)
                {
                    output.WriteLine("\t\t\ttarget.Write(\"" + member.PublicMemberName() +
                        " = \");");
                    output.Write("\t\t\t");
                    output.WriteLine(member.ToStringer());
                    output.WriteLine("\t\t\ttarget.WriteLine();");
                }

                output.WriteLine(
    @"            target.DecreaseIndent();
            target.Write(""}"");");
            }

            output.WriteLine("\t\t}");
            output.WriteLine("\t}");
        }

        static void WriteToStringHelperClass(TextWriter output,
            PocoNamespace dataModel)
        {
            output.WriteLine(@"    static class ToStringHelper
    {
    }

    public class TextWriterIndenter : global::System.IDisposable
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

        public void Write(int value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        public void Write(uint value)
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
