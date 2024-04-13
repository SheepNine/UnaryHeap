using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace UnaryHeap.Quake
{
    /// <summary>
    /// Represents an entity in a Quake .map source file.
    /// </summary>
    public class MapEntity
    {
        /// <summary>
        /// The attribute keys/values of the entity.
        /// </summary>
        public IDictionary<string, string> Attributes { get { return attributes; } }
        private readonly Dictionary<string, string> attributes;

        /// <summary>
        /// The brushes of the entity.
        /// </summary>
        public IEnumerable<MapBrush> Brushes { get { return brushes; } }
        private readonly MapBrush[] brushes;

        /// <summary>
        /// The number of brushes of the entityt.
        /// </summary>
        public int NumBrushes { get { return brushes.Length; } }

        /// <summary>
        /// Initializes a new instance of the MapEntity class.
        /// </summary>
        /// <param name="attributes">The attributes keys/values of the entity.</param>
        /// <param name="brushes">The brushes of the entity.</param>
        public MapEntity(IDictionary<string, string> attributes, IEnumerable<MapBrush> brushes)
        {
            this.attributes = new Dictionary<string, string>(attributes);
            this.brushes = brushes.ToArray();
        }
    }

    /// <summary>
    /// Represents a brush in an entity of a Quake .map source file.
    /// </summary>
    public class MapBrush
    {
        /// <summary>
        /// The planes that comprise the brush.
        /// </summary>
        public IList<MapPlane> Planes { get { return planes; } }
        private readonly MapPlane[] planes;

        /// <summary>
        /// Initializes a new instance of the MapBrush class.
        /// </summary>
        /// <param name="planes">The planes that comprise the brush.</param>
        public MapBrush(IEnumerable<MapPlane> planes)
        {
            this.planes = planes.ToArray();
        }
    }

    /// <summary>
    /// Represents a single plane of a brush of a Quake .map source file.
    /// </summary>
    public class MapPlane
    {
        // e.g. ( -2160 1312 64 ) ( -2160 1280 64 ) ( -2160 1280 0 ) SKY4 0 0 0 1.000000 1.000000

        /// <summary>
        /// Surface point 1's x-coordinate.
        /// </summary>
        public int P1X { get; private set; }

        /// <summary>
        /// Surface point 1's y-coordinate.
        /// </summary>
        public int P1Y { get; private set; }

        /// <summary>
        /// Surface point 1's z-coordinate.
        /// </summary>
        public int P1Z { get; private set; }

        /// <summary>
        /// Surface point 2's x-coordinate.
        /// </summary>
        public int P2X { get; private set; }

        /// <summary>
        /// Surface point 2's y-coordinate.
        /// </summary>
        public int P2Y { get; private set; }

        /// <summary>
        /// >Surface point 2's z-coordinate.
        /// </summary>
        public int P2Z { get; private set; }

        /// <summary>
        /// Surface point 3's x-coordinate.
        /// </summary>
        public int P3X { get; private set; }

        /// <summary>
        /// Surface point 3's y-coordinate.
        /// </summary>
        public int P3Y { get; private set; }

        /// <summary>
        /// Surface point 3's z-coordinate.
        /// </summary>
        public int P3Z { get; private set; }

        /// <summary>
        /// Surface texture data.
        /// </summary>
        public PlaneTexture Texture { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MapPlane class.
        /// </summary>
        /// <param name="p1x">Surface point 1's x-coordinate.</param>
        /// <param name="p1y">Surface point 1's y-coordinate.</param>
        /// <param name="p1z">Surface point 1's z-coordinate.</param>
        /// <param name="p2x">Surface point 2's x-coordinate.</param>
        /// <param name="p2y">Surface point 2's y-coordinate.</param>
        /// <param name="p2z">Surface point 2's z-coordinate.</param>
        /// <param name="p3x">Surface point 3's x-coordinate.</param>
        /// <param name="p3y">Surface point 3's y-coordinate.</param>
        /// <param name="p3z">Surface point 3's z-coordinate.</param>
        /// <param name="texture">Surface texture data.</param>
        public MapPlane(int p1x, int p1y, int p1z, int p2x, int p2y, int p2z,
            int p3x, int p3y, int p3z, PlaneTexture texture)
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
            Texture = texture;
        }
    }

    /// <summary>
    /// Represents the texure data of a map plane.
    /// </summary>
    public class PlaneTexture
    {
        /// <summary>
        /// Surface texture name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Surface texture's x-offset.
        /// </summary>
        public int OffsetX { get; private set; }

        /// <summary>
        /// >Surface texture's y-offset.
        /// </summary>
        public int OffsetY { get; private set; }

        /// <summary>
        /// Surface texture's rotation.
        /// </summary>
        public int Rotation { get; private set; }

        /// <summary>
        /// Surface texture's x-scale.
        /// </summary>
        public double ScaleX { get; private set; }

        /// <summary>
        /// Surface texture's y-scale.
        /// </summary>
        public double ScaleY { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MapPlane class.
        /// </summary>
        /// <param name="textureName">Surface texture name.</param>
        /// <param name="offsetX">Surface texture's x-offset.</param>
        /// <param name="offsetY">Surface texture's y-offset.</param>
        /// <param name="rotation">Surface texture's rotation.</param>
        /// <param name="scaleX">Surface texture's x-scale.</param>
        /// <param name="scaleY">Surface texture's y-scale.</param>
        public PlaneTexture(string textureName, int offsetX,
            int offsetY, int rotation, double scaleX, double scaleY)
        {
            Name = textureName;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Rotation = rotation;
            ScaleX = scaleX;
            ScaleY = scaleY;
        }
    }

    /// <summary>
    /// Provides static methods for reading Quake .map source files.
    /// </summary>
    public static class MapFileFormat
    {
        /// <summary>
        /// Read a Quake .map source file from disk.
        /// </summary>
        /// <param name="filename">The filename from which to read.</param>
        /// <returns>The entities described by the map file.</returns>
        public static MapEntity[] Load(string filename)
        {
            using var reader = File.OpenText(filename);
            return Load(reader);
        }

        /// <summary>
        /// Read a Quake .map source file from a reader.
        /// </summary>
        /// <param name="reader">The TextReader from which to read.</param>
        /// <returns>The entities described by the map file.</returns>
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
                    int.Parse(p1X, CultureInfo.InvariantCulture),
                    int.Parse(p1Y, CultureInfo.InvariantCulture),
                    int.Parse(p1Z, CultureInfo.InvariantCulture),
                    int.Parse(p2X, CultureInfo.InvariantCulture),
                    int.Parse(p2Y, CultureInfo.InvariantCulture),
                    int.Parse(p2Z, CultureInfo.InvariantCulture),
                    int.Parse(p3X, CultureInfo.InvariantCulture),
                    int.Parse(p3Y, CultureInfo.InvariantCulture),
                    int.Parse(p3Z, CultureInfo.InvariantCulture),
                    new PlaneTexture(
                        textureName,
                        int.Parse(offsetX, CultureInfo.InvariantCulture),
                        int.Parse(offsetY, CultureInfo.InvariantCulture),
                        int.Parse(rotation, CultureInfo.InvariantCulture),
                        double.Parse(scaleX, CultureInfo.InvariantCulture),
                        double.Parse(scaleY, CultureInfo.InvariantCulture)
                    )
                ));
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
