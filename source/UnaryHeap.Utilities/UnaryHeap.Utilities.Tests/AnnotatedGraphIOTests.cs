using System;
using System.Collections.Generic;
using System.IO;
using UnaryHeap.Utilities.Core;
using NUnit.Framework;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class AnnotatedGraphIOTests
    {
        [Test]
        public void EmptyDirectedGraph()
        {
            var text =
                "{\"structure\":{\"directed\":true,\"vertex_count\":0,\"edges\":[]}," +
                "\"graph_metadata\":{},\"vertex_metadata\":[],\"edge_metadata\":[]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.True(sut.IsDirected);
                Assert.AreEqual(0, sut.NumVertices);
                Assert.IsEmpty(sut.Edges);
            });
        }

        [Test]
        public void NoMetadata()
        {
            var text =
                "{\"structure\":{\"directed\":false,\"vertex_count\":3," +
                "\"edges\":[[0,1],[0,2]]},\"graph_metadata\":{}," + 
                "\"vertex_metadata\":[{},{},{}],\"edge_metadata\":[{},{}]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.False(sut.IsDirected);
                Assert.AreEqual(3, sut.NumVertices);
                Assert.AreEqual(new[] { Tuple.Create(0, 1), Tuple.Create(0, 2) }, sut.Edges);
            });
        }

        [Test]
        public void Metadata()
        {
            var text =
                "{\"structure\":{\"directed\":false,\"vertex_count\":3," +
                "\"edges\":[[0,1],[0,2]]}," +
                "\"graph_metadata\":{\"g\":\"reat\"}," +
                "\"vertex_metadata\":[{\"v\":\"0\"},{\"v\":\"1\"},{\"v\":\"2\"}]," +
                "\"edge_metadata\":[{\"e\":\"01\"},{\"e\":null}]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.False(sut.IsDirected);
                Assert.AreEqual(3, sut.NumVertices);
                Assert.AreEqual(new[] { Tuple.Create(0, 1), Tuple.Create(0, 2) }, sut.Edges);

                Assert.AreEqual("reat", sut.GetGraphMetadatum("g"));
                Assert.AreEqual("0", sut.GetVertexMetadatum(0, "v"));
                Assert.AreEqual("1", sut.GetVertexMetadatum(1, "v"));
                Assert.AreEqual("2", sut.GetVertexMetadatum(2, "v"));
                Assert.AreEqual("01", sut.GetEdgeMetadatum(0, 1, "e"));
                Assert.Null(sut.GetEdgeMetadatum(0, 2, "e", "default_value"));
            });
        }

        public static IEnumerable<string> InvalidJsonData
        {
            get
            {
                return new[] {
                    "",

                    @"{""graph_metadata"":{},
                    ""vertex_metadata"":[{},{},{}],
                    ""edge_metadata"":[{},{}]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""vertex_metadata"":[{},{},{}],
                    ""edge_metadata"":[{},{}]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":{},
                    ""edge_metadata"":[{},{}]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":{},
                    ""vertex_metadata"":[{},{},{}],}",

                    @"{""structure"":null,
                    ""graph_metadata"":{},
                    ""vertex_metadata"":[{},{},{}],
                    ""edge_metadata"":[{},{}]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":null,
                    ""vertex_metadata"":[{},{},{}],
                    ""edge_metadata"":[{},{}]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":{},
                    ""vertex_metadata"":null,
                    ""edge_metadata"":[{},{}]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":{},
                    ""vertex_metadata"":[{},{},{}],
                    ""edge_metadata"":null}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":{},
                    ""vertex_metadata"":[{},null,{}],
                    ""edge_metadata"":[{},{}]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":{},
                    ""vertex_metadata"":[{},{},{}],
                    ""edge_metadata"":[{},null]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":{},
                    ""vertex_metadata"":[{},{}],
                    ""edge_metadata"":[{},{}]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":{},
                    ""vertex_metadata"":[{},{},{}],
                    ""edge_metadata"":[{},{},{}]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":{""a"":true},
                    ""vertex_metadata"":[{},{},{}],
                    ""edge_metadata"":[{},{},{}]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":{},
                    ""vertex_metadata"":[{},{""a"":true},{}],
                    ""edge_metadata"":[{},{},{}]}",

                    @"{""structure"":{""directed"":false,""vertex_count"":3,
                    ""edges"":[[0,1],[0,2]]},
                    ""graph_metadata"":{},
                    ""vertex_metadata"":[{},{},{}],
                    ""edge_metadata"":[{""a"":true},{},{}]}"
                };
            }
        }

        [Test]
        public void InvalidJson([ValueSource("InvalidJsonData")]string text)
        {
            Assert.Throws<InvalidDataException>(
                () => { AnnotatedGraph.FromJson(new StringReader(text)); });
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { AnnotatedGraph.FromJson(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new AnnotatedGraph(true).ToJson(null); });
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
                Assert.AreEqual(text, buffer.ToString());
            }
        }
    }
}
