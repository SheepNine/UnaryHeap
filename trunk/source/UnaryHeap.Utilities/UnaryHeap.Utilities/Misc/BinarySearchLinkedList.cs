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
            public IBsllNode<T> PrevNode { get { return PrevListNode; } }
            public BinarySearchLinkedList<T>.TreeNode OwnerTreeNode;
            public ListNode NextListNode;
            public IBsllNode<T> NextNode { get { return NextListNode; } }
        }

        class TreeNode
        {
            #region Cached Values

            public int Height;
            public TreeNode LeftmostLeafNode;
            public TreeNode RightmostLeafNode;

            #endregion


            #region Primary Values

            public TreeNode ParentTreeNode;
            public TreeNode LeftTreeNode;
            public TreeNode RightTreeNode;
            public ListNode ChildListNode;

            #endregion


            public TreeNode()
            {
                LeftmostLeafNode = this;
                RightmostLeafNode = this;
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
            var leftChild = queue.Dequeue();
            var rightChild = queue.Dequeue();

            var newParent = new TreeNode()
            {
                LeftTreeNode = leftChild,
                LeftmostLeafNode = leftChild.LeftmostLeafNode,

                RightTreeNode = rightChild,
                RightmostLeafNode = rightChild.RightmostLeafNode,

                Height = Math.Max(leftChild.Height, rightChild.Height) + 1,
            };

            leftChild.ParentTreeNode = newParent;
            rightChild.ParentTreeNode = newParent;

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
            var leftTreeNode = new TreeNode()
            {
                Height = 1,
                LeftTreeNode = null,
                ParentTreeNode = typedNode.OwnerTreeNode,
                RightTreeNode = null,
            };
            var rightTreeNode = new TreeNode()
            {
                Height = 1,
                LeftTreeNode = null,
                ParentTreeNode = typedNode.OwnerTreeNode,
                RightTreeNode = null,
            };

            if (insertAfter)
            {
                leftTreeNode.ChildListNode = typedNode;
                rightTreeNode.ChildListNode = newListNode;
            }
            else
            {
                leftTreeNode.ChildListNode = newListNode;
                rightTreeNode.ChildListNode = typedNode;
            }

            var nodeOwner = typedNode.OwnerTreeNode;
            leftTreeNode.ChildListNode.OwnerTreeNode = leftTreeNode;
            rightTreeNode.ChildListNode.OwnerTreeNode = rightTreeNode;

            // --- Convert target node into parent for new nodes ---
            nodeOwner.Height = 2;
            nodeOwner.ChildListNode = null;
            nodeOwner.LeftTreeNode = leftTreeNode;
            nodeOwner.LeftmostLeafNode = leftTreeNode;
            nodeOwner.RightTreeNode = rightTreeNode;
            nodeOwner.RightmostLeafNode = rightTreeNode;

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

            if (treeNodeParent.LeftTreeNode == linkedNode.OwnerTreeNode)
                AssumeIdentity(treeNodeParent, treeNodeParent.RightTreeNode);
            else if (treeNodeParent.RightTreeNode == linkedNode.OwnerTreeNode)
                AssumeIdentity(treeNodeParent, treeNodeParent.LeftTreeNode);
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
            parentNode.LeftTreeNode = childNode.LeftTreeNode;
            parentNode.RightTreeNode = childNode.RightTreeNode;

            if (parentNode.ChildListNode != null)
            {
                parentNode.ChildListNode.OwnerTreeNode = parentNode;
                parentNode.LeftmostLeafNode = parentNode;
                parentNode.RightmostLeafNode = parentNode;
            }
            else
            {
                parentNode.LeftTreeNode.ParentTreeNode = parentNode;
                parentNode.LeftmostLeafNode = parentNode.LeftTreeNode.LeftmostLeafNode;
                parentNode.RightTreeNode.ParentTreeNode = parentNode;
                parentNode.RightmostLeafNode = parentNode.RightTreeNode.RightmostLeafNode;
            }
        }

        #endregion


        #region Tree balancing

        static void UpdateCachedValues(TreeNode node)
        {
            while (node != null)
            {
                UpdateCachedNodeProperties(node);

                var delta = node.LeftTreeNode.Height - node.RightTreeNode.Height;

                if (delta == -2)
                {
                    if (node.RightTreeNode.LeftTreeNode.Height == node.RightTreeNode.RightTreeNode.Height + 1)
                        RotateTree(node.RightTreeNode, node.RightTreeNode.LeftTreeNode);

                    RotateTree(node, node.RightTreeNode);

                    // Optimization: rotate tree updated cached properties for node
                    node = node.ParentTreeNode;
                }

                if (delta == 2)
                {
                    if (node.LeftTreeNode.RightTreeNode.Height == node.LeftTreeNode.LeftTreeNode.Height + 1)
                        RotateTree(node.LeftTreeNode, node.LeftTreeNode.RightTreeNode);

                    RotateTree(node, node.LeftTreeNode);

                    // Optimization: rotate tree updated cached properties for node
                    node = node.ParentTreeNode;
                }

                node = node.ParentTreeNode;
            }
        }

        static void RotateTree(TreeNode root, TreeNode pivot)
        {
            if (pivot == root.LeftTreeNode)
                RightRotation(root, pivot);
            else
                LeftRotation(root, pivot);
        }

        static void RightRotation(TreeNode root, TreeNode pivot)
        {
            // --- Update child Pointers ---

            root.LeftTreeNode = pivot.RightTreeNode;
            pivot.RightTreeNode = root;


            // --- Update Parent pointers ---

            pivot.ParentTreeNode = root.ParentTreeNode;
            root.ParentTreeNode = pivot;
            root.LeftTreeNode.ParentTreeNode = root;


            // --- Redirect grandparent to new child ---

            if (pivot.ParentTreeNode != null)
            {
                if (pivot.ParentTreeNode.LeftTreeNode == root)
                    pivot.ParentTreeNode.LeftTreeNode = pivot;
                else
                    pivot.ParentTreeNode.RightTreeNode = pivot;
            }


            // --- Update caches ---

            UpdateCachedNodeProperties(root);
            UpdateCachedNodeProperties(pivot);
        }

        static void LeftRotation(TreeNode root, TreeNode pivot)
        {
            // --- Update child Pointers ---

            root.RightTreeNode = pivot.LeftTreeNode;
            pivot.LeftTreeNode = root;


            // --- Update Parent pointers ---

            pivot.ParentTreeNode = root.ParentTreeNode;
            root.ParentTreeNode = pivot;
            root.RightTreeNode.ParentTreeNode = root;


            // --- Redirect grandparent to new child ---

            if (pivot.ParentTreeNode != null)
            {
                if (pivot.ParentTreeNode.LeftTreeNode == root)
                    pivot.ParentTreeNode.LeftTreeNode = pivot;
                else
                    pivot.ParentTreeNode.RightTreeNode = pivot;
            }


            // --- Update caches ---

            UpdateCachedNodeProperties(root);
            UpdateCachedNodeProperties(pivot);
        }

        static void UpdateCachedNodeProperties(TreeNode node)
        {
            node.Height = Math.Max(node.LeftTreeNode.Height, node.RightTreeNode.Height) + 1;
            node.LeftmostLeafNode = node.LeftTreeNode.LeftmostLeafNode;
            node.RightmostLeafNode = node.RightTreeNode.RightmostLeafNode;
        }

        #endregion


        #region Search

        /// <summary>
        /// Search the BinarySearchLinkedList to find two nodes bracketing a given value.
        /// </summary>
        /// <param name="searchValue">The vlaue for which to find bracketing nodes.</param>
        /// <param name="comparator">A delegate comparing searchValue to two adjacent node data.</param>
        /// <param name="left">The left bracketing node according to the Comparator delegate.</param>
        /// <param name="right">The right bracketing node according to the Comparator delegate.</param>
        public void BinarySearch(T searchValue,
            BinarySearchComparator<T> comparator,
            out IBsllNode<T> left, out IBsllNode<T> right)
        {
            BinarySearch(searchValue,
                (t) => t,
                new Func<T, T, T, int>(comparator),
                out left, out right);
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
        /// <param name="left">The left bracketing node according to the Comparator delegate.</param>
        /// <param name="right">The right bracketing node according to the Comparator delegate.</param>
        public void BinarySearch<TSearch, TCompare>(
            TSearch searchValue,
            Func<T, TCompare> dataSelector,
            Func<TSearch, TCompare, TCompare, int> comparator,
            out IBsllNode<T> left, out IBsllNode<T> right)
        {
            if (null == dataSelector)
                throw new ArgumentNullException("dataSelector");
            if (null == comparator)
                throw new ArgumentNullException("comparator");

            TreeNode iter = root;
            TreeNode leftNode, rightNode;

            while (iter.ChildListNode == null)
            {
                leftNode = iter.LeftTreeNode.RightmostLeafNode;
                rightNode = iter.RightTreeNode.LeftmostLeafNode;

                var compResult = comparator(
                        searchValue,
                        dataSelector(leftNode.ChildListNode.Data),
                        dataSelector(rightNode.ChildListNode.Data));

                if (compResult > 0)
                {
                    iter = iter.RightTreeNode;
                }
                else if (compResult < 0)
                {
                    iter = iter.LeftTreeNode;
                }
                else // Found exact match
                {
                    left = leftNode.ChildListNode;
                    right = rightNode.ChildListNode;
                    return;
                }
            }

            left = iter.ChildListNode;
            right = iter.ChildListNode;
        }

        #endregion
    }

    /// <summary>
    /// Comparison delegate for the BinarySearchLinkedList.BinarySearch method.
    /// </summary>
    /// <param name="searchValue">The vlaue for which to find bracketing nodes.</param>
    /// <param name="leftValue">The left bracketing value.</param>
    /// <param name="rightValue">The right bracketing value.</param>
    /// <returns>A negative value, if searchValue is 'closer to' leftValue.
    /// Zero, if searchValue is 'exactly between' leftValue and rightValue.
    /// A positive value, if searchValue is 'closer to' rightValue.</returns>
    public delegate int BinarySearchComparator<T>(T searchValue, T leftValue, T rightValue);

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

        IBsllNode<T> PrevNode { get; }
        IBsllNode<T> NextNode { get; }
    }
}

#endif