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
                    new MazeConnector(true, false),
                    args[1]);
            }
            else if (3 == args.Length)
            {
                return Run(
                    new VoronoiMazeLayout(int.Parse(args[0]), int.Parse(args[1])),
                    new BiggestWallEdgeWeightAssignment(),
                    new MazeConnector(true, false),
                    args[2]);
            }
            else
            {
                Console.Error.WriteLine("Incorrect number of arguments.");
                return 1;
            }
        }

        static int Run(
            IMazeLayout layout, IEdgeWeightAssignment edgeWeights,
            MazeConnector connector, string outputFilename)
        {
            Graph2D physicalGraph, logicalGraph;
            layout.Generate(out physicalGraph, out logicalGraph);

            return Run(physicalGraph, logicalGraph, connector, edgeWeights, outputFilename);
        }

        static int Run(
            Graph2D physicalGraph, Graph2D logicalGraph, MazeConnector connector,
            IEdgeWeightAssignment edgeWeights, string outputFilename)
        {
            connector.ConnectRooms(logicalGraph, physicalGraph, edgeWeights);

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
