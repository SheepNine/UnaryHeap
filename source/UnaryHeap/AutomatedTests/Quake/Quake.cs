using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnaryHeap.Algorithms;
using UnaryHeap.DataType;
using UnaryHeap.DataType.Tests;

namespace Quake
{
    class Facet
    {
        List<Point3D> winding;
        Hyperplane3D plane;

        public Facet(Hyperplane3D plane) : this(plane, plane.MakePolytope(100000))
        {
        }

        public Facet(Hyperplane3D plane, List<Point3D> winding)
        {
            this.plane = plane;
            this.winding = new List<Point3D>(winding);
        }

        public void Split(Hyperplane3D partitioningPlane, out Facet frontSurface,
            out Facet backSurface)
        {
            if (partitioningPlane.Equals(plane))
            {
                frontSurface = this;
                backSurface = null;
                return;
            }
            if (partitioningPlane.Coplane.Equals(plane))
            {
                frontSurface = null;
                backSurface = this;
                return;
            }

            var windingPointHalfspaces = winding.Select(point =>
                partitioningPlane.DetermineHalfspaceOf(point)).ToList();
            var windingPoints = new List<Point3D>(winding);

            if (windingPointHalfspaces.All(hs => hs >= 0))
            {
                frontSurface = this;
                backSurface = null;
                return;
            }
            if (windingPointHalfspaces.All(hs => hs <= 0))
            {
                frontSurface = null;
                backSurface = this;
                return;
            }

            for (var i = windingPoints.Count - 1; i >= 0; i--)
            {
                var j = (i + 1) % windingPoints.Count;

                if (windingPointHalfspaces[i] * windingPointHalfspaces[j] >= 0)
                    continue;

                windingPointHalfspaces.Insert(i + 1, 0);
                windingPoints.Insert(i + 1,
                    partitioningPlane.Pierce(windingPoints[i], windingPoints[j]));
            }
            throw new NotImplementedException("I don't know how to split yet!");
        }

        public bool InFrontOf(Facet target)
        {
            return winding.All(point => target.plane.DetermineHalfspaceOf(point) >= 0);
        }
    }

    class QuakeBSP : BinarySpacePartitioner<Facet, Hyperplane3D>
    {
        public QuakeBSP(IPartitioner<Facet, Hyperplane3D> partitioner) : base(partitioner)
        {
        }

        /// <summary>
        /// Checks whether two surfaces are mutually convex (that is, neither one is
        /// behind the other). Surfaces which are convex do not need to be partitioned.
        /// </summary>
        /// <param name="a">The first surface to check.</param>
        /// <param name="b">The second surface to check.</param>
        /// <returns>True, if a is in the front halfspace of b and vice versa;
        /// false otherwise.</returns>
        protected override bool AreConvex(Facet a, Facet b)
        {
            return a.InFrontOf(b) && b.InFrontOf(a);
        }

        /// <summary>
        /// Checks if a surface is a 'hint surface' used to speed up the first few levels
        /// of BSP partitioning by avoiding an exhaustive search for a balanced plane.
        /// </summary>
        /// <param name="surface">The surface to check.</param>
        /// <param name="depth">The current depth of the BSP tree.</param>
        /// <returns>True of this surface should be used for a partitioning plane
        /// (and discarded from the final BSP tree), false otherwise.</returns>
        protected override bool IsHintSurface(Facet surface, int depth)
        {
            // TODO: implement some hint surfaces
            return false;
        }

        /// <summary>
        /// Splits a surface into two subsurfaces lying on either side of a
        /// partitioning plane.
        /// If surface lies on the partitioningPlane, it should be considered in the
        /// front halfspace of partitioningPlane if its front halfspace is identical
        /// to that of partitioningPlane. Otherwise, it should be considered in the 
        /// back halfspace of partitioningPlane.
        /// </summary>
        /// <param name="surface">The surface to split.</param>
        /// <param name="partitioningPlane">The plane used to split surface.</param>
        /// <param name="frontSurface">The subsurface of surface lying in the front
        /// halfspace of partitioningPlane, or null, if surface is entirely in the
        /// back halfspace of partitioningPlane.</param>
        /// <param name="backSurface">The subsurface of surface lying in the back
        /// halfspace of partitioningPlane, or null, if surface is entirely in the
        /// front halfspace of partitioningPlane.</param>
        protected override void Split(Facet surface, Hyperplane3D partitioningPlane,
            out Facet frontSurface, out Facet backSurface)
        {
            surface.Split(partitioningPlane, out frontSurface, out backSurface);
        }
    }

