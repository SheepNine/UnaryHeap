using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Core;
using NUnit.Framework;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class SimpleGraphTests
    {
        [Test]
        public void IsDirected()
        {
            Assert.False(new SimpleGraph(false).IsDirected);
            Assert.True(new SimpleGraph(true).IsDirected);
        }

        [Test]
        public void AddRemoveVertex()
        {
            var sut = new SimpleGraph(false);

            Assert.AreEqual(0, sut.NumVertices);

            Assert.AreEqual(0, sut.AddVertex());
            Assert.AreEqual(1, sut.NumVertices);

            Assert.AreEqual(1, sut.AddVertex());
            Assert.AreEqual(2, sut.AddVertex());
            Assert.AreEqual(3, sut.NumVertices);
            Assert.AreEqual(new[] { 0, 1, 2 }, sut.Vertices);
        }

        [Test]
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

            Assert.AreEqual(
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

                Assert.AreEqual(aOut.ToString(), bOut.ToString());
            }
        }

        [Test]
        [Category(Traits.Slow)]
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

            Assert.AreEqual(667, sut.NumVertices);
            Assert.Less(watch.ElapsedMilliseconds, 500);
        }

        [Test]
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

            Assert.AreEqual(new int[] { b }, sut.GetNeighbours(a));
            Assert.AreEqual(new int[] { c }, sut.GetNeighbours(b));
            Assert.AreEqual(new int[] { d }, sut.GetNeighbours(c));
            Assert.AreEqual(new int[] { a }, sut.GetNeighbours(d));

            Assert.AreEqual(1, sut.NumNeighbours(a));
            Assert.AreEqual(1, sut.NumNeighbours(b));
            Assert.AreEqual(1, sut.NumNeighbours(c));
            Assert.AreEqual(1, sut.NumNeighbours(d));
        }

        [Test]
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

            Assert.AreEqual(new int[] { b, d }, sut.GetNeighbours(a));
            Assert.AreEqual(new int[] { a, c }, sut.GetNeighbours(b));
            Assert.AreEqual(new int[] { b, d }, sut.GetNeighbours(c));
            Assert.AreEqual(new int[] { a, c }, sut.GetNeighbours(d));

            Assert.AreEqual(2, sut.NumNeighbours(a));
            Assert.AreEqual(2, sut.NumNeighbours(b));
            Assert.AreEqual(2, sut.NumNeighbours(c));
            Assert.AreEqual(2, sut.NumNeighbours(d));
        }

        [Test]
        public void RemoveVertex_Directed()
        {
            var sut = new SimpleGraph(true);
            sut.AddVertex();
            sut.AddVertex();
            sut.AddVertex();
            sut.AddEdge(0, 1);
            sut.AddEdge(1, 2);
            sut.AddEdge(2, 0);

            Assert.AreEqual(new[] { 0, 1, 2 }, sut.Vertices);
            sut.RemoveVertex(1);
            Assert.AreEqual(new[] { 0, 1 }, sut.Vertices);

            Assert.True(sut.HasEdge(1, 0));
            Assert.False(sut.HasEdge(0, 1));
        }

        [Test]
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

        [Test]
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

            Assert.IsEmpty(sut.GetNeighbours(0));
            Assert.IsEmpty(sut.GetNeighbours(1));
            Assert.IsEmpty(sut.GetNeighbours(2));
            Assert.IsEmpty(sut.GetNeighbours(3));

            Assert.AreEqual(0, sut.NumNeighbours(0));
            Assert.AreEqual(0, sut.NumNeighbours(1));
            Assert.AreEqual(0, sut.NumNeighbours(2));
            Assert.AreEqual(0, sut.NumNeighbours(3));
        }

        [Test]
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

            Assert.IsEmpty(sut.GetNeighbours(0));
            Assert.IsEmpty(sut.GetNeighbours(1));
            Assert.IsEmpty(sut.GetNeighbours(2));
            Assert.IsEmpty(sut.GetNeighbours(3));

            Assert.AreEqual(0, sut.NumNeighbours(0));
            Assert.AreEqual(0, sut.NumNeighbours(1));
            Assert.AreEqual(0, sut.NumNeighbours(2));
            Assert.AreEqual(0, sut.NumNeighbours(3));
        }

        [Test]
        public void AddEdge_Directed()
        {
            var sut = new SimpleGraph(true);
            var a = sut.AddVertex();
            var b = sut.AddVertex();

            sut.AddEdge(a, b);

            Assert.True(sut.HasEdge(a, b));
            Assert.False(sut.HasEdge(b, a));
            Assert.AreEqual(new int[] { b }, sut.GetNeighbours(a));
            Assert.AreEqual(new int[] { }, sut.GetNeighbours(b));

            Assert.AreEqual(1, sut.NumNeighbours(a));
            Assert.AreEqual(0, sut.NumNeighbours(b));
        }

        [Test]
        public void AddEdge_Undirected()
        {
            var sut = new SimpleGraph(false);
            var a = sut.AddVertex();
            var b = sut.AddVertex();

            sut.AddEdge(a, b);

            Assert.True(sut.HasEdge(a, b));
            Assert.True(sut.HasEdge(b, a));
            Assert.AreEqual(new int[] { b }, sut.GetNeighbours(a));
            Assert.AreEqual(new int[] { a }, sut.GetNeighbours(b));

            Assert.AreEqual(1, sut.NumNeighbours(a));
            Assert.AreEqual(1, sut.NumNeighbours(b));
        }

        [Test]
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

        [Test]
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

        [Test]
        public void AddEdge_ForbidLoop()
        {
            var sut = new SimpleGraph(false);
            sut.AddVertex();

            Assert.That(
                Assert.Throws<ArgumentException>(
                    () => { sut.AddEdge(0, 0); })
                .Message.StartsWith(
                    "Start vertex equals end vertex."));
        }

        [Test]
        public void AddEdge_Duplicate()
        {
            var sut = new SimpleGraph(false);
            sut.AddVertex();
            sut.AddVertex();
            sut.AddEdge(0, 1);
            Assert.True(sut.HasEdge(0, 1));

            Assert.That(
                Assert.Throws<ArgumentException>(
                    () => { sut.AddEdge(0, 1); })
                .Message.StartsWith(
                    "The given edge already exists in the graph."));
        }

        [Test]
        public void RemoveNonexistentEdge()
        {
            var sut = new SimpleGraph(false);
            sut.AddVertex();
            sut.AddVertex();

            Assert.False(sut.HasEdge(0, 1));

            Assert.That(
                Assert.Throws<ArgumentException>(
                    () => { sut.RemoveEdge(0, 1); })
                .Message.StartsWith(
                    "The given edge was not present in the graph."));
        }

        [Test]
        public void Edges_Directed()
        {
            var sut = K(4, true);

            Assert.AreEqual(new[] {
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

            Assert.AreEqual(new[] {
                Tuple.Create(0, 1),
                Tuple.Create(0, 2),
                Tuple.Create(1, 0),
                Tuple.Create(1, 2),
                Tuple.Create(2, 0),
                Tuple.Create(2, 1),
            }, sut.Edges);

            sut.RemoveVertex(0);

            Assert.AreEqual(new[] {
                Tuple.Create(0, 1),
                Tuple.Create(1, 0),
            }, sut.Edges);
        }

        [Test]
        public void Edges_Undirected()
        {
            var sut = K(4, false);

            Assert.AreEqual(new[] {
                Tuple.Create(0, 1),
                Tuple.Create(0, 2),
                Tuple.Create(0, 3),
                Tuple.Create(1, 2),
                Tuple.Create(1, 3),
                Tuple.Create(2, 3),
            }, sut.Edges);

            sut.RemoveVertex(0);

            Assert.AreEqual(new[] {
                Tuple.Create(0, 1),
                Tuple.Create(0, 2),
                Tuple.Create(1, 2),
            }, sut.Edges);

            sut.RemoveVertex(0);

            Assert.AreEqual(new[] {
                Tuple.Create(0, 1),
            }, sut.Edges);
        }

        [Test]
        public void Clone()
        {
            var source = K(5, true);
            var sut = source.Clone();

            while (source.NumVertices > 0)
                source.RemoveVertex(0);

            Assert.True(source.IsDirected);
            Assert.AreEqual(0, source.NumVertices);
            Assert.AreEqual(0, source.Edges.Count());

            Assert.True(sut.IsDirected);
            Assert.AreEqual(5, sut.NumVertices);
            Assert.AreEqual(20, sut.Edges.Count());
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            var sut = new SimpleGraph(true);
            sut.AddVertex();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.RemoveVertex(-1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.RemoveVertex(1); });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.GetNeighbours(-1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.GetNeighbours(1); });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.NumNeighbours(-1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.NumNeighbours(1); });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.HasEdge(-1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.HasEdge(1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.HasEdge(0, -1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.HasEdge(0, 1); });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.AddEdge(-1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.AddEdge(1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.AddEdge(0, -1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.AddEdge(0, 1); });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.RemoveEdge(-1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.RemoveEdge(1, 0); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.RemoveEdge(0, -1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { sut.RemoveEdge(0, 1); });

            Assert.Throws<ArgumentNullException>(
                () => { sut.RemoveVertices(null); });
            Assert.Throws<ArgumentException>(
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
