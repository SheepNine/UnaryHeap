using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disassembler
{
    class LabelSet
    {
        private SortedDictionary<int, string> labels = new SortedDictionary<int, string>();

        public void Record(int address)
        {
            Record(address, string.Format("UL_{0:X4}", address));
        }

        public void Record(int address, string label)
        {
            if (!labels.ContainsKey(address))
                labels.Add(address, label);
        }

        public string GetLabel(int address)
        {
            if (labels.ContainsKey(address))
                return labels[address];
            else
                return null;
        }
    }
}
