using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnaryHeap.Algorithms;
using UnaryHeap.DataType;

namespace Qtwols
{
    public static class QuakeExtensions
    {
        public static QuakeSpatial.Brush CreateSpatialBrush(MapBrush brush)
        {
            var brushMaterial = brush.GetBrushMaterial();

            var map = new Dictionary<Hyperplane3D, Func<Facet3D, QuakeSurface>>();
            foreach (var plane in brush.Planes)
            {
                map.Add(plane.GetHyperplane(), (facet) =>
                    new QuakeSurface(facet, plane.Texture, QuakeSurface.AIR, brushMaterial));
            }

            return QuakeSurface.Spatial.ReifyImplicitBrush(map);
        }

        static int GetBrushMaterial(this MapBrush brush)
        {
            var textureName = brush.Planes[0].Texture.Name;

            if (textureName.StartsWith("*lava", StringComparison.Ordinal))
                return QuakeSurface.LAVA;
            else if (textureName.StartsWith("*slime", StringComparison.Ordinal))
                return QuakeSurface.SLIME;
            else if (textureName.StartsWith('*'))
                return QuakeSurface.WATER;
            else if (textureName.StartsWith("sky", StringComparison.Ordinal))
                return QuakeSurface.SKY;
            else
                return QuakeSurface.SOLID;
        }

        static Hyperplane3D GetHyperplane(this MapPlane plane)
        {
            return new Hyperplane3D(
                new Point3D(plane.P3X, plane.P3Y, plane.P3Z),
                new Point3D(plane.P2X, plane.P2Y, plane.P2Z),
                new Point3D(plane.P1X, plane.P1Y, plane.P1Z)
            );
        }

        public static void SaveRawFile(this QuakeSpatial.IBspTree tree,
            string filename, IEnumerable<QuakeSurface> extraSurfaces)
        {
            var surfaces = new List<QuakeSurface>(extraSurfaces);
            tree.InOrderTraverse((nodeIndex) =>
            {
                if (tree.IsLeaf(nodeIndex))
                    surfaces.AddRange(tree.Surfaces(nodeIndex).Select(s => s.Surface));
            });

            SaveRawFile(surfaces, filename);
        }

        public static void SaveRawFile(this IEnumerable<QuakeSurface> surfaces, string filename)
        {
            var textureNames = surfaces.Select(s => s.Texture.Name).Distinct().ToList();

            using (var writer = new BinaryWriter(File.Create(filename)))
            {
                writer.Write(textureNames.Count);

                foreach (var textureName in textureNames)
                {
                    var bytes = new byte[16];
                    Encoding.ASCII.GetBytes(textureName.ToUpperInvariant(), bytes);
                    writer.Write(bytes);

                    var vertexData = new List<float>();
                    var indices = new List<int>();
                    var i = 0;

                    foreach (var surface in surfaces.Where(
                        s => s.Texture.Name.Equals(textureName,
                            StringComparison.OrdinalIgnoreCase)))
                    {
                        var facet = surface.Facet;
                        var plane = facet.Plane;
                        var normalLength = Math.Sqrt(
                            (double)(plane.A.Squared + plane.B.Squared + plane.C.Squared));
                        var points = facet.Points.ToList();

                        foreach (var point in points)
                        {
                            vertexData.Add(Convert.ToSingle((double)point.X / 10.0));
                            vertexData.Add(Convert.ToSingle((double)point.Y / 10.0));
                            vertexData.Add(Convert.ToSingle((double)point.Z / 10.0));
                            vertexData.Add(Convert.ToSingle((double)plane.A / normalLength));
                            vertexData.Add(Convert.ToSingle((double)plane.B / normalLength));
                            vertexData.Add(Convert.ToSingle((double)plane.C / normalLength));
                            surface.MapTexture(point, out float u, out float v);
                            vertexData.Add(u);
                            vertexData.Add(v);
                        }

                        // Reverse winding for Godot
                        indices.AddRange(facet.Triangulate().SelectMany(tuple =>
                            new int[] { tuple.Item3 + i, tuple.Item2 + i, tuple.Item1 + i }));
                        i += points.Count;
                    }

                    writer.Write(vertexData.Count / 8);
                    foreach (var coord in vertexData)
                        writer.Write(coord);
                    writer.Write(indices.Count);
                    foreach (var index in indices)
                        writer.Write(index);
                }
            }
        }
    }

