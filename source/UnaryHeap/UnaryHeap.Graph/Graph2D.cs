﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.DataType;

namespace UnaryHeap.Graph
{
    /// <summary>
    /// Represents an extension of the AnnotatedGraph class
    /// that assigns 2D coordinates to the vertices.
    /// </summary>
    public partial class Graph2D
    {
        #region Constants

        const string VertexLocationMetadataKey = "xy";

        /// <summary>
        /// Checks if the given metadatum key is reserved by the Graph2D class.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if metadata with the associated key is managed by the Graph2D class;
        /// false otherwise.</returns>
        public static bool IsReservedMetadataKey(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            return VertexLocationMetadataKey == key;
        }

        #endregion


        #region Member Variables

        AnnotatedGraph structure;
        List<Point2D> locationFromVertex;
        SortedDictionary<Point2D, int> vertexFromLocation;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Graph2D class.
        /// </summary>
        /// <param name="directed">Whether or not the resulting graph is directed.</param>
        public Graph2D(bool directed)
        {
            structure = new AnnotatedGraph(directed);
            locationFromVertex = new List<Point2D>();
            vertexFromLocation = new SortedDictionary<Point2D, int>(new Point2DComparer());
        }

        #endregion


        #region Properties

        /// <summary>
        /// Indicates whether the current Graph2D instance is a directed graph.
        /// </summary>
        public bool IsDirected
        {
            get { return structure.IsDirected; }
        }

        /// <summary>
        /// Gets the number of vertices in the current Graph2D instance.
        /// </summary>
        public int NumVertices
        {
            get { return structure.NumVertices; }
        }

        /// <summary>
        /// Gets the locations of the vertices in the current Graph2D instance.
        /// </summary>
        public IEnumerable<Point2D> Vertices
        {
            get { return locationFromVertex; }
        }

        /// <summary>
        /// Gets [start, end] vertex location tuples for the edges in the
        /// current Graph2D instance.
        /// </summary>
        /// <remarks>
        /// For undirected graphs, each edge occurs only once in the resulting enumeration.
        /// </remarks>
        public IEnumerable<Tuple<Point2D, Point2D>> Edges
        {
            get { return structure.Edges.Select(GetPointsForVertices); }
        }

        Tuple<Point2D, Point2D> GetPointsForVertices(Tuple<int, int> e)
        {
            return Tuple.Create(locationFromVertex[e.Item1], locationFromVertex[e.Item2]);
        }

        #endregion


        #region Public Methods

        #region Graph Methods

        /// <summary>
        /// Adds a new vertex to the current Graph2D instance.
        /// </summary>
        /// <param name="coordinates">The coordinates of the new vertex.</param>
        /// <exception cref="System.ArgumentNullException">coordinates is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// There is already a vertex at the specified coordinates.</exception>
        public void AddVertex(Point2D coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);
            if (vertexFromLocation.ContainsKey(coordinates))
                throw new ArgumentException(
                    "Graph already contains a vertex at the coordinates specified.",
                    nameof(coordinates));

            var index = structure.AddVertex();
            structure.SetVertexMetadatum(
                index, VertexLocationMetadataKey, coordinates.ToString());

            locationFromVertex.Add(coordinates);
            vertexFromLocation.Add(coordinates, index);
        }

        /// <summary>
        /// Determines whether the graph contains a vertex at the coordinates specified.
        /// </summary>
        /// <param name="coordinates">The coordinates to query.</param>
        /// <returns>Whether the graph contains a vertex at the coordinates specified.</returns>
        /// <exception cref="System.ArgumentNullException">coordinates is null.</exception>
        public bool HasVertex(Point2D coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);

