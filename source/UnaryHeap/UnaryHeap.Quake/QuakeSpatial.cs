using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
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

            var brushPlanes = brush.Planes.Select(p => p.GetHyperplane()).ToArray();
            var surfaces = brush.Planes.Select((plane, i) =>
            {
                var facet = new Facet3D(brushPlanes[i], 100000);
                foreach (var j in Enumerable.Range(0, brush.Planes.Count))
                {
                    if (facet == null)
                        break;
                    if (i == j)
                        continue;
                    facet.Split(brushPlanes[j],
                        out Facet3D front, out Facet3D back);
                    facet = back;
                }
                return facet == null ? null : new QuakeSurface(facet, plane.Texture,
                    QuakeSpatial.AIR, brushMaterial, brushPlanes);
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
        /// <param name="textures">The textures of the BSP file.</param>
        /// <param name="tree">The BSP tree to write.</param>
        /// <param name="filename">The name of the file to which to write.</param>
        /// <param name="extraSurfaces">Any additional surfaces.</param>
        public static void SaveRawFile(this QuakeSpatial.IBspTree tree,
            IDictionary<string, BspFile.Texture> textures, string filename,
            IEnumerable<QuakeSurface> extraSurfaces)
        {
            var surfaces = new List<QuakeSurface>(extraSurfaces);
            tree.InOrderTraverse((nodeIndex) =>
            {
                if (tree.IsLeaf(nodeIndex))
                    surfaces.AddRange(tree.Surfaces(nodeIndex).Select(s => s.Surface));
            });

            SaveRawFile(surfaces, textures, filename);
        }

        /// <summary>
        /// Write a list of surfaces to a file.
        /// </summary>
        /// <param name="textures">The textures of the BSP file.</param>
        /// <param name="surfaces">The surfaces to write.</param>
        /// <param name="filename">The name of the file to which to write.</param>
        public static void SaveRawFile(this IEnumerable<QuakeSurface> surfaces,
            IDictionary<string, BspFile.Texture> textures, string filename)
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
                            surface.MapTexture(textures, point, out float u, out float v);
                            vertexData.Add(u);
                            vertexData.Add(v);
                        }
                        var faceIdx = Enumerable.Range(i, points.Count).ToList();

                        while (faceIdx.Count > 2)
                        {
                            var check = 0;

                            while (true)
                            {
                                var p1 = points[faceIdx[(check + 0) % faceIdx.Count] - i];
                                var p2 = points[faceIdx[(check + 1) % faceIdx.Count] - i];
                                var p3 = points[faceIdx[(check + 2) % faceIdx.Count] - i];

                                if (AreColinear(p1, p2, p3))
                                {
                                    check += 1;
                                    if (check == faceIdx.Count)
                                        throw new InvalidOperationException(
                                            "All points degenerate");
                                }
                                else
                                {
                                    // Reversed winding for Godot
                                    indices.Add(faceIdx[(check + 2) % faceIdx.Count]);
                                    indices.Add(faceIdx[(check + 1) % faceIdx.Count]);
                                    indices.Add(faceIdx[(check + 0) % faceIdx.Count]);
                                    faceIdx.RemoveAt(check + 1);
                                    break;
                                }
                            }
                        }

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

        static bool AreColinear(Point3D p1, Point3D p2, Point3D p3)
        {
            var v1 = new Point3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            var v2 = new Point3D(p3.X - p1.X, p3.Y - p1.Y, p3.Z - p1.Z);

            return v1.X * v2.Y == v2.X * v1.Y
                && v1.Y * v2.Z == v2.Y * v1.Z
                && v1.Z * v2.X == v2.Z * v1.X;
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
        private QuakeSpatial() : base(new QuakeDebug()) { }

        class QuakeDebug : IDebug
        {
            public void SplittingPlaneChosen(long elapsedTimeMs, int surfaceCount,
                int depth, Hyperplane3D partitionPlane)
            {
                if (depth > 3) return;

                Console.WriteLine($"\tSplitting plane chosen for {surfaceCount} surfaces at "
                    + $"level {depth} in {elapsedTimeMs} ms");
            }

            public void InsideFilled(Point3D interiorPoint, HashSet<BigInteger> result,
                int leafCount)
            {
            }
        }
    }

    /// <summary>
    /// Represents a surface of a Quake map brush.
    /// </summary>
    public class QuakeSurface : QuakeSpatial.SurfaceBase
    {
        /// <summary>
        /// The texture of the surface.
        /// </summary>
        public PlaneTexture Texture { get; private set; }
        Hyperplane3D[] brushPlanes;

        /// <summary>
        /// Gets a copy of a surface with the front and back sides reversed.
        /// </summary>
        public override QuakeSurface Cosurface
        {
            get
            {
                return new QuakeSurface(Facet.Cofacet, Texture,
                    BackMaterial, FrontMaterial, brushPlanes);
            }
        }

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
        /// <param name="brushPlanes">The planes of the parent brush.</param>
        public QuakeSurface(Facet3D facet, PlaneTexture texture,
            int frontMaterial, int backMaterial, Hyperplane3D[] brushPlanes)
            : base(facet, frontMaterial, backMaterial)
        {
            Texture = texture;
            this.brushPlanes = brushPlanes;
        }

        /// <summary>
        /// Checks if this surface is a 'hint surface' used to speed up the first few levels
        /// of BSP partitioning by avoiding an exhaustive search for a balanced plane.
        /// </summary>
        /// <param name="depth">The current depth of the BSP tree.</param>
        /// <returns>True of this surface should be used for a partitioning plane
        /// (and discarded from the final BSP tree), false otherwise.</returns>
        public override bool IsHintSurface(int depth)
        {
            return Texture.Name == $"HINT{depth}";
        }

        /// <summary>
        /// Splits a surface into two subsurfaces lying on either side of a
        /// partitioning plane.
        /// If surface lies on the partitioningPlane, it should be considered in the
        /// front halfspace of partitioningPlane if its front halfspace is identical
        /// to that of partitioningPlane. Otherwise, it should be considered in the 
        /// back halfspace of partitioningPlane.
        /// </summary>
        /// <param name="partitioningPlane">The plane used to split surface.</param>
        /// <param name="frontSurface">The subsurface of surface lying in the front
        /// halfspace of partitioningPlane, or null, if surface is entirely in the
        /// back halfspace of partitioningPlane.</param>
        /// <param name="backSurface">The subsurface of surface lying in the back
        /// halfspace of partitioningPlane, or null, if surface is entirely in the
        /// front halfspace of partitioningPlane.</param>
        public override void Split(Hyperplane3D partitioningPlane,
            out QuakeSurface frontSurface, out QuakeSurface backSurface)
        {
            frontSurface = null;
            backSurface = null;
            Facet.Split(partitioningPlane,
                out Facet3D frontFacet, out Facet3D backFacet);
            if (frontFacet != null)
                frontSurface = new QuakeSurface(frontFacet, Texture,
                    FrontMaterial, BackMaterial, brushPlanes);
            if (backFacet != null)
                backSurface = new QuakeSurface(backFacet, Texture,
                    FrontMaterial, BackMaterial, brushPlanes);
        }

        /// <summary>
        /// Makes a copy of a surface, with the front material replaced.
        /// </summary>
        /// <param name="material">The material to fill in the front.</param>
        /// <returns>The copied surface.</returns>
        public override QuakeSurface FillFront(int material)
        {
            return new QuakeSurface(Facet, Texture, material, BackMaterial, brushPlanes);
        }

        /// <summary>
        /// Computes the texture coordinates of a given point.
        /// </summary>
        /// <param name="textures">The texture data of the map.</param>
        /// <param name="point">The point to map.</param>
        /// <param name="u">The U coordinate of the point.</param>
        /// <param name="v">The V coordinate of the point.</param>
        public void MapTexture(IDictionary<string, BspFile.Texture> textures, Point3D point,
            out float u, out float v)
        {
            if (!textures.ContainsKey(Texture.Name.ToUpperInvariant()))
            {
                u = 0;
                v = 0;
                return;
            }

            // cref QBSP's ParseBrush() method for more details on how textures get mapped
            var texture = textures[Texture.Name.ToUpperInvariant()];
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

            u = (Urot + Texture.OffsetX) / texture.Width;
            v = (Vrot + Texture.OffsetY) / texture.Height;
        }

        /// <summary>
        /// Whether this surface is two-sided (i.e. both its front and back halves are
        /// interior spaces.
        /// </summary>
        public override bool IsTwoSided
        {
            get { return BackMaterial < QuakeSpatial.SKY; }
        }

        /// <summary>
        /// Makes a copy of a surface that has no T-joins with the given facet.
        /// </summary>
        /// <param name="facet">The facet to heal with.</param>
        /// <returns>The healed surface.</returns>
        public override QuakeSurface HealWith(Facet3D facet)
        {
            var newPoints = facet.Points
                .Where(point => Facet.Plane.DetermineHalfspaceOf(point) == 0
                    && brushPlanes.All(plane => plane.DetermineHalfspaceOf(point) <= 0))
                .Except(Facet.Points)
                .Distinct()
                .ToList();
            if (newPoints.Count == 0)
                return this;
            else 
                return new QuakeSurface(Facet.Heal(newPoints), Texture,
                    FrontMaterial, BackMaterial, brushPlanes);
        }
    }
}
