using System;
using System.IO;
using UnaryHeap.Utilities.D2;

namespace UnaryHeap.Utilities.Apps
{
    /// <summary>
    /// Represents the logic behind a console utility application that can convert
    /// JSON-formatted Graph2D objects to SVG.
    /// </summary>
    public abstract class GraphRendererApp
    {
        /// <summary>
        /// The main method of the application.
        /// </summary>
        /// <remarks>
        /// If run with no arguments, the Graph2D is read from Console.In and the SVG is written to Console.Out.
        /// If run with one argument, the Graph2D is read from the filename specified, and the SVG is written to the same filename, only with extension 'svg'.
        /// If run with two arguments, the Graph2D is read from the first filename specified, and the SVG is written to the second filename specified.
        /// </remarks>
        /// <param name="args">The arguments from the command prompt.</param>
        /// <returns>0 if successful; otherwise, a description of the error is written to Console.Error and the method returns 1.</returns>
        public static int MainMethod(string[] args)
        {
            try
            {
                switch (args.Length)
                {
                    case 0:
                        new ConsoleGraphRenderApp().Run();
                        return 0;
                    case 1:
                        new FileGraphRenderApp(args[0]).Run();
                        return 0;
                    case 2:
                        new FileGraphRenderApp(args[0], args[1]).Run();
                        return 0;
                }

                Console.Error.WriteLine("The syntax of the command is incorrect.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR: " + ex.Message);
            }

            return 1;
        }

        /// <summary>
        /// Runs the application: a Graph2D object is deserialized from the input stream,
        /// and an SVG representation is serialized to the output stream.
        /// </summary>
        public void Run()
        {
            var graph = ReadGraph();
            var settings = new SvgFormatterSettings(graph);

            WriteSvg(graph, settings);
        }

        void WriteSvg(Graph2D graph, SvgFormatterSettings settings)
        {
            TextWriter output = null;
            try
            {
                output = AcquireOutput();
                SvgGraph2DFormatter.Generate(graph, output, settings);
            }
            finally
            {
                if (null != output)
                    ReleaseOutput(output);
            }
        }


        Graph2D ReadGraph()
        {
            TextReader input = null;

            try
            {
                input = AcquireInput();
                return Graph2D.FromJson(input);
            }
            finally
            {
                if (null != input)
                    ReleaseInput(input);
            }
        }

        /// <summary>
        /// Gets a TextReader containing a JSON-formatted UnaryHeap.Utilities.Graph2D object.
        /// </summary>
        /// <returns>A TextReader containing a JSON-formatted UnaryHeap.Utilities.Graph2D object.</returns>
        protected abstract TextReader AcquireInput();
        /// <summary>
        /// Releases the TextReader returned from the AcquireInput method.
        /// </summary>
        /// <param name="reader">The TextReader returned from the AcquireInput method.</param>
        protected abstract void ReleaseInput(TextReader reader);

        /// <summary>
        /// Gets a TextWriter to which the SVG will be written.
        /// </summary>
        /// <returns>A TextWriter to which the SVG will be written</returns>
        protected abstract TextWriter AcquireOutput();

        /// <summary>
        /// Releases the TextWriter returned from the AcquireOutput method.
        /// </summary>
        /// <param name="writer">The TextWriter returned from the AcquireOutput method.</param>
        protected abstract void ReleaseOutput(TextWriter writer);
    }

    /// <summary>
    /// Implementation of UnaryHeap.Utilities.GraphRenderApp which reads from and writes to
    /// arbitrary streams.
    /// </summary>
    public class StreamGraphRenderApp : GraphRendererApp
    {
        TextReader input;
        TextWriter output;

        /// <summary>
        /// Initializes a new instance of the UnaryHeap.Utilities.StreamGraphRenderApp class.
        /// </summary>
        /// <param name="input">The input stream from which to read.</param>
        /// <param name="output">The output stream to which to write.</param>
        /// <exception cref="System.ArgumentNullException">input or output are null.</exception>
        public StreamGraphRenderApp(TextReader input, TextWriter output)
        {
            if (null == input)
                throw new ArgumentNullException("input");
            if (null == output)
                throw new ArgumentNullException("output");

            this.input = input;
            this.output = output;
        }

        /// <summary>
        /// Gets a TextReader containing a JSON-formatted UnaryHeap.Utilities.Graph2D object.
        /// </summary>
        /// <returns>A TextReader containing a JSON-formatted UnaryHeap.Utilities.Graph2D object.</returns>
        protected override TextReader AcquireInput()
        {
            return input;
        }

        /// <summary>
        /// Releases the TextReader returned from the AcquireInput method.
        /// </summary>
        /// <param name="reader">The TextReader returned from the AcquireInput method.</param>
        protected override void ReleaseInput(TextReader reader)
        {
        }

