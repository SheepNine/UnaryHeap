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

        public string FormalParameterType
        {
            get
            {
                return "global::System.Collections.Generic.IEnumerable<{0}>"
                    .ICFormat(elementType.TypeName);
            }
        }

        public virtual string ConstructorCheck(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "if (!ConstructorHelper.CheckArrayValue({0}, " +
                "ConstructorHelper.CheckValue, {1})) throw new " +
                "global::System.ArgumentNullException(nameof({0}), " +
                "\"Array contains null value\");",
                variableName,
                elementType.IsNullable.ToToken());
        }
    }
}
