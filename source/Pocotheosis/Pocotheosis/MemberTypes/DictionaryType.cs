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

        public DictionaryType(PrimitiveType keyType, PrimitiveType valueType)
        {
            this.keyType = keyType;
            this.valueType = valueType;
        }

        public string FormalParameter(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "global::System.Collections.Generic.IDictionary<{0}, {1}> {2}",
                keyType.TypeName, valueType.TypeName, TempVarName(variableName));
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
