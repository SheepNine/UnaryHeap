using System.IO;

namespace Pocotheosis
{
    abstract class PocoMember
    {
        protected string name;

        public PocoMember(string name)
        {
            this.name = name;
        }

        public void WriteDeclaration(TextWriter output)
        {
            output.Write("public ");
            WriteType(output);
            output.Write(" ");
            output.Write(name);
            output.Write(" { get; private set; }");
        }

        public void WriteFormalParameter(TextWriter output)
        {
            WriteType(output);
            output.Write(" ");
            output.Write(name);
        }

        public void WriteAssignment(TextWriter output)
        {
            output.Write("this.");
            output.Write(name);
            output.Write(" = ");
            output.Write(name);
            output.Write(";");
        }

        protected abstract void WriteType(TextWriter output);
    }

    class Int32PocoMember : PocoMember
    {
        public Int32PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("global::System.Int32");
        }
    }

    class Int64PocoMember : PocoMember
    {
        public Int64PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("global::System.Int64");
        }
    }
}