    class MapEntity
    {
        public IDictionary<string, string> Attributes { get { return attributes; } }
        private readonly Dictionary<string, string> attributes;
        public IEnumerable<MapBrush> Brushes { get { return brushes; } }
        private readonly List<MapBrush> brushes;

        public MapEntity(Dictionary<string, string> attributes, List<MapBrush> brushes)
        {
            this.attributes = attributes;
            this.brushes = brushes;
        }
    }

    class MapBrush
    {
        public IEnumerable<MapPlane> Planes { get { return planes; } }
        private readonly List<MapPlane> planes;

        public MapBrush(List<MapPlane> planes)
        {
            this.planes = planes;
        }

        public List<Facet> MakeFacets()
        {
            return planes.Select((plane, i) =>
            {
                // TODO: precalculate hyperplanes; this is wasteful
                var facet = new Facet(new Hyperplane3D(plane.P1, plane.P2, plane.P3));
                foreach (var j in Enumerable.Range(0, planes.Count))
                {
                    if (facet == null)
                        break;

                    if (i == j)
                        continue;
                    var splitPlane = planes[j];
                    facet.Split(new Hyperplane3D(splitPlane.P1, splitPlane.P2, splitPlane.P3),
                        out Facet front, out Facet back);
                    facet = back;
                }
                return facet;
            }).Where(plane => plane != null).ToList();
        }
    }

    class MapPlane
    {
        // e.g. ( -2160 1312 64 ) ( -2160 1280 64 ) ( -2160 1280 0 ) SKY4 0 0 0 1.000000 1.000000
        public Point3D P1 { get; private set; }
        public Point3D P2 { get; private set; }
        public Point3D P3 { get; private set; }
        public string TextureName { get; private set; }
        public Rational OffsetX { get; private set; }
        public Rational OffsetY { get; private set; }
        public Rational Rotation { get; private set; }
        public Rational ScaleX { get; private set; }
        public Rational ScaleY { get; private set; }

