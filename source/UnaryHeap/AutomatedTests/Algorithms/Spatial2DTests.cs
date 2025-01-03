﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnaryHeap.DataType;
using UnaryHeap.DataType.Tests;
using Assert = NUnit.Framework.Legacy.ClassicAssert;
using CollectionAssert = NUnit.Framework.Legacy.CollectionAssert;

namespace UnaryHeap.Algorithms.Tests
{
    [TestFixture]
    public class Spatial2DTests
    {
        [Test]
        public void Cofacets()
        {
            var p1 = new Point2D(1, 1);
            var p2 = new Point2D(2, 2);
            var facet = new Facet2D(new Hyperplane2D(p1, p2), p1, p2);

            var tree = ConstructBspTree(new[]
            {
                new VanillaSurface(facet, 0, 1),
                new VanillaSurface(facet.Cofacet, 0, 1),
            });

            CheckTree(tree,
            @"- (-1)x + (1)y + (0)
                | [1,1->2,2]
                | [2,2->1,1]");
        }

        [Test]
        public void ConvexBox()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    -1, 1,
                    -1, -1,
                    1, -1
                ).WithPolygon(
                    0, 1, 2, 3
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"| [-1,-1 ->  1,-1],
                [ 1,-1 ->  1, 1],
                [-1, 1 -> -1,-1],
                [ 1, 1 -> -1, 1]");

            var portals = Portalize(tree);
            CheckPortals(tree, portals, @"");
        }

