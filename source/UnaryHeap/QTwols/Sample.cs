using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.DataType;

namespace Qtwols
{
    class Surface : Spatial2D<Surface>.SurfaceBase
    {
        public Surface(Facet2D facet, int frontDensity, int backDensity)
            : base(facet, frontDensity, backDensity)
        {
        }

        /// <summary>
        /// Two-sided surfaces are used in a few places:
        /// * During CSG, to emit cosurfaces in the output
        /// * During portalization, to allow a portal to be created (one-sided lines will clip
        ///   a BSP tree node's portals away
        /// </summary>
        public override bool IsTwoSided
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// This is called at the end of CSG for each two-sideed surface, so that the CSG
        /// output contains facets for both sides of the two-sided surface.
        /// </summary>
        public override Surface Cosurface
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// This method is used to locate hit surfaces in the input list of surfaces to BSP.
        /// Hint surfaces do not added to the BSP tree.
        /// TODO: Recast as int? HintLevel. Current implementation will leave them in if they
        /// are not used as a splitting plane.
        /// </summary>
        public override bool IsHintSurface(int depth)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a copy of the surface with the front material replaced. Nothing special here;
        /// only needs to be implemented because the Spatial library can't know the semantics
        /// of the surface constructor.
        /// </summary>
        public override Surface FillFront(int material)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a copy of this surface with the edges healed together.
        /// In 2D, this is not needed and can be a no-op.
        /// In 3D, the facet class provides an AddPointsToEdge method to create the healed facet.
        /// In higher dimensions, it isn't clear what this has to do.
        /// </summary>
        public override Surface HealEdges(List<Facet2D> facets)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Divides a surface by the given splitting plane. The Facet classes provide a splitting
        /// implementaiton, so this code only needs to do any additional work for splitting
        /// the surface that is implementation-specific (e.g. for DooM, keeping track of the
        /// linedef for a seg.
        /// </summary>
        public override void Split(Hyperplane2D partitioningPlane,
            out Surface frontSurface, out Surface backSurface)
        {
            throw new NotImplementedException();
        }
    }

