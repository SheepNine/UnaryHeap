using System.Globalization;

namespace Pocotheosis.MemberTypes
{

    abstract partial class PrimitiveType : IPocoType
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

        public virtual bool NeedsConstructorCheck
        {
            get { return false; }
        }

        public virtual string SerializerMethod
        {
            get { return "SerializationHelpers.Serialize"; }
        }

        public string TempVarName(string variableName)
        {
            return "t" + variableName;
        }

        public string FormalParameter(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} {1}",
                TypeName, TempVarName(variableName));
        }

        public string ConstructorCheck(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "if (!ConstructorHelper.CheckValue({0}, {1})) " +
                "throw new global::System.ArgumentNullException(nameof({0}));",
                TempVarName(variableName), IsNullable.ToToken());
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

    partial class StringType : PrimitiveType
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

        public override bool NeedsConstructorCheck
        {
            get { return true; }
        }
    }

    class NullableStringType : StringType
    {
        public static new readonly NullableStringType Instance = new NullableStringType();

        public override bool IsNullable
        {
            get { return true; }
        }

        public override bool NeedsConstructorCheck
        {
            get { return false; }
        }
    }

    partial class EnumType : PrimitiveType
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
    }

    partial class ClassType : PrimitiveType
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

        public override bool NeedsConstructorCheck
        {
            get { return !isNullable; }
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
    }
}
