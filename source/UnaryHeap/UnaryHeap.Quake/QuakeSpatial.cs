using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
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
        /// <param name="tree">The BSP tree to write.</param>
        /// <param name="filename">The name of the file to which to write.</param>
        public static void SaveRawFile(this QuakeSpatial.IBspTree tree, string filename)
        {
            var surfaces = new List<QuakeSurface>();
            tree.InOrderTraverse((nodeIndex) =>
            {
                if (tree.IsLeaf(nodeIndex))
                    surfaces.AddRange(tree.Surfaces(nodeIndex));
            });

            SaveRawFile(surfaces, filename);
        }

        /// <summary>
        /// Write a list of surfaces to a file.
        /// </summary>
        /// <param name="surfaces">The surfaces to write.</param>
        /// <param name="filename">The name of the file to which to write.</param>
        public static void SaveRawFile(this IEnumerable<QuakeSurface> surfaces, string filename)
        {
            var vertsAndnormals = new List<float>();
            var indices = new List<int>();
            var i = 0;

            foreach (var surface in surfaces)
            {
                var facet = surface.Facet;
                var plane = facet.Plane;
                var normalLength = Math.Sqrt(
                    (double)(plane.A.Squared + plane.B.Squared + plane.C.Squared));
                var points = facet.Points.ToList();

                foreach (var point in points)
                {
                    vertsAndnormals.Add(Convert.ToSingle((double)point.X / 10.0));
                    vertsAndnormals.Add(Convert.ToSingle((double)point.Y / 10.0));
                    vertsAndnormals.Add(Convert.ToSingle((double)point.Z / 10.0));
                    vertsAndnormals.Add(Convert.ToSingle((double)plane.A / normalLength));
                    vertsAndnormals.Add(Convert.ToSingle((double)plane.B / normalLength));
                    vertsAndnormals.Add(Convert.ToSingle((double)plane.C / normalLength));
                }
                var facetIndices = Enumerable.Range(i, points.Count).ToList();

                while (facetIndices.Count > 2)
                {
                    var check = 0;

                    while (true)
                    {
                        var p1 = points[facetIndices[(check + 0) % facetIndices.Count] - i];
                        var p2 = points[facetIndices[(check + 1) % facetIndices.Count] - i];
                        var p3 = points[facetIndices[(check + 2) % facetIndices.Count] - i];

                        if (AreColinear(p1, p2, p3))
                        {
                            check += 1;
                            if (check == facetIndices.Count)
                                throw new InvalidOperationException("All points degenerate");
                        }
                        else
                        {
                            // Reversed winding for Godot, which expects left-handed winding
                            indices.Add(facetIndices[(check + 2) % facetIndices.Count]);
                            indices.Add(facetIndices[(check + 1) % facetIndices.Count]);
                            indices.Add(facetIndices[(check + 0) % facetIndices.Count]);
                            facetIndices.RemoveAt(check + 1);
                            break;
                        }
                    }
                }

                i += points.Count;
            }

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
            public void SplittingPlaneChosen(long elapsedTimeMs, List<QuakeSurface> surfaces,
                int depth, Hyperplane3D partitionPlane)
            {
                if (depth > 3) return;

                Console.WriteLine($"\tSplitting plane chosen for {surfaces.Count} surfaces at "
                    + $"level {depth} in {elapsedTimeMs} ms");
            }

            public void PartitionOccurred(long elapsedTimeMs,
                List<QuakeSurface> surfacesToPartition, int depth, Hyperplane3D partitionPlane,
                List<QuakeSurface> frontSurfaces, List<QuakeSurface> backSurfaces)
            {
            }

            public void InsideFilled(Point3D interiorPoint, HashSet<int> result,
                int leafCount)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                        "\t{0,5}/{1,-5} leaves filled after considering point ({2})",
                        result.Count,
                        leafCount,
                        interiorPoint
                    )
                );
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

        /// <summary>
        /// Gets a copy of a surface with the front and back sides reversed.
        /// </summary>
        public override QuakeSurface Cosurface
        {
            get
            {
                return new QuakeSurface(Facet.Cofacet, Texture, BackMaterial, FrontMaterial);
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
        public QuakeSurface(Facet3D facet, PlaneTexture texture,
            int frontMaterial, int backMaterial)
            :base(facet, frontMaterial, backMaterial)
        {
            Texture = texture;
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
                    FrontMaterial, BackMaterial);
            if (backFacet != null)
                backSurface = new QuakeSurface(backFacet, Texture,
                    FrontMaterial, BackMaterial);
        }

        /// <summary>
        /// Makes a copy of a surface, with the front material replaced.
        /// </summary>
        /// <param name="material">The material to fill in the front.</param>
        /// <returns>The copied surface.</returns>
        public override QuakeSurface FillFront(int material)
        {
            return new QuakeSurface(Facet, Texture, material, BackMaterial);
        }

        /// <summary>
        /// Whether this surface is two-sided (i.e. both its front and back halves are
        /// interior spaces.
        /// </summary>
        public override bool IsTwoSided
        {
            get { return BackMaterial < QuakeSpatial.SKY; }
        }
    }
}
