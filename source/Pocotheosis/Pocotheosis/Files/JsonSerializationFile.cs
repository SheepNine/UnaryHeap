using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteJsonSerializationFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using (var output = File.CreateText(outputFileName))
            {
                WriteNamespaceHeader(dataModel, output);

                output.WriteLine("\tpublic static partial class PocoJson");
                output.Write("\t{");

                WriteBuiltInHelperMethods(output);

                foreach (var enume in dataModel.Enums)
                    WriteEnumJsonSerializationDeclaration(enume, output);

                foreach (var clasz in dataModel.Classes)
                {
                    output.WriteLine();
                    WriteClassJsonSerializationDeclaration(clasz, output);
                }

                output.WriteLine("\t}");

                WriteNamespaceFooter(output);
            }
        }

        static void WriteBuiltInHelperMethods(StreamWriter output)
        {
            output.WriteLine(@"
        static void WarmReader(global::Newtonsoft.Json.JsonReader input)
        {
            if (input.TokenType == global::Newtonsoft.Json.JsonToken.None)
                AdvanceToken(input);
        }

        static void RequireTokenType(global::Newtonsoft.Json.JsonReader input,
            global::Newtonsoft.Json.JsonToken expected)
        {
            if (input.TokenType != expected)
                throw new global::System.IO.InvalidDataException(
                    string.Format(
                        global::System.Globalization.CultureInfo.InvariantCulture,
                        ""Expected {0} token but found {1} token"",
                        expected, input.TokenType));
        }

        static void AdvanceToken(global::Newtonsoft.Json.JsonReader input)
        {
            if (!input.Read())
                throw new global::System.IO.InvalidDataException(
                    ""Unexpected end of stream"");
        }

        static void ConsumeTokenType(global::Newtonsoft.Json.JsonReader input,
            global::Newtonsoft.Json.JsonToken expected)
        {
            RequireTokenType(input, expected);
            AdvanceToken(input);
        }

        static string GetPropertyName(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, global::Newtonsoft.Json.JsonToken.PropertyName);
            var result = (string)input.Value;
            AdvanceToken(input);
            return result;
        }

        static void RequirePropertyName(global::Newtonsoft.Json.JsonReader input,
            string expected)
        {
            RequireTokenType(input, global::Newtonsoft.Json.JsonToken.PropertyName);
            if ((string)input.Value != expected)
                throw new global::System.IO.InvalidDataException(
                    string.Format(
                        global::System.Globalization.CultureInfo.InvariantCulture,
                        ""Expected property '{0}' but found property '{1}'"",
                        expected, input.Value));
            AdvanceToken(input);
        }

        static void IterateObject(global::Newtonsoft.Json.JsonReader input,
            global::System.Action iterate)
        {
            ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.StartObject);
            while (input.TokenType != global::Newtonsoft.Json.JsonToken.EndObject)
                iterate();
            input.Read();
        }

        static void IterateArray(global::Newtonsoft.Json.JsonReader input,
            global::System.Action iterate)
        {
            ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.StartArray);
            while (input.TokenType != global::Newtonsoft.Json.JsonToken.EndArray)
                iterate();
            input.Read();
        }

        static T ConsumePrimitiveToken<T>(global::Newtonsoft.Json.JsonReader input,
            global::Newtonsoft.Json.JsonToken expectedToken,
            global::System.Func<object, T> callback,
            bool isNullable)
        {

            T result;
            if (input.TokenType == expectedToken)
            {
                result = callback(input.Value);
            }
            else if (input.TokenType == global::Newtonsoft.Json.JsonToken.Null && isNullable)
            {
                result = default(T);
            }
            else
            {
                throw new global::System.IO.InvalidDataException(
                    string.Format(
                        global::System.Globalization.CultureInfo.InvariantCulture,
                        ""Expected {0} token but found {1} token"",
                        expectedToken,
                        input.TokenType));
            }
            AdvanceToken(input);
            return result;
        }

        static T ConsumeIntegerOrStringToken<T>(global::Newtonsoft.Json.JsonReader input,
            global::System.Func<long, T> numericCallback,
            global::System.Func<string, T> stringCallback)
        {
            T result;
            if (input.TokenType == global::Newtonsoft.Json.JsonToken.Integer)
            {
                result = numericCallback((long)input.Value);
            }
            else if (input.TokenType == global::Newtonsoft.Json.JsonToken.String)
            {
                result = stringCallback((string)input.Value);
            }
            else
            {
                throw new global::System.IO.InvalidDataException(
                    string.Format(
                        global::System.Globalization.CultureInfo.InvariantCulture,
                        ""Expected Integer/String token but found {0} token"",
                        input.TokenType));
            }
            AdvanceToken(input);
            return result;
        }

        static T ConsumeEnum<T>(global::Newtonsoft.Json.JsonReader input, bool isNullable)
                where T: struct
        {
            return ConsumePrimitiveToken<T>(input, global::Newtonsoft.Json.JsonToken.String,
                (o) => global::System.Enum.Parse<T>((string)o), isNullable);
        }


        static void SerializeDictionary<TKey, TValue>(
            global::System.Collections.Generic.IEnumerable<
                global::System.Collections.Generic.KeyValuePair<TKey, TValue>> dictionary,
            global::Newtonsoft.Json.JsonWriter output,
            global::System.Action<TKey, global::Newtonsoft.Json.JsonWriter> keySerializer,
            global::System.Action<TValue, global::Newtonsoft.Json.JsonWriter> valueSerializer)
        {
            output.WriteStartArray();
            foreach (var datum in dictionary)
            {
                output.WriteStartObject();
                output.WritePropertyName(""k"");
                keySerializer(datum.Key, output);
                output.WritePropertyName(""v"");
                valueSerializer(datum.Value, output);
                output.WriteEndObject();
            }
            output.WriteEndArray();
        }

        static global::System.Collections.Generic.SortedDictionary<TKey, TValue>
                DeserializeDictionary<TKey, TValue>(
            global::Newtonsoft.Json.JsonReader input,
            global::System.Func<global::Newtonsoft.Json.JsonReader, bool, TKey>
                keyDeserializer,
            global::System.Func<global::Newtonsoft.Json.JsonReader, bool, TValue>
                valueDeserializer,
            bool keyIsNullable, bool valueIsNullable)
        {
            var result = new global::System.Collections.Generic.SortedDictionary<TKey, TValue>();
            IterateArray(input, () =>
            {
                ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.StartObject);
                RequirePropertyName(input, ""k"");
                var key = keyDeserializer(input, keyIsNullable);
                RequirePropertyName(input, ""v"");
                var value = valueDeserializer(input, valueIsNullable);
                ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.EndObject);

                result.Add(key, value);
            });
            return result;
        }

        static void SerializeJsonObject<TKey, TValue>(
            global::System.Collections.Generic.IEnumerable<
                global::System.Collections.Generic.KeyValuePair<TKey, TValue>> dictionary,
            global::Newtonsoft.Json.JsonWriter output,
            global::System.Func<TKey, string> keySerializer,
            global::System.Action<TValue, global::Newtonsoft.Json.JsonWriter> valueSerializer)
        {
            output.WriteStartObject();
            foreach (var datum in dictionary)
            {
                output.WritePropertyName(keySerializer(datum.Key));
                valueSerializer(datum.Value, output);
            }
            output.WriteEndObject();
        }

        static global::System.Collections.Generic.SortedDictionary<TKey, TValue>
                DeserializeJsonObject<TKey, TValue>(
            global::Newtonsoft.Json.JsonReader input,
            global::System.Func<string, bool, TKey> keyDeserializer,
            global::System.Func<global::Newtonsoft.Json.JsonReader, bool, TValue>
                valueDeserializer,
            bool keyIsNullable, bool valueIsNullable)
        {
            var result
                = new global::System.Collections.Generic.SortedDictionary<TKey, TValue>();
            IterateObject(input, () =>
            {
                var key = GetPropertyName(input);
                result.Add(keyDeserializer(key, keyIsNullable),
                        valueDeserializer(input, valueIsNullable));
            });
            return result;
        }

        static void SerializeList<T>(
            global::System.Collections.Generic.IEnumerable<T> array,
            global::Newtonsoft.Json.JsonWriter output,
            global::System.Action<T, global::Newtonsoft.Json.JsonWriter> elementSerializer)
        {
            output.WriteStartArray();
            foreach (var element in array)
                elementSerializer(element, output);
            output.WriteEndArray();
        }

        static global::System.Collections.Generic.IList<T> DeserializeList<T>(
            global::Newtonsoft.Json.JsonReader input,
            global::System.Func<global::Newtonsoft.Json.JsonReader, bool, T> elementDeserializer,
            bool elementIsNullable)
        {
            var result = new global::System.Collections.Generic.List<T>();
            IterateArray(input, () =>
            {
                result.Add(elementDeserializer(input, elementIsNullable));
            });
            return result;
        }


        static void Serialize(bool value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static bool DeserializeBool(global::Newtonsoft.Json.JsonReader input, bool isNullable)
        {
            return ConsumePrimitiveToken(input, Newtonsoft.Json.JsonToken.Boolean,
                o => (bool)o, isNullable);
        }

        static void Serialize(string value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static string DeserializeString(global::Newtonsoft.Json.JsonReader input, bool isNullable)
        {
            return ConsumePrimitiveToken(input, Newtonsoft.Json.JsonToken.String,
                o => (string)o, isNullable);
        }

        static void Serialize(long value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static long DeserializeInt64(global::Newtonsoft.Json.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                global::System.Convert.ToInt64,
                i => long.Parse(i, global::System.Globalization.CultureInfo.InvariantCulture));
        }

        static void Serialize(int value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static int DeserializeInt32(global::Newtonsoft.Json.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                global::System.Convert.ToInt32,
                i => int.Parse(i, global::System.Globalization.CultureInfo.InvariantCulture));
        }

        static void Serialize(short value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static short DeserializeInt16(global::Newtonsoft.Json.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                global::System.Convert.ToInt16,
                i => short.Parse(i, global::System.Globalization.CultureInfo.InvariantCulture));
        }

        static void Serialize(sbyte value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static sbyte DeserializeSByte(global::Newtonsoft.Json.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                global::System.Convert.ToSByte,
                i => sbyte.Parse(i, global::System.Globalization.CultureInfo.InvariantCulture));
        }

        static void Serialize(ulong value, global::Newtonsoft.Json.JsonWriter writer)
        {
            // Special case: Newtonsoft.JSON cannot read back a large ulong as
            // it is internally storing it as a long, so serialize big large ulongs
            // as strings
            if (value >= uint.MaxValue)
                writer.WriteValue(value.ToString(
                    global::System.Globalization.CultureInfo.InvariantCulture));
            else
                writer.WriteValue(value);
        }

        static ulong DeserializeUInt64(global::Newtonsoft.Json.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                global::System.Convert.ToUInt64,
                i => ulong.Parse(i, global::System.Globalization.CultureInfo.InvariantCulture));
        }

        static void Serialize(uint value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static uint DeserializeUInt32(global::Newtonsoft.Json.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                global::System.Convert.ToUInt32,
                i => uint.Parse(i, global::System.Globalization.CultureInfo.InvariantCulture));
        }

        static void Serialize(ushort value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static ushort DeserializeUInt16(global::Newtonsoft.Json.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                global::System.Convert.ToUInt16,
                i => ushort.Parse(i, global::System.Globalization.CultureInfo.InvariantCulture));
        }

        static void Serialize(byte value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static byte DeserializeByte(global::Newtonsoft.Json.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                global::System.Convert.ToByte,
                i => byte.Parse(i, global::System.Globalization.CultureInfo.InvariantCulture));
        }
");
        }

        static void WriteEnumJsonSerializationDeclaration(PocoEnumDefinition enume,
            TextWriter output)
        {
            output.WriteLine(@"
        static {0} Deserialize{0}(global::Newtonsoft.Json.JsonReader input, bool isNullable)
        {{
            return ConsumeEnum<{0}>(input, isNullable);
        }}

        static void Serialize({0} value, global::Newtonsoft.Json.JsonWriter writer)
        {{
            writer.WriteValue(value.ToString());
        }}", enume.Name);
        }

        static void WriteClassJsonSerializationDeclaration(PocoClass clasz,
            TextWriter output)
        {
            output.WriteLine("\t\tpublic static void Serialize(this {0} @this, "
                    + "global::Newtonsoft.Json.JsonWriter output)", clasz.Name);
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\tif (@this == null)");
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\toutput.WriteNull();");
            output.WriteLine("\t\t\t\treturn;");
            output.WriteLine("\t\t\t}");
            output.WriteLine();

            output.WriteLine("\t\t\toutput.WriteStartObject();");
            foreach (var member in clasz.Members)
            {
                output.WriteLine("\t\t\toutput.WritePropertyName(\"{0}\");",
                    member.PublicMemberName());
                output.WriteLine("\t\t\t{0}", member.JsonSerializer());
            }
            output.WriteLine("\t\t\toutput.WriteEndObject();");
            output.WriteLine("\t\t}");

            output.WriteLine();

            output.WriteLine("\t\tpublic static {0} Deserialize{0}("
                    + "global::Newtonsoft.Json.JsonReader input, bool isNullable = false)",
                    clasz.Name);
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\tWarmReader(input);");
            output.WriteLine("\t\t\tif (input.TokenType == "
                    + "global::Newtonsoft.Json.JsonToken.Null)");
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tif (isNullable)");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\tinput.Read();");
            output.WriteLine("\t\t\t\t\treturn null;");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t\telse");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\tthrow new global::System.IO.InvalidDataException(");
            output.WriteLine("\t\t\t\t\t\t\"Found null when expecting a non-null object\");");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t}");
            output.WriteLine();

            foreach (var member in clasz.Members)
            {
                output.WriteLine("\t\t\t{0} = default;", member.FormalParameter());
            }
            output.WriteLine();
            output.WriteLine("\t\t\tIterateObject(input, () =>");
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tvar propertyName = GetPropertyName(input);");
            output.WriteLine("\t\t\t\tswitch (propertyName)");
            output.WriteLine("\t\t\t\t{");
            foreach (var member in clasz.Members)
            {
                output.WriteLine("\t\t\t\t\tcase \"{0}\":", member.PublicMemberName());
                output.WriteLine("\t\t\t\t\t\t{0}", member.JsonDeserializer());
                output.WriteLine("\t\t\t\t\t\tbreak;");
            }
            output.WriteLine("\t\t\t\t\tdefault:");
            output.WriteLine("\t\t\t\t\t\tthrow new global::System.IO.InvalidDataException("
                    + "\"Unexpected property \" + propertyName);");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t});");

            output.WriteLine("\t\t\treturn new {0}({1});", clasz.Name,
                string.Join(", ", clasz.Members.Select(member => member.TempVarName())));

            output.WriteLine("\t\t}");
        }
    }
}

namespace Pocotheosis.MemberTypes
{
    partial class PrimitiveType
    {
        public string GetJsonDeserializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = {1}(input, {2});",
                TempVarName(variableName), JsonDeserializerMethod,
                IsNullable.ToToken());
        }

        public string GetJsonSerializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Serialize(@this.{0}, output);",
                PublicMemberName(variableName));
        }
    }

    partial class ArrayType
    {
        public string GetJsonDeserializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} = DeserializeList(input, {1}, {2});",
                TempVarName(variableName), elementType.JsonDeserializerMethod,
                elementType.IsNullable.ToToken());
        }

        public string GetJsonSerializer(string variableName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "SerializeList(@this.{0}, output, {1});",
                PublicMemberName(variableName),
                "Serialize");
        }
    }
    partial class DictionaryType
    {
        public string GetJsonDeserializer(string variableName)
        {
            if (keyType.TypeName == "string")
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} = DeserializeJsonObject(input, (key, isNullable) => "
                        + "key, {1}, {2}, {3});",
                    TempVarName(variableName),
                    valueType.JsonDeserializerMethod,
                    keyType.IsNullable.ToToken(),
                    valueType.IsNullable.ToToken());
            }
            else if (keyType.IsEnum)
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} = DeserializeJsonObject(input, (key, isNullable) => "
                        + "global::System.Enum.Parse<{1}>(key), {2}, {3}, {4});",
                    TempVarName(variableName),
                    keyType.TypeName,
                    valueType.JsonDeserializerMethod,
                    keyType.IsNullable.ToToken(),
                    valueType.IsNullable.ToToken());
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} = DeserializeDictionary(input, {1}, {2}, {3}, {4});",
                    TempVarName(variableName), keyType.JsonDeserializerMethod,
                    valueType.JsonDeserializerMethod,
                    keyType.IsNullable.ToToken(),
                    valueType.IsNullable.ToToken());
            }
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
    }
}