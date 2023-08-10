using System.Globalization;
using System.IO;

namespace Pocotheosis.MemberTypes
{
    abstract class PrimitiveType : IPocoType
    {
        public abstract string TypeName { get; }
        public abstract string DeserializerMethod { get; }
        public abstract string JsonDeserializerMethod { get; }

        public virtual bool IsComparable
        {
            get { return true; }
        }

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

        public virtual string PublicMemberDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "public {0} {1} {{ get {{ return {2}; }} }}",
                TypeName, PublicMemberName(variableName), BackingStoreName(variableName));
        }

        public string BackingStoreDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private {0} {1};",
                TypeName, BackingStoreName(variableName));
        }

        public virtual string FormalParameter(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} {1}",
                TypeName, TempVarName(variableName));
        }

        public virtual string Assignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = {1};",
                BackingStoreName(variableName), TempVarName(variableName));
        }

        public string GetEqualityTester(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "EquatableHelper.AreEqual(this.{0}, other.{0})",
                BackingStoreName(variableName));
        }

        public virtual string GetHasher(string variableName)
        {
            return BackingStoreName(variableName) + ".GetHashCode()";
        }

        public virtual string GetDeserializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "var {0} = {1}(input);",
                TempVarName(variableName), DeserializerMethod);
        }

        public virtual string GetJsonDeserializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = {1}(input);",
                TempVarName(variableName), JsonDeserializerMethod);
        }

        public virtual string GetSerializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "SerializationHelpers.Serialize({0}, output);",
                BackingStoreName(variableName));
        }

        public virtual string GetJsonSerializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Serialize(@this.{0}, output);",
                PublicMemberName(variableName));
        }

        public virtual string ToStringOutput(string variableName)
        {
            return "target.Write(" + variableName + ");";
        }

        public virtual string ConstructorCheck(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "if (!ConstructorHelper.CheckValue({0})) " +
                "throw new global::System.ArgumentNullException(nameof({0}));",
                TempVarName(variableName));
        }

        public virtual string BuilderDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private {0} {1};",
                BuilderTypeName, BackingStoreName(variableName));
        }

        public virtual string BuilderAssignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = {1};",
                BackingStoreName(variableName), BuilderUnreifier(TempVarName(variableName)));
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
                "throw new global::System.ArgumentNullException(nameof(value));");
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
        public override string JsonDeserializerMethod
        {
            get { return "DeserializeBool"; }
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
        public override string JsonDeserializerMethod
        {
            get { return "DeserializeSByte"; }
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
        public override string JsonDeserializerMethod
        {
            get { return "DeserializeInt16"; }
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
        public override string JsonDeserializerMethod
        {
            get { return "DeserializeInt32"; }
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
        public override string JsonDeserializerMethod
        {
            get { return "DeserializeInt64"; }
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
        public override string JsonDeserializerMethod
        {
            get { return "DeserializeByte"; }
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
        public override string JsonDeserializerMethod
        {
            get { return "DeserializeUInt16"; }
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
        public override string JsonDeserializerMethod
        {
            get { return "DeserializeUInt32"; }
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
        public override string JsonDeserializerMethod
        {
            get { return "DeserializeUInt64"; }
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
        public override string JsonDeserializerMethod
        {
            get { return "DeserializeString"; }
        }

        public override string ToStringOutput(string variableName)
        {
            return "target.Write(\"'\" + " + variableName + " + \"'\");";
        }
    }

    class EnumType : PrimitiveType
    {
        PocoEnumDefinition enumType;

        public EnumType(PocoEnumDefinition enumType)
        {
            this.enumType = enumType;
        }
        public override string TypeName { get { return enumType.Name; } }
        public override string DeserializerMethod
        {
            get { return "SerializationHelpers.Deserialize" + enumType.Name; }
        }
        public override string JsonDeserializerMethod
        {
            get { return "Deserialize" + enumType.Name; }
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

        public override string TypeName
        {
            get { return className; }
        }

        public override bool IsComparable
        {
            get { return false; }
        }

        public override string DeserializerMethod
        {
            get { return className + ".Deserialize"; }
        }

        public override string JsonDeserializerMethod
        {
            get { return "Deserialize" + className; }
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
                "throw new global::System.ArgumentNullException(nameof(value));");
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
