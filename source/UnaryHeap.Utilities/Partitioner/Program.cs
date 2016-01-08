using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.Misc;

namespace Partitioner
{
    class Program
    {
        static void Main(string[] args)
        {
            var graph = LoadGraph(args[1]);
            var treeRoot = Graph2DBinarySpacePartitioner.WithExhaustivePartitioner()
                .ConstructBspTree(graph);

            var nodeCount = treeRoot.NodeCount;

            var nextLeafId = 0;
            var nextBranchId = 1 + nodeCount / 2;
            var idOfNode = new Dictionary<IBspNode<GraphEdge, Hyperplane2D>, int>();

            var nextPlaneId = 0;
            var idOfPlane = new Dictionary<Hyperplane2D, int>();

            var nextRoomId = 0;
            var idOfRoom = new Dictionary<string, int>();

            var nextVertexId = 0;
            var idOfVertex = new SortedDictionary<Point2D, int>(new Point2DComparer());

            var nextSurfaceId = 0;
            var idOfSurface = new Dictionary<GraphEdge, int>();

            treeRoot.PostOrderTraverse(node =>
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
                    NameObject(idOfPlane, node.PartitionPlane, ref nextPlaneId);
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
                        idOfRoom[surface.RoomName()]);

                writer.WriteNodeCount(nodeWithId.Length);
                foreach (var node in nodeWithId)
                    if (node.IsLeaf)
                        writer.WriteLeafNode(
                            idOfRoom[node.RoomName()],
                            node.NonPassageWalls().Count(),
                            idOfSurface[node.NonPassageWalls().First()]);
                    else
                        writer.WriteBranchNode(
                            idOfPlane[node.PartitionPlane],
                            idOfNode[node.FrontChild],
                            idOfNode[node.BackChild]);
            }
        }

        private static void NameObject<T>(
            IDictionary<T, int> manifest, T newItem, ref int newIndex)
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

        static Graph2D LoadGraph(string filename)
        {
            using (var file = File.OpenText(filename))
                return Graph2D.FromJson(file);
        }
    }

    static class Extensions
    {
        public static IEnumerable<GraphEdge> NonPassageWalls(
            this IBspNode<GraphEdge, Hyperplane2D> node)
        {
            return node.Surfaces.Where(surface => false == surface.IsPassage());
        }

        public static string RoomName(this IBspNode<GraphEdge, Hyperplane2D> node)
        {
            return node.NonPassageWalls()
                .Select(surface => surface.RoomName())
                .Distinct()
                .SingleOrDefault();
        }

        public static bool IsPassage(this GraphEdge surface)
        {
            return surface.Metadata.ContainsKey("passage") ?
                bool.Parse(surface.Metadata["passage"]) : false;
        }

        public static string RoomName(this GraphEdge surface)
        {
            return surface.Metadata.ContainsKey("room") ?
                surface.Metadata["room"] : null;
        }
    }
}
