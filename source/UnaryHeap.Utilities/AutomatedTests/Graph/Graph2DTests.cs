using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnaryHeap.DataType;

namespace UnaryHeap.Graph.Tests
{
    [TestFixture]
    public class Graph2DTests
    {
        [Test]
        public void VertexManipulation()
        {
            var sut = new Graph2D(true);

            Assert.True(sut.IsDirected);

            var vertex = new Point2D(1, 1);
            var vertex2 = new Point2D(2, 2);

            Assert.False(sut.HasVertex(vertex));
            Assert.False(sut.HasVertex(vertex2));
            Assert.AreEqual(0, sut.NumVertices);
            Assert.IsEmpty(sut.Vertices);

            sut.AddVertex(vertex);

            Assert.True(sut.HasVertex(vertex));
            Assert.False(sut.HasVertex(vertex2));
            Assert.AreEqual(1, sut.NumVertices);
            Assert.AreEqual(new[] { vertex }, sut.Vertices);

            sut.MoveVertex(vertex, vertex2);

            Assert.False(sut.HasVertex(vertex));
            Assert.True(sut.HasVertex(vertex2));
            Assert.AreEqual(1, sut.NumVertices);
            Assert.AreEqual(new[] { vertex2 }, sut.Vertices);

            sut.RemoveVertex(vertex2);

            Assert.False(sut.HasVertex(vertex));
            Assert.False(sut.HasVertex(vertex2));
            Assert.AreEqual(0, sut.NumVertices);
            Assert.IsEmpty(sut.Vertices);
        }

        [Test]
        public void VertexManipulation_IndexShift()
        {
            var vertex1 = new Point2D(1, 1);
            var vertex2 = new Point2D(2, 2);
            var vertex3 = new Point2D(3, 3);

            var sut = new Graph2D(false);
            sut.AddVertex(vertex1);
            sut.AddVertex(vertex2);
            sut.AddVertex(vertex3);
            sut.AddEdge(vertex1, vertex3);

            sut.RemoveVertex(vertex2);

            Assert.True(sut.HasVertex(vertex1));
            Assert.True(sut.HasVertex(vertex3));
            Assert.True(sut.HasEdge(vertex1, vertex3));
        }

        [Test]
        public void EdgeManipulation()
        {
            var sut = new Graph2D(false);

            var vertices = new[] {
                new Point2D(0, 0),
                new Point2D(0, 1),
                new Point2D(1, 0),
            };

            foreach (var vertex in vertices)
                sut.AddVertex(vertex);

            Assert.IsEmpty(sut.Edges);

            sut.AddEdge(vertices[0], vertices[1]);
            sut.AddEdge(vertices[1], vertices[2]);
            sut.AddEdge(vertices[2], vertices[0]);

            Assert.AreEqual(new[] {
                Tuple.Create(vertices[0], vertices[1]),
                Tuple.Create(vertices[0], vertices[2]),
                Tuple.Create(vertices[1], vertices[2]) },
                sut.Edges);

            Assert.AreEqual(new[] {
                vertices[0],
                vertices[2]
            }, sut.GetNeighbours(vertices[1]));
            Assert.AreEqual(2, sut.NumNeighbours(vertices[1]));

            Assert.True(sut.HasEdge(vertices[2], vertices[1]));

            sut.RemoveEdge(vertices[2], vertices[1]);

            Assert.False(sut.HasEdge(vertices[2], vertices[1]));
            Assert.AreEqual(new[] {
                Tuple.Create(vertices[0], vertices[1]),
                Tuple.Create(vertices[0], vertices[2]) },
                sut.Edges);

            Assert.AreEqual(new[] {
                vertices[0],
            }, sut.GetNeighbours(vertices[1]));
            Assert.AreEqual(1, sut.NumNeighbours(vertices[1]));
        }

        [Test]
        public void AddDuplicateVertex()
        {
            var sut = new Graph2D(true);
            sut.AddVertex(Point2D.Origin);

            Assert.That(
                Assert.Throws<ArgumentException>(
                    () => { sut.AddVertex(Point2D.Origin); })
                .Message.StartsWith(
                    "Graph already contains a vertex at the coordinates specified."));
        }

