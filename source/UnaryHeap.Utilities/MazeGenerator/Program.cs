using System;
using System.IO;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;

namespace MazeGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            if (2 == args.Length)
            {
                return Run(
                    new VoronoiMazeLayout(int.Parse(args[0])),
                    new BiggestWallEdgeWeightAssignment(),
                    args[1]);
            }
            else if (3 == args.Length)
            {
                return Run(
                    new VoronoiMazeLayout(int.Parse(args[0]), int.Parse(args[1])),
                    new BiggestWallEdgeWeightAssignment(),
                    args[2]);
            }
            else
            {
                Console.Error.WriteLine("Incorrect number of arguments.");
                return 1;
            }
        }

        static int Run(
            IMazeLayout layout, IEdgeWeightAssignment edgeWeights, string outputFilename)
        {
            Graph2D physicalGraph, logicalGraph;
            layout.Generate(out physicalGraph, out logicalGraph);

            return Run(physicalGraph, logicalGraph, edgeWeights, outputFilename);
        }

        static int Run(
            Graph2D physicalGraph, Graph2D logicalGraph,
            IEdgeWeightAssignment edgeWeights, string outputFilename)
        {
            MazeConnector.ConnectRooms(
                logicalGraph, physicalGraph, edgeWeights, true, false);

            using (var output = File.CreateText(outputFilename))
                MazeWriter.WriteMaze(output, physicalGraph);

            return 0;
        }
    }

    interface IMazeLayout
    {
        void Generate(out Graph2D physicalGraph, out Graph2D logicalGraph);
    }

    class LatticeMazeLayout : IMazeLayout
    {
        int size;

        public LatticeMazeLayout(int size)
        {
            this.size = size;
        }

        public void Generate(out Graph2D physicalGraph, out Graph2D logicalGraph)
        {
            physicalGraph = new Graph2D(false);
            logicalGraph = new Graph2D(false);

            foreach (var y in Enumerable.Range(0, size + 1))
                foreach (var x in Enumerable.Range(0, size + 1))
                {
                    physicalGraph.AddVertex(new Point2D(x, y));

                    if (x > 0)
                        physicalGraph.AddEdge(new Point2D(x, y), new Point2D(x - 1, y));
                    if (y > 0)
                        physicalGraph.AddEdge(new Point2D(x, y), new Point2D(x, y - 1));
                }

            var oneHalf = new Rational(1, 2);

            foreach (var y in Enumerable.Range(0, size))
                foreach (var x in Enumerable.Range(0, size))
                {
                    logicalGraph.AddVertex(new Point2D(x + oneHalf, y + oneHalf));

                    if (x > 0)
                    {
                        logicalGraph.AddEdge(
                            new Point2D(x + oneHalf, y + oneHalf),
                            new Point2D(x - oneHalf, y + oneHalf));
                        logicalGraph.SetDualEdge(
                            new Point2D(x + oneHalf, y + oneHalf),
                            new Point2D(x - oneHalf, y + oneHalf),
                            new Point2D(x, y),
                            new Point2D(x, y + 1));
                    }
                    if (y > 0)
                    {
                        logicalGraph.AddEdge(
                            new Point2D(x + oneHalf, y + oneHalf),
                            new Point2D(x + oneHalf, y - oneHalf));
                        logicalGraph.SetDualEdge(
                            new Point2D(x + oneHalf, y + oneHalf),
                            new Point2D(x + oneHalf, y - oneHalf),
                            new Point2D(x, y),
                            new Point2D(x + 1, y));
                    }
                }
        }
    }

    class VoronoiMazeLayout : IMazeLayout
    {
        int size;
        int? seed;

        public VoronoiMazeLayout(int size, int? seed = null)
        {
            this.size = size;
            this.seed = seed;
        }

        public void Generate(out Graph2D physicalGraph, out Graph2D logicalGraph)
        {
            var sites = Point2D.GenerateRandomPoints(size, seed);

            var listener = new MazeListener();
            FortunesAlgorithm.Execute(sites, listener);

            physicalGraph = listener.PhysicalGraph;
            logicalGraph = listener.LogicalGraph;

            MakeShortWallsImpassable(
                logicalGraph, physicalGraph, new Rational(size, 100), false);
        }

        static void MakeShortWallsImpassable(
            Graph2D logicalGraph, Graph2D physicalGraph, Rational minLength, bool highlight)
        {
            var minLengthSquared = minLength.Squared;

            foreach (var edge in logicalGraph.Edges.ToArray())
            {
                var dual = logicalGraph.GetDualEdge(edge.Item1, edge.Item2);

                if (Point2D.Quadrance(dual.Item1, dual.Item2) < minLengthSquared)
                {
                    logicalGraph.RemoveEdge(edge.Item1, edge.Item2);

                    if (highlight)
                        physicalGraph.SetEdgeMetadatum(
                            dual.Item1, dual.Item2, "color", "#80FF80");
                }
            }
        }
    }
}
