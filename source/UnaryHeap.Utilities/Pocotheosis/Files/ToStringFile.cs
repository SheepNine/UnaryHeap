using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteToStringFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                dataModel.WriteNamespaceHeader(file);
                WriteToStringHelperClass(file, dataModel);
                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    WriteClassToStringImplementation(pocoClass, file);
                }
                dataModel.WriteNamespaceFooter(file);
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
        }

        public void WriteIndented(TextWriterIndenter target)
        {");
            if (clasz.Members.Count() == 0)
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
                    output.WriteLine(member.GetToStringer());
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
        public static string FormatValue(bool value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(string value, global::System.IFormatProvider format)
        {
            return value;
        }
        public static string FormatValue(byte value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(ushort value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(uint value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(ulong value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(sbyte value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(short value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(int value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(long value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }");

            foreach (var enume in dataModel.Enums)
            {
                output.WriteLine(
@"        public static string FormatValue({0} value, global::System.IFormatProvider format)
        {{
            return value.ToString();
        }}", enume.Name);
            }

            foreach (var classe in dataModel.Classes)
            {
                output.WriteLine(
@"        public static string FormatValue({0} value, global::System.IFormatProvider format)
        {{
            return value.ToString();
        }}", classe.Name);
            }

            output.WriteLine(@"        public static void WriteArrayMember<T>(
            global::System.Text.StringBuilder builder,
            string memberName, global::System.Collections.Generic.IList<T> memberValues,
            global::System.Func<T, global::System.IFormatProvider, string> memberFormatter,
            global::System.IFormatProvider format)
        {
            builder.AppendLine();
            builder.Append(""\t"");
            builder.Append(memberName);
            builder.Append("": "");
            if (memberValues.Count > 0)
                builder.Append(string.Join("", "", global::System.Linq.Enumerable.Select(
                    memberValues, member => memberFormatter(member, format))));
            else
                builder.Append(""<empty>"");
        }

        public static void WriteDictionaryMember<TKey, TValue>(
            global::System.Text.StringBuilder builder,
            string memberName,
            global::System.Collections.Generic.SortedDictionary<TKey, TValue> memberValues,
            global::System.Func<TKey, global::System.IFormatProvider, string> keyFormatter,
            global::System.Func<TValue, global::System.IFormatProvider, string> valueFormatter,
            global::System.IFormatProvider format)
        {
            if (memberValues.Count > 0)
            {
                foreach (var iter in memberValues)
                {
                    builder.AppendLine();
                    builder.Append(""\t"");
                    builder.Append(keyFormatter(iter.Key, format));
                    builder.Append("": "");
                    builder.Append(valueFormatter(iter.Value, format));
                }
            }
            else
            {
                builder.Append(""<empty>"");
            }
        }

        public static void WriteMember<T>(global::System.Text.StringBuilder builder,
            string memberName, T memberValue,
            global::System.Func<T, global::System.IFormatProvider, string> memberFormatter,
            global::System.IFormatProvider format)
        {
            builder.AppendLine();
            builder.Append(""\t"");
            builder.Append(memberName);
            builder.Append("": "");
            builder.Append(memberFormatter(memberValue, format));
        }
    }

    public class TextWriterIndenter : global::System.IDisposable
    {
        public string IndentString { get; set; }

        bool atStartOfLine = true;
        int indentLevel = 0;
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

        public void WriteLine(char[] buffer)
        {
            if (buffer != null && buffer.Length > 0)
            {
                WriteIndentIfRequired();
                target.Write(buffer);
            }

            target.WriteLine();
            atStartOfLine = true;
        }

        public void WriteLine(double value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        public void WriteLine(decimal value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        public void WriteLine(float value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        public void WriteLine(bool value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        public void WriteLine(int value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        public void WriteLine(uint value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        public void WriteLine(ulong value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        public void WriteLine(long value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        public void WriteLine(char value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
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

        public void Write(char[] buffer)
        {
            if (buffer != null && buffer.Length > 0)
            {
                WriteIndentIfRequired();
                target.Write(buffer);
            }
        }

        public void Write(double value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        public void Write(decimal value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        public void Write(float value)
        {
            WriteIndentIfRequired();
            target.Write(value);
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

        public void Write(char value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }
    }");
        }
    }
}
