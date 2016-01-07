using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.D2;

namespace Partitioner
{
    class Program
    {
        static void Main(string[] args)
        {
            var surfaces = Check(LoadSurfaces(args[1]));
            var treeRoot = BinarySpacePartitioner.ConstructBspTree(surfaces);

            var nodeCount = treeRoot.NodeCount;

            var nextLeafId = 0;
            var nextBranchId = 1 + nodeCount / 2;
            var idOfNode = new Dictionary<BinarySpacePartitioner.BspNode, int>();

            var nextPlaneId = 0;
            var idOfPlane = new Dictionary<Hyperplane2D, int>();

            var nextRoomId = 0;
            var idOfRoom = new Dictionary<string, int>();

            var nextVertexId = 0;
            var idOfVertex = new SortedDictionary<Point2D, int>(new Point2DComparer());

            var nextSurfaceId = 0;
            var idOfSurface = new Dictionary<Surface, int>();

            treeRoot.PostOrder(node =>
            {
                if (node.IsLeaf)
                {
                    NameObject(idOfNode, node, ref nextLeafId);
                    NameObject(idOfRoom, node.RoomName(), ref nextRoomId);

                    foreach (var surface in node.NonPassageWalls())
                    {
                        NameObject(idOfSurface, surface, ref nextSurfaceId);
                        NameObject(idOfVertex, surface.Start, ref nextVertexId);
                        NameObject(idOfVertex, surface.End, ref nextVertexId);
                    }
                }
                else
                {
                    NameObject(idOfNode, node, ref nextBranchId);
                    NameObject(idOfPlane, node.Splitter, ref nextPlaneId);
                }
            });

            var nodeWithId = ReverseMapping(idOfNode);
            var surfaceWithId = ReverseMapping(idOfSurface);
            var planeWithId = ReverseMapping(idOfPlane);
            var roomWithId = ReverseMapping(idOfRoom);
            var vertexWithId = ReverseMapping(idOfVertex);

            using (var writer = new FileWriter(@"C:\Users\SheepNine\Desktop\gamedata.dat"))
            {
                writer.WriteVertexCount(vertexWithId.Length);
                foreach (var vertex in vertexWithId)
                    writer.WriteVertex(vertex);

                writer.WritePlaneCount(planeWithId.Length);
                foreach (var plane in planeWithId)
                    writer.WritePlane(plane);

                writer.WriteRoomCount(roomWithId.Length);
                foreach (var room in roomWithId)
                    writer.WriteRoom(room);

                writer.WriteSurfaceCount(surfaceWithId.Length);
                foreach (var surface in surfaceWithId)
                    writer.WriteSurface(idOfVertex[surface.Start],
                        idOfVertex[surface.End],
                        idOfRoom[surface.RoomName]);

                writer.WriteNodeCount(nodeWithId.Length);
                foreach (var node in nodeWithId)
                    if (node.IsLeaf)
                        writer.WriteLeafNode(
                            idOfRoom[node.RoomName()],
                            node.NonPassageWalls().Count(),
                            idOfSurface[node.NonPassageWalls().First()]);
                    else
                        writer.WriteBranchNode(
                            idOfPlane[node.Splitter],
                            idOfNode[node.FrontChild],
                            idOfNode[node.BackChild]);
            }
        }

        private static void NameObject<T>(IDictionary<T, int> manifest, T newItem, ref int newIndex)
        {
            if (false == manifest.ContainsKey(newItem))
                manifest.Add(newItem, newIndex++);
        }

        private static T[] ReverseMapping<T>(IDictionary<T, int> manifest)
        {
            var result = new T[manifest.Count];

            foreach (var item in manifest)
                result[item.Value] = item.Key;

            return result;
        }

        private static List<Surface> Check(List<Surface> surfaces)
        {
            foreach (var surface in surfaces)
            {
                if (false == surface.IsPassage && null == surface.RoomName)
                    throw new ArgumentException("Missing room/passage signifier.");
            }

            return surfaces;
        }

        static List<Surface> LoadSurfaces(string filename)
        {
            using (var file = File.OpenText(filename))
                return Surface.LoadSurfaces(Graph2D.FromJson(file));
        }
    }

    static class Extensions
    {
        public static IEnumerable<Surface> NonPassageWalls(this BinarySpacePartitioner.BspNode node)
        {
            return node.Surfaces.Where(surface => false == surface.IsPassage);
        }

        public static string RoomName(this BinarySpacePartitioner.BspNode node)
        {
                return node.NonPassageWalls()
                    .Select(surface => surface.RoomName)
                    .Distinct()
                    .SingleOrDefault();
        }
    }
}
