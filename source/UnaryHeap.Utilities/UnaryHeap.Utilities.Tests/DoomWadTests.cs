using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.Doom;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class DoomWadTests
    {
        [Fact]
        public void DoomMapNames()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Ultimate Doom\base\DOOM.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            var sut = new DoomWad(new WadFile(wadFileName));

            Assert.Equal(new[] {
                "E1M1", "E1M2", "E1M3", "E1M4", "E1M5", "E1M6", "E1M7", "E1M8", "E1M9",
                "E2M1", "E2M2", "E2M3", "E2M4", "E2M5", "E2M6", "E2M7", "E2M8", "E2M9",
                "E3M1", "E3M2", "E3M3", "E3M4", "E3M5", "E3M6", "E3M7", "E3M8", "E3M9",
                "E4M1", "E4M2", "E4M3", "E4M4", "E4M5", "E4M6", "E4M7", "E4M8", "E4M9",
            }, sut.ListMaps());

            sut.CreateSvgOfMap("E2M3", @"C:\Users\SheepNine\Desktop\e1m1.svg");
        }

        [Fact]
        public void Doom2MapNames()
        {
            var wadFileName = @"D:\Steam\steamapps\common\Doom 2\base\DOOM2.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            var sut = new DoomWad(wadFileName);

            Assert.Equal(new[] {
                "MAP01", "MAP02", "MAP03", "MAP04", "MAP05", "MAP06", "MAP07", "MAP08",
                "MAP09", "MAP10", "MAP11", "MAP12", "MAP13", "MAP14", "MAP15", "MAP16",
                "MAP17", "MAP18", "MAP19", "MAP20", "MAP21", "MAP22", "MAP23", "MAP24",
                "MAP25", "MAP26", "MAP27", "MAP28", "MAP29", "MAP30", "MAP31", "MAP32",
            }, sut.ListMaps());
        }

        [Fact]
        public void TeethVertices()
        {
            var wadFileName =
                @"D:\Steam\steamapps\common\Master Levels of Doom\master\wads\TEETH.WAD";
            Assert.True(File.Exists(wadFileName), "Could not locate input file.");

            var sut = new DoomWad(wadFileName);

            var result = sut.GetVertices("MAP31");

            Assert.Equal(1415, result.Length);
            Assert.Equal(new Point2D(29, 537), result[0]);
            Assert.Equal(new Point2D(-107, 289), result[150]);
        }
    }

    public class DoomWad
    {
        WadFile data;

        public DoomWad(WadFile data)
        {
            this.data = data;
        }

        public DoomWad(string fileName)
            :this(new WadFile(fileName))
        {
        }

        public string[] ListMaps()
        {
            var result = new List<string>();

            foreach (var episode in Enumerable.Range(1, 4))
                foreach (var level in Enumerable.Range(1, 9))
                {
                    var name = string.Format("E{0}M{1}", episode, level);

                    if (-1 != data.FindLumpByName(name))
                        result.Add(name);
                }

            foreach (var level in Enumerable.Range(1, 32))
            {
                var name = string.Format("MAP{0:d2}", level);

                if (-1 != data.FindLumpByName(name))
                    result.Add(name);
            }

            return result.ToArray();
        }

        public Point2D[] GetVertices(string mapName)
        {
            var mapLumpIndex = data.FindLumpByName(mapName);

            if (-1 == mapLumpIndex)
                throw new ArgumentOutOfRangeException("mapName");

            var vertexLumpIndex = data.FindLumpByName("VERTEXES", mapLumpIndex);

            if (-1 == vertexLumpIndex)
                throw new InvalidDataException("Missing VERTEXES lump.");
            if (4 != vertexLumpIndex - mapLumpIndex)
                throw new InvalidDataException("VERTEXES lump not at expected location.");

            var vertexLumpData = data.GetLumpData(vertexLumpIndex);

            if (0 != vertexLumpData.Length % 4)
                throw new InvalidDataException("VERTEXES lump has incorrect size.");

            var result = new List<Point2D>();

            for (int i = 0; i < vertexLumpData.Length / 4; i++)
            {
                var x = (short)((vertexLumpData[4 * i + 1] << 8) | vertexLumpData[4 * i + 0]);
                var y = (short)((vertexLumpData[4 * i + 3] << 8) | vertexLumpData[4 * i + 2]);

                result.Add(new Point2D(x, y));
            }

            return result.ToArray();
        }

        public Linedef[] GetLinedefs(string mapName)
        {
            var mapLumpIndex = data.FindLumpByName(mapName);

            if (-1 == mapLumpIndex)
                throw new ArgumentOutOfRangeException("mapName");

            var linedefLumpIndex = data.FindLumpByName("LINEDEFS", mapLumpIndex);

            if (-1 == linedefLumpIndex)
                throw new InvalidDataException("Missing LINEDEFS lump.");
            if (2 != linedefLumpIndex - mapLumpIndex)
                throw new InvalidDataException("LINEDEFS lump not at expected location.");

            var linedefLumpData = data.GetLumpData(linedefLumpIndex);

            if (0 != linedefLumpData.Length % 14)
                throw new InvalidDataException("LINEDEFS lump has incorrect size.");

            var result = new List<Linedef>();

            for (int i = 0; i < linedefLumpData.Length / 14; i++)
                result.Add(new Linedef(linedefLumpData, 14 * i));

            return result.ToArray();
        }

        public void CreateSvgOfMap(string mapName, string outputFileName)
        {
            var vertices = GetVertices(mapName);
            var linedefs = GetLinedefs(mapName);

            var graph = new Graph2D(false);

            foreach (var vertex in vertices)
                if (false == graph.HasVertex(vertex))
                    graph.AddVertex(vertex);

            foreach (var linedef in linedefs)
            {
                var start = vertices[linedef.StartVertex];
                var end = vertices[linedef.EndVertex];

                if (graph.HasEdge(start, end))
                    continue;

                graph.AddEdge(start, end);

                if (-1 == linedef.LeftSidedef)
                {
                    graph.SetEdgeMetadatum(start, end, "color", "red");
                    graph.SetEdgeMetadatum(start, end, "order", "1");
                }
                else
                {
                    graph.SetEdgeMetadatum(start, end, "color", "yellow");
                    graph.SetEdgeMetadatum(start, end, "order", "0");
                }
            }


            var settings = new SvgFormatterSettings()
            {
                MajorAxisSize = 1400,
                VertexDiameter = 0,
                OutlineThickness = 0,
                BackgroundColor = "black",
                VertexColor = "green",
                EdgeThickness = 2
            };

            using (var stream = File.CreateText(outputFileName))
                SvgGraph2DFormatter.Generate(graph, stream, settings);
        }
    }

    public class Linedef
    {
        public short StartVertex { get; private set; }
        public short EndVertex { get; private set; }
        public short Flags { get; private set; }
        public short ActionCode { get; private set; }
        public short SectorTag { get; private set; }
        public short RightSidedef { get; private set; }
        public short LeftSidedef { get; private set; }

        public Linedef(byte[] data, int offset)
        {
            StartVertex = (short)((data[offset + 1] << 8) | data[offset + 0]);
            EndVertex = (short)((data[offset + 3] << 8) | data[offset + 2]);
            Flags = (short)((data[offset + 5] << 8) | data[offset + 4]);
            ActionCode = (short)((data[offset + 7] << 8) | data[offset + 6]);
            SectorTag = (short)((data[offset + 9] << 8) | data[offset + 8]);
            RightSidedef = (short)((data[offset + 11] << 8) | data[offset + 10]);
            LeftSidedef = (short)((data[offset + 13] << 8) | data[offset + 12]);
        }
    }
}
