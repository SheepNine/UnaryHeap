using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.DataType;

namespace UnaryHeap.Quake
{
    /// <summary>
    /// TODO
    /// </summary>
    public static class QuakeExtensions
    {
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public static QuakeSpatial.Brush CreateSpatialBrush(MapBrush brush)
        {
            var firstMapPlane = brush.Planes[0];
            var brushMaterial = QuakeSpatial.SOLID;

            if (firstMapPlane.Texture.Name.StartsWith("*lava", StringComparison.Ordinal))
                brushMaterial = QuakeSpatial.LAVA;
            else if (firstMapPlane.Texture.Name.StartsWith("*slime", StringComparison.Ordinal))
                brushMaterial = QuakeSpatial.SLIME;
            else if (firstMapPlane.Texture.Name.StartsWith("*", StringComparison.Ordinal))
                brushMaterial = QuakeSpatial.WATER;
            else if (firstMapPlane.Texture.Name.StartsWith("sky", StringComparison.Ordinal))
                brushMaterial = QuakeSpatial.SKY;

            var surfaces = brush.Planes.Select((plane, i) =>
            {
                var facet = new Facet3D(plane.GetHyperplane(), 100000);
                foreach (var j in Enumerable.Range(0, brush.Planes.Count))
                {
                    if (facet == null)
                        break;
                    if (i == j)
                        continue;
                    facet.Split(GetHyperplane(brush.Planes[j]),
                        out Facet3D front, out Facet3D back);
                    facet = back;
                }
                return facet == null ? null : new QuakeSurface(facet, plane.Texture,
                    QuakeSpatial.AIR, brushMaterial);
            }).Where(plane => plane != null).ToList();

            if (surfaces.Count < 4)
            {
                throw new InvalidDataException("Degenerate brush");
            }

            return QuakeSpatial.Instance.MakeBrush(surfaces, brushMaterial);
        }

        static Hyperplane3D GetHyperplane(this MapPlane plane)
        {
            return new Hyperplane3D(
                new Point3D(plane.P3X, plane.P3Y, plane.P3Z),
                new Point3D(plane.P2X, plane.P2Y, plane.P2Z),
                new Point3D(plane.P1X, plane.P1Y, plane.P1Z)
            );
        }

        /// <summary>
        /// Write the surfaces of a BSP tree to a file.
        /// </summary>
        /// <param name="culledTree">The BSP tree to write.</param>
        /// <param name="filename">The name of the file to which to write.</param>
        public static void SaveRawFile(this QuakeSpatial.BspNode culledTree, string filename)
        {
            var vertsAndnormals = new List<float>();
            var indices = new List<int>();
            var i = 0;
            culledTree.InOrderTraverse((node) =>
            {
                if (!node.IsLeaf)
                    return;

                foreach (var surface in node.Surfaces)
                {
                    var facet = surface.Facet;
                    var plane = facet.Plane;
                    var normalLength = Math.Sqrt(
                        (double)(plane.A.Squared + plane.B.Squared + plane.C.Squared));

                    foreach (var point in facet.Points)
                    {
                        vertsAndnormals.Add(Convert.ToSingle((double)point.X / 10.0));
                        vertsAndnormals.Add(Convert.ToSingle((double)point.Y / 10.0));
                        vertsAndnormals.Add(Convert.ToSingle((double)point.Z / 10.0));
                        vertsAndnormals.Add(Convert.ToSingle((double)plane.A / normalLength));
                        vertsAndnormals.Add(Convert.ToSingle((double)plane.B / normalLength));
                        vertsAndnormals.Add(Convert.ToSingle((double)plane.C / normalLength));
                    }
                    for (int j = facet.Points.Count() - 1; j > 1; j--)
                    {
                        indices.Add(i + j);
                        indices.Add(i + j - 1);
                        indices.Add(i);
                    }
                    i += facet.Points.Count();
                }
            });

            using (var writer = new BinaryWriter(File.Create(filename)))
            {
                writer.Write(vertsAndnormals.Count / 6);
                foreach (var coord in vertsAndnormals)
                    writer.Write(coord);
                writer.Write(indices.Count);
                foreach (var index in indices)
                    writer.Write(index);
            }
        }
    }



