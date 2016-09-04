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

    class BoolPocoMember : PocoMember
    {
        public BoolPocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("global::System.Boolean");
        }
    }

    class BytePocoMember : PocoMember
    {
        public BytePocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("global::System.Byte");
        }
    }

    class Int16PocoMember : PocoMember
    {
        public Int16PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("global::System.Int16");
        }
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



    class SBytePocoMember : PocoMember
    {
        public SBytePocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("global::System.SByte");
        }
    }

    class UInt16PocoMember : PocoMember
    {
        public UInt16PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("global::System.UInt16");
        }
    }

    class UInt32PocoMember : PocoMember
    {
        public UInt32PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("global::System.UInt32");
        }
    }

    class UInt64PocoMember : PocoMember
    {
        public UInt64PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("global::System.UInt64");
        }
    }

    class StringPocoMember : PocoMember
    {
        public StringPocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("global::System.String");
        }
    }
}
