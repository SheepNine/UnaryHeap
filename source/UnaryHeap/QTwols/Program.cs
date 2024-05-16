using System.Linq;
using UnaryHeap.Quake;
using UnaryHeap.DataType;
using System.Globalization;
using System.Collections.Generic;
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Qtwols
{
    public class Program
    {
        const string MapDirectory = @"..\..\..\..\quake_map_source";
        const string GolangResourceDirectory = @"C:\Users\marsh\Documents\FirstGoLang";

        public static void Main(string[] args)
        {
            var LEVEL = args[0];
            ReadPakData(LEVEL, out Color[] palette, out BspFile bsp);

            var instrumentation = new Instrumentation();

            var inputFile = Path.Combine(MapDirectory, Path.ChangeExtension(LEVEL, "MAP"));
            var bspHintFile = $"{inputFile}.bsphint";
            var culledOutput = Path.Combine(GolangResourceDirectory,
                Path.ChangeExtension(LEVEL, "raw"));

            var entities = MapFileFormat.Load(inputFile);
            instrumentation.StepComplete("Map loaded");

            var worldSpawnEntity = entities.Single(
                entity => entity.Attributes["classname"] == "worldspawn");
            var brushes = worldSpawnEntity.Brushes
                .Where(b => !b.IsSpecialBrush)
                .Select(QuakeExtensions.CreateSpatialBrush).ToList();
            instrumentation.StepComplete("Brushes calculated");

            var interiorPoints = entities.Where(e => e.NumBrushes == 0
                    && e.Attributes.ContainsKey("origin")
                    && e.Attributes["classname"] != "path_corner"
                    && e.Attributes["classname"] != "light")
                .Select(e =>
                {
                    var tokens = e.Attributes["origin"].Split();
                    return new Point3D(
                        int.Parse(tokens[0], CultureInfo.InvariantCulture),
                        int.Parse(tokens[1], CultureInfo.InvariantCulture),
                        int.Parse(tokens[2], CultureInfo.InvariantCulture)
                    );
                }).ToList();
            // no instrumentation; finding interior points takes ~0ms

            //brushes.SelectMany(b => b.Surfaces).SaveRawFile(bsp.Textures, brushOutput);

            var csgSurfaces = QuakeSpatial.Instance.ConstructSolidGeometry(brushes).Where(
                s => !IsSolid(s.FrontMaterial)
            );

            var mobileBrushes = entities.Where(entity =>
                    entity.Attributes["classname"] != "worldspawn" && entity.NumBrushes > 0)
                .SelectMany(entity => entity.Brushes
                    .Where(b => !b.IsSpecialBrush)
                    .Select(QuakeExtensions.CreateSpatialBrush))
                .ToList();
            var mobileBrushSurfaces = QuakeSpatial.Instance.ConstructSolidGeometry(mobileBrushes)
                .Where(s => !IsSolid(s.FrontMaterial));

            instrumentation.StepComplete("CSG computed");

            if (File.Exists(bspHintFile))
            {
                Console.WriteLine("Loading bsphint file");
                csgSurfaces = csgSurfaces.Concat(LoadBspHints(bspHintFile));
            }

            var unculledTree = QuakeSpatial.Instance.ConstructBspTree(
                QuakeSpatial.Instance.ExhaustivePartitionStrategy(1, 10), csgSurfaces);
            instrumentation.StepComplete("BSP computed");

            var bounds = unculledTree.CalculateBoundingBox();
            Console.WriteLine($"Map extents: {bounds}");

            QuakeSpatial.Instance.Portalize(unculledTree,
                out IEnumerable<QuakeSpatial.Portal> portals,
                out IEnumerable<Tuple<int, Facet3D>> bspHints
            );
            instrumentation.StepComplete("Portals computed");

            var culledTree = QuakeSpatial.Instance.CullOutside(
                unculledTree, portals, interiorPoints);
            instrumentation.StepComplete("Culled BSP computed");
            Console.WriteLine($"{(culledTree.NodeCount + 1) / 2}/"
                + $"{(unculledTree.NodeCount + 1) / 2} leaves remain");

            //unculledTree.SaveRawFile(bsp.Textures, unculledOutput);
            culledTree.SaveRawFile(bsp.Textures, culledOutput, mobileBrushSurfaces);
            SaveBspHint(bspHintFile, bspHints.ToList());
            DumpTextures(palette, bsp, Path.Combine(GolangResourceDirectory, "textures", LEVEL));
            instrumentation.StepComplete("Output generated");

            instrumentation.JobComplete();
        }

        static bool IsSolid(int material)
        {
            return material == QuakeSpatial.SKY || material == QuakeSpatial.SOLID;
        }

        static void SaveBspHint(string bspHintFile, List<Tuple<int, Facet3D>> bspHints)
        {
            using (var writer = File.CreateText(bspHintFile))
            {
                writer.WriteLine(bspHints.Count);
                foreach (var bspHint in bspHints)
                {
                    var points = bspHint.Item2.Points.ToList();
                    writer.WriteLine(bspHint.Item1);
                    writer.WriteLine(points.Count);
                    foreach (var point in points)
                    {
                        writer.WriteLine(point.ToString());
                    }
                }
            }
        }

        static List<QuakeSurface> LoadBspHints(string bspHintFile)
        {
            var result = new List<QuakeSurface>();

            using (var reader = File.OpenText(bspHintFile))
            {
                var hintCount = ReadInt(reader);
                foreach (var i in Enumerable.Range(0, hintCount))
                {
                    var depth = ReadInt(reader);
                    var pointCount = ReadInt(reader);
                    var points = new List<Point3D>();
                    foreach (var j in Enumerable.Range(0, pointCount))
                        points.Add(Point3D.Parse(reader.ReadLine()));

                    var plane = new Hyperplane3D(points[0], points[1], points[2]);
                    var facet = new Facet3D(plane, points);

                    result.Add(new QuakeSurface(facet,
                        new PlaneTexture($"HINT{depth}", 0, 0, 0, 0, 0),
                        QuakeSpatial.AIR, QuakeSpatial.AIR, Array.Empty<Hyperplane3D>()));
                }
            }

            return result;
        }

        private static int ReadInt(StreamReader reader)
        {
            return int.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
        }

        private static void DumpTextures(Color[] palette, BspFile bsp, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            foreach (var kv in bsp.Textures)
            {
                var texture = kv.Value;
                var outputFile = Path.Combine(outputDirectory,
                    Path.ChangeExtension(texture.Name.ToUpperInvariant().Replace('*', '★'),
                        "png"));
                texture.SaveMip(palette, outputFile, ImageFormat.Png);
            }
        }

        const string Pak0Filename =
            @"C:\Program Files (x86)\Steam\steamapps\common\Quake\id1\PAK0.PAK";
        const string Pak1Filename =
            @"C:\Program Files (x86)\Steam\steamapps\common\Quake\id1\PAK1.PAK";

        static void ReadPakData(string level, out Color[] palette, out BspFile bsp)
        {
            var pak0 = new Pak1File(Pak0Filename);
            var pak1 = new Pak1File(Pak1Filename);

            palette = pak0.ReadPalette();

            if (level.Equals("start", StringComparison.Ordinal)
                || level.StartsWith("e1", StringComparison.Ordinal))
                bsp = pak0.ReadBsp(level);
            else
                bsp = pak1.ReadBsp(level);
        }
    }
}
