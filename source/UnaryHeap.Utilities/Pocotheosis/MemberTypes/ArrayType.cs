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

        public void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(TempVarName(variableName));
            output.Write(" = SerializationHelpers.DeserializeList(input, ");
            output.Write(elementType.DeserializerMethod);
            output.Write(");");
        }

        public void WriteEqualityComparison(string variableName, TextWriter output)
        {
            output.Write("EquatableHelper.ListEquals(this.");
            output.Write(BackingStoreName(variableName));
            output.Write(", other.");
            output.Write(BackingStoreName(variableName));
            output.Write(", EquatableHelper.AreEqual)");
        }

        public void WriteFormalParameter(string variableName, TextWriter output)
        {
            output.Write("global::System.Collections.Generic.IEnumerable<");
            output.Write(elementType.TypeName);
            output.Write("> ");
            output.Write(TempVarName(variableName));
        }

        public void WriteHash(string variableName, TextWriter output)
        {
            output.Write("HashHelper.GetListHashCode(");
            output.Write(BackingStoreName(variableName));
            output.Write(")");
        }

        public void WriteSerialization(string variableName, TextWriter output)
        {
            output.Write("SerializationHelpers.SerializeList(");
            output.Write(BackingStoreName(variableName));
            output.Write(", output, SerializationHelpers.Serialize);");
        }

        public void WriteToStringOutput(string variableName, TextWriter output)
        {
            output.Write("\t\t\tToStringHelper.WriteArrayMember(result, \"");
            output.Write(variableName);
            output.Write("\", ");
            output.Write(BackingStoreName(variableName));
            output.Write(", ToStringHelper.FormatValue, format);");
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

        public void WriteBuilderPlumbing(string variableName, TextWriter output)
        {
            output.WriteLine(@"            //{0}
            public int Num{0}
            {{
                get {{ return {1}.Count; }}
            }}
            
            public {2} Get{0}(int index)
            {{
                return {1}[index];
            }}
            
            public void Set{0}(int index, {3} value)
            {{
                if (!ConstructorHelper.CheckValue(value))
                    throw new global::System.ArgumentNullException(""value"");
                {1}[index] = {4};
            }}
            
            public void Append{0}({3} value)
            {{
                if (!ConstructorHelper.CheckValue(value))
                    throw new global::System.ArgumentNullException(""value"");
                {1}.Add({4});
            }}
            
            public void Insert{0}At(int index, {3} value)
            {{
                if (!ConstructorHelper.CheckValue(value))
                    throw new global::System.ArgumentNullException(""value"");
                {1}.Insert(index, {4});
            }}
            
            public void Remove{0}At(int index)
            {{
                {1}.RemoveAt(index);
            }}
            
            public void Clear{0}()
            {{
                {1}.Clear();
            }}
            
            public global::System.Collections.Generic.IEnumerable<{2}> {0}Values
            {{
                get {{ return {1}; }}
            }}",
            PublicMemberName(variableName), BackingStoreName(variableName),
            elementType.BuilderTypeName, elementType.TypeName,
            elementType.BuilderUnreifier("value"));
        }
    }
}
