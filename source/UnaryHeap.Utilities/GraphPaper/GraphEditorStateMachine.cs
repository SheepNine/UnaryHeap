using System;
using System.Collections.Generic;
using System.IO;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.UI;

namespace GraphPaper
{
    class Graph2DCreateArgs
    {
        public bool Directed { get; private set; }

        public Graph2DCreateArgs(bool directed)
        {
            Directed = directed;
        }
    }

    class GraphEditorStateMachine : ModelEditorStateMachine<
        Graph2DCreateArgs, Graph2D, ReadOnlyGraph2D>
    {
        public GraphEditorStateMachine() : base(new Prompts())
        {
        }

        protected override Graph2D Clone(Graph2D instance)
        {
            return instance.Clone();
        }

        protected override Graph2D CreateEmptyModel(Graph2DCreateArgs args)
        {
            return new Graph2D(args.Directed);
        }

        protected override Graph2D ReadModelFromDisk(string fileName)
        {
            using (var stream = File.OpenText(fileName))
                return Graph2D.FromJson(stream);
        }

        protected override ReadOnlyGraph2D Wrap(Graph2D instance)
        {
            return new ReadOnlyGraph2D(instance);
        }

        protected override void WriteModelToDisk(Graph2D instance, string fileName)
        {
            using (var stream = File.CreateText(fileName))
                instance.ToJson(stream);
        }
    }

    class ReadOnlyGraph2D
    {
        Graph2D graph;

        public ReadOnlyGraph2D(Graph2D graph)
        {
            this.graph = graph;
        }

        public IEnumerable<Tuple<Point2D, Point2D>> Edges
        {
            get { return graph.Edges; }
        }

        public IEnumerable<Point2D> Vertices
        {
            get { return graph.Vertices; }
        }

        public Orthotope2D Extents
        {
            get
            {
                if (0 == graph.NumVertices)
                    return new Orthotope2D(-5, -5, 5, 5);

                var result = Orthotope2D.FromPoints(graph.Vertices);

                if (0 == result.X.Size || 0 == result.Y.Size)
                    result = result.GetPadded(1);

                return result;
            }
        }

        public bool HasVertex(Point2D coordinates)
        {
            return graph.HasVertex(coordinates);
        }

        public bool HasEdge(Point2D start, Point2D end)
        {
            return graph.HasEdge(start, end);
        }
    }
}
