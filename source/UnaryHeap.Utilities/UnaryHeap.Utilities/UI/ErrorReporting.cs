using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace UnaryHeap.Utilities.UI
{
    /// <summary>
    /// Contains utility methods for generating exception reports.
    /// 
    /// A Windows Forms app can use methods in this class to hook their
    /// unhandled exception events, and to fork itself in a special error
    /// reporting mode.
    /// </summary>
    public static class ErrorReporting
    {
        /// <summary>
        /// Runs the application as an error reporter, if the command-line parameters
        /// are correct. Otherwise, runs the application normally.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        /// <param name="programMain">The program main method,
        /// to be run in normal scenarios</param>
        /// <returns>The error code returned from programMain.</returns>
        public static int ErrorHandlingMain(String[] args, Func<String[], int> programMain)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (programMain == null)
                throw new ArgumentNullException("programMain");

            if (args.Length == 2 && string.Equals(args[0], "crash-report")
                    && File.Exists(Path.GetFullPath(args[1])))
                return ReportError(args[1]);
            else
            {
                ErrorReporting.HookUnhandledExceptionEvents(1);
                return programMain(args);
            }
        }

        private static int ReportError(string crashReportFileName)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CrashReport(File.ReadAllText(crashReportFileName)));
            return 0;
        }

        /// <summary>
        /// Add hooks to the runtime to lauch the crash rport and exit
        /// if an unhandled exception occurs.
        /// </summary>
        /// <param name="exitCode">The exit code to return from the process
        /// if an unhandled exception occurs.</param>
        public static void HookUnhandledExceptionEvents(int exitCode)
        {
            Application.ThreadException +=
                (sender, e) => ExitAfterLaunchingCrashReport(e.Exception, exitCode);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                ExitAfterLaunchingCrashReport((Exception)e.ExceptionObject, exitCode);
        }

        /// <summary>
        /// Launch a crash report and terminate the current process.
        /// </summary>
        /// <param name="ex">The exception to report.</param>
        /// <param name="exitCode">The exit code to return from the process.</param>
        public static void ExitAfterLaunchingCrashReport(Exception ex, int exitCode)
        {
            var path = GenerateCrashFile(ex);
            Process.Start(Application.ExecutablePath, "crash-report \"" + path + "\"");
            Environment.Exit(exitCode);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "Safe; for display in UI")]
        static string GenerateCrashFile(Exception ex)
        {
            var fileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            using (var file = File.CreateText(fileName))
            {
                file.Write("An unhandled exception has occurred in ");
                file.Write(Path.GetFileName(Application.ExecutablePath).ToLowerInvariant());
                file.Write(" at ");
                file.WriteLine(DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
                file.WriteLine();

                while (ex != null)
                {
                    file.Write(ex.GetType().ToString());
                    file.Write(": ");
                    file.WriteLine(ex.Message);
                    file.WriteLine(ex.StackTrace);

                    ex = ex.InnerException;
                    if (ex != null)
                    {
                        file.WriteLine();
                        file.WriteLine("Inner exception:");
                    }
                }
            }
            return fileName;
        }
    }
}
