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
        public static void Main(string[] args)
        {
            //var pak0filename =
            //    @"C:\Program Files (x86)\Steam\steamapps\common\Quake\id1\PAK0.PAK";
            //var pak1filename =
            //    @"C:\Program Files (x86)\Steam\steamapps\common\Quake\id1\PAK1.PAK";
            //RipTextureAssets(pak0filename, pak1filename);

            var instrumentation = new Instrumentation();

            var LEVEL = args[0];
            var inputFile = $"C:\\Users\\marsh\\source\\repos\\UnaryHeap\\quakeMaps\\{LEVEL}.MAP";
            var bspHintFile = $"{inputFile}.bsphint";
            var brushOutput = $"C:\\Users\\marsh\\Documents\\FirstGoLang\\{LEVEL}_brushes.raw";
            var unculledOutput = $"C:\\Users\\marsh\\Documents\\FirstGoLang\\{LEVEL}_nocull.raw";
            var culledOutput = $"C:\\Users\\marsh\\Documents\\FirstGoLang\\{LEVEL}.raw";

            var entities = MapFileFormat.Load(inputFile);
            instrumentation.StepComplete("Map loaded");

            var worldSpawnEntity = entities.Single(
                entity => entity.Attributes["classname"] == "worldspawn");
            var brushes = worldSpawnEntity.Brushes
                .Select(QuakeExtensions.CreateSpatialBrush).ToList();
            instrumentation.StepComplete("Brushes calculated");

            var interiorPoints = entities.Where(e => e.NumBrushes == 0
                    && e.Attributes.ContainsKey("origin")
                    && e.Attributes["classname"] != "path_corner")
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

            brushes.SelectMany(b => b.Surfaces).SaveRawFile(brushOutput);

            var csgSurfaces = QuakeSpatial.Instance.ConstructSolidGeometry(brushes).Where(
                s => !IsSolid(s.FrontMaterial)
            );
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

            var subsetSizes = QuakeSpatial.Instance.LeafSubsets(unculledTree, portals);
            Console.Write('\t');
            Console.WriteLine(string.Join(", ", subsetSizes));

            var culledTree = QuakeSpatial.Instance.CullOutside(
                unculledTree, portals, interiorPoints);
            instrumentation.StepComplete("Culled BSP computed");

            unculledTree.SaveRawFile(unculledOutput);
            culledTree.SaveRawFile(culledOutput);
            SaveBspHint(bspHintFile, bspHints.ToList());
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
                        QuakeSpatial.AIR, QuakeSpatial.AIR));
                }
            }

            return result;
        }

        private static int ReadInt(StreamReader reader)
        {
            return int.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
        }


        static void RipTextureAssets(string pak0filename, string pak1filename)
        {
            Directory.CreateDirectory("textures");
            var pak0 = new Pak1File(pak0filename);
            var palette = pak0.ReadPalette();

            foreach (var m in Enumerable.Range(1, 8))
                DumpTextures(palette, pak0.ReadBsp($"e1m{m}"));

            DumpTextures(palette, pak0.ReadBsp($"start"));

            var pak1 = new Pak1File(pak1filename);
            foreach (var e in Enumerable.Range(2, 3))
                foreach (var m in Enumerable.Range(1, 8))
                {
                    if (e == 2 && m == 8) continue;
                    if (e == 3 && m == 8) continue;
                    DumpTextures(palette, pak1.ReadBsp($"e{e}m{m}"));
                }
            foreach (var i in Enumerable.Range(1, 6))
                DumpTextures(palette, pak1.ReadBsp($"dm{i}"));
            DumpTextures(palette, pak1.ReadBsp($"end"));
        }

        private static void DumpTextures(Color[] palette, BspFile bsp)
        {
            var name = Path.GetFileNameWithoutExtension(bsp.Name);
            Directory.CreateDirectory($"textures/{name}");
            foreach (var texture in bsp.Textures)
            {
                if (texture == null) continue;
                texture.SaveMip(palette, $"textures/{name}/{texture.Name.Replace('*', '.')}.png",
                    ImageFormat.Png);
            }
        }
    }
}