        [Test]
        public void MoveDuplicateVertex()
        {
            var sut = new Graph2D(true);
            sut.AddVertex(Point2D.Origin);
            sut.AddVertex(new Point2D(1, 1));

            Assert.That(
                Assert.Throws<ArgumentException>(
                    () => { sut.MoveVertex(Point2D.Origin, new Point2D(1, 1)); })
                .Message.StartsWith(
                    "Graph already contains a vertex at the coordinates specified."));
        }

        [Test]
        public void MoveVertexToInitialPosition()
        {
            var sut = new Graph2D(true);
            sut.AddVertex(Point2D.Origin);
            sut.MoveVertex(Point2D.Origin, Point2D.Origin);
        }

        [Test]
        public void Clone()
        {
            var a = new Point2D(1, 0);
            var b = new Point2D(0, 1);
            var source = new Graph2D(true);
            source.AddVertex(a);
            source.AddVertex(b);
            source.AddEdge(a, b);

            var sut = source.Clone();

            Assert.True(sut.HasVertex(a));
            Assert.True(sut.HasVertex(b));
            Assert.True(sut.HasEdge(a, b));
            Assert.AreEqual(new[] { Tuple.Create(a, b) }, sut.Edges);
        }

        [Test]
        public void Metadata()
        {
            const string Key = "fish";
            const string Value = "trout";
            const string Default = "mackarel";
            var a = new Point2D(0, 1);
            var b = new Point2D(1, 0);
            var sut = new Graph2D(true);
            sut.AddVertex(a);
            sut.AddVertex(b);
            sut.AddEdge(a, b);

            Assert.AreEqual(Default, sut.GetGraphMetadatum(Key, Default));
            Assert.AreEqual(0, sut.GraphMetadata.Count);
            sut.SetGraphMetadatum(Key, Value);
            Assert.AreEqual(Value, sut.GetGraphMetadatum(Key, Default));
            Assert.AreEqual(1, sut.GraphMetadata.Count);
            Assert.AreEqual(Value, sut.GraphMetadata[Key]);
            sut.UnsetGraphMetadatum(Key);
            Assert.AreEqual(Default, sut.GetGraphMetadatum(Key, Default));
            Assert.AreEqual(0, sut.GraphMetadata.Count);

            Assert.AreEqual(Default, sut.GetVertexMetadatum(a, Key, Default));
            Assert.AreEqual(1, sut.GetVertexMetadata(a).Count);
            sut.SetVertexMetadatum(a, Key, Value);
            Assert.AreEqual(Value, sut.GetVertexMetadatum(a, Key, Default));
            Assert.AreEqual(2, sut.GetVertexMetadata(a).Count);
            Assert.AreEqual(Value, sut.GetVertexMetadata(a)[Key]);
            sut.UnsetVertexMetadatum(a, Key);
            Assert.AreEqual(Default, sut.GetVertexMetadatum(a, Key, Default));
            Assert.AreEqual(1, sut.GetVertexMetadata(a).Count);

            Assert.AreEqual(Default, sut.GetEdgeMetadatum(a, b, Key, Default));
            Assert.AreEqual(0, sut.GetEdgeMetadata(a, b).Count);
            sut.SetEdgeMetadatum(a, b, Key, Value);
            Assert.AreEqual(Value, sut.GetEdgeMetadatum(a, b, Key, Default));
            Assert.AreEqual(1, sut.GetEdgeMetadata(a, b).Count);
            Assert.AreEqual(Value, sut.GetEdgeMetadata(a, b)[Key]);
            sut.UnsetEdgeMetadatum(a, b, Key);
            Assert.AreEqual(Default, sut.GetEdgeMetadatum(a, b, Key, Default));
            Assert.AreEqual(0, sut.GetEdgeMetadata(a, b).Count);
        }

        [Test]
        public void MetatadaReservedKey()
        {
            var a = new Point2D(0, 1);
            var sut = new Graph2D(true);
            sut.AddVertex(a);

            Assert.Throws<InvalidOperationException>(
                () => { sut.SetVertexMetadatum(a, "xy", "bacon"); });
            Assert.Throws<InvalidOperationException>(
                () => { sut.UnsetVertexMetadatum(a, "xy"); });
        }