        /// <summary>
        /// Gets a TextWriter to which the SVG will be written.
        /// </summary>
        /// <returns>A TextWriter to which the SVG will be written</returns>
        protected override TextWriter AcquireOutput()
        {
            return output;
        }

        /// <summary>
        /// Releases the TextWriter returned from the AcquireOutput method.
        /// </summary>
        /// <param name="writer">The TextWriter returned from the AcquireOutput method.</param>
        protected override void ReleaseOutput(TextWriter writer)
        {
        }
    }

    /// <summary>
    /// Implementation of UnaryHeap.Utilities.GraphRenderApp which reads from and writes to standard streams.
    /// </summary>
    class ConsoleGraphRenderApp : StreamGraphRenderApp
    {
        public ConsoleGraphRenderApp()
            : base(Console.In, Console.Out)
        {
        }
    }

    /// <summary>
    /// Implementation of UnaryHeap.Utilities.GraphRenderApp which reads from and writes to files on disk.
    /// </summary>
    public class FileGraphRenderApp : GraphRendererApp
    {
        string inputJsonFile;
        string outputSvgFile;

        /// <summary>
        /// Initializes a new instance of the UnaryHeap.Utilities.FileGraphReaderApp class using the specified
        /// input file name and a default output file name.
        /// </summary>
        /// <param name="inputJsonFile">The name of the input file.</param>
        public FileGraphRenderApp(string inputJsonFile)
        {
            if (null == inputJsonFile)
                throw new ArgumentNullException("inputJsonFile");
            if (0 == inputJsonFile.Length)
                throw new ArgumentOutOfRangeException("inputJsonFile");

            inputJsonFile = Path.GetFullPath(inputJsonFile);

            if (false == File.Exists(inputJsonFile))
                throw new ArgumentException("Input file not found.", "inputJsonFile");
            if (string.Equals(".svg", Path.GetExtension(inputJsonFile), StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Input file name has extension 'svg' and collides with default output file name. Output file name must be specified.", "inputJsonFile");

            this.inputJsonFile = inputJsonFile;
            this.outputSvgFile = Path.ChangeExtension(inputJsonFile, "svg");
        }

        /// <summary>
        /// Initializes a new instance of the UnaryHeap.Utilities.FileGraphReaderApp class using the specified
        /// input and output file names.
        /// </summary>
        /// <param name="inputJsonFile">The name of the input file.</param>
        /// <param name="outputSvgFile">The name of the output file.</param>
        public FileGraphRenderApp(string inputJsonFile, string outputSvgFile)
        {
            if (null == inputJsonFile)
                throw new ArgumentNullException("inputJsonFile");
            if (null == outputSvgFile)
                throw new ArgumentNullException("outputSvgFile");
            if (0 == inputJsonFile.Length)
                throw new ArgumentOutOfRangeException("inputJsonFile");
            if (0 == outputSvgFile.Length)
                throw new ArgumentOutOfRangeException("outputSvgFile");

            inputJsonFile = Path.GetFullPath(inputJsonFile);
            outputSvgFile = Path.GetFullPath(outputSvgFile);

            if (false == File.Exists(inputJsonFile))
                throw new ArgumentException("Input file not found.", "inputJsonFile");
            if (string.Equals(inputJsonFile, outputSvgFile, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Input file name cannot be the same as output file name.");

            this.inputJsonFile = inputJsonFile;
            this.outputSvgFile = outputSvgFile;
        }

        /// <summary>
        /// Gets a TextReader containing a JSON-formatted UnaryHeap.Utilities.Graph2D object.
        /// </summary>
        /// <returns>A TextReader containing a JSON-formatted UnaryHeap.Utilities.Graph2D object.</returns>
        protected override TextReader AcquireInput()
        {
            return File.OpenText(inputJsonFile);
        }

        /// <summary>
        /// Releases the TextReader returned from the AcquireInput method.
        /// </summary>
        /// <param name="reader">The TextReader returned from the AcquireInput method.</param>
        protected override void ReleaseInput(TextReader reader)
        {
            reader.Close();
        }

        /// <summary>
        /// Gets a TextWriter to which the SVG will be written.
        /// </summary>
        /// <returns>A TextWriter to which the SVG will be written</returns>
        protected override TextWriter AcquireOutput()
        {
            return File.CreateText(outputSvgFile);
        }

        /// <summary>
        /// Releases the TextWriter returned from the AcquireOutput method.
        /// </summary>
        /// <param name="writer">The TextWriter returned from the AcquireOutput method.</param>
        protected override void ReleaseOutput(TextWriter writer)
        {
            writer.Close();
        }
    }
}