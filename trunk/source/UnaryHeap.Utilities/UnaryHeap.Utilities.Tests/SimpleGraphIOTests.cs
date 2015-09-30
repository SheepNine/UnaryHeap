using System;
using System.Collections.Generic;
using System.IO;
using UnaryHeap.Utilities.Core;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class SimpleGraphIOTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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

        [Theory]
        [MemberData("InvalidJsonData")]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void InvalidJson(string text)
        {
            Assert.Throws<InvalidDataException>(
                () => { SimpleGraph.FromJson(new StringReader(text)); });
        }

        public static IEnumerable<object[]> InvalidJsonData
        {
            get
            {
                return new[] {
                    new object[] {
                        "" },
                    new object[] {
                        "{\"vertex_count\":0,\"edges\":[]}" },
                    new object[] {
                        "{\"directed\":true,\"edges\":[]}" },
                    new object[] {
                        "{\"directed\":true,\"vertex_count\":0}" },
                    new object[] {
                        "{\"directed\":true,\"vertex_count\":0,\"edges\":null}" },
                    new object[] {
                        "{\"directed\":true,\"vertex_count\":0,\"edges\":[null]}" },
                    new object[] {
                        "{\"directed\":true,\"vertex_count\":-4,\"edges\":[]}" },
                    new object[] {
                        "{\"directed\":true,\"vertex_count\":0,\"edges\":[[1]]}" },
                    new object[] {
                        "{\"directed\":true,\"vertex_count\":0,\"edges\":[[1,2,3]]}" },
                    new object[] {
                        "{\"directed\":true,\"vertex_count\":1,\"edges\":[[0,0]]}" },
                    new object[] {
                        "{\"directed\":true,\"vertex_count\":2,\"edges\":[[0,1],[0,1]]}" },
                    new object[] {
                        "{\"directed\":null,\"vertex_count\":0,\"edges\":[]}" },
                    new object[] {
                        "{\"directed\":true,\"vertex_count\":null,\"edges\":[]}" },
                };
            }
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("input",
                () => { SimpleGraph.FromJson(null); });
            Assert.Throws<ArgumentNullException>("output",
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
                Assert.Equal(text, buffer.ToString());
            }
        }
    }
}