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
            System.Windows.Forms.MessageBox.Show(input.ToString());
            return null;
        }
    }

    class MetadataSet
    {
        SortedDictionary<string, string> data;

        public MetadataSet(IReadOnlyDictionary<string, string> source)
            : this(new[] { source })
        {
        }

        public MetadataSet(IEnumerable<IReadOnlyDictionary<string, string>> sources)
        {
            foreach (var source in sources)
            {
                if (null == data)
                    InitializeData(source);
                else
                    AppendData(source);
            }
        }

        void InitializeData(IReadOnlyDictionary<string, string> source)
        {
            data = new SortedDictionary<string, string>();

            foreach (var entry in source)
                data.Add(entry.Key, entry.Value);
        }

        void AppendData(IReadOnlyDictionary<string, string> source)
        {
            foreach (var key in data.Keys.ToArray())
            {
                if (false == source.ContainsKey(key))
                    data[key] = null;
                else if (data[key] != source[key])
                    data[key] = null;
            }

            foreach (var key in source.Keys)
            {
                if (false == data.ContainsKey(key))
                    data.Add(key, null);
            }
        }

        public MetadataChange GetChangeTo(MetadataSet output)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var datum in data)
            {
                if (null == datum.Value)
                    result.AppendLine(string.Format("{0}: <varies>", datum.Key));
                else
                    result.AppendLine(string.Format("{0}: {1}", datum.Key, datum.Value));
            }

            return result.ToString();
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
