using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace UnaryHeap.Graph
{
    public partial class SimpleGraph
    {
        /// <summary>
        /// Writes a JSON object representation of the current SimpleGraph instance.
        /// </summary>
        /// <param name="output">The TextWriter to which the JSON is written.</param>
        /// <exception cref="System.ArgumentNullException">output is null.</exception>
        public void ToJson(TextWriter output)
        {
            if (null == output)
                throw new ArgumentNullException(nameof(output));

            using (var writer = new JsonTextWriter(output))
                ToJson(writer);
        }

        internal void ToJson(JsonTextWriter writer)
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

        /// <summary>
        /// Constructs a new SimpleGraph object from a JSON object representation.
        /// </summary>
        /// <param name="input">The TextReader from which the JSON is read.</param>
        /// <returns>The UnaryHeap.Utilities.SimpleGraph specified by the JSON object.</returns>
        /// <exception cref="System.ArgumentNullException">input is null.</exception>
        /// <exception cref="System.IO.InvalidDataException">
        /// input contains an incorrectly-formatted JSON
        /// object, or there are errors in the JSON object data.</exception>
        public static SimpleGraph FromJson(TextReader input)
        {
            if (null == input)
                throw new ArgumentNullException(nameof(input));

            try
            {
                using (var reader = new JsonTextReader(input))
                {
                    var data = new JsonSerializer().Deserialize<SimpleGraphPoco>(reader);

                    if (null == data)
                        throw new InvalidDataException("Input JSON empty.");

                    return data.Convert();
                }
            }
            catch (JsonSerializationException ex)
            {
                throw new InvalidDataException(
                    "Input JSON data is not correctly formatted.", ex);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidDataException("Input JSON data contains errors", ex);
            }
        }

        internal class SimpleGraphPoco
        {
            [JsonRequired]
            public bool directed { get; set; }
            [JsonRequired]
            public int vertex_count { get; set; }
            [JsonRequired]
            public int[][] edges { get; set; }

            public SimpleGraph Convert()
            {
                ThrowIfDataNotValid();

                var result = new SimpleGraph(directed);

                for (int i = 0; i < vertex_count; i++)
                    result.AddVertex();

                foreach (var edge in edges)
                    result.AddEdge(edge[0], edge[1]);

                return result;
            }

            void ThrowIfDataNotValid()
            {
                if (0 > vertex_count)
                    throw new InvalidDataException("Vertex count is negative.");

                foreach (var edge in edges)
                {
                    if (null == edge)
                        throw new InvalidDataException("Null value in edges.");
                    if (2 != edge.Length)
                        throw new InvalidDataException("Invalid edge.");
                }
            }
        }
    }
}