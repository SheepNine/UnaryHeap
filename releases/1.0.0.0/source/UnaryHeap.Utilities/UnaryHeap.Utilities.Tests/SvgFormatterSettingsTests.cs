using System;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class SvgFormatterSettingsTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Defaults()
        {
            var sut = new SvgFormatterSettings();

            Assert.Equal(640, sut.MajorAxisSize);
            Assert.Equal(AxisOption.FromData, sut.MajorAxis);
            Assert.Equal(50, sut.VertexDiameter);
            Assert.Equal(15, sut.EdgeThickness);
            Assert.Equal(5, sut.OutlineThickness);
            Assert.Equal("lightgray", sut.BackgroundColor);
            Assert.Equal("white", sut.VertexColor);
            Assert.Equal("darkgray", sut.EdgeColor);
            Assert.Equal("black", sut.OutlineColor);
            Assert.True(sut.InvertYAxis);
            Assert.True(sut.PadImage);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void DefaultsFromGraph()
        {
            var sut = new SvgFormatterSettings(new Graph2D(false));

            Assert.Equal(640, sut.MajorAxisSize);
            Assert.Equal(AxisOption.FromData, sut.MajorAxis);
            Assert.Equal(50, sut.VertexDiameter);
            Assert.Equal(15, sut.EdgeThickness);
            Assert.Equal(5, sut.OutlineThickness);
            Assert.Equal("lightgray", sut.BackgroundColor);
            Assert.Equal("white", sut.VertexColor);
            Assert.Equal("darkgray", sut.EdgeColor);
            Assert.Equal("black", sut.OutlineColor);
            Assert.True(sut.InvertYAxis);
            Assert.True(sut.PadImage);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Mutators()
        {
            var sut = new SvgFormatterSettings();

            sut.MajorAxisSize = 1640;
            sut.MajorAxis = AxisOption.X;
            sut.VertexDiameter = 150;
            sut.EdgeThickness = 115;
            sut.OutlineThickness = 15;
            sut.BackgroundColor = "red";
            sut.VertexColor = "green";
            sut.EdgeColor = "blue";
            sut.OutlineColor = "yellow";
            sut.InvertYAxis = false;
            sut.PadImage = false;

            Assert.Equal(1640, sut.MajorAxisSize);
            Assert.Equal(AxisOption.X, sut.MajorAxis);
            Assert.Equal(150, sut.VertexDiameter);
            Assert.Equal(115, sut.EdgeThickness);
            Assert.Equal(15, sut.OutlineThickness);
            Assert.Equal("red", sut.BackgroundColor);
            Assert.Equal("green", sut.VertexColor);
            Assert.Equal("blue", sut.EdgeColor);
            Assert.Equal("yellow", sut.OutlineColor);
            Assert.False(sut.InvertYAxis);
            Assert.False(sut.PadImage);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void PaddingThickness()
        {
            var sut = new SvgFormatterSettings();

            Assert.Equal(30, sut.PaddingThickness);
            sut.OutlineThickness = 0;
            Assert.Equal(25, sut.PaddingThickness);
            sut.VertexDiameter = 0;
            Assert.Equal(new Rational(15, 2), sut.PaddingThickness);
            sut.OutlineThickness = 5;
            Assert.Equal(new Rational(25, 2), sut.PaddingThickness);
            sut.PadImage = false;
            Assert.Equal(0, sut.PaddingThickness);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void OverridesFromGraph()
        {
            var source = new Graph2D(false);
            source.SetGraphMetadatum("major_axis_size", "200");
            Assert.Equal(200, new SvgFormatterSettings(source).MajorAxisSize);

            source.SetGraphMetadatum("major_axis", "X");
            Assert.Equal(AxisOption.X, new SvgFormatterSettings(source).MajorAxis);
            source.SetGraphMetadatum("major_axis", "x");
            Assert.Equal(AxisOption.X, new SvgFormatterSettings(source).MajorAxis);

            source.SetGraphMetadatum("major_axis", "Y");
            Assert.Equal(AxisOption.Y, new SvgFormatterSettings(source).MajorAxis);
            source.SetGraphMetadatum("major_axis", "y");
            Assert.Equal(AxisOption.Y, new SvgFormatterSettings(source).MajorAxis);

            source.SetGraphMetadatum("vertex_size", "100");
            Assert.Equal(100, new SvgFormatterSettings(source).VertexDiameter);

            source.SetGraphMetadatum("edge_size", "50");
            Assert.Equal(50, new SvgFormatterSettings(source).EdgeThickness);

            source.SetGraphMetadatum("outline_size", "6");
            Assert.Equal(6, new SvgFormatterSettings(source).OutlineThickness);

            source.SetGraphMetadatum("background_color", "pink");
            Assert.Equal("pink", new SvgFormatterSettings(source).BackgroundColor);

            source.SetGraphMetadatum("vertex_color", "purple");
            Assert.Equal("purple", new SvgFormatterSettings(source).VertexColor);

            source.SetGraphMetadatum("edge_color", "brown");
            Assert.Equal("brown", new SvgFormatterSettings(source).EdgeColor);

            source.SetGraphMetadatum("outline_color", "puce");
            Assert.Equal("puce", new SvgFormatterSettings(source).OutlineColor);

            source.SetGraphMetadatum("invert_y", "false");
            Assert.False(new SvgFormatterSettings(source).InvertYAxis);

            source.SetGraphMetadatum("pad", "false");
            Assert.False(new SvgFormatterSettings(source).PadImage);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            var sut = new SvgFormatterSettings();

            Assert.Throws<ArgumentNullException>("source", () => { new SvgFormatterSettings(null); });
            Assert.Throws<ArgumentNullException>("value", () => { sut.BackgroundColor = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.EdgeColor = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.EdgeThickness = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.MajorAxisSize = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.OutlineColor = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.OutlineThickness = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.VertexColor = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.VertexDiameter = null; });

            Assert.Throws<ArgumentOutOfRangeException>("value",
                () => { sut.BackgroundColor = string.Empty; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.EdgeColor = string.Empty; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.OutlineColor = string.Empty; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.VertexColor = string.Empty; });

            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.EdgeThickness = -1; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.MajorAxisSize = 0; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.OutlineThickness = -1; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.VertexDiameter = -1; });
        }
    }
}
