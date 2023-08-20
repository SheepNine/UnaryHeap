using System.Globalization;
using System.IO;

namespace Pocotheosis.MemberTypes
{
    class ArrayType : IPocoType
    {
        private PrimitiveType elementType;

        public ArrayType(PrimitiveType baseType)
        {
            this.elementType = baseType;
        }

        public bool IsComparable
        {
            get { return false; }
        }

        public string PublicMemberName(string variableName)
        {
            return variableName;
        }

        public string BackingStoreName(string variableName)
        {
            return "__" + variableName;
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

        public string Assignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "this.{0} = global::System.Linq.Enumerable.ToArray({1}); " +
                "this.{2} = new ListWrapper<{3}>({0});",
                BackingStoreName(variableName), TempVarName(variableName),
                PublicMemberName(variableName), elementType.TypeName);
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

        public string GetJsonDeserializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = DeserializeList(input, {1}, {2});",
                TempVarName(variableName), elementType.JsonDeserializerMethod,
                elementType.IsNullable.ToToken());
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

        public string GetJsonSerializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "SerializeList(@this.{0}, output, {1});",
                PublicMemberName(variableName),
                "Serialize");
        }

        public string ToStringOutput(string variableName)
        {
            return @"{
                target.Write(""["");
                var separator = """";
                foreach (var iter in " + variableName + @")
                {
                    target.Write(separator);
                    separator = "", "";
                    " + elementType.ToStringOutput("iter") + @"
                }
                target.Write(""]"");
            }";
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

        public virtual string BuilderDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private global::System.Collections.Generic.IList<{0}> {1};",
                elementType.BuilderTypeName, BackingStoreName(variableName));
        }

        public virtual string BuilderAssignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = BuilderHelper.UnreifyArray({1}, t => {2});",
                BackingStoreName(variableName), TempVarName(variableName),
                elementType.BuilderUnreifier("t"));
        }

        public void WriteBuilderPlumbing(string variableName, string singularName,
            TextWriter output)
        {
            output.WriteLine(@"            //{0}
            public int Num{0}
            {{
                get {{ return {1}.Count; }}
            }}
            
            public {2} Get{5}(int index)
            {{
                return {1}[index];
            }}
            
            public void Set{5}(int index, {3} value)
            {{
                if (!ConstructorHelper.CheckValue(value, {6}))
                    throw new global::System.ArgumentNullException(nameof(value));
                {1}[index] = {4};
            }}
            
            public void Append{5}({3} value)
            {{
                if (!ConstructorHelper.CheckValue(value, {6}))
                    throw new global::System.ArgumentNullException(nameof(value));
                {1}.Add({4});
            }}
            
            public void Insert{5}At(int index, {3} value)
            {{
                if (!ConstructorHelper.CheckValue(value, {6}))
                    throw new global::System.ArgumentNullException(nameof(value));
                {1}.Insert(index, {4});
            }}
            
            public void Remove{5}At(int index)
            {{
                {1}.RemoveAt(index);
            }}
            
            public void Clear{0}()
            {{
                {1}.Clear();
            }}
            
            public global::System.Collections.Generic.IEnumerable<{2}> {5}Values
            {{
                get {{ return {1}; }}
            }}",
            PublicMemberName(variableName),
            BackingStoreName(variableName),
            elementType.BuilderTypeName,
            elementType.TypeName,
            elementType.BuilderUnreifier("value"),
            PublicMemberName(singularName),
            elementType.IsNullable.ToToken());
        }
    }
}
