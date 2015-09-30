using System;
using System.IO;
using UnaryHeap.Algorithms;
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

            HeightMapMazeConnector.ConnectRooms(
                listener.LogicalGraph, listener.PhysicalGraph, new XGradient());

            using (var output = File.CreateText(outputFilename))
                MazeWriter.WriteMaze(output, listener.PhysicalGraph);
            
            return 0;
        }
    }
}
