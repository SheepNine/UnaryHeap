using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Core;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class AnnotatedGraphTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void GraphMetadatum()
        {
            const string KEY = "color";
            var sut = new AnnotatedGraph(true);

            Assert.Null(sut.GetGraphMetadatum(KEY));
            Assert.Equal("blue", sut.GetGraphMetadatum(KEY, "blue"));

            sut.SetGraphMetadatum(KEY, "red");

            Assert.Equal("red", sut.GetGraphMetadatum(KEY));
            Assert.Equal("red", sut.GetGraphMetadatum(KEY, "blue"));

            sut.UnsetGraphMetadatum(KEY);

            Assert.Null(sut.GetGraphMetadatum(KEY));
            Assert.Equal("blue", sut.GetGraphMetadatum(KEY, "blue"));

            sut.UnsetGraphMetadatum(KEY);
            sut.SetGraphMetadatum(KEY, null);

            Assert.Null(sut.GetGraphMetadatum(KEY));
            Assert.Null(sut.GetGraphMetadatum(KEY, "blue"));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void VertexMetadatum()
        {
            const string KEY = "sticky";
            var sut = new AnnotatedGraph(true);
            var index = sut.AddVertex();

            Assert.Null(sut.GetVertexMetadatum(index, KEY));
            Assert.Equal("true", sut.GetVertexMetadatum(index, KEY, "true"));

            sut.SetVertexMetadatum(index, KEY, "false");

            Assert.Equal("false", sut.GetVertexMetadatum(index, KEY));
            Assert.Equal("false", sut.GetVertexMetadatum(index, KEY, "true"));

            sut.UnsetVertexMetadatum(index, KEY);

            Assert.Null(sut.GetVertexMetadatum(index, KEY));
            Assert.Equal("true", sut.GetVertexMetadatum(index, KEY, "true"));

            sut.UnsetVertexMetadatum(index, KEY);
            sut.SetVertexMetadatum(index, KEY, null);

            Assert.Null(sut.GetVertexMetadatum(index, KEY));
            Assert.Null(sut.GetVertexMetadatum(index, KEY, "true"));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void VertexMetadatum_NoVertex()
        {
            var KEY = "derp";
            var VALUE = "herp";
            var sut = new AnnotatedGraph(true);

            Assert.Throws<InvalidOperationException>(
                () => { sut.UnsetVertexMetadatum(0, KEY); });
            Assert.Throws<InvalidOperationException>(
                () => { sut.GetVertexMetadatum(0, KEY); });
            Assert.Throws<InvalidOperationException>(
                () => { sut.SetVertexMetadatum(0, KEY, VALUE); });

            sut.RemoveVertex(sut.AddVertex());

            Assert.Throws<InvalidOperationException>(
                () => { sut.UnsetVertexMetadatum(0, KEY); });
            Assert.Throws<InvalidOperationException>(
                () => { sut.GetVertexMetadatum(0, KEY); });
            Assert.Throws<InvalidOperationException>(
                () => { sut.SetVertexMetadatum(0, KEY, VALUE); });
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void EdgeMetadatum_Directed()
        {
            const string KEY = "elevation";
            var sut = new AnnotatedGraph(true);
            var a = sut.AddVertex();
            var b = sut.AddVertex();
            sut.AddEdge(a, b);
            sut.AddEdge(b, a);

            Assert.Null(sut.GetEdgeMetadatum(a, b, KEY));
            Assert.Equal("2.5", sut.GetEdgeMetadatum(a, b, KEY, "2.5"));
            Assert.Null(sut.GetEdgeMetadatum(b, a, KEY));

            sut.SetEdgeMetadatum(a, b, KEY, "8.6");
            sut.SetEdgeMetadatum(b, a, KEY, "-8.6");

            Assert.Equal("8.6", sut.GetEdgeMetadatum(a, b, KEY));
            Assert.Equal("8.6", sut.GetEdgeMetadatum(a, b, KEY, "2.5"));
            Assert.Equal("-8.6", sut.GetEdgeMetadatum(b, a, KEY));

            sut.UnsetEdgeMetadatum(a, b, KEY);

            Assert.Null(sut.GetEdgeMetadatum(a, b, KEY));
            Assert.Equal("2.5", sut.GetEdgeMetadatum(a, b, KEY, "2.5"));
            Assert.Equal("-8.6", sut.GetEdgeMetadatum(b, a, KEY));

            sut.UnsetEdgeMetadatum(a, b, KEY);
            sut.SetEdgeMetadatum(a, b, KEY, null);

            Assert.Null(sut.GetEdgeMetadatum(a, b, KEY));
            Assert.Null(sut.GetEdgeMetadatum(a, b, KEY, "2.5"));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void EdgeMetadatum_Undirected()
        {
            const string KEY = "elevation";
            var sut = new AnnotatedGraph(false);
            var a = sut.AddVertex();
            var b = sut.AddVertex();
            sut.AddEdge(a, b);

            Assert.Null(sut.GetEdgeMetadatum(a, b, KEY));
            Assert.Equal("2.5", sut.GetEdgeMetadatum(a, b, KEY, "2.5"));
            Assert.Null(sut.GetEdgeMetadatum(b, a, KEY));

            sut.SetEdgeMetadatum(a, b, KEY, "8.6");

            Assert.Equal("8.6", sut.GetEdgeMetadatum(a, b, KEY));
            Assert.Equal("8.6", sut.GetEdgeMetadatum(a, b, KEY, "2.5"));
            Assert.Equal("8.6", sut.GetEdgeMetadatum(b, a, KEY));

            sut.UnsetEdgeMetadatum(b, a, KEY);

            Assert.Null(sut.GetEdgeMetadatum(a, b, KEY));
            Assert.Equal("2.5", sut.GetEdgeMetadatum(a, b, KEY, "2.5"));
            Assert.Null(sut.GetEdgeMetadatum(b, a, KEY));

            sut.UnsetEdgeMetadatum(a, b, KEY);
            sut.SetEdgeMetadatum(a, b, KEY, null);

            Assert.Null(sut.GetEdgeMetadatum(a, b, KEY));
            Assert.Null(sut.GetEdgeMetadatum(a, b, KEY, "2.5"));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void EdgeMetadatum_NoEdge()
        {
            var KEY = "derp";
            var VALUE = "herp";
            var sut = new AnnotatedGraph(true);
            sut.AddVertex();
            sut.AddVertex();

            Assert.Throws<InvalidOperationException>(
                () => { sut.UnsetEdgeMetadatum(0, 1, KEY); });
            Assert.Throws<InvalidOperationException>(
                () => { sut.GetEdgeMetadatum(0, 1, KEY); });
            Assert.Throws<InvalidOperationException>(
                () => { sut.SetEdgeMetadatum(0, 1, KEY, VALUE); });

            sut.AddEdge(0, 1);
            sut.RemoveEdge(0, 1);

            Assert.Throws<InvalidOperationException>(
                () => { sut.UnsetEdgeMetadatum(0, 1, KEY); });
            Assert.Throws<InvalidOperationException>(
                () => { sut.GetEdgeMetadatum(0, 1, KEY); });
            Assert.Throws<InvalidOperationException>(
                () => { sut.SetEdgeMetadatum(0, 1, KEY, VALUE); });

            sut.AddEdge(0, 1);
            sut.RemoveVertex(1);
            sut.AddVertex();

            Assert.Throws<InvalidOperationException>(
                () => { sut.UnsetEdgeMetadatum(0, 1, KEY); });
            Assert.Throws<InvalidOperationException>(
                () => { sut.GetEdgeMetadatum(0, 1, KEY); });
            Assert.Throws<InvalidOperationException>(
                () => { sut.SetEdgeMetadatum(0, 1, KEY, VALUE); });
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void EdgeMetadatum_Deletes()
        {
            var KEY = "shibby";

            var sut = new AnnotatedGraph(false);
            sut.AddVertex();
            sut.AddVertex();
            sut.AddVertex();
            sut.AddVertex();
            sut.AddEdge(0, 3);
            sut.AddEdge(2, 3);
            sut.SetEdgeMetadatum(0, 3, KEY, "DUDE");
            sut.SetEdgeMetadatum(2, 3, KEY, "SWEET");

            sut.RemoveVertex(1);

            Assert.Equal("DUDE", sut.GetEdgeMetadatum(0, 2, KEY));
            Assert.Equal("SWEET", sut.GetEdgeMetadatum(1, 2, KEY));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void GraphMethods()
        {
            Assert.Equal(false, new AnnotatedGraph(false).IsDirected);
            Assert.Equal(true, new AnnotatedGraph(true).IsDirected);

            var expected = new SimpleGraph(true);
            var actual = new AnnotatedGraph(true);

            Assert.Equal(expected.NumVertices, actual.NumVertices);

            expected.AddVertex(); actual.AddVertex();

            Assert.Equal(expected.NumVertices, actual.NumVertices);
            Assert.Equal(expected.Vertices, actual.Vertices);

            expected.AddVertex(); actual.AddVertex();
            expected.AddVertex(); actual.AddVertex();

            Assert.Equal(expected.GetNeighbours(0), actual.GetNeighbours(0));

            expected.AddEdge(0, 1); actual.AddEdge(0, 1);

            Assert.Equal(expected.GetNeighbours(0), actual.GetNeighbours(0));
            Assert.Equal(expected.HasEdge(0, 1), actual.HasEdge(0, 1));
            Assert.Equal(expected.HasEdge(1, 2), actual.HasEdge(1, 2));
            Assert.Equal(expected.Edges, actual.Edges);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Clone()
        {
            const string Marklar = "marklar";
            var source = new AnnotatedGraph(true);

            source.SetGraphMetadatum(Marklar, Marklar);
            for (int i = 0; i < 5; i++)
            {
                source.AddVertex();
                source.SetVertexMetadatum(i, Marklar, Marklar);

                for (int j = 0; j < i; j++)
                {
                    source.AddEdge(i, j);
                    source.SetEdgeMetadatum(i, j, Marklar, Marklar);
                    source.AddEdge(j, i);
                    source.SetEdgeMetadatum(j, i, Marklar, Marklar);
                }
            }

            var sut = source.Clone();

            source.UnsetGraphMetadatum(Marklar);
            for (int i = 4; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    source.UnsetEdgeMetadatum(i, j, Marklar);
                    source.UnsetEdgeMetadatum(j, i, Marklar);
                    source.RemoveEdge(i, j);
                    source.RemoveEdge(j, i);
                }

                source.UnsetVertexMetadatum(i, Marklar);
                source.RemoveVertex(i);
            }

            Assert.True(sut.IsDirected);
            Assert.Equal(5, sut.NumVertices);
            Assert.Equal(20, sut.Edges.Count());

            for (int i = 0; i < 5; i++)
            {
                for (int j = i + 1; j < 5; j++)
                {
                    Assert.Equal(Marklar, sut.GetEdgeMetadatum(i, j, Marklar));
                    Assert.Equal(Marklar, sut.GetEdgeMetadatum(j, i, Marklar));
                }

                Assert.Equal(Marklar, sut.GetVertexMetadatum(i, Marklar));
            }
            Assert.Equal(Marklar, sut.GetGraphMetadatum(Marklar));
        }

        [Fact]
        public void RemoveVertices()
        {
            var actual = new AnnotatedGraph(false);
            var expected = new AnnotatedGraph(false);

            foreach (var i in Enumerable.Range(0, 20))
            {
                actual.AddVertex();
                expected.AddVertex();

                actual.SetVertexMetadatum(i, "index", i.ToString());
                expected.SetVertexMetadatum(i, "index", i.ToString());

                foreach (var j in Enumerable.Range(0, i))
                {
                    actual.AddEdge(i, j);
                    expected.AddEdge(i, j);

                    actual.SetEdgeMetadatum(i, j, "indices", string.Format("{0} {1}", i, j));
                    expected.SetEdgeMetadatum(i, j, "indices", string.Format("{0} {1}", i, j));
                }
            }

            var verticesToRemove = Enumerable.Range(0, 20).Where(i => i % 3 == 1)
                .Reverse().ToArray();
            var map = actual.RemoveVertices(verticesToRemove);

            foreach (var vertexToRemove in verticesToRemove)
                expected.RemoveVertex(vertexToRemove);

            AssertJsonEqual(expected, actual);

            Assert.Equal(
                new[] {
                    00, -1, 01,
                    02, -1, 03,
                    04, -1, 05,
                    06, -1, 07,
                    08, -1, 09,
                    10, -1, 11,
                    12, -1,
                }, map);
        }

        [Fact]
        public void RemoveVertices_Speed()
        {
            var sut = new AnnotatedGraph(false);

            foreach (var i in Enumerable.Range(0, 1000))
            {
                sut.AddVertex();
                sut.SetVertexMetadatum(i, "index", i.ToString());

                foreach (var j in Enumerable.Range(0, i))
                {
                    sut.AddEdge(i, j);
                    sut.SetEdgeMetadatum(i, j, "indices", string.Format("{0} {1}", i, j));
                }
            }

            var verticesToRemove = Enumerable.Range(0, 1000).Where(i => i % 3 == 1);

            var watch = new Stopwatch();
            watch.Start();
            sut.RemoveVertices(verticesToRemove);
            watch.Stop();

            Assert.Equal(667, sut.NumVertices);
            Assert.True(350 > watch.ElapsedMilliseconds);
        }

        static void AssertJsonEqual(AnnotatedGraph a, AnnotatedGraph b)
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            var sut = new AnnotatedGraph(false);
            sut.AddVertex();
            sut.AddVertex();
            sut.AddEdge(0, 1);

            Assert.Throws<ArgumentNullException>("key",
                () => { sut.UnsetGraphMetadatum(null); });
            Assert.Throws<ArgumentNullException>("key",
                () => { sut.SetGraphMetadatum(null, "blob"); });
            Assert.Throws<ArgumentNullException>("key",
                () => { sut.GetGraphMetadatum(null); });

            Assert.Throws<ArgumentNullException>("key",
                () => { sut.UnsetVertexMetadatum(0, null); });
            Assert.Throws<ArgumentNullException>("key",
                () => { sut.SetVertexMetadatum(0, null, "blob"); });
            Assert.Throws<ArgumentNullException>("key",
                () => { sut.GetVertexMetadatum(0, null); });

            Assert.Throws<ArgumentNullException>("key",
                () => { sut.UnsetEdgeMetadatum(0, 1, null); });
            Assert.Throws<ArgumentNullException>("key",
                () => { sut.SetEdgeMetadatum(0, 1, null, "blob"); });
            Assert.Throws<ArgumentNullException>("key",
                () => { sut.GetEdgeMetadatum(0, 1, null); });
        }
    }
}
