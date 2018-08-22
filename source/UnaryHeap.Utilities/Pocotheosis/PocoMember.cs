using System;
using System.IO;

namespace Pocotheosis
{
    public interface IPocoMember
    {
        string PublicMemberName();
        string BackingStoreName();
        string TempVarName();
        string BuilderReifier();
        void WritePublicMemberDeclaration(TextWriter output);
        void WriteBackingStoreDeclaration(TextWriter output);
        void WriteFormalParameter(TextWriter output);
        void WriteAssignment(TextWriter output);
        void WriteEqualityComparison(TextWriter output);
        void WriteHash(TextWriter output);
        void WriteDeserialization(TextWriter output);
        void WriteSerialization(TextWriter output);
        void WriteToStringOutput(TextWriter output);
        void WriteConstructorCheck(TextWriter output);
        void WriteBuilderDeclaration(TextWriter output);
        void WriteBuilderAssignment(TextWriter output);
        void WriteBuilderPlumbing(TextWriter output);
    }

    class PocoMember : IPocoMember
    {
        string name;
        IPocoType type;

        public PocoMember(string variableName, IPocoType type)
        {
            name = variableName;
            this.type = type;
        }

        public string PublicMemberName()
        {
            return type.PublicMemberName(name);
        }

        public string BackingStoreName()
        {
            return type.BackingStoreName(name);
        }

        public string TempVarName()
        {
            return type.TempVarName(name);
        }

        public string BuilderReifier()
        {
            return type.BuilderReifier(type.BackingStoreName(name));
        }

        public void WriteAssignment(TextWriter output)
        {
            type.WriteAssignment(name, output);
        }

        public void WritePublicMemberDeclaration(TextWriter output)
        {
            type.WritePublicMemberDeclaration(name, output);
        }

        public void WriteBackingStoreDeclaration(TextWriter output)
        {
            type.WriteBackingStoreDeclaration(name, output);
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

        public void WriteConstructorCheck(TextWriter output)
        {
            type.WriteConstructorCheck(name, output);
        }

        public void WriteBuilderDeclaration(TextWriter output)
        {
            type.WriteBuilderDeclaration(name, output);
        }

        public void WriteBuilderAssignment(TextWriter output)
        {
            type.WriteBuilderAssignment(name, output);
        }

        public void WriteBuilderPlumbing(TextWriter output)
        {
            type.WriteBuilderPlumbing(name, output);
        }
    }

    interface IPocoType
    {
        string PublicMemberName(string variableName);
        string BackingStoreName(string variableName);
        string TempVarName(string variableName);
        string BuilderReifier(string variableName);
        void WritePublicMemberDeclaration(string variableName, TextWriter output);
        void WriteBackingStoreDeclaration(string variableName, TextWriter output);
        void WriteFormalParameter(string variableName, TextWriter output);
        void WriteAssignment(string variableName, TextWriter output);
        void WriteEqualityComparison(string variableName, TextWriter output);
        void WriteHash(string variableName, TextWriter output);
        void WriteDeserialization(string variableName, TextWriter output);
        void WriteSerialization(string variableName, TextWriter output);
        void WriteToStringOutput(string variableName, TextWriter output);
        void WriteConstructorCheck(string variableName, TextWriter output);
        void WriteBuilderDeclaration(string variableName, TextWriter output);
        void WriteBuilderAssignment(string variableName, TextWriter output);
        void WriteBuilderPlumbing(string variableName, TextWriter output);
    }

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

        public virtual void WriteToStringOutput(string variableName, TextWriter output)
        {
            output.Write("\t\t\tToStringHelper.WriteMember(result, \"");
            output.Write(variableName);
            output.Write("\", ");
            output.Write(BackingStoreName(variableName));
            output.Write(", ToStringHelper.FormatValue, format);");
        }

        public virtual void WriteConstructorCheck(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\tif (!ConstructorHelper.CheckValue({0})) " +
                "throw new global::System.ArgumentNullException(\"{1}\");",
                TempVarName(variableName), variableName);
        }

