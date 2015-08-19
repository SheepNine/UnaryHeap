using System;
using System.Collections.Generic;
using System.IO;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class Graph2DIOTests
    {
        [Fact]
        public void EmptyDirectedGraph()
        {
            var text =
                "{\"structure\":{\"directed\":true,\"vertex_count\":2,\"edges\":[]}," +
                "\"graph_metadata\":{},\"vertex_metadata\":[{\"xy\":\"-1,-1\"},{\"xy\":\"1,1\"}]," +
                "\"edge_metadata\":[]}";

            RoundTripTest(text, (sut) =>
            {
                Assert.True(sut.IsDirected);
                Assert.Equal(2, sut.NumVertices);
                Assert.True(sut.HasVertex(new Point2D(-1, -1)));
                Assert.Equal(new[] { new Point2D(-1, -1), new Point2D(1, 1) }, sut.Vertices);
            });
        }

        public static IEnumerable<object[]> InvalidJsonData
        {
            get
            {
                return new[] {
                    new object[] { "" },
                    new object[] {
@"{""structure"":{""directed"":true,""vertex_count"":2,""edges"":[]},
""graph_metadata"":{},
""vertex_metadata"":[{},{""xy"":""1,1""}],
""edge_metadata"":[]}"
                    },

                    new object[] {
@"{""structure"":{""directed"":true,""vertex_count"":2,""edges"":[]},
""graph_metadata"":{},
""vertex_metadata"":[{""xy"":null},{""xy"":""1,1""}],
""edge_metadata"":[]}"
                    },

                    new object[] {
@"{""structure"":{""directed"":true,""vertex_count"":2,""edges"":[]},
""graph_metadata"":{},
""vertex_metadata"":[{""xy"":""bacon""},{""xy"":""1,1""}],
""edge_metadata"":[]}" },

                    new object[] {
@"{""structure"":{""directed"":true,""vertex_count"":2,""edges"":[]},
""graph_metadata"":{},
""vertex_metadata"":[{""xy"":""1,1""},{""xy"":""1,1""}],
""edge_metadata"":[]}"
                    },
                };
            }
        }

        [Theory]
        [MemberData("InvalidJsonData")]
        public void InvalidJson(string text)
        {
            Assert.Throws<InvalidDataException>(
                () => { Graph2D.FromJson(new StringReader(text)); });
        }

        private void RoundTripTest(string text, Action<Graph2D> assertionCallback)
        {
            Graph2D sut;

            using (var buffer = new StringReader(text))
                sut = Graph2D.FromJson(buffer);

            assertionCallback(sut);

            using (var buffer = new StringWriter())
            {
                sut.ToJson(buffer);
                Assert.Equal(text, buffer.ToString());
            }
        }
    }
}