        public MapPlane(Point3D p1, Point3D p2, Point3D p3, string textureName, Rational offsetX,
            Rational offsetY, Rational rotation, Rational scaleX, Rational scaleY)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            TextureName = textureName;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Rotation = rotation;
            ScaleX = scaleX;
            ScaleY = scaleY;
        }
    }

    static class QuakeMap
    {
        public static MapEntity[] ParseMap(string filename)
        {
            using var reader = File.OpenText(filename);
            return ParseMap(reader);
        }

        public static MapEntity[] ParseMap(TextReader reader)
        {
            var result = new List<MapEntity>();
            while (true)
            {
                ChompWhitespace(reader);
                if (reader.Peek() == -1) break;
                result.Add(ParseEntity(reader));
            }
            return result.ToArray();
        }

        private static MapEntity ParseEntity(TextReader reader)
        {
            var brushes = new List<MapBrush>();
            var attributes = new Dictionary<string, string>();
            Chomp(reader, '{');
            while (true)
            {
                ChompWhitespace(reader);
                if (reader.Peek() == '}')
                    break;

                switch (reader.Peek())
                {
                    case '}':
                        break;
                    case '{':
                        brushes.Add(ParseBrush(reader));
                        break;
                    case '"':
                        ParseAttribute(reader, attributes);
                        break;
                    case -1:
                        throw new InvalidDataException("Unexpected EoF");
                    default:
                        throw new InvalidDataException($"Unexpected char ${reader.Peek()}");
                }
            }
            Chomp(reader, '}');
            return new MapEntity(attributes, brushes);
        }

        private static MapBrush ParseBrush(TextReader reader)
        {
            var planes = new List<MapPlane>();

            Chomp(reader, '{');
            while (true)
            {
                ChompWhitespace(reader);
                if (reader.Peek() == '}')
                    break;

                Chomp(reader, '(');

                ChompWhitespace(reader);
                var p1X = ChompToken(reader);

                ChompWhitespace(reader);
                var p1Y = ChompToken(reader);

                ChompWhitespace(reader);
                var p1Z = ChompToken(reader);

                ChompWhitespace(reader);
                Chomp(reader, ')');

                ChompWhitespace(reader);
                Chomp(reader, '(');

                ChompWhitespace(reader);
                var p2X = ChompToken(reader);

                ChompWhitespace(reader);
                var p2Y = ChompToken(reader);

                ChompWhitespace(reader);
                var p2Z = ChompToken(reader);

                ChompWhitespace(reader);
                Chomp(reader, ')');

                ChompWhitespace(reader);
                Chomp(reader, '(');

                ChompWhitespace(reader);
                var p3X = ChompToken(reader);

                ChompWhitespace(reader);
                var p3Y = ChompToken(reader);

                ChompWhitespace(reader);
                var p3Z = ChompToken(reader);

                ChompWhitespace(reader);
                Chomp(reader, ')');

                ChompWhitespace(reader);
                var textureName = ChompToken(reader);

                ChompWhitespace(reader);
                var offsetX = ChompToken(reader);

                ChompWhitespace(reader);
                var offsetY = ChompToken(reader);

                ChompWhitespace(reader);
                var rotation = ChompToken(reader);

                ChompWhitespace(reader);
                var scaleX = ChompToken(reader);

                ChompWhitespace(reader);
                var scaleY = ChompToken(reader);

                var p1 = new Point3D(int.Parse(p1X), int.Parse(p1Y), int.Parse(p1Z));
                var p2 = new Point3D(int.Parse(p2X), int.Parse(p2Y), int.Parse(p2Z));
                var p3 = new Point3D(int.Parse(p3X), int.Parse(p3Y), int.Parse(p3Z));

                planes.Add(new MapPlane(
                    p1, p2, p3, textureName,
                    int.Parse(offsetX), int.Parse(offsetY), int.Parse(rotation),
                    Rationalize(scaleX), Rationalize(scaleY)));
            }
            Chomp(reader, '}');

            return new MapBrush(planes);
        }

        private static Rational Rationalize(string value)
        {
            var doubleValue = double.Parse(value);
            var result = new Rational(Convert.ToInt32(100000.0f * doubleValue), 100000);
            if ((double)result != doubleValue)
                throw new ArgumentOutOfRangeException($"Failed to rationalize ${value}");
            return result;
        }

        private static string ChompToken(TextReader reader)
        {
            var builder = new StringBuilder();
            while (true)
            {
                if (IsWhitespace(reader.Peek()) || IsSpecial(reader.Peek()))
                    break;
                builder.Append(SafeRead(reader));
            }
            return builder.ToString();
        }

        private static void ParseAttribute(TextReader reader, Dictionary<string, string> output)
        {
            var attributeName = ParseString(reader);
            ChompWhitespace(reader);
            var attributeValue = ParseString(reader);
            output[attributeName] = attributeValue;
        }

        private static string ParseString(TextReader reader)
        {
            var builder = new StringBuilder();
            Chomp(reader, '"');
            while (reader.Peek() != '"')
                builder.Append(SafeRead(reader));
            Chomp(reader, '"');
            return builder.ToString();
        }

        private static char SafeRead(TextReader reader)
        {
            if (reader.Peek() == -1)
                throw new InvalidDataException("Unexpected EoF");
            return (char)reader.Read();
        }

        private static void Chomp(TextReader reader, char expected)
        {
            if (reader.Peek() != expected)
                throw new InvalidDataException($"Expected {expected} but found {reader.Peek()}");
            reader.Read();
        }

        private static void ChompWhitespace(TextReader reader)
        {
            while (IsWhitespace(reader.Peek()))
                reader.Read();
        }

        private static bool IsWhitespace(int ci)
        {
            if (ci == -1) return false;
            var c = (char)ci;
            return c == ' ' || c == '\r' || c == '\n' || c == '\t';
        }

        private static bool IsSpecial(int ci)
        {
            if (ci == -1) return false;
            var c = (char)ci;
            return c == '{' || c == '}' || c == '(' || c == ')' || c == '"';
        }
    }

    [TestFixture]
    public class Quake
    {
        const string Dir = @"..\..\..\..\..\quakemaps";

        [Test]
        public void DM2()
        {
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            var output = QuakeMap.ParseMap(Path.Combine(Dir, "DM2.MAP"));
            Assert.AreEqual(271, output.Length);
            var worldSpawn = output.Single(
                entity => entity.Attributes["classname"] == "worldspawn");
            var facets = worldSpawn.Brushes.SelectMany(brush => brush.MakeFacets()).ToList();
            Assert.AreEqual(65536, facets.Count);
        }

        [Test]
        public void All()
        {
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            foreach (var file in Directory.GetFiles(Dir, "*.MAP"))
            {
                Console.WriteLine(file);
                QuakeMap.ParseMap(file);
            }
        }
    }
}
