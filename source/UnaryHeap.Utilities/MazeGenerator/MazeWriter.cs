using System.IO;
using UnaryHeap.Utilities.D2;

namespace MazeGenerator
{
    static class MazeWriter
    {
        public static void WriteMaze(
            TextWriter output, Graph2D graph, SvgFormatterSettings settings = null)
        {
            if (null == settings)
                settings = new SvgFormatterSettings()
                {
                    EdgeThickness = 2,
                    OutlineThickness = 0,
                    VertexDiameter = 0,
                    MajorAxisSize = 1000,
                    EdgeColor = "#404040",
                    BackgroundColor = "#C0C0C0"
                };

            SvgGraph2DFormatter.Generate(graph, output, settings);
        }
    }
}