        public virtual void WriteBuilderDeclaration(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\tprivate " + BuilderTypeName + " "
                + BackingStoreName(variableName) + ";");
        }

        public virtual void WriteBuilderAssignment(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t\t" + BackingStoreName(variableName) + " = " + BuilderUnreifier(TempVarName(variableName)) + ";");
        }

        public virtual void WriteBuilderPlumbing(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t// --- " + variableName + " ---");

            output.WriteLine("\t\t\tpublic Builder With" + PublicMemberName(variableName) + "(" + TypeName + " value)");
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tif (!ConstructorHelper.CheckValue(value)) " +
                "throw new global::System.ArgumentNullException(\"value\");");
            output.WriteLine("\t\t\t\t" + BackingStoreName(variableName) + " = " + BuilderUnreifier("value") + ";");
            output.WriteLine("\t\t\t\treturn this;");
            output.WriteLine("\t\t\t}");
            output.WriteLine("\t\t\tpublic " + TypeName + " " + PublicMemberName(variableName));
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tget");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\treturn " + BuilderReifier(BackingStoreName(variableName)) + ";");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t\tset");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\tif (!ConstructorHelper.CheckValue(value)) " +
                "throw new global::System.ArgumentNullException(\"value\");");
            output.WriteLine("\t\t\t\t\t" + BackingStoreName(variableName) + " = " + BuilderUnreifier("value") + ";");
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

        public override void WriteBuilderPlumbing(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t// --- " + variableName + " ---");

            output.WriteLine("\t\t\tpublic Builder With" + PublicMemberName(variableName) + "(" + TypeName + " value)");
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tif (!ConstructorHelper.CheckValue(value)) " +
                "throw new global::System.ArgumentNullException(\"value\");");
            output.WriteLine("\t\t\t\t" + BackingStoreName(variableName) + " = " + BuilderUnreifier("value") + ";");
            output.WriteLine("\t\t\t\treturn this;");
            output.WriteLine("\t\t\t}");

            output.WriteLine("\t\t\tpublic " + BuilderTypeName + " " + PublicMemberName(variableName));
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tget");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\treturn " + BackingStoreName(variableName) + ";");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t}");
        }
    }

    class ArrayType : IPocoType
    {
        private PrimitiveType elementType;

        public ArrayType(PrimitiveType baseType)
        {
            this.elementType = baseType;
        }

#if TEST_POCO_NAME_GEN
        public string PublicMemberName(string variableName)
        {
            return "pubbsl_" + variableName;
        }

        public string BackingStoreName(string variableName)
        {
            return "privsl_" + variableName;
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
            return "null";
        }
#endif

        public void WriteAssignment(string variableName, TextWriter output)
        {
            output.Write("this.");
            output.Write(BackingStoreName(variableName));
            output.Write(" = global::System.Linq.Enumerable.ToArray(");
            output.Write(TempVarName(variableName));
            output.WriteLine(");");
            output.Write("\t\t\tthis.");
            output.Write(PublicMemberName(variableName));
            output.Write(" = new ListWrapper<");
            output.Write(elementType.TypeName);
            output.Write(">(");
            output.Write(BackingStoreName(variableName));
            output.WriteLine(");");
        }

        public void WriteBackingStoreDeclaration(string variableName, TextWriter output)
        {
            output.Write("private global::System.Collections.Generic.IList<");
            output.Write(elementType.TypeName);
            output.Write("> ");
            output.Write(BackingStoreName(variableName));
            output.WriteLine(";");
        }

        public void WritePublicMemberDeclaration(string variableName, TextWriter output)
        {
            output.Write("public global::System.Collections.Generic.IReadOnlyList<");
            output.Write(elementType.TypeName);
            output.Write("> ");
            output.Write(PublicMemberName(variableName));
            output.Write(" { get; private set; }");
        }

        public void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(TempVarName(variableName));
            output.Write(" = SerializationHelpers.DeserializeList(input, ");
            output.Write(elementType.DeserializerMethod);
            output.Write(");");
        }

        public void WriteEqualityComparison(string variableName, TextWriter output)
        {
            output.Write("EquatableHelper.ListEquals(this.");
            output.Write(BackingStoreName(variableName));
            output.Write(", other.");
            output.Write(BackingStoreName(variableName));
            output.Write(", EquatableHelper.AreEqual)");
        }

        public void WriteFormalParameter(string variableName, TextWriter output)
        {
            output.Write("global::System.Collections.Generic.IEnumerable<");
            output.Write(elementType.TypeName);
            output.Write("> ");
            output.Write(TempVarName(variableName));
        }

        public void WriteHash(string variableName, TextWriter output)
        {
            output.Write("HashHelper.GetListHashCode(");
            output.Write(BackingStoreName(variableName));
            output.Write(")");
        }

        public void WriteSerialization(string variableName, TextWriter output)
        {
            output.Write("SerializationHelpers.SerializeList(");
            output.Write(BackingStoreName(variableName));
            output.Write(", output, SerializationHelpers.Serialize);");
        }

        public void WriteToStringOutput(string variableName, TextWriter output)
        {
            output.Write("\t\t\tToStringHelper.WriteArrayMember(result, \"");
            output.Write(variableName);
            output.Write("\", ");
            output.Write(BackingStoreName(variableName));
            output.Write(", ToStringHelper.FormatValue, format);");
        }

        public virtual void WriteConstructorCheck(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\tif (!ConstructorHelper.CheckArrayValue({0}, " +
                "ConstructorHelper.CheckValue)) throw new " +
                "global::System.ArgumentNullException(\"{1}\", " +
                "\"Array contains null value\");",
                TempVarName(variableName),
                variableName);
        }

        public virtual void WriteBuilderDeclaration(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t//private global::System.Collections.Generic.IList<"
                + elementType.BuilderTypeName + "> "
                + BackingStoreName(variableName) + ";");
        }

        public virtual void WriteBuilderAssignment(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t\t//" + BackingStoreName(variableName) + " = null;");
        }

        public void WriteBuilderPlumbing(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t//" + variableName);
        }
    }

    class DictionaryType : IPocoType
    {
        private PrimitiveType keyType;
        private PrimitiveType valueType;

#if TEST_POCO_NAME_GEN
        public string PublicMemberName(string variableName)
        {
            return "pubbsd_" + variableName;
        }

        public string BackingStoreName(string variableName)
        {
            return "privsd_" + variableName;
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
            return "null";
        }
#endif

        public DictionaryType(PrimitiveType keyType, PrimitiveType valueType)
        {
            this.keyType = keyType;
            this.valueType = valueType;
        }

        public void WriteAssignment(string variableName, TextWriter output)
        {
            output.Write("this.");
            output.Write(BackingStoreName(variableName));
            output.Write(" = new global::System.Collections.Generic.SortedDictionary<");
            output.Write(keyType.TypeName);
            output.Write(", ");
            output.Write(valueType.TypeName);
            output.Write(">(");
            output.Write(TempVarName(variableName));
            output.WriteLine(");");

            output.Write("\t\t\tthis.");
            output.Write(PublicMemberName(variableName));
            output.Write(" = new DictionaryWrapper<");
            output.Write(keyType.TypeName);
            output.Write(", ");
            output.Write(valueType.TypeName);
            output.Write(">(");
            output.Write(BackingStoreName(variableName));
            output.Write(");");
        }

        public void WriteBackingStoreDeclaration(string variableName, TextWriter output)
        {
            output.Write("private global::System.Collections.Generic.SortedDictionary<");
            output.Write(keyType.TypeName);
            output.Write(", ");
            output.Write(valueType.TypeName);
            output.Write("> ");
            output.Write(BackingStoreName(variableName));
            output.WriteLine(";");
        }

        public void WritePublicMemberDeclaration(string variableName, TextWriter output)
        { 
            output.Write("public global::System.Collections.Generic.IReadOnlyDictionary<");
            output.Write(keyType.TypeName);
            output.Write(", ");
            output.Write(valueType.TypeName);
            output.Write("> ");
            output.Write(PublicMemberName(variableName));
            output.Write(" { get; private set; }");
        }

        public void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(TempVarName(variableName));
            output.Write(" = SerializationHelpers.DeserializeDictionary(input, ");
            output.Write(keyType.DeserializerMethod);
            output.Write(", ");
            output.Write(valueType.DeserializerMethod);
            output.Write(");");
        }

        public void WriteEqualityComparison(string variableName, TextWriter output)
        {
            output.Write("EquatableHelper.DictionaryEquals(this.");
            output.Write(BackingStoreName(variableName));
            output.Write(", other.");
            output.Write(BackingStoreName(variableName));
            output.Write(", EquatableHelper.AreEqual)");
        }

        public void WriteFormalParameter(string variableName, TextWriter output)
        {
            output.Write("global::System.Collections.Generic.IDictionary<");
            output.Write(keyType.TypeName);
            output.Write(", ");
            output.Write(valueType.TypeName);
            output.Write("> ");
            output.Write(TempVarName(variableName));
        }

        public void WriteHash(string variableName, TextWriter output)
        {
            output.Write("HashHelper.GetDictionaryHashCode(");
            output.Write(BackingStoreName(variableName));
            output.Write(")");
        }

        public void WriteSerialization(string variableName, TextWriter output)
        {
            output.Write("SerializationHelpers.SerializeDictionary(");
            output.Write(BackingStoreName(variableName));
            output.Write(", output, SerializationHelpers.Serialize, ");
            output.Write("SerializationHelpers.Serialize);");
        }

        public void WriteToStringOutput(string variableName, TextWriter output)
        {
            output.Write("\t\t\tToStringHelper.WriteDictionaryMember(result, \"");
            output.Write(variableName);
            output.Write("\", ");
            output.Write(BackingStoreName(variableName));
            output.Write(", ToStringHelper.FormatValue, ToStringHelper.FormatValue, format);");
        }

        public virtual void WriteConstructorCheck(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\tif (!ConstructorHelper.CheckDictionaryValue({0}, " +
                "ConstructorHelper.CheckValue, ConstructorHelper.CheckValue)) throw new " +
                "global::System.ArgumentNullException(\"{1}\", " +
                "\"Dictionary contains null value\");",
                TempVarName(variableName),
                variableName);
        }

        public virtual void WriteBuilderDeclaration(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t//private global::System.Collections.Generic.SortedDictionary<"
                + keyType.TypeName + ", " + valueType.BuilderTypeName + "> "
                + BackingStoreName(variableName) + ";");
        }

        public virtual void WriteBuilderAssignment(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t\t//" + BackingStoreName(variableName) + " = null;");
        }

        public void WriteBuilderPlumbing(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t//" + variableName);
        }
    }
}
