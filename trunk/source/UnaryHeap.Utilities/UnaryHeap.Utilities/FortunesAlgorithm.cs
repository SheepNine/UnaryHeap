#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.Misc;

namespace UnaryHeap.Algorithms
{
    public static class FortunesAlgorithm
    {
        public static Graph2D ComputeDelanuayTriangulation(IEnumerable<Point2D> sites)
        {
            var result = new Graph2D(false);

            var siteEvents = new PriorityQueue<Circle2D>(
                sites.Select(site => new Circle2D(site)), new CircleBottomComparer());

            var topmostSites = new List<Point2D>();
            topmostSites.Add(siteEvents.Dequeue().Center);

            while (siteEvents.Peek().Center.Y == topmostSites[0].Y)
                topmostSites.Add(siteEvents.Dequeue().Center);

            for (int i = 0; i < topmostSites.Count; i++)
            {
                result.AddVertex(topmostSites[i]);

                if (i > 0)
                    result.AddEdge(topmostSites[i], topmostSites[i - 1]);
            }

            var beachLine = new BeachLine(topmostSites);

            while (true)
            {
                if (siteEvents.IsEmpty && beachLine.circleEvents.IsEmpty)
                {
                    break;
                }
                else if (siteEvents.IsEmpty)
                {
                    beachLine.RemoveArc(beachLine.circleEvents.Dequeue().Arc, result);
                }
                else if (beachLine.circleEvents.IsEmpty)
                {
                    beachLine.AddSite(siteEvents.Dequeue().Center, result);
                }
                else
                {
                    var site = siteEvents.Peek();
                    var circle = beachLine.circleEvents.Peek();

                    if (CircleBottomComparer.CompareBottoms(site, circle.SqueezePoint) == -1)
                        beachLine.AddSite(siteEvents.Dequeue().Center, result);
                    else
                        beachLine.RemoveArc(beachLine.circleEvents.Dequeue().Arc, result);
                }

                while (false == beachLine.circleEvents.IsEmpty && beachLine.circleEvents.Peek().IsStale)
                    beachLine.circleEvents.Dequeue();
            }

            return result;
        }

        class CircleEvent : IComparable<CircleEvent>
        {
            public IBsllNode<BeachArc> Arc;
            public Circle2D SqueezePoint;

            public CircleEvent(IBsllNode<BeachArc> arc, Circle2D squeezePoint)
            {
                Arc = arc;
                SqueezePoint = squeezePoint;
            }

            public bool IsStale
            {
                get { return Arc.Data.SqueezePoint != SqueezePoint; }
            }

            public int CompareTo(CircleEvent other)
            {
                return CircleBottomComparer.CompareBottoms(this.SqueezePoint, other.SqueezePoint);
            }
        }

        class BeachLine
        {
            BinarySearchLinkedList<BeachArc> arcs;
            public PriorityQueue<CircleEvent> circleEvents;

            public BeachLine(IEnumerable<Point2D> initialSites)
            {
                arcs = new BinarySearchLinkedList<BeachArc>(initialSites.Select(site => new BeachArc(site)));
                circleEvents = new PriorityQueue<CircleEvent>();
            }

            public void AddSite(Point2D site, Graph2D delaunay)
            {
                IBsllNode<BeachArc> left, right;
                arcs.BinarySearch(site, (arc) => arc.Site, CompareArcs, out left, out right);

                delaunay.AddVertex(site);

                IBsllNode<BeachArc> newArc;
                if (left == right)
                {
                    var node = left;

                    DeinitCircleEvent(node);

                    delaunay.AddEdge(node.Data.Site, site);

                    newArc = arcs.InsertAfter(node, new BeachArc(site));
                    arcs.InsertAfter(newArc, new BeachArc(node.Data.Site));
                }
                else
                {
                    DeinitCircleEvent(left);
                    DeinitCircleEvent(right);

                    delaunay.AddEdge(left.Data.Site, site);
                    delaunay.AddEdge(site, right.Data.Site);

                    newArc = arcs.InsertAfter(left, new BeachArc(site));
                }

                InitCircleEvent(newArc.PrevNode);
                InitCircleEvent(newArc.NextNode);
            }

            public void RemoveArc(IBsllNode<BeachArc> arcNode, Graph2D delaunay)
            {
                var left = arcNode.PrevNode;
                var right = arcNode.NextNode;

                delaunay.AddEdge(left.Data.Site, right.Data.Site);

                DeinitCircleEvent(left);
                DeinitCircleEvent(right);

                arcs.Delete(arcNode);

                InitCircleEvent(left);
                InitCircleEvent(right);
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
                circleEvents.Enqueue(new CircleEvent(arcNode, circumcircle));
            }

            static void DeinitCircleEvent(IBsllNode<BeachArc> node)
            {
                if (null == node)
                    return;

                node.Data.SqueezePoint = null;
            }


            static int CompareArcs(Point2D s, Point2D a, Point2D b)
            {
                // --- A,B on same Y : intercept is halfway between them ---

                if (a.Y == b.Y)
                    return s.X.CompareTo((a.X + b.X) / 2);


                // --- If a site is on the directrix, it is  ---

                if (a.Y == s.Y)
                    return s.X.CompareTo(a.X);
                if (b.Y == s.Y)
                    return s.X.CompareTo(b.X);


                // --- Check that the site is on the backside of the  ---

                var pDiff = Parabola.FromLocusDirectrix(a, s.Y) - Parabola.FromLocusDirectrix(b, s.Y);

                if (pDiff.EvaluateDerivative(s.X) < Rational.Zero)
                    return -pDiff.A.Sign;


                // --- Evaluate parabolas to determine answer

                return pDiff.Evaulate(s.X).Sign;
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
        
        class Parabola
        {
            public Rational A { get; private set; }
            public Rational B { get; private set; }
            public Rational C { get; private set; }

            public Parabola(Rational a, Rational b, Rational c)
            {
                A = a;
                B = b;
                C = c;
            }

            public static Parabola FromLocusDirectrix(Point2D locus, Rational directrixY)
            {
                var d = 2 * (locus.Y - directrixY);

                var a = 1;
                var b = -2 * locus.X;
                var c = (locus.X.Squared + locus.Y.Squared - directrixY * directrixY);

                return new Parabola(a / d, b / d, c / d);
            }

            public static Parabola operator -(Parabola p1, Parabola p2)
            {
                return new Parabola(p1.A - p2.A, p1.B - p2.B, p1.C - p2.C);
            }

            public Rational Evaulate(Rational x)
            {
                return C + x * (B + x * A);
            }

            public Rational EvaluateDerivative(Rational x)
            {
                return B + 2 * A * x;
            }
        }
    }
}

#endif