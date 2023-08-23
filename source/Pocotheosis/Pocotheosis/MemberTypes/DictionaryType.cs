using System.Globalization;
using System.IO;

namespace Pocotheosis.MemberTypes
{
    partial class DictionaryType : IPocoType
    {
        private PrimitiveType keyType;
        private PrimitiveType valueType;

        public bool IsComparable
        {
            get { return false; }
        }

        public bool NeedsConstructorCheck
        {
            get { return true; }
        }

        public string TempVarName(string variableName)
        {
            return "t" + variableName;
        }

        public virtual string BuilderReifier(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                "BuilderHelper.ReifyDictionary({0}, t => {1})",
                variableName, valueType.BuilderReifier("t"));
        }

        public DictionaryType(PrimitiveType keyType, PrimitiveType valueType)
        {
            this.keyType = keyType;
            this.valueType = valueType;
        }

        public string BackingStoreDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private {3}.SortedDictionary<{0}, {1}> {2};",
                keyType.TypeName, valueType.TypeName, BackingStoreName(variableName),
                "global::System.Collections.Generic");
        }

        public string PublicMemberDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "public {3}.IReadOnlyDictionary<{0}, {1}> {2} {{ get; private set; }}",
                keyType.TypeName, valueType.TypeName, PublicMemberName(variableName),
                "global::System.Collections.Generic");
        }

        public string GetDeserializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "var {0} = SerializationHelpers.DeserializeDictionary(input, {1}, {2});",
                TempVarName(variableName), keyType.DeserializerMethod,
                valueType.DeserializerMethod);
        }

        public string GetEqualityTester(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "EquatableHelper.DictionaryEquals(this.{0}, other.{0}, EquatableHelper.AreEqual)",
                BackingStoreName(variableName));
        }

        public string FormalParameter(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "global::System.Collections.Generic.IDictionary<{0}, {1}> {2}",
                keyType.TypeName, valueType.TypeName, TempVarName(variableName));
        }

        public string GetHasher(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                "HashHelper.GetDictionaryHashCode({0})", 
                BackingStoreName(variableName));
        }

        public string GetSerializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "SerializationHelpers.SerializeDictionary({0}, output, {1}, {2});",
                BackingStoreName(variableName),
                keyType.SerializerMethod,
                valueType.SerializerMethod);
        }

        public virtual string ConstructorCheck(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "if (!ConstructorHelper.CheckDictionaryValue({0}, " +
                "ConstructorHelper.CheckValue, ConstructorHelper.CheckValue, " +
                "{1})) throw new " +
                "global::System.ArgumentNullException(nameof({0}), " +
                "\"Dictionary contains null value\");",
                TempVarName(variableName),
                valueType.IsNullable.ToToken());
        }
    }
}
