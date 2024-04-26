using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
        where TSurface : Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>.SurfaceBase
    {
        /// <summary>
        /// Represents a convex polytope for constructive solid geometry.
        /// </summary>
        public class Brush
        {
            /// <summary>
            /// The surfaces of the polytope.
            /// </summary>
            public IEnumerable<TSurface> Surfaces { get { return surfaces; } }
            readonly TSurface[] surfaces;

            /// <summary>
            /// The bounding box that contains all the surfaces of the polytope.
            /// </summary>
            public TBounds Bounds { get; private set; }

            /// <summary>
            /// The material on the interior of the brush.
            /// </summary>
            public int Material { get; private set; }

            /// <summary>
            /// Initializes a new instance of the Brush class.
            /// </summary>
            /// <param name="surfaces">The surfaces of the polytope.</param>
            /// <param name="bounds">
            /// The bounding box that contains all the surfaces of the polytope.
            /// </param>
            /// <param name="material">The material on the interior of the brush.</param>
            public Brush(IEnumerable<TSurface> surfaces, TBounds bounds, int material)
            {
                this.surfaces = surfaces.ToArray();
                Bounds = bounds;
                Material = material;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Brush class.
        /// </summary>
        /// <param name="surfaces">The surfaces of the polytope.</param>
        /// <param name="material">The material on the interior of the brush.</param>
        public Brush MakeBrush(IEnumerable<TSurface> surfaces, int material)
        {
            return new Brush(surfaces,
                dimension.CalculateBounds(surfaces.Select(s => s.Facet)), material);
        }

        /// <summary>
        /// Perform constructive solid geometry on a set of brushes.
        /// </summary>
        /// <param name="brushes">The brushes from which to construct geometry.</param>
        /// <returns>A set of facets for the brushes.</returns>
        public IEnumerable<TSurface> ConstructSolidGeometry(IList<Brush> brushes)
        {
            var result = new List<TSurface>();

            foreach (var i in Enumerable.Range(0, brushes.Count))
            {
                var overwrite = false;
                var sourceBrush = brushes[i];
                List<TSurface> surfaces = sourceBrush.Surfaces.ToList();

                foreach (var j in Enumerable.Range(0, brushes.Count))
                {
                    if (i == j)
                    {
                        // Brushes don't clip with themselves
                        // but note that we have reached the source brush in the clip iteration
                        // to toggle the behaviour of facets coplanar with a clip brush surface
                        overwrite = true;
                        continue;
                    }

                    var clipBrush = brushes[j];

                    if (!dimension.BoundsOverlap(sourceBrush.Bounds, clipBrush.Bounds))
                    {
                        // All facets must lie outside of the clip brush so we can skip it
                        continue;
                    }

                    surfaces = ClipSurfaces(surfaces, clipBrush, overwrite);
                }

                result.AddRange(surfaces);
            }

            result.AddRange(result.ToList().Where(s => s.IsTwoSided).Select(s => s.Cosurface));

            return result;
        }

        private List<TSurface> ClipSurfaces(List<TSurface> surfaces,
            Brush clipBrush, bool overwrite)
        {
            var result = new List<TSurface>();

            foreach (var surface in surfaces)
            {
                ClipSurface(surface, clipBrush, overwrite,
                    out List<TSurface> inside, out List<TSurface> outside);

                if (inside.Count == 0)
                {
                    // Surface was entirely outside of clip brush, so retain the un-split
                    // portion only
                    result.Add(surface);
                }
                else
                {
                    result.AddRange(outside);
                    result.AddRange(inside.Where(surface =>
                            surface.BackMaterial > clipBrush.Material)
                        .Select(surface => surface.FillFront(clipBrush.Material)));
                }
            }

            return result;
        }

        private void ClipSurface(TSurface surface, Brush clipBrush, bool overwrite,
            out List<TSurface> inside, out List<TSurface> outside)
        {
            inside = new List<TSurface>();
            outside = new List<TSurface>();
            var coplanar = false;

            foreach (var plane in clipBrush.Surfaces.Select(
                s => dimension.GetPlane(s.Facet)))
            {
                if (plane.Equals(dimension.GetPlane(surface.Facet)))
                {
                    coplanar = true;
                    continue;
                }

                surface.Split(plane, out TSurface outsidePiece, out surface);

                if (outsidePiece != null)
                    outside.Add(outsidePiece);
                if (surface == null)
                    break;
            }

            if (surface != null)
            {
                if (coplanar)
                {
                    if (clipBrush.Material > surface.BackMaterial)
                    {
                        inside.Add(surface);
                    }
                    else if (clipBrush.Material < surface.BackMaterial)
                    {
                        outside.Add(surface);
                    }
                    else
                    {
                        (overwrite ? inside : outside).Add(surface);
                    }
                }
                else
                {
                    inside.Add(surface);
                }
            }
        }
    }
}
