﻿namespace Pocotheosis.MemberTypes
{
    sealed partial class DictionaryType : IPocoType
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
            get { return $"_nsG_.IDictionary<{keyType.TypeName}, {valueType.TypeName}>"; }
        }

        public string InputCheck(string variableName)
        {
            return $"CheckValue({variableName}, CheckValue, "
                + $"{valueType.IsNullable.ToToken()})";
        }
    }
}
