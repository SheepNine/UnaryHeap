using System.IO;

namespace Pocotheosis
{
    abstract class PocoMember
    {
        public string name { get; private set; }

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

        public void WriteSerialization(TextWriter output)
        {
            output.Write("SerializationHelpers.Serialize(");
            output.Write(name);
            output.Write(", output);");
        }

        public abstract void WriteDeserialization(TextWriter output);
    }

    class BoolPocoMember : PocoMember
    {
        public BoolPocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("bool");
        }

        public override void WriteDeserialization(TextWriter output)
        {
            output.Write("var ");
            output.Write(name);
            output.Write(" = SerializationHelpers.DeserializeBool(input);");
        }
    }

    class BytePocoMember : PocoMember
    {
        public BytePocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("byte");
        }

        public override void WriteDeserialization(TextWriter output)
        {
            output.Write("var ");
            output.Write(name);
            output.Write(" = SerializationHelpers.DeserializeByte(input);");
        }
    }

    class Int16PocoMember : PocoMember
    {
        public Int16PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("short");
        }

        public override void WriteDeserialization(TextWriter output)
        {
            output.Write("var ");
            output.Write(name);
            output.Write(" = SerializationHelpers.DeserializeInt16(input);");
        }
    }

    class Int32PocoMember : PocoMember
    {
        public Int32PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("int");
        }

        public override void WriteDeserialization(TextWriter output)
        {
            output.Write("var ");
            output.Write(name);
            output.Write(" = SerializationHelpers.DeserializeInt32(input);");
        }
    }

    class Int64PocoMember : PocoMember
    {
        public Int64PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("long");
        }

        public override void WriteDeserialization(TextWriter output)
        {
            output.Write("var ");
            output.Write(name);
            output.Write(" = SerializationHelpers.DeserializeInt64(input);");
        }
    }



    class SBytePocoMember : PocoMember
    {
        public SBytePocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("sbyte");
        }

        public override void WriteDeserialization(TextWriter output)
        {
            output.Write("var ");
            output.Write(name);
            output.Write(" = SerializationHelpers.DeserializeSByte(input);");
        }
    }

    class UInt16PocoMember : PocoMember
    {
        public UInt16PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("ushort");
        }

        public override void WriteDeserialization(TextWriter output)
        {
            output.Write("var ");
            output.Write(name);
            output.Write(" = SerializationHelpers.DeserializeUInt16(input);");
        }
    }

    class UInt32PocoMember : PocoMember
    {
        public UInt32PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("uint");
        }

        public override void WriteDeserialization(TextWriter output)
        {
            output.Write("var ");
            output.Write(name);
            output.Write(" = SerializationHelpers.DeserializeUInt32(input);");
        }
    }

    class UInt64PocoMember : PocoMember
    {
        public UInt64PocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("ulong");
        }

        public override void WriteDeserialization(TextWriter output)
        {
            output.Write("var ");
            output.Write(name);
            output.Write(" = SerializationHelpers.DeserializeUInt64(input);");
        }
    }

    class StringPocoMember : PocoMember
    {
        public StringPocoMember(string name) : base(name) { }

        protected override void WriteType(TextWriter output)
        {
            output.Write("string");
        }

        public override void WriteEqualityComparison(TextWriter output)
        {
            output.Write("string.Equals(this.");
            output.Write(name);
            output.Write(", other.");
            output.Write(name);
            output.Write(")");
        }

        public override void WriteDeserialization(TextWriter output)
        {
            output.Write("var ");
            output.Write(name);
            output.Write(" = SerializationHelpers.DeserializeString(input);");
        }
    }
}
