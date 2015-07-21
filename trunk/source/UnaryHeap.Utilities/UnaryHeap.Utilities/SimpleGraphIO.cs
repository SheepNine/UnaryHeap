#if INCLUDE_WORK_IN_PROGRESS

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace UnaryHeap.Utilities
{
    public partial class SimpleGraph
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        public void ToJson(TextWriter output)
        {
            using (var writer = new JsonTextWriter(output))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("directed");
                writer.WriteValue(IsDirected);

                writer.WritePropertyName("vertex_count");
                writer.WriteValue(NumVertices);

                writer.WritePropertyName("edges");
                writer.WriteStartArray();
                
                foreach (var i in Enumerable.Range(0, NumVertices))
                    foreach (var j in adjacencies[i])
                    {
                        writer.WriteStartArray();
                        writer.WriteValue(i);
                        writer.WriteValue(j);
                        writer.WriteEndArray();
                    }

                writer.WriteEndArray();

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static SimpleGraph FromJson(TextReader input)
        {
            using (var reader = new JsonTextReader(input))
                return new JsonSerializer().Deserialize<SimpleGraphPoco>(reader).Convert();
        }

        class SimpleGraphPoco
        {
            public bool directed { get; set; }
            public int vertex_count { get; set; }
            public int[][] edges { get; set; }

            public SimpleGraph Convert()
            {
                var result = new SimpleGraph(directed);

                foreach (var i in Enumerable.Range(0, vertex_count))
                    result.AddVertex();

                foreach (var edge in edges)
                    result.AddEdge(edge[0], edge[1]);

                return result;
            }
        }
    }
}

#endif