using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.DataType;

namespace UnaryHeap.Algorithms
{
    /// <summary>
    /// Provides an implementation of Fortune's algorithm for computing the Delaunay
    /// triangulation and the Voronoi diagram of a set of points.
    /// </summary>
    public static class FortunesAlgorithm
    {
        /// <summary>
        /// Calculate which of two adjacent beach line arcs is intersected by the vertical
        /// line produced by a site.
        /// </summary>
        /// <param name="site">The site projecting the intersection line.</param>
        /// <param name="arcAFocus">The site from which the first arc is produced.</param>
        /// <param name="arcBFocus">The site from which the second arc is produced.</param>
        /// <returns>-1, if arcA is intersected. 1, if arcB is intersected.
        /// 0, if the intersection line contains the intersection point of the arcs.</returns>
        public static int DetermineBeachLineArcIntersected(
            Point2D site, Point2D arcAFocus, Point2D arcBFocus)
        {
            if (null == site)
                throw new ArgumentNullException("site");
            if (null == arcAFocus)
                throw new ArgumentNullException("arcAFocus");
            if (null == arcBFocus)
                throw new ArgumentNullException("arcBFocus");

            if (arcAFocus.Y == arcBFocus.Y)
            {
                if (arcAFocus.X >= arcBFocus.X)
                    throw new ArgumentException("Arc foci are not ordered correctly.");

                return site.X.CompareTo((arcAFocus.X + arcBFocus.X) / 2);
            }

            if (site.Y == arcAFocus.Y)
                return site.X.CompareTo(arcAFocus.X);

            if (site.Y == arcBFocus.Y)
                return site.X.CompareTo(arcBFocus.X);

            var difference = Parabola.Difference(
                Parabola.FromFocusDirectrix(arcAFocus, site.Y),
                Parabola.FromFocusDirectrix(arcBFocus, site.Y));

            if (difference.EvaluateDerivative(site.X) < 0)
                return -difference.A.Sign;

            return difference.Evaulate(site.X).Sign;
        }

        /// <summary>
        /// Adds points to a set of points covering a square area which will guarantee
        /// that the Voronoi vertices for the point set do not exceed the convex hull
        /// of the augmented set.
        /// </summary>
        /// <param name="points">The points to which to add a boundary.</param>
        public static Point2D[] AddBoundarySites(IEnumerable<Point2D> points)
        {
            if (null == points)
                throw new ArgumentNullException("points");

            var boundary = Orthotope2D.FromPoints(points).GetScaled(new Rational(5, 4));

            if (boundary.X.Size != boundary.Y.Size)
                throw new ArgumentException("Input points do not cover a square area.");

            var result = points.ToList();

            for (int i = 0; i < 5; i++)
            {
                var coeff = new Rational(2 * i + 1, 10);
                result.Add(new Point2D(
                    boundary.X.Min + coeff * boundary.X.Size, boundary.Y.Min));
                result.Add(new Point2D(
                    boundary.X.Min + coeff * boundary.X.Size, boundary.Y.Max));
                result.Add(new Point2D(
                    boundary.X.Min, boundary.Y.Min + coeff * boundary.Y.Size));
                result.Add(new Point2D(
                    boundary.X.Max, boundary.Y.Min + coeff * boundary.Y.Size));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Run Fortune's algorithm over a set of sites.
        /// </summary>
        /// <param name="sites">The input sites to the algorithm.</param>
        /// <param name="listener">The listener </param>
        public static void Execute(
            IEnumerable<Point2D> sites, IFortunesAlgorithmListener listener)
        {
            if (null == sites)
                throw new ArgumentNullException("sites");
            if (null == listener)
                throw new ArgumentNullException("listener");

            var cachedSites = sites.ToList();
            var uniqueSites = new SortedSet<Point2D>(cachedSites, new Point2DComparer());

            if (uniqueSites.Contains(null))
                throw new ArgumentNullException("sites");
            if (uniqueSites.Count < cachedSites.Count)
                throw new ArgumentException(
                    "Enumerable contains one or more duplicate points.", "sites");
            if (uniqueSites.Count < 3)
                throw new ArgumentException(
                    "Input sites are colinear.", "sites");

            var siteEvents = new PriorityQueue<Circle2D>(
                sites.Select(site => new Circle2D(site)), new CircleBottomComparer());

            var topmostSites = RemoveTopmostSitesFromQueue(siteEvents);

            if (siteEvents.IsEmpty)
                throw new ArgumentException(
                    "Input sites are colinear.", "sites");

            var beachLine = new BeachLine(topmostSites, listener);
            while (true)
            {
                if (siteEvents.IsEmpty && beachLine.circleEvents.IsEmpty)
                {
                    break;
                }
                else if (siteEvents.IsEmpty)
                {
                    beachLine.RemoveArc(beachLine.circleEvents.Dequeue().Arc);
                }
                else if (beachLine.circleEvents.IsEmpty)
                {
                    beachLine.AddSite(siteEvents.Dequeue().Center);
                }
                else
                {
                    var site = siteEvents.Peek();
                    var circle = beachLine.circleEvents.Peek();

                    if (-1 == CircleBottomComparer.CompareCircles(
                        site, circle.Arc.Data.SqueezePoint))
                        beachLine.AddSite(siteEvents.Dequeue().Center);
                    else
                        beachLine.RemoveArc(beachLine.circleEvents.Dequeue().Arc);
                }
            }

            if (false == beachLine.CircleEventHandled)
                throw new ArgumentException("Input sites are colinear.", "sites");

            beachLine.EmitRays();
            listener.AlgorithmComplete();
        }

        private static List<Point2D> RemoveTopmostSitesFromQueue(
            PriorityQueue<Circle2D> siteEvents)
        {
            var topmostSites = new List<Point2D>();
            topmostSites.Add(siteEvents.Dequeue().Center);

            while (!siteEvents.IsEmpty && siteEvents.Peek().Center.Y == topmostSites[0].Y)
                topmostSites.Add(siteEvents.Dequeue().Center);

            return topmostSites;
        }


        class CircleEvent : IComparable<CircleEvent>
        {
            public IBsllNode<BeachArc> Arc;
            Circle2D initialSqueezePoint;

            public CircleEvent(IBsllNode<BeachArc> arc)
            {
                Arc = arc;
                initialSqueezePoint = arc.Data.SqueezePoint;
            }

            public bool IsStale
            {
                get { return Arc.Data.SqueezePoint != initialSqueezePoint; }
            }

            public int CompareTo(CircleEvent other)
            {
                if (null == other)
                    return -1;

                return CircleBottomComparer.CompareCircles(
                    this.initialSqueezePoint, other.initialSqueezePoint);
            }
        }

        class BeachArc
        {
            public Point2D Site;
            public Circle2D SqueezePoint;

            public BeachArc(Point2D site)
            {
                Site = site;
                SqueezePoint = null;
            }
        }

        class BeachLine
        {
            IComparer<Point2D> pointComparer = new Point2DComparer();
            BinarySearchLinkedList<BeachArc> arcs;
            public PriorityQueue<CircleEvent> circleEvents;
            IFortunesAlgorithmListener listener;
            SortedSet<Point2D> voronoiVertices;
            SortedDictionary<Point2D, SortedDictionary<Point2D, Point2D>> voronoiRays;
            public bool CircleEventHandled = false;

            public BeachLine(List<Point2D> initialSites, IFortunesAlgorithmListener listener)
            {
                arcs = new BinarySearchLinkedList<BeachArc>(
                    initialSites.Select(site => new BeachArc(site)));
                circleEvents = new PriorityQueue<CircleEvent>();
                this.listener = listener;

                InitializeListener(initialSites);

                voronoiVertices = new SortedSet<Point2D>(pointComparer);
                voronoiRays = new SortedDictionary<Point2D, SortedDictionary<Point2D, Point2D>>(
                    pointComparer);
            }

            void InitializeListener(List<Point2D> initialSites)
            {
                for (int i = 0; i < initialSites.Count; i++)
                    listener.EmitDelaunayVertex(initialSites[i]);
            }

            public void AddSite(Point2D site)
            {
                var searchResults = arcs.BinarySearch(site, ArcsBinarySearchDelegate);

                listener.EmitDelaunayVertex(site);

                IBsllNode<BeachArc> newArc;
                if (1 == searchResults.Length)
                {
                    var node = searchResults[0];

                    DeinitCircleEvent(node);

                    newArc = arcs.InsertAfter(node, new BeachArc(site));
                    arcs.InsertAfter(newArc, new BeachArc(node.Data.Site));
                }
                else
                {
                    var left = searchResults[0];
                    var right = searchResults[1];

                    DeinitCircleEvent(left);
                    DeinitCircleEvent(right);

                    HandleVoronoiHalfEdges(left.Data.Site, site, right.Data.Site);

                    newArc = arcs.InsertAfter(left, new BeachArc(site));
                }

                InitCircleEvent(newArc.PrevNode);
                InitCircleEvent(newArc.NextNode);

                RemoveStaleCircleEvents();
            }

            static int ArcsBinarySearchDelegate(
                Point2D searchValue, BeachArc predValue, BeachArc succValue)
            {
                return DetermineBeachLineArcIntersected(
                    searchValue, predValue.Site, succValue.Site);
            }

            public void RemoveArc(IBsllNode<BeachArc> arcNode)
            {
                var left = arcNode.PrevNode;
                var right = arcNode.NextNode;

                HandleVoronoiHalfEdges(left.Data.Site, arcNode.Data.Site, right.Data.Site);

                DeinitCircleEvent(left);
                DeinitCircleEvent(right);

                arcs.Delete(arcNode);

                InitCircleEvent(left);
                InitCircleEvent(right);

                RemoveStaleCircleEvents();
            }

            void InitCircleEvent(IBsllNode<BeachArc> arcNode)
            {
                if (null == arcNode || null == arcNode.PrevNode || null == arcNode.NextNode)
                    return;

                var circumcircle = Circle2D.Circumcircle(
                    arcNode.PrevNode.Data.Site, arcNode.Data.Site, arcNode.NextNode.Data.Site);

                if (null == circumcircle)
                    return;

                var siteA = arcNode.PrevNode.Data.Site;
                var siteB = arcNode.Data.Site;
                var siteC = arcNode.NextNode.Data.Site;

                var dxab = siteA.X - siteB.X;
                var dyab = siteA.Y - siteB.Y;
                var dxbc = siteC.X - siteB.X;
                var dybc = siteC.Y - siteB.Y;

                var det = dxab * dybc - dxbc * dyab;

                if (det <= 0)
                    return;

                arcNode.Data.SqueezePoint = circumcircle;
                circleEvents.Enqueue(new CircleEvent(arcNode));
            }

            static void DeinitCircleEvent(IBsllNode<BeachArc> node)
            {
                if (null == node)
                    return;

                node.Data.SqueezePoint = null;
            }

            void RemoveStaleCircleEvents()
            {
                while (false == circleEvents.IsEmpty && circleEvents.Peek().IsStale)
                    circleEvents.Dequeue();
            }

            void HandleVoronoiHalfEdges(Point2D site1, Point2D site2, Point2D site3)
            {
                CircleEventHandled = true;
                var cc = Point2D.Circumcenter(site1, site2, site3);

                if (!voronoiVertices.Contains(cc))
                {
                    voronoiVertices.Add(cc);
                    listener.EmitVoronoiVertex(cc);
                }

                RecordHalfEdge(site1, site2, cc);
                RecordHalfEdge(site2, site3, cc);
                RecordHalfEdge(site3, site1, cc);
            }

            void RecordHalfEdge(Point2D siteA, Point2D siteB, Point2D endpoint)
            {
                if (voronoiRays.ContainsKey(siteA) && voronoiRays[siteA].ContainsKey(siteB))
                {
                    var otherEndpoint = voronoiRays[siteA][siteB];
                    voronoiRays[siteA].Remove(siteB);
                    listener.EmitDualEdges(siteA, siteB, endpoint, otherEndpoint);
                }
                else if (voronoiRays.ContainsKey(siteB) &&
                    voronoiRays[siteB].ContainsKey(siteA))
                {
                    var otherEndpoint = voronoiRays[siteB][siteA];
                    voronoiRays[siteB].Remove(siteA);
                    listener.EmitDualEdges(siteA, siteB, endpoint, otherEndpoint);
                }
                else
                {
                    if (false == voronoiRays.ContainsKey(siteA))
                        voronoiRays.Add(siteA,
                            new SortedDictionary<Point2D, Point2D>(pointComparer));

                    voronoiRays[siteA].Add(siteB, endpoint);
                }
            }

            public void EmitRays()
            {
                foreach (var entry in voronoiRays)
                {
                    var site1 = entry.Key;

                    foreach (var subEntry in entry.Value)
                    {
                        var site2 = subEntry.Key;
                        var p = subEntry.Value;

                        listener.EmitDualEdges(site1, site2, p, null);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Interface to listen for events in Fortune's algorithm and respond accordingly.
    /// </summary>
    public interface IFortunesAlgorithmListener
    {
        /// <summary>
        /// Called when Fortune's algorithm encounters a site.
        /// </summary>
        /// <param name="p">The location of the site.</param>
        void EmitDelaunayVertex(Point2D p);

        /// <summary>
        /// Called when Fortune's algorithm finds a Voronoi vertex. This is either
        /// at circle events or when a site event intersects the beachline at one
        /// of its intersection points.
        /// </summary>
        /// <param name="p">The location of the vertex.</param>
        void EmitVoronoiVertex(Point2D p);

        /// <summary>
        /// Called when Fortune's algorithm identifies a dual pair of
        /// Voronoi/Delaunay edges
        /// </summary>
        /// <param name="d1">The first endpoint of the Delaunay edge.</param>
        /// <param name="d2">The second endpoint of the Delaunay edge.</param>
        /// <param name="v1">The first endpoint of the Voronoi edge.</param>
        /// <param name="v2">The second endpoint of the Voronoi edge, or null,
        /// indicating that the Voronoi edge has one endpoint at infinity.
        /// v2 may also be equal to v1.</param>
        void EmitDualEdges(Point2D d1, Point2D d2, Point2D v1, Point2D v2);

        /// <summary>
        /// Called once all the vertices and edges of the Delaunay and Voronoi
        /// graphs have been enumerated.
        /// </summary>
        void AlgorithmComplete();
    }

#if INCLUDE_WORK_IN_PROGRESS
    /// <summary>
    /// Provides an implementation of the IFortunesAlgorithmListener interface
    /// which produces a Graph2D object of the resulting graphs.
    /// </summary>
    public class GraphFortunesAlgorithmListener : IFortunesAlgorithmListener
    {
        /// <summary>
        /// The Graph2D to which the output of Fortune's algorithm is recorded.
        /// </summary>
        public Graph2D Graph { get; private set; }
        string delaunayColor;
        string voronoiColor;

        /// <summary>
        /// Initializes a new instance of the GraphFortunesAlgorithmListener class.
        /// </summary>
        /// <param name="delaunayColor">
        /// The color to apply to Delaunay diagram vertices/edges.</param>
        /// <param name="voronoiColor">
        /// The color to apply to Voronoi diagram vertices/edges.</param>
        public GraphFortunesAlgorithmListener(string delaunayColor, string voronoiColor)
        {
            Graph = new Graph2D(false);
            this.delaunayColor = delaunayColor;
            this.voronoiColor = voronoiColor;
        }

        /// <summary>
        /// Called when Fortune's algorithm encounters a site.
        /// </summary>
        /// <param name="p">The location of the site.</param>
        public void EmitDelaunayVertex(Point2D p)
        {
            if (null == p)
                throw new ArgumentNullException("p");

            Graph.AddVertex(p);
            Graph.SetVertexMetadatum(p, "color", delaunayColor);
        }

        /// <summary>
        /// Called when Fortune's algorithm finds an edge between two input sites.
        /// </summary>
        /// <param name="p1">The first endpoint of the edge.</param>
        /// <param name="p2">The second endpoint of the edge.</param>
        public void EmitDelaunayEdge(Point2D p1, Point2D p2)
        {
            if (null == p1)
                throw new ArgumentNullException("p1");
            if (null == p2)
                throw new ArgumentNullException("p2");

            Graph.AddEdge(p1, p2);
            Graph.SetEdgeMetadatum(p1, p2, "color", delaunayColor);
        }

        /// <summary>
        /// Called when Fortune's algorithm finds a Voronoi vertex. This is either
        /// at circle events or when a site event intersects the beachline at one
        /// of its intersection points.
        /// </summary>
        /// <param name="p">The location of the vertex.</param>
        public void EmitVoronoiVertex(Point2D p)
        {
            if (null == p)
                throw new ArgumentNullException("p");

            Graph.AddVertex(p);
            Graph.SetVertexMetadatum(p, "color", voronoiColor);
        }

        /// <summary>
        /// Called when Fortune's algorithm finds two Voronoi half-edges that
        /// can be linked together.
        /// </summary>
        /// <param name="p1">The first endpoint of the edge.</param>
        /// <param name="p2">The second endpoint of the edge.</param>
        /// <param name="site1">The site on one side of the edge.</param>
        /// <param name="site2">The site on the other side of the edge.</param>
        public void EmitVoronoiEdge(Point2D p1, Point2D p2, Point2D site1, Point2D site2)
        {
            if (null == p1)
                throw new ArgumentNullException("p1");
            if (null == p2)
                throw new ArgumentNullException("p2");
            if (null == site1)
                throw new ArgumentNullException("site1");
            if (null == site2)
                throw new ArgumentNullException("site2");

            if (false == p1.Equals(p2))
            {
                Graph.AddEdge(p1, p2);
                Graph.SetEdgeMetadatum(p1, p2, "color", voronoiColor);
            }
        }

        /// <summary>
        /// Called when Fortune's algorithm finds a Voronoi edge with one endpoint
        /// at infinity.
        /// </summary>
        /// <param name="p">The finite endpoint of the edge.</param>
        /// <param name="site1">The site on one side of the edge.</param>
        /// <param name="site2">The site on the other side of the edge.</param>
        public void EmitVoronoiRay(Point2D p, Point2D site1, Point2D site2)
        {
        }
    }
#endif
}