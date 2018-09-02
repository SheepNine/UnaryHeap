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
            return "BuilderHelper.ReifyDictionary(" + variableName + ", t => " + valueType.BuilderReifier("t") + ")";
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
            output.WriteLine("\t\t\tprivate global::System.Collections.Generic.SortedDictionary<"
                + keyType.TypeName + ", " + valueType.BuilderTypeName + "> "
                + BackingStoreName(variableName) + ";");
        }

        public virtual void WriteBuilderAssignment(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t\t" + BackingStoreName(variableName) + " = BuilderHelper.UnreifyDictionary(" + TempVarName(variableName) + ", t => " + valueType.BuilderUnreifier("t") + ");");
        }

        public void WriteBuilderPlumbing(string variableName, TextWriter output)
        {
            output.WriteLine(@"			// {0}
			public {4} Get{0}({2} key)
			{{
				return {1}[key];
			}}

			public void Add{0}({2} key, {3} value)
			{{
				if (!ConstructorHelper.CheckValue(value)) throw new global::System.ArgumentNullException(""value"");
				{1}[key] = {5};
			}}

			public void Remove{0}({2} key)
			{{
				{1}.Remove(key);
			}}

			public void Clear{0}()
			{{
				{1}.Clear();
			}}

			public bool Contains{0}Key({2} key)
			{{
				return {1}.ContainsKey(key);
			}}

			public int Count{0}
			{{
				get {{ return {1}.Count; }}
			}}

			public global::System.Collections.Generic.IEnumerable<{2}> {0}Keys
			{{
				get {{ return {1}.Keys; }}
			}}

			public global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<{2}, {4}>> {0}Entries
			{{
				get {{ return {1}; }}
			}}",
            PublicMemberName(variableName), BackingStoreName(variableName), keyType.TypeName, valueType.TypeName, valueType.BuilderTypeName, valueType.BuilderUnreifier("value"));
        }
    }
}