    /// <summary>
    /// Implements the Spatial3D abstract class with customizations
    /// for surfaces that come from Quake maps.
    /// </summary>
    public class QuakeSpatial : Spatial3D<QuakeSurface>
    {
        /// <summary>
        /// Density value for empty space.
        /// </summary>
        public static readonly int AIR; // = 0

        /// <summary>
        /// Density value for water brushes.
        /// </summary>
        public static readonly int WATER = 1;

        /// <summary>
        /// Density value for slime brushes.
        /// </summary>
        public static readonly int SLIME = 2;

        /// <summary>
        /// Density value for lava brushes.
        /// </summary>
        public static readonly int LAVA = 3;

        /// <summary>
        /// Density value for sky brushes.
        /// </summary>
        public static readonly int SKY = 4;

        /// <summary>
        /// Density value for solid brushes.
        /// </summary>
        public static readonly int SOLID = 5;

        /// <summary>
        /// Gets the singleton instance of the QuakeSpatial class.
        /// </summary>
        public static readonly QuakeSpatial Instance = new();
        private QuakeSpatial() : base(new QuakeDimension(), new NoDebug()) { }

        class QuakeDimension : Dimension
        {
            public override Orthotope3D CalculateBounds(IEnumerable<QuakeSurface> surfaces)
            {
                return Orthotope3D.FromPoints(
                    surfaces.SelectMany(surface => surface.Facet.Points));
            }

            public override QuakeSurface FillFront(QuakeSurface surface, int newFrontMaterial)
            {
                return new QuakeSurface(surface.Facet, surface.Texture,
                    newFrontMaterial, surface.BackMaterial);
            }

            public override int GetBackMaterial(QuakeSurface surface)
            {
                return surface.BackMaterial;
            }

            public override int GetFrontMaterial(QuakeSurface surface)
            {
                return surface.FrontMaterial;
            }

            public override QuakeSurface GetCosurface(QuakeSurface surface)
            {
                return new QuakeSurface(GetCofacet(surface.Facet), surface.Texture,
                    surface.BackMaterial, surface.FrontMaterial);
            }

            public override Facet3D GetFacet(QuakeSurface surface)
            {
                return surface.Facet;
            }

            public override bool IsHintSurface(QuakeSurface surface, int depth)
            {
                return surface.Texture.Name == $"HINT{depth}";
            }

            public override void Split(QuakeSurface surface, Hyperplane3D partitioningPlane,
                out QuakeSurface frontSurface, out QuakeSurface backSurface)
            {
                if (null == surface)
                    throw new ArgumentNullException(nameof(surface));
                if (null == partitioningPlane)
                    throw new ArgumentNullException(nameof(partitioningPlane));

                frontSurface = null;
                backSurface = null;
                surface.Facet.Split(partitioningPlane,
                    out Facet3D frontFacet, out Facet3D backFacet);
                if (frontFacet != null)
                    frontSurface = new QuakeSurface(frontFacet, surface.Texture,
                        surface.FrontMaterial, surface.BackMaterial);
                if (backFacet != null)
                    backSurface = new QuakeSurface(backFacet, surface.Texture,
                        surface.FrontMaterial, surface.BackMaterial);
            }
        }
    }

    /// <summary>
    /// Represents a surface of a Quake map brush.
    /// </summary>
    public class QuakeSurface
    {
        /// <summary>
        /// The facet of this surface.
        /// </summary>
        public Facet3D Facet { get; private set; }

        /// <summary>
        /// The texture of the surface.
        /// </summary>
        public PlaneTexture Texture { get; private set; }

        /// <summary>
        /// The material on the front of the surface.
        /// </summary>
        public int FrontMaterial { get; private set; }

        /// <summary>
        /// The material on the front of the surface.
        /// </summary>
        public int BackMaterial { get; private set; }

        /// <summary>
        /// Initialies a new instance of the QuakeSurface class.
        /// </summary>
        /// <param name="facet">The facet of this surface.</param>
        /// <param name="texture">The texture of the surface.</param>
        /// <param name="frontMaterial">
        /// The material on the front of the surface.
        /// </param>
        /// <param name="backMaterial">
        /// The material on the front of the surface.
        /// </param>
        public QuakeSurface(Facet3D facet, PlaneTexture texture,
            int frontMaterial, int backMaterial)
        {
            Facet = facet;
            Texture = texture;
            FrontMaterial = frontMaterial;
            BackMaterial = backMaterial;
        }
    }
}
