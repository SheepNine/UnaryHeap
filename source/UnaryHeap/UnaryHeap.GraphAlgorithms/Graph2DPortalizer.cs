using UnaryHeap.Algorithms;
using UnaryHeap.DataType;
using UnaryHeap.Graph;

namespace UnaryHeap.GraphAlgorithms
{
    public class GraphPortalizer : Portalizer<GraphSegment, Hyperplane2D, Facet2D, Orthotope2D>
    {
        public GraphPortalizer() : base(GraphDimension.Instance)
        {
        }
    }
}
