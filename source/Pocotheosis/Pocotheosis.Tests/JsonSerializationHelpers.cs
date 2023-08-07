using Newtonsoft.Json;
using Pocotheosis.Tests.Pocos;
using System;

namespace Pocotheosis.Tests
{
    public static class JsonSerializationHelpers
    {
        public static bool DeserializeBool(JsonReader input)
        {
            if (!input.Read())
                throw new Exception("Unexpected end of stream");
            if (input.TokenType != JsonToken.Boolean)
                throw new Exception("Expected a boolean");
            return (bool)input.Value;
        }

        public static string DeserializeString(JsonReader input)
        {
            if (!input.Read())
                throw new Exception("Unexpected end of stream");
            if (input.TokenType != JsonToken.String)
                throw new Exception("Expected a string");
            return (string)input.Value;
        }

        public static long DeserializeInt64(JsonReader input)
        {
            if (!input.Read())
                throw new Exception("Unexpected end of stream");
            if (input.TokenType != JsonToken.Integer)
                throw new Exception("Expected an integer");
            return (long)input.Value;
        }

        public static int DeserializeInt32(JsonReader input)
        {
            throw new NotImplementedException();
        }

        public static short DeserializeInt16(JsonReader input)
        {
            throw new NotImplementedException();
        }

        public static sbyte DeserializeSByte(JsonReader input)
        {
            throw new NotImplementedException();
        }

        public static ulong DeserializeUInt64(JsonReader input)
        {
            throw new NotImplementedException();
        }

        public static uint DeserializeUInt32(JsonReader input)
        {
            throw new NotImplementedException();
        }

        public static ushort DeserializeUInt16(JsonReader input)
        {
            throw new NotImplementedException();
        }

        public static byte DeserializeByte(JsonReader input)
        {
            if (!input.Read())
                throw new Exception("Unexpected end of stream");
            if (input.TokenType != JsonToken.Integer)
                throw new Exception("Expected an integer");
            return Convert.ToByte((long)input.Value);
        }


        public static void Serialize(bool value, JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(string value, JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(long value, JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(int value, JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(short value, JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(sbyte value, JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(ulong value, JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(uint value, JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(ushort value, JsonWriter writer)
        {
            writer.WriteValue(value);
        }

        public static void Serialize(byte value, JsonWriter writer)
        {
            writer.WriteValue(value);
        }





        public static TestEnum DeserializeTestEnum(JsonReader input)
        {
            if (!input.Read())
                throw new Exception("Unexpected end of stream");
            if (input.TokenType != JsonToken.String)
                throw new Exception("Expected a string");
            return Enum.Parse<TestEnum>((string)input.Value);
        }

        public static ScoreTuple DeserializeScoreTuple(JsonReader input)
        {
            return ScoreTuple.Deserialize(input);
        }

        public static Point DeserializePoint(JsonReader input)
        {
            return Point.Deserialize(input);
        }

        public static void Serialize(TestEnum value, JsonWriter writer)
        {
            writer.WriteValue(value.ToString());
        }

        public static void Serialize(ScoreTuple value, JsonWriter writer)
        {
            value.Serialize(writer);
        }

        public static void Serialize(Point value, JsonWriter writer)
        {
            value.Serialize(writer);
        }





        public static void SerializeList<T>(
            global::System.Collections.Generic.IList<T> array,
            JsonWriter output,
            global::System.Action<T, JsonWriter> elementSerializer)
        {
            throw new NotImplementedException();
        }

        public static global::System.Collections.Generic.IList<T> DeserializeList<T>(
            JsonReader input,
            global::System.Func<JsonReader, T> elementDeserializer)
        {
            throw new NotImplementedException();
        }





        public static void SerializeDictionary<TKey, TValue>(
            global::System.Collections.Generic.SortedDictionary<TKey, TValue> dictionary,
            JsonWriter output,
            global::System.Action<TKey, JsonWriter> keySerializer,
            global::System.Action<TValue, JsonWriter> valueSerializer)
        {
            throw new NotImplementedException();
        }

        public static global::System.Collections.Generic.SortedDictionary<TKey, TValue>
                DeserializeDictionary<TKey, TValue>(
            JsonReader input,
            global::System.Func<JsonReader, TKey> keyDeserializer,
            global::System.Func<JsonReader, TValue> valueDeserializer)
        {
            throw new NotImplementedException();
        }
    }
}
