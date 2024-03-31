using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Quake
{
    class MapEntity
    {
        public IDictionary<string, string> Attributes { get { return attributes; } }
        private readonly Dictionary<string, string> attributes;
        public IEnumerable<MapBrush> Brushes { get { return brushes; } }
        private readonly MapBrush[] brushes;

        public MapEntity(IDictionary<string, string> attributes, IEnumerable<MapBrush> brushes)
        {
            this.attributes = new Dictionary<string, string>(attributes);
            this.brushes = brushes.ToArray();
        }
    }

    class MapBrush
    {
        public IList<MapPlane> Planes { get { return planes; } }
        private readonly MapPlane[] planes;

        public MapBrush(IEnumerable<MapPlane> planes)
        {
            this.planes = planes.ToArray();
        }
    }

    class MapPlane
    {
        // e.g. ( -2160 1312 64 ) ( -2160 1280 64 ) ( -2160 1280 0 ) SKY4 0 0 0 1.000000 1.000000
        public int P1X { get; private set; }
        public int P1Y { get; private set; }
        public int P1Z { get; private set; }
        public int P2X { get; private set; }
        public int P2Y { get; private set; }
        public int P2Z { get; private set; }
        public int P3X { get; private set; }
        public int P3Y { get; private set; }
        public int P3Z { get; private set; }
        public string TextureName { get; private set; }
        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }
        public int Rotation { get; private set; }
        public double ScaleX { get; private set; }
        public double ScaleY { get; private set; }

        public MapPlane(int p1x, int p1y, int p1z, int p2x, int p2y, int p2z,
            int p3x, int p3y, int p3z, string textureName, int offsetX,
            int offsetY, int rotation, double scaleX, double scaleY)
        {
            P1X = p1x;
            P1Y = p1y;
            P1Z = p1z;
            P2X = p2x;
            P2Y = p2y;
            P2Z = p2z;
            P3X = p3x;
            P3Y = p3y;
            P3Z = p3z;
            TextureName = textureName;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Rotation = rotation;
            ScaleX = scaleX;
            ScaleY = scaleY;
        }
    }

    static class MapFileFormat
    {
        public static MapEntity[] Load(string filename)
        {
            using var reader = File.OpenText(filename);
            return Load(reader);
        }

        public static MapEntity[] Load(TextReader reader)
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

        static MapEntity ParseEntity(TextReader reader)
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

        static MapBrush ParseBrush(TextReader reader)
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

                planes.Add(new MapPlane(
                    int.Parse(p1X), int.Parse(p1Y), int.Parse(p1Z),
                    int.Parse(p2X), int.Parse(p2Y), int.Parse(p2Z),
                    int.Parse(p3X), int.Parse(p3Y), int.Parse(p3Z),
                    textureName,
                    int.Parse(offsetX), int.Parse(offsetY), int.Parse(rotation),
                    double.Parse(scaleX), double.Parse(scaleY)));
            }
            Chomp(reader, '}');

            return new MapBrush(planes);
        }

        static string ChompToken(TextReader reader)
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

        static void ParseAttribute(TextReader reader, Dictionary<string, string> output)
        {
            var attributeName = ParseString(reader);
            ChompWhitespace(reader);
            var attributeValue = ParseString(reader);
            output[attributeName] = attributeValue;
        }

        static string ParseString(TextReader reader)
        {
            var builder = new StringBuilder();
            Chomp(reader, '"');
            while (reader.Peek() != '"')
                builder.Append(SafeRead(reader));
            Chomp(reader, '"');
            return builder.ToString();
        }

        static char SafeRead(TextReader reader)
        {
            if (reader.Peek() == -1)
                throw new InvalidDataException("Unexpected EoF");
            return (char)reader.Read();
        }

        static void Chomp(TextReader reader, char expected)
        {
            if (reader.Peek() != expected)
                throw new InvalidDataException($"Expected {expected} but found {reader.Peek()}");
            reader.Read();
        }

        static void ChompWhitespace(TextReader reader)
        {
            while (IsWhitespace(reader.Peek()))
                reader.Read();
        }

        static bool IsWhitespace(int ci)
        {
            if (ci == -1) return false;
            var c = (char)ci;
            return c == ' ' || c == '\r' || c == '\n' || c == '\t';
        }

        static bool IsSpecial(int ci)
        {
            if (ci == -1) return false;
            var c = (char)ci;
            return c == '{' || c == '}' || c == '(' || c == ')' || c == '"';
        }
    }
}
