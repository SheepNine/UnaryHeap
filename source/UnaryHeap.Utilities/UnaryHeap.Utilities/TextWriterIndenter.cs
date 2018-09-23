using System;
using System.IO;

namespace UnaryHeap.Utilities
{
    /// <summary>
    /// Inserts indentation into a TextWriter as data is being written to it.
    /// </summary>
    public class TextWriterIndenter : IDisposable
    {
        /// <summary>
        /// Get or set the string used for a single level of indentation.
        /// </summary>
        public string IndentString { get; set; }

        bool atStartOfLine = true;
        int indentLevel = 0;
        TextWriter target;

        /// <summary>
        /// Initializes a new instance of the TextWriter class.
        /// </summary>
        /// <param name="target">The TextWriter that will receive indented text.</param>
        public TextWriterIndenter(TextWriter target)
        {
            this.target = target;
            IndentString = "\t";
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Control and its child
        /// controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.</param>
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

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be
        /// written to the underlying device.
        /// </summary>
        public void Flush()
        {
            target.Flush();
        }

        /// <summary>
        /// Increases by one the number of indentations added at the start of each line.
        /// </summary>
        public void IncreaseIndent()
        {
            indentLevel += 1;
        }

        /// <summary>
        /// Decreases by one the number of indentations added at the start of each line.
        /// </summary>
        public void DecreaseIndent()
        {
            indentLevel = Math.Max(0, indentLevel - 1);
        }

        void WriteIndentIfRequired()
        {
            if (!atStartOfLine) return;

            for (var i = 0; i < indentLevel; i++)
                target.Write(IndentString);

            atStartOfLine = false;
        }

        /// <summary>
        /// Writes data to the target, and starts a new line.
        /// </summary>
        /// <param name="value">The data to write.</param>
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

        /// <summary>
        /// Writes data to the target, and starts a new line.
        /// </summary>
        /// <param name="buffer">The data to write.</param>
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

        /// <summary>
        /// Writes data to the target, and starts a new line.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void WriteLine(double value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        /// <summary>
        /// Writes data to the target, and starts a new line.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void WriteLine(decimal value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        /// <summary>
        /// Writes data to the target, and starts a new line.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void WriteLine(float value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        /// <summary>
        /// Writes data to the target, and starts a new line.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void WriteLine(bool value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        /// <summary>
        /// Writes data to the target, and starts a new line.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void WriteLine(int value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        /// <summary>
        /// Writes data to the target, and starts a new line.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void WriteLine(uint value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        /// <summary>
        /// Writes data to the target, and starts a new line.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void WriteLine(ulong value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        /// <summary>
        /// Writes data to the target, and starts a new line.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void WriteLine(long value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }

        /// <summary>
        /// Writes data to the target, and starts a new line.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void WriteLine(char value)
        {
            WriteIndentIfRequired();
            target.WriteLine(value);
            atStartOfLine = true;
        }


        /// <summary>
        /// Writes data to the target.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void Write(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                WriteIndentIfRequired();
                target.Write(value);
            }
        }

        /// <summary>
        /// Writes data to the target.
        /// </summary>
        /// <param name="buffer">The data to write.</param>
        public void Write(char[] buffer)
        {
            if (buffer != null && buffer.Length > 0)
            {
                WriteIndentIfRequired();
                target.Write(buffer);
            }
        }

        /// <summary>
        /// Writes data to the target.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void Write(double value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        /// <summary>
        /// Writes data to the target.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void Write(decimal value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        /// <summary>
        /// Writes data to the target.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void Write(float value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        /// <summary>
        /// Writes data to the target.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void Write(bool value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        /// <summary>
        /// Writes data to the target.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void Write(int value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        /// <summary>
        /// Writes data to the target.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void Write(uint value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        /// <summary>
        /// Writes data to the target.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void Write(ulong value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        /// <summary>
        /// Writes data to the target.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void Write(long value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }

        /// <summary>
        /// Writes data to the target.
        /// </summary>
        /// <param name="value">The data to write.</param>
        public void Write(char value)
        {
            WriteIndentIfRequired();
            target.Write(value);
        }
    }
}
