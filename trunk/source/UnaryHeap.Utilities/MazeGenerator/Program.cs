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
}
