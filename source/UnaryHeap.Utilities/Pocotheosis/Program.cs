using System.IO;

namespace Pocotheosis
{
    class Program
    {
        static void Main(string[] args)
        {
            File.WriteAllText("Pocos.cs", @"namespace NAMESPAAAACE
{
    public class POCO1
    {
        public global::System.Int32 VAR1 { get; private set; }
        public global::System.Int64 VAR2 { get; private set; }

        public POCO1(global::System.Int32 VAR1, global::System.Int64 VAR2)
        {
            this.VAR1 = VAR1;
            this.VAR2 = VAR2;
        }
    }
}");
        }
    }
}
