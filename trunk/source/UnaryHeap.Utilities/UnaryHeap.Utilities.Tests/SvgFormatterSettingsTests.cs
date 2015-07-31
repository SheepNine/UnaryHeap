using System;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class SvgFormatterSettingsTests
    {
        [Fact]
        public void Defaults()
        {
            var sut = new SvgFormatterSettings();

            Assert.Equal(640, sut.MajorAxisSize);
            Assert.Equal(AxisOption.FromData, sut.MajorAxis);
            Assert.Equal(50, sut.VertexDiameter);
            Assert.Equal(15, sut.LineThickness);
            Assert.Equal(5, sut.OutlineThickness);
            Assert.Equal("lightgray", sut.BackgroundColor);
            Assert.Equal("white", sut.VertexColor);
            Assert.Equal("darkgray", sut.LineColor);
            Assert.Equal("black", sut.OutlineColor);
            Assert.True(sut.InvertYAxis);
            Assert.True(sut.PadImage);
        }

        [Fact]
        public void Mutators()
        {
            var sut = new SvgFormatterSettings();

            sut.MajorAxisSize = 1640;
            sut.MajorAxis = AxisOption.X;
            sut.VertexDiameter = 150;
            sut.LineThickness = 115;
            sut.OutlineThickness = 15;
            sut.BackgroundColor = "red";
            sut.VertexColor = "green";
            sut.LineColor = "blue";
            sut.OutlineColor = "yellow";
            sut.InvertYAxis = false;
            sut.PadImage = false;

            Assert.Equal(1640, sut.MajorAxisSize);
            Assert.Equal(AxisOption.X, sut.MajorAxis);
            Assert.Equal(150, sut.VertexDiameter);
            Assert.Equal(115, sut.LineThickness);
            Assert.Equal(15, sut.OutlineThickness);
            Assert.Equal("red", sut.BackgroundColor);
            Assert.Equal("green", sut.VertexColor);
            Assert.Equal("blue", sut.LineColor);
            Assert.Equal("yellow", sut.OutlineColor);
            Assert.False(sut.InvertYAxis);
            Assert.False(sut.PadImage);
        }

        [Fact]
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
        public void SimpleArgumentExceptions()
        {
            var sut = new SvgFormatterSettings();

            Assert.Throws<ArgumentNullException>("value", () => { sut.BackgroundColor = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.LineColor = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.LineThickness = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.MajorAxisSize = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.OutlineColor = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.OutlineThickness = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.VertexColor = null; });
            Assert.Throws<ArgumentNullException>("value", () => { sut.VertexDiameter = null; });

            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.BackgroundColor = string.Empty; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.LineColor = string.Empty; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.OutlineColor = string.Empty; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.VertexColor = string.Empty; });

            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.LineThickness = -1; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.MajorAxisSize = 0; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.OutlineThickness = -1; });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { sut.VertexDiameter = -1; });
        }
    }
}