            return vertexFromLocation.ContainsKey(coordinates);
        }

        /// <summary>
        /// Changes the coordinates of a vertex.
        /// </summary>
        /// <param name="origin">The coordinates of the vertex to change.</param>
        /// <param name="destination">The coordinates to apply to the selected vertex.</param>
        /// <exception cref="System.ArgumentNullException">
        /// oridin or destination are null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The graph does not contain a vertex at origin,
        /// or it contains a vertex at destination.</exception>
        public void MoveVertex(Point2D origin, Point2D destination)
        {
            ArgumentNullException.ThrowIfNull(origin);
            ArgumentNullException.ThrowIfNull(destination);

            if (origin.Equals(destination))
                return;

            var originIndex = IndexOf(origin, nameof(origin));

            if (vertexFromLocation.ContainsKey(destination))
                throw new ArgumentException(
                    "Graph already contains a vertex at the coordinates specified.",
                    nameof(destination));

            locationFromVertex[originIndex] = destination;
            vertexFromLocation.Remove(origin);
            vertexFromLocation.Add(destination, originIndex);
            structure.SetVertexMetadatum(
                originIndex, VertexLocationMetadataKey, destination.ToString());
        }

        /// <summary>
        /// Removes a vertex from the current Graph2D instance, as well as all
        /// edges incident to that vertex.
        /// </summary>
        /// <param name="vertex">The coordinates of the vertex to remove.</param>
        /// <exception cref="System.ArgumentNullException">vertex is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The graph does not contain the specified vertex.</exception>
        public void RemoveVertex(Point2D vertex)
        {
            ArgumentNullException.ThrowIfNull(vertex);

            var index = IndexOf(vertex, nameof(vertex));

            structure.RemoveVertex(index);
            vertexFromLocation = new SortedDictionary<Point2D, int>(
                vertexFromLocation.Where(l => l.Value != index).ToDictionary(
                    l => l.Key, l => l.Value > index ? l.Value - 1 : l.Value),
                    vertexFromLocation.Comparer);
            locationFromVertex.RemoveAt(index);
        }


        /// <summary>
        /// Removes several vertices from the current Graph2D, as well as all
        /// edges incident to that vertex. This method is much faster than calling
        /// RemoveVertex() multiple times.
        /// </summary>
        /// <param name="vertices">The indices of the vertices to remove.</param>
        /// <returns>An array containing the new vertex index of the vertices in this
        /// Graph2D, or -1 if that vertex was deleted.</returns>
        public void RemoveVertices(IEnumerable<Point2D> vertices)
        {
            var verticesToRemove = CollectVertices(vertices, nameof(vertices));
            var indicesToRemove = verticesToRemove.Select(v => vertexFromLocation[v]).ToList();

            var map = structure.RemoveVertices(indicesToRemove);

            locationFromVertex = Enumerable.Range(0, map.Length)
                .Where(i => -1 != map[i])
                .Select(i => locationFromVertex[i])
                .ToList();

            vertexFromLocation = RemappedIndices(map);
        }

        SortedSet<Point2D> CollectVertices(IEnumerable<Point2D> vertices, string paramName)
        {
            if (null == vertices)
                throw new ArgumentNullException(paramName);

            var result = new SortedSet<Point2D>(new Point2DComparer());

            foreach (var vertex in vertices)
            {
                if (null == vertex)
                    throw new ArgumentNullException(paramName);

                if (false == vertexFromLocation.ContainsKey(vertex))
                    throw new ArgumentException(
                        "The specified vertex is not present in the graph.", paramName);

                if (result.Contains(vertex))
                    throw new ArgumentException(
                        "Enumeration contains duplicate vertex.", paramName);

                result.Add(vertex);
            }

            return result;
        }

        SortedDictionary<Point2D, int> RemappedIndices(int[] map)
        {
            var result = new SortedDictionary<Point2D, int>(vertexFromLocation.Comparer);

            foreach (var entry in vertexFromLocation)
            {
                if (-1 != map[entry.Value])
                    result.Add(entry.Key, map[entry.Value]);
            }

            return result;
        }

        /// <summary>
        /// Adds a new edge to the current AnnotatedGraph instance.
        /// </summary>
        /// <param name="from">The coordinates of the source vertex.</param>
        /// <param name="to">The coordinates of the destination vertex.</param>
        /// <exception cref="System.ArgumentNullException">from or to are null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The graph does not contain a vertex at from or to.</exception>
        public void AddEdge(Point2D from, Point2D to)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);

            var fromIndex = IndexOf(from, nameof(from));
            var toIndex = IndexOf(to, nameof(to));

            structure.AddEdge(fromIndex, toIndex);
        }

        /// <summary>
        /// Remove an edge from the current AnnotatedGraph instance.
        /// </summary>
        /// <param name="from">The coordinates of the source vertex.</param>
        /// <param name="to">The coordinates of the destination vertex.</param>
        /// <exception cref="System.ArgumentNullException">from or to are null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The graph does not contain a vertex at from or to.</exception>
        public void RemoveEdge(Point2D from, Point2D to)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);

            var fromIndex = IndexOf(from, nameof(from));
            var toIndex = IndexOf(to, nameof(to));

            structure.RemoveEdge(fromIndex, toIndex);
        }

        /// <summary>
        /// Determines whether the current AnnotatedGraph instance has the specified edge.
        /// </summary>
        /// <param name="from">The coordinates of the source vertex.</param>
        /// <param name="to">The coordinates of the destination vertex.</param>
        /// <returns>True, if there is an edge with the given from/to indices;
        /// otherwise, False.</returns>
        /// <exception cref="System.ArgumentNullException">from or to are null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The graph does not contain a vertex at from or to.</exception>
        public bool HasEdge(Point2D from, Point2D to)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);

            var fromIndex = IndexOf(from, nameof(from));
            var toIndex = IndexOf(to, nameof(to));

            return structure.HasEdge(fromIndex, toIndex);
        }

        /// <summary>
        /// Determine which vertices are neighbours of the specified vertex.
        /// </summary>
        /// <param name="from">The coordinates of the vertex to query.</param>
        /// <returns>An array containing the coordinates of vertices connected to
        /// the specified vertex.</returns>
        /// <exception cref="System.ArgumentNullException">from is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The graph does not contain the specified vertex.</exception>
        public Point2D[] GetNeighbours(Point2D from)
        {
            ArgumentNullException.ThrowIfNull(from);

            var fromIndex = IndexOf(from, nameof(from));

            return structure.GetNeighbours(fromIndex)
                .Select(i => locationFromVertex[i]).ToArray();
        }

        /// <summary>
        /// Determine how many vertices are neighbours of the specified vertex.
        /// </summary>
        /// <param name="from">The coordinates of the vertex to query.</param>
        /// <returns>The number of vertices connected to
        /// the specified vertex.</returns>
        /// <exception cref="System.ArgumentNullException">from is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The graph does not contain the specified vertex.</exception>
        public int NumNeighbours(Point2D from)
        {
            ArgumentNullException.ThrowIfNull(from);

            var fromIndex = IndexOf(from, nameof(from));

            return structure.NumNeighbours(fromIndex);
        }

        /// <summary>
        /// Creates a copy of the current Graph2D object.
        /// </summary>
        /// <returns>A copy of the current Graph2D object.</returns>
        public Graph2D Clone()
        {
            var result = new Graph2D(IsDirected);
            result.structure = structure.Clone();
            result.vertexFromLocation = new SortedDictionary<Point2D, int>(
                vertexFromLocation, vertexFromLocation.Comparer);
            result.locationFromVertex = new List<Point2D>(locationFromVertex);

            return result;
        }

        #endregion


        #region Metadata Methods

        /// <summary>
        /// Removes a metadata entry (if present) from the graph.
        /// </summary>
        /// <param name="key">The key of the metadata entry to remove.</param>
        /// <exception cref="System.ArgumentNullException">key is null.</exception>
        public void UnsetGraphMetadatum(string key)
        {
            structure.UnsetGraphMetadatum(key);
        }

        /// <summary>
        /// Adds or updates the value of a metadata entry of the graph.
        /// </summary>
        /// <param name="key">The key of the metadata entry to set.</param>
        /// <param name="value">The value of the metadata entry to set.</param>
        /// <exception cref="System.ArgumentNullException">key is null.</exception>
        public void SetGraphMetadatum(string key, string value)
        {
            structure.SetGraphMetadatum(key, value);
        }

        /// <summary>
        /// Gets the value of a metadata entry of the graph.
        /// </summary>
        /// <param name="key">The key of the metadata entry to retrieve.</param>
        /// <param name="defaultValue">
        /// The value to return if the graph does not have a metadata entry
        /// with the given key.</param>
        /// <returns>The value of the metadata entry with the specified key, or defaultValue,
        /// if no entry with that key exists.</returns>
        /// <exception cref="System.ArgumentNullException">key is null.</exception>
        public string GetGraphMetadatum(string key, string defaultValue = null)
        {
            return structure.GetGraphMetadatum(key, defaultValue);
        }

        /// <summary>
        /// Gets all of the metadata entries of the graph.
        /// </summary>
        /// <returns>A dictionary of all the metadata entries of the graph.</returns>
        public IReadOnlyDictionary<string, string> GraphMetadata
        {
            get { return structure.GraphMetadata; }
        }

        /// <summary>
        /// Removes a metadata entry (if present) from the specified vertex.
        /// </summary>
        /// <param name="vertex">
        /// The coordinates of the vertex from which to remove the metadata entry.</param>
        /// <param name="key">The name of the metadata entry to remove.</param>
        /// <exception cref="System.InvalidOperationException">
        /// The specified vertex is not present in the graph, or key is "xy".</exception>
        /// <exception cref="System.ArgumentNullException">vertex or key are null.</exception>
        public void UnsetVertexMetadatum(Point2D vertex, string key)
        {
            ArgumentNullException.ThrowIfNull(vertex);

            FailIfReserved(key);

            var vertexIndex = IndexOf(vertex, nameof(vertex));
            structure.UnsetVertexMetadatum(vertexIndex, key);
        }

        /// <summary>
        /// Adds or updates the value of a metadata entry of the specified vertex.
        /// </summary>
        /// <param name="vertex">The coordinates of the vertex to which to add
        /// the metadata entry.</param>
        /// <param name="key">The key of the metadata entry to set.</param>
        /// <param name="value">The value of the metadata entry to set.</param>
        /// <exception cref="System.InvalidOperationException">
        /// The specified vertex is not present in the graph, or key is "xy".</exception>
        /// <exception cref="System.ArgumentNullException">vertex or key are null.</exception>
        public void SetVertexMetadatum(Point2D vertex, string key, string value)
        {
            ArgumentNullException.ThrowIfNull(vertex);

            FailIfReserved(key);

            var vertexIndex = IndexOf(vertex, nameof(vertex));
            structure.SetVertexMetadatum(vertexIndex, key, value);
        }

        /// <summary>
        /// Gets the value of a metadata entry of the specified vertex.
        /// </summary>
        /// <param name="vertex">The coordinates of the vertex from which to retrieve the
        /// metadata entry.</param>
        /// <param name="key">The key of the metadata entry to retrieve.</param>
        /// <param name="defaultValue">The value to return if the vertex does not have a
        /// metadata entry with the given key.</param>
        /// <returns>The value of the metadata entry with the specified key, or defaultValue,
        /// if no entry with that key exists.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The specified vertex is not present in the graph.</exception>
        /// <exception cref="System.ArgumentNullException">vertex or key are null.</exception>
        public string GetVertexMetadatum(
            Point2D vertex, string key, string defaultValue = null)
        {
            ArgumentNullException.ThrowIfNull(vertex);

            var vertexIndex = IndexOf(vertex, nameof(vertex));
            return structure.GetVertexMetadatum(vertexIndex, key, defaultValue);
        }

        /// <summary>
        /// Gets all of the metadata entries of a vertex.
        /// </summary>
        /// <returns>A dictionary of all the metadata entries of a vertex.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The specified vertex is not present in the graph.</exception>
        /// <exception cref="System.ArgumentNullException">vertex is null.</exception>
        public IReadOnlyDictionary<string, string> GetVertexMetadata(Point2D vertex)
        {
            ArgumentNullException.ThrowIfNull(vertex);

            var vertexIndex = IndexOf(vertex, nameof(vertex));
            return structure.GetVertexMetadata(vertexIndex);
        }

        /// <summary>
        /// Removes a metadata value (if present) from the specified edge.
        /// </summary>
        /// <param name="from">The coordinates of the source vertex.</param>
        /// <param name="to">The coordinates of the destination vertex.</param>
        /// <param name="key">The name of the metadata entry to remove.</param>
        /// <exception cref="System.InvalidOperationException">
        /// The specified edge is not present in the graph.</exception>
        /// <exception cref="System.ArgumentNullException">
        /// from, to, or key is null.</exception>
        public void UnsetEdgeMetadatum(Point2D from, Point2D to, string key)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);

            var fromIndex = IndexOf(from, nameof(from));
            var toIndex = IndexOf(to, nameof(to));

            structure.UnsetEdgeMetadatum(fromIndex, toIndex, key);
        }

        /// <summary>
        /// Adds or updates the value of a metadata entry of the specified edge.
        /// </summary>
        /// <param name="from">The coordinates of the source vertex.</param>
        /// <param name="to">The coordinates of the destination vertex.</param>
        /// <param name="key">The key of the metadata entry to set.</param>
        /// <param name="value">The value of the metadata entry to set.</param>
        /// <exception cref="System.InvalidOperationException">
        /// The specified edge is not present in the graph.</exception>
        /// <exception cref="System.ArgumentNullException">
        /// from, to, or key is null.</exception>
        public void SetEdgeMetadatum(Point2D from, Point2D to, string key, string value)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);

            var fromIndex = IndexOf(from, nameof(from));
            var toIndex = IndexOf(to, nameof(to));

            structure.SetEdgeMetadatum(fromIndex, toIndex, key, value);
        }

        /// <summary>
        /// Gets the value of a metadata entry of the specified edge.
        /// </summary>
        /// <param name="from">The coordinates of the source vertex.</param>
        /// <param name="to">The coordinates of the destination vertex.</param>
        /// <param name="key">The key of the metadata entry to retrieve.</param>
        /// <param name="defaultValue">The value to return if the edge does not have a metadata
        /// entry with the given key.</param>
        /// <returns>The value of the metadata entry with the specified key, or defaultValue,
        /// if no entry with that key exists.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The specified edge is not present in the graph.</exception>
        /// <exception cref="System.ArgumentNullException">
        /// from, to, or key is null.</exception>
        public string GetEdgeMetadatum(
            Point2D from, Point2D to, string key, string defaultValue = null)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);

            var fromIndex = IndexOf(from, nameof(from));
            var toIndex = IndexOf(to, nameof(to));

            return structure.GetEdgeMetadatum(fromIndex, toIndex, key, defaultValue);
        }

        /// <summary>
        /// Gets all of the metadata entries of an edge.
        /// </summary>
        /// <returns>A dictionary of all the metadata entries of an edge.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The specified edge is not present in the graph.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// from or to is null.</exception>
        public IReadOnlyDictionary<string, string> GetEdgeMetadata(
            Point2D from, Point2D to)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);

            var fromIndex = IndexOf(from, nameof(from));
            var toIndex = IndexOf(to, nameof(to));

            return structure.GetEdgeMetadata(fromIndex, toIndex);
        }

        #endregion

        #endregion


        #region Helper Methods

        int IndexOf(Point2D location, string paramName)
        {
            int result;
            if (false == vertexFromLocation.TryGetValue(location, out result))
                throw new ArgumentException(
                    "The specified vertex is not present in the graph.", paramName);

            return result;
        }

        static void FailIfReserved(string key)
        {
            if (IsReservedMetadataKey(key))
                throw new InvalidOperationException(
                    "The specified metadata key is reserved.");
        }

        #endregion
    }
}