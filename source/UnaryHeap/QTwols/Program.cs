using System.Linq;
using UnaryHeap.Quake;
using UnaryHeap.DataType;
using System.Globalization;

namespace Qtwols
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string LEVEL = "E1M1";
            var inputFile = $"C:\\Users\\marsh\\source\\repos\\UnaryHeap\\quakeMaps\\{LEVEL}.MAP";
            var unculledOutput = $"C:\\Users\\marsh\\Documents\\FirstGoLang\\{LEVEL}_nocull.raw";
            var culledOutput = $"C:\\Users\\marsh\\Documents\\FirstGoLang\\{LEVEL}.raw";

            var entities = MapFileFormat.Load(inputFile);
            var worldSpawnEntity = entities.Single(
                entity => entity.Attributes["classname"] == "worldspawn");
            var brushes = worldSpawnEntity.Brushes
                .Select(QuakeExtensions.CreateSpatialBrush).ToList();
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

            var csgSurfaces = QuakeSpatial.Instance.ConstructSolidGeometry(brushes).Where(s =>
                s.FrontMaterial != QuakeSpatial.SKY && s.FrontMaterial != QuakeSpatial.SOLID
            );
            var unculledTree = QuakeSpatial.Instance.ConstructBspTree(
                QuakeSpatial.Instance.ExhaustivePartitionStrategy(1, 10), csgSurfaces);
            var portals = QuakeSpatial.Instance.Portalize(unculledTree, s =>
                s.BackMaterial == QuakeSpatial.SKY || s.BackMaterial == QuakeSpatial.SOLID
            );
            var culledTree = QuakeSpatial.Instance.CullOutside(
                unculledTree, portals, interiorPoints);

            unculledTree.SaveRawFile(unculledOutput);
            culledTree.SaveRawFile(culledOutput);
        }
    }
}
