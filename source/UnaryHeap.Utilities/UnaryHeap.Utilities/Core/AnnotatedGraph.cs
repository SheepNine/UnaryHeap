﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Utilities.Core
{
    /// <summary>
    /// Represents an extension of the UnaryHeap.Utilities.SimpleGraph class 
    /// that can associate arbitrary string metadata with the graph, its
    /// vertices and its edges.
    /// </summary>
    public partial class AnnotatedGraph
    {
        #region Member Variables

        SimpleGraph structure;
        SortedDictionary<string, string> graphMetadata;
        List<SortedDictionary<string, string>> vertexMetadata;
        SortedDictionary<ulong, SortedDictionary<string, string>> edgeMetadata;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the UnaryHeap.Utilities.AnnotatedGraph class.
        /// </summary>
        /// <param name="directed">Whether or not the resulting graph is directed.</param>
        public AnnotatedGraph(bool directed)
        {
            structure = new SimpleGraph(directed);
            graphMetadata = new SortedDictionary<string, string>();
            vertexMetadata = new List<SortedDictionary<string, string>>();
            edgeMetadata = new SortedDictionary<ulong, SortedDictionary<string, string>>();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Indicates whether the current UnaryHeap.Utilities.AnnotatedGraph instance is a directed graph.
        /// </summary>
        public bool IsDirected
        {
            get { return structure.IsDirected; }
        }

        /// <summary>
        /// Gets the number of vertices in the current UnaryHeap.Utilities.AnnotatedGraph instance.
        /// </summary>
        public int NumVertices
        {
            get { return structure.NumVertices; }
        }

        /// <summary>
        /// Gets the indices of the vertices in the current UnaryHeap.Utilities.AnnotatedGraph instance.
        /// </summary>
        public IEnumerable<int> Vertices
        {
            get { return structure.Vertices; }
        }

        /// <summary>
        /// Gets [start, end] vertex index tuples for the edges in the current UnaryHeap.Utilities.AnnotatedGraph instance.
        /// </summary>
        /// <remarks>For undirected graphs, each edge occurs only once in the resulting enumeration.</remarks>
        public IEnumerable<Tuple<int, int>> Edges
        {
            get { return structure.Edges; }
        }

        #endregion


        #region Public Methods

        #region Graph Methods

        /// <summary>
        /// Adds a new vertex to the current UnaryHeap.Utilities.AnnotatedGraph instance.
        /// </summary>
        /// <returns>The index of the newly-created vertex.</returns>
        public int AddVertex()
        {
            var result = structure.AddVertex();
            vertexMetadata.Add(new SortedDictionary<string, string>());
            return result;
        }

        /// <summary>
        /// Removes a vertex from the current UnaryHeap.Utilities.AnnotatedGraph instance, as well as all
        /// edges incident to that vertex.
        /// </summary>
        /// <param name="index">The index of the vertex to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index is negative or the current UnaryHeap.Utilities.AnnotatedGraph instance does not contain a vertex with the given index.</exception>
        public void RemoveVertex(int index)
        {
            structure.RemoveVertex(index);
            vertexMetadata.RemoveAt(index);
            edgeMetadata = new SortedDictionary<ulong, SortedDictionary<string, string>>(
                structure.Edges.Select(e => EdgeKey(e.Item1, e.Item2)).ToDictionary(e => e, e => edgeMetadata[e]));
        }

        /// <summary>
        /// Adds a new edge to the current UnaryHeap.Utilities.AnnotatedGraph instance.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <param name="to">The index of the destination vertex.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">from or to is negative or the current UnaryHeap.Utilities.AnnotatedGraph instance does not contain a vertex with the given index.</exception>
        /// <exception cref="System.ArgumentException">from and to are equal, or an edge already exists between from and to.</exception>
        public void AddEdge(int from, int to)
        {
            structure.AddEdge(from, to);
            edgeMetadata.Add(EdgeKey(from, to), new SortedDictionary<string, string>());
        }

        /// <summary>
        /// Remove an edge from the current UnaryHeap.Utilities.AnnotatedGraph instance.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <param name="to">The index of the destination vertex.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">from or to is negative or the current UnaryHeap.Utilities.AnnotatedGraph instance does not contain a vertex with the given index.</exception>
        /// <exception cref="System.ArgumentException">from and to are equal, or no edge exists between from and to.</exception>
        public void RemoveEdge(int from, int to)
        {
            structure.RemoveEdge(from, to);
            edgeMetadata.Remove(EdgeKey(from, to));
        }

        /// <summary>
        /// Determines whether the current UnaryHeap.Utilities.AnnotatedGraph instance has the specified edge.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <param name="to">The index of the destination vertex.</param>
        /// <returns>True, if there is an edge with the given from/to indices; otherwise, False.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">from or to is negative or the current UnaryHeap.Utilities.AnnotatedGraph instance does not contain a vertex with the given index.</exception>
        public bool HasEdge(int from, int to)
        {
            return structure.HasEdge(from, to);
        }

        /// <summary>
        /// Determine which vertices are neighbours of the specified vertex.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <returns>An array containing the vertex indices of vertices connected to the specified vertex.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is negative or the current UnaryHeap.Utilities.AnnotatedGraph instance does not contain a vertex with the given index.</exception>
        public int[] GetNeighbours(int from)
        {
            return structure.GetNeighbors(from);
        }

        /// <summary>
        /// Creates a copy of the current UnaryHeap.Utilities.AnnotatedGraph object.
        /// </summary>
        /// <returns>A copy of the current UnaryHeap.Utilities.AnnotatedGraph object.</returns>
        public AnnotatedGraph Clone()
        {
            var result = new AnnotatedGraph(IsDirected);
            result.structure = structure.Clone();

            // --- Deep copy dictionaries ---
            result.graphMetadata = new SortedDictionary<string, string>(graphMetadata);
            result.vertexMetadata = vertexMetadata.Select(v => new SortedDictionary<string, string>(v)).ToList();
            result.edgeMetadata = new SortedDictionary<ulong, SortedDictionary<string, string>>();
            foreach (var e in edgeMetadata)
                result.edgeMetadata.Add(e.Key, new SortedDictionary<string, string>(e.Value));

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
            UnsetMetadatum(graphMetadata, key);
        }

        /// <summary>
        /// Adds or updates the value of a metadata entry of the graph.
        /// </summary>
        /// <param name="key">The key of the metadata entry to set.</param>
        /// <param name="value">The value of the metadata entry to set.</param>
        /// <exception cref="System.ArgumentNullException">key is null.</exception>
        public void SetGraphMetadatum(string key, string value)
        {
            SetMetadatum(graphMetadata, key, value);
        }

        /// <summary>
        /// Gets the value of a metadata entry of the graph.
        /// </summary>
        /// <param name="key">The key of the metadata entry to retrieve.</param>
        /// <param name="defaultValue">The value to return if the graph does not have a metadata entry with the given key.</param>
        /// <returns>The value of the metadata entry with the specified key, or defaultValue, if no entry with that key exists.</returns>
        /// <exception cref="System.ArgumentNullException">key is null.</exception>
        public string GetGraphMetadatum(string key, string defaultValue = null)
        {
            return GetMetadatum(graphMetadata, key, defaultValue);
        }


        /// <summary>
        /// Removes a metadata entry (if present) from the specified vertex.
        /// </summary>
        /// <param name="index">The index of the vertex from which to remove the metadata entry.</param>
        /// <param name="key">The name of the metadata entry to remove.</param>
        /// <exception cref="System.InvalidOperationException">The specified vertex is not present in the graph.</exception>
        /// <exception cref="System.ArgumentNullException">key is null.</exception>
        public void UnsetVertexMetadatum(int index, string key)
        {
            UnsetMetadatum(MetadataForVertex(index), key);
        }

        /// <summary>
        /// Adds or updates the value of a metadata entry of the specified vertex.
        /// </summary>
        /// <param name="index">The index of the vertex to which to add the metadata entry.</param>
        /// <param name="key">The key of the metadata entry to set.</param>
        /// <param name="value">The value of the metadata entry to set.</param>
        /// <exception cref="System.InvalidOperationException">The specified vertex is not present in the graph.</exception>
        /// <exception cref="System.ArgumentNullException">key is null.</exception>
        public void SetVertexMetadatum(int index, string key, string value)
        {
            SetMetadatum(MetadataForVertex(index), key, value);
        }

        /// <summary>
        /// Gets the value of a metadata entry of the specified vertex.
        /// </summary>
        /// <param name="index">The index of the vertex from which to retrieve the metadata entry.</param>
        /// <param name="key">The key of the metadata entry to retrieve.</param>
        /// <param name="defaultValue">The value to return if the vertex does not have a metadata entry with the given key.</param>
        /// <returns>The value of the metadata entry with the specified key, or defaultValue, if no entry with that key exists.</returns>
        /// <exception cref="System.InvalidOperationException">The specified vertex is not present in the graph.</exception>
        /// <exception cref="System.ArgumentNullException">key is null.</exception>
        public string GetVertexMetadatum(int index, string key, string defaultValue = null)
        {
            return GetMetadatum(MetadataForVertex(index), key, defaultValue);
        }


        /// <summary>
        /// Removes a metadata value (if present) from the specified edge.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <param name="to">The index of the destination vertex.</param>
        /// <param name="key">The name of the metadata entry to remove.</param>
        /// <exception cref="System.InvalidOperationException">The specified edge is not present in the graph.</exception>
        /// <exception cref="System.ArgumentNullException">key is null.</exception>
        public void UnsetEdgeMetadatum(int from, int to, string key)
        {
            UnsetMetadatum(MetadataForEdge(from, to), key);
        }

        /// <summary>
        /// Adds or updates the value of a metadata entry of the specified edge.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <param name="to">The index of the destination vertex.</param>
        /// <param name="key">The key of the metadata entry to set.</param>
        /// <param name="value">The value of the metadata entry to set.</param>
        /// <exception cref="System.InvalidOperationException">The specified edge is not present in the graph.</exception>
        /// <exception cref="System.ArgumentNullException">key is null.</exception>
        public void SetEdgeMetadatum(int from, int to, string key, string value)
        {
            SetMetadatum(MetadataForEdge(from, to), key, value);
        }

        /// <summary>
        /// Gets the value of a metadata entry of the specified edge.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <param name="to">The index of the destination vertex.</param>
        /// <param name="key">The key of the metadata entry to retrieve.</param>
        /// <param name="defaultValue">The value to return if the edge does not have a metadata entry with the given key.</param>
        /// <returns>The value of the metadata entry with the specified key, or defaultValue, if no entry with that key exists.</returns>
        /// <exception cref="System.InvalidOperationException">The specified edge is not present in the graph.</exception>
        /// <exception cref="System.ArgumentNullException">key is null.</exception>
        public string GetEdgeMetadatum(int from, int to, string key, string defaultValue = null)
        {
            return GetMetadatum(MetadataForEdge(from, to), key, defaultValue);
        }

        #endregion

        #endregion


        #region Helper Methods

        SortedDictionary<string, string> MetadataForVertex(int index)
        {
            if (0 > index || structure.NumVertices <= index)
                throw new InvalidOperationException("Vertex does not exist in graph.");

            return vertexMetadata[index];
        }

        SortedDictionary<string, string> MetadataForEdge(int from, int to)
        {
            if (0 > from || structure.NumVertices <= from)
                throw new InvalidOperationException("'from' vertex does not exist in graph.");
            if (0 > to || structure.NumVertices <= to)
                throw new InvalidOperationException("'to' vertex does not exist in graph.");
            if (false == structure.HasEdge(from, to))
                throw new InvalidOperationException("Edge does not exist in graph.");

            return edgeMetadata[EdgeKey(from, to)];
        }

        static void UnsetMetadatum(SortedDictionary<string, string> metadata, string key)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            metadata.Remove(key);
        }

        static void SetMetadatum(SortedDictionary<string, string> metadata, string key, string value)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            metadata[key] = value;
        }

        static string GetMetadatum(SortedDictionary<string, string> metadata, string key, string defaultValue)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            if (metadata.ContainsKey(key))
                return metadata[key];
            else
                return defaultValue;
        }

        ulong EdgeKey(int from, int to)
        {
            structure.OrientCorrectlyForUndirectedGraph(ref from, ref to);
            return (((ulong)(uint)from) << 32) | (ulong)(uint)to;
        }

        #endregion
    }
}
