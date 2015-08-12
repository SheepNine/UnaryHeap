using System;
using System.IO;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class Graph2DTests
    {
        [Fact]
        public void VertexManipulation()
        {
            var sut = new Graph2D(true);

            Assert.True(sut.IsDirected);

            var vertex = new Point2D(1, 1);
            var vertex2 = new Point2D(2, 2);

            Assert.False(sut.HasVertex(vertex));
            Assert.False(sut.HasVertex(vertex2));
            Assert.Equal(0, sut.NumVertices);
            Assert.Empty(sut.Vertices);

            sut.AddVertex(vertex);

            Assert.True(sut.HasVertex(vertex));
            Assert.False(sut.HasVertex(vertex2));
            Assert.Equal(1, sut.NumVertices);
            Assert.Equal(new[] { vertex }, sut.Vertices);

            sut.MoveVertex(vertex, vertex2);

            Assert.False(sut.HasVertex(vertex));
            Assert.True(sut.HasVertex(vertex2));
            Assert.Equal(1, sut.NumVertices);
            Assert.Equal(new[] { vertex2 }, sut.Vertices);

            sut.RemoveVertex(vertex2);

            Assert.False(sut.HasVertex(vertex));
            Assert.False(sut.HasVertex(vertex2));
            Assert.Equal(0, sut.NumVertices);
            Assert.Empty(sut.Vertices);
        }

        [Fact]
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

            Assert.Empty(sut.Edges);

            sut.AddEdge(vertices[0], vertices[1]);
            sut.AddEdge(vertices[1], vertices[2]);
            sut.AddEdge(vertices[2], vertices[0]);

            Assert.Equal(new[] {
                Tuple.Create(vertices[0], vertices[1]),
                Tuple.Create(vertices[0], vertices[2]),
                Tuple.Create(vertices[1], vertices[2]) },
                sut.Edges);

            Assert.Equal(new[] {
                vertices[0],
                vertices[2]
            }, sut.GetNeighbours(vertices[1]));

            Assert.True(sut.HasEdge(vertices[2], vertices[1]));

            sut.RemoveEdge(vertices[2], vertices[1]);

            Assert.False(sut.HasEdge(vertices[2], vertices[1]));
            Assert.Equal(new[] {
                Tuple.Create(vertices[0], vertices[1]),
                Tuple.Create(vertices[0], vertices[2]) },
                sut.Edges);

            Assert.Equal(new[] {
                vertices[0],
            }, sut.GetNeighbours(vertices[1]));
        }

        [Fact]
        public void AddDuplicateVertex()
        {
            var sut = new Graph2D(true);
            sut.AddVertex(Point2D.Origin);

            Assert.StartsWith("Graph already contains a vertex at the coordinates specified.",
                Assert.Throws<ArgumentException>("coordinates",
                () => { sut.AddVertex(Point2D.Origin); }).Message);
        }

        [Fact]
        public void MoveDuplicateVertex()
        {
            var sut = new Graph2D(true);
            sut.AddVertex(Point2D.Origin);
            sut.AddVertex(new Point2D(1, 1));

            Assert.StartsWith("Graph already contains a vertex at the coordinates specified.",
                Assert.Throws<ArgumentException>("destination",
                () => { sut.MoveVertex(Point2D.Origin, new Point2D(1, 1)); }).Message);
        }

        [Fact]
        public void MoveVertexToInitialPosition()
        {
            var sut = new Graph2D(true);
            sut.AddVertex(Point2D.Origin);
            sut.MoveVertex(Point2D.Origin, Point2D.Origin);
        }

        [Fact]
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
            Assert.Equal(new[] { Tuple.Create(a, b) }, sut.Edges);
        }

        [Fact]
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

            Assert.Equal(Default, sut.GetGraphMetadatum(Key, Default));
            sut.SetGraphMetadatum(Key, Value);
            Assert.Equal(Value, sut.GetGraphMetadatum(Key, Default));
            sut.UnsetGraphMetadatum(Key);
            Assert.Equal(Default, sut.GetGraphMetadatum(Key, Default));

            Assert.Equal(Default, sut.GetVertexMetadatum(a, Key, Default));
            sut.SetVertexMetadatum(a, Key, Value);
            Assert.Equal(Value, sut.GetVertexMetadatum(a, Key, Default));
            sut.UnsetVertexMetadatum(a, Key);
            Assert.Equal(Default, sut.GetVertexMetadatum(a, Key, Default));

            Assert.Equal(Default, sut.GetEdgeMetadatum(a, b, Key, Default));
            sut.SetEdgeMetadatum(a, b, Key, Value);
            Assert.Equal(Value, sut.GetEdgeMetadatum(a, b, Key, Default));
            sut.UnsetEdgeMetadatum(a, b, Key);
            Assert.Equal(Default, sut.GetEdgeMetadatum(a, b, Key, Default));
        }

        [Fact]
        public void MetatadaReservedKey()
        {
            var a = new Point2D(0, 1);
            var sut = new Graph2D(true);
            sut.AddVertex(a);

            Assert.Throws<InvalidOperationException>(() => { sut.SetVertexMetadatum(a, "xy", "bacon"); });
            Assert.Throws<InvalidOperationException>(() => { sut.UnsetVertexMetadatum(a, "xy"); });
        }

        [Fact]
        public void ToJson()
        {
            var sut = new Graph2D(false);
            sut.AddVertex(new Point2D(1, 3));

            using (var buffer = new StringWriter())
            {
                sut.ToJson(buffer);
                Assert.Equal("{\"structure\":{\"directed\":false,\"vertex_count\":1,\"edges\":[]},\"graph_metadata\":{},\"vertex_metadata\":[{\"xy\":\"1,3\"}],\"edge_metadata\":[]}", buffer.ToString());
            }

            sut.MoveVertex(new Point2D(1, 3), new Point2D(3, 1));

            using (var buffer = new StringWriter())
            {
                sut.ToJson(buffer);
                Assert.Equal("{\"structure\":{\"directed\":false,\"vertex_count\":1,\"edges\":[]},\"graph_metadata\":{},\"vertex_metadata\":[{\"xy\":\"3,1\"}],\"edge_metadata\":[]}", buffer.ToString());
            }
        }

        [Fact]
        public void VertexNotPresentExceptions()
        {
            var a = new Point2D(2, 2);
            var sut = new Graph2D(false);
            sut.AddVertex(a);

            Assert.Throws<ArgumentException>("origin", () => { sut.MoveVertex(Point2D.Origin, new Point2D(1, 1)); });
            Assert.Throws<ArgumentException>("vertex", () => { sut.RemoveVertex(Point2D.Origin); });
            Assert.Throws<ArgumentException>("from", () => { sut.AddEdge(Point2D.Origin, a); });
            Assert.Throws<ArgumentException>("to", () => { sut.AddEdge(a, Point2D.Origin); });
            Assert.Throws<ArgumentException>("from", () => { sut.RemoveEdge(Point2D.Origin, a); });
            Assert.Throws<ArgumentException>("to", () => { sut.RemoveEdge(a, Point2D.Origin); });
            Assert.Throws<ArgumentException>("from", () => { sut.HasEdge(Point2D.Origin, a); });
            Assert.Throws<ArgumentException>("to", () => { sut.HasEdge(a, Point2D.Origin); });
            Assert.Throws<ArgumentException>("from", () => { sut.GetNeighbours(Point2D.Origin); });
            Assert.Throws<ArgumentException>("vertex", () => { sut.GetVertexMetadatum(Point2D.Origin, "bacon"); });
            Assert.Throws<ArgumentException>("vertex", () => { sut.SetVertexMetadatum(Point2D.Origin, "bacon", "delicious"); });
            Assert.Throws<ArgumentException>("vertex", () => { sut.UnsetVertexMetadatum(Point2D.Origin, "bacon"); });
            Assert.Throws<ArgumentException>("from", () => { sut.GetEdgeMetadatum(Point2D.Origin, a, "bacon"); });
            Assert.Throws<ArgumentException>("from", () => { sut.SetEdgeMetadatum(Point2D.Origin, a, "bacon", "delicious"); });
            Assert.Throws<ArgumentException>("from", () => { sut.UnsetEdgeMetadatum(Point2D.Origin, a, "bacon"); });
            Assert.Throws<ArgumentException>("to", () => { sut.GetEdgeMetadatum(a, Point2D.Origin, "bacon"); });
            Assert.Throws<ArgumentException>("to", () => { sut.SetEdgeMetadatum(a, Point2D.Origin, "bacon", "delicious"); });
            Assert.Throws<ArgumentException>("to", () => { sut.UnsetEdgeMetadatum(a, Point2D.Origin, "bacon"); });
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            var a = new Point2D(0, 1);
            var b = new Point2D(1, 0);
            var sut = new Graph2D(true);
            sut.AddVertex(a);
            sut.AddVertex(b);
            sut.AddEdge(a, b);

            Assert.Throws<ArgumentNullException>("coordinates", () => { sut.AddVertex(null); });
            Assert.Throws<ArgumentNullException>("coordinates", () => { sut.HasVertex(null); });
            Assert.Throws<ArgumentNullException>("origin", () => { sut.MoveVertex(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("destination", () => { sut.MoveVertex(Point2D.Origin, null); });
            Assert.Throws<ArgumentNullException>("vertex", () => { sut.RemoveVertex(null); });
            Assert.Throws<ArgumentNullException>("from", () => { sut.AddEdge(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("to", () => { sut.AddEdge(Point2D.Origin, null); });
            Assert.Throws<ArgumentNullException>("from", () => { sut.RemoveEdge(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("to", () => { sut.RemoveEdge(Point2D.Origin, null); });
            Assert.Throws<ArgumentNullException>("from", () => { sut.HasEdge(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("to", () => { sut.HasEdge(Point2D.Origin, null); });
            Assert.Throws<ArgumentNullException>("from", () => { sut.GetNeighbours(null); });
            Assert.Throws<ArgumentNullException>("output", () => { sut.ToJson(null); });
            Assert.Throws<ArgumentNullException>("vertex", () => { sut.GetVertexMetadatum(null, "bacon"); });
            Assert.Throws<ArgumentNullException>("vertex", () => { sut.SetVertexMetadatum(null, "bacon", "delicious"); });
            Assert.Throws<ArgumentNullException>("vertex", () => { sut.UnsetVertexMetadatum(null, "bacon"); });
            Assert.Throws<ArgumentNullException>("from", () => { sut.GetEdgeMetadatum(null, a, "bacon"); });
            Assert.Throws<ArgumentNullException>("from", () => { sut.SetEdgeMetadatum(null, a, "bacon", "delicious"); });
            Assert.Throws<ArgumentNullException>("from", () => { sut.UnsetEdgeMetadatum(null, a, "bacon"); });
            Assert.Throws<ArgumentNullException>("to", () => { sut.GetEdgeMetadatum(a, null, "bacon"); });
            Assert.Throws<ArgumentNullException>("to", () => { sut.SetEdgeMetadatum(a, null, "bacon", "delicious"); });
            Assert.Throws<ArgumentNullException>("to", () => { sut.UnsetEdgeMetadatum(a, null, "bacon"); });
        }
    }
}
