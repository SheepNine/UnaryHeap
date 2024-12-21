using System.Collections.Generic;

namespace UnaryHeap.Utilities
{
    /// <summary>
    /// Dynamically assigns ordinal indices to arbitrary objects.
    /// </summary>
    public class IndexAssigner<T>
    {
        private SortedDictionary<T, int> objectToIndexMap;
        private List<T> indexToObjectMap;

        /// <summary>
        /// Constructs a new instance of the IndexAssigner class with the default
        /// object comparer.
        /// </summary>
        public IndexAssigner()
        {
            objectToIndexMap = new SortedDictionary<T, int>();
            indexToObjectMap = new List<T>();
        }

        /// <summary>
        /// Constructs a new instance of the IndexAssigner class with the specified
        /// object comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use to de-duplicate objects</param>
        public IndexAssigner(IComparer<T> comparer)
        {
            objectToIndexMap = new SortedDictionary<T, int>(comparer);
            indexToObjectMap = new List<T>();
        }

        /// <summary>
        /// The number of objects that have had ordinals assigned.
        /// </summary>
        public int Count
        {
            get { return objectToIndexMap.Count; }
        }

        /// <summary>
        /// Gets the item with the specified index.
        /// </summary>
        /// <param name="index">The index to look up.</param>
        /// <returns>The item with the specified index.</returns>
        public T this[int index]
        {
            get { return indexToObjectMap[index]; }
        }

        /// <summary>
        /// Gets the index of the specified object. 
        /// </summary>
        /// <param name="item">The item to which to assign an index.</param>
        /// <returns>A new index if item has not previously been seen;
        /// otherwise, returns the previously-assigned index.</returns>
        public int GetIndex(T item)
        {
            if (!objectToIndexMap.TryGetValue(item, out int value))
            {
                value = indexToObjectMap.Count;
                objectToIndexMap.Add(item, value);
                indexToObjectMap.Add(item);
            }

            return value;
        }
    }
}
