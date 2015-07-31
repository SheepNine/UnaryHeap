using System;

namespace UnaryHeap.Utilities
{
    /// <summary>
    /// Specifies color and size information for the UnaryHeap.Utilities.SvgGraph2DFormatter.
    /// </summary>
    public class SvgFormatterSettings
    {
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
                    throw new ArgumentOutOfRangeException("value", "value must be greater than zero.");

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
                    throw new ArgumentOutOfRangeException("value", "value must be greater than or equal to zero.");

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
                    throw new ArgumentOutOfRangeException("value", "value must be greater than or equal to zero.");

                edgeThickness = value;
            }
        }

        Rational outlineThickness = 5;
        /// <summary>
        /// The thickness, in pixels, of the outline of vertices and edges in the output SVG image.
        /// </summary>
        public Rational OutlineThickness
        {
            get { return outlineThickness; }
            set
            {
                if (null == value)
                    throw new ArgumentNullException("value");
                if (0 > value)
                    throw new ArgumentOutOfRangeException("value", "value must be greater than or equal to zero.");

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
                if (string.Empty == value)
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
                if (string.Empty == value)
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
                if (string.Empty == value)
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
                if (string.Empty == value)
                    throw new ArgumentOutOfRangeException("value");

                outlineColor = value;
            }
        }

        bool invertYaxis = true;
        /// <summary>
        /// Whether to invert the Y axis so that the output image appears in a right-handed coordinate system.
        /// </summary>
        public bool InvertYAxis
        {
            get { return invertYaxis; }
            set { invertYaxis = value; }
        }

        bool padImage = true;
        /// <summary>
        /// Whether to increase the output SVG view box so that vertcies/edges are not clipped off along the boundary of the image.
        /// </summary>
        public bool PadImage
        {
            get { return padImage; }
            set { padImage = value; }
        }

        /// <summary>
        /// Gets the size, in pixels, of the padding inside the image that prevents vertices and edges from being clipped by
        /// the SVG viewport.
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
