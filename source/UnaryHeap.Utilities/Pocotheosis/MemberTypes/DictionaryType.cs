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
            return string.Format("BuilderHelper.ReifyDictionary({0}, t => {1})",
                variableName, valueType.BuilderReifier("t"));
        }
#endif

        public DictionaryType(PrimitiveType keyType, PrimitiveType valueType)
        {
            this.keyType = keyType;
            this.valueType = valueType;
        }

        public void WriteAssignment(string variableName, TextWriter output)
        {
            output.WriteLine("this.{0} = new {4}.SortedDictionary<{1}, {2}>({3});",
                BackingStoreName(variableName), keyType.TypeName,
                valueType.TypeName, TempVarName(variableName),
                "global::System.Collections.Generic");

            output.Write("\t\t\tthis.{0}= new DictionaryWrapper<{1}, {2}>({3});",
                PublicMemberName(variableName), keyType.TypeName,
                valueType.TypeName, BackingStoreName(variableName));
        }

        public void WriteBackingStoreDeclaration(string variableName, TextWriter output)
        {
            output.Write("private {3}.SortedDictionary<{0}, {1}> {2};",
                keyType.TypeName, valueType.TypeName, BackingStoreName(variableName),
                "global::System.Collections.Generic");
        }

        public void WritePublicMemberDeclaration(string variableName, TextWriter output)
        {
            output.Write("public {3}.IReadOnlyDictionary<{0}, {1}> {2} {{ get; private set; }}",
                keyType.TypeName, valueType.TypeName, PublicMemberName(variableName),
                "global::System.Collections.Generic");
        }

        public void WriteDeserialization(string variableName, TextWriter output)
        {
            output.Write("var {0} = SerializationHelpers.DeserializeDictionary(input, {1}, {2});",
                TempVarName(variableName), keyType.DeserializerMethod,
                valueType.DeserializerMethod);
        }

        public void WriteEqualityComparison(string variableName, TextWriter output)
        {
            output.Write(
                "EquatableHelper.DictionaryEquals(this.{0}, other.{0}, EquatableHelper.AreEqual)",
                BackingStoreName(variableName));
        }

        public void WriteFormalParameter(string variableName, TextWriter output)
        {
            output.Write("global::System.Collections.Generic.IDictionary<{0}, {1}> {2}",
                keyType.TypeName, valueType.TypeName, TempVarName(variableName));
        }

        public void WriteHash(string variableName, TextWriter output)
        {
            output.Write("HashHelper.GetDictionaryHashCode({0})", 
                BackingStoreName(variableName));
        }

        public void WriteSerialization(string variableName, TextWriter output)
        {
            output.Write("SerializationHelpers.SerializeDictionary({0}, output, {1}, {1});",
                BackingStoreName(variableName),
                "SerializationHelpers.Serialize");
        }

        public string ToStringOutput(string variableName)
        {
            return @"{
                target.Write(""("");
                target.IncreaseIndent();
                var separator = """";
                foreach (var iter in " + variableName + @")
                {
                    target.Write(separator);
                    separator = "","";
                    target.WriteLine();
                    " + keyType.ToStringOutput("iter.Key") + @"
                    target.Write("" -> "");
                    " + valueType.ToStringOutput("iter.Value") + @"
                }
                target.DecreaseIndent();
                if (" + variableName + @".Count > 0)
                    target.WriteLine();
                target.Write("")"");
            }";
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
            output.WriteLine("\t\t\tprivate {3}.SortedDictionary<{0}, {1}> {2};",
                keyType.TypeName, valueType.BuilderTypeName, BackingStoreName(variableName),
                "global::System.Collections.Generic");
        }

        public virtual void WriteBuilderAssignment(string variableName, TextWriter output)
        {
            output.WriteLine("\t\t\t\t{0} = BuilderHelper.UnreifyDictionary({1}, t => {2});",
                BackingStoreName(variableName),TempVarName(variableName),
                valueType.BuilderUnreifier("t"));
        }

        public void WriteBuilderPlumbing(string variableName, string singularName,
            TextWriter output)
        {
            output.WriteLine(@"            // {0}
            public {4} Get{6}({2} key)
            {{
                return {1}[key];
            }}

            public void Set{6}({2} key, {3} value)
            {{
                if (!ConstructorHelper.CheckValue(value))
                    throw new global::System.ArgumentNullException(""value"");
                {1}[key] = {5};
            }}

            public void Remove{6}({2} key)
            {{
                {1}.Remove(key);
            }}

            public void Clear{0}()
            {{
                {1}.Clear();
            }}

            public bool Contains{6}Key({2} key)
            {{
                return {1}.ContainsKey(key);
            }}

            public int Count{0}
            {{
                get {{ return {1}.Count; }}
            }}

            public global::System.Collections.Generic.IEnumerable<{2}> {6}Keys
            {{
                get {{ return {1}.Keys; }}
            }}

            public global::System.Collections.Generic.IEnumerable<
                global::System.Collections.Generic.KeyValuePair<{2}, {4}>> {6}Entries
            {{
                get {{ return {1}; }}
            }}",
            PublicMemberName(variableName), BackingStoreName(variableName), keyType.TypeName,
            valueType.TypeName, valueType.BuilderTypeName, valueType.BuilderUnreifier("value"),
            PublicMemberName(singularName));
        }
    }
}
