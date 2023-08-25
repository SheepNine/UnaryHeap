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
                file.EmitCode(
@"    public abstract partial class Poco
    {
        public abstract void Serialize(_nsI_.Stream output);

        public string Checksum
        {
            get
            {
                var buffer = new _nsI_.MemoryStream();
                SerializeWithId(buffer);
                buffer.Seek(0, _nsI_.SeekOrigin.Begin);
                using (var sha256 = global::System.Security.Cryptography.SHA256.Create())
                {
                    var hash = sha256.ComputeHash(buffer);
                    var chars = _nsL_.Enumerable.Select(hash, b => b.ToString(
                        ""x2"", global::System.Globalization.CultureInfo.InvariantCulture));
                    return string.Join(string.Empty, chars);
                }
            }
        }
    }"
                );
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
                "_nsI_.Stream output)");
            output.WriteLine("\t\t{");

            foreach (var member in clasz.Members) output.EmitCode(
$"            {member.Serializer()}"
            );

            output.WriteLine("\t\t}");
            output.WriteLine();
            output.Write("\t\tpublic static ");
            output.Write(clasz.Name);
            output.WriteLine(" Deserialize(_nsI_.Stream input)");
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
            output.EmitCode(
@"    public static class SerializationHelpers
    {
        public static void Serialize(bool value, _nsI_.Stream output)
        {
            output.WriteByte(value ? (byte)0xFF : (byte)0x00);
        }

        public static void Serialize(byte value, _nsI_.Stream output)
        {
            output.WriteByte(value);
        }

        public static void Serialize(short value, _nsI_.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
        }

        public static void Serialize(int value, _nsI_.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
            output.WriteByte((byte)((value >> 16) & 0xFF));
            output.WriteByte((byte)((value >> 24) & 0xFF));
        }

        public static void Serialize(long value, _nsI_.Stream output)
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

        public static void Serialize(sbyte value, _nsI_.Stream output)
        {
            output.WriteByte((byte)value);
        }

        public static void Serialize(ushort value, _nsI_.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
        }

        public static void Serialize(uint value, _nsI_.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
            output.WriteByte((byte)((value >> 16) & 0xFF));
            output.WriteByte((byte)((value >> 24) & 0xFF));
        }

        public static void Serialize(ulong value, _nsI_.Stream output)
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

        public static void Serialize(string value, _nsI_.Stream output)
        {
            if (value == null) {
                Serialize(-1, output);
            }
            else {
                var bytes = _nsT_.Encoding.UTF8.GetBytes(value);
                Serialize(bytes.Length, output);
                foreach (var b in bytes)
                    output.WriteByte(b);
            }
        }"
            );
            foreach (var enume in dataModel.Enums) output.EmitCode(
$"",
$"        public static void Serialize({enume.Name} value, _nsI_.Stream output)",
$"        {{",
$"            Serialize((byte)value, output);",
$"        }}"
            );
            foreach (var classe in dataModel.Classes) output.EmitCode(
$"",
$"        public static void Serialize({classe.Name} value, _nsI_.Stream output)",
$"        {{",
$"            value.Serialize(output);",
$"        }}",
$"",
$"        public static void SerializeWithId({classe.Name} value, _nsI_.Stream output)",
$"        {{",
$"            if (value == null)",
$"                Serialize(-1, output);",
$"            else",
$"                value.SerializeWithId(output);",
$"        }}"
            );
            output.EmitCode(
@"
        public static bool DeserializeBool(_nsI_.Stream input)
        {
            switch (DeserializeByte(input))
            {
                case 0x00:
                    return false;
                case 0xFF:
                    return true;
                default:
                    throw new _nsI_.InvalidDataException(""Invalid boolean value"");
            }
        }

        public static byte DeserializeByte(_nsI_.Stream input)
        {
            var result = input.ReadByte();
            if (result == -1)
                throw new _nsI_.InvalidDataException(""Unexpected end-of-stream"");
            return (byte)result;
        }

        public static short DeserializeInt16(_nsI_.Stream input)
        {
            var byte0 = DeserializeByte(input);
            var byte1 = DeserializeByte(input);
            return (short)(
                (byte1 << 8) |
                (byte0));
        }

        public static int DeserializeInt32(_nsI_.Stream input)
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

        public static int? DeserializePocoIdentifier(_nsI_.Stream input)
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

        public static long DeserializeInt64(_nsI_.Stream input)
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

        public static sbyte DeserializeSByte(_nsI_.Stream input)
        {
            return (sbyte)DeserializeByte(input);
        }

        public static ushort DeserializeUInt16(_nsI_.Stream input)
        {
            return (ushort)DeserializeInt16(input);
        }

        public static uint DeserializeUInt32(_nsI_.Stream input)
        {
            return (uint)DeserializeInt32(input);
        }

        public static ulong DeserializeUInt64(_nsI_.Stream input)
        {
            return (ulong)DeserializeInt64(input);
        }");

            foreach (var enume in dataModel.Enums) output.EmitCode(
$"        public static {enume.Name} Deserialize{enume.Name}(_nsI_.Stream input)",
$"        {{",
$"            return ({enume.Name})DeserializeByte(input);",
$"        }}"
            );

            output.EmitCode(
@"        public static string DeserializeString(_nsI_.Stream input)
        {
            var length = DeserializeInt32(input);
            if (length == -1)
                return null;
            var bytes = new byte[length];
            foreach (var i in _nsL_.Enumerable.Range(0, bytes.Length))
                bytes[i] = DeserializeByte(input);
            return _nsT_.Encoding.UTF8.GetString(bytes);
        }

        public static void SerializeList<T>(_nsG_.IList<T> array,
            _nsI_.Stream output, _nsS_.Action<T, _nsI_.Stream> elementSerializer)
        {
            SerializationHelpers.Serialize(array.Count, output);
            for (var i = 0; i < array.Count; i++)
                elementSerializer(array[i], output);
        }

        public static _nsG_.IList<T> DeserializeList<T>(_nsI_.Stream input,
            _nsS_.Func<_nsI_.Stream, T> elementDeserializer)
        {
            var size = SerializationHelpers.DeserializeInt32(input);
            var result = new T[size];
            for (var i = 0; i < size; i++)
                result[i] = elementDeserializer(input);
            return result;
        }

        public static void SerializeDictionary<TKey, TValue>(
            _nsG_.SortedDictionary<TKey, TValue> dictionary,
            _nsI_.Stream output,
            _nsS_.Action<TKey, _nsI_.Stream> keySerializer,
            _nsS_.Action<TValue, _nsI_.Stream> valueSerializer)
        {
            SerializationHelpers.Serialize(dictionary.Count, output);
            foreach (var iter in dictionary)
            {
                keySerializer(iter.Key, output);
                valueSerializer(iter.Value, output);
            }
        }

        public static _nsG_.SortedDictionary<TKey, TValue> DeserializeDictionary<TKey, TValue>(
            _nsI_.Stream input,
            _nsS_.Func<_nsI_.Stream, TKey> keyDeserializer,
            _nsS_.Func<_nsI_.Stream, TValue> valueDeserializer)
        {
            var size = SerializationHelpers.DeserializeInt32(input);
            var result = new _nsG_.SortedDictionary<TKey, TValue>();
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
        public static int GetListHashCode<T>(_nsG_.IList<T> list)
        {
            int result = 0;
            foreach (var element in list)
                result = ((result << 19) | (result >> 13))
                    ^ (element == null ? 0x0EADBEEF : element.GetHashCode());
            return result;
        }

        public static int GetDictionaryHashCode<TKey, TValue>(
            _nsG_.IDictionary<TKey, TValue> dictionary)
        {
            int result = 0;
            foreach (var iter in dictionary)
            {
                result = ((result << 19) | (result >> 13)) ^ (iter.Key.GetHashCode());
                result = ((result << 19) | (result >> 13))
                    ^ (iter.Value == null ? 0x0EADBEEF : iter.Value.GetHashCode());
            }
            return result;
        }
    }");
        }
    }
}

namespace Pocotheosis.MemberTypes
{
    partial class PrimitiveType
    {
        public string GetDeserializer(string variableName)
        {
            return $"var t{variableName} = {DeserializerMethod}(input);";
        }

        public string GetSerializer(string variableName, string privateName)
        {
            return $"{SerializerMethod}({privateName}, output);";
        }
    }

    partial class ArrayType
    {
        public string GetDeserializer(string variableName)
        {
            return $"var t{variableName} = SerializationHelpers.DeserializeList(input, "
                + $"{elementType.DeserializerMethod});";
        }

        public string GetSerializer(string variableName, string privateName)
        {
            return $"SerializationHelpers.SerializeList({privateName}, output, "
                + $"{elementType.SerializerMethod});";
        }
    }

    partial class DictionaryType
    {
        public string GetDeserializer(string variableName)
        {
            return $"var t{variableName} = SerializationHelpers.DeserializeDictionary(input, "
                + $"{keyType.DeserializerMethod}, {valueType.DeserializerMethod});";
        }

        public string GetSerializer(string variableName, string privateName)
        {
            return $"SerializationHelpers.SerializeDictionary({privateName}, output, "
                + $"{keyType.SerializerMethod}, {valueType.SerializerMethod});";
        }
    }
}