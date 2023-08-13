using System.Globalization;
using System.IO;

namespace Pocotheosis.MemberTypes
{
    class DictionaryType : IPocoType
    {
        private PrimitiveType keyType;
        private PrimitiveType valueType;

        public bool IsComparable
        {
            get { return false; }
        }

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
            return string.Format(CultureInfo.InvariantCulture, 
                "BuilderHelper.ReifyDictionary({0}, t => {1})",
                variableName, valueType.BuilderReifier("t"));
        }
#endif

        public DictionaryType(PrimitiveType keyType, PrimitiveType valueType)
        {
            this.keyType = keyType;
            this.valueType = valueType;
        }

        public string Assignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "this.{0} = new {5}.SortedDictionary<{1}, {2}>({3}); " +
                "this.{4} = new WrapperDictionary<{1}, {2}>({0});",
                BackingStoreName(variableName), keyType.TypeName,
                valueType.TypeName, TempVarName(variableName),
                PublicMemberName(variableName),
                "global::System.Collections.Generic");
        }

        public string BackingStoreDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private {3}.SortedDictionary<{0}, {1}> {2};",
                keyType.TypeName, valueType.TypeName, BackingStoreName(variableName),
                "global::System.Collections.Generic");
        }

        public string PublicMemberDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "public {3}.IReadOnlyDictionary<{0}, {1}> {2} {{ get; private set; }}",
                keyType.TypeName, valueType.TypeName, PublicMemberName(variableName),
                "global::System.Collections.Generic");
        }

        public string GetDeserializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "var {0} = SerializationHelpers.DeserializeDictionary(input, {1}, {2});",
                TempVarName(variableName), keyType.DeserializerMethod,
                valueType.DeserializerMethod);
        }

        public string GetJsonDeserializer(string variableName)
        {
            if (keyType.TypeName == "string")
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} = DeserializeJsonObject(input, (key) => key, {1});",
                    TempVarName(variableName),
                    valueType.JsonDeserializerMethod);
            }
            else if (keyType.IsEnum)
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} = DeserializeJsonObject(input, "
                        + "(key) => global::System.Enum.Parse<{1}>(key), {2});",
                    TempVarName(variableName),
                    keyType.TypeName,
                    valueType.JsonDeserializerMethod);
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} = DeserializeDictionary(input, {1}, {2});",
                    TempVarName(variableName), keyType.JsonDeserializerMethod,
                    valueType.JsonDeserializerMethod);
            }
        }

        public string GetEqualityTester(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "EquatableHelper.DictionaryEquals(this.{0}, other.{0}, EquatableHelper.AreEqual)",
                BackingStoreName(variableName));
        }

        public string FormalParameter(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "global::System.Collections.Generic.IDictionary<{0}, {1}> {2}",
                keyType.TypeName, valueType.TypeName, TempVarName(variableName));
        }

        public string GetHasher(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                "HashHelper.GetDictionaryHashCode({0})", 
                BackingStoreName(variableName));
        }

        public string GetSerializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "SerializationHelpers.SerializeDictionary({0}, output, {1}, {1});",
                BackingStoreName(variableName),
                "SerializationHelpers.Serialize");
        }

        public string GetJsonSerializer(string variableName)
        {
            if (keyType.TypeName == "string")
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "SerializeJsonObject(@this.{0}, output, s => s, {1});",
                    PublicMemberName(variableName),
                    "Serialize");
            }
            else if (keyType.IsEnum)
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "SerializeJsonObject(@this.{0}, output, e => e.ToString(), {1});",
                    PublicMemberName(variableName),
                    "Serialize");
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "SerializeDictionary(@this.{0}, output, {1}, {1});",
                    PublicMemberName(variableName),
                    "Serialize");
            }
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

        public virtual string ConstructorCheck(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "if (!ConstructorHelper.CheckDictionaryValue({0}, " +
                "ConstructorHelper.CheckValue, ConstructorHelper.CheckValue)) throw new " +
                "global::System.ArgumentNullException(nameof({0}), " +
                "\"Dictionary contains null value\");",
                TempVarName(variableName));
        }

        public virtual string BuilderDeclaration(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "private {3}.SortedDictionary<{0}, {1}> {2};",
                keyType.TypeName, valueType.BuilderTypeName, BackingStoreName(variableName),
                "global::System.Collections.Generic");
        }

        public virtual string BuilderAssignment(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = BuilderHelper.UnreifyDictionary({1}, t => {2});",
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
                    throw new global::System.ArgumentNullException(nameof(value));
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
