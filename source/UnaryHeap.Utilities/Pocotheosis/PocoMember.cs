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

        public virtual void WriteEqualityComparison(TextWriter output)
        {
            output.Write("this.");
            output.Write(name);
            output.Write(" == other.");
            output.Write(name);
        }

        public void WriteToStringOutput(TextWriter output)
        {
            output.Write("\t\t\tresult.Append(\"");
            output.Write(name);
            output.WriteLine(": \");");
            output.Write("\t\t\tresult.Append(");
            output.Write(name);
            output.WriteLine(".ToString(format));");
        }

        protected abstract void WriteType(TextWriter output);

        internal void WriteSerialization(TextWriter output)
        {
            output.Write("SerializationHelpers.Serialize(");
            output.Write(name);
            output.Write(", output);");
        }
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

        public override void WriteEqualityComparison(TextWriter output)
        {
            output.Write("global::System.String.Equals(this.");
            output.Write(name);
            output.Write(", other.");
            output.Write(name);
            output.Write(")");
        }
    }
}
