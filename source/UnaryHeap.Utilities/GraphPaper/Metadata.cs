using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnaryHeap.Utilities.D2;

namespace GraphPaper
{
    class Metadata
    {
        public static MetadataChange ViewAndEdit(MetadataSet input)
        {
            var output = ShowUIForEdit(input);

            if (null == output)
                return null;

            return input.GetChangeTo(output);
        }

        static MetadataSet ShowUIForEdit(MetadataSet input)
        {
            return null;
        }
    }

    class MetadataSet
    {
        public MetadataChange GetChangeTo(MetadataSet output)
        {
            throw new NotImplementedException();
        }
    }

    class MetadataChange
    {
        internal void UpdateGraphMetadata(Graph2D graph)
        {
            throw new NotImplementedException();
        }

        internal void UpdateVertexMetadata(Graph2D graph, GraphObjectSelection selection)
        {
            throw new NotImplementedException();
        }

        internal void UpdateEdgeMetadata(Graph2D graph, GraphObjectSelection selection)
        {
            throw new NotImplementedException();
        }
    }
}
