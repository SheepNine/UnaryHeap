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
        public static void Generate(Graph2D graph, TextWriter destination, SvgFormatterSettings options)
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

            extents = extents.GetPadded(PaddingThicknessFactor(options) * (xAxisIsAnchor ? extents.X.Size : extents.Y.Size));
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
                        writer.WriteAttributeString("fill", options.BackgroundColor);
                    }
                    writer.WriteEndElement();

                    // --- Edges ---

                    if (options.EdgeThickness > 0)
                    {
                        if (options.OutlineThickness > 0)
                        {
                            writer.WriteStartElement("g");
                            {
                                var strokeWidth = graphUnitsPerPixel * (options.EdgeThickness + 2 * options.OutlineThickness);

                                writer.WriteAttributeString("stroke-width", FormatRational(strokeWidth));
                                writer.WriteAttributeString("stroke", options.OutlineColor);
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
                            var strokeWidth = graphUnitsPerPixel * options.EdgeThickness;

                            writer.WriteAttributeString("stroke-width", FormatRational(strokeWidth));
                            writer.WriteAttributeString("stroke", options.EdgeColor);
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


                    // --- Vertices ---

                    if (options.VertexDiameter > 0)
                    {
                        if (options.OutlineThickness > 0)
                        {
                            writer.WriteStartElement("g");
                            writer.WriteAttributeString("fill", options.OutlineColor);
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
                        writer.WriteAttributeString("fill", options.VertexColor);
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

        static Rational PaddingThicknessFactor(SvgFormatterSettings settings)
        {
            var ptp = settings.PaddingThickness;
            return ptp / (settings.MajorAxisSize - 2 * ptp);
        }

        static string FormatRational(Rational value)
        {
            return ((double)value).ToString();
        }
    }
}