    internal class Sample
    {
        public static void Derp()
        {
            var brushSurfaces = Enumerable.Empty<Surface>();
            var interiorPoints = Enumerable.Empty<Point2D>();

            // --- Initialize the spatial object ---
            // The Spatial generic class has already been specialized twice, in the form of
            // Spatial2D and Spatial3D, which provides the geometry algorithm implementations
            // while leaving the application-specific interpretataion of 'surface' open for
            // customization.

            var spatial = new Spatial2D<Surface>();


            // --- Set up and run CSG ---
            // Constructive solid geometry is primarily a 3D concern; 2D games like DooM
            // would generally require the level editor to directly specify the geometry and thus
            // not require or provide a CSG step.
            // In contrast, 3D levels were usually edited by specifying a set of 'brushes'
            // representing convex polytopes of solid material, and would construct walls,
            // ceilings, floors, decorations, and other level geometry by combining them
            // together. Quake, in particular, would allow for different brush 'matrials' of
            // liquids, solids, and sky textures. The CSG step would process this implicit form
            // which was more tractible from an editing standpoint and produce a set of surfaces
            // which would be passed to later stages.
            // Individual brushes are implicity defined by specifying the set of hyperplanes that
            // make up each surface of the brush, as well as texture information and any other
            // game-specific metadata for the surface.
            // The Spatial library provides a utility method for reifying this implicit definition
            // into a concrete set of surfaces by computing the real facets that result from 
            // intersecting the hyperplanes. These brushes are assigned a 'material' density value
            // which can be thought of as a priority (larger numbers being higher).
            // These brushes can then be passed to the ConstructSolidGeometry method, which will
            // take the brush surfaces, and clip them so that parts of surfaces which lie within
            // other brushes of equal or higher density are removed, and parts which lie within
            // brushes of lower density have their front surface density values updated to reflect
            // this fact. Surfaces will always be oriented such that the higher density value is
            // on the back halfspace of the surface.
            // As a last step, those surfaces which are two-sided have copies of their cosurface
            // included in the results. This allows for effects such as Quake's water brushes
            // where the water surface is visible from both sides.

            var brushes = new[]
            {
                spatial.ReifyImplicitBrush(new Dictionary<Hyperplane2D, Func<Facet2D, Surface>>())
            };
            var surfaces = spatial.ConstructSolidGeometry(brushes);


            // --- Set up and run BSP ---
            // Fundamentally, the Binary Space Paritioning data structure can be thought of as
            // a multi-dimensional binary search structure. The input to partitioning is a set
            // of surfaces, and the algorithm proceeds by building a tree with the following
            // rules:
            // * if the set of surfaces is convex (no surface behind any other), then return
            //   a leaf node containing those surfaces
            // * otherwise, choose an arbitrary hyperplane which divides the set into two
            //   non-empty subsets, and return a branch node with the splitting plane and
            //   child nodes set to the result of recursinvely partitioning each subset
            // In the case that a surface intersects the partitioning plane, it is split into
            // two pieces and each one is put into one of the two subsets.
            // The Spatial implementaion is truly general. Some presentations of the BSP
            // data structure assume that one of the surfaces is chosen as the splitting surface
            // (as this would allow for the algorithm to build a tree where each leaf and branch
            // contained exactly one surface). In contrast, the Spatial algorithm lets any
            // arbitrary hyperplane be chosen, so long as it results in two non-empty subsets
            // of the input surfaces. Otherwise, the algorithm would enter infinite recursion
            // as it would not make progress towards its base case.
            // Users of the spatial class are free to implement whatever partition plane
            // selection strategy they wish. For convenience, the class also provides for an 
            // 'exhaustive' partitioning strategy which takes each plane in the input set,
            // and for each one, computes a score based on two factors:
            // * imbalance: the difference between the number of surfaces in each subset
            // * split: the number of surfaces which are split by the partitioning plane
            // The 'best' splitting plane is the one which minimizes these two values: an ideal
            // splitting plane has exactly equal subset sizes and does not need to split any
            // surfaces, as splitting surfaces adds to the final total of surfaces that need to
            // be processed by subsequent steps. The exhaustive strategy multiplies each factor
            // score by an input weight and chooses the plane with the lowest weighted score.
            // TODO: Enforce weight >= 1
            // The slowest part of the exhaustive partitioning algorithm is the first few levels
            // of the tree where a partitioning plane must be chosen from amongst the total input
            // geometry. Each subsequent level speeds up by a quadradic factor. To speed up the
            // construction process, a set of 'hint' surfaces may be included in the input. These
            // surfaces will not be added to the resulting BSP tree, but if present will be chosen
            // as the partitioning plane for the level of the tree to which they have been
            // assigned, greatly speeding up overall processing by letting the modeller use their
            // judgement to choose good 'global' splitting planes and letting the algorithm take
            // over finding good 'local' splitting planes.
            // The Spatial class also has a callback which can be helpful for emitting progress
            // update messages, if desired.
            // Although the partitioning algorithm, by its divide-and-conquer nature, is amenable
            // to parallelization, the Spatial class so far does not have this implemented, but
            // expect it in a future release.

            var partitioningStrategy =
                spatial.ExhaustivePartitionStrategy(splitWeight: 10, imbalanceWeight: 1);
            spatial.SplittingPlaneChosen += (sender, e) =>
            {
                Console.WriteLine($"Chose a plane for {e.SurfaceCount} surfaces " +
                    $"in {e.ElapsedMs} ms");
            };
            var bspTree = spatial.ConstructBspTree(partitioningStrategy, surfaces);


            // --- Set up and run portalization ---
            // After the BSP tree has been constructed, the relative position of the leaves of 
            // the tree can be inferred, but some algorithms require more direct connectivity
            // informmation, answering the question of 'if a vector originating in this leaf
            // crossed this hyperplane, what leaf would it end up in?'
            // These sorts of questions can be answered by constructing the 'portals' of the 
            // tree. Each portal lists a facet for the area of the portal, as well as the BSP
            // leaf node index of the leaves on either side of the portal.
            // Portals are constructed by taking a polytope that encompasses all of the surfaces
            // in the tree, and subdividing it according to the hierarchy of splitting planes
            // from the leaves of the tree. When a leaf is reached, the portals for tha leaf are
            // clipped so that only the portions of portals which lie on the front half of any
            // one-sided surfaces in the leaf remain.
            // As a by-product of portalization, the facets of the partitioning planes are also
            // returned, keyed along with information about their depth in the tree. This output
            // could be retained and reconstructed as hint surfaces to immediately recreate the 
            // same BSP tree for a given set of input surfaces, to make the second and subsequent
            // partitionings go very quickly.

            spatial.Portalize(bspTree,
                out IEnumerable<Portal<Facet2D>> portals,
                out IEnumerable<Tuple<int, Facet2D>> partitioningFacets);


            // --- Set up and run backface culling ---
            // Constructive Solid Geometry is a technique which can lead to a lot of redundant
            // surfaces. Consider the simple case in 3D of a square room made of six
            // axially-aligned brushes. Creating the brushes would result in 36 surfaces, but
            // a player starting inside of the box (and unable through normal means to leave it)
            // would only be able to see the six surfaces facing the interior.
            // To reduce this waste, the Spatial class has support for outside culling.
            // This process takes an input BSP tree, along with the generated portalization
            // of that tree, and a set of points that the modeller specifies as being 'interior'
            // points (commonly, this would the the locations of player starts, enemies, powerups,
            // and other such objects).
            // The leaves of the BSP tree are marked by applying a recursive traversal of the 
            // portals, starting from the interior points, to identify which leaves can be reached
            // and thus should be kept.
            // Any leaves which are not marked by this process, once completed, are removed
            // from the output BSP tree.

            spatial.InsideFilled += (sender, e) =>
            {
                Console.WriteLine($"Point {e.InteriorLeaves} found a new interior space; "
                    + $"now {e.InteriorLeaves.Count} of {e.LeafCount} "
                    + "leaves are marked as interior");
            };
            var culledBspTree = spatial.CullOutside(bspTree, portals, interiorPoints);


            // --- Set up and run T-Join healing ---
            // Consider a square polygon ABCD in 3D:
            // B-----A
            // |     |
            // |     |
            // |     |
            // C-----D
            // Now, split that polygon by a vertical plane, resulting in two polygons
            // ABFE and EFCD:
            // A--E--D
            // |  |  |
            // |  |  |
            // |  |  |
            // B--F--C
            // Next, split the left polygon by a horizontal plane. Because of the way that BSP
            // trees are constructed, the right polygon would not be split by that plane.
            // This would result in three polygons: AGHE, HGBF, and EFCD:
            // A--E--D
            // |  |  |
            // G--H  |
            // |  |  |
            // B--F--C
            // The splitting planes being in a T-shape gives rise to the name T-Join.
            // In this configuration, the geometry has a zero-area 'hole' EHF.
            // Mathematically, this would not be a problem, but in implementations these holes
            // present problems to graphics libraries that work very hard to guarantee
            // that two triangles that share a pair of vertices have no gaps in them, but make
            // no such guarantees for triangles whose edges are colinear but not coterminous.
            // This problem will manifest as single pixels on the screen which are not drawn by
            // either triangle, leading to holes where the background colour (or the previous
            // frame, if the buffer is not cleared between frames) leaks through.
            // To fix this, the Spatial library provides the HealEdges method.  This will sieve
            // the vertices of each polygon through the BSP tree, and ensure that in the output
            // BSP tree, the polygon EFCD is replaced by polygon EHFCD, thereby sealing the
            // geometry.
            // T-Joins are not an issue for the 2D case, as the facet-plane intersection is
            // already a single point, so there is no way to form a T-join.
            // A similar problem probably also exists in higher dimensions, but its character
            // is not clear to the author: do points need to be added to facets, or edges,
            // or what?


            var healedTree = spatial.HealEdges(bspTree);


            // --- Set up and run visible surface determination ---
            // The Spatial class does not current provide a VSD algorithm such as that which
            // is implemented by the qvis tool.
            // When it does, it will compute a potentially-visible set (PVS) for each BSP leaf
            // of the leaves which can be viewed from within that source leaf.
            // The portals of the BSP tree will be used as inputs to this process.
            // NB: For efficiency, the BSP tree resulting from an outside culling should be 
            // re-portalized and that tree/portal set used for VSD, rather than the original tree.
        }
    }
}
