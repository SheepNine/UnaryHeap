using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.Graph.Tests
{
    [TestFixture]
    public class SimpleGraphIOTests
    {
        [Test]
        public void EmptyDirectedGraph()
        {
            var text = "{\"directed\":true,\"vertex_count\":0,\"edges\":[]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.True(sut.IsDirected);
                Assert.AreEqual(0, sut.NumVertices);
                Assert.IsEmpty(sut.Edges);
            });
        }

        [Test]
        public void EmptyUndirectedGraph()
        {
            var text = "{\"directed\":false,\"vertex_count\":0,\"edges\":[]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.False(sut.IsDirected);
                Assert.AreEqual(0, sut.NumVertices);
                Assert.IsEmpty(sut.Edges);
            });
        }

        [Test]
        public void VertexOnlyGraph()
        {
            var text = "{\"directed\":true,\"vertex_count\":3,\"edges\":[]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.True(sut.IsDirected);
                Assert.AreEqual(3, sut.NumVertices);
                Assert.IsEmpty(sut.Edges);
            });
        }

        [Test]
        public void K2Graph()
        {
            var text = "{\"directed\":true,\"vertex_count\":2,\"edges\":[[0,1],[1,0]]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.True(sut.IsDirected);
                Assert.AreEqual(2, sut.NumVertices);
                Assert.AreEqual(new[] { Tuple.Create(0, 1), Tuple.Create(1, 0) }, sut.Edges);
            });
        }

        [Test]
        public void InvalidJson([ValueSource("InvalidJsonData")] string text)
        {
            Assert.Throws<InvalidDataException>(
                () => { SimpleGraph.FromJson(new StringReader(text)); });
        }

        public static IEnumerable<string> InvalidJsonData
        {
            get
            {
                return new[] {
                    "",
                    "{\"vertex_count\":0,\"edges\":[]}",
                    "{\"directed\":true,\"edges\":[]}",
                    "{\"directed\":true,\"vertex_count\":0}",
                    "{\"directed\":true,\"vertex_count\":0,\"edges\":null}",
                    "{\"directed\":true,\"vertex_count\":0,\"edges\":[null]}",
                    "{\"directed\":true,\"vertex_count\":-4,\"edges\":[]}",
                    "{\"directed\":true,\"vertex_count\":0,\"edges\":[[1]]}",
                    "{\"directed\":true,\"vertex_count\":0,\"edges\":[[1,2,3]]}",
                    "{\"directed\":true,\"vertex_count\":1,\"edges\":[[0,0]]}",
                    "{\"directed\":true,\"vertex_count\":2,\"edges\":[[0,1],[0,1]]}",
                    "{\"directed\":null,\"vertex_count\":0,\"edges\":[]}",
                    "{\"directed\":true,\"vertex_count\":null,\"edges\":[]}"
                };
            }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { SimpleGraph.FromJson(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new SimpleGraph(true).ToJson(null); });
        }

        void RoundTripTest(string text, Action<SimpleGraph> assertionCallback)
        {
            SimpleGraph sut;

            using (var buffer = new StringReader(text))
                sut = SimpleGraph.FromJson(buffer);

            assertionCallback(sut);

            using (var buffer = new StringWriter())
            {
                sut.ToJson(buffer);
                Assert.AreEqual(text, buffer.ToString());
            }
        }
    }
}