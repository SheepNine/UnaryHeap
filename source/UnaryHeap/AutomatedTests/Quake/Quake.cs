using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnaryHeap.DataType;

namespace AutomatedTests.Quake
{
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
