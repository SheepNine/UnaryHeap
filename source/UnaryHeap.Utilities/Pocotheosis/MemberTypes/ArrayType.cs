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
            return "null";
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
            output.WriteLine("\t\t\t//private global::System.Collections.Generic.IList<"
                + elementType.BuilderTypeName + "> "
                + BackingStoreName(variableName) + ";");
        }

        public virtual void WriteBuilderAssignment(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t\t//" + BackingStoreName(variableName) + " = null;");
        }

        public void WriteBuilderPlumbing(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t//" + variableName);
        }
    }
}
