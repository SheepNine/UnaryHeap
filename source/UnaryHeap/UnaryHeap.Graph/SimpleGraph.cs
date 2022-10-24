using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Graph
{
    /// <summary>
    /// Represents a mathematical graph (a set of vertices and a set of
    /// edges connecting vertices) which may only contain at most one
    /// edge from a given start vertex to a given end vertex.
    /// </summary>
    /// <remarks>
    /// A SimpleGraph may be directed or undirected. If it is undirected, then
    /// edges are symmetric; that is, if there exists an edge from
    /// vertex A to vertex B, then there also exists an edge from
    /// vertex B to vertex A. If it is directed, then edges are not
    /// symmetric, and an edge from vertex A to vertex B may exist
    /// without a corresponding edge from vertex B to vertex A.
    /// </remarks>
    public partial class SimpleGraph
    {
        #region Member Variables

        bool directed;
        List<SortedSet<int>> adjacencies;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SimpleGraph class.
        /// </summary>
        /// <param name="directed">Whether or not the resulting graph is directed.</param>
        public SimpleGraph(bool directed)
        {
            this.directed = directed;
            this.adjacencies = new List<SortedSet<int>>();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Indicates whether the current SimpleGraph instance is a directed graph.
        /// </summary>
        public bool IsDirected
        {
            get { return directed; }
        }

        /// <summary>
        /// Gets the number of vertices in the current SimpleGraph instance.
        /// </summary>
        public int NumVertices
        {
            get { return adjacencies.Count; }
        }

        /// <summary>
        /// Gets the indices of the vertices in the current SimpleGraph instance.
        /// </summary>
        public IEnumerable<int> Vertices
        {
            get { return Enumerable.Range(0, adjacencies.Count); }
        }

        /// <summary>
        /// Gets [start, end] vertex index tuples for the edges in the
        /// current SimpleGraph instance.
        /// </summary>
        /// <remarks>For undirected graphs, each edge occurs only once in
        /// the resulting enumeration.</remarks>
        public IEnumerable<Tuple<int, int>> Edges
        {
            get
            {
                return Enumerable.Range(0, adjacencies.Count).SelectMany(i =>
                    adjacencies[i].Select(j =>
                        Tuple.Create(i, j)
                    )
                );
            }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Adds a new vertex to the current SimpleGraph instance.
        /// </summary>
        /// <returns>The index of the newly-created vertex.</returns>
        public int AddVertex()
        {
            adjacencies.Add(new SortedSet<int>());
            return adjacencies.Count - 1;
        }

        /// <summary>
        /// Removes a vertex from the current SimpleGraph instance, as well as all
        /// edges incident to that vertex.
        /// </summary>
        /// <param name="index">The index of the vertex to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is negative or the current SimpleGraph instance does not contain a vertex
        /// with the given index.</exception>
        public void RemoveVertex(int index)
        {
            VertexIndexRangeCheck(index, nameof(index));

            adjacencies.RemoveAt(index);
            foreach (var i in Enumerable.Range(0, adjacencies.Count))
                adjacencies[i] = new SortedSet<int>(
                    RemoveVertexFromAdjacencies(adjacencies[i], index));
        }

        static IEnumerable<int> RemoveVertexFromAdjacencies(
            SortedSet<int> adjacency, int vertex)
        {
            return adjacency.Where(j => j != vertex).Select(j => j > vertex ? j - 1 : j);
        }

        /// <summary>
        /// Removes several vertices from the current SimpleGraph, as well as all
        /// edges incident to that vertex. This method is much faster than calling
        /// RemoveVertex() multiple times.
        /// </summary>
        /// <param name="indexes">The indices of the vertices to remove.</param>
        /// <returns>An array containing the new vertex index of the vertices in this
        /// SimpleGraph, or -1 if that vertex was deleted.</returns>
        public int[] RemoveVertices(IEnumerable<int> indexes)
        {
            var set = CollectVertices(indexes, nameof(indexes));
            var map = MakeVertexIndexMap(set);

            adjacencies = Enumerable.Range(0, NumVertices).Where(i => -1 != map[i]).Select(
                i => new SortedSet<int>(adjacencies[i].Select(j => map[j]).Where(j => -1 != j)
                )).ToList();

            return map;
        }

        static SortedSet<int> CollectVertices(IEnumerable<int> input, string paramName)
        {
            if (null == input)
                throw new ArgumentNullException(paramName);

            var result = new SortedSet<int>();

            foreach (var item in input)
            {
                if (result.Contains(item))
                    throw new ArgumentException(
                        "Enumerable contains duplicate index.", paramName);

                result.Add(item);
            }

            return result;
        }

        int[] MakeVertexIndexMap(SortedSet<int> set)
        {
            var result = Enumerable.Range(0, NumVertices).ToArray();

            foreach (var entry in set)
            {
                result[entry] = -1;

                for (int i = entry + 1; i < NumVertices; i++)
                    result[i] -= 1;
            }

            return result;
        }

        /// <summary>
        /// Adds a new edge to the current SimpleGraph instance.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <param name="to">The index of the destination vertex.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// from or to is negative or the current SimpleGraph instance does not contain
        /// a vertex with the given index.</exception>
        /// <exception cref="System.ArgumentException">
        /// from and to are equal, or an edge already exists between from and to.</exception>
        public void AddEdge(int from, int to)
        {
            VertexIndexRangeCheck(from, nameof(from));
            VertexIndexRangeCheck(to, nameof(to));
            OrientCorrectlyForUndirectedGraph(ref from, ref to);
            VertexIndicesDistinctCheck(from, to);

            var adjacenciesFrom = adjacencies[from];

            if (adjacenciesFrom.Contains(to))
                throw new ArgumentException("The given edge already exists in the graph.");

            adjacenciesFrom.Add(to);
        }

        /// <summary>
        /// Remove an edge from the current SimpleGraph instance.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <param name="to">The index of the destination vertex.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// from or to is negative or the current SimpleGraph instance does not contain a
        /// vertex with the given index.</exception>
        /// <exception cref="System.ArgumentException">
        /// from and to are equal, or no edge exists between from and to.</exception>
        public void RemoveEdge(int from, int to)
        {
            VertexIndexRangeCheck(from, nameof(from));
            VertexIndexRangeCheck(to, nameof(to));
            OrientCorrectlyForUndirectedGraph(ref from, ref to);

            var adjacenciesFrom = adjacencies[from];

            if (false == adjacenciesFrom.Contains(to))
                throw new ArgumentException("The given edge was not present in the graph.");

            adjacenciesFrom.Remove(to);
        }

        /// <summary>
        /// Determines whether the current SimpleGraph instance has the specified edge.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <param name="to">The index of the destination vertex.</param>
        /// <returns>True, if there is an edge with the given from/to indices;
        /// otherwise, False.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// from or to is negative or the current SimpleGraph instance does not contain a
        /// vertex with the given index.</exception>
        public bool HasEdge(int from, int to)
        {
            VertexIndexRangeCheck(from, nameof(from));
            VertexIndexRangeCheck(to, nameof(to));
            OrientCorrectlyForUndirectedGraph(ref from, ref to);

            return adjacencies[from].Contains(to);
        }

        /// <summary>
        /// Determine which vertices are neighbours of the specified vertex.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <returns>An array containing the vertex indices of vertices connected
        /// to the specified vertex.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is negative or the current SimpleGraph instance does not contain a
        /// vertex with the given index.</exception>
        public int[] GetNeighbours(int from)
        {
            VertexIndexRangeCheck(from, nameof(from));

            var result = new SortedSet<int>(adjacencies[from]);

            if (false == directed)
                result.UnionWith(Enumerable.Range(0, NumVertices)
                    .Where(i => adjacencies[i].Contains(from)));

            return result.ToArray();
        }

        /// <summary>
        /// Determine how many vertices are neighbours of the specified vertex.
        /// </summary>
        /// <param name="from">The index of the source vertex.</param>
        /// <returns>The number of vertices connected
        /// to the specified vertex.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is negative or the current SimpleGraph instance does not contain a
        /// vertex with the given index.</exception>
        public int NumNeighbours(int from)
        {
            VertexIndexRangeCheck(from, nameof(from));

            var result = adjacencies[from].Count;

            if (false == directed)
                result += Enumerable.Range(0, NumVertices)
                    .Count(i => adjacencies[i].Contains(from));

            return result;
        }

        /// <summary>
        /// Creates a copy of the current SimpleGraph instance.
        /// </summary>
        /// <returns>A copy of the current SimpleGraph instance.</returns>
        public SimpleGraph Clone()
        {
            var result = new SimpleGraph(directed);
            result.adjacencies = this.adjacencies.Select(ACopy).ToList();

            return result;
        }

        static SortedSet<int> ACopy(SortedSet<int> input)
        {
            return new SortedSet<int>(input);
        }

        #endregion


        #region Helper Methods

        internal void OrientCorrectlyForUndirectedGraph(ref int from, ref int to)
        {
            if (directed)
                return;

            if (from > to)
                Swap(ref from, ref to);
        }

        static void Swap(ref int a, ref int b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        void VertexIndexRangeCheck(int index, string paramName)
        {
            if (0 > index || adjacencies.Count <= index)
                throw new ArgumentOutOfRangeException(paramName);
        }

        static void VertexIndicesDistinctCheck(int from, int to)
        {
            if (from == to)
                throw new ArgumentException("Start vertex equals end vertex.");
        }

        #endregion
    }
}
