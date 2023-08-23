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

        public DictionaryType(PrimitiveType keyType, PrimitiveType valueType)
        {
            this.keyType = keyType;
            this.valueType = valueType;
        }

        public string FormalParameterType 
        {
            get
            {
                return "global::System.Collections.Generic.IDictionary<{0}, {1}>"
                    .ICFormat(keyType.TypeName, valueType.TypeName);
            }
        }

        public virtual string ConstructorCheck(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "if (!ConstructorHelper.CheckDictionaryValue({0}, " +
                "ConstructorHelper.CheckValue, ConstructorHelper.CheckValue, " +
                "{1})) throw new " +
                "global::System.ArgumentNullException(nameof({0}), " +
                "\"Dictionary contains null value\");",
                variableName,
                valueType.IsNullable.ToToken());
        }
    }
}
