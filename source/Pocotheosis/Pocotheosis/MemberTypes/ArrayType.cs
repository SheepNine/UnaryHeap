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
            get
            {
                return "_nsG_.IEnumerable<{0}>"
                    .ICFormat(elementType.TypeName);
            }
        }

        public virtual string ConstructorCheck(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "if (!ConstructorHelper.CheckArrayValue({0}, " +
                "ConstructorHelper.CheckValue, {1})) throw new " +
                "_nsS_.ArgumentNullException(nameof({0}), " +
                "\"Array contains null value\");",
                variableName,
                elementType.IsNullable.ToToken());
        }
    }
}
