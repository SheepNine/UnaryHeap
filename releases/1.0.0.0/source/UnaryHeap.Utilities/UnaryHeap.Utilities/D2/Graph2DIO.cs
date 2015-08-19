using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Core;

namespace UnaryHeap.Utilities.D2
{
    partial class Graph2D
    {
        /// <summary>
        /// Writes a JSON object representation of the current UnaryHeap.Utilities.Graph2D instance.
        /// </summary>
        /// <param name="output">The TextWriter to which the JSON is written.</param>
        /// <exception cref="System.ArgumentNullException">output is null.</exception>
        public void ToJson(TextWriter output)
        {
            structure.ToJson(output);
        }

        /// <summary>
        /// Constructs a new UnaryHeap.Utilities.Graph2D object from a JSON object representation.
        /// </summary>
        /// <param name="input">The TextReader from which the JSON is read.</param>
        /// <returns>The UnaryHeap.Utilities.Graph2D specified by the JSON object.</returns>
        /// <exception cref="System.ArgumentNullException">input is null.</exception>
        /// <exception cref="System.IO.InvalidDataException">input contains an incorrectly-formatted JSON
        /// object, or there are errors in the JSON object data.</exception>
        public static Graph2D FromJson(TextReader input)
        {
            var structure = AnnotatedGraph.FromJson(input);

            var result = new Graph2D(structure.IsDirected);
            result.structure = structure;
            result.locationFromVertex = new List<Point2D>();
            result.vertexFromLocation = new SortedDictionary<Point2D, int>(new Point2DComparer());

            // --- Initialize vertex coordinates ---
            foreach (var i in Enumerable.Range(0, result.NumVertices))
            {
                var xyMeta = structure.GetVertexMetadatum(i, VertexLocationMetadataKey, null);
                if (null == xyMeta)
                    throw new InvalidDataException("Vertex coordinates undefined or null.");

                Point2D coordinates;
                try
                {
                    coordinates = Point2D.Parse(xyMeta);
                }
                catch (FormatException)
                {
                    throw new InvalidDataException("Invalid vertex coordinates.");
                }

                if (result.vertexFromLocation.ContainsKey(coordinates))
                    throw new InvalidDataException("Duplicate vertex coordinates.");

                result.locationFromVertex.Add(coordinates);
                result.vertexFromLocation.Add(coordinates, i);
            }

            return result;
        }
    }
}
