using System;
using System.IO;

namespace Pocotheosis
{
    interface IPocoMember
    {
        string name { get; }

        void WriteDeclaration(TextWriter output);
        void WriteFormalParameter(TextWriter output);
        void WriteAssignment(TextWriter output);
        void WriteEqualityComparison(TextWriter output);
        void WriteHash(TextWriter output);
        void WriteDeserialization(TextWriter output);
        void WriteSerialization(TextWriter output);
        void WriteToStringOutput(TextWriter output);
    }

    class PocoMember : IPocoMember
    {
        public string name { get; private set; }
        IPocoType type;

        public PocoMember(string variableName, IPocoType type)
        {
            name = variableName;
            this.type = type;
        }

        public void WriteAssignment(TextWriter output)
        {
            type.WriteAssignment(name, output);
        }

        public void WriteDeclaration(TextWriter output)
        {
            type.WriteDeclaration(name, output);
        }

        public void WriteDeserialization(TextWriter output)
        {
            type.WriteDeserialization(name, output);
        }

        public void WriteEqualityComparison(TextWriter output)
        {
            type.WriteEqualityComparison(name, output);
        }

        public void WriteFormalParameter(TextWriter output)
        {
            type.WriteFormalParameter(name, output);
        }

        public void WriteHash(TextWriter output)
        {
            type.WriteHash(name, output);
        }

        public void WriteSerialization(TextWriter output)
        {
            type.WriteSerialization(name, output);
        }

        public void WriteToStringOutput(TextWriter output)
        {
            type.WriteToStringOutput(name, output);
        }
    }

    interface IPocoType
    {
        void WriteDeclaration(string variableName, TextWriter output);
        void WriteFormalParameter(string variableName, TextWriter output);
        void WriteAssignment(string variableName, TextWriter output);
        void WriteEqualityComparison(string variableName, TextWriter output);
        void WriteHash(string variableName, TextWriter output);
        void WriteDeserialization(string variableName, TextWriter output);
        void WriteSerialization(string variableName, TextWriter output);
        void WriteToStringOutput(string variableName, TextWriter output);
    }

    abstract class PrimitiveType : IPocoType
    {
        protected abstract string TypeName { get; }

        public virtual void WriteDeclaration(string variableName, TextWriter output)
        {
            output.Write("public ");
            output.Write(TypeName);
            output.Write(" ");
            output.Write(variableName);
            output.Write(" { get; private set; }");
        }

        public virtual void WriteFormalParameter(string variableName, TextWriter output)
        {
            output.Write(TypeName);
            output.Write(" ");
            output.Write(variableName);
        }

        public virtual void WriteAssignment(string variableName, TextWriter output)
        {
            output.Write("this.");
            output.Write(variableName);
            output.Write(" = ");
            output.Write(variableName);
            output.Write(";");
        }

        public virtual void WriteEqualityComparison(string variableName, TextWriter output)
        {
            output.Write("this.");
            output.Write(variableName);
            output.Write(" == other.");
            output.Write(variableName);
        }

        public virtual void WriteHash(string variableName, TextWriter output)
        {
            output.Write(variableName);
            output.Write(".GetHashCode()");
        }

        public abstract void WriteDeserialization(string variableName, TextWriter output);

        public virtual void WriteSerialization(string variableName, TextWriter output)
        {
            output.Write("SerializationHelpers.Serialize(");
            output.Write(variableName);
            output.Write(", output);");
        }

        public virtual void WriteToStringOutput(string variableName, TextWriter output)
        {
            output.Write("\t\t\tresult.Append(\"");
            output.Write(variableName);
            output.WriteLine(": \");");
            output.Write("\t\t\tresult.Append(");
            output.Write(variableName);
            output.WriteLine(".ToString(format));");
        }
    }

    class BoolType : PrimitiveType
    {
        public static readonly IPocoType Instance = new BoolType();

        protected override string TypeName { get { return "bool"; } }

        public override void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = SerializationHelpers.DeserializeBool(input);");
        }
    }

    class Int8Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new Int8Type();

        protected override string TypeName { get { return "sbyte"; } }

        public override void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = SerializationHelpers.DeserializeSByte(input);");
        }
    }

    class Int16Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new Int16Type();

        protected override string TypeName { get { return "short"; } }

        public override void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = SerializationHelpers.DeserializeInt16(input);");
        }
    }

    class Int32Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new Int32Type();

        protected override string TypeName { get { return "int"; } }

        public override void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = SerializationHelpers.DeserializeInt32(input);");
        }
    }

    class Int64Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new Int64Type();

        protected override string TypeName { get { return "long"; } }

        public override void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = SerializationHelpers.DeserializeInt64(input);");
        }
    }

    class UInt8Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new UInt8Type();

        protected override string TypeName { get { return "byte"; } }

        public override void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = SerializationHelpers.DeserializeByte(input);");
        }
    }

    class UInt16Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new UInt16Type();

        protected override string TypeName { get { return "ushort"; } }

        public override void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = SerializationHelpers.DeserializeUInt16(input);");
        }
    }

    class UInt32Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new UInt32Type();

        protected override string TypeName { get { return "uint"; } }

        public override void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = SerializationHelpers.DeserializeUInt32(input);");
        }
    }

    class UInt64Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new UInt64Type();

        protected override string TypeName { get { return "ulong"; } }

        public override void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = SerializationHelpers.DeserializeUInt64(input);");
        }
    }

    class StringType : PrimitiveType
    {
        public static readonly IPocoType Instance = new StringType();

        protected override string TypeName { get { return "string"; } }

        public override void WriteEqualityComparison(string variableName, TextWriter output)
        {
            output.Write("string.Equals(this.");
            output.Write(variableName);
            output.Write(", other.");
            output.Write(variableName);
            output.Write(")");
        }

        public override void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = SerializationHelpers.DeserializeString(input);");
        }
    }

    class EnumType : PrimitiveType
    {
        PocoEnum enumType;

        public EnumType(PocoEnum enumType)
        {
            this.enumType = enumType;
        }
        protected override string TypeName { get { return enumType.Name; } }

        public override void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = (");
            output.Write(enumType.Name);
            output.WriteLine(")SerializationHelpers.DeserializeByte(input);");
        }

        public override void WriteSerialization(string variableName, TextWriter output)
        {
            output.Write("SerializationHelpers.Serialize((byte)");
            output.Write(variableName);
            output.Write(", output);");
        }

        public override void WriteToStringOutput(string variableName, TextWriter output)
        {
            output.Write("\t\t\tresult.Append(\"");
            output.Write(variableName);
            output.WriteLine(": \");");
            output.Write("\t\t\tresult.Append(");
            output.Write(variableName);
            output.WriteLine(".ToString());");
        }
    }
}
