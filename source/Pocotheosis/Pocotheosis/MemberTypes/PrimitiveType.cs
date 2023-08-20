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

        public virtual bool IsEnum
        {
            get { return false; }
        }

        public virtual bool IsNullable
        {
            get { return false; }
        }

        public virtual string SerializerMethod
        {
            get { return "SerializationHelpers.Serialize"; }
        }

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
            if (IsNullable)
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} == null ? 0x0EADBEEF : {0}.GetHashCode()",
                    BackingStoreName(variableName));
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0}.GetHashCode()",
                    BackingStoreName(variableName));
            }
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
                "{0} = {1}(input, {2});",
                TempVarName(variableName), JsonDeserializerMethod,
                IsNullable.ToToken());
        }

        public virtual string GetSerializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0}({1}, output);",
                SerializerMethod,
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
                "if (!ConstructorHelper.CheckValue({0}, {1})) " +
                "throw new global::System.ArgumentNullException(nameof({0}));",
                TempVarName(variableName), IsNullable.ToToken());
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
            output.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "\t\t\t\t\tif (!ConstructorHelper.CheckValue(value, {0})) " +
                "throw new global::System.ArgumentNullException(nameof(value));",
                IsNullable.ToToken()));
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
            return string.Format(CultureInfo.InvariantCulture,
                "target.Write({0} == null ? \"null\" : \"'\" + {0} + \"'\");",
                variableName);
        }
    }

    class NullableStringType : StringType
    {
        public static new readonly NullableStringType Instance = new NullableStringType();

        public override bool IsNullable { get { return true; } }
    }

    class EnumType : PrimitiveType
    {
        PocoEnumDefinition enumType;

        public EnumType(PocoEnumDefinition enumType)
        {
            this.enumType = enumType;
        }
        public override string TypeName { get { return enumType.Name; } }
        public override bool IsEnum { get { return true; } }
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
        bool isNullable;

        public override bool IsNullable { get { return isNullable; } }
        public override string SerializerMethod
        {
            get
            {
                if (isNullable)
                    return "SerializationHelpers.SerializeWithId";
                else
                    return "SerializationHelpers.Serialize";
            }
        }

        public ClassType(string className, bool isNullable)
        {
            this.className = className;
            this.isNullable = isNullable;
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
            get
            {
                if (isNullable)
                    return string.Format(CultureInfo.InvariantCulture,
                        "DeserializeWithId<{0}>", TypeName);
                else
                    return className + ".Deserialize";
            }
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
            if (isNullable)
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} == null ? null : {0}.Build()", variableName);
            else
                return string.Format(CultureInfo.InvariantCulture,
                    "{0}.Build()", variableName);
        }

        public override string BuilderUnreifier(string variableName)
        {
            if (isNullable)
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} == null ? null : {0}.ToBuilder()", variableName);
            else
                return string.Format(CultureInfo.InvariantCulture,
                    "{0}.ToBuilder()", variableName);
        }

        public override string ToStringOutput(string variableName)
        {
            if (isNullable)
            {
                return string.Format(CultureInfo.InvariantCulture,
                    @"if ({0} == null) {{ target.Write(""null""); }} " +
                    @"else {{ {0}.WriteIndented(target); }}",
                    variableName);
            }
            else
                return variableName + ".WriteIndented(target);";
        }

        public override void WriteBuilderPlumbing(string variableName, string singularName,
            TextWriter output)
        {
            output.WriteLine("\t\t\t// --- " + variableName + " ---");

            output.WriteLine("\t\t\tpublic Builder With" + PublicMemberName(variableName) +
                "(" + TypeName + " value)");
            output.WriteLine("\t\t\t{");
            output.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "\t\t\t\tif (!ConstructorHelper.CheckValue(value, {0})) " +
                "throw new global::System.ArgumentNullException(nameof(value));",
                IsNullable.ToToken()));
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
