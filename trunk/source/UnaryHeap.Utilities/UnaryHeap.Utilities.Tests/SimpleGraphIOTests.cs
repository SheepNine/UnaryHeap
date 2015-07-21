#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.IO;
using UnaryHeap.Utilities;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class SimpleGraphIOTests
    {
        [Fact]
        public void EmptyDirectedGraph()
        {
            var text = "{\"directed\":true,\"vertex_count\":0,\"edges\":[]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.True(sut.IsDirected);
                Assert.Equal(0, sut.NumVertices);
                Assert.Empty(sut.Edges);
            });
        }

        [Fact]
        public void EmptyUndirectedGraph()
        {
            var text = "{\"directed\":false,\"vertex_count\":0,\"edges\":[]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.False(sut.IsDirected);
                Assert.Equal(0, sut.NumVertices);
                Assert.Empty(sut.Edges);
            });
        }

        [Fact]
        public void VertexOnlyGraph()
        {
            var text = "{\"directed\":true,\"vertex_count\":3,\"edges\":[]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.True(sut.IsDirected);
                Assert.Equal(3, sut.NumVertices);
                Assert.Empty(sut.Edges);
            });
        }

        [Fact]
        public void K2Graph()
        {
            var text = "{\"directed\":true,\"vertex_count\":2,\"edges\":[[0,1],[1,0]]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.True(sut.IsDirected);
                Assert.Equal(2, sut.NumVertices);
                Assert.Equal(new[] { Tuple.Create(0, 1), Tuple.Create(1, 0) }, sut.Edges);
            });
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
                Assert.Equal(text, buffer.ToString());
            }
        }
    }
}

#endif