        [Test]
        public void ToJson()
        {
            var sut = new Graph2D(false);
            sut.AddVertex(new Point2D(1, 3));

            using (var buffer = new StringWriter())
            {
                sut.ToJson(buffer);
                Assert.AreEqual(
                    "{\"structure\":{\"directed\":false,\"vertex_count\":1,\"edges\":[]}," +
                    "\"graph_metadata\":{},\"vertex_metadata\":[{\"xy\":\"1,3\"}]," +
                    "\"edge_metadata\":[]}",
                    buffer.ToString());
            }

            sut.MoveVertex(new Point2D(1, 3), new Point2D(3, 1));

            using (var buffer = new StringWriter())
            {
                sut.ToJson(buffer);
                Assert.AreEqual(
                    "{\"structure\":{\"directed\":false,\"vertex_count\":1,\"edges\":[]}," +
                    "\"graph_metadata\":{},\"vertex_metadata\":[{\"xy\":\"3,1\"}]," +
                    "\"edge_metadata\":[]}",
                    buffer.ToString());
            }
        }

        [Test]
        public void VertexNotPresentExceptions()
        {
            var a = new Point2D(2, 2);
            var sut = new Graph2D(false);
            sut.AddVertex(a);

            Assert.Throws<ArgumentException>(
                () => { sut.MoveVertex(Point2D.Origin, new Point2D(1, 1)); });
            Assert.Throws<ArgumentException>(
                () => { sut.RemoveVertex(Point2D.Origin); });
            Assert.Throws<ArgumentException>(
                () => { sut.AddEdge(Point2D.Origin, a); });
            Assert.Throws<ArgumentException>(
                () => { sut.AddEdge(a, Point2D.Origin); });
            Assert.Throws<ArgumentException>(
                () => { sut.RemoveEdge(Point2D.Origin, a); });
            Assert.Throws<ArgumentException>(
                () => { sut.RemoveEdge(a, Point2D.Origin); });
            Assert.Throws<ArgumentException>(
                () => { sut.HasEdge(Point2D.Origin, a); });
            Assert.Throws<ArgumentException>(
                () => { sut.HasEdge(a, Point2D.Origin); });
            Assert.Throws<ArgumentException>(
                () => { sut.GetNeighbours(Point2D.Origin); });
            Assert.Throws<ArgumentException>(
                () => { sut.NumNeighbours(Point2D.Origin); });
            Assert.Throws<ArgumentException>(
                () => { sut.GetVertexMetadatum(Point2D.Origin, "bacon"); });
            Assert.Throws<ArgumentException>(
                () => { sut.SetVertexMetadatum(Point2D.Origin, "bacon", "delicious"); });
            Assert.Throws<ArgumentException>(
                () => { sut.UnsetVertexMetadatum(Point2D.Origin, "bacon"); });
            Assert.Throws<ArgumentException>(
                () => { sut.GetEdgeMetadatum(Point2D.Origin, a, "bacon"); });
            Assert.Throws<ArgumentException>(
                () => { sut.SetEdgeMetadatum(Point2D.Origin, a, "bacon", "delicious"); });
            Assert.Throws<ArgumentException>(
                () => { sut.UnsetEdgeMetadatum(Point2D.Origin, a, "bacon"); });
            Assert.Throws<ArgumentException>(
                () => { sut.GetEdgeMetadatum(a, Point2D.Origin, "bacon"); });
            Assert.Throws<ArgumentException>(
                () => { sut.SetEdgeMetadatum(a, Point2D.Origin, "bacon", "delicious"); });
            Assert.Throws<ArgumentException>(
                () => { sut.UnsetEdgeMetadatum(a, Point2D.Origin, "bacon"); });
        }

        [Test]
        public void RemoveVertices()
        {
            var expected = MakeGrid(5);
            var actual = MakeGrid(5);

            var verticesToRemove = expected.Vertices.Where(i => i.X == i.Y).ToArray();

            foreach (var vertexToRemove in verticesToRemove)
                expected.RemoveVertex(vertexToRemove);

            actual.RemoveVertices(verticesToRemove);

            AssertJsonEqual(expected, actual);
            AssertSvgEqual(expected, actual);
        }

