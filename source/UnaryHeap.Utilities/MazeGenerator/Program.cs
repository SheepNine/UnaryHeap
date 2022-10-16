using System;
using System.IO;
using UnaryHeap.Graph;
using UnaryHeap.Utilities.D2;

namespace MazeGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                if (4 == args.Length)
                {
                    return Run(
                        ParseMazeLayout(args[0]),
                        ParseEdgeWeightAssignment(args[1]),
                        ParseMazeConnector(args[2]),
                        Path.GetFullPath(args[3])
                    );
                }
                else
                {
                    Console.Error.WriteLine("The syntax of the command is incorrect.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.Write("ERROR: ");
                Console.Error.WriteLine(ex.Message);
            }

            return 1;
        }

        #region Command-line Parameter Interpretation

        static IMazeLayout ParseMazeLayout(string token)
        {
            // Options are:
            // L:size
            // F:size:(hH):<seed>

            var tokens = token.Split(':');

            if (2 > tokens.Length)
                throw new ArgumentException("Incorrect maze layout token.");

            if (tokens[0] == "L")
            {
                if (2 != tokens.Length)
                    throw new ArgumentException("Incorrect maze layout token.");

                int size;
                if (false == int.TryParse(tokens[1], out size))
                    throw new ArgumentException("Incorrect maze layout token.");

                return new LatticeMazeLayout(size);
            }
            else if (tokens[0] == "F")
            {
                if (3 != tokens.Length && 4 != tokens.Length)
                    throw new ArgumentException("Incorrect maze layout token.");

                int size;
                if (false == int.TryParse(tokens[1], out size))
                    throw new ArgumentException("Incorrect maze layout token.");

                bool highlightShortEdges;

                if (tokens[2] == "h")
                    highlightShortEdges = false;
                else if (tokens[2] == "H")
                    highlightShortEdges = true;
                else
                    throw new ArgumentException("Incorrect maze layout token.!?");

                int? seed = null;
                if (4 == tokens.Length)
                {
                    int seedVal;
                    if (false == int.TryParse(tokens[3], out seedVal))
                        throw new ArgumentException("Incorrect maze layout token.");

                    seed = seedVal;
                }

                return new VoronoiMazeLayout(size, highlightShortEdges, seed);
            }
            else
            {
                throw new ArgumentException("Incorrect maze layout token.");
            }
        }

        static IEdgeWeightAssignment ParseEdgeWeightAssignment(string token)
        {
            var tokens = token.Split(':');

            switch (tokens[0])
            {
                case "B":
                    return new BiggestWallEdgeWeightAssignment();
                case "R":
                    {
                        if (2 < tokens.Length)
                            throw new ArgumentException("Incorrect edge weight token.");

                        int? seed = null;
                        if (2 == tokens.Length)
                        {
                            int seedVal;
                            if (false == int.TryParse(tokens[1], out seedVal))
                                throw new ArgumentException("Incorrect edge weight token.");

                            seed = seedVal;
                        }

                        return new RandomEdgeWeightAssignment(seed);
                    }
                case "H":
                    {
                        if (3 > tokens.Length)
                            throw new ArgumentException("Incorrect edge weight token.");

                        bool negateWeights;
                        if (tokens[1] == "n")
                            negateWeights = false;
                        else if (tokens[1] == "N")
                            negateWeights = true;
                        else
                            throw new ArgumentException("Incorrect edge weight token.");

                        IHeightMap heightMap;
                        switch (tokens[2])
                        {
                            case "X":
                                {
                                    if (3 < tokens.Length)
                                        throw new ArgumentException(
                                            "Incorrect edge weight token.");

                                    heightMap = new XGradient();
                                }
                                break;
                            case "Y":
                                {
                                    if (3 < tokens.Length)
                                        throw new ArgumentException(
                                            "Incorrect edge weight token.");

                                    heightMap = new YGradient();
                                }
                                break;
                            case "X+Y":
                                {
                                    if (3 < tokens.Length)
                                        throw new ArgumentException(
                                            "Incorrect edge weight token.");

                                    heightMap = new XYSumGradient();
                                }
                                break;
                            case "X-Y":
                                {
                                    if (3 < tokens.Length)
                                        throw new ArgumentException(
                                            "Incorrect edge weight token.");

                                    heightMap = new XYDifferenceGradient();
                                }
                                break;
                            case "E":
                                {
                                    if (4 < tokens.Length)
                                        throw new ArgumentException(
                                            "Incorrect edge weight token.");

                                    heightMap = new EuclideanDistanceGradient(
                                        Point2D.Parse(tokens[3]));
                                }
                                break;
                            case "M":
                                {
                                    if (4 < tokens.Length)
                                        throw new ArgumentException(
                                            "Incorrect edge weight token.");

                                    heightMap = new ManhattanDistanceGradient(
                                        Point2D.Parse(tokens[3]));
                                }
                                break;
                            default:
                                throw new ArgumentException("Incorrect edge weight token.");
                        }

                        return new HeightMapEdgeWeightAssignment(heightMap, negateWeights);
                    }
                default:
                    throw new ArgumentException("Incorrect edge weight token.");
            }
        }

        static MazeConnector ParseMazeConnector(string token)
        {
            // (mM)(cC)

            bool mergeDeadEnds, changeColor;

            if (2 != token.Length)
                throw new ArgumentException("Incorrect maze connector token.");

            if (token[0] == 'm')
                mergeDeadEnds = false;
            else if (token[0] == 'M')
                mergeDeadEnds = true;
            else
                throw new ArgumentException("Incorrect maze connector token.");

            if (token[1] == 'c')
                changeColor = false;
            else if (token[1] == 'C')
                changeColor = true;
            else
                throw new ArgumentException("Incorrect maze connector token.");

            return new MazeConnector(mergeDeadEnds, changeColor);
        }

        #endregion


        #region Execution

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

            Directory.CreateDirectory(Path.GetDirectoryName(outputFilename));

            using (var output = File.CreateText(outputFilename))
                MazeWriter.WriteMaze(output, physicalGraph);

            return 0;
        }

        #endregion
    }


    interface IMazeLayout
    {
        void Generate(out Graph2D physicalGraph, out Graph2D logicalGraph);
    }
}
