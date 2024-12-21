using System;
using System.Globalization;
using UnaryHeap.DataType;

namespace UnaryHeap.Graph
{
    /// <summary>
    /// Contains extension methods for the Graph2D class.
    /// </summary>
    public static partial class Graph2DExtensions
    {
        /// <summary>
        /// The key used for the DualEdge metadata methods.
        /// </summary>
        public const string DualMetadataKey = "dual";

        /// <summary>
        /// Removes a dual edge record (if present) from the specified edge.
        /// </summary>
        /// <param name="this">The Graph2D from which to remove the metadata.</param>
        /// <param name="p1">The first edge vertex.</param>
        /// <param name="p2">The second edge vertex.</param>
        public static void UnsetDualEdge(this Graph2D @this,
            Point2D p1, Point2D p2)
        {
            ArgumentNullException.ThrowIfNull(@this);

            @this.UnsetEdgeMetadatum(p1, p2, DualMetadataKey);
        }

        /// <summary>
        /// Record a pair of dual edge vertices for a given edge.
        /// </summary>
        /// <param name="this">The Graph2D for which to record the data.</param>
        /// <param name="p1">The first edge vertex.</param>
        /// <param name="p2">The second edge vertex.</param>
        /// <param name="d1">The first dual edge vertex.</param>
        /// <param name="d2">The second dual edge vertex.</param>
        public static void SetDualEdge(this Graph2D @this,
            Point2D p1, Point2D p2,
            Point2D d1, Point2D d2)
        {
            ArgumentNullException.ThrowIfNull(@this);

            @this.SetEdgeMetadatum(p1, p2, DualMetadataKey,
                string.Format(CultureInfo.InvariantCulture, "{0};{1}", d1, d2));
        }

        /// <summary>
        /// Retrieve a recorded pair of dual edge vertices for a given edge.
        /// </summary>
        /// <param name="this">The Graph2D from which to retrieve the data.</param>
        /// <param name="p1">The first edge vertex.</param>
        /// <param name="p2">The second edge vertex.</param>
        /// <returns>The stored dual edge vertices.</returns>
        public static Tuple<Point2D, Point2D> GetDualEdge(this Graph2D @this,
            Point2D p1, Point2D p2)
        {
            ArgumentNullException.ThrowIfNull(@this);

            var data = @this.GetEdgeMetadatum(p1, p2, DualMetadataKey);

            if (null == data)
                throw new InvalidOperationException(
                    "Graph has no 'dual' metadata for the specified edge.");

            var tokens = data.Split(';');

            if (tokens.Length != 2)
                throw new InvalidOperationException(
                    "'Dual' metadata for edge has incorrect number of arguments.");

            return Tuple.Create(Point2D.Parse(tokens[0]), Point2D.Parse(tokens[1]));
        }
    }
}
