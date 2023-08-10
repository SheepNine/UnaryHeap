using System.Collections.Generic;

namespace Pocotheosis.Tests.Pocos
{
    public static partial class JsonSerializationHelpers
    {
        public static void WarmReader(global::Newtonsoft.Json.JsonReader input)
        {
            if (input.TokenType == global::Newtonsoft.Json.JsonToken.None)
                AdvanceToken(input);
        }


        static void RequireTokenType(global::Newtonsoft.Json.JsonReader input,
            global::Newtonsoft.Json.JsonToken expected)
        {
            if (input.TokenType != expected)
                throw new global::System.IO.InvalidDataException(
                    string.Format("Expected {0} token but found {1} token",
                        expected, input.TokenType));
        }

        public static void AdvanceToken(global::Newtonsoft.Json.JsonReader input)
        {
            if (!input.Read())
                throw new global::System.IO.InvalidDataException(
                    "Unexpected end of stream");
        }

        public static void ConsumeTokenType(global::Newtonsoft.Json.JsonReader input,
            global::Newtonsoft.Json.JsonToken expected)
        {
            RequireTokenType(input, expected);
            AdvanceToken(input);
        }

        public static string GetPropertyName(global::Newtonsoft.Json.JsonReader input)
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

        public static void IterateObject(global::Newtonsoft.Json.JsonReader input,
            global::System.Action iterate)
        {
            ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.StartObject);
            while (input.TokenType != global::Newtonsoft.Json.JsonToken.EndObject)
                iterate();
            input.Read();
        }

        public static void IterateArray(global::Newtonsoft.Json.JsonReader input,
            global::System.Action iterate)
        {
            ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.StartArray);
            while (input.TokenType != global::Newtonsoft.Json.JsonToken.EndArray)
                iterate();
            input.Read();
        }



        public static bool DeserializeBool(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Boolean);
            var result = (bool)input.Value;
            AdvanceToken(input);
            return result;
        }

        public static string DeserializeString(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.String);
            var result = (string)input.Value;
            AdvanceToken(input);
            return result;
        }

        public static long DeserializeInt64(global::Newtonsoft.Json.JsonReader input)
        {
            if (input.TokenType == global::Newtonsoft.Json.JsonToken.Integer)
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Integer);
            var result = (long)input.Value;
            AdvanceToken(input);
            return result;
        }

        public static int DeserializeInt32(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Integer);
            var result = global::System.Convert.ToInt32(input.Value);
            AdvanceToken(input);
            return result;
        }

        public static short DeserializeInt16(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Integer);
            var result = global::System.Convert.ToInt16(input.Value);
            AdvanceToken(input);
            return result;
        }

        public static sbyte DeserializeSByte(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Integer);
            var result = global::System.Convert.ToSByte(input.Value);
            AdvanceToken(input);
            return result;
        }

        public static ulong DeserializeUInt64(global::Newtonsoft.Json.JsonReader input)
        {
            ulong result;
            if (input.TokenType == global::Newtonsoft.Json.JsonToken.Integer)
            {
                result = global::System.Convert.ToUInt64(input.Value);
            }
            else if (input.TokenType == global::Newtonsoft.Json.JsonToken.String)
            {
                result = ulong.Parse((string)input.Value);
            }
            else
            {
                throw new global::System.IO.InvalidDataException(
                    string.Format("Expected Integer/String token but found {0} token",
                        input.TokenType));
            }
            AdvanceToken(input);
            return result;
        }

        public static uint DeserializeUInt32(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Integer);
            var result = global::System.Convert.ToUInt32(input.Value);
            AdvanceToken(input);
            return result;
        }

        public static ushort DeserializeUInt16(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Integer);
            var result = global::System.Convert.ToUInt16(input.Value);
            AdvanceToken(input);
            return result;
        }

        public static byte DeserializeByte(global::Newtonsoft.Json.JsonReader input)
        {
            RequireTokenType(input, Newtonsoft.Json.JsonToken.Integer);
            var result = global::System.Convert.ToByte((long)input.Value);
            AdvanceToken(input);
            return result;
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
            if (value >= uint.MaxValue)
                writer.WriteValue(value.ToString(
                    global::System.Globalization.CultureInfo.InvariantCulture));
            else
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
            var result = global::System.Enum.Parse<TestEnum>((string)input.Value);
            AdvanceToken(input);
            return result;
        }

        public static void Serialize(TestEnum value, global::Newtonsoft.Json.JsonWriter writer)
        {
            writer.WriteValue(value.ToString());
        }





        public static void SerializeList<T>(
            global::System.Collections.Generic.IEnumerable<T> array,
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

            IterateArray(input, () =>
            {
                result.Add(elementDeserializer(input));
            });

            return result;
        }



        public static void SerializeJsonObject<TValue>(
            global::System.Collections.Generic.IEnumerable<KeyValuePair<string, TValue>> dictionary,
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

            IterateObject(input, () =>
            {
                var key = GetPropertyName(input);
                result.Add(key, valueDeserializer(input));
            });

            return result;
        }


        public static void SerializeDictionary<TKey, TValue>(
            global::System.Collections.Generic.IEnumerable<KeyValuePair<TKey, TValue>> dictionary,
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

            IterateArray(input, () =>
            {
                ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.StartObject);
                RequirePropertyName(input, "k");
                var key = keyDeserializer(input);
                RequirePropertyName(input, "v");
                var value = valueDeserializer(input);
                ConsumeTokenType(input, global::Newtonsoft.Json.JsonToken.EndObject);

                result.Add(key, value);
            });

            return result;
        }
    }
}
