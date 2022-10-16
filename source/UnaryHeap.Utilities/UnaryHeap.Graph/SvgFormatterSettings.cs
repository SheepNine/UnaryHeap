using System;
using UnaryHeap.Utilities.Core;

namespace UnaryHeap.Graph
{
    /// <summary>
    /// Specifies color and size information for the SvgGraph2DFormatter class.
    /// </summary>
    public class SvgFormatterSettings
    {
        /// <summary>
        /// Initializes a new instance of the SvgFormatterSettings class with default settings.
        /// </summary>
        public SvgFormatterSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SvgFormatterSettings class
        /// by reading override defaults from the specified Graph2D object.
        /// </summary>
        public SvgFormatterSettings(Graph2D source)
        {
            if (null == source)
                throw new ArgumentNullException("source");

            MajorAxisSize = Rational.Parse(
                source.GetGraphMetadatum("major_axis_size", majorAxisSize.ToString()));
            MajorAxis = (AxisOption)Enum.Parse(typeof(AxisOption),
                source.GetGraphMetadatum("major_axis", majorAxis.ToString()), true);
            VertexDiameter = Rational.Parse(
                source.GetGraphMetadatum("vertex_size", vertexDiameter.ToString()));
            EdgeThickness = Rational.Parse(
                source.GetGraphMetadatum("edge_size", edgeThickness.ToString()));
            OutlineThickness = Rational.Parse(
                source.GetGraphMetadatum("outline_size", outlineThickness.ToString()));

            BackgroundColor = source.GetGraphMetadatum(
                "background_color", backgroundColor);
            VertexColor = source.GetGraphMetadatum(
                "vertex_color", vertexColor);
            EdgeColor = source.GetGraphMetadatum(
                "edge_color", edgeColor);
            OutlineColor = source.GetGraphMetadatum(
                "outline_color", outlineColor);
            InvertYAxis = bool.Parse(source.GetGraphMetadatum(
                "invert_y", invertYaxis.ToString()));
            PadImage = bool.Parse(source.GetGraphMetadatum(
                "pad", padImage.ToString()));
        }

        Rational majorAxisSize = 640;
        /// <summary>
        /// The width, in pixels, of the major axis of the output SVG image.
        /// </summary>
        public Rational MajorAxisSize
        {
            get { return majorAxisSize; }
            set
            {
                if (null == value)
                    throw new ArgumentNullException("value");
                if (0 >= value)
                    throw new ArgumentOutOfRangeException(
                        "value", "value must be greater than zero.");

                majorAxisSize = value;
            }
        }

        AxisOption majorAxis = AxisOption.FromData;
        /// <summary>
        /// Specifies how the SVGWriter determines which axis to anchor to ImageSize.
        /// </summary>
        public AxisOption MajorAxis
        {
            get { return majorAxis; }
            set { majorAxis = value; }
        }

        Rational vertexDiameter = 50;
        /// <summary>
        /// The diameter, in pixels, of vertices in the output SVG image.
        /// </summary>
        public Rational VertexDiameter
        {
            get { return vertexDiameter; }
            set
            {
                if (null == value)
                    throw new ArgumentNullException("value");
                if (0 > value)
                    throw new ArgumentOutOfRangeException(
                        "value", "value must be greater than or equal to zero.");

                vertexDiameter = value;
            }
        }

        Rational edgeThickness = 15;
        /// <summary>
        /// The thickness, in pixels, of edges in the output SVG image.
        /// </summary>
        public Rational EdgeThickness
        {
            get { return edgeThickness; }
            set
            {
                if (null == value)
                    throw new ArgumentNullException("value");
                if (0 > value)
                    throw new ArgumentOutOfRangeException(
                        "value", "value must be greater than or equal to zero.");

                edgeThickness = value;
            }
        }

        Rational outlineThickness = 5;
        /// <summary>
        /// The thickness, in pixels, of the outline of vertices and edges in
        /// the output SVG image.
        /// </summary>
        public Rational OutlineThickness
        {
            get { return outlineThickness; }
            set
            {
                if (null == value)
                    throw new ArgumentNullException("value");
                if (0 > value)
                    throw new ArgumentOutOfRangeException(
                        "value", "value must be greater than or equal to zero.");

                outlineThickness = value;
            }
        }

        string backgroundColor = "lightgray";
        /// <summary>
        /// The color used to fill the background.
        /// </summary>
        public string BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                if (null == value)
                    throw new ArgumentNullException("value");
                if (0 == value.Length)
                    throw new ArgumentOutOfRangeException("value");

                backgroundColor = value;
            }
        }

        string vertexColor = "white";
        /// <summary>
        /// The color used to render vertices.
        /// </summary>
        public string VertexColor
        {
            get { return vertexColor; }
            set
            {
                if (null == value)
                    throw new ArgumentNullException("value");
                if (0 == value.Length)
                    throw new ArgumentOutOfRangeException("value");

                vertexColor = value;
            }
        }

        string edgeColor = "darkgray";
        /// <summary>
        /// The color used to render edges.
        /// </summary>
        public string EdgeColor
        {
            get { return edgeColor; }
            set
            {
                if (null == value)
                    throw new ArgumentNullException("value");
                if (0 == value.Length)
                    throw new ArgumentOutOfRangeException("value");

                edgeColor = value;
            }
        }

        string outlineColor = "black";
        /// <summary>
        /// The color used to render vertex and edge outlines.
        /// </summary>
        public string OutlineColor
        {
            get { return outlineColor; }
            set
            {
                if (null == value)
                    throw new ArgumentNullException("value");
                if (0 == value.Length)
                    throw new ArgumentOutOfRangeException("value");

                outlineColor = value;
            }
        }

        bool invertYaxis = true;
        /// <summary>
        /// Whether to invert the Y axis so that the output image appears in a right-handed
        /// coordinate system.
        /// </summary>
        public bool InvertYAxis
        {
            get { return invertYaxis; }
            set { invertYaxis = value; }
        }

        bool padImage = true;
        /// <summary>
        /// Whether to increase the output SVG view box so that vertcies/edges are not
        /// clipped off along the boundary of the image.
        /// </summary>
        public bool PadImage
        {
            get { return padImage; }
            set { padImage = value; }
        }

        /// <summary>
        /// Gets the size, in pixels, of the padding inside the image that prevents vertices
        /// and edges from being clipped by the SVG viewport.
        /// </summary>
        public Rational PaddingThickness
        {
            get
            {
                if (false == padImage)
                    return Rational.Zero;

                return OutlineThickness + Rational.Max(VertexDiameter, EdgeThickness) / 2;
            }
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
}
