using System;
using System.Linq;
using UnaryHeap.Utilities.Core;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class SimpleGraphTests
    {
        [Fact]
        public void IsDirected()
        {
            Assert.False(new SimpleGraph(false).IsDirected);
            Assert.True(new SimpleGraph(true).IsDirected);
        }

        [Fact]
        public void AddRemoveVertex()
        {
            var sut = new SimpleGraph(false);

            Assert.Equal(0, sut.NumVertices);

            Assert.Equal(0, sut.AddVertex());
            Assert.Equal(1, sut.NumVertices);

            Assert.Equal(1, sut.AddVertex());
            Assert.Equal(2, sut.AddVertex());
            Assert.Equal(3, sut.NumVertices);
            Assert.Equal(new[] { 0, 1, 2 }, sut.Vertices);
        }

        [Fact]
        public void GetNeighbors_Directed()
        {
            var sut = new SimpleGraph(true);
            var a = sut.AddVertex();
            var b = sut.AddVertex();
            var c = sut.AddVertex();
            var d = sut.AddVertex();
            sut.AddEdge(a, b);
            sut.AddEdge(b, c);
            sut.AddEdge(c, d);
            sut.AddEdge(d, a);

            Assert.Equal(new int[] { b }, sut.GetNeighbors(a));
            Assert.Equal(new int[] { c }, sut.GetNeighbors(b));
            Assert.Equal(new int[] { d }, sut.GetNeighbors(c));
            Assert.Equal(new int[] { a }, sut.GetNeighbors(d));
        }

        [Fact]
        public void GetNeighbors_Undirected()
        {
            var sut = new SimpleGraph(false);
            var a = sut.AddVertex();
            var b = sut.AddVertex();
            var c = sut.AddVertex();
            var d = sut.AddVertex();
            sut.AddEdge(a, b);
            sut.AddEdge(b, c);
            sut.AddEdge(c, d);
            sut.AddEdge(d, a);

            Assert.Equal(new int[] { b, d }, sut.GetNeighbors(a));
            Assert.Equal(new int[] { a, c }, sut.GetNeighbors(b));
            Assert.Equal(new int[] { b, d }, sut.GetNeighbors(c));
            Assert.Equal(new int[] { a, c }, sut.GetNeighbors(d));
        }

        [Fact]
        public void RemoveVertex_Directed()
        {
            var sut = new SimpleGraph(true);
            sut.AddVertex();
            sut.AddVertex();
            sut.AddVertex();
            sut.AddEdge(0, 1);
            sut.AddEdge(1, 2);
            sut.AddEdge(2, 0);

            Assert.Equal(new[] { 0, 1, 2 }, sut.Vertices);
            sut.RemoveVertex(1);
            Assert.Equal(new[] { 0, 1 }, sut.Vertices);

            Assert.True(sut.HasEdge(1, 0));
            Assert.False(sut.HasEdge(0, 1));
        }

        [Fact]
        public void RemoveVertex_Undirected()
        {
            var sut = new SimpleGraph(false);
            sut.AddVertex();
            sut.AddVertex();
            sut.AddVertex();
            sut.AddEdge(0, 1);
            sut.AddEdge(1, 2);
            sut.AddEdge(2, 0);

            sut.RemoveVertex(1);

            Assert.True(sut.HasEdge(1, 0));
            Assert.True(sut.HasEdge(0, 1));
        }

        [Fact]
        public void RemoveVertex_Directed_Cross()
        {
            var sut = new SimpleGraph(true);
            sut.AddVertex();
            sut.AddVertex();
            sut.AddVertex();
            sut.AddVertex();
            sut.AddVertex();
            sut.AddEdge(0, 1);
            sut.AddEdge(0, 2);
            sut.AddEdge(0, 3);
            sut.AddEdge(0, 4);

            sut.RemoveVertex(0);

            Assert.Empty(sut.GetNeighbors(0));
            Assert.Empty(sut.GetNeighbors(1));
            Assert.Empty(sut.GetNeighbors(2));
            Assert.Empty(sut.GetNeighbors(3));
        }

        [Fact]
        public void RemoveVertex_Undirected_Cross()
        {
            var sut = new SimpleGraph(false);
            sut.AddVertex();
            sut.AddVertex();
            sut.AddVertex();
            sut.AddVertex();
            sut.AddVertex();
            sut.AddEdge(0, 1);
            sut.AddEdge(0, 2);
            sut.AddEdge(0, 3);
            sut.AddEdge(0, 4);

            sut.RemoveVertex(0);

            Assert.Empty(sut.GetNeighbors(0));
            Assert.Empty(sut.GetNeighbors(1));
            Assert.Empty(sut.GetNeighbors(2));
            Assert.Empty(sut.GetNeighbors(3));
        }

        [Fact]
        public void AddEdge_Directed()
        {
            var sut = new SimpleGraph(true);
            var a = sut.AddVertex();
            var b = sut.AddVertex();

            sut.AddEdge(a, b);

            Assert.True(sut.HasEdge(a, b));
            Assert.False(sut.HasEdge(b, a));
            Assert.Equal(new int[] { b }, sut.GetNeighbors(a));
            Assert.Equal(new int[] { }, sut.GetNeighbors(b));
        }

        [Fact]
        public void AddEdge_Undirected()
        {
            var sut = new SimpleGraph(false);
            var a = sut.AddVertex();
            var b = sut.AddVertex();

            sut.AddEdge(a, b);

            Assert.True(sut.HasEdge(a, b));
            Assert.True(sut.HasEdge(b, a));
            Assert.Equal(new int[] { b }, sut.GetNeighbors(a));
            Assert.Equal(new int[] { a }, sut.GetNeighbors(b));
        }

        [Fact]
        public void RemoveEdge_Directed()
        {
            var sut = new SimpleGraph(true);
            var a = sut.AddVertex();
            var b = sut.AddVertex();
            sut.AddEdge(a, b);
            sut.AddEdge(b, a);

            sut.RemoveEdge(a, b);

            Assert.False(sut.HasEdge(a, b));
            Assert.True(sut.HasEdge(b, a));
        }

        [Fact]
        public void RemoveEdge_Undirected()
        {
            var sut = new SimpleGraph(false);
            var a = sut.AddVertex();
            var b = sut.AddVertex();
            sut.AddEdge(a, b);

            sut.RemoveEdge(b, a);

            Assert.False(sut.HasEdge(a, b));
            Assert.False(sut.HasEdge(b, a));
        }

        [Fact]
        public void AddEdge_ForbidLoop()
        {
            var sut = new SimpleGraph(false);
            sut.AddVertex();

            Assert.StartsWith("Start vertex equals end vertex.",
                Assert.Throws<ArgumentException>(() => { sut.AddEdge(0, 0); }).Message);
        }

        [Fact]
        public void AddEdge_Duplicate()
        {
            var sut = new SimpleGraph(false);
            sut.AddVertex();
            sut.AddVertex();
            sut.AddEdge(0, 1);
            Assert.True(sut.HasEdge(0, 1));

            Assert.StartsWith("The given edge already exists in the graph.",
                Assert.Throws<ArgumentException>(() => { sut.AddEdge(0, 1); }).Message);
        }

        [Fact]
        public void RemoveNonexistentEdge()
        {
            var sut = new SimpleGraph(false);
            sut.AddVertex();
            sut.AddVertex();

            Assert.False(sut.HasEdge(0, 1));

            Assert.StartsWith("The given edge was not present in the graph.",
                Assert.Throws<ArgumentException>(() => { sut.RemoveEdge(0, 1); }).Message);
        }

        [Fact]
        public void Edges_Directed()
        {
            var sut = K(4, true);

            Assert.Equal(new[] {
                Tuple.Create(0, 1),
                Tuple.Create(0, 2),
                Tuple.Create(0, 3),
                Tuple.Create(1, 0),
                Tuple.Create(1, 2),
                Tuple.Create(1, 3),
                Tuple.Create(2, 0),
                Tuple.Create(2, 1),
                Tuple.Create(2, 3),
                Tuple.Create(3, 0),
                Tuple.Create(3, 1),
                Tuple.Create(3, 2),
            }, sut.Edges);

            sut.RemoveVertex(0);

            Assert.Equal(new[] {
                Tuple.Create(0, 1),
                Tuple.Create(0, 2),
                Tuple.Create(1, 0),
                Tuple.Create(1, 2),
                Tuple.Create(2, 0),
                Tuple.Create(2, 1),
            }, sut.Edges);

            sut.RemoveVertex(0);

            Assert.Equal(new[] {
                Tuple.Create(0, 1),
                Tuple.Create(1, 0),
            }, sut.Edges);
        }

        [Fact]
        public void Edges_Undirected()
        {
            var sut = K(4, false);

            Assert.Equal(new[] {
                Tuple.Create(0, 1),
                Tuple.Create(0, 2),
                Tuple.Create(0, 3),
                Tuple.Create(1, 2),
                Tuple.Create(1, 3),
                Tuple.Create(2, 3),
            }, sut.Edges);

            sut.RemoveVertex(0);

            Assert.Equal(new[] {
                Tuple.Create(0, 1),
                Tuple.Create(0, 2),
                Tuple.Create(1, 2),
            }, sut.Edges);

            sut.RemoveVertex(0);

            Assert.Equal(new[] {
                Tuple.Create(0, 1),
            }, sut.Edges);
        }

        [Fact]
        public void Clone()
        {
            var source = K(5, true);
            var sut = source.Clone();

            while (source.NumVertices > 0)
                source.RemoveVertex(0);

            Assert.True(source.IsDirected);
            Assert.Equal(0, source.NumVertices);
            Assert.Equal(0, source.Edges.Count());

            Assert.True(sut.IsDirected);
            Assert.Equal(5, sut.NumVertices);
            Assert.Equal(20, sut.Edges.Count());
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            var sut = new SimpleGraph(true);
            sut.AddVertex();

            Assert.Throws<ArgumentOutOfRangeException>("index", () => { sut.RemoveVertex(-1); });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => { sut.RemoveVertex(1); });

            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.GetNeighbors(-1); });
            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.GetNeighbors(1); });

            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.HasEdge(-1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.HasEdge(1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>("to", () => { sut.HasEdge(0, -1); });
            Assert.Throws<ArgumentOutOfRangeException>("to", () => { sut.HasEdge(0, 1); });

            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.AddEdge(-1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.AddEdge(1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>("to", () => { sut.AddEdge(0, -1); });
            Assert.Throws<ArgumentOutOfRangeException>("to", () => { sut.AddEdge(0, 1); });

            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.RemoveEdge(-1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.RemoveEdge(1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>("to", () => { sut.RemoveEdge(0, -1); });
            Assert.Throws<ArgumentOutOfRangeException>("to", () => { sut.RemoveEdge(0, 1); });
        }

        static SimpleGraph K(int i, bool directed)
        {
            var sut = new SimpleGraph(directed);

            foreach (var a in Enumerable.Range(0, i))
            {
                sut.AddVertex();

                foreach (var b in Enumerable.Range(0, a))
                {
                    sut.AddEdge(a, b);

                    if (directed)
                        sut.AddEdge(b, a);
                }
            }
            return sut;
        }
    }
}
