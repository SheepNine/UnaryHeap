namespace Pocotheosis.Tests.Pocos
{
    public static class JsonSerializationHelpers
    {
        static void RequireTokenType(global::Newtonsoft.Json.JsonReader input,
            global::Newtonsoft.Json.JsonToken expected)
        {
            if (input.TokenType != expected)
                throw new global::System.IO.InvalidDataException(
                    string.Format("Expected {0} token but found {1} token",
                        expected, input.TokenType));
        }

        static void AdvanceToken(global::Newtonsoft.Json.JsonReader input)
        {
            if (!input.Read())
                throw new global::System.IO.InvalidDataException(
                    "Unexpected end of stream");
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
                    string.Format("Expected property '{0}' but found property '{1}'",
                        expected, input.Value));
            AdvanceToken(input);
        }

        public static bool DeserializeBool(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Boolean);
            return (bool)input.Value;
        }

        public static string DeserializeString(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.String);
            return (string)input.Value;
        }

        public static long DeserializeInt64(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Integer);
            return (long)input.Value;
        }

        public static int DeserializeInt32(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Integer);
            return global::System.Convert.ToInt32(input.Value);
        }

        public static short DeserializeInt16(global::Newtonsoft.Json.JsonReader input)
        {
            throw new global::System.NotImplementedException();
        }

        public static sbyte DeserializeSByte(global::Newtonsoft.Json.JsonReader input)
        {
            throw new global::System.NotImplementedException();
        }

        public static ulong DeserializeUInt64(global::Newtonsoft.Json.JsonReader input)
        {
            throw new global::System.NotImplementedException();
        }

        public static uint DeserializeUInt32(global::Newtonsoft.Json.JsonReader input)
        {
            throw new global::System.NotImplementedException();
        }

        public static ushort DeserializeUInt16(global::Newtonsoft.Json.JsonReader input)
        {
            throw new global::System.NotImplementedException();
        }

        public static byte DeserializeByte(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Integer);
            return global::System.Convert.ToByte((long)input.Value);
        }


        public static void Serialize(bool value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(string value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(long value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(int value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(short value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(sbyte value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(ulong value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(uint value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(ushort value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(byte value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value);
        }





        public static TestEnum DeserializeTestEnum(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.String);
            return global::System.Enum.Parse<TestEnum>((string)input.Value);
        }

        public static void Serialize(TestEnum value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value.ToString());
        }

        public static void Serialize(ScoreTuple value, global::Newtonsoft.Json.JsonWriter writer)
        {
            value.Serialize(writer);
        }

        public static void Serialize(Point value, global::Newtonsoft.Json.JsonWriter writer)
        {
            value.Serialize(writer);
        }
        
        public static void Serialize(BoolPoco value, global::Newtonsoft.Json.JsonWriter writer)
        {
            value.Serialize(writer);
        }





        public static void SerializeList<T>(
            global::System.Collections.Generic.IList<T> array,
            global::Newtonsoft.Json.JsonWriter output,
            global::System.Action<T, global::Newtonsoft.Json.JsonWriter> elementSerializer)
        {
            output.WriteStartArray();
            foreach (var element in array)
                elementSerializer(element, output);
            output.WriteEndArray();
        }

        public static global::System.Collections.Generic.IList<T> DeserializeList<T>(
            global::Newtonsoft.Json.JsonReader input,
            global::System.Func<global::Newtonsoft.Json.JsonReader, T> elementDeserializer)
        {
            var result = new global::System.Collections.Generic.List<T>();

            ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.StartArray);

            while (input.TokenType != global::Newtonsoft.Json.JsonToken.EndArray)
            {
                result.Add(elementDeserializer(input));
                AdvanceToken(input);
            }

            AdvanceToken(input);

            return result;
        }



        public static void SerializeJsonObject<TValue>(
            global::System.Collections.Generic.SortedDictionary<string, TValue> dictionary,
            global::Newtonsoft.Json.JsonWriter output,
            global::System.Action<TValue, global::Newtonsoft.Json.JsonWriter> valueSerializer)
        {
            output.WriteStartObject();

            foreach (var datum in dictionary)
            {
                output.WritePropertyName(datum.Key);
                valueSerializer(datum.Value, output);
            }

            output.WriteEndObject();
        }

        public static global::System.Collections.Generic.SortedDictionary<string, TValue>
                DeserializeJsonObject<TValue>(
            global::Newtonsoft.Json.JsonReader input,
            global::System.Func<global::Newtonsoft.Json.JsonReader, TValue> valueDeserializer)
        {
            var result
                = new global::System.Collections.Generic.SortedDictionary<string, TValue>();

            ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.StartObject);

            while (input.TokenType != global::Newtonsoft.Json.JsonToken.EndObject)
            {
                var key = GetPropertyName(input);
                result.Add(key, valueDeserializer(input));
                AdvanceToken(input);
            }

            AdvanceToken(input);

            return result;
        }


        public static void SerializeDictionary<TKey, TValue>(
            global::System.Collections.Generic.SortedDictionary<TKey, TValue> dictionary,
            global::Newtonsoft.Json.JsonWriter output,
            global::System.Action<TKey, global::Newtonsoft.Json.JsonWriter> keySerializer,
            global::System.Action<TValue, global::Newtonsoft.Json.JsonWriter> valueSerializer)
        {
            output.WriteStartArray();

            foreach (var datum in dictionary)
            {
                output.WriteStartObject();
                output.WritePropertyName("k");
                keySerializer(datum.Key, output);
                output.WritePropertyName("v");
                valueSerializer(datum.Value, output);
                output.WriteEndObject();
            }

            output.WriteEndArray();
        }

        public static global::System.Collections.Generic.SortedDictionary<TKey, TValue>
                DeserializeDictionary<TKey, TValue>(
            global::Newtonsoft.Json.JsonReader input,
            global::System.Func<global::Newtonsoft.Json.JsonReader, TKey> keyDeserializer,
            global::System.Func<global::Newtonsoft.Json.JsonReader, TValue> valueDeserializer)
        {
            var result = new global::System.Collections.Generic.SortedDictionary<TKey, TValue>();

            ConsumeTokenType(input, Newtonsoft.Json.JsonToken.StartArray);

            while (input.TokenType != global::Newtonsoft.Json.JsonToken.EndArray)
            {
                ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.StartObject);

                RequirePropertyName(input, "k");
                var key = keyDeserializer(input);
                AdvanceToken(input);

                RequirePropertyName(input, "v");
                var value = valueDeserializer(input);
                AdvanceToken(input);

                result.Add(key, value);

                ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.EndObject);
            }

            AdvanceToken(input);

            return result;
        }
    }
}
