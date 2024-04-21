using System.Linq;
using UnaryHeap.Quake;
using UnaryHeap.DataType;
using System.Globalization;
using System.Collections.Generic;
using System;
using System.IO;

namespace Qtwols
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var instrumentation = new Instrumentation();

            var LEVEL = args[0];
            var inputFile = $"C:\\Users\\marsh\\source\\repos\\UnaryHeap\\quakeMaps\\{LEVEL}.MAP";
            var bspHintFile = $"{inputFile}.bsphint";
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
                    && e.Attributes.ContainsKey("origin"))
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

            QuakeSpatial.Instance.Portalize(unculledTree, s => IsSolid(s.BackMaterial),
                out IEnumerable<QuakeSpatial.Portal> portals,
                out IEnumerable<Tuple<int, Facet3D>> bspHints
            );
            instrumentation.StepComplete("Portals computed");

            var subsetSizes = QuakeSpatial.Instance.LeafSubsets(unculledTree, portals);
            Console.Write('\t');
            Console.WriteLine(string.Join(", ", subsetSizes));

            var culledTree = QuakeSpatial.Instance.CullOutside(
                unculledTree, portals, interiorPoints, s => IsSolid(s.BackMaterial));
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
    }
}
