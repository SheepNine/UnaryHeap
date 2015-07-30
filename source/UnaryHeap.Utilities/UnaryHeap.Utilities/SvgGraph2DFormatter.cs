#if INCLUDE_WORK_IN_PROGRESS
using System;
using System.IO;
using System.Xml;
using System.Drawing;

namespace UnaryHeap.Utilities
{
    /// <summary>
    /// Provides methods for producing an SVG file of a UnaryHeap.Utilities.Graph2D object.
    /// </summary>
    public static class SvgGraph2DFormatter
    {
        /// <summary>
        /// Produces an SVG file for a UnaryHeap.Utilities.Graph2D object.
        /// </summary>
        /// <param name="graph">The graph to format.</param>
        /// <param name="destination">The writer to which the SVG content will be written.</param>
        /// <param name="options">The formatting options applied to the output SVG file.</param>
        public static void Generate(Graph2D graph, TextWriter destination, FormattingOptions options)
        {
            var extents = Orthotope2D.FromPoints(graph.Vertices);

            var majorAxis = options.MajorAxis;
            if (AxisOption.FromData == majorAxis)
            {
                if (extents.X.Size >= extents.Y.Size)
                    majorAxis = AxisOption.X;
                else
                    majorAxis = AxisOption.Y;
            }
            var xAxisIsAnchor = (majorAxis == AxisOption.X);

            extents = extents.GetPadded(options.PaddingThicknessFactor * (xAxisIsAnchor ? extents.X.Size : extents.Y.Size));
            var graphUnitsPerPixel = (xAxisIsAnchor ? extents.X.Size : extents.Y.Size) / options.MajorAxisSize;

            Rational outputWidth, outputHeight;

            if (xAxisIsAnchor)
            {
                outputWidth = options.MajorAxisSize;
                outputHeight = options.MajorAxisSize * extents.Y.Size / extents.X.Size;
            }
            else
            {
                outputWidth = options.MajorAxisSize * extents.X.Size / extents.Y.Size;
                outputHeight = options.MajorAxisSize;
            }
            Rational invertScalar = options.InvertYAxis ? -1 : 1;

            using (var writer = XmlWriter.Create(destination))
            {
                writer.WriteStartElement("svg", "http://www.w3.org/2000/svg");
                {
                    writer.WriteAttributeString("version", "1.1");
                    writer.WriteAttributeString("width", FormatRational(outputWidth.Ceiling));
                    writer.WriteAttributeString("height", FormatRational(outputHeight.Ceiling));
                    writer.WriteAttributeString("viewBox", string.Join(" ",
                        FormatRational(extents.X.Min),
                        FormatRational(options.InvertYAxis ? -extents.Y.Max : extents.Y.Min),
                        FormatRational(extents.X.Size),
                        FormatRational(extents.Y.Size)));

                    writer.WriteStartElement("rect");
                    {
                        writer.WriteAttributeString("x", FormatRational(extents.X.Min));
                        writer.WriteAttributeString("y", FormatRational(options.InvertYAxis ? -extents.Y.Max : extents.Y.Min));
                        writer.WriteAttributeString("width", FormatRational(extents.X.Size));
                        writer.WriteAttributeString("height", FormatRational(extents.Y.Size));
                        writer.WriteAttributeString("fill", FormatColor(options.BackgroundColor));
                    }
                    writer.WriteEndElement();

                    // --- Edges ---

                    if (options.OutlineThickness > 0)
                    {
                        writer.WriteStartElement("g");
                        {
                            var strokeWidth = graphUnitsPerPixel * (options.LineThickness + 2 * options.OutlineThickness);

                            writer.WriteAttributeString("stroke-width", FormatRational(strokeWidth));
                            writer.WriteAttributeString("stroke", FormatColor(options.OutlineColor));
                            writer.WriteAttributeString("stroke-linecap", "round");

                            foreach (var edge in graph.Edges)
                            {
                                writer.WriteStartElement("line");
                                writer.WriteAttributeString("x1", FormatRational(edge.Item1.X));
                                writer.WriteAttributeString("y1", FormatRational(edge.Item1.Y * invertScalar));
                                writer.WriteAttributeString("x2", FormatRational(edge.Item2.X));
                                writer.WriteAttributeString("y2", FormatRational(edge.Item2.Y * invertScalar));
                                writer.WriteEndElement();
                            }
                        }
                        writer.WriteEndElement();
                    }

                    writer.WriteStartElement("g");
                    {
                        var strokeWidth = graphUnitsPerPixel * options.LineThickness;

                        writer.WriteAttributeString("stroke-width", FormatRational(strokeWidth));
                        writer.WriteAttributeString("stroke", FormatColor(options.LineColor));
                        writer.WriteAttributeString("stroke-linecap", "round");

                        foreach (var edge in graph.Edges)
                        {
                            writer.WriteStartElement("line");
                            writer.WriteAttributeString("x1", FormatRational(edge.Item1.X));
                            writer.WriteAttributeString("y1", FormatRational(edge.Item1.Y * invertScalar));
                            writer.WriteAttributeString("x2", FormatRational(edge.Item2.X));
                            writer.WriteAttributeString("y2", FormatRational(edge.Item2.Y * invertScalar));
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();


                    // --- Vertices ---

                    if (options.VertexDiameter > 0)
                    {
                        if (options.OutlineThickness > 0)
                        {
                            writer.WriteStartElement("g");
                            writer.WriteAttributeString("fill", FormatColor(options.OutlineColor));
                            {
                                foreach (var vertex in graph.Vertices)
                                {
                                    var r = graphUnitsPerPixel * (options.VertexDiameter / 2 + options.OutlineThickness);

                                    writer.WriteStartElement("circle");
                                    writer.WriteAttributeString("cx", FormatRational(vertex.X));
                                    writer.WriteAttributeString("cy", FormatRational(vertex.Y * invertScalar));
                                    writer.WriteAttributeString("r", FormatRational(r));
                                    writer.WriteEndElement();
                                }
                            }
                            writer.WriteEndElement();
                        }

                        writer.WriteStartElement("g");
                        writer.WriteAttributeString("fill", FormatColor(options.VertexColor));
                        {
                            foreach (var vertex in graph.Vertices)
                            {
                                var r = graphUnitsPerPixel * (options.VertexDiameter / 2);

                                writer.WriteStartElement("circle");
                                writer.WriteAttributeString("cx", FormatRational(vertex.X));
                                writer.WriteAttributeString("cy", FormatRational(vertex.Y * invertScalar));
                                writer.WriteAttributeString("r", FormatRational(graphUnitsPerPixel * (options.VertexDiameter / 2)));
                                writer.WriteEndElement();
                            }
                        }
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }
        }

        static string FormatColor(Color value)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", value.R, value.G, value.B);
        }

        static string FormatRational(Rational value)
        {
            return ((double)value).ToString();
        }
    }

    /// <summary>
    /// Specifies the axis used to convert image units to graph units.
    /// </summary>
    public enum AxisOption
    {
        /// <summary>
        /// Always use the X axis.
        /// </summary>
        X,
        /// <summary>
        /// Always use the Y axis.
        /// </summary>
        Y,
        /// <summary>
        /// Use whichever axis has the largest range in graph units.
        /// </summary>
        FromData,
    }

    /// <summary>
    /// Specifies color and size information for the UnaryHeap.Utilities.SvgGraph2DFormatter.
    /// </summary>
    public class FormattingOptions
    {
        /// <summary>
        /// The width, in pixels, of the major axis of the output SVG image.
        /// </summary>
        public Rational MajorAxisSize = 640;
        /// <summary>
        /// Specifies how the SVGWriter determines which axis to anchor to ImageSize.
        /// </summary>
        public AxisOption MajorAxis = AxisOption.FromData;

        /// <summary>
        /// The diameter, in pixels, of vertices in the output SVG image.
        /// </summary>
        public Rational VertexDiameter = 50;
        /// <summary>
        /// The thickness, in pixels, of edges in the output SVG image.
        /// </summary>
        public Rational LineThickness = 15;
        /// <summary>
        /// The thickness, in pixels, of the outline of vertices and edges in the output SVG image.
        /// </summary>
        public Rational OutlineThickness = 5;

        /// <summary>
        /// The color used to fill the background.
        /// </summary>
        public Color BackgroundColor = Color.LightGray;
        /// <summary>
        /// The color used to render vertices.
        /// </summary>
        public Color VertexColor = Color.White;
        /// <summary>
        /// The color used to render edges.
        /// </summary>
        public Color LineColor = Color.DarkGray;
        /// <summary>
        /// The color used to render vertex and edge outlines.
        /// </summary>
        public Color OutlineColor = Color.Black;

        /// <summary>
        /// Whether to invert the Y axis so that the output image appears in a right-handed coordinate system.
        /// </summary>
        public bool InvertYAxis = true;
        /// <summary>
        /// Whether to increase the output SVG view box so that vertcies/edges are not clipped off along the boundary of the image.
        /// </summary>
        public bool PadImage = true;


        internal Rational PaddingThicknessInPixels
        {
            get
            {
                if (false == PadImage)
                    return Rational.Zero;

                var result = Rational.Max(
                    (VertexDiameter / 2 + OutlineThickness),
                    (LineThickness / 2 + OutlineThickness));

                if (result >= MajorAxisSize / 2)
                    throw new ArgumentException("Padding is too thick to produce the SVG.");

                return result;
            }
        }

        internal Rational PaddingThicknessFactor
        {
            get
            {
                var ptp = PaddingThicknessInPixels;
                return ptp / (MajorAxisSize - 2 * ptp);
            }
        }
    }
}
#endif