        [Test]
        public void ConvexBoxInverted()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    1, -1,
                    -1, -1,
                    -1, 1
                ).WithPolygon(
                    0, 1, 2, 3
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (1)x + (0)y + (-1)
                | [1,1 -> 1,-1]
                - (0)x + (-1)y + (-1)
                  | [1,-1 -> -1,-1]
                  - (-1)x + (0)y + (-1)
                    | [-1,-1 -> -1,1]
                    | [-1,1 -> 1,1]");

            var portals = Portalize(tree);
            CheckPortals(tree, portals, @"
                (1) [1,2] -> [1,1] (14)
                (1) [1,-1] -> [1,-2] (5)
                (5) [-1,-1] -> [-2,-1] (13)
                (13) [-1,1] -> [-1,2] (14)
            ");
        }

        [Test]
        public void LShape()
        {
            var builder = new GraphBuilder().WithPoints(
                    2, 2,
                    -2, 2,
                    -2, 0,
                    0, 0,
                    0, -2,
                    2, -2
                ).WithPolygon(
                    0, 1, 2, 3, 4, 5
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (0)x + (1)y + (0)
                | [-2, 0 ->  0, 0],
                  [ 2, 0 ->  2, 2],
                  [-2, 2 -> -2, 0],
                  [ 2, 2 -> -2, 2]
                | [ 0,-2 ->  2,-2],
                  [ 2,-2 ->  2, 0],
                  [ 0, 0 ->  0,-2]");

            var portals = Portalize(tree);
            CheckPortals(tree, portals, @"
                (1) [0,0] -> [2,0] (2)
            ");

            // All leaves are interior so culling should not remove anything
            var culledTree = CullOutside(tree, portals, new[] { new Point2D(1, 1) });
            CheckTree(culledTree,
            @"- (0)x + (1)y + (0)
                | [-2, 0 ->  0, 0],
                  [ 2, 0 ->  2, 2],
                  [-2, 2 -> -2, 0],
                  [ 2, 2 -> -2, 2]
                | [ 0,-2 ->  2,-2],
                  [ 2,-2 ->  2, 0],
                  [ 0, 0 ->  0,-2]");
        }

        [Test]
        public void LShapeReverseSplittingPlane()
        {
            var builder = new GraphBuilder().WithPoints(
                    2, 2,
                    -2, 2,
                    -2, 0,
                    0, 0,
                    0, -2,
                    2, -2
                ).WithPolygon(
                    0, 1, 2, 3, 4, 5
                ).WithHint(
                    0, 4, 3
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (-1)x + (0)y + (0)
                | [-2, 0 ->  0, 0],
                  [-2, 2 -> -2, 0],
                  [ 0, 2 -> -2, 2]
                | [ 0,-2 ->  2,-2],
                  [ 2,-2 ->  2, 2],
                  [ 0, 0 ->  0,-2],
                  [ 2, 2 ->  0, 2]");

            var portals = Portalize(tree);
            CheckPortals(tree, portals, @"
                (1) [0,0] -> [0,2] (2)
            ");
        }

        [Test]
        public void SBlock()
        {
            var builder = new GraphBuilder().WithPoints(
                1, 0,
                2, 0,
                2, 2,
                1, 2,
                1, 3,
                0, 3,
                0, 1,
                1, 1
            ).WithPolygon(
                0, 1, 2, 3, 4, 5, 6, 7
            );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (-1)x + (0)y + (1)
                | [0,1 -> 1,1],
                  [1,2 -> 1,3],
                  [0,3 -> 0,1],
                  [1,3 -> 0,3]
                | [1,0 -> 2,0],
                  [2,0 -> 2,2],
                  [1,1 -> 1,0],
                  [2,2 -> 1,2]");

            var portals = Portalize(tree);
            CheckPortals(tree, portals, @"
                (1) [1,1] -> [1,2] (2)
            ");
        }

        [Test]
        public void OOfDestruction()
        {
            var builder = new GraphBuilder().WithPoints(
                3, 3,
                3, 0,
                0, 0,
                0, 3,

                2, 2,
                1, 2,
                1, 1,
                2, 1
            ).WithPolygon(
                0, 1, 2, 3
            ).WithPolygon(
                4, 5, 6, 7
            ).WithHint(
                0, 6, 5
            );

            var unculledTree = builder.ConstructBspTree();
            CheckTree(unculledTree,
            @"- (-1)x + (0)y + (1)
                - (0)x + (-1)y + (0)
                  | [1,0 -> 0,0]
                  - (-1)x + (0)y + (0)
                    | [0,0 -> 0,3]
                    | [0,3 -> 1,3]
                - (1)x + (0)y + (-3)
                  | [3,3 -> 3,0]
                  - (0)x + (-1)y + (2)
                    - (0)x + (1)y + (-1)
                      | [1,1 -> 2,1],
                        [2,1 -> 2,2],
                        [1,2 -> 1,1],
                        [2,2 -> 1,2]
                      | [3,0 -> 1,0]
                    | [1,3 -> 3,3]");

            var portals = Portalize(unculledTree);
            CheckPortals(unculledTree, portals, @"
                (3) [0,0] -> [-1,0] (9)
                (9) [0,3] -> [0,4] (10)
                (3) [1,-1] -> [1,0] (28)
                (5) [3,0] -> [3,-1] (28)
                (10) [1,3] -> [1,4] (14)
                (5) [3,4] -> [3,3] (14)
            ");

            var culledTree = CullOutside(unculledTree, portals, new[] { new Point2D(1, 1) });
            CheckTree(culledTree,
            @"| [1,1 -> 2,1],
                [2,1 -> 2,2],
                [1,2 -> 1,1],
                [2,2 -> 1,2]");
        }

        [Test]
        public void LShapeInverted()
        {
            var builder = new GraphBuilder().WithPoints(
                    2, 2,
                    2, -2,
                    0, -2,
                    0, 0,
                    -1, 0,
                    -2, 2
                ).WithPolygon(
                    0, 1, 2, 3, 4, 5
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (1)x + (0)y + (-2)
                | [2,2 -> 2,-2]
                - (0)x + (-1)y + (0)
                  - (0)x + (-1)y + (-2)
                    | [2,-2 ->  0,-2]
                    | [0,-2 ->  0, 0],
                      [0, 0 -> -1, 0]
                  - (-1)x + (-1/2)y + (-1)
                    | [-1,0 -> -2,2]
                    | [-2,2 ->  2,2]");
        }

        [Test]
        public void LShapeHinted()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    -1, 1,
                    -1, 0,
                    0, 0,
                    0, -1,
                    1, -1
                ).WithPolygon(
                    0, 1, 2, 3, 4, 5
                ).WithHint(
                    0, 0, 3
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (1)x + (-1)y + (0)
                | [0,-1 -> 1,-1],
                  [1,-1 -> 1, 1],
                  [0, 0 -> 0,-1]
                | [-1,0 ->  0,0],
                  [-1,1 -> -1,0],
                  [ 1,1 -> -1,1]");

            var portalSet = Portalize(tree);
            CheckPortals(tree, portalSet, @"
                (1) [1,1] -> [0,0] (2)
            ");
        }

        [Test]
        public void TwoRooms()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    3, 1,
                    3, 3,
                    1, 3,

                    4, 1,
                    5, 1,
                    5, 3,
                    4, 3
                ).WithPolygon(
                    0, 1, 2, 3
                ).WithPolygon(
                    4, 5, 6, 7
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (-1)x + (0)y + (3)
                | [1,1 -> 3,1],
                  [3,1 -> 3,3],
                  [1,3 -> 1,1],
                  [3,3 -> 1,3]
                | [4,1 -> 5,1],
                  [5,1 -> 5,3],
                  [4,3 -> 4,1],
                  [5,3 -> 4,3]");

            var portals = Portalize(tree);
            CheckPortals(tree, portals, @"");

            var culledTree = CullOutside(tree, portals, new[] { new Point2D(2, 2) });
            CheckTree(culledTree,
            @"| [1,1 -> 3,1],
                [3,1 -> 3,3],
                [1,3 -> 1,1],
                [3,3 -> 1,3]");
        }

        [Test]
        public void UShape()
        {
            var builder = new GraphBuilder().WithPoints(
                    2, -2,
                    2, 1,
                    0, 2,
                    -2, 1,
                    -2, -2,

                    -1, -2,
                    -1, 1,
                    1, 1,
                    1, -2
                ).WithPolygon(
                    0, 1, 2, 3, 4, 5, 6, 7, 8
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (0)x+(1)y+(-1)
                | [-1,1 ->  1,1],
                  [ 2,1 ->  0,2],
                  [ 0,2 -> -2,1]
                - (-1)x+(0)y+(-1)
                  | [-2,-2 -> -1,-2],
                    [-1,-2 -> -1, 1],
                    [-2, 1 -> -2,-2]
                  | [ 1,-2 ->  2,-2],
                    [ 2,-2 ->  2, 1],
                    [ 1, 1 ->  1,-2]");

            var portalSet = Portalize(tree);
            CheckPortals(tree, portalSet, @"
                (1) [-2,1] -> [-1,1] (5)
                (1) [1,1] -> [2,1] (6)
            ");
        }

        [Test]
        public void RingRoom()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    -1, 1,
                    -1, -1,
                    1, -1,

                    2, 2,
                    2, -2,
                    -2, -2,
                    -2, 2,

                    4, 4,
                    -4, 4,
                    -4, -4,
                    4, -4
                ).WithPolygon(
                    0, 1, 2, 3
                ).WithPolygon(
                    4, 5, 6, 7
                ).WithPolygon(
                    8, 9, 10, 11
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (1)x+(0)y+(-2)
                | [2,-4 -> 4,-4],
                  [4,-4 -> 4, 4],
                  [2, 2 -> 2,-2],
                  [4, 4 -> 2, 4]
                - (0)x+(-1)y+(-2)
                  | [-4,-4 ->  2,-4],
                    [-4,-2 -> -4,-4],
                    [ 2,-2 -> -2,-2]
                  - (-1)x+(0)y+(-2)
                    | [-2,-2 -> -2, 2],
                      [-4, 4 -> -4,-2],
                      [-2, 4 -> -4, 4]
                    - (0)x+(-1)y+(1)
                      | [-1,-1 ->  1,-1],
                        [ 1,-1 ->  1, 1],
                        [-1, 1 -> -1,-1],
                        [ 1, 1 -> -1, 1]
                      | [-2, 2 ->  2, 2],
                        [ 2, 4 -> -2, 4]");

            var portal = Portalize(tree);
            CheckPortals(tree, portal, @"
                (1) [2,-2] -> [2,-4] (5)
                (1) [2,4] -> [2,2] (30)
                (5) [-2,-2] -> [-4,-2] (13)
                (13) [-2,2] -> [-2,4] (30)
            ");

            var middleRoomTree = CullOutside(tree, portal, new[] { new Point2D(0, 0) });
            CheckTree(middleRoomTree,
            @"| [-1,-1 ->  1,-1],
                [ 1,-1 ->  1, 1],
                [-1, 1 -> -1,-1],
                [ 1, 1 -> -1, 1]");

            var outerRingRoomBsp = CullOutside(tree, portal, new[] { new Point2D(3, 3) });
            CheckTree(outerRingRoomBsp,
            @"- (1)x+(0)y+(-2)
                | [2,-4 -> 4,-4],
                  [4,-4 -> 4, 4],
                  [2, 2 -> 2,-2],
                  [4, 4 -> 2, 4]
                - (0)x+(-1)y+(-2)
                  | [-4,-4 ->  2,-4],
                    [-4,-2 -> -4,-4],
                    [ 2,-2 -> -2,-2]
                  - (-1)x+(0)y+(-2)
                    | [-2,-2 -> -2, 2],
                      [-4, 4 -> -4,-2],
                      [-2, 4 -> -4, 4]
                    | [-2, 2 ->  2, 2],
                      [ 2, 4 -> -2, 4]");
        }

        [Test]
        public void FourCubeRoom()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    2, 1,
                    2, -1,

                    1, -1,
                    1, -2,
                    -1, -2,

                    -1, -1,
                    -2, -1,
                    -2, 1,

                    -1, 1,
                    -1, 2,
                    1, 2
                ).WithPolygon(
                    0, 1, 2, 3
                ).WithPolygon(
                    3, 4, 5, 6
                ).WithPolygon(
                    6, 7, 8, 9
                ).WithPolygon(
                    9, 10, 11, 0
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (0)x + (1)y + (-1)
                - (-1)x + (0)y + (-1)
                  | [-2,1 -> -1,1],
                    [-1,1 -> -1,2]
                  - (0)x + (1)y + (-2)
                    | [-1,2 - >1,2]
                    | [1,1 -> 2,1],
                      [1,2 -> 1,1]
                - (0)x + (-1)y + (-1)
                  - (1)x + (0)y + (-1)
                    | [1,-1 -> 1,-2],
                      [2,-1 -> 1,-1]
                    - (0)x + (-1)y + (-2)
                      | [1,-2 -> -1,-2]
                      | [-1,-2 -> -1,-1],
                        [-1,-1 -> -2,-1]
                  - (1)x + (0)y + (-2)
                    | [2,1 -> 2,-1]
                    - (-1)x + (0)y + (-2)
                      | [-2,-1 -> -2,1]
                      | [-1,-1 ->  1,-1],
                        [ 1,-1 ->  1, 1],
                        [-1, 1 -> -1,-1],
                        [ 1, 1 -> -1, 1]");

            var portalSet = Portalize(tree);

            var middleRoomTree = CullOutside(tree, portalSet, new[] { new Point2D(0, 0) });
            CheckTree(middleRoomTree,
            @"| [-1,-1 ->  1,-1],
                [ 1,-1 ->  1, 1],
                [-1, 1 -> -1,-1],
                [ 1, 1 -> -1, 1]");
        }

        [Test]
        public void EightRoom()
        {
            var builder = new GraphBuilder().WithPoints(
                    0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0,
                    0, 1, 1, 1, 2, 1, 3, 1, 4, 1, 5, 1, 6, 1, 7, 1,
                    0, 2, 1, 2, 2, 2, 3, 2, 4, 2, 5, 2, 6, 2, 7, 2,
                    0, 3, 1, 3, 2, 3, 3, 3, 4, 3, 5, 3, 6, 3, 7, 3
                ).WithPolygon(
                    0, 1, 9, 8
                ).WithPolygon(
                    2, 3, 11, 10
                ).WithPolygon(
                    4, 5, 13, 12
                ).WithPolygon(
                    6, 7, 15, 14
                ).WithPolygon(
                    16, 17, 25, 24
                ).WithPolygon(
                    18, 19, 27, 26
                ).WithPolygon(
                    20, 21, 29, 28
                ).WithPolygon(
                    22, 23, 31, 30
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (0)x + (1)y + (-2)
                - (-1)x + (0)y + (3)
                  - (1)x + (0)y + (-2)
                    | [2,2->3,2],
                      [3,2->3,3],
                      [2,3->2,2],
                      [3,3->2,3]
                    | [0,2->1,2],
                      [1,2->1,3],
                      [0,3->0,2],
                      [1,3->0,3]
                  - (1)x + (0)y + (-6)
                    | [6,2->7,2],
                      [7,2->7,3],
                      [6,3->6,2],
                      [7,3->6,3]
                    | [4,2->5,2],
                      [5,2->5,3],
                      [4,3->4,2],
                      [5,3->4,3]
                - (1)x + (0)y + (-4)
                  - (-1)x + (0)y + (5)
                    | [4,0->5,0],
                      [5,0->5,1],
                      [4,1->4,0],
                      [5,1->4,1]
                    | [6,0->7,0],
                      [7,0->7,1],
                      [6,1->6,0],
                      [7,1->6,1]
                  - (1)x + (0)y + (-2)
                    | [2,0->3,0],
                      [3,0->3,1],
                      [2,1->2,0],
                      [3,1->2,1]
                    | [0,0->1,0],
                      [1,0->1,1],
                      [0,1->0,0],
                      [1,1->0,1]");

            var portals = Portalize(tree);
            CheckPortals(tree, portals, @"");

            var half = new Rational(1, 2);
            var points = new[]
            {
                new Point2D(0 + half, 0 + half),
                new Point2D(2 + half, 0 + half),
                new Point2D(4 + half, 0 + half),
                new Point2D(6 + half, 0 + half),
                new Point2D(0 + half, 2 + half),
                new Point2D(2 + half, 2 + half),
                new Point2D(4 + half, 2 + half),
                new Point2D(6 + half, 2 + half),
            };

            var superset = new List<List<Point2D>>() { new List<Point2D>() };
            foreach (var point in points)
                superset = superset.SelectMany(
                    s => new[] { s.ToList(), s.Append(point).ToList() }).ToList();

            Assert.AreEqual(256, superset.Count);

            foreach (var pointSet in superset)
            {
                if (pointSet.Count == 0) continue;

                var foo = CullOutside(tree, portals, pointSet);
            }
        }


        [Test]
        public void NonSolidSurfaces()
        {
            var sut = new GraphBuilder().WithPoints(
                0, 1,
                -1, 1,
                -1, -1,
                0, -1,
                1, -1,
                1, 1
            ).WithPolygon(
                0, 1, 2, 3, 4, 5
            ).WithTwoSidedEdge(
                3, 0
            );

            var tree = sut.ConstructBspTree();
            CheckTree(tree,
            @"- (-1)x + (0)y + (0)
                | [-1,-1 ->  0,-1],
                  [ 0,-1 ->  0, 1],
                  [-1, 1 -> -1,-1],
                  [ 0, 1 -> -1, 1]
                | [ 0,-1 ->  1,-1],
                  [ 1,-1 ->  1, 1],
                  [ 1, 1 ->  0, 1]");

            var portals = Portalize(tree).ToList();
            CheckPortals(tree, portals, @"
                (1) [0,-1] -> [0,1] (2)
            ");
        }

        [Test]
        public void TJoins()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 0,
                    2, 0,
                    3, 0,
                    3, 2,
                    2, 2,
                    0, 2,
                    0, 1,
                    1, 1
                ).WithPolygon(
                    0, 1, 2, 3, 4, 5, 6, 7
                ).WithTwoSidedEdge(
                    4, 1
                );

            var tree = builder.ConstructBspTree();
            CheckTree(tree,
            @"- (1)x + (0)y + (-2)
                | [2,0 -> 3,0],
                  [3,0 -> 3,2],
                  [2,1 -> 2,0],
                  [2,2 -> 2,1],
                  [3,2 -> 2,2]
                - (0)x + (1)y + (-1)
                  | [0,1 -> 1,1],
                    [0,2 -> 0,1],
                    [2,2 -> 0,2]
                  | [1,0 -> 2,0],
                    [1,1 -> 1,0]");
        }

        [Test]
        public void CsgMaterialSuperiority()
        {
            // Coplanar facets of brushes with different materials
            var start = new Point2D(1, 2);
            var end = new Point2D(3, 4);

            CheckCsgOutput(ConstructSolidGeometry(new[]
            {
                Monofacet(0, 1, start, end),
                Monofacet(1, 2, start, end),
            }), @"
                B1 (0) [1,2] -> [3,4] (2)
            ");

            CheckCsgOutput(ConstructSolidGeometry(new[]
            {
                Monofacet(0, 2, start, end),
                Monofacet(1, 1, start, end),
            }), @"
                B0 (0) [1,2] -> [3,4] (2)
            ");
        }

        [Test]
        public void CsgOneBrush()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(1, -1),
                new(-1, -1),
                new(-1, 1),
            };

            var brushes = new[]
            {
                MakeBrush(0, 1, false, points[0], points[1], points[2], points[3]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [1,1] -> [1,-1] (1)
                B0 (0) [1,-1] -> [-1,-1] (1)
                B0 (0) [-1,-1] -> [-1,1] (1)
                B0 (0) [-1,1] -> [1,1] (1)
            ");
        }

        [Test]
        public void CsgDisjointBrushes()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(1, -1),
                new(-1, -1),
                new(-1, 1),

                new(4, 1),
                new(4, -1),
                new(3, -1),
                new(3, 1),
            };

            var brushes = new[]
            {
                MakeBrush(0, 1, false, points[0], points[1], points[2], points[3]),
                MakeBrush(1, 1, false, points[4], points[5], points[6], points[7]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [1,1] -> [1,-1] (1)
                B0 (0) [1,-1] -> [-1,-1] (1)
                B0 (0) [-1,-1] -> [-1,1] (1)
                B0 (0) [-1,1] -> [1,1] (1)

                B1 (0) [4,1] -> [4,-1] (1)
                B1 (0) [4,-1] -> [3,-1] (1)
                B1 (0) [3,-1] -> [3,1] (1)
                B1 (0) [3,1] -> [4,1] (1)
            ");
        }

        [Test]
        public void CsgButteBrushes()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(1, -1),
                new(0, -1),
                new(-1, -1),
                new(-1, 1),
                new(0, 1),
            };

            var brushes = new[]
            {
                MakeBrush(0, 1, false, points[0], points[1], points[2], points[5]),
                MakeBrush(1, 1, false, points[2], points[3], points[4], points[5]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [0,1] -> [1,1] (1)
                B0 (0) [1,1] -> [1,-1] (1)
                B0 (0) [1,-1] -> [0,-1] (1)
                B1 (0) [0,-1] -> [-1,-1] (1)
                B1 (0) [-1,-1] -> [-1,1] (1)
                B1 (0) [-1,1] -> [0,1] (1)
             ");
        }

        [Test]
        public void CsgButteBrushesDifferentMaterial()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(1, -1),
                new(0, -1),
                new(-1, -1),
                new(-1, 1),
                new(0, 1),
            };

            var brushes = new[]
            {
                MakeBrush(0, 1, false, points[0], points[1], points[2], points[5]),
                MakeBrush(1, 2, false, points[2], points[3], points[4], points[5]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [0,1] -> [1,1] (1)
                B0 (0) [1,1] -> [1,-1] (1)
                B0 (0) [1,-1] -> [0,-1] (1)
                B1 (0) [0,-1] -> [-1,-1] (2)
                B1 (0) [-1,-1] -> [-1,1] (2)
                B1 (0) [-1,1] -> [0,1] (2)

                B1 (1) [0,1] -> [0,-1] (2)
            ");
        }

        [Test]
        public void CsgCoplanarFaces()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(1, -1),
                new(-1, -1),
                new(-1, 1),
                new(0, 1),
            };

            var brushes = new[]
            {
                MakeBrush(0, 1, false, points[0], points[1], points[2], points[4]),
                MakeBrush(1, 1, false, points[4], points[1], points[2], points[3]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [0,1] -> [1,1] (1)
                B0 (0) [1,1] -> [1,-1] (1)
                B1 (0) [1,-1] -> [-1,-1] (1)
                B1 (0) [-1,-1] -> [-1,1] (1)
                B1 (0) [-1,1] -> [0,1] (1)
            ");
        }

        [Test]
        public void CsgSplitHeals()
        {
            var points = new Point2D[]
            {
                new(3, 0),
                new(0, 1),
                new(0, 3),
                new(3, 3),
                new(2, 2),
                new(2, 3),
            };

            var firstBrushes = new[]
            {
                MakeBrush(0, 1, false, points[0], points[1], points[2]),
                MakeBrush(0, 1, false, points[1], points[2], points[0]),
                MakeBrush(0, 1, false, points[2], points[0], points[1]),
            };
            var secondBrushes = new[]
            {
                MakeBrush(1, 1, false, points[3], points[4], points[5]),
                MakeBrush(1, 1, false, points[4], points[5], points[3]),
                MakeBrush(1, 1, false, points[5], points[3], points[4]),
            };

            var expectedSurfaces = @"
                B0 (0) [0,3] -> [3,0] (1)
                B0 (0) [3,0] -> [0,1] (1)
                B0 (0) [0,1] -> [0,3] (1)
                B1 (0) [3,3] -> [2,2] (1)
                B1 (0) [2,2] -> [2,3] (1)
                B1 (0) [2,3] -> [3,3] (1)
            ";

            foreach (var i in Enumerable.Range(0, 3))
                foreach (var j in Enumerable.Range(0, 3))
                {
                    var brushes = new[] { firstBrushes[i], secondBrushes[j] };
                    var surfaces = ConstructSolidGeometry(brushes);
                    CheckCsgOutput(surfaces, expectedSurfaces);
                }
        }

        [Test]
        public void AABB_DslTest()
        {
            var brushes = new[]
            {
                AABB(0, 1, false, 1, 2, 3, 4),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [1,4] -> [3,4] (1)
                B0 (0) [3,4] -> [3,2] (1)
                B0 (0) [3,2] -> [1,2] (1)
                B0 (0) [1,2] -> [1,4] (1)
            ");
        }

        [Test]
        public void MaterialPrecedence_HigherOverrides()
        {
            var brushes = new[]
            {
                AABB(0, 1, false, 0, 0, 10, 10),
                AABB(1, 2, false, 5, 0, 15, 10),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B1 (0) [15,0] -> [5,0] (2)
                B0 (0) [5,0] -> [0,0] (1)
                B0 (0) [0,0] -> [0,10] (1)
                B0 (0) [0,10] -> [5,10] (1)
                B1 (0) [5,10] -> [15,10] (2)
                B1 (0) [15,10] -> [15,0] (2)

                B1 (1) [5,0] -> [5,10] (2)
            ");
        }

        [Test]
        public void MaterialPrecedence_LowerOverrides()
        {
            var brushes = new[]
            {
                AABB(0, 2, false, 5, 0, 15, 10),
                AABB(1, 1, false, 0, 0, 10, 10),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [15,0] -> [5,0] (2)
                B1 (0) [5,0] -> [0,0] (1)
                B1 (0) [0,0] -> [0,10] (1)
                B1 (0) [0,10] -> [5,10] (1)
                B0 (0) [5,10] -> [15,10] (2)
                B0 (0) [15,10] -> [15,0] (2)

                B0 (1) [5,0] -> [5,10] (2)
            ");
        }

        [Test]
        public void ButteJoins()
        {
            const int SOLID = 10;
            var points = new Point2D[]
            {
                new(2, 1),
                new(-2, 1),
                new(-2, 2),
                new(2, 2),

                new(2, -2),
                new(-2, -2),
                new(-2, -1),
                new(2, -1),

                new(2, 3),
                new(3, 3),
                new(3, -3),
                new(2, -3),

                new(-3, 3),
                new(-2, 3),
                new(-2, -3),
                new(-3, -3),
            };

            var brushes = new[]
            {
                MakeBrush(0, SOLID, false, points[0], points[1], points[2], points[3]),
                MakeBrush(1, SOLID, false, points[4], points[5], points[6], points[7]),
                MakeBrush(2, SOLID, false, points[8], points[9], points[10], points[11]),
                MakeBrush(3, SOLID, false, points[12], points[13], points[14], points[15]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B2 (0) [2,2] -> [2,3] (10)
                B2 (0) [2,3] -> [3,3] (10)
                B2 (0) [3,3] -> [3,-3] (10)
                B2 (0) [3,-3] -> [2,-3] (10)
                B2 (0) [2,-3] -> [2,-2] (10)
                B1 (0) [2,-2] -> [-2,-2] (10)
                B3 (0) [-2,-2] -> [-2,-3] (10)
                B3 (0) [-2,-3] -> [-3,-3] (10)
                B3 (0) [-3,-3] -> [-3,3] (10)
                B3 (0) [-3,3] -> [-2,3] (10)
                B3 (0) [-2,3] -> [-2,2] (10)
                B0 (0) [-2,2] -> [2,2] (10)

                B0 (0) [2,1] -> [-2,1] (10)
                B3 (0) [-2,1] -> [-2,-1] (10)
                B1 (0) [-2,-1] -> [2,-1] (10)
                B2 (0) [2,-1] -> [2,1] (10)
            ");

            var tree = ConstructBspTree(surfaces);

            var portalSet = Portalize(tree);
            var middleRoomTree = CullOutside(tree, portalSet, new[] { new Point2D(0, 0) });
            Assert.AreEqual(1, middleRoomTree.NodeCount);
        }

        [Test]
        public void PortalWaterBlob()
        {
            const int SOLID = 10;
            const int WATER = 5;

            var brushes = new[]
            {
                AABB(0, SOLID, false, -10, -10, -9, 10),
                AABB(1, SOLID, false,   9, -10, 10, 10),
                AABB(2, SOLID, false, -10, -10, 10, -9),
                AABB(3, SOLID, false, -10,   9, 10, 10),
                AABB(4, WATER, false, -10, -10,  5,  5),
            };

            var interiorPoints = new Point2D[]
            {
                new(0, 0),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            var rawTree = ConstructBspTree(surfaces);
            var rawPortals = Portalize(rawTree).ToList();
            var culledTree = CullOutside(rawTree, rawPortals, interiorPoints);
            var cullPortals = Portalize(culledTree).ToList();
            CheckPortals(culledTree, cullPortals, @"
                (1) [5,5] -> [9,5] (5)
                (1) [-9,5] -> [5,5] (6)
                (5) [5,5] -> [5,-9] (6)
            ");
        }

        [Test]
        public void PortalWaterBlob_NonAABB()
        {
            const int SOLID = 10;
            const int WATER = 5;

            var brushes = new[]
            {
                AABB(0, SOLID, false, -10, -10, -9, 10),
                AABB(1, SOLID, false,   9, -10, 10, 10),
                AABB(2, SOLID, false, -10, -10, 10, -9),
                AABB(3, SOLID, false, -10,   9, 10, 10),
                AABB(4, WATER, false,  -9,  -9,  5,  5),
            };

            var interiorPoints = new Point2D[]
            {
                new(0, 0),
            };

            var transform = AffineMapping
                .From(Point2D.Origin, new Point2D(1, 0), new Point2D(0, 1))
                .Onto(Point2D.Origin, new Point2D(1, 1), new Point2D(-1, 1));

            var surfaces = Transform(transform,
                ConstructSolidGeometry(brushes));
            interiorPoints = Tranfsform(transform, interiorPoints);

            var rawTree = ConstructBspTree(surfaces);
            var rawPortals = Portalize(rawTree).ToList();
            var culledTree = CullOutside(rawTree, rawPortals, interiorPoints);
            var cullPortals = Portalize(culledTree).ToList();
            CheckPortals(culledTree, cullPortals, @"
                (1) [0,10] -> [4,14] (5)
                (1) [-14,-4] -> [0,10] (6)
                (5) [0,10] -> [14,-4] (6)
            ");
        }

        #region Test DSL

        static List<VanillaSurface> ConstructSolidGeometry(IList<Vanilla2D.Brush> brushes)
        {
            return Vanilla2D.Instance.ConstructSolidGeometry(brushes).ToList();
        }

        static void CheckCsgOutput(IEnumerable<VanillaSurface> segments, string expected)
        {
            var actualLines = segments.Select(segment => $"{segment.Tag} "
                + $"({segment.FrontMaterial}) [{segment.Facet.Start}] -> [{segment.Facet.End}] "
                + $"({segment.BackMaterial})").ToList();

            if (expected == null)
            {
                Console.WriteLine(string.Join(Environment.NewLine, actualLines));
                Assert.Fail("Set up expectation");
            }

            var expectedLines = expected.Split(Environment.NewLine)
                .Select(s => s.Trim())
                .Where(s => !s.StartsWith("//"))
                .Where(s => s.Length > 0).ToList();
            CollectionAssert.AreEquivalent(expectedLines, actualLines);
        }

        static Vanilla2D.IBspTree ConstructBspTree(IEnumerable<VanillaSurface> surfaces)
        {
            return Vanilla2D.Instance.ConstructBspTree(
                Vanilla2D.Instance.ExhaustivePartitionStrategy(1, 10),
                surfaces);
        }

        static void CheckTree(Vanilla2D.IBspTree root, string expected)
        {
            var actualTree = StringifyTree(root);

            if (expected == null)
            {
                Console.WriteLine(actualTree);
                Assert.Fail("Set up expectation!");
            }

            var expectedTree = expected.Replace(" ", "").Replace(Environment.NewLine, "");

            Assert.AreEqual(expectedTree, actualTree);
        }

        static string StringifyTree(Vanilla2D.IBspTree tree)
        {
            var result = new StringBuilder();

            tree.PreOrderTraverse((node) =>
            {
                if (tree.IsLeaf(node))
                {
                    result.Append('|');
                    result.Append(string.Join(",", tree.Surfaces(node)
                        .OrderBy(s => s.Surface.Facet.Start, new Point2DComparer())
                        .Select(s => $"[{s.Surface.Facet.Start}->{s.Surface.Facet.End}]")));
                }
                else
                {
                    result.Append($"-{DebugString(tree.PartitionPlane(node)).Replace(" ", "")}");
                }
            });

            return result.ToString();
        }

        static string DebugString(Hyperplane2D plane)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "({0})x + ({1})y + ({2})", plane.A, plane.B, plane.C);
        }

        List<Portal<Facet2D>> Portalize(Vanilla2D.IBspTree tree)
        {
            Vanilla2D.Instance.Portalize(tree,
                out IEnumerable<Portal<Facet2D>> portals,
                out _);
            return portals.ToList();
        }

        static void CheckPortals(Vanilla2D.IBspTree tree,
            IEnumerable<Portal<Facet2D>> portals, string expected)
        {
            var actualLines = portals.Select(portal =>
            {
                var frontNodeName = portal.Front;
                var backNodeName = portal.Back;
                var startPoint = portal.Facet.Start;
                var endPoint = portal.Facet.End;

                if (frontNodeName < backNodeName)
                    return $"({frontNodeName}) [{startPoint}] -> [{endPoint}] ({backNodeName})";
                else
                    return $"({backNodeName}) [{endPoint}] -> [{startPoint}] ({frontNodeName})";

            }).ToList();

            if (expected == null)
            {
                Console.WriteLine(string.Join(Environment.NewLine, actualLines));
                Assert.Fail("Set up expectation");
            }

            var expectedLines = expected.Split(Environment.NewLine)
                .Select(s => s.Trim())
                .Where(s => !s.StartsWith("//"))
                .Where(s => s.Length > 0).ToList();
            CollectionAssert.AreEquivalent(expectedLines, actualLines);
        }

        Vanilla2D.IBspTree CullOutside(Vanilla2D.IBspTree rawTree,
            List<Portal<Facet2D>> rawPortals, IEnumerable<Point2D> interiorPoints)
        {
            return Vanilla2D.Instance.CullOutside(rawTree, rawPortals, interiorPoints);
        }

        static Vanilla2D.Brush MakeBrush(int index, int material, bool isTwoSided,
            params Point2D[] points)
        {
            return Vanilla2D.Instance.MakeBrush(
                Enumerable.Range(0, points.Length).Select(i =>
                new VanillaSurface(points[i], points[(i + 1) % points.Length], 0, material,
                    isTwoSided, null, $"B{index}")));
        }

        static Vanilla2D.Brush AABB(int index, int material, bool isTwoSided,
            Rational minX, Rational minY, Rational maxX, Rational maxY)
        {
            var facets = new Orthotope2D(minX, minY, maxX, maxY)
                .MakeFacets().Select(f => f.Cofacet);

            return Vanilla2D.Instance.MakeBrush(
                facets.Select(facet => new VanillaSurface(facet, 0, material, isTwoSided,
                    null, $"B{index}")));
        }

        static Vanilla2D.Brush Monofacet(int index, int backMaterial,
            Point2D from, Point2D to)
        {
            var facet = new Facet2D(new Hyperplane2D(from, to), from, to);
            var segment = new VanillaSurface(facet.Start, facet.End, 0, backMaterial,
                false, -1, $"B{index}");

            return Vanilla2D.Instance.MakeBrush(
                new[] { segment });
        }

        #region Affine transformation of points

        static IEnumerable<VanillaSurface> Transform(Matrix3D transform,
            IEnumerable<VanillaSurface> segments)
        {
            return segments.Select(s => new VanillaSurface(Transform(transform, s.Facet),
                s.FrontMaterial, s.BackMaterial, s.IsTwoSided, s.HintDepth, s.Tag));
        }

        static Facet2D Transform(Matrix3D transform, Facet2D facet)
        {
            var tStart = AffineTransform(transform, facet.Start);
            var tEnd = AffineTransform(transform, facet.End);
            return new Facet2D(new Hyperplane2D(tStart, tEnd), tStart, tEnd);
        }

        static Point2D[] Tranfsform(Matrix3D transform, IEnumerable<Point2D> points)
        {
            return points.Select(p => AffineTransform(transform, p)).ToArray();
        }

        static Point2D AffineTransform(Matrix3D transform, Point2D point)
        {
            return (transform * point.Homogenized()).Dehomogenized();
        }

        #endregion

        class GraphBuilder
        {
            readonly List<Point2D> points = new();
            readonly List<VanillaSurface> surfaces = new();

            public GraphBuilder WithPoints(params Rational[] pointXYs)
            {
                if (pointXYs.Length % 2 == 1)
                    throw new ArgumentException("Missing Y value for last point!");

                for (var i = 0; i < pointXYs.Length; i += 2)
                {
                    var point = new Point2D(pointXYs[i], pointXYs[i + 1]);
                    points.Add(point);
                }

                return this;
            }

            public GraphBuilder WithPolygon(params int[] indices)
            {
                foreach (var i in Enumerable.Range(0, indices.Length))
                {
                    var start = points[indices[i]];
                    var end = points[indices[(i + 1) % indices.Length]];
                    AddSurface(start, end, false, null, string.Empty);
                }

                return this;
            }

            public GraphBuilder WithHint(int depth, int p1index, int p2index)
            {
                AddSurface(points[p1index], points[p2index], false, depth, string.Empty);
                return this;
            }

            public GraphBuilder WithTwoSidedEdge(int p1index, int p2index)
            {
                AddSurface(points[p1index], points[p2index], true, null, string.Empty);
                return this;
            }

            void AddSurface(Point2D start, Point2D end, bool isTwoSided, int? hintDepth,
                string tag)
            {
                surfaces.Add(new VanillaSurface(start, end, 0, 1, isTwoSided, hintDepth, tag));
            }

            public Vanilla2D.IBspTree ConstructBspTree()
            {
                return Vanilla2D.Instance.ConstructBspTree(
                    Vanilla2D.Instance.ExhaustivePartitionStrategy(1, 10),
                    surfaces);
            }
        }

        #endregion


        #region Testable Surface2D implementation

        class VanillaSurface : Vanilla2D.SurfaceBase
        {
            readonly bool isTwoSided;
            public int? HintDepth { get; private set; }
            public string Tag { get; private set; }

            public override bool IsTwoSided
            {
                get { return isTwoSided; }
            }

            public override VanillaSurface Cosurface
            {
                get
                {
                    return new VanillaSurface(Facet.Cofacet, BackMaterial,
                        FrontMaterial, isTwoSided, HintDepth, Tag);
                }
            }

            public override VanillaSurface FillFront(int material)
            {
                return new VanillaSurface(Facet, material, BackMaterial,
                    isTwoSided, HintDepth, Tag);
            }

            public override int? HintLevel
            {
                get { return HintDepth; }
            }

            public override void Split(Hyperplane2D partitioningPlane,
                out VanillaSurface frontSurface, out VanillaSurface backSurface)
            {
                frontSurface = null;
                backSurface = null;
                Facet.Split(partitioningPlane, out Facet2D frontFacet, out Facet2D backFacet);
                if (frontFacet != null)
                    frontSurface = new VanillaSurface(frontFacet, FrontMaterial,
                        BackMaterial, isTwoSided, HintDepth, Tag);
                if (backFacet != null)
                    backSurface = new VanillaSurface(backFacet, FrontMaterial,
                        BackMaterial, isTwoSided, HintDepth, Tag);
            }

            public VanillaSurface(Facet2D facet, int frontDensity, int backDensity,
                bool isTwoSided = false, int? hintDepth = null, string tag = null)
                : base(facet, frontDensity, backDensity)
            {
                this.isTwoSided = isTwoSided;
                HintDepth = hintDepth;
                Tag = tag;
            }

            public VanillaSurface(Point2D start, Point2D end, int frontDensity,
                int backDensity, bool isTwoSided, int? hintDepth, string tag)
                : this(new Facet2D(new Hyperplane2D(start, end), start, end),
                      frontDensity, backDensity, isTwoSided, hintDepth, tag)
            {
            }

            /// <summary>
            /// Makes a copy of a surface, with any edges between the given facets and this
            /// surface's facets healed.
            /// </summary>
            /// <param name="facets">The other surfaces that are potentially adjacent
            /// to this surface.</param>
            /// <returns>A new Surface that has no cracks with the input facets.</returns>
            public override VanillaSurface HealEdges(List<Facet2D> facets)
            {
                return this; // 2D case cannot have edge cracks
            }

            public override string ToString()
            {
                return $"[{Facet.Start.X},{Facet.Start.Y} -> {Facet.End.X},{Facet.End.Y}]";
            }
        }

        class Vanilla2D : Spatial2D<VanillaSurface>
        {
            static readonly Vanilla2D instance = new();
            public static Vanilla2D Instance { get { return instance; } }
            public Vanilla2D() : base() { }
        }

        #endregion
    }
}
