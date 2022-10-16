using System.Linq;
using UnaryHeap.Graph;
using UnaryHeap.DataType;

namespace MazeGenerator
{
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
}
