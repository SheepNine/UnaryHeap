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
                // TODO: type alias for _nsJ_
                WriteNamespaceHeader(dataModel, output);
                output.EmitCode(
$"    using _nsJ_ = global::Newtonsoft.Json;",
$"",
$"    public static partial class PocoJson",
$"    {{"
                );
                WriteBuiltInHelperMethods(output);
                foreach (var enume in dataModel.Enums)
                    WriteEnumJsonSerializationDeclaration(enume, output);
                foreach (var clasz in dataModel.Classes)
                    WriteClassJsonSerializationDeclaration(clasz, output);
                output.EmitCode(
$"    }}"
                );
                WriteNamespaceFooter(output);
            }
        }

        static void WriteBuiltInHelperMethods(StreamWriter output)
        {
            output.WriteLine(@"
        static void WarmReader(_nsJ_.JsonReader input)
        {
            if (input.TokenType == _nsJ_.JsonToken.None)
                AdvanceToken(input);
        }

        static void RequireTokenType(_nsJ_.JsonReader input, _nsJ_.JsonToken expected)
        {
            if (input.TokenType != expected)
                throw new _nsI_.InvalidDataException(
                    $""Expected {expected} token but found {input.TokenType} token"");
        }

        static void AdvanceToken(_nsJ_.JsonReader input)
        {
            if (!input.Read())
                throw new _nsI_.InvalidDataException(""Unexpected end of stream"");
        }

        static void ConsumeTokenType(_nsJ_.JsonReader input, _nsJ_.JsonToken expected)
        {
            RequireTokenType(input, expected);
            AdvanceToken(input);
        }

        static string GetPropertyName(_nsJ_.JsonReader input)
        {
            RequireTokenType(input, _nsJ_.JsonToken.PropertyName);
            var result = (string)input.Value;
            AdvanceToken(input);
            return result;
        }

        static void RequirePropertyName(_nsJ_.JsonReader input,
            string expected)
        {
            RequireTokenType(input, _nsJ_.JsonToken.PropertyName);
            if ((string)input.Value != expected)
                throw new _nsI_.InvalidDataException(
                    $""Expected property '{expected}' but found property '{input.Value}'"");
            AdvanceToken(input);
        }

        static void IterateObject(_nsJ_.JsonReader input, _nsS_.Action iterate)
        {
            ConsumeTokenType(input, _nsJ_.JsonToken.StartObject);
            while (input.TokenType != _nsJ_.JsonToken.EndObject)
                iterate();
            input.Read();
        }

        static void IterateArray(_nsJ_.JsonReader input, _nsS_.Action iterate)
        {
            ConsumeTokenType(input, _nsJ_.JsonToken.StartArray);
            while (input.TokenType != _nsJ_.JsonToken.EndArray)
                iterate();
            input.Read();
        }

        static T ConsumePrimitiveToken<T>(_nsJ_.JsonReader input,
            _nsJ_.JsonToken expectedToken, _nsS_.Func<object, T> callback,
            bool isNullable)
        {
            T result;
            if (input.TokenType == expectedToken)
            {
                result = callback(input.Value);
            }
            else if (input.TokenType == _nsJ_.JsonToken.Null && isNullable)
            {
                result = default(T);
            }
            else
            {
                throw new _nsI_.InvalidDataException(
                    $""Expected {expectedToken} token but found {input.TokenType} token"");
            }
            AdvanceToken(input);
            return result;
        }

        static T ConsumeIntegerOrStringToken<T>(_nsJ_.JsonReader input,
            _nsS_.Func<long, T> numericCallback, _nsS_.Func<string, T> stringCallback)
        {
            T result;
            if (input.TokenType == _nsJ_.JsonToken.Integer)
            {
                result = numericCallback((long)input.Value);
            }
            else if (input.TokenType == _nsJ_.JsonToken.String)
            {
                result = stringCallback((string)input.Value);
            }
            else
            {
                throw new _nsI_.InvalidDataException(
                    $""Expected Integer/String token but found {input.TokenType} token"");
            }
            AdvanceToken(input);
            return result;
        }

        static T ConsumeEnum<T>(_nsJ_.JsonReader input, bool isNullable)
                where T: struct
        {
            return ConsumePrimitiveToken<T>(input, _nsJ_.JsonToken.String,
                (o) => _nsS_.Enum.Parse<T>((string)o), isNullable);
        }


        static void SerializeDictionary<TKey, TValue>(
            _nsG_.IEnumerable<_nsG_.KeyValuePair<TKey, TValue>> dictionary,
            _nsJ_.JsonWriter output,
            _nsS_.Action<TKey, _nsJ_.JsonWriter> keySerializer,
            _nsS_.Action<TValue, _nsJ_.JsonWriter> valueSerializer)
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

        static _nsG_.SortedDictionary<TKey, TValue> DeserializeDictionary<TKey, TValue>(
            _nsJ_.JsonReader input,
            _nsS_.Func<_nsJ_.JsonReader, bool, TKey> keyDeserializer,
            _nsS_.Func<_nsJ_.JsonReader, bool, TValue> valueDeserializer,
            bool keyIsNullable, bool valueIsNullable)
        {
            var result = new _nsG_.SortedDictionary<TKey, TValue>();
            IterateArray(input, () =>
            {
                ConsumeTokenType(input, _nsJ_.JsonToken.StartObject);
                RequirePropertyName(input, ""k"");
                var key = keyDeserializer(input, keyIsNullable);
                RequirePropertyName(input, ""v"");
                var value = valueDeserializer(input, valueIsNullable);
                ConsumeTokenType(input, _nsJ_.JsonToken.EndObject);

                result.Add(key, value);
            });
            return result;
        }

        static void SerializeJsonObject<TKey, TValue>(
            _nsG_.IEnumerable<_nsG_.KeyValuePair<TKey, TValue>> dictionary,
            _nsJ_.JsonWriter output,
            _nsS_.Func<TKey, string> keySerializer,
            _nsS_.Action<TValue, _nsJ_.JsonWriter> valueSerializer)
        {
            output.WriteStartObject();
            foreach (var datum in dictionary)
            {
                output.WritePropertyName(keySerializer(datum.Key));
                valueSerializer(datum.Value, output);
            }
            output.WriteEndObject();
        }

        static _nsG_.SortedDictionary<TKey, TValue> DeserializeJsonObject<TKey, TValue>(
            _nsJ_.JsonReader input,
            _nsS_.Func<string, bool, TKey> keyDeserializer,
            _nsS_.Func<_nsJ_.JsonReader, bool, TValue> valueDeserializer,
            bool keyIsNullable, bool valueIsNullable)
        {
            var result
                = new _nsG_.SortedDictionary<TKey, TValue>();
            IterateObject(input, () =>
            {
                var key = GetPropertyName(input);
                result.Add(keyDeserializer(key, keyIsNullable),
                        valueDeserializer(input, valueIsNullable));
            });
            return result;
        }

        static void SerializeList<T>(
            _nsG_.IEnumerable<T> array,
            _nsJ_.JsonWriter output,
            _nsS_.Action<T, _nsJ_.JsonWriter> elementSerializer)
        {
            output.WriteStartArray();
            foreach (var element in array)
                elementSerializer(element, output);
            output.WriteEndArray();
        }

        static _nsG_.IList<T> DeserializeList<T>(
            _nsJ_.JsonReader input,
            _nsS_.Func<_nsJ_.JsonReader, bool, T> elementDeserializer,
            bool elementIsNullable)
        {
            var result = new _nsG_.List<T>();
            IterateArray(input, () =>
            {
                result.Add(elementDeserializer(input, elementIsNullable));
            });
            return result;
        }


        static void Serialize(bool value, _nsJ_.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static bool DeserializeBool(_nsJ_.JsonReader input, bool isNullable)
        {
            return ConsumePrimitiveToken(input, Newtonsoft.Json.JsonToken.Boolean,
                o => (bool)o, isNullable);
        }

        static void Serialize(string value, _nsJ_.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static string DeserializeString(_nsJ_.JsonReader input, bool isNullable)
        {
            return ConsumePrimitiveToken(input, Newtonsoft.Json.JsonToken.String,
                o => (string)o, isNullable);
        }

        static void Serialize(long value, _nsJ_.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static long DeserializeInt64(_nsJ_.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                _nsS_.Convert.ToInt64,
                i => long.Parse(i, _nsGl_.CultureInfo.InvariantCulture));
        }

        static void Serialize(int value, _nsJ_.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static int DeserializeInt32(_nsJ_.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                _nsS_.Convert.ToInt32,
                i => int.Parse(i, _nsGl_.CultureInfo.InvariantCulture));
        }

        static void Serialize(short value, _nsJ_.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static short DeserializeInt16(_nsJ_.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                _nsS_.Convert.ToInt16,
                i => short.Parse(i, _nsGl_.CultureInfo.InvariantCulture));
        }

        static void Serialize(sbyte value, _nsJ_.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static sbyte DeserializeSByte(_nsJ_.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                _nsS_.Convert.ToSByte,
                i => sbyte.Parse(i, _nsGl_.CultureInfo.InvariantCulture));
        }

        static void Serialize(ulong value, _nsJ_.JsonWriter writer)
        {
            // Special case: Newtonsoft.JSON cannot read back a large ulong as
            // it is internally storing it as a long, so serialize big large ulongs
            // as strings
            if (value >= uint.MaxValue)
                writer.WriteValue(value.ToString(
                    _nsGl_.CultureInfo.InvariantCulture));
            else
                writer.WriteValue(value);
        }

        static ulong DeserializeUInt64(_nsJ_.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                _nsS_.Convert.ToUInt64,
                i => ulong.Parse(i, _nsGl_.CultureInfo.InvariantCulture));
        }

        static void Serialize(uint value, _nsJ_.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static uint DeserializeUInt32(_nsJ_.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                _nsS_.Convert.ToUInt32,
                i => uint.Parse(i, _nsGl_.CultureInfo.InvariantCulture));
        }

        static void Serialize(ushort value, _nsJ_.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static ushort DeserializeUInt16(_nsJ_.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                _nsS_.Convert.ToUInt16,
                i => ushort.Parse(i, _nsGl_.CultureInfo.InvariantCulture));
        }

        static void Serialize(byte value, _nsJ_.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        static byte DeserializeByte(_nsJ_.JsonReader input, bool isNullable)
        {
            return ConsumeIntegerOrStringToken(input,
                _nsS_.Convert.ToByte,
                i => byte.Parse(i, _nsGl_.CultureInfo.InvariantCulture));
        }
");
        }

        static void WriteEnumJsonSerializationDeclaration(PocoEnumDefinition enume,
            TextWriter output)
        {
            output.EmitCode(
$"",
$"        static {enume.Name} Deserialize{enume.Name}(_nsJ_.JsonReader input, bool isNullable)",
$"        {{",
$"            return ConsumeEnum<{enume.Name}>(input, isNullable);",
$"        }}",
$"",
$"        static void Serialize({enume.Name} value, _nsJ_.JsonWriter writer)",
$"        {{",
$"            writer.WriteValue(value.ToString());",
$"        }}"
            );
        }

        static void WriteClassJsonSerializationDeclaration(PocoClass clasz,
            TextWriter output)
        {
            var parms = string.Join(", ",
                clasz.Members.Select(member => "_" + member.BackingStoreName));

            output.EmitCode(
$"",
$"        public static void Serialize(this {clasz.Name} @this, _nsJ_.JsonWriter output)",
$"        {{",
$"            if (@this == null)",
$"            {{",
$"                output.WriteNull();",
$"                return;",
$"            }}",
$"",
$"            output.WriteStartObject();"
            );
            foreach (var member in clasz.Members)
            {
                output.EmitCode(
$"            output.WritePropertyName(\"{member.PublicMemberName}\");",
$"            {member.JsonSerializer()}"
                );
            }
            output.EmitCode(
 $"            output.WriteEndObject();",
 $"        }}",
 $"",
 $"        public static {clasz.Name} Deserialize{clasz.Name}(",
 $"                _nsJ_.JsonReader input, bool isNullable = false)",
 $"        {{",
 $"            WarmReader(input);",
 $"            if (input.TokenType == _nsJ_.JsonToken.Null)",
 $"            {{",
 $"                if (isNullable)",
 $"                {{",
 $"                    input.Read();",
 $"                    return null;",
 $"                }}",
 $"                else",
 $"                {{",
 $"                    throw new _nsI_.InvalidDataException(",
 $"                        \"Found null when expecting a non-null object\");",
 $"                }}",
 $"            }}",
 $""
            );
            foreach (var member in clasz.Members) output.EmitCode(
$"            {member.FormalParameterType} _{member.BackingStoreName} = default;"
            );
            output.EmitCode(
$"",
$"            IterateObject(input, () =>",
$"            {{",
$"                var propertyName = GetPropertyName(input);",
$"                switch (propertyName)",
$"                {{"
            );
            foreach (var member in clasz.Members) output.EmitCode(
$"                    case \"{member.PublicMemberName}\":",
$"                        _{member.BackingStoreName} = {member.JsonDeserializer()};",
$"                        break;"
            );
            output.EmitCode(
$"                    default:",
$"                        throw new _nsI_.InvalidDataException(",
$"                            $\"Unexpected property {{propertyName}}\");",
$"                }}",
$"            }});",
$"            return new {clasz.Name}({parms});",
$"        }}"
            );
        }
    }
}

namespace Pocotheosis.MemberTypes
{
    partial class PrimitiveType
    {
        public string GetJsonDeserializer(string variableName)
        {
            return $"{JsonDeserializerMethod}(input, {IsNullable.ToToken()})";
        }

        public string GetJsonSerializer(string variableName)
        {
            return $"Serialize(@this.{variableName}, output);";
        }
    }

    partial class ArrayType
    {
        public string GetJsonDeserializer(string variableName)
        {
            return $"DeserializeList(input, {elementType.JsonDeserializerMethod}, "
                + $"{elementType.IsNullable.ToToken()})";
        }

        public string GetJsonSerializer(string variableName)
        {
            return $"SerializeList(@this.{variableName}, output, Serialize);";
        }
    }

    partial class DictionaryType
    {
        public string GetJsonDeserializer(string variableName)
        {
            if (keyType.TypeName == "string")
                return $"DeserializeJsonObject(input, (key, isNullable) => key, "
                    + $"{valueType.JsonDeserializerMethod}, {keyType.IsNullable.ToToken()}, "
                    + $"{valueType.IsNullable.ToToken()})";
            else if (keyType.IsEnum)
                return $"DeserializeJsonObject(input, (key, isNullable) => "
                    + $"_nsS_.Enum.Parse<{keyType.TypeName}>(key), "
                    + $"{valueType.JsonDeserializerMethod}, {keyType.IsNullable.ToToken()}, "
                    + $"{valueType.IsNullable.ToToken()})";
            else
                return $"DeserializeDictionary(input, {keyType.JsonDeserializerMethod}, "
                    + $"{valueType.JsonDeserializerMethod}, {keyType.IsNullable.ToToken()}, "
                    + $"{valueType.IsNullable.ToToken()})";
        }

        public string GetJsonSerializer(string variableName)
        {
            if (keyType.TypeName == "string")
                return $"SerializeJsonObject(@this.{variableName}, output, "
                    + "s => s, Serialize);";
            else if (keyType.IsEnum)
                return $"SerializeJsonObject(@this.{variableName}, output, "
                    + "e => e.ToString(), Serialize);";
            else
                return $"SerializeDictionary(@this.{variableName}, output, "
                    + "Serialize, Serialize);";
        }
    }
}