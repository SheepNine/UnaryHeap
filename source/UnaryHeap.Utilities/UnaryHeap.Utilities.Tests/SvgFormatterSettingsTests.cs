using System;
using UnaryHeap.DataType;
using NUnit.Framework;
using UnaryHeap.Graph;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class SvgFormatterSettingsTests
    {
        [Test]
        public void Defaults()
        {
            var sut = new SvgFormatterSettings();

            Assert.AreEqual((Rational)640, sut.MajorAxisSize);
            Assert.AreEqual(AxisOption.FromData, sut.MajorAxis);
            Assert.AreEqual((Rational)50, sut.VertexDiameter);
            Assert.AreEqual((Rational)15, sut.EdgeThickness);
            Assert.AreEqual((Rational)5, sut.OutlineThickness);
            Assert.AreEqual("lightgray", sut.BackgroundColor);
            Assert.AreEqual("white", sut.VertexColor);
            Assert.AreEqual("darkgray", sut.EdgeColor);
            Assert.AreEqual("black", sut.OutlineColor);
            Assert.True(sut.InvertYAxis);
            Assert.True(sut.PadImage);
        }

        [Test]
        public void DefaultsFromGraph()
        {
            var sut = new SvgFormatterSettings(new Graph2D(false));

            Assert.AreEqual((Rational)640, sut.MajorAxisSize);
            Assert.AreEqual(AxisOption.FromData, sut.MajorAxis);
            Assert.AreEqual((Rational)50, sut.VertexDiameter);
            Assert.AreEqual((Rational)15, sut.EdgeThickness);
            Assert.AreEqual((Rational)5, sut.OutlineThickness);
            Assert.AreEqual("lightgray", sut.BackgroundColor);
            Assert.AreEqual("white", sut.VertexColor);
            Assert.AreEqual("darkgray", sut.EdgeColor);
            Assert.AreEqual("black", sut.OutlineColor);
            Assert.True(sut.InvertYAxis);
            Assert.True(sut.PadImage);
        }

        [Test]
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

            Assert.AreEqual((Rational)1640, sut.MajorAxisSize);
            Assert.AreEqual(AxisOption.X, sut.MajorAxis);
            Assert.AreEqual((Rational)150, sut.VertexDiameter);
            Assert.AreEqual((Rational)115, sut.EdgeThickness);
            Assert.AreEqual((Rational)15, sut.OutlineThickness);
            Assert.AreEqual("red", sut.BackgroundColor);
            Assert.AreEqual("green", sut.VertexColor);
            Assert.AreEqual("blue", sut.EdgeColor);
            Assert.AreEqual("yellow", sut.OutlineColor);
            Assert.False(sut.InvertYAxis);
            Assert.False(sut.PadImage);
        }

        [Test]
        public void PaddingThickness()
        {
            var sut = new SvgFormatterSettings();

            Assert.AreEqual((Rational)30, sut.PaddingThickness);
            sut.OutlineThickness = 0;
            Assert.AreEqual((Rational)25, sut.PaddingThickness);
            sut.VertexDiameter = 0;
            Assert.AreEqual(new Rational(15, 2), sut.PaddingThickness);
            sut.OutlineThickness = 5;
            Assert.AreEqual(new Rational(25, 2), sut.PaddingThickness);
            sut.PadImage = false;
            Assert.AreEqual((Rational)0, sut.PaddingThickness);
        }

        [Test]
        public void OverridesFromGraph()
        {
            var source = new Graph2D(false);
            source.SetGraphMetadatum("major_axis_size", "200");
            Assert.AreEqual((Rational)200, new SvgFormatterSettings(source).MajorAxisSize);

            source.SetGraphMetadatum("major_axis", "X");
            Assert.AreEqual(AxisOption.X, new SvgFormatterSettings(source).MajorAxis);
            source.SetGraphMetadatum("major_axis", "x");
            Assert.AreEqual(AxisOption.X, new SvgFormatterSettings(source).MajorAxis);

            source.SetGraphMetadatum("major_axis", "Y");
            Assert.AreEqual(AxisOption.Y, new SvgFormatterSettings(source).MajorAxis);
            source.SetGraphMetadatum("major_axis", "y");
            Assert.AreEqual(AxisOption.Y, new SvgFormatterSettings(source).MajorAxis);

            source.SetGraphMetadatum("vertex_size", "100");
            Assert.AreEqual((Rational)100, new SvgFormatterSettings(source).VertexDiameter);

            source.SetGraphMetadatum("edge_size", "50");
            Assert.AreEqual((Rational)50, new SvgFormatterSettings(source).EdgeThickness);

            source.SetGraphMetadatum("outline_size", "6");
            Assert.AreEqual((Rational)6, new SvgFormatterSettings(source).OutlineThickness);

            source.SetGraphMetadatum("background_color", "pink");
            Assert.AreEqual("pink", new SvgFormatterSettings(source).BackgroundColor);

            source.SetGraphMetadatum("vertex_color", "purple");
            Assert.AreEqual("purple", new SvgFormatterSettings(source).VertexColor);

            source.SetGraphMetadatum("edge_color", "brown");
            Assert.AreEqual("brown", new SvgFormatterSettings(source).EdgeColor);

            source.SetGraphMetadatum("outline_color", "puce");
            Assert.AreEqual("puce", new SvgFormatterSettings(source).OutlineColor);

            source.SetGraphMetadatum("invert_y", "false");
            Assert.False(new SvgFormatterSettings(source).InvertYAxis);

            source.SetGraphMetadatum("pad", "false");
            Assert.False(new SvgFormatterSettings(source).PadImage);
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            var sut = new SvgFormatterSettings();

            Assert.Throws<ArgumentNullException>(
                () => { new SvgFormatterSettings(null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.BackgroundColor = null; });
            Assert.Throws<ArgumentNullException>(
                () => { sut.EdgeColor = null; });
            Assert.Throws<ArgumentNullException>(
                () => { sut.EdgeThickness = null; });
            Assert.Throws<ArgumentNullException>(
                () => { sut.MajorAxisSize = null; });
            Assert.Throws<ArgumentNullException>(
                () => { sut.OutlineColor = null; });
            Assert.Throws<ArgumentNullException>(
                () => { sut.OutlineThickness = null; });
            Assert.Throws<ArgumentNullException>(
                () => { sut.VertexColor = null; });
            Assert.Throws<ArgumentNullException>(
                () => { sut.VertexDiameter = null; });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.BackgroundColor = string.Empty; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.EdgeColor = string.Empty; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.OutlineColor = string.Empty; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.VertexColor = string.Empty; });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.EdgeThickness = -1; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.MajorAxisSize = 0; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.OutlineThickness = -1; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.VertexDiameter = -1; });
        }
    }
}
