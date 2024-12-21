using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnaryHeap.Graph
{
    partial class AnnotatedGraph
    {
        /// <summary>
        /// Writes a JSON object representation of the current AnnotatedGraph instance.
        /// </summary>
        /// <param name="output">The TextWriter to which the JSON is written.</param>
        /// <exception cref="System.ArgumentNullException">output is null.</exception>
        public void ToJson(TextWriter output)
        {
            ArgumentNullException.ThrowIfNull(output);

            using (var writer = new JsonTextWriter(output))
                ToJson(writer);
        }

        internal void ToJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("structure");
            structure.ToJson(writer);

            writer.WritePropertyName("graph_metadata");
            WriteMetadataCollection(writer, graphMetadata);

            writer.WritePropertyName("vertex_metadata");
            writer.WriteStartArray();
            foreach (var vertexMetadatum in vertexMetadata)
                WriteMetadataCollection(writer, vertexMetadatum);
            writer.WriteEndArray();

            writer.WritePropertyName("edge_metadata");
            writer.WriteStartArray();
            foreach (var edge in structure.Edges)
                WriteMetadataCollection(writer, edgeMetadata[EdgeKey(edge.Item1, edge.Item2)]);
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        static void WriteMetadataCollection(
            JsonTextWriter writer, IDictionary<string, string> metadata)
        {
            writer.WriteStartObject();

            foreach (var metadatum in metadata)
            {
                writer.WritePropertyName(metadatum.Key);
                writer.WriteValue(metadatum.Value);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Constructs a new AnnotatedGraph object from a JSON object representation.
        /// </summary>
        /// <param name="input">The TextReader from which the JSON is read.</param>
        /// <returns>The AnnotatedGraph specified by the JSON object.</returns>
        /// <exception cref="System.ArgumentNullException">input is null.</exception>
        /// <exception cref="System.IO.InvalidDataException">
        /// input contains an incorrectly-formatted JSON object, or there are errors
        /// in the JSON object data.</exception>
        public static AnnotatedGraph FromJson(TextReader input)
        {
            ArgumentNullException.ThrowIfNull(input);

            try
            {
                using (var reader = new JsonTextReader(input))
                {
                    var data = new JsonSerializer().Deserialize<AnnotatedGraphPoco>(reader);

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
        }

        sealed class AnnotatedGraphPoco
        {
            [JsonRequired]
            public SimpleGraph.SimpleGraphPoco structure { get; set; }
            [JsonRequired]
            public SortedDictionary<string, string> graph_metadata { get; set; }
            [JsonRequired]
            public List<SortedDictionary<string, string>> vertex_metadata { get; set; }
            [JsonRequired]
            public List<SortedDictionary<string, string>> edge_metadata { get; set; }

            public AnnotatedGraph Convert()
            {
                var result = new AnnotatedGraph(structure.directed);
                result.structure = structure.Convert();

                ThrowIfInvalid();

                result.graphMetadata = graph_metadata;
                result.vertexMetadata = vertex_metadata;
                result.edgeMetadata =
                    new SortedDictionary<ulong, SortedDictionary<string, string>>(
                        Enumerable.Range(0, structure.edges.Length).ToDictionary(
                        i => result.EdgeKey(structure.edges[i][0], structure.edges[i][1]),
                        i => edge_metadata[i]));

                return result;
            }

            void ThrowIfInvalid()
            {
                if (vertex_metadata.Any(m => null == m))
                    throw new InvalidDataException("Found null vertex metadata.");
                if (vertex_metadata.Count != structure.vertex_count)
                    throw new InvalidDataException(
                        "Vertex metadata length does not match number of vertices.");

                if (edge_metadata.Any(e => null == e))
                    throw new InvalidDataException("Found null edge metadata.");
                if (edge_metadata.Count != structure.edges.Length)
                    throw new InvalidDataException(
                        "Edge metadata length does not match number of edges.");
            }
        }
    }
}