        [Test]
        public void RemoveVertices_Speed()
        {
            var sut = MakeGrid(40);

            var verticesToRemove = sut.Vertices.Where(v => (v.X - v.Y).AbsoluteValue < 3)
                .ToArray();

            var watch = new Stopwatch();
            watch.Start();
            sut.RemoveVertices(verticesToRemove);
            watch.Stop();

            Assert.AreEqual(1406, sut.NumVertices);
            Assert.Less(watch.ElapsedMilliseconds, 100);
        }

        static void AssertJsonEqual(Graph2D expected, Graph2D actual)
        {
            using (var expectedOut = new StringWriter())
            using (var actualOut = new StringWriter())
            {
                expected.ToJson(expectedOut);
                actual.ToJson(actualOut);

                Assert.AreEqual(expectedOut.ToString(), actualOut.ToString());
            }
        }

        static void AssertSvgEqual(Graph2D expected, Graph2D actual)
        {
            using (var expectedOut = new StringWriter())
            using (var actualOut = new StringWriter())
            {
                SvgGraph2DFormatter.Generate(expected, expectedOut);
                SvgGraph2DFormatter.Generate(actual, actualOut);

                Assert.AreEqual(expectedOut.ToString(), actualOut.ToString());
            }
        }

        private Graph2D MakeGrid(int edgeCount)
        {
            var result = new Graph2D(false);

            foreach (var x in Enumerable.Range(0, edgeCount))
                foreach (var y in Enumerable.Range(0, edgeCount))
                {
                    result.AddVertex(new Point2D(x, y));

                    if (x > 0)
                        result.AddEdge(new Point2D(x, y), new Point2D(x - 1, y));
                    if (y > 0)
                        result.AddEdge(new Point2D(x, y), new Point2D(x, y - 1));
                }

            return result;
        }

        [Test]
        public void DualMetadata()
        {
            var p1 = new Point2D(1, 1);
            var p2 = new Point2D(2, 2);
            var d1 = new Point2D(1, 2);
            var d2 = new Point2D(2, 1);

            var sut = new Graph2D(false);

            sut.AddVertex(p1);
            sut.AddVertex(p2);
            sut.AddEdge(p1, p2);

            sut.SetDualEdge(p1, p2, d1, d2);
            Assert.AreEqual("1,2;2,1", sut.GetEdgeMetadatum(
                p1, p2, Graph2DExtensions.DualMetadataKey));

            var dual = sut.GetDualEdge(p1, p2);

            Assert.AreEqual(d1, dual.Item1);
            Assert.AreEqual(d2, dual.Item2);

            sut.UnsetDualEdge(p1, p2);

            Assert.Null(sut.GetEdgeMetadatum(
                p1, p2, Graph2DExtensions.DualMetadataKey));
        }

        [Test]
        public void ReservedKeys()
        {
            Assert.True(Graph2D.IsReservedMetadataKey("xy"));
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            var a = new Point2D(0, 1);
            var b = new Point2D(1, 0);
            var sut = new Graph2D(true);
            sut.AddVertex(a);
            sut.AddVertex(b);
            sut.AddEdge(a, b);

            Assert.Throws<ArgumentNullException>(
                () => { sut.AddVertex(null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.HasVertex(null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.MoveVertex(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.MoveVertex(Point2D.Origin, null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.RemoveVertex(null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.AddEdge(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.AddEdge(Point2D.Origin, null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.RemoveEdge(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.RemoveEdge(Point2D.Origin, null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.HasEdge(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.HasEdge(Point2D.Origin, null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.GetNeighbours(null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.NumNeighbours(null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.ToJson(null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.GetVertexMetadatum(null, "bacon"); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.SetVertexMetadatum(null, "bacon", "delicious"); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.UnsetVertexMetadatum(null, "bacon"); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.GetEdgeMetadatum(null, a, "bacon"); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.SetEdgeMetadatum(null, a, "bacon", "delicious"); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.UnsetEdgeMetadatum(null, a, "bacon"); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.GetEdgeMetadatum(a, null, "bacon"); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.SetEdgeMetadatum(a, null, "bacon", "delicious"); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.UnsetEdgeMetadatum(a, null, "bacon"); });
            Assert.Throws<ArgumentNullException>(
                () => { Graph2D.IsReservedMetadataKey(null); });
        }
    }
}
