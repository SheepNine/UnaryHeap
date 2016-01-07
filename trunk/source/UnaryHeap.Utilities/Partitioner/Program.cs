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
            var treeRoot = ConstructBspTree(surfaces);

            var nodeCount = treeRoot.NodeCount;

            var nextLeafId = 0;
            var nextBranchId = 1 + nodeCount / 2;
            var idOfNode = new Dictionary<BspNode, int>();

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
                    NameObject(idOfRoom, node.RoomName, ref nextRoomId);

                    foreach (var surface in node.NonPassageWalls)
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
                            idOfRoom[node.RoomName],
                            node.NonPassageWalls.Count(),
                            idOfSurface[node.NonPassageWalls.First()]);
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

        static BspNode ConstructBspTree(IEnumerable<Surface> inputSurfaces)
        {
            var surfaces = inputSurfaces.ToList();
            if (0 == surfaces.Count)
                throw new ArgumentException("No surfaces in input surfaces");

            if (AllConvex(surfaces))
                return BspNode.LeafNode(surfaces);

            var splitter = ChooseSplitter(surfaces);
            List<Surface> frontSurfaces, backSurfaces;
            Partition(surfaces, splitter, out frontSurfaces, out backSurfaces);

            var frontChild = ConstructBspTree(frontSurfaces);
            var backChild = ConstructBspTree(backSurfaces);
            return BspNode.BranchNode(splitter, frontChild, backChild);
        }

        private static bool AllConvex(List<Surface> surfaces)
        {
            foreach (var i in Enumerable.Range(0, surfaces.Count))
                foreach (var j in Enumerable.Range(i + 1, surfaces.Count - i - 1))
                    if (false == surfaces[i].IsConvexWith(surfaces[j]))
                        return false;

            return true;
        }

        static void Partition(List<Surface> surfaces, Hyperplane2D splitter,
            out List<Surface> frontSurfaces, out List<Surface> backSurfaces)
        {
            frontSurfaces = new List<Surface>();
            backSurfaces = new List<Surface>();

            foreach (var surface in surfaces)
            {
                Surface frontSurface, backSurface;
                surface.Split(splitter, out frontSurface, out backSurface);

                if (null != frontSurface)
                    frontSurfaces.Add(frontSurface);
                if (null != backSurface)
                    backSurfaces.Add(backSurface);
            }
        }

        static Hyperplane2D ChooseSplitter(List<Surface> surfacesToPartition)
        {
            var hyperplanes = surfacesToPartition.Select(s => s.Hyperplane)
                .Distinct().ToList();

            return hyperplanes.Select(h => ComputeScore(h, surfacesToPartition))
                .Where(s => s != null).OrderBy(s => s.Score).First().Splitter;
        }

        static SplitterScore ComputeScore(
            Hyperplane2D splitter, List<Surface> surfacesToPartition)
        {
            int splits = 0;
            int front = 0;
            int back = 0;

            foreach (var surface in surfacesToPartition)
            {
                var start = splitter.DetermineHalfspaceOf(surface.Start);
                var end = splitter.DetermineHalfspaceOf(surface.End);

                if (start > 0)
                {
                    if (end > 0)
                        front += 1;
                    else if (end < 0)
                        splits += 1;
                    else // end == 0
                        front += 1;
                }
                else if (start < 0)
                {
                    if (end > 0)
                        splits += 1;
                    else if (end < 0)
                        back += 1;
                    else // end == 0
                        back += 1;
                }
                else // start == 0
                {
                    if (end > 0)
                        front += 1;
                    else if (end < 0)
                        back += 1;
                    else // end == 0
                        if (surface.Hyperplane.Equals(splitter))
                        front += 1;
                    else
                        back += 1;
                }
            }

            if (splits == 0 && (front == 0 || back == 0))
                return null;
            else
                return new SplitterScore(splitter, front, back, splits);
        }

        class SplitterScore
        {
            private int back;
            private int front;
            private int splits;
            private Hyperplane2D splitter;

            public SplitterScore(Hyperplane2D splitter, int front, int back, int splits)
            {
                this.splitter = splitter;
                this.front = front;
                this.back = back;
                this.splits = splits;
            }

            public override string ToString()
            {
                return string.Format("{0} : {1} : {2}", front, splits, back);
            }

            public int Score
            {
                get { return Math.Abs(back - front) + 10 * splits; }
            }

            public Hyperplane2D Splitter
            {
                get { return splitter; }
            }
        }
    }
}
