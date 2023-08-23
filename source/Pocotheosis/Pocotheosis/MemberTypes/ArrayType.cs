using System.Globalization;
using System.IO;

namespace Pocotheosis.MemberTypes
{
    partial class ArrayType : IPocoType
    {
        private PrimitiveType elementType;

        public ArrayType(PrimitiveType baseType)
        {
            this.elementType = baseType;
        }

        public bool NeedsConstructorCheck
        {
            get { return true; }
        }

        public bool IsComparable
        {
            get { return false; }
        }

        public string TempVarName(string variableName)
        {
            return "t" + variableName;
        }

        public virtual string BuilderReifier(string variableName)
        {
            return "BuilderHelper.ReifyArray(" + variableName + ", t => " +
                elementType.BuilderReifier("t") + ")";
        }

        public string BackingStoreDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private global::System.Collections.Generic.IList<{0}> {1};",
                elementType.TypeName, BackingStoreName(variableName));
        }

        public string PublicMemberDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "public {2}.IReadOnlyList<{0}> {1} {{ get; private set; }}",
                elementType.TypeName,
                PublicMemberName(variableName),
                "global::System.Collections.Generic");
        }

        public string GetDeserializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "var {0} = SerializationHelpers.DeserializeList(input, {1});",
                TempVarName(variableName), elementType.DeserializerMethod);
        }

        public string GetEqualityTester(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "EquatableHelper.ListEquals(this.{0}, other.{0}, EquatableHelper.AreEqual)",
                BackingStoreName(variableName));
        }

        public string FormalParameter(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "global::System.Collections.Generic.IEnumerable<{0}> {1}",
                elementType.TypeName, TempVarName(variableName));
        }

        public string GetHasher(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                "HashHelper.GetListHashCode({0})",
                BackingStoreName(variableName));
        }

        public string GetSerializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "SerializationHelpers.SerializeList({0}, output, {1});",
                BackingStoreName(variableName),
                elementType.SerializerMethod);
        }

        public virtual string ConstructorCheck(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "if (!ConstructorHelper.CheckArrayValue({0}, " +
                "ConstructorHelper.CheckValue, {1})) throw new " +
                "global::System.ArgumentNullException(nameof({0}), " +
                "\"Array contains null value\");",
                TempVarName(variableName),
                elementType.IsNullable.ToToken());
        }
    }
}
