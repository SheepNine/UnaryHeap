using System;
using System.Collections.Generic;
using UnaryHeap.Utilities.D2;

namespace Partitioner
{
    class Surface
    {
        Point2D start;
        Point2D end;
        Hyperplane2D hyperplane;
        IReadOnlyDictionary<string, string> metadata;

        public Surface(Point2D start, Point2D end,
            IReadOnlyDictionary<string, string> metadata)
        {
            this.start = start;
            this.end = end;
            this.hyperplane = new Hyperplane2D(start, end);
            this.metadata = metadata;
        }

        public Point2D Start
        {
            get { return start; }
        }

        public Point2D End
        {
            get { return end; }
        }

        public Hyperplane2D Hyperplane
        {
            get { return hyperplane; }
        }

        public IReadOnlyDictionary<string, string> Metadata
        {
            get { return metadata; }
        }

        public static List<Surface> LoadSurfaces(Graph2D source)
        {
            var result = new List<Surface>();

            foreach (var edge in source.Edges)
                result.Add(new Surface(edge.Item1, edge.Item2,
                    source.GetEdgeMetadata(edge.Item1, edge.Item2)));

            return result;
        }

        public void Split(Hyperplane2D splitter,
            out Surface frontSurface, out Surface backSurface)
        {
            var startSpace = splitter.DetermineHalfspaceOf(start);
            var endSpace = splitter.DetermineHalfspaceOf(end);

            if (startSpace > 0)
            {
                if (endSpace > 0)
                {
                    frontSurface = this;
                    backSurface = null;
                }
                else if (endSpace < 0)
                {
                    var middle = splitter.FindIntersection(hyperplane);
                    frontSurface = new Surface(start, middle, metadata);
                    backSurface = new Surface(middle, end, metadata);
                }
                else // endSpace == 0
                {
                    frontSurface = this;
                    backSurface = null;
                }
            }
            else if (startSpace < 0)
            {
                if (endSpace > 0)
                {
                    var middle = splitter.FindIntersection(hyperplane);
                    frontSurface = new Surface(end, middle, metadata);
                    backSurface = new Surface(middle, start, metadata);
                }
                else if (endSpace < 0)
                {
                    frontSurface = null;
                    backSurface = this;
                }
                else // endSpace == 0
                {
                    frontSurface = null;
                    backSurface = this;
                }
            }
            else // startSpace == 0
            {
                if (endSpace > 0)
                {
                    frontSurface = this;
                    backSurface = null;
                }
                else if (endSpace < 0)
                {
                    frontSurface = null;
                    backSurface = this;
                }
                else // endSpace == 0
                {
                    if (hyperplane.Equals(splitter))
                    {
                        frontSurface = this;
                        backSurface = null;
                    }
                    else
                    {
                        frontSurface = null;
                        backSurface = this;
                    }
                }
            }
        }

        public bool IsConvexWith(Surface other)
        {
            return
                this.hyperplane.DetermineHalfspaceOf(other.start) >= 0 &&
                this.hyperplane.DetermineHalfspaceOf(other.end) >= 0 &&
                other.hyperplane.DetermineHalfspaceOf(this.start) >= 0 &&
                other.hyperplane.DetermineHalfspaceOf(this.end) >= 0;
        }

        public bool IsPassage
        {
            get
            {
                return metadata.ContainsKey("passage") ?
                    bool.Parse(metadata["passage"]) : false;
            }
        }

        public string RoomName
        {
            get
            {
                return metadata.ContainsKey("room") ?
                    metadata["room"] : null;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1} {2},{3} {4}",
                (double)start.X, (double)start.Y,
                (double)end.X,   (double)end.Y,
                IsPassage ? "<passage>" : RoomName);
        }
    }
}
