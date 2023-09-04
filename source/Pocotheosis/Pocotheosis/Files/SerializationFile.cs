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

            using var file = File.CreateText(outputFileName);
            WriteNamespaceHeader(dataModel, file,
                new[] { "_nsS_", "_nsG_", "_nsL_", "_nsI_", "_nsT_", "_nsGl_", "_nsSC_" });
            WriteSerializationHelperClass(file, dataModel);
            foreach (var pocoClass in dataModel.Classes)
                WriteClassSerialization(pocoClass, file);
            WriteNamespaceFooter(file);
        }

        static void WriteSerializationHelperClass(TextWriter output,
            PocoNamespace dataModel)
        {
            output.EmitCode(
@"    public interface ISerializablePoco : IPoco
    {
        public void Serialize(_nsI_.Stream output);
        public void SerializeWithId(_nsI_.Stream output);
        public string Checksum { get; }
    }

    public abstract partial class Poco
    {
        protected static void SerializeWithId(int streamingId,
                _nsS_.Action<_nsI_.Stream> serializer, _nsI_.Stream output)
        {
            Serialize(streamingId, output);
            serializer(output);
        }

        public static T DeserializeNullable<T>(int streamingId,
                _nsS_.Func<_nsI_.Stream, T> deserializer, _nsI_.Stream input) where T: Poco
        {
            var id = DeserializePocoIdentifier(input);
            if (id == null)
                throw new _nsI_.InvalidDataException(""Unexpected end of stream"");
            if (id == -1) return null;
            if (id != streamingId)
                throw new _nsI_.InvalidDataException(""Unexpected POCO type"");
            return deserializer(input);
        }

        public static Poco DeserializeWithId(_nsI_.Stream input)
        {
            var id = DeserializePocoIdentifier(input);
            if (id == null) throw new _nsI_.InvalidDataException(""Unexpected end of stream"");
            if (id == -1) return null;

            Poco result;
            switch (id)
            {"
            );
            foreach (var pocoClass in dataModel.Classes) output.EmitCode(
$"                case {pocoClass.StreamingId}:",
$"                    result = {pocoClass.Name}.Deserialize(input);",
$"                    break;"
            );
            output.EmitCode(
@"                default:
                    throw new _nsI_.InvalidDataException();
            }

            return result;
        }

        protected static string ComputeChecksum(_nsS_.Action<_nsI_.Stream> serializer)
        {
            var buffer = new _nsI_.MemoryStream();
            serializer(buffer);
            buffer.Seek(0, _nsI_.SeekOrigin.Begin);
            using (var sha256 = _nsSC_.SHA256.Create())
            {
                var hash = sha256.ComputeHash(buffer);
                var chars = _nsL_.Enumerable.Select(hash, b => b.ToString(
                    ""x2"", _nsGl_.CultureInfo.InvariantCulture));
                return string.Join(string.Empty, chars);
            }
        }

        protected static void Serialize(bool value, _nsI_.Stream output)
        {
            output.WriteByte(value ? (byte)0xFF : (byte)0x00);
        }

        protected static void Serialize(byte value, _nsI_.Stream output)
        {
            output.WriteByte(value);
        }

        protected static void Serialize(short value, _nsI_.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
        }

        protected static void Serialize(int value, _nsI_.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
            output.WriteByte((byte)((value >> 16) & 0xFF));
            output.WriteByte((byte)((value >> 24) & 0xFF));
        }

        protected static void Serialize(long value, _nsI_.Stream output)
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

        protected static void Serialize(sbyte value, _nsI_.Stream output)
        {
            output.WriteByte((byte)value);
        }

        protected static void Serialize(ushort value, _nsI_.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
        }

        protected static void Serialize(uint value, _nsI_.Stream output)
        {
            output.WriteByte((byte)((value >> 0) & 0xFF));
            output.WriteByte((byte)((value >> 8) & 0xFF));
            output.WriteByte((byte)((value >> 16) & 0xFF));
            output.WriteByte((byte)((value >> 24) & 0xFF));
        }

        protected static void Serialize(ulong value, _nsI_.Stream output)
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

        protected static void Serialize(string value, _nsI_.Stream output)
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
$"        protected static void Serialize({enume.Name} value, _nsI_.Stream output)",
$"        {{",
$"            Serialize((byte)value, output);",
$"        }}"
            );
            foreach (var classe in dataModel.Classes) output.EmitCode(
$"",
$"        protected static void Serialize({classe.Name} value, _nsI_.Stream output)",
$"        {{",
$"            value.Serialize(output);",
$"        }}",
$"",
$"        protected static void SerializeWithId({classe.Name} value, _nsI_.Stream output)",
$"        {{",
$"            if (value == null)",
$"                Serialize(-1, output);",
$"            else",
$"                value.SerializeWithId(output);",
$"        }}"
            );
            output.EmitCode(
@"
        protected static bool DeserializeBool(_nsI_.Stream input)
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

        protected static byte DeserializeByte(_nsI_.Stream input)
        {
            var result = input.ReadByte();
            if (result == -1)
                throw new _nsI_.InvalidDataException(""Unexpected end-of-stream"");
            return (byte)result;
        }

        protected static short DeserializeInt16(_nsI_.Stream input)
        {
            var byte0 = DeserializeByte(input);
            var byte1 = DeserializeByte(input);
            return (short)(
                (byte1 << 8) |
                (byte0));
        }

        protected static int DeserializeInt32(_nsI_.Stream input)
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

        protected static int? DeserializePocoIdentifier(_nsI_.Stream input)
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

        protected static long DeserializeInt64(_nsI_.Stream input)
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

        protected static sbyte DeserializeSByte(_nsI_.Stream input)
        {
            return (sbyte)DeserializeByte(input);
        }

        protected static ushort DeserializeUInt16(_nsI_.Stream input)
        {
            return (ushort)DeserializeInt16(input);
        }

        protected static uint DeserializeUInt32(_nsI_.Stream input)
        {
            return (uint)DeserializeInt32(input);
        }

        protected static ulong DeserializeUInt64(_nsI_.Stream input)
        {
            return (ulong)DeserializeInt64(input);
        }");

            foreach (var enume in dataModel.Enums) output.EmitCode(
$"",
$"        protected static {enume.Name} Deserialize{enume.Name}(_nsI_.Stream input)",
$"        {{",
$"            return ({enume.Name})DeserializeByte(input);",
$"        }}"
            );

            output.EmitCode(
@"
        protected static string DeserializeString(_nsI_.Stream input)
        {
            var length = DeserializeInt32(input);
            if (length == -1)
                return null;
            var bytes = new byte[length];
            foreach (var i in _nsL_.Enumerable.Range(0, bytes.Length))
                bytes[i] = DeserializeByte(input);
            return _nsT_.Encoding.UTF8.GetString(bytes);
        }

        protected static void Serialize<T>(_nsG_.IList<T> array,
            _nsI_.Stream output, _nsS_.Action<T, _nsI_.Stream> elementSerializer)
        {
            Serialize(array.Count, output);
            for (var i = 0; i < array.Count; i++)
                elementSerializer(array[i], output);
        }

        protected static _nsG_.IList<T> DeserializeList<T>(_nsI_.Stream input,
            _nsS_.Func<_nsI_.Stream, T> elementDeserializer)
        {
            var size = DeserializeInt32(input);
            var result = new T[size];
            for (var i = 0; i < size; i++)
                result[i] = elementDeserializer(input);
            return result;
        }

        protected static void Serialize<TKey, TValue>(
            _nsG_.SortedDictionary<TKey, TValue> dictionary,
            _nsI_.Stream output,
            _nsS_.Action<TKey, _nsI_.Stream> keySerializer,
            _nsS_.Action<TValue, _nsI_.Stream> valueSerializer)
        {
            Serialize(dictionary.Count, output);
            foreach (var iter in dictionary)
            {
                keySerializer(iter.Key, output);
                valueSerializer(iter.Value, output);
            }
        }

        protected static _nsG_.SortedDictionary<TKey, TValue> DeserializeDictionary<TKey, TValue>(
            _nsI_.Stream input,
            _nsS_.Func<_nsI_.Stream, TKey> keyDeserializer,
            _nsS_.Func<_nsI_.Stream, TValue> valueDeserializer)
        {
            var size = DeserializeInt32(input);
            var result = new _nsG_.SortedDictionary<TKey, TValue>();
            for (var i = 0; i < size; i++)
            {
                var key = keyDeserializer(input);
                var value = valueDeserializer(input);
                result[key] = value;
            }
            return result;
        }
    }");
        }

        private static void WriteClassSerialization(PocoClass clasz, TextWriter output)
        {

            output.EmitCode(
$"",
$"    public partial class {clasz.Name} : ISerializablePoco",
$"    {{"
            );
            WriteClassSerializationContents(clasz, output);
            output.EmitCode(
$"    }}"
            ); ;
        }

        public static void WriteClassSerializationContents(PocoClass clasz, TextWriter output)
        {
            var constructorParams = string.Join(", ",
                clasz.Members.Select(member => member.BackingStoreName));

            output.EmitCode(
$"        public string Checksum",
$"        {{",
$"            get {{ return ComputeChecksum(SerializeWithId); }}",
$"        }}",
$"",
$"        public void SerializeWithId(_nsI_.Stream output)",
$"        {{",
$"            SerializeWithId({clasz.StreamingId}, Serialize, output);",
$"        }}",
$"",
$"        public void Serialize(_nsI_.Stream output)",
$"        {{"
            );
            foreach (var member in clasz.Members) output.EmitCode(
$"            {member.Serializer()}"
            );
            output.EmitCode(
$"        }}",
$"",
$"        public static {clasz.Name} DeserializeNullable(_nsI_.Stream input)",
$"        {{",
$"            return DeserializeNullable({clasz.StreamingId}, Deserialize, input);",
$"        }}",
$"",
$"        public static {clasz.Name} Deserialize(_nsI_.Stream input)",
$"        {{"
            );
            foreach (var member in clasz.Members) output.EmitCode(
$"            {member.Deserializer()}"
            );
            output.EmitCode(
$"            return new {clasz.Name}({constructorParams});",
$"        }}"
            );
        }
    }
}

namespace Pocotheosis.MemberTypes
{
    partial class PrimitiveType
    {
        public string GetDeserializer(string privateName)
        {
            return $"var {privateName} = {DeserializerMethod}(input);";
        }

        public string GetSerializer(string variableName, string privateName)
        {
            return $"{SerializerMethod}({privateName}, output);";
        }
    }

    partial class ArrayType
    {
        public string GetDeserializer(string privateName)
        {
            return $"var {privateName} = DeserializeList(input, "
                + $"{elementType.DeserializerMethod});";
        }

        public string GetSerializer(string variableName, string privateName)
        {
            return $"Serialize({privateName}, output, {elementType.SerializerMethod});";
        }
    }

    partial class DictionaryType
    {
        public string GetDeserializer(string privateName)
        {
            return $"var {privateName} = DeserializeDictionary(input, "
                + $"{keyType.DeserializerMethod}, {valueType.DeserializerMethod});";
        }

        public string GetSerializer(string variableName, string privateName)
        {
            return $"Serialize({privateName}, output, "
                + $"{keyType.SerializerMethod}, {valueType.SerializerMethod});";
        }
    }
}