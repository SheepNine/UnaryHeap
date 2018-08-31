using System.IO;

namespace Pocotheosis.MemberTypes
{
    class DictionaryType : IPocoType
    {
        private PrimitiveType keyType;
        private PrimitiveType valueType;

#if TEST_POCO_NAME_GEN
        public string PublicMemberName(string variableName)
        {
            return "pubbsd_" + variableName;
        }

        public string BackingStoreName(string variableName)
        {
            return "privsd_" + variableName;
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

        public DictionaryType(PrimitiveType keyType, PrimitiveType valueType)
        {
            this.keyType = keyType;
            this.valueType = valueType;
        }

        public void WriteAssignment(string variableName, TextWriter output)
        {
            output.Write("this.");
            output.Write(BackingStoreName(variableName));
            output.Write(" = new global::System.Collections.Generic.SortedDictionary<");
            output.Write(keyType.TypeName);
            output.Write(", ");
            output.Write(valueType.TypeName);
            output.Write(">(");
            output.Write(TempVarName(variableName));
            output.WriteLine(");");

            output.Write("\t\t\tthis.");
            output.Write(PublicMemberName(variableName));
            output.Write(" = new DictionaryWrapper<");
            output.Write(keyType.TypeName);
            output.Write(", ");
            output.Write(valueType.TypeName);
            output.Write(">(");
            output.Write(BackingStoreName(variableName));
            output.Write(");");
        }

        public void WriteBackingStoreDeclaration(string variableName, TextWriter output)
        {
            output.Write("private global::System.Collections.Generic.SortedDictionary<");
            output.Write(keyType.TypeName);
            output.Write(", ");
            output.Write(valueType.TypeName);
            output.Write("> ");
            output.Write(BackingStoreName(variableName));
            output.WriteLine(";");
        }

        public void WritePublicMemberDeclaration(string variableName, TextWriter output)
        {
            output.Write("public global::System.Collections.Generic.IReadOnlyDictionary<");
            output.Write(keyType.TypeName);
            output.Write(", ");
            output.Write(valueType.TypeName);
            output.Write("> ");
            output.Write(PublicMemberName(variableName));
            output.Write(" { get; private set; }");
        }

        public void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var ");
            output.Write(TempVarName(variableName));
            output.Write(" = SerializationHelpers.DeserializeDictionary(input, ");
            output.Write(keyType.DeserializerMethod);
            output.Write(", ");
            output.Write(valueType.DeserializerMethod);
            output.Write(");");
        }

        public void WriteEqualityComparison(string variableName, TextWriter output)
        {
            output.Write("EquatableHelper.DictionaryEquals(this.");
            output.Write(BackingStoreName(variableName));
            output.Write(", other.");
            output.Write(BackingStoreName(variableName));
            output.Write(", EquatableHelper.AreEqual)");
        }

        public void WriteFormalParameter(string variableName, TextWriter output)
        {
            output.Write("global::System.Collections.Generic.IDictionary<");
            output.Write(keyType.TypeName);
            output.Write(", ");
            output.Write(valueType.TypeName);
            output.Write("> ");
            output.Write(TempVarName(variableName));
        }

        public void WriteHash(string variableName, TextWriter output)
        {
            output.Write("HashHelper.GetDictionaryHashCode(");
            output.Write(BackingStoreName(variableName));
            output.Write(")");
        }

        public void WriteSerialization(string variableName, TextWriter output)
        {
            output.Write("SerializationHelpers.SerializeDictionary(");
            output.Write(BackingStoreName(variableName));
            output.Write(", output, SerializationHelpers.Serialize, ");
            output.Write("SerializationHelpers.Serialize);");
        }

        public void WriteToStringOutput(string variableName, TextWriter output)
        {
            output.Write("\t\t\tToStringHelper.WriteDictionaryMember(result, \"");
            output.Write(variableName);
            output.Write("\", ");
            output.Write(BackingStoreName(variableName));
            output.Write(", ToStringHelper.FormatValue, ToStringHelper.FormatValue, format);");
        }

        public virtual void WriteConstructorCheck(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\tif (!ConstructorHelper.CheckDictionaryValue({0}, " +
                "ConstructorHelper.CheckValue, ConstructorHelper.CheckValue)) throw new " +
                "global::System.ArgumentNullException(\"{1}\", " +
                "\"Dictionary contains null value\");",
                TempVarName(variableName),
                variableName);
        }

        public virtual void WriteBuilderDeclaration(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t//private global::System.Collections.Generic.SortedDictionary<"
                + keyType.TypeName + ", " + valueType.BuilderTypeName + "> "
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