    public class QuakeSpatial : Spatial3D<QuakeSurface> { }

    public class QuakeSurface : QuakeSpatial.SurfaceBase
    {
        public static readonly QuakeSpatial Spatial = new();

        public static readonly int AIR; // = 0
        public static readonly int WATER = 1;
        public static readonly int SLIME = 2;
        public static readonly int LAVA = 3;
        public static readonly int SKY = 4;
        public static readonly int SOLID = 5;

        public PlaneTexture Texture { get; private set; }

        public override QuakeSurface Cosurface
        {
            get { return new QuakeSurface(Facet.Cofacet, Texture, BackMaterial, FrontMaterial); }
        }

        public QuakeSurface(Facet3D facet, PlaneTexture texture,
            int frontMaterial, int backMaterial)
            : base(facet, frontMaterial, backMaterial)
        {
            Texture = texture;
        }

        public override int? HintLevel
        {
            get
            {
                if (Texture.Name.StartsWith("HINT", StringComparison.OrdinalIgnoreCase))
                {
                    return int.Parse(Texture.Name.AsSpan(4), CultureInfo.InvariantCulture);
                }
                else
                {
                    return null;
                }
            }
        }

        public override void Split(Hyperplane3D partitioningPlane,
            out QuakeSurface frontSurface, out QuakeSurface backSurface)
        {
            frontSurface = null;
            backSurface = null;
            Facet.Split(partitioningPlane,
                out Facet3D frontFacet, out Facet3D backFacet);
            if (frontFacet != null)
                frontSurface = new QuakeSurface(frontFacet, Texture,
                    FrontMaterial, BackMaterial);
            if (backFacet != null)
                backSurface = new QuakeSurface(backFacet, Texture,
                    FrontMaterial, BackMaterial);
        }

        public override QuakeSurface FillFront(int material)
        {
            return new QuakeSurface(Facet, Texture, material, BackMaterial);
        }

        public void MapTexture(Point3D point, out float u, out float v)
        {
            var Aabs = Facet.Plane.A.AbsoluteValue;
            var Babs = Facet.Plane.B.AbsoluteValue;
            var Cabs = Facet.Plane.C.AbsoluteValue;

            double U, V;

            if (Cabs >= Aabs && Cabs >= Babs)
            {
                U = (double)point.X;
                V = -(double)point.Y;
            }
            else if (Aabs >= Babs)
            {
                U = (double)point.Y;
                V = -(double)point.Z;
            }
            else
            {
                U = (double)point.X;
                V = -(double)point.Z;
            }

            // Scale texture coordinates
            U /= Texture.ScaleX;
            V /= Texture.ScaleY;

            // Rotate texture coordinates and return
            var rotRad = Texture.Rotation / 180.0 * Math.PI;
            var sin = Math.Sin(rotRad);
            var cos = Math.Cos(rotRad);

            var Urot = (float)(cos * U - sin * V);
            var Vrot = (float)(sin * U + cos * V);

            u = Urot + Texture.OffsetX;
            v = Vrot + Texture.OffsetY;
        }

        public override QuakeSurface HealEdges(List<Facet3D> facets)
        {
            var extraPoints = facets.SelectMany(facet => facet.Points)
                .Where(p => Facet.Plane.DetermineHalfspaceOf(p) == 0)
                .Except(Facet.Points)
                .Distinct()
                .ToList();

            return new QuakeSurface(Facet.AddPointsToEdge(extraPoints),
                Texture, FrontMaterial, BackMaterial);
        }

        public override bool IsTwoSided
        {
            get { return BackMaterial < SKY; }
        }
    }
}
