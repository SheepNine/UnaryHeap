using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patchwork
{
    class MruList
    {
        List<string> recentArrangments = new List<string>();
        public MruList()
        {

        }

        public int Count
        {
            get { return recentArrangments.Count; }
        }

        public string this[int index]
        {
            get { return recentArrangments[index]; }
        }

        public void AddToList(string newItem)
        {
            recentArrangments.Remove(newItem);
            recentArrangments.Insert(0, newItem);
        }

        public void RemoveFromList(string filename)
        {
            recentArrangments.Remove(filename);
        }
    }
}
