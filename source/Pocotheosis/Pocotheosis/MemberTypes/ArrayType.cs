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

        public string FormalParameter(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "global::System.Collections.Generic.IEnumerable<{0}> {1}",
                elementType.TypeName, TempVarName(variableName));
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
