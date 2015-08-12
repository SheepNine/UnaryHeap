using System;
using System.Collections.Generic;
using System.IO;
using UnaryHeap.Utilities.Core;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class AnnotatedGraphIOTests
    {
        [Fact]
        public void EmptyDirectedGraph()
        {
            var text =
                "{\"structure\":{\"directed\":true,\"vertex_count\":0,\"edges\":[]}," +
                "\"graph_metadata\":{},\"vertex_metadata\":[],\"edge_metadata\":[]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.True(sut.IsDirected);
                Assert.Equal(0, sut.NumVertices);
                Assert.Empty(sut.Edges);
            });
        }

        [Fact]
        public void NoMetadata()
        {
            var text =
                "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]}," +
                "\"graph_metadata\":{},\"vertex_metadata\":[{},{},{}],\"edge_metadata\":[{},{}]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.False(sut.IsDirected);
                Assert.Equal(3, sut.NumVertices);
                Assert.Equal(new[] { Tuple.Create(0, 1), Tuple.Create(0, 2) }, sut.Edges);
            });
        }

        [Fact]
        public void Metadata()
        {
            var text =
                "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]}," +
                "\"graph_metadata\":{\"g\":\"reat\"}," +
                "\"vertex_metadata\":[{\"v\":\"0\"},{\"v\":\"1\"},{\"v\":\"2\"}]," +
                "\"edge_metadata\":[{\"e\":\"01\"},{\"e\":null}]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.False(sut.IsDirected);
                Assert.Equal(3, sut.NumVertices);
                Assert.Equal(new[] { Tuple.Create(0, 1), Tuple.Create(0, 2) }, sut.Edges);

                Assert.Equal("reat", sut.GetGraphMetadatum("g"));
                Assert.Equal("0", sut.GetVertexMetadatum(0, "v"));
                Assert.Equal("1", sut.GetVertexMetadatum(1, "v"));
                Assert.Equal("2", sut.GetVertexMetadatum(2, "v"));
                Assert.Equal("01", sut.GetEdgeMetadatum(0, 1, "e"));
                Assert.Null(sut.GetEdgeMetadatum(0, 2, "e", "default_value"));
            });
        }

        public static IEnumerable<object[]> InvalidJsonData
        {
            get
            {
                return new[] {
                    new object[] { "" },
                    new object[] { "{\"graph_metadata\":{},\"vertex_metadata\":[{},{},{}],\"edge_metadata\":[{},{}]}" },
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"vertex_metadata\":[{},{},{}],\"edge_metadata\":[{},{}]}" },
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{},\"edge_metadata\":[{},{}]}" },
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{},\"vertex_metadata\":[{},{},{}],}" },
                    new object[] { "{\"structure\":null,\"graph_metadata\":{},\"vertex_metadata\":[{},{},{}],\"edge_metadata\":[{},{}]}" },
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":null,\"vertex_metadata\":[{},{},{}],\"edge_metadata\":[{},{}]}" },
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{},\"vertex_metadata\":null,\"edge_metadata\":[{},{}]}" },
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{},\"vertex_metadata\":[{},{},{}],\"edge_metadata\":null}" },                    
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{},\"vertex_metadata\":[{},null,{}],\"edge_metadata\":[{},{}]}" },
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{},\"vertex_metadata\":[{},{},{}],\"edge_metadata\":[{},null]}" },                    
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{},\"vertex_metadata\":[{},{}],\"edge_metadata\":[{},{}]}" },
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{},\"vertex_metadata\":[{},{},{}],\"edge_metadata\":[{},{},{}]}" },
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{\"a\":true},\"vertex_metadata\":[{},{},{}],\"edge_metadata\":[{},{},{}]}" },
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{},\"vertex_metadata\":[{},{\"a\":true},{}],\"edge_metadata\":[{},{},{}]}" },
                    new object[] { "{\"structure\":{\"directed\":false,\"vertex_count\":3,\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{},\"vertex_metadata\":[{},{},{}],\"edge_metadata\":[{\"a\":true},{},{}]}" },
                };
            }
        }

        [Theory]
        [MemberData("InvalidJsonData")]
        public void InvalidJson(string text)
        {
            Assert.Throws<InvalidDataException>(() => { AnnotatedGraph.FromJson(new StringReader(text)); });
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("input", () => { AnnotatedGraph.FromJson(null); });
            Assert.Throws<ArgumentNullException>("output", () => { new AnnotatedGraph(true).ToJson(null); });
        }

        private void RoundTripTest(string text, Action<AnnotatedGraph> assertionCallback)
        {
            AnnotatedGraph sut;

            using (var buffer = new StringReader(text))
                sut = AnnotatedGraph.FromJson(buffer);

            assertionCallback(sut);

            using (var buffer = new StringWriter())
            {
                sut.ToJson(buffer);
                Assert.Equal(text, buffer.ToString());
            }
        }
    }
}
