#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Utilities.Misc
{

    /// <summary>
    /// Represents a combination of a linked list (providing O(1) insertion and removal) and a
    /// binary tree (provding O(lg(n)) searching of ordered data).
    /// </summary>
    /// <remarks>
    /// User is responsible for maintaining order of the BinarySearchLinkedList.
    /// </remarks>
    /// <typeparam name="T">The type of data in the BinarySearchLinkedList.</typeparam>
    public class BinarySearchLinkedList<T>
    {
        #region Classes

        class ListNode : IBsllNode<T>
        {
            public T Data { get; set; }

            public ListNode PrevListNode;
            public BinarySearchLinkedList<T>.TreeNode OwnerTreeNode;
            public ListNode NextListNode;

            IBsllNode<T> IBsllNode<T>.PrevNode { get { return PrevListNode; } }
            IBsllNode<T> IBsllNode<T>.NextNode { get { return NextListNode; } }
        }

        class TreeNode
        {
            #region Cached Values

            public int Height;
            public TreeNode FirstLeafNode;
            public TreeNode LastLeafNode;

            #endregion


            #region Primary Values

            public TreeNode ParentTreeNode;
            public TreeNode PredTreeNode;
            public TreeNode SuccTreeNode;
            public ListNode ChildListNode;

            #endregion


            public TreeNode()
            {
                FirstLeafNode = this;
                LastLeafNode = this;
            }
        }

        #endregion


        #region Member Variables

        TreeNode root;
        int length;

        #endregion


        #region Construction

        /// <summary>
        /// Initializes a new instance of the BinarySearchLinkedList class.
        /// </summary>
        /// <param name="data">The initial data to populate in the BinarySearchLinkedList.</param>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentException">data is empty.</exception>
        public BinarySearchLinkedList(IEnumerable<T> data)
        {
            BuildTree(data, out root, out length);
        }

        static void BuildTree(IEnumerable<T> data, out TreeNode root, out int numLeaves)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var queue = new Queue<TreeNode>(data.Select(
                datum => new TreeNode()
                {
                    ChildListNode = new ListNode() { Data = datum },
                    Height = 1
                }));

            InitListNodeReferences(queue);

            if (queue.Count == 0)
                throw new ArgumentException("Data contains no elements.", "data");

            numLeaves = queue.Count;

            if (queue.Count == 1)
            {
                root = queue.Dequeue();
                return;
            }

            var branches = queue.Count - 1;
            var fullTree = 0;

            while (fullTree < branches)
                fullTree = (fullTree << 1) + 1;

            for (int i = 0; i < (queue.Count + branches - fullTree) >> 1; i++ )
                CombineNodes(queue);

            foreach (var i in Enumerable.Range(0, fullTree - branches))
                queue.Enqueue(queue.Dequeue());

            while (queue.Count > 1)
                CombineNodes(queue);

            root = queue.Dequeue();
        }

        static void InitListNodeReferences(Queue<TreeNode> queue)
        {
            TreeNode prev = null;
            foreach (var node in queue)
            {
                node.ChildListNode.OwnerTreeNode = node;
                if (prev != null)
                {
                    prev.ChildListNode.NextListNode = node.ChildListNode;
                    node.ChildListNode.PrevListNode = prev.ChildListNode;
                }
                prev = node;
            }
        }

        static void CombineNodes(Queue<TreeNode> queue)
        {
            var predChild = queue.Dequeue();
            var succChild = queue.Dequeue();

            var newParent = new TreeNode()
            {
                PredTreeNode = predChild,
                FirstLeafNode = predChild.FirstLeafNode,

                SuccTreeNode = succChild,
                LastLeafNode = succChild.LastLeafNode,

                Height = Math.Max(predChild.Height, succChild.Height) + 1,
            };

            predChild.ParentTreeNode = newParent;
            succChild.ParentTreeNode = newParent;

            queue.Enqueue(newParent);
        }

        #endregion


        #region Properties

        /// <summary>
        /// The number of nodes in the BinarySearchLinkedList.
        /// </summary>
        public int Length
        {
            get { return length; }
        }

        #endregion


        #region Insertion

        /// <summary>
        /// Inserts a new node into the BinarySearchLinkedList before the specified node.
        /// </summary>
        /// <param name="data">The new value to add.</param>
        /// <param name="node">The node to insert before.</param>
        /// <returns>The newly-created node.</returns>
        public IBsllNode<T> InsertBefore(T data, IBsllNode<T> node)
        {
            return Insert(node, false, data);
        }

        /// <summary>
        /// Inserts a new node into the BinarySearchLinkedList after the specified node.
        /// </summary>
        /// <param name="node">The node to insert after.</param>
        /// <param name="data">The new value to add.</param>
        /// <returns>The newly-created node.</returns>
        public IBsllNode<T> InsertAfter(IBsllNode<T> node, T data)
        {
            return Insert(node, true, data);
        }

        IBsllNode<T> Insert(IBsllNode<T> node, bool insertAfter, T data)
        {
            if (null == node)
                throw new ArgumentNullException("node");

            var typedNode = node as BinarySearchLinkedList<T>.ListNode;

            if (null == typedNode)
                throw new ArgumentException(string.Empty, "node");

            // --- Patch in new leaf data to doubly-linked list ---
            var newListNode = new ListNode()
            {
                Data = data,
            };

            if (insertAfter)
            {
                newListNode.PrevListNode = typedNode;
                newListNode.NextListNode = typedNode.NextListNode;
            }
            else
            {
                newListNode.PrevListNode = typedNode.PrevListNode;
                newListNode.NextListNode = typedNode;
            }

            if (newListNode.NextListNode != null)
                newListNode.NextListNode.PrevListNode = newListNode;
            if (newListNode.PrevListNode != null)
                newListNode.PrevListNode.NextListNode = newListNode;

            // --- Build new nodes for target's leaf and the new leaf ---
            var newPredTreeNode = new TreeNode()
            {
                Height = 1,
                PredTreeNode = null,
                ParentTreeNode = typedNode.OwnerTreeNode,
                SuccTreeNode = null,
            };
            var newSuccTreeNode = new TreeNode()
            {
                Height = 1,
                PredTreeNode = null,
                ParentTreeNode = typedNode.OwnerTreeNode,
                SuccTreeNode = null,
            };

            if (insertAfter)
            {
                newPredTreeNode.ChildListNode = typedNode;
                newSuccTreeNode.ChildListNode = newListNode;
            }
            else
            {
                newPredTreeNode.ChildListNode = newListNode;
                newSuccTreeNode.ChildListNode = typedNode;
            }

            var nodeOwner = typedNode.OwnerTreeNode;
            newPredTreeNode.ChildListNode.OwnerTreeNode = newPredTreeNode;
            newSuccTreeNode.ChildListNode.OwnerTreeNode = newSuccTreeNode;

            // --- Convert target node into parent for new nodes ---
            nodeOwner.Height = 2;
            nodeOwner.ChildListNode = null;
            nodeOwner.PredTreeNode = newPredTreeNode;
            nodeOwner.FirstLeafNode = newPredTreeNode;
            nodeOwner.SuccTreeNode = newSuccTreeNode;
            nodeOwner.LastLeafNode = newSuccTreeNode;

            // --- Update and rebalance tree ---
            UpdateCachedValues(nodeOwner);

            while (root.ParentTreeNode != null)
                root = root.ParentTreeNode;

            length++;

            // --- Return pointer to new node ---
            return newListNode;
        }

        #endregion


        #region Deletion

        /// <summary>
        /// Remove the specified node from the BinarySearchLinkedList.
        /// </summary>
        /// <param name="node">The node to delete.</param>
        public void Delete(IBsllNode<T> node)
        {
            if (null == node)
                throw new ArgumentNullException("node");

            var linkedNode = node as BinarySearchLinkedList<T>.ListNode;

            if (null == linkedNode)
                throw new ArgumentException(string.Empty, "node");

            if (linkedNode.OwnerTreeNode == root)
                throw new ArgumentException("Cannot delete last element in tree");


            // --- Patch out deleted leaf from linked list ---

            if (linkedNode.PrevListNode != null)
                linkedNode.PrevListNode.NextListNode = linkedNode.NextListNode;
            if (linkedNode.NextListNode != null)
                linkedNode.NextListNode.PrevListNode = linkedNode.PrevListNode;

            // --- Convert target's parent into target's sibling ---

            var treeNodeParent = linkedNode.OwnerTreeNode.ParentTreeNode;

            if (treeNodeParent.PredTreeNode == linkedNode.OwnerTreeNode)
                AssumeIdentity(treeNodeParent, treeNodeParent.SuccTreeNode);
            else if (treeNodeParent.SuccTreeNode == linkedNode.OwnerTreeNode)
                AssumeIdentity(treeNodeParent, treeNodeParent.PredTreeNode);
            else
                throw new NotImplementedException(
                    "Unreachable code reached. Something is wrong with the tree.");


            // --- Update and rebalance tree ---

            UpdateCachedValues(treeNodeParent.ParentTreeNode);

            while (root.ParentTreeNode != null)
                root = root.ParentTreeNode;

            length--;
        }

        static void AssumeIdentity(TreeNode parentNode, TreeNode childNode)
        {
            parentNode.Height = childNode.Height;
            parentNode.ChildListNode = childNode.ChildListNode;
            parentNode.PredTreeNode = childNode.PredTreeNode;
            parentNode.SuccTreeNode = childNode.SuccTreeNode;

            if (parentNode.ChildListNode != null)
            {
                parentNode.ChildListNode.OwnerTreeNode = parentNode;
                parentNode.FirstLeafNode = parentNode;
                parentNode.LastLeafNode = parentNode;
            }
            else
            {
                parentNode.PredTreeNode.ParentTreeNode = parentNode;
                parentNode.FirstLeafNode = parentNode.PredTreeNode.FirstLeafNode;
                parentNode.SuccTreeNode.ParentTreeNode = parentNode;
                parentNode.LastLeafNode = parentNode.SuccTreeNode.LastLeafNode;
            }
        }

        #endregion


        #region Tree balancing

        static void UpdateCachedValues(TreeNode node)
        {
            while (node != null)
            {
                UpdateCachedNodeProperties(node);

                var delta = node.PredTreeNode.Height - node.SuccTreeNode.Height;

                if (delta == -2)
                {
                    if (node.SuccTreeNode.PredTreeNode.Height == node.SuccTreeNode.SuccTreeNode.Height + 1)
                        RotateTree(node.SuccTreeNode, node.SuccTreeNode.PredTreeNode);

                    RotateTree(node, node.SuccTreeNode);

                    // Optimization: rotate tree updated cached properties for node
                    node = node.ParentTreeNode;
                }

                if (delta == 2)
                {
                    if (node.PredTreeNode.SuccTreeNode.Height == node.PredTreeNode.PredTreeNode.Height + 1)
                        RotateTree(node.PredTreeNode, node.PredTreeNode.SuccTreeNode);

                    RotateTree(node, node.PredTreeNode);

                    // Optimization: rotate tree updated cached properties for node
                    node = node.ParentTreeNode;
                }

                node = node.ParentTreeNode;
            }
        }

        static void RotateTree(TreeNode root, TreeNode pivot)
        {
            if (pivot == root.PredTreeNode)
                RotatePredecessorUp(root, pivot);
            else
                RotateSuccessorUp(root, pivot);
        }

        /// <summary>
        /// The predecessor of node root is rotated into its place, making root its successor.
        /// </summary>
        static void RotatePredecessorUp(TreeNode root, TreeNode pivot)
        {
            // --- Update child Pointers ---

            root.PredTreeNode = pivot.SuccTreeNode;
            pivot.SuccTreeNode = root;


            // --- Update Parent pointers ---

            pivot.ParentTreeNode = root.ParentTreeNode;
            root.ParentTreeNode = pivot;
            root.PredTreeNode.ParentTreeNode = root;


            // --- Redirect grandparent to new child ---

            if (pivot.ParentTreeNode != null)
            {
                if (pivot.ParentTreeNode.PredTreeNode == root)
                    pivot.ParentTreeNode.PredTreeNode = pivot;
                else
                    pivot.ParentTreeNode.SuccTreeNode = pivot;
            }


            // --- Update caches ---

            UpdateCachedNodeProperties(root);
            UpdateCachedNodeProperties(pivot);
        }

        /// <summary>
        /// The successor of node root is rotated into its place, making root its predecessor.
        /// </summary>
        static void RotateSuccessorUp(TreeNode root, TreeNode pivot)
        {
            // --- Update child Pointers ---

            root.SuccTreeNode = pivot.PredTreeNode;
            pivot.PredTreeNode = root;


            // --- Update Parent pointers ---

            pivot.ParentTreeNode = root.ParentTreeNode;
            root.ParentTreeNode = pivot;
            root.SuccTreeNode.ParentTreeNode = root;


            // --- Redirect grandparent to new child ---

            if (pivot.ParentTreeNode != null)
            {
                if (pivot.ParentTreeNode.PredTreeNode == root)
                    pivot.ParentTreeNode.PredTreeNode = pivot;
                else
                    pivot.ParentTreeNode.SuccTreeNode = pivot;
            }


            // --- Update caches ---

            UpdateCachedNodeProperties(root);
            UpdateCachedNodeProperties(pivot);
        }

        static void UpdateCachedNodeProperties(TreeNode node)
        {
            node.Height = Math.Max(node.PredTreeNode.Height, node.SuccTreeNode.Height) + 1;
            node.FirstLeafNode = node.PredTreeNode.FirstLeafNode;
            node.LastLeafNode = node.SuccTreeNode.LastLeafNode;
        }

        #endregion


        #region Search

        /// <summary>
        /// Search the BinarySearchLinkedList to find two nodes bracketing a given value.
        /// </summary>
        /// <param name="searchValue">The vlaue for which to find bracketing nodes.</param>
        /// <param name="comparator">A delegate comparing searchValue to two adjacent node data.</param>
        /// <param name="pred">The predecessor bracketing node according to the Comparator delegate.</param>
        /// <param name="succ">The successor bracketing node according to the Comparator delegate.</param>
        public void BinarySearch(T searchValue,
            BinarySearchComparator<T> comparator,
            out IBsllNode<T> pred, out IBsllNode<T> succ)
        {
            BinarySearch(searchValue,
                (t) => t,
                new Func<T, T, T, int>(comparator),
                out pred, out succ);
        }

        /// <summary>
        /// Search the BinarySearchLinkedList to find two nodes bracketing a given value.
        /// </summary>
        /// <typeparam name="TSearch">The type of searchValue.</typeparam>
        /// <typeparam name="TCompare">The type of the return value for DataSelector.</typeparam>
        /// <param name="searchValue">The vlaue for which to find bracketing nodes.</param>
        /// <param name="dataSelector">A delegate returning a field of T.</param>
        /// <param name="comparator">A delegate comparing searchValue to the DataSelector return value for
        /// two adjacent nodes.</param>
        /// <param name="pred">The predecessor bracketing node according to the Comparator delegate.</param>
        /// <param name="succ">The successor bracketing node according to the Comparator delegate.</param>
        public void BinarySearch<TSearch, TCompare>(
            TSearch searchValue,
            Func<T, TCompare> dataSelector,
            Func<TSearch, TCompare, TCompare, int> comparator,
            out IBsllNode<T> pred, out IBsllNode<T> succ)
        {
            if (null == dataSelector)
                throw new ArgumentNullException("dataSelector");
            if (null == comparator)
                throw new ArgumentNullException("comparator");

            TreeNode iter = root;
            TreeNode predNode, succNode;

            while (iter.ChildListNode == null)
            {
                predNode = iter.PredTreeNode.LastLeafNode;
                succNode = iter.SuccTreeNode.FirstLeafNode;

                var compResult = comparator(
                        searchValue,
                        dataSelector(predNode.ChildListNode.Data),
                        dataSelector(succNode.ChildListNode.Data));

                if (compResult > 0)
                {
                    iter = iter.SuccTreeNode;
                }
                else if (compResult < 0)
                {
                    iter = iter.PredTreeNode;
                }
                else // Found exact match
                {
                    pred = predNode.ChildListNode;
                    succ = succNode.ChildListNode;
                    return;
                }
            }

            pred = iter.ChildListNode;
            succ = iter.ChildListNode;
        }

        #endregion
    }

    /// <summary>
    /// Comparison delegate for the BinarySearchLinkedList.BinarySearch method. The value
    /// being searched is compared to two sequential nodes in the linked list, and
    /// the delegate determines which node is 'closer' to the search data.
    /// </summary>
    /// <param name="searchValue">The vlaue for which to find bracketing nodes.</param>
    /// <param name="predValue">The first bracketing value.</param>
    /// <param name="succValue">The second bracketing value.</param>
    /// <returns>A negative value, if searchValue is 'closer to' predValue.
    /// Zero, if searchValue is 'exactly between' predValue and succValue.
    /// A positive value, if searchValue is 'closer to' succValue.</returns>
    public delegate int BinarySearchComparator<T>(T searchValue, T predValue, T succValue);

    /// <summary>
    /// Represents a node of a BinarySearchLinkedList.
    /// </summary>
    /// <typeparam name="T">The type of data in the BinarySearchLinkedList.</typeparam>
    public interface IBsllNode<T>
    {
        /// <summary>
        /// Returns the value stored in this node.
        /// </summary>
        T Data { get; }

        /// <summary>
        /// Returns the list node preceding this node in the linked list.
        /// </summary>
        IBsllNode<T> PrevNode { get; }

        /// <summary>
        /// Returns ths list node following this node in the linked list.
        /// </summary>
        IBsllNode<T> NextNode { get; }
    }
}

#endif