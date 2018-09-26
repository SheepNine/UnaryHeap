using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pocotheosis;
using System.IO;

namespace Pocotheosis.MemberTypes
{
    abstract class PrimitiveType : IPocoType
    {
        public abstract string TypeName { get; }
        public abstract string DeserializerMethod { get; }

#if TEST_POCO_NAME_GEN
        public string PublicMemberName(string variableName)
        {
            return "shared_" + variableName;
        }

        public string BackingStoreName(string variableName)
        {
            return PublicMemberName(variableName);
        }

        public string TempVarName(string variableName)
        {
            return "t_" + variableName;
        }
#else
        public string PublicMemberName(string variableName)
        {
            return variableName;
        }

        public string BackingStoreName(string variableName)
        {
            return "__" + variableName;
        }

        public string TempVarName(string variableName)
        {
            return "t" + variableName;
        }

        public virtual string BuilderReifier(string variableName)
        {
            return variableName;
        }

        public virtual string BuilderUnreifier(string variableName)
        {
            return variableName;
        }
#endif

        public virtual void WritePublicMemberDeclaration(string variableName, TextWriter output)
        {
            output.Write("public ");
            output.Write(TypeName);
            output.Write(" ");
            output.Write(PublicMemberName(variableName));
            output.Write(" { get { return " + BackingStoreName(variableName) + "; } }");
        }

        public void WriteBackingStoreDeclaration(string variableName, TextWriter output)
        {
            output.Write("private " + TypeName + " " + BackingStoreName(variableName) + ";");
        }

        public virtual void WriteFormalParameter(string variableName, TextWriter output)
        {
            output.Write(TypeName);
            output.Write(" ");
            output.Write(TempVarName(variableName));
        }

        public virtual void WriteAssignment(string variableName, TextWriter output)
        {
            output.Write(BackingStoreName(variableName));
            output.Write(" = ");
            output.Write(TempVarName(variableName));
            output.Write(";");
        }

        public virtual void WriteEqualityComparison(string variableName, TextWriter output)
        {
            output.Write("EquatableHelper.AreEqual(this.");
            output.Write(BackingStoreName(variableName));
            output.Write(", other.");
            output.Write(BackingStoreName(variableName));
            output.Write(")");
        }

        public virtual void WriteHash(string variableName, TextWriter output)
        {
            output.Write(BackingStoreName(variableName));
            output.Write(".GetHashCode()");
        }

        public virtual void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(TempVarName(variableName));
            output.Write(" = ");
            output.Write(DeserializerMethod);
            output.Write("(input);");
        }

        public virtual void WriteSerialization(string variableName, TextWriter output)
        {
            output.Write("SerializationHelpers.Serialize(");
            output.Write(BackingStoreName(variableName));
            output.Write(", output);");
        }

        public virtual string ToStringOutput(string variableName)
        {
            return "target.Write(" + variableName + ");";
        }

        public virtual void WriteConstructorCheck(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\tif (!ConstructorHelper.CheckValue({0})) " +
                "throw new global::System.ArgumentNullException(\"{1}\");",
                TempVarName(variableName), variableName);
        }

        public virtual void WriteBuilderDeclaration(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\tprivate " + BuilderTypeName + " " +
                BackingStoreName(variableName) + ";");
        }

        public virtual void WriteBuilderAssignment(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t\t" + BackingStoreName(variableName) + " = " +
                BuilderUnreifier(TempVarName(variableName)) + ";");
        }

        public virtual void WriteBuilderPlumbing(string variableName, string singularName,
            TextWriter output)
        {
            output.WriteLine("\t\t\t// --- " + variableName + " ---");

            output.WriteLine("\t\t\tpublic " + TypeName + " " + PublicMemberName(variableName));
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tget");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\treturn " +
                BuilderReifier(BackingStoreName(variableName)) + ";");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t\tset");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\tif (!ConstructorHelper.CheckValue(value)) " +
                "throw new global::System.ArgumentNullException(\"value\");");
            output.WriteLine("\t\t\t\t\t" + BackingStoreName(variableName) +
                " = " + BuilderUnreifier("value") + ";");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t}");
        }

        public virtual string BuilderTypeName
        {
            get { return TypeName; }
        }
    }

    class BoolType : PrimitiveType
    {
        public static readonly BoolType Instance = new BoolType();

        public override string TypeName { get { return "bool"; } }
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.DeserializeBool"; }
        }
    }

    class Int8Type : PrimitiveType
    {
        public static readonly Int8Type Instance = new Int8Type();

        public override string TypeName { get { return "sbyte"; } }
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.DeserializeSByte"; }
        }
    }

    class Int16Type : PrimitiveType
    {
        public static readonly Int16Type Instance = new Int16Type();

        public override string TypeName { get { return "short"; } }
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.DeserializeInt16"; }
        }
    }

    class Int32Type : PrimitiveType
    {
        public static readonly Int32Type Instance = new Int32Type();

        public override string TypeName { get { return "int"; } }
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.DeserializeInt32"; }
        }
    }

    class Int64Type : PrimitiveType
    {
        public static readonly Int64Type Instance = new Int64Type();

        public override string TypeName { get { return "long"; } }
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.DeserializeInt64"; }
        }
    }

    class UInt8Type : PrimitiveType
    {
        public static readonly UInt8Type Instance = new UInt8Type();

        public override string TypeName { get { return "byte"; } }
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.DeserializeByte"; }
        }
    }

    class UInt16Type : PrimitiveType
    {
        public static readonly UInt16Type Instance = new UInt16Type();

        public override string TypeName { get { return "ushort"; } }
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.DeserializeUInt16"; }
        }
    }

    class UInt32Type : PrimitiveType
    {
        public static readonly UInt32Type Instance = new UInt32Type();

        public override string TypeName { get { return "uint"; } }
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.DeserializeUInt32"; }
        }
    }

    class UInt64Type : PrimitiveType
    {
        public static readonly UInt64Type Instance = new UInt64Type();

        public override string TypeName { get { return "ulong"; } }
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.DeserializeUInt64"; }
        }
    }

    class StringType : PrimitiveType
    {
        public static readonly StringType Instance = new StringType();

        public override string TypeName { get { return "string"; } }
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.DeserializeString"; }
        }

        public override string ToStringOutput(string variableName)
        {
            return "target.Write(\"'\" + " + variableName + " + \"'\");";
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
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.Deserialize" + enumType.Name; }
        }

        public override string ToStringOutput(string variableName)
        {
            return "target.Write(" + variableName + ".ToString());";
        }
    }

    class ClassType : PrimitiveType
    {
        string className;

        public ClassType(string className)
        {
            this.className = className;
        }
        public override string TypeName { get { return className; } }

        public override string DeserializerMethod
        {
            get { return className + ".Deserialize"; }
        }

        public override string BuilderTypeName
        {
            get { return className + ".Builder"; }
        }

        public override string BuilderReifier(string variableName)
        {
            return variableName + ".Build()";
        }

        public override string BuilderUnreifier(string variableName)
        {
            return variableName + ".ToBuilder()";
        }

        public override string ToStringOutput(string variableName)
        {
            return variableName + ".WriteIndented(target);";
        }

        public override void WriteBuilderPlumbing(string variableName, string singularName,
            TextWriter output)
        {
            output.WriteLine("\t\t\t// --- " + variableName + " ---");

            output.WriteLine("\t\t\tpublic Builder With" + PublicMemberName(variableName) +
                "(" + TypeName + " value)");
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tif (!ConstructorHelper.CheckValue(value)) " +
                "throw new global::System.ArgumentNullException(\"value\");");
            output.WriteLine("\t\t\t\t" + BackingStoreName(variableName) + " = " +
                BuilderUnreifier("value") + ";");
            output.WriteLine("\t\t\t\treturn this;");
            output.WriteLine("\t\t\t}");

            output.WriteLine("\t\t\tpublic " + BuilderTypeName + " " +
                PublicMemberName(variableName));
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tget");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\treturn " + BackingStoreName(variableName) + ";");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t}");
        }
    }
}
