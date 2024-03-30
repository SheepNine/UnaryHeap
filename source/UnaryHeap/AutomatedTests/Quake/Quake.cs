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
        //public IDictionary<string, string> Attributes;
        //public IEnumerable<MapBrush> Brushes;
    }

    /*class MapBrush
    {
        //public IEnumerable<MapPlane> Planes;
    }

    class MapPlane
    {
        // ( -2160 1312 64 ) ( -2160 1280 64 ) ( -2160 1280 0 ) SKY4 0 0 0 1.000000 1.000000
        //public MapPoint P1;
        //public MapPoint P2;
        //public MapPoint P3;
        //public string TextureName;
        //public Rational OffsetX;
        //public Rational OffsetY;
        //public Rational Rotation;
        //public Rational ScaleX;
        //public Rational ScaleY;
    }

    class MapPoint
    {
        public Rational X;
        public Rational Y;
        public Rational Z;
    }*/

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
            //Console.WriteLine("Reading an entity");
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
                        ParseBrush(reader);
                        break;
                    case '"':
                        ParseAttribute(reader);
                        break;
                    case -1:
                        throw new InvalidDataException("Unexpected EoF");
                    default:
                        throw new InvalidDataException($"Unexpected char ${reader.Peek()}");
                }
            }
            Chomp(reader, '}');
            return null;
        }

        private static void ParseBrush(TextReader reader)
        {
            //Console.WriteLine("\tWith brush:");
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
                var texture = ChompToken(reader);

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

                //Console.WriteLine($"\t|{p1X}|{p1Y}|{p1Z}|{p2X}|{p2Y}|{p2Z}|{p3X}|{p3Y}|{p3Z}"
                //    + $"|{texture}|{offsetX}|{offsetY}|{rotation}|{scaleX}|{scaleY}|");
            }
            Chomp(reader, '}');
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

        private static void ParseAttribute(TextReader reader)
        {
            var attributeName = ParseString(reader);
            ChompWhitespace(reader);
            var attributeValue = ParseString(reader);
            //Console.WriteLine($"\t{attributeName} = {attributeValue}");
        }

        private static object ParseString(TextReader reader)
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
        [Test]
        public void DM2()
        {
            var output = QuakeMap.ParseMap(@"C:\Users\marsh\source\repos\Website\maps\DM2.MAP");
        }

        [Test]
        public void All()
        {
            foreach (var file in
                    Directory.GetFiles(@"C:\Users\marsh\source\repos\Website\maps", " *.MAP"))
                QuakeMap.ParseMap(file);
        }
    }
}
