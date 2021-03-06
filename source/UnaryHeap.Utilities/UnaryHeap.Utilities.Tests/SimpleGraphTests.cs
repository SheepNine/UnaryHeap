﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Core;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class SimpleGraphTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void IsDirected()
        {
            Assert.False(new SimpleGraph(false).IsDirected);
            Assert.True(new SimpleGraph(true).IsDirected);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        public void RemoveVertices()
        {
            var sut = new SimpleGraph(false);
            var sut2 = new SimpleGraph(false);

            foreach (var i in Enumerable.Range(0, 20))
            {
                sut.AddVertex();
                sut2.AddVertex();

                foreach (var j in Enumerable.Range(0, i))
                {
                    sut.AddEdge(i, j);
                    sut2.AddEdge(i, j);
                }
            }

            var verticesToRemove = Enumerable.Range(0, 20).Where(i => i % 3 == 1)
                .Reverse().ToArray();
            var map = sut.RemoveVertices(verticesToRemove);

            foreach (var vertexToRemove in verticesToRemove)
                sut2.RemoveVertex(vertexToRemove);

            AssertJsonEqual(sut, sut2);

            Assert.Equal(
                new[] { 0, -1, 1, 2, -1, 3, 4, -1, 5, 6, -1, 7, 8, -1, 9, 10, -1, 11, 12, -1, },
                map);
        }

        static void AssertJsonEqual(SimpleGraph a, SimpleGraph b)
        {
            using (var aOut = new StringWriter())
            using (var bOut = new StringWriter())
            {
                a.ToJson(aOut);
                b.ToJson(bOut);

                Assert.Equal(aOut.ToString(), bOut.ToString());
            }
        }

        [Fact]
        public void RemoveVertices_Speed()
        {
            var sut = new SimpleGraph(false);

            foreach (var i in Enumerable.Range(0, 1000))
            {
                sut.AddVertex();

                foreach (var j in Enumerable.Range(0, i))
                    sut.AddEdge(i, j);
            }

            var verticesToRemove = Enumerable.Range(0, 1000).Where(i => i % 3 == 1)
                .Reverse().ToArray();

            var watch = new Stopwatch();
            watch.Start();
            sut.RemoveVertices(verticesToRemove);
            watch.Stop();

            Assert.Equal(667, sut.NumVertices);
            Assert.True(250 > watch.ElapsedMilliseconds);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void GetNeighbours_Directed()
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

            Assert.Equal(new int[] { b }, sut.GetNeighbours(a));
            Assert.Equal(new int[] { c }, sut.GetNeighbours(b));
            Assert.Equal(new int[] { d }, sut.GetNeighbours(c));
            Assert.Equal(new int[] { a }, sut.GetNeighbours(d));

            Assert.Equal(1, sut.NumNeighbours(a));
            Assert.Equal(1, sut.NumNeighbours(b));
            Assert.Equal(1, sut.NumNeighbours(c));
            Assert.Equal(1, sut.NumNeighbours(d));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void GetNeighbours_Undirected()
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

            Assert.Equal(new int[] { b, d }, sut.GetNeighbours(a));
            Assert.Equal(new int[] { a, c }, sut.GetNeighbours(b));
            Assert.Equal(new int[] { b, d }, sut.GetNeighbours(c));
            Assert.Equal(new int[] { a, c }, sut.GetNeighbours(d));

            Assert.Equal(2, sut.NumNeighbours(a));
            Assert.Equal(2, sut.NumNeighbours(b));
            Assert.Equal(2, sut.NumNeighbours(c));
            Assert.Equal(2, sut.NumNeighbours(d));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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

            Assert.Empty(sut.GetNeighbours(0));
            Assert.Empty(sut.GetNeighbours(1));
            Assert.Empty(sut.GetNeighbours(2));
            Assert.Empty(sut.GetNeighbours(3));

            Assert.Equal(0, sut.NumNeighbours(0));
            Assert.Equal(0, sut.NumNeighbours(1));
            Assert.Equal(0, sut.NumNeighbours(2));
            Assert.Equal(0, sut.NumNeighbours(3));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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

            Assert.Empty(sut.GetNeighbours(0));
            Assert.Empty(sut.GetNeighbours(1));
            Assert.Empty(sut.GetNeighbours(2));
            Assert.Empty(sut.GetNeighbours(3));

            Assert.Equal(0, sut.NumNeighbours(0));
            Assert.Equal(0, sut.NumNeighbours(1));
            Assert.Equal(0, sut.NumNeighbours(2));
            Assert.Equal(0, sut.NumNeighbours(3));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void AddEdge_Directed()
        {
            var sut = new SimpleGraph(true);
            var a = sut.AddVertex();
            var b = sut.AddVertex();

            sut.AddEdge(a, b);

            Assert.True(sut.HasEdge(a, b));
            Assert.False(sut.HasEdge(b, a));
            Assert.Equal(new int[] { b }, sut.GetNeighbours(a));
            Assert.Equal(new int[] { }, sut.GetNeighbours(b));

            Assert.Equal(1, sut.NumNeighbours(a));
            Assert.Equal(0, sut.NumNeighbours(b));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void AddEdge_Undirected()
        {
            var sut = new SimpleGraph(false);
            var a = sut.AddVertex();
            var b = sut.AddVertex();

            sut.AddEdge(a, b);

            Assert.True(sut.HasEdge(a, b));
            Assert.True(sut.HasEdge(b, a));
            Assert.Equal(new int[] { b }, sut.GetNeighbours(a));
            Assert.Equal(new int[] { a }, sut.GetNeighbours(b));

            Assert.Equal(1, sut.NumNeighbours(a));
            Assert.Equal(1, sut.NumNeighbours(b));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void AddEdge_ForbidLoop()
        {
            var sut = new SimpleGraph(false);
            sut.AddVertex();

            Assert.StartsWith("Start vertex equals end vertex.",
                Assert.Throws<ArgumentException>(() => { sut.AddEdge(0, 0); }).Message);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            var sut = new SimpleGraph(true);
            sut.AddVertex();

            Assert.Throws<ArgumentOutOfRangeException>("index", () => { sut.RemoveVertex(-1); });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => { sut.RemoveVertex(1); });

            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.GetNeighbours(-1); });
            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.GetNeighbours(1); });

            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.NumNeighbours(-1); });
            Assert.Throws<ArgumentOutOfRangeException>("from", () => { sut.NumNeighbours(1); });

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

            Assert.Throws<ArgumentNullException>("indexes",
                () => { sut.RemoveVertices(null); });
            Assert.Throws<ArgumentException>("indexes",
                () => { sut.RemoveVertices(new[] { 0, 0 }); });
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
