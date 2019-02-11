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

#if TEST_POCO_NAME_GEN
        public string PublicMemberName(string variableName)
        {
            return "pubbsl_" + variableName;
        }

        public string BackingStoreName(string variableName)
        {
            return "privsl_" + variableName;
        }

        public string TempVarName(string variableName)
        {
            return "t_" + variableName;
        }
#else
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
#endif

        public void WriteAssignment(string variableName, TextWriter output)
        {
            output.Write("this.");
            output.Write(BackingStoreName(variableName));
            output.Write(" = global::System.Linq.Enumerable.ToArray(");
            output.Write(TempVarName(variableName));
            output.WriteLine(");");
            output.Write("\t\t\tthis.");
            output.Write(PublicMemberName(variableName));
            output.Write(" = new ListWrapper<");
            output.Write(elementType.TypeName);
            output.Write(">(");
            output.Write(BackingStoreName(variableName));
            output.WriteLine(");");
        }

        public void WriteBackingStoreDeclaration(string variableName, TextWriter output)
        {
            output.Write("private global::System.Collections.Generic.IList<");
            output.Write(elementType.TypeName);
            output.Write("> ");
            output.Write(BackingStoreName(variableName));
            output.WriteLine(";");
        }

        public void WritePublicMemberDeclaration(string variableName, TextWriter output)
        {
            output.Write("public global::System.Collections.Generic.IReadOnlyList<");
            output.Write(elementType.TypeName);
            output.Write("> ");
            output.Write(PublicMemberName(variableName));
            output.Write(" { get; private set; }");
        }

        public string GetDeserializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "var {0} = SerializationHelpers.DeserializeList(input, {1});",
                TempVarName(variableName), elementType.DeserializerMethod);
        }

        public string GetEqualityTester(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "EquatableHelper.ListEquals(this.{0}, other.{0}, EquatableHelper.AreEqual)",
                BackingStoreName(variableName));
        }

        public void WriteFormalParameter(string variableName, TextWriter output)
        {
            output.Write("global::System.Collections.Generic.IEnumerable<");
            output.Write(elementType.TypeName);
            output.Write("> ");
            output.Write(TempVarName(variableName));
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
                "SerializationHelpers.Serialize");
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

        public virtual void WriteConstructorCheck(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\tif (!ConstructorHelper.CheckArrayValue({0}, " +
                "ConstructorHelper.CheckValue)) throw new " +
                "global::System.ArgumentNullException(\"{1}\", " +
                "\"Array contains null value\");",
                TempVarName(variableName),
                variableName);
        }

        public virtual void WriteBuilderDeclaration(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\tprivate global::System.Collections.Generic.IList<"
                + elementType.BuilderTypeName + "> "
                + BackingStoreName(variableName) + ";");
        }

        public virtual void WriteBuilderAssignment(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t\t" + BackingStoreName(variableName) +
                " = BuilderHelper.UnreifyArray(" + TempVarName(variableName) +
                ", t => " + elementType.BuilderUnreifier("t") + ");");
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
                if (!ConstructorHelper.CheckValue(value))
                    throw new global::System.ArgumentNullException(""value"");
                {1}[index] = {4};
            }}
            
            public void Append{5}({3} value)
            {{
                if (!ConstructorHelper.CheckValue(value))
                    throw new global::System.ArgumentNullException(""value"");
                {1}.Add({4});
            }}
            
            public void Insert{5}At(int index, {3} value)
            {{
                if (!ConstructorHelper.CheckValue(value))
                    throw new global::System.ArgumentNullException(""value"");
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
            PublicMemberName(variableName), BackingStoreName(variableName),
            elementType.BuilderTypeName, elementType.TypeName,
            elementType.BuilderUnreifier("value"),
            PublicMemberName(singularName));
        }
    }
}
