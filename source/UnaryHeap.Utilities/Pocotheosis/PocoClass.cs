using System.Collections.Generic;

namespace Pocotheosis
{
    public class PocoClass
    {
        public string Name { get; private set; }
        public int StreamingId { get; private set; }
        List<IPocoMember> members;
        public IEnumerable<IPocoMember> Members { get { return members; } }

        internal PocoClass(string name, int id, IEnumerable<IPocoMember> members)
        {
            Name = name;
            StreamingId = id;
            this.members = new List<IPocoMember>(members);
        }
    }
}
