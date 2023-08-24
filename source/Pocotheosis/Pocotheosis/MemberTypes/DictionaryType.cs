using System.Globalization;

namespace Pocotheosis.MemberTypes
{
    partial class DictionaryType : IPocoType
    {
        readonly PrimitiveType keyType;
        readonly PrimitiveType valueType;

        public DictionaryType(PrimitiveType keyType, PrimitiveType valueType)
        {
            this.keyType = keyType;
            this.valueType = valueType;
        }

        public bool NeedsConstructorCheck
        {
            get { return true; }
        }

        public bool IsComparable
        {
            get { return false; }
        }

        public string FormalParameterType 
        {
            get
            {
                return "_nsG_.IDictionary<{0}, {1}>"
                    .ICFormat(keyType.TypeName, valueType.TypeName);
            }
        }

        public virtual string ConstructorCheck(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "if (!ConstructorHelper.CheckDictionaryValue({0}, " +
                "ConstructorHelper.CheckValue, ConstructorHelper.CheckValue, " +
                "{1})) throw new " +
                "_nsS_.ArgumentNullException(nameof({0}), " +
                "\"Dictionary contains null value\");",
                variableName,
                valueType.IsNullable.ToToken());
        }
    }
}
