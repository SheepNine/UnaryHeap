using System;
using System.Globalization;

namespace Pocotheosis.MemberTypes
{
    partial class ArrayType : IPocoType
    {
        readonly PrimitiveType elementType;

        public ArrayType(PrimitiveType elementType)
        {
            this.elementType = elementType;
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
            get { return $"_nsG_.IEnumerable<{elementType.TypeName}>"; }
        }

        public string InputCheck(string variableName)
        {
            return $"ConstructorHelper.CheckArrayValue({variableName}, "
                + $"ConstructorHelper.CheckValue, {elementType.IsNullable.ToToken()})";
        }
    }
}
