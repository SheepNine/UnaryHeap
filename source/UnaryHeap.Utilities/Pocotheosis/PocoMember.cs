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
        string TypeName { get; }
        string DeserializerMethod { get; }
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
        public abstract string TypeName { get; }
        public abstract string DeserializerMethod { get; }

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

        public virtual void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = ");
            output.Write(DeserializerMethod);
            output.Write("(input);");
        }

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

        public override string TypeName { get { return "bool"; } }
        public override string DeserializerMethod { get { return "SerializationHelpers.DeserializeBool"; } }
    }

    class Int8Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new Int8Type();

        public override string TypeName { get { return "sbyte"; } }
        public override string DeserializerMethod { get { return "SerializationHelpers.DeserializeSByte"; } }
    }

    class Int16Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new Int16Type();

        public override string TypeName { get { return "short"; } }
        public override string DeserializerMethod { get { return "SerializationHelpers.DeserializeInt16"; } }
    }

    class Int32Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new Int32Type();

        public override string TypeName { get { return "int"; } }
        public override string DeserializerMethod { get { return "SerializationHelpers.DeserializeInt32"; } }
    }

    class Int64Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new Int64Type();

        public override string TypeName { get { return "long"; } }
        public override string DeserializerMethod { get { return "SerializationHelpers.DeserializeInt64"; } }
    }

    class UInt8Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new UInt8Type();

        public override string TypeName { get { return "byte"; } }
        public override string DeserializerMethod { get { return "SerializationHelpers.DeserializeByte"; } }
    }

    class UInt16Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new UInt16Type();

        public override string TypeName { get { return "ushort"; } }
        public override string DeserializerMethod { get { return "SerializationHelpers.DeserializeUInt16"; } }
    }

    class UInt32Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new UInt32Type();

        public override string TypeName { get { return "uint"; } }
        public override string DeserializerMethod { get { return "SerializationHelpers.DeserializeUInt32"; } }
    }

    class UInt64Type : PrimitiveType
    {
        public static readonly IPocoType Instance = new UInt64Type();

        public override string TypeName { get { return "ulong"; } }
        public override string DeserializerMethod { get { return "SerializationHelpers.DeserializeUInt64"; } }
    }

    class StringType : PrimitiveType
    {
        public static readonly IPocoType Instance = new StringType();

        public override string TypeName { get { return "string"; } }
        public override string DeserializerMethod { get { return "SerializationHelpers.DeserializeString"; } }

        public override void WriteEqualityComparison(string variableName, TextWriter output)
        {
            output.Write("string.Equals(this.");
            output.Write(variableName);
            output.Write(", other.");
            output.Write(variableName);
            output.Write(")");
        }
    }

    class EnumType : PrimitiveType
    {
        PocoEnum enumType;

        public EnumType(PocoEnum enumType)
        {
            this.enumType = enumType;
        }
        public override string TypeName { get { return enumType.Name; } }
        public override string DeserializerMethod { get { return "(" + enumType.Name + ")SerializationHelpers.DeserializeByte"; } }

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

    class ArrayType : IPocoType
    {
        private IPocoType elementType;

        public ArrayType(IPocoType baseType)
        {
            this.elementType = baseType;
        }

        public string TypeName
        {
             get { return elementType.TypeName + "[]"; }
        }
        public string DeserializerMethod { get { throw new NotImplementedException(); } }

        public void WriteAssignment(string variableName, TextWriter output)
        {
            output.Write("this.");
            output.Write(variableName);
            output.Write(" = global::System.Linq.Enumerable.ToArray(");
            output.Write(variableName);
            output.Write(");");
        }

        public void WriteDeclaration(string variableName, TextWriter output)
        {
            output.Write("public global::System.Collections.Generic.IList<");
            output.Write(elementType.TypeName);
            output.Write("> ");
            output.Write(variableName);
            output.Write(" { get; private set; }");
        }

        public void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(variableName);
            output.Write(" = SerializationHelpers.DeserializeList(input, ");
            output.Write(elementType.DeserializerMethod);
            output.Write(");");
        }

        public void WriteEqualityComparison(string variableName, TextWriter output)
        {
            output.Write("EquatableHelper.ListEquals(this.");
            output.Write(variableName);
            output.Write(", other.");
            output.Write(variableName);
            output.Write(")");
        }

        public void WriteFormalParameter(string variableName, TextWriter output)
        {
            output.Write("global::System.Collections.Generic.IEnumerable<");
            output.Write(elementType.TypeName);
            output.Write("> ");
            output.Write(variableName);
        }

        public void WriteHash(string variableName, TextWriter output)
        {
            output.Write("HashHelper.GetListHashCode(");
            output.Write(variableName);
            output.Write(")");
        }

        public void WriteSerialization(string variableName, TextWriter output)
        {
            output.Write("SerializationHelpers.SerializeList(");
            output.Write(variableName);
            output.Write(", output, SerializationHelpers.Serialize);");
        }

        public void WriteToStringOutput(string variableName, TextWriter output)
        {
            output.Write("\t\t\tresult.Append(\"");
            output.Write(variableName);
            output.WriteLine(" size: \");");
            output.Write("\t\t\tresult.Append(");
            output.Write(variableName);
            output.WriteLine(".Count);");
        }
    }
}
