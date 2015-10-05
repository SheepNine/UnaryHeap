﻿using System;
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
                return Run(int.Parse(args[0]), args[1]);
            }
            else if (3 == args.Length)
            {
                return Run(int.Parse(args[0]), int.Parse(args[1]), args[2]);
            }
            else
            {
                Console.Error.WriteLine("Incorrect number of arguments.");
                return 1;
            }
        }

        static int Run(int size, string outputFilename)
        {
            return Run(Point2D.GenerateRandomPoints(size), outputFilename);
        }

        static int Run(int size, int seed, string outputFilename)
        {
            return Run(Point2D.GenerateRandomPoints(size, seed), outputFilename);
        }

        static int Run(Point2D[] sites, string outputFilename)
        {
            var listener = new MazeListener();
            FortunesAlgorithm.Execute(sites, listener);

            MakeShortWallsImpassable(
                listener.LogicalGraph, listener.PhysicalGraph,
                new Rational(sites.Length, 100), false);

            HeightMapMazeConnector.ConnectRooms(
                listener.LogicalGraph, listener.PhysicalGraph,
                new BiggestWallEdgeWeightAssignment(), true);

            using (var output = File.CreateText(outputFilename))
                MazeWriter.WriteMaze(output, listener.PhysicalGraph);

            return 0;
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

        static void MakeSquareLatticeGraphs(
            int size, out Graph2D physicalGraph, out Graph2D logicalGraph)
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
}
