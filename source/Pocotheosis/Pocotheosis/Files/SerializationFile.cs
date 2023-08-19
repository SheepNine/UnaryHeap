using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteSerializationFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using (var file = File.CreateText(outputFileName))
            {
                WriteNamespaceHeader(dataModel, file);
                file.WriteLine("\tpublic abstract partial class Poco");
                file.WriteLine("\t{");
                file.WriteLine("\t\tpublic abstract void Serialize(" +
                    "global::System.IO.Stream output);");
                file.WriteLine();
                file.WriteLine(@"        public string Checksum
        {
            get
            {
                var buffer = new global::System.IO.MemoryStream();
                Serialize(buffer);
                buffer.Seek(0, global::System.IO.SeekOrigin.Begin);
                using (var sha256 = global::System.Security.Cryptography.SHA256.Create())
                {
                    var hash = sha256.ComputeHash(buffer);
                    var chars = global::System.Linq.Enumerable.Select(hash, b => b.ToString(
                        ""x2"", global::System.Globalization.CultureInfo.InvariantCulture));
                    return string.Join("""", chars);
                }
            }
        }
    }
");

                foreach (var pocoClass in dataModel.Classes)
                {
                    WriteSerializationImplementation(pocoClass, file);
                    file.WriteLine();
                }
                WriteSerializationHelperClass(file, dataModel);
                WriteNamespaceFooter(file);
            }
        }

        public static void WriteSerializationImplementation(PocoClass clasz, TextWriter output)
        {
            output.Write("\tpublic partial class ");
            output.WriteLine(clasz.Name);
            output.WriteLine("\t{");

            output.WriteLine("\t\tpublic override void Serialize(" +
                "global::System.IO.Stream output)");
            output.WriteLine("\t\t{");
            foreach (var member in clasz.Members)
            {
                output.Write("\t\t\t");
                output.WriteLine(member.Serializer());
            }
            output.WriteLine("\t\t}");
            output.WriteLine();
            output.Write("\t\tpublic static ");
            output.Write(clasz.Name);
            output.WriteLine(" Deserialize(global::System.IO.Stream input)");
            output.WriteLine("\t\t{");
            foreach (var member in clasz.Members)
            {
                output.Write("\t\t\t");
                output.WriteLine(member.Deserializer());
            }
            output.Write("\t\t\treturn new ");
            output.Write(clasz.Name);
            output.Write("(");
            output.Write(string.Join(", ", clasz.Members.Select(member => member.TempVarName())));
            output.WriteLine(");");
            output.WriteLine("\t\t}");

            output.WriteLine("\t}");
        }

        static void WriteSerializationHelperClass(TextWriter output,
            PocoNamespace dataModel)
        {
            output.WriteLine(@"    public static class SerializationHelpers
    {
        public static void Serialize(bool value, global::System.IO.Stream output)
        {
            output.WriteByte(value ? (byte)0xFF : (byte)0x00);
        }

        public static void Serialize(byte value, global::System.IO.Stream output)
        {
            output.WriteByte(value);
        }

        public static void Serialize(short value, global::System.IO.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
        }

        public static void Serialize(int value, global::System.IO.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
            output.WriteByte((byte)((value >> 16) & 0xFF));
            output.WriteByte((byte)((value >> 24) & 0xFF));
        }

        public static void Serialize(long value, global::System.IO.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
            output.WriteByte((byte)((value >> 16) & 0xFF));
            output.WriteByte((byte)((value >> 24) & 0xFF));
            output.WriteByte((byte)((value >> 32) & 0xFF));
            output.WriteByte((byte)((value >> 40) & 0xFF));
            output.WriteByte((byte)((value >> 48) & 0xFF));
            output.WriteByte((byte)((value >> 56) & 0xFF));
        }

        public static void Serialize(sbyte value, global::System.IO.Stream output)
        {
            output.WriteByte((byte)value);
        }

        public static void Serialize(ushort value, global::System.IO.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
        }

        public static void Serialize(uint value, global::System.IO.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
            output.WriteByte((byte)((value >> 16) & 0xFF));
            output.WriteByte((byte)((value >> 24) & 0xFF));
        }

        public static void Serialize(ulong value, global::System.IO.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
            output.WriteByte((byte)((value >> 16) & 0xFF));
            output.WriteByte((byte)((value >> 24) & 0xFF));
            output.WriteByte((byte)((value >> 32) & 0xFF));
            output.WriteByte((byte)((value >> 40) & 0xFF));
            output.WriteByte((byte)((value >> 48) & 0xFF));
            output.WriteByte((byte)((value >> 56) & 0xFF));
        }

        public static void Serialize(string value, global::System.IO.Stream output)
        {
            if (value == null) {
                Serialize(-1, output);
            }
            else {
                var bytes = global::System.Text.Encoding.UTF8.GetBytes(value);
                Serialize(bytes.Length, output);
                foreach (var b in bytes)
                    output.WriteByte(b);
            }
        }");

            foreach (var enume in dataModel.Enums)
            {
                output.WriteLine(
@"        public static void Serialize({0} value, global::System.IO.Stream output)
        {{
            Serialize((byte)value, output);
        }}", enume.Name);
            }

            foreach (var classe in dataModel.Classes)
            {
                output.WriteLine(
@"        public static void Serialize({0} value, global::System.IO.Stream output)
        {{
            value.Serialize(output);
        }}", classe.Name);
                output.WriteLine(
@"        public static void SerializeWithId({0} value, global::System.IO.Stream output)
        {{
            if (value == null)
                Serialize(-1, output);
            else
                value.SerializeWithId(output);
        }}", classe.Name);
            }

            output.WriteLine("        public static bool DeserializeBool("
                + @"global::System.IO.Stream input)
        {
            switch (DeserializeByte(input))
            {
                case 0x00:
                    return false;
                case 0xFF:
                    return true;
                default:
                    throw new global::System.IO.InvalidDataException(""Invalid boolean value"");
            }
        }

        public static byte DeserializeByte(global::System.IO.Stream input)
        {
            var result = input.ReadByte();
            if (result == -1)
                throw new global::System.IO.InvalidDataException(""Unexpected end-of-stream"");
            return (byte)result;
        }

        public static short DeserializeInt16(global::System.IO.Stream input)
        {
            var byte0 = DeserializeByte(input);
            var byte1 = DeserializeByte(input);
            return (short)(
                (byte1 << 8) |
                (byte0));
        }

        public static int DeserializeInt32(global::System.IO.Stream input)
        {
            var byte0 = DeserializeByte(input);
            var byte1 = DeserializeByte(input);
            var byte2 = DeserializeByte(input);
            var byte3 = DeserializeByte(input);
            return (int)(
                (byte3 << 24) |
                (byte2 << 16) |
                (byte1 << 8) |
                (byte0));
        }

        public static int? DeserializePocoIdentifier(global::System.IO.Stream input)
        {
            var byte0 = input.ReadByte();
            if (byte0 == -1) return null;
            var byte1 = DeserializeByte(input);
            var byte2 = DeserializeByte(input);
            var byte3 = DeserializeByte(input);
            return (int)(
                (byte3 << 24) |
                (byte2 << 16) |
                (byte1 << 8) |
                (byte0));
        }

        public static long DeserializeInt64(global::System.IO.Stream input)
        {
            var byte0 = DeserializeByte(input);
            var byte1 = DeserializeByte(input);
            var byte2 = DeserializeByte(input);
            var byte3 = DeserializeByte(input);
            var byte4 = DeserializeByte(input);
            var byte5 = DeserializeByte(input);
            var byte6 = DeserializeByte(input);
            var byte7 = DeserializeByte(input);
            return (long)(
                ((long)byte7 << 56) |
                ((long)byte6 << 48) |
                ((long)byte5 << 40) |
                ((long)byte4 << 32) |
                ((long)byte3 << 24) |
                ((long)byte2 << 16) |
                ((long)byte1 << 8) |
                ((long)byte0));
        }

        public static sbyte DeserializeSByte(global::System.IO.Stream input)
        {
            return (sbyte)DeserializeByte(input);
        }

        public static ushort DeserializeUInt16(global::System.IO.Stream input)
        {
            return (ushort)DeserializeInt16(input);
        }

        public static uint DeserializeUInt32(global::System.IO.Stream input)
        {
            return (uint)DeserializeInt32(input);
        }

        public static ulong DeserializeUInt64(global::System.IO.Stream input)
        {
            return (ulong)DeserializeInt64(input);
        }");

            foreach (var enume in dataModel.Enums)
            {
                output.WriteLine(
@"        public static {0} Deserialize{0}(global::System.IO.Stream input)
        {{
            return ({0})DeserializeByte(input);
        }}", enume.Name);
            }

            output.WriteLine(@"        public static string DeserializeString("
                + @"global::System.IO.Stream input)
        {
            var length = DeserializeInt32(input);
            if (length == -1)
                return null;
            var bytes = new byte[length];
            foreach (var i in global::System.Linq.Enumerable.Range(0, bytes.Length))
                bytes[i] = DeserializeByte(input);
            return global::System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static void SerializeList<T>(
            global::System.Collections.Generic.IList<T> array,
            global::System.IO.Stream output,
            global::System.Action<T, global::System.IO.Stream> elementSerializer)
        {
            SerializationHelpers.Serialize(array.Count, output);
            for (var i = 0; i < array.Count; i++)
                elementSerializer(array[i], output);
        }

        public static global::System.Collections.Generic.IList<T> DeserializeList<T>(
            global::System.IO.Stream input,
            global::System.Func<global::System.IO.Stream, T> elementDeserializer)
        {
            var size = SerializationHelpers.DeserializeInt32(input);
            var result = new T[size];
            for (var i = 0; i < size; i++)
                result[i] = elementDeserializer(input);
            return result;
        }

        public static void SerializeDictionary<TKey, TValue>(
            global::System.Collections.Generic.SortedDictionary<TKey, TValue> dictionary,
            global::System.IO.Stream output,
            global::System.Action<TKey, global::System.IO.Stream> keySerializer,
            global::System.Action<TValue, global::System.IO.Stream> valueSerializer)
        {
            SerializationHelpers.Serialize(dictionary.Count, output);
            foreach (var iter in dictionary)
            {
                keySerializer(iter.Key, output);
                valueSerializer(iter.Value, output);
            }
        }

        public static global::System.Collections.Generic.SortedDictionary<TKey, TValue>
                DeserializeDictionary<TKey, TValue>(
            global::System.IO.Stream input,
            global::System.Func<global::System.IO.Stream, TKey> keyDeserializer,
            global::System.Func<global::System.IO.Stream, TValue> valueDeserializer)
        {
            var size = SerializationHelpers.DeserializeInt32(input);
            var result = new global::System.Collections.Generic.SortedDictionary<TKey, TValue>();
            for (var i = 0; i < size; i++)
            {
                var key = keyDeserializer(input);
                var value = valueDeserializer(input);
                result[key] = value;
            }
            return result;
        }
    }

    static class HashHelper
    {
        public static int GetListHashCode<T>(global::System.Collections.Generic.IList<T> list)
        {
            int result = 0;
            foreach (var element in list)
                result = ((result << 19) | (result >> 13)) ^ (element.GetHashCode());
            return result;
        }

        public static int GetDictionaryHashCode<TKey, TValue>(
            global::System.Collections.Generic.IDictionary<TKey, TValue> dictionary)
        {
            int result = 0;
            foreach (var iter in dictionary)
            {
                result = ((result << 19) | (result >> 13)) ^ (iter.Key.GetHashCode());
                result = ((result << 19) | (result >> 13)) ^ (iter.Value.GetHashCode());
            }
            return result;
        }
    }");
        }
    }
}
