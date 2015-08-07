#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Utilities.Misc
{
    /// <summary>
    /// Represents a strongly-typed queue of objects where the least element is the one at the front of the queue.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification="'Priority queue' is a well-known computer science concept.")]
    public class PriorityQueue<T>
    {
        #region Member Variables

        List<T> heap;
        IComparer<T> comparer;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty and has the default
        /// initial capacity and uses the default comparison for type T.
        /// </summary>
        public PriorityQueue() : this(Comparer<T>.Default) { }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty and has the specified
        /// initial capacity and uses the default comparison for type T.
        /// </summary>
        /// <param name="capacity">The initial capacity of the PriorityQueue.</param>
        public PriorityQueue(int capacity) : this(capacity, Comparer<T>.Default) { }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that contains elements copied from the
        /// specified collection and has sufficient capacity to accommodate the number of elements copied,
        /// and uses the default comparison for type T.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new PriorityQueue.</param>
        public PriorityQueue(IEnumerable<T> collection) : this(collection, Comparer<T>.Default) { }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty and has the default
        /// initial capacity and uses the specified comparison for type T.
        /// </summary>
        /// <param name="comparer">The comparison defining the priority order of the objects in the queue.</param>
        public PriorityQueue(IComparer<T> comparer) : this(new List<T>(), comparer) { }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty and has the specified
        /// initial capacity and uses the specified comparison for type T.
        /// </summary>
        /// <param name="capacity">The initial capacity of the PriorityQueue.</param>
        /// <param name="comparer">The comparison defining the priority order of the objects in the queue.</param>
        public PriorityQueue(int capacity, IComparer<T> comparer) : this(new List<T>(capacity), comparer) { }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that contains elements copied from the
        /// specified collection and has sufficient capacity to accommodate the number of elements copied,
        /// and uses the specified comparison for type T.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new PriorityQueue.</param>
        /// <param name="comparer">The comparison defining the priority order of the objects in the queue.</param>
        public PriorityQueue(IEnumerable<T> collection, IComparer<T> comparer) : this(new List<T>(collection), comparer) { }

        PriorityQueue(List<T> heap, IComparer<T> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            this.heap = heap;
            this.comparer = comparer;

            foreach (var i in Enumerable.Range(0, (heap.Count + 1) >> 1).Reverse())
                SiftDown(i);
        }

        #endregion

        #region Public Methods and Properties

        /// <summary>
        /// Adds an object to the PriorityQueue.
        /// </summary>
        /// <param name="item">The object to be added to the PriorityQueue. The value can be null for reference types.</param>
        public void Enqueue(T item)
        {
            heap.Add(item);
            SiftUp(heap.Count - 1);
        }

        /// <summary>
        /// Removes and returns the object at the front of the PriorityQueue.
        /// </summary>
        /// <returns>The object at the front of the PriorityQueue.</returns>
        public T Dequeue()
        {
            try
            {
                return Peek();
            }
            finally
            {
                heap[0] = heap[heap.Count - 1];
                heap.RemoveAt(heap.Count - 1);
                if (!IsEmpty)
                    SiftDown(0);
            }
        }

        /// <summary>
        /// Returns the object at the front of the PriorityQueue without removing it.
        /// </summary>
        /// <returns>The object at the front of the PriorityQueue.</returns>
        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException();
            return heap[0];
        }

        /// <summary>
        /// Gets whether there are any objects in the PriorityQueue.
        /// </summary>
        public bool IsEmpty
        {
            get { return heap.Count == 0; }
        }

        #endregion


        #region Helper Methods

        void SiftDown(int index)
        {
            while (true)
            {
                var leftChild = LeftChild(index);
                var rightChild = RightChild(index);
                if (rightChild < heap.Count) // Both children present
                {
                    if (comparer.Compare(heap[leftChild], heap[rightChild]) <= 0 && comparer.Compare(heap[leftChild], heap[index]) <= 0)
                    {
                        Swap(index, leftChild);
                        index = leftChild;
                    }
                    else if (comparer.Compare(heap[rightChild], heap[index]) <= 0)
                    {
                        Swap(index, rightChild);
                        index = rightChild;
                    }
                    else
                        break;
                }
                else if (leftChild < heap.Count) // Only left child present
                {
                    if (comparer.Compare(heap[leftChild], heap[index]) <= 0)
                    {
                        Swap(index, leftChild);
                        index = leftChild;
                    }
                    else
                        break;
                }
                else
                    break;
            }
        }

        void SiftUp(int index)
        {
            while (index > 0)
            {
                var parent = Parent(index);
                if (comparer.Compare(heap[index], heap[parent]) > 0)
                    break;
                Swap(index, parent);
                index = parent;
            }
        }

        static int LeftChild(int index)
        {
            return 1 + (index << 1);
        }

        static int RightChild(int index)
        {
            return (1 + index) << 1;
        }

        static int Parent(int index)
        {
            return (index - 1) >> 1;
        }

        void Swap(int a, int b)
        {
            var temp = heap[a];
            heap[a] = heap[b];
            heap[b] = temp;
        }
        #endregion
    }
}

#endif