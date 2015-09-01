#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.Misc;

namespace UnaryHeap.Algorithms
{
    /// <summary>
    /// Provides an implementation of Fortune's algorithm for computing the Delaunay triangulation
    /// and the Voronoi diagram of a set of points.
    /// </summary>
    public static class FortunesAlgorithm
    {
        /// <summary>
        /// Run Fortune's algorithm over a set of sites.
        /// </summary>
        /// <param name="sites">The input sites to the algorithm.</param>
        /// <param name="listener">The listener </param>
        public static void ComputeDelanuayTriangulation(
            IEnumerable<Point2D> sites, IFortunesAlgorithmListener listener)
        {
            if (null == sites)
                throw new ArgumentNullException("sites");
            if (null == listener)
                throw new ArgumentNullException("listener");

            var siteEvents = new PriorityQueue<Circle2D>(
                sites.Select(site => new Circle2D(site)), new CircleBottomComparer());

            var topmostSites = RemoveTopmostSitesFromQueue(siteEvents);

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

                    if (CircleBottomComparer.CompareCircles(site, circle.Arc.Data.SqueezePoint) == -1)
                        beachLine.AddSite(siteEvents.Dequeue().Center);
                    else
                        beachLine.RemoveArc(beachLine.circleEvents.Dequeue().Arc);
                }
            }
        }

        private static List<Point2D> RemoveTopmostSitesFromQueue(PriorityQueue<Circle2D> siteEvents)
        {
            var topmostSites = new List<Point2D>();
            topmostSites.Add(siteEvents.Dequeue().Center);

            while (siteEvents.Peek().Center.Y == topmostSites[0].Y)
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
                {
                    listener.EmitDelaunayVertex(initialSites[i]);

                    if (i > 0)
                        listener.EmitDelaunayEdge(initialSites[i], initialSites[i - 1]);
                }
            }

            public void AddSite(Point2D site)
            {
                var searchResults = arcs.BinarySearch(site, CompareArcs);

                listener.EmitDelaunayVertex(site);

                IBsllNode<BeachArc> newArc;
                if (1 == searchResults.Length)
                {
                    var node = searchResults[0];

                    DeinitCircleEvent(node);

                    listener.EmitDelaunayEdge(node.Data.Site, site);

                    newArc = arcs.InsertAfter(node, new BeachArc(site));
                    arcs.InsertAfter(newArc, new BeachArc(node.Data.Site));
                }
                else
                {
                    var left = searchResults[0];
                    var right = searchResults[1];

                    DeinitCircleEvent(left);
                    DeinitCircleEvent(right);

                    listener.EmitDelaunayEdge(left.Data.Site, site);
                    listener.EmitDelaunayEdge(site, right.Data.Site);
                    HandleVoronoiHalfEdges(left.Data.Site, site, right.Data.Site);

                    newArc = arcs.InsertAfter(left, new BeachArc(site));
                }

                InitCircleEvent(newArc.PrevNode);
                InitCircleEvent(newArc.NextNode);

                RemoveStaleCircleEvents();
            }

            public void RemoveArc(IBsllNode<BeachArc> arcNode)
            {
                var left = arcNode.PrevNode;
                var right = arcNode.NextNode;

                listener.EmitDelaunayEdge(left.Data.Site, right.Data.Site);
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


            static int CompareArcs(Point2D s, BeachArc a, BeachArc b)
            {
                // --- A,B on same Y : intercept is halfway between them ---

                if (a.Site.Y == b.Site.Y)
                    return s.X.CompareTo((a.Site.X + b.Site.X) / 2);


                // --- If a site is on the directrix, it is  ---

                if (a.Site.Y == s.Y)
                    return s.X.CompareTo(a.Site.X);
                if (b.Site.Y == s.Y)
                    return s.X.CompareTo(b.Site.X);


                // --- Check that the site is on the backside of the  ---

                var pDiff = Parabola.Difference(
                    Parabola.FromFocusDirectrix(a.Site, s.Y),
                    Parabola.FromFocusDirectrix(b.Site, s.Y));

                if (pDiff.EvaluateDerivative(s.X) < Rational.Zero)
                    return -pDiff.A.Sign;


                // --- Evaluate parabolas to determine answer

                return pDiff.Evaulate(s.X).Sign;
            }

            void HandleVoronoiHalfEdges(Point2D site1, Point2D site2, Point2D site3)
            {
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

                    if (!endpoint.Equals(otherEndpoint))
                        listener.EmitVoronoiEdge(endpoint, otherEndpoint, siteA, siteB);

                    return;
                }

                if (voronoiRays.ContainsKey(siteB) && voronoiRays[siteB].ContainsKey(siteA))
                {
                    var otherEndpoint = voronoiRays[siteB][siteA];
                    voronoiRays[siteB].Remove(siteA);

                    if (!endpoint.Equals(otherEndpoint))
                        listener.EmitVoronoiEdge(endpoint, otherEndpoint, siteA, siteB);

                    return;
                }

                if (false == voronoiRays.ContainsKey(siteA))
                    voronoiRays.Add(siteA, new SortedDictionary<Point2D, Point2D>(pointComparer));

                voronoiRays[siteA].Add(siteB, endpoint);
            }
        }

        /// <summary>
        /// Generates a set of points randomly distributed in a square area.
        /// </summary>
        /// <param name="numPoints">The number of points to generate.</param>
        /// <param name="seed">The random number seed, or null to use the default seed.</param>
        /// <returns>A set of points randomly distributed in a square area.</returns>
        public static Point2D[] RandomPoints(int numPoints, int? seed = null)
        {
            var result = new List<Point2D>();
            var random = seed.HasValue ? new Random(seed.Value) : new Random();

            for (int i = 0; i < numPoints; i++)
                result.Add(new Point2D(i, random.Next(numPoints)));

            // --- Make it square ---
            result[0] = new Point2D(0, 0);
            result[numPoints - 1] = new Point2D(numPoints - 1, numPoints - 1);

            return result.ToArray();
        }

        /// <summary>
        /// Adds points to a set of points covering a square area which will guarantee
        /// that the vertices of the Voronoi vertices for the point set do not exceed
        /// the convex hull of the augmented set.
        /// </summary>
        /// <param name="points">The points to which to add a boundary.</param>
        public static Point2D[] AddBoundaryPointsToConstrainVoronoi(IEnumerable<Point2D> points)
        {
            var boundary = Orthotope2D.FromPoints(points).GetScaled(new Rational(5, 4));

            if (boundary.X.Size != boundary.Y.Size)
                throw new ArgumentException("Input points do not cover a square area.");

            var result = points.ToList();

            for (int i = 0; i < 5; i++)
            {
                var coeff = new Rational(2 * i + 1, 10);
                result.Add(new Point2D(boundary.X.Min + coeff * boundary.X.Size, boundary.Y.Min));
                result.Add(new Point2D(boundary.X.Min + coeff * boundary.X.Size, boundary.Y.Max));
                result.Add(new Point2D(boundary.X.Min, boundary.Y.Min + coeff * boundary.Y.Size));
                result.Add(new Point2D(boundary.X.Max, boundary.Y.Min + coeff * boundary.Y.Size));
            }

            return result.ToArray();
        }
    }

    /// <summary>
    /// Interface to listen for events in Fortune's algorithm and respond accordingly.
    /// </summary>
    public interface IFortunesAlgorithmListener
    {
        /// <summary>
        /// Called when Fortune's algorithm finds an edge between two input sites.
        /// </summary>
        /// <param name="p1">The first endpoint of the edge.</param>
        /// <param name="p2">The second endpoint of the edge.</param>
        void EmitDelaunayEdge(Point2D p1, Point2D p2);

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
        /// Called when Fortune's algorithm finds two Voronoi half-edges that
        /// can be linked together.
        /// </summary>
        /// <param name="p1">The first endpoint of the edge.</param>
        /// <param name="p2">The second endpoint of the edge.</param>
        /// <param name="site1">The site on one side of the edge.</param>
        /// <param name="site2">The site on the other side of the edge.</param>
        void EmitVoronoiEdge(Point2D p1, Point2D p2, Point2D site1, Point2D site2);
    }

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
        /// <param name="delaunayColor">The color to apply to Delaunay diagram vertices/edges.</param>
        /// <param name="voronoiColor">The color to apply to Voronoi diagram vertices/edges.</param>
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
    }
}

#endif