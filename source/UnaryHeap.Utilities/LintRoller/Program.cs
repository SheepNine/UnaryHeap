using System;
using System.IO;
using System.Linq;
using System.Web;

namespace LintRoller
{
    static class Lint
    {
        public static int Main(string[] args)
        {
            var reporter = Reporter.MakeReporter(args[2], Console.Out);
            CheckAllCSFiles(Path.GetFullPath(args[0]), int.Parse(args[1]), reporter);
            return reporter.FailedFiles;
        }

        static void CheckAllCSFiles(string rootDirectory, int maxChars, Reporter reporter)
        {
            reporter.ReportStart(rootDirectory, maxChars);

            foreach (var file in Directory.GetFiles(rootDirectory, "*.cs", SearchOption.AllDirectories))
                CheckCSFile(rootDirectory, file.Replace(rootDirectory + "\\", string.Empty), maxChars, reporter);

            reporter.ReportEnd();
        }

        static void CheckCSFile(string rootDirectory, string relativeFileName, int maxChars, Reporter reporter)
        {
            var file = Path.Combine(rootDirectory, relativeFileName);
            var lines = File.ReadAllLines(file);
            var longLines = Enumerable.Range(0, lines.Length).Where(i => lines[i].Length > maxChars).ToArray();
            var containsTabs = lines.Any(line => line.Contains('\t'));

            if (longLines.Length > 0 || containsTabs)
                reporter.ReportBadFile(relativeFileName, lines, longLines, containsTabs);
        }
    }


    abstract class Reporter
    {
        public int FailedFiles { get; private set; }

        public abstract void ReportStart(string rootDirectory, int maxChars);

        public void ReportBadFile(string relativeFileName, string[] lines, int[] longLineIndices, bool containsTabs)
        {
            FailedFiles += 1;
            ReportLintyFileDetails(relativeFileName, lines, longLineIndices, containsTabs);
        }
        protected abstract void ReportLintyFileDetails(string relativeFileName, string[] lines, int[] longLineIndices, bool containsTabs);

        public abstract void ReportEnd();

        public static Reporter MakeReporter(string code, TextWriter output)
        {
            if (string.IsNullOrEmpty(code))
                return new SilentReporter();
            else if (string.Equals(code, "html", StringComparison.OrdinalIgnoreCase))
                return new HtmlReporter(output);
            else if (string.Equals(code, "text", StringComparison.OrdinalIgnoreCase))
                return new TextReporter(output);
            else
                throw new ArgumentOutOfRangeException("code");
        }
    }


    class SilentReporter : Reporter
    {
        public override void ReportStart(string rootDirectory, int maxChars) { }
        protected override void ReportLintyFileDetails(string relativeFileName, string[] lines, int[] longLineIndices, bool containsTabs) { }
        public override void ReportEnd() { }
    }


    class TextReporter : Reporter
    {
        TextWriter output;
        int maxChars;

        public TextReporter(TextWriter output)
        {
            this.output = output;
        }

        public override void ReportStart(string rootDirectory, int maxChars)
        {
            this.maxChars = maxChars;
        }

        protected override void ReportLintyFileDetails(string relativeFileName, string[] lines, int[] longLineIndices, bool containsTabs)
        {
            output.WriteLine(new string('-', relativeFileName.Length));
            output.Write(relativeFileName);
            if (containsTabs)
                output.Write(" (CONTAINS TABS)");
            output.WriteLine();
            output.WriteLine(new string('-', relativeFileName.Length));

            foreach (var lineIndex in longLineIndices)
                output.WriteLine("\t{0:D4}: {1}... {2:D3}", lineIndex + 1, lines[lineIndex].Substring(0, maxChars), lines[lineIndex].Length);

            output.WriteLine();
        }

        public override void ReportEnd()
        {
            if (FailedFiles == 0)
                output.WriteLine("No lint detected!");
        }
    }


    class HtmlReporter : Reporter
    {
        TextWriter output;
        int maxChars;

        public HtmlReporter(TextWriter output)
        {
            this.output = output;
        }

        public override void ReportStart(string rootDirectory, int maxChars)
        {
            this.maxChars = maxChars;

            output.Write("<html><head><style>");
            output.Write("body{background:#EEEEEE}");
            output.Write("pre{font-size:12px}");
            output.Write("table{border-collapse:collapse;border:1px solid black;background:#FFFFFF}");
            output.Write("td{border:1px solid black}");
            output.Write("td.tblhdr{background:#C0C0C0;font-weight:bold}");
            output.Write("</style></head><body>");
        }

        protected override void ReportLintyFileDetails(string relativeFileName, string[] lines, int[] longLineIndices, bool containsTabs)
        {
            output.Write("<p><table><tr><td class=\"tblhdr\" colspan=\"3\">");
            output.Write(HttpUtility.HtmlEncode(relativeFileName));
            if (containsTabs)
                output.Write(" (CONTAINS TABS)");
            output.Write("</td></tr>");

            foreach (var lineIndex in longLineIndices)
                output.Write("<tr><td>{0:D4}</td><td><pre>{2}...</pre></td><td>{1:D3}</td></tr>", lineIndex + 1, lines[lineIndex].Length, HttpUtility.HtmlEncode(lines[lineIndex].Substring(0, maxChars)));

            output.Write("</table></p>");
        }

        public override void ReportEnd()
        {
            if (FailedFiles == 0)
                output.Write("<h1>No lint detected!</h1>");

            output.Write("</body></html>");
        }
    }
}