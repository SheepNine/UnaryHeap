#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace UnaryHeap.Utilities
{
    partial class Graph2D
    {
        #region Constants

        const string VertexLocationMetadataKey = "xy";

        #endregion


        #region Member Variables

        AnnotatedGraph structure;
        List<Point2D> locationFromVertex;
        SortedDictionary<Point2D, int> vertexFromLocation;

        #endregion


        #region Constructor

        public Graph2D(bool directed)
        {
            structure = new AnnotatedGraph(directed);
            locationFromVertex = new List<Point2D>();
            vertexFromLocation = new SortedDictionary<Point2D, int>(new Point2DComparer());
        }

        #endregion


        #region Properties

        public bool IsDirected
        {
            get { return structure.IsDirected; }
        }

        public int NumVertices
        {
            get { return structure.NumVertices; }
        }

        public IEnumerable<Point2D> Vertices
        {
            get { return locationFromVertex; }
        }

        public IEnumerable<Tuple<Point2D, Point2D>> Edges
        {
            get { return structure.Edges.Select(e => Tuple.Create(locationFromVertex[e.Item1], locationFromVertex[e.Item2])); }
        }

        #endregion


        #region Public Methods

        #region Graph Methods

        public int AddVertex(Point2D location)
        {
            if (null == location)
                throw new ArgumentNullException();

            var result = structure.AddVertex();
            structure.SetVertexMetadatum(result, VertexLocationMetadataKey, location.ToString());

            locationFromVertex.Add(location);
            vertexFromLocation.Add(location, result);

            return result;
        }

        public void MoveVertex(Point2D origin, Point2D destination)
        {
            if (null == origin)
                throw new ArgumentNullException();
            if (null == destination)
                throw new ArgumentNullException();
            if (origin.Equals(destination))
                return;

            var originIndex = IndexOf(origin);
            if (vertexFromLocation.ContainsKey(destination))
                throw new InvalidOperationException();

            locationFromVertex[originIndex] = destination;
            vertexFromLocation.Remove(origin);
            vertexFromLocation.Add(destination, originIndex);
            structure.SetVertexMetadatum(originIndex, VertexLocationMetadataKey, destination.ToString());
        }

        public void RemoveVertex(Point2D vertex)
        {
            var index = IndexOf(vertex);

            structure.RemoveVertex(index);

            vertexFromLocation.Remove(vertex);
            locationFromVertex.RemoveAt(index);
        }

        public void AddEdge(Point2D from, Point2D to)
        {
            var fromIndex = IndexOf(from);
            var toIndex = IndexOf(to);

            structure.AddEdge(fromIndex, toIndex);
        }

        public void RemoveEdge(Point2D from, Point2D to)
        {
            var fromIndex = IndexOf(from);
            var toIndex = IndexOf(to);

            structure.RemoveEdge(fromIndex, toIndex);
        }

        public bool HasEdge(Point2D from, Point2D to)
        {
            var fromIndex = IndexOf(from);
            var toIndex = IndexOf(to);

            return structure.HasEdge(fromIndex, toIndex);
        }

        public Point2D[] GetNeighbours(Point2D from)
        {
            var fromIndex = IndexOf(from);
            return structure.GetNeighbours(fromIndex).Select(i => locationFromVertex[i]).ToArray();
        }

        public Graph2D Clone()
        {
            var result = new Graph2D(IsDirected);
            result.structure = structure.Clone();

            return result;
        }

        #endregion


        #region Metadata Methods

        public void UnsetGraphMetadatum(string key)
        {
            structure.UnsetGraphMetadatum(key);
        }

        public void SetGraphMetadatum(string key, string value)
        {
            structure.SetGraphMetadatum(key, value);
        }

        public string GetGraphMetadatum(string key, string defaultValue = null)
        {
            ThrowIfReservedKey(key);
            return structure.GetGraphMetadatum(key, defaultValue);
        }

        public void UnsetVertexMetadatum(Point2D vertex, string key)
        {
            ThrowIfReservedKey(key);
            structure.UnsetVertexMetadatum(IndexOf(vertex), key);
        }

        public void SetVertexMetadatum(Point2D vertex, string key, string value)
        {
            ThrowIfReservedKey(key);
            structure.SetVertexMetadatum(IndexOf(vertex), key, value);
        }

        public string GetVertexMetadatum(Point2D vertex, string key, string defaultValue = null)
        {
            return structure.GetVertexMetadatum(IndexOf(vertex), key, defaultValue);
        }

        public void UnsetEdgeMetadatum(Point2D from, Point2D to, string key)
        {
            structure.UnsetEdgeMetadatum(IndexOf(from), IndexOf(to), key);
        }

        public void SetEdgeMetadatum(Point2D from, Point2D to, string key, string value)
        {
            structure.SetEdgeMetadatum(IndexOf(from), IndexOf(to), key, value);
        }

        public string GetEdgeMetadatum(Point2D from, Point2D to, string key, string defaultValue = null)
        {
            return structure.GetEdgeMetadatum(IndexOf(from), IndexOf(to), key, defaultValue);
        }

        #endregion

        #endregion


        #region Helper Methods

        int IndexOf(Point2D vertex)
        {
            if (null == vertex)
                throw new ArgumentNullException();

            int result;
            if (false == vertexFromLocation.TryGetValue(vertex, out result))
                throw new InvalidOperationException();

            return result;
        }

        static void ThrowIfReservedKey(string key)
        {
            if (VertexLocationMetadataKey == key)
                throw new ArgumentException(string.Format("Vertex metadatum key '{0}' is reserved.", VertexLocationMetadataKey));
        }

        #endregion
    }

    partial class Graph2D
    {
        public void ToJson(TextWriter output)
        {
            structure.ToJson(output);
        }

        public void FromJson(TextReader input)
        {
            throw new NotImplementedException();
        }
    }
}

#endif