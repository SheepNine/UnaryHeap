using System;
using System.Collections.Generic;
using System.IO;

namespace Pocotheosis
{
    static class BoilerplateCode
    {
        public static void WriteConstructorHelperClass(TextWriter output,
            PocoNamespace dataModel)
        {
            output.WriteLine(@"
    static class ConstructorHelper
    {
        public static bool CheckValue(bool value)
        {
            return true;
        }
        public static bool CheckValue(string value)
        {
            return value != null;
        }
        public static bool CheckValue(byte value)
        {
            return true;
        }
        public static bool CheckValue(ushort value)
        {
            return true;
        }
        public static bool CheckValue(uint value)
        {
            return true;
        }
        public static bool CheckValue(ulong value)
        {
            return true;
        }
        public static bool CheckValue(sbyte value)
        {
            return true;
        }
        public static bool CheckValue(short value)
        {
            return true;
        }
        public static bool CheckValue(int value)
        {
            return true;
        }
        public static bool CheckValue(long value)
        {
            return true;
        }");

            foreach (var enume in dataModel.Enums)
            {
                output.WriteLine(string.Format("        public static bool CheckValue("
                    + "{0} value) "
                    + "{{ return true; }}", enume.Name));
            }

            foreach (var classe in dataModel.Classes)
            {
                output.WriteLine(string.Format("        public static bool CheckValue("
                    + "{0} value) "
                    + "{{ return value != null; }}", classe.Name));
            }

            output.WriteLine(@"        public static bool CheckArrayValue<T>(
            global::System.Collections.Generic.IEnumerable<T> memberValues,
            global::System.Func<T, bool> memberChecker)
        {
            if (memberValues == null)
                return false;
            foreach (var memberValue in memberValues)
                if (!memberChecker(memberValue))
                    return false;
            return true;
        }

        public static bool CheckDictionaryValue<TKey, TValue>(
            global::System.Collections.Generic.IDictionary<TKey, TValue> memberValues,
            global::System.Func<TKey, bool> keyChecker,
            global::System.Func<TValue, bool> valueChecker)
        {
            if (memberValues == null)
                return false;
            foreach (var memberValue in memberValues)
                if (!keyChecker(memberValue.Key) || !valueChecker(memberValue.Value))
                    return false;
            return true;
        }
    }");
        }
        public static void WriteEqualityHelperClass(TextWriter output,
            PocoNamespace dataModel)
        {
            output.WriteLine(@"    static class EquatableHelper
    {
        public static bool AreEqual(bool a, bool b) { return a == b; }
        public static bool AreEqual(string a, string b) { return string.Equals(a, b); }
        public static bool AreEqual(byte a, byte b) { return a == b; }
        public static bool AreEqual(ushort a, ushort b) { return a == b; }
        public static bool AreEqual(uint a, uint b) { return a == b; }
        public static bool AreEqual(ulong a, ulong b) { return a == b; }
        public static bool AreEqual(sbyte a, sbyte b) { return a == b; }
        public static bool AreEqual(short a, short b) { return a == b; }
        public static bool AreEqual(int a, int b) { return a == b; }
        public static bool AreEqual(long a, long b) { return a == b; }");

            foreach (var enume in dataModel.Enums)
            {
                output.WriteLine(string.Format("        public static bool AreEqual("
                    + "{0} a, {0} b) "
                    + "{{ return a == b; }}", enume.Name));
            }

            foreach (var classe in dataModel.Classes)
            {
                output.WriteLine(string.Format("        public static bool AreEqual("
                    + "{0} a, {0} b) "
                    + "{{ return a.Equals(b); }}", classe.Name));
            }

        output.WriteLine(@"        public static bool ListEquals<T>("
            + @"global::System.Collections.Generic.IList<T> a, "
            + @"global::System.Collections.Generic.IList<T> b, "
            + @"global::System.Func<T, T, bool> comparator)
        {
            if (a.Count != b.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
                if (!comparator(a[i], b[i]))
                    return false;

            return true;
        }

        public static bool DictionaryEquals<TKey, TValue>(global::System.Collections.Generic.SortedDictionary<TKey, TValue> a,
            global::System.Collections.Generic.SortedDictionary<TKey, TValue> b,
            global::System.Func<TValue, TValue, bool> valueComparator)
        {
            if (a.Count != b.Count)
                return false;

            foreach (var key in a.Keys)
            {
                if (!b.ContainsKey(key))
                    return false;
                if (!valueComparator(a[key], b[key]))
                    return false;
            }
            return true;
        }
    }");
        }

        public static void WriteToStringHelperClass(TextWriter output,
            PocoNamespace dataModel)
        {
            output.WriteLine(@"    static class ToStringHelper
    {
        public static string FormatValue(bool value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(string value, global::System.IFormatProvider format)
        {
            return value;
        }
        public static string FormatValue(byte value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(ushort value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(uint value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(ulong value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(sbyte value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(short value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(int value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }
        public static string FormatValue(long value, global::System.IFormatProvider format)
        {
            return value.ToString(format);
        }");

            foreach (var enume in dataModel.Enums)
            {
                output.WriteLine(
@"        public static string FormatValue({0} value, global::System.IFormatProvider format)
        {{
            return value.ToString();
        }}", enume.Name);
            }

            foreach (var classe in dataModel.Classes)
            {
                output.WriteLine(
@"        public static string FormatValue({0} value, global::System.IFormatProvider format)
        {{
            return value.ToString();
        }}", classe.Name);
            }

            output.WriteLine(@"        public static void WriteArrayMember<T>(
            global::System.Text.StringBuilder builder,
            string memberName, global::System.Collections.Generic.IList<T> memberValues,
            global::System.Func<T, global::System.IFormatProvider, string> memberFormatter,
            global::System.IFormatProvider format)
        {
            builder.AppendLine();
            builder.Append(""\t"");
            builder.Append(memberName);
            builder.Append("": "");
            if (memberValues.Count > 0)
                builder.Append(string.Join("", "", global::System.Linq.Enumerable.Select(
                    memberValues, member => memberFormatter(member, format))));
            else
                builder.Append(""<empty>"");
        }

        public static void WriteDictionaryMember<TKey, TValue>(
            global::System.Text.StringBuilder builder,
            string memberName, global::System.Collections.Generic.SortedDictionary<TKey, TValue> memberValues,
            global::System.Func<TKey, global::System.IFormatProvider, string> keyFormatter,
            global::System.Func<TValue, global::System.IFormatProvider, string> valueFormatter,
            global::System.IFormatProvider format)
        {
            if (memberValues.Count > 0)
            {
                foreach (var iter in memberValues)
                {
                    builder.AppendLine();
                    builder.Append(""\t"");
                    builder.Append(keyFormatter(iter.Key, format));
                    builder.Append("": "");
                    builder.Append(valueFormatter(iter.Value, format));
                }
            }
            else
            {
                builder.Append(""<empty>"");
            }
        }

        public static void WriteMember<T>(global::System.Text.StringBuilder builder,
            string memberName, T memberValue,
            global::System.Func<T, global::System.IFormatProvider, string> memberFormatter,
            global::System.IFormatProvider format)
        {
            builder.AppendLine();
            builder.Append(""\t"");
            builder.Append(memberName);
            builder.Append("": "");
            builder.Append(memberFormatter(memberValue, format));
        }
    }");
        }

        public static void WriteSerializationHelperClass(TextWriter output,
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
            var bytes = global::System.Text.Encoding.UTF8.GetBytes(value);
            Serialize(bytes.Length, output);
            foreach (var b in bytes)
                output.WriteByte(b);
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
                (byte7 << 56) |
                (byte6 << 48) |
                (byte5 << 40) |
                (byte4 << 32) |
                (byte3 << 24) |
                (byte2 << 16) |
                (byte1 << 8) |
                (byte0));
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
            var bytes = new byte[DeserializeInt32(input)];
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

        public static global::System.Collections.Generic.SortedDictionary<TKey, TValue> DeserializeDictionary<TKey, TValue>(
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

        public static int GetDictionaryHashCode<TKey, TValue>(global::System.Collections.Generic.IDictionary<TKey, TValue> dictionary)
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

        public static void WriteRoutingClass(TextWriter output, string route)
        {
            output.WriteLine(@"    public interface I{0}RoutedPoco
    {{
        void RouteTo(I{0}Destination destination);
    }}

    public class {0}Router
    {{
        global::System.Collections.Generic.List<I{0}Destination> destinations;

        public {0}Router(params I{0}Destination[] destinaions)
            :this((global::System.Collections.Generic.IEnumerable<I{0}Destination>)destinaions)
        {{

        }}

        public {0}Router(global::System.Collections.Generic.IEnumerable<I{0}Destination> " +
        @"destinations)
        {{
            this.destinations = global::System.Linq.Enumerable.ToList(destinations);
        }}

        public void Route(I{0}RoutedPoco poco)
        {{
            foreach (var destination in destinations)
            {{
                poco.RouteTo(destination);
            }}
        }}
    }}", route);
        }

        public static void WriteStreamingCommonClasses(TextWriter output)
        {
            output.WriteLine(@"    public interface IPocoSource
    {
        Poco Receive();
    }

    public class PocoReader : IPocoSource, global::System.IDisposable
    {
        global::System.IO.Stream source;

        public PocoReader(global::System.IO.Stream source)
        {
            this.source = source;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        public Poco Receive()
        {
            return Poco.DeserializeWithId(source);
        }
    }

    public interface IPocoSink
    {
        IPocoSink Send(Poco poco);
        IPocoSink Flush();
    }

    public class PocoWriter : IPocoSink, global::System.IDisposable
    {
        global::System.IO.Stream destination;

        public PocoWriter(global::System.IO.Stream destination)
        {
            this.destination = destination;
        }

        public void Dispose()
        {
            destination.Dispose();
        }

        public IPocoSink Send(Poco poco)
        {
            poco.SerializeWithId(destination);
            return this;
        }

        public IPocoSink Flush()
        {
            destination.Flush();
            return this;
        }
    }");
        }

        public static void WriteStreamingBaseClass(TextWriter output, PocoNamespace dataModel)
        {
            output.WriteLine(@"    public partial class Poco
    {
        protected abstract int getIdentifier();

        public void SerializeWithId(global::System.IO.Stream output)
        {
            SerializationHelpers.Serialize(getIdentifier(), output);
            Serialize(output);
        }

        public static Poco DeserializeWithId(global::System.IO.Stream input)
        {
            var id = SerializationHelpers.DeserializePocoIdentifier(input);
            if (id == null) return null;

            switch (id)
            {");

            foreach (var pocoClass in dataModel.Classes)
            {
                output.Write("\t\t\tcase ");
                output.Write(pocoClass.Name);
                output.WriteLine(".Identifier:");
                output.Write("\t\t\t\treturn ");
                output.Write(pocoClass.Name);
                output.WriteLine(".Deserialize(input);");
            }

            output.WriteLine(
@"                default:
                    throw new global::System.IO.InvalidDataException();
            }
        }
    }

    public abstract class LengthPrefixedPocoStreamer : IPocoSink
    {
        const int BUFFER_SIZE = 2 + 0xFFFF;

        byte[] readBuffer = new byte[BUFFER_SIZE];
        byte[] writeBuffer = new byte[BUFFER_SIZE];
        int validBytes = 0;
        global::System.IO.Stream stream;
        bool isClosed = false;
        global::System.Collections.Concurrent.BlockingCollection<Poco> writeObjects =
            new global::System.Collections.Concurrent.BlockingCollection<Poco>();

        public LengthPrefixedPocoStreamer(global::System.IO.Stream stream)
        {
            this.stream = stream;
            new global::System.Threading.Thread(WriterMain) { IsBackground = true }.Start();
        }

        protected void BeginRead()
        {
            stream.BeginRead(readBuffer, validBytes,
                BUFFER_SIZE - validBytes, ReaderMain, null);
        }

        private void ReaderMain(global::System.IAsyncResult result)
        {
            try
            {
                int bytesRead = stream.EndRead(result);

                if (bytesRead == 0)
                {
                    Close();
                    Deliver(MakeConnectionLostPoco());
                }
                else
                {
                    validBytes += bytesRead;
                    UnframeMessages();
                    BeginRead();
                }
            }
            catch (global::System.Exception)
            {
                Close();
                Deliver(MakeConnectionLostPoco());
            }
        }

        protected abstract Poco MakeConnectionLostPoco();

        void UnframeMessages()
        {
            while (true)
            {
                if (validBytes < 2)
                    return;

                int frameDataSize = (ushort)(readBuffer[0] | (readBuffer[1] << 8));

                if (validBytes < frameDataSize + 2)
                    return;

                using (var tempStream = new global::System.IO.MemoryStream(
                        readBuffer, 2, frameDataSize))
                    Deliver(Poco.DeserializeWithId(tempStream));

                var validDataOffset = 2 + frameDataSize;
                var remainingBytes = validBytes - validDataOffset;

                for (int i = 0; i < remainingBytes; i++)
                    readBuffer[i] = readBuffer[i + validDataOffset];

                validBytes = remainingBytes;
            }
        }

        protected abstract void Deliver(Poco poco);

        void WriterMain()
        {
            try
            {

                using (var tempStream = new global::System.IO.MemoryStream(
                        writeBuffer, 2, BUFFER_SIZE - 2))
                {
                    while (true)
                    {
                        tempStream.Seek(0, global::System.IO.SeekOrigin.Begin);
                        writeObjects.Take().SerializeWithId(tempStream);
                        var frameSize = tempStream.Position;
                        writeBuffer[0] = (byte)(frameSize & 0xFF);
                        writeBuffer[1] = (byte)((frameSize >> 8) & 0xFF);
                        stream.Write(writeBuffer, 0, (int)(frameSize + 2));
                    }
                }
            }
            catch (global::System.Exception)
            {
                Close();
            }
        }

        object closeLock = new object();
        public void Close()
        {
            if (isClosed)
                return;

            lock (closeLock)
            {
                stream.Close();
                isClosed = true;
            }
        }

        public IPocoSink Send(Poco poco)
        {
            writeObjects.Add(poco);
            return this;
        }

        public IPocoSink Flush()
        {
            return this;
        }
    }");
        }

        public static void WriteNetworkingClientClasses(TextWriter output)
        {
            output.WriteLine(@"    abstract partial class ClientControlPoco : Poco
    {
    }

    class ServerConnectionLost : ClientControlPoco
    {
        public const int Identifier = 1;

        public ServerConnectionLost()
        {
        }

        public override void Serialize(global::System.IO.Stream output)
        {
        }

        public static ServerConnectionLost Deserialize(global::System.IO.Stream input)
        {
            return new ServerConnectionLost();
        }

        protected override int getIdentifier()
        {
            return Identifier;
        }

        public override string ToString()
        {
            return ""<DISCONNECTED>"";
        }
    }

    public class PocoClientEndpoint : LengthPrefixedPocoStreamer, IPocoSource
    {
        private global::System.EventHandler receiveHandler;
        private global::System.Collections.Concurrent.BlockingCollection<Poco> readObjects;

        public PocoClientEndpoint(global::System.IO.Stream stream) : this(stream, null)
        {

        }

        public PocoClientEndpoint(global::System.IO.Stream stream,
            global::System.EventHandler receiveHandler) : base(stream)
        {
            this.receiveHandler = receiveHandler ?? ((sender, e) => { });
            readObjects = new global::System.Collections.Concurrent.BlockingCollection<Poco>();
            BeginRead();
        }

        protected override void Deliver(Poco poco)
        {
            readObjects.Add(poco);
            receiveHandler(this, global::System.EventArgs.Empty);
        }

        public bool HasData
        {
            get { return readObjects.Count > 0; }
        }

        public Poco Receive()
        {
            return readObjects.Take();
        }

        protected override Poco MakeConnectionLostPoco()
        {
            return new ServerConnectionLost();
        }
    }");
        }

        public static void WriteNetworkingServerClasses(TextWriter output)
        {
            output.WriteLine(@"    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    abstract partial class ServerControlPoco : Poco
    {
        public const byte TypeIdentifier = 0xff;

        public static Poco DeserializeControlPocoWithId(Stream input)
        {
            var id = SerializationHelpers.DeserializePocoIdentifier(input);
            if (id == null) return null;

            switch (id)
            {
                case ClientConnectionAdded.Identifier:
                    return ClientConnectionAdded.Deserialize(input);
                case ClientConnectionLost.Identifier:
                    return ClientConnectionLost.Deserialize(input);
                case ShutdownRequested.Identifier:
                    return ShutdownRequested.Deserialize(input);
                default:
                    throw new InvalidDataException();
            }
        }
    }

    class ClientConnectionLost : ServerControlPoco
    {
        public const int Identifier = 1;

        public ClientConnectionLost()
        {
        }

        public override void Serialize(global::System.IO.Stream output)
        {
        }

        public static ClientConnectionLost Deserialize(global::System.IO.Stream input)
        {
            return new ClientConnectionLost();
        }

        protected override int getIdentifier()
        {
            return Identifier;
        }

        public override string ToString()
        {
            return ""<DISCONNECTED>"";
        }
    }

    class ClientConnectionAdded : ServerControlPoco
    {
        public const int Identifier = 2;

        public ClientConnectionAdded()
        {
        }

        public override void Serialize(Stream output)
        {
        }

        public static ClientConnectionAdded Deserialize(Stream input)
        {
            return new ClientConnectionAdded();
        }

        protected override int getIdentifier()
        {
            return Identifier;
        }

        public override string ToString()
        {
            return ""<JOINED>"";
        }
    }

    class ShutdownRequested : ServerControlPoco
    {
        public const int Identifier = 3;

        public ShutdownRequested()
        {
        }

        public override void Serialize(Stream output)
        {
        }

        public static ShutdownRequested Deserialize(Stream input)
        {
            return new ShutdownRequested();
        }

        protected override int getIdentifier()
        {
            return Identifier;
        }

        public override string ToString()
        {
            return ""<SHUTDOWN>"";
        }
    }

    public class PocoServerEndpoint
    {
        class PocoServerConnection : LengthPrefixedPocoStreamer
        {
            private BlockingCollection<Tuple<Guid, Poco>> readObjects;
            Guid id;

            public PocoServerConnection(
                BlockingCollection<Tuple<Guid, Poco>> readObjects, Guid id, Stream stream)
                : base(stream)
            {
                this.readObjects = readObjects;
                this.id = id;
                BeginRead();
            }

            protected override void Deliver(Poco poco)
            {
                readObjects.Add(Tuple.Create(id, poco));
            }

            protected override Poco MakeConnectionLostPoco()
            {
                return new ClientConnectionLost();
            }
        }

        private SortedDictionary<Guid, PocoServerConnection> connections;
        private BlockingCollection<Tuple<Guid, Poco>> readObjects;
        private object connectionLock = new object();
        private Boolean isOpen = true;

        public PocoServerEndpoint()
        {
            connections = new SortedDictionary<Guid, PocoServerConnection>();
            readObjects = new BlockingCollection<Tuple<Guid, Poco>>();
        }

        public void AddConnection(Guid id, Stream stream)
        {
            lock (connectionLock)
            {
                if (isOpen)
                {
                    connections.Add(id, new PocoServerConnection(readObjects, id, stream));
                    readObjects.Add(Tuple.Create(id, (Poco)new ClientConnectionAdded()));
                }
                else
                {
                    stream.Close();
                }
            }
        }

        public void Send(Poco poco, IEnumerable<Guid> recipients)
        {
            lock (connectionLock)
            {
                foreach (var recipient in recipients)
                    connections[recipient].Send(poco);
            }
        }

        public void Send(Poco poco, params Guid[] recipients)
        {
            Send(poco, (IEnumerable<Guid>)recipients);
        }

        public bool HasData
        {
            get { return readObjects.Count > 0; }
        }

        public Tuple<Guid, Poco> Receive()
        {
            var result = readObjects.Take();

            if (result.Item2 == null)
            {
                lock (connectionLock)
                {
                    connections.Remove(result.Item1);
                }
            }

            return result;
        }

        public void Disconnect(Guid id)
        {
            connections[id].Close();
        }

        public void DisconnectAll()
        {
            lock (connectionLock)
            {
                foreach (var connection in connections)
                    connection.Value.Close();
            }
        }

        public void Close()
        {
            lock (connectionLock)
            {
                readObjects.Add(Tuple.Create(Guid.Empty, (Poco)new ShutdownRequested()));
                foreach (var connection in connections)
                    connection.Value.Close();
                isOpen = false;
            }
        }
    }


    public interface IServer
    {
        void Start();
        void RequestShutdown();
        void WaitUntilServerShutdownComplete();
    }

    public interface IServerLogicFactory
    {
        IServerLogic Create(IServerLogicCallbacks callbacks);
    }

    public interface IServerLogic
    {
        void Process(Guid sender, Poco poco);
        void Shutdown();
    }

    public interface IServerLogicCallbacks
    {
        void Send(Poco poco, params Guid[] recipients);
        void RequestDisconnect(Guid connectionId);
        void RequestShutdown();
    }


    class Server : IServerLogicCallbacks, IServer
    {
        public static IServer Create(IPAddress address, int port, IServerLogicFactory factory)
        {
            return Create(address, port, factory, null);
        }

        public static IServer Create(IPAddress address, int port, IServerLogicFactory factory,
                Stream record)
        {
            return new Server(address, port, factory, record);
        }

        PocoServerEndpoint endpoint;
        IServerLogic logic;
        TcpListener listener;
        PocoServerRecordWriter writer;
        ManualResetEvent serverThreadFinished;

        private Server(IPAddress address, int port, IServerLogicFactory factory, Stream record)
        {
            endpoint = new PocoServerEndpoint();
            listener = new TcpListener(address, port);
            logic = factory.Create(this);
            this.writer = new PocoServerRecordWriter(record);
            serverThreadFinished = new ManualResetEvent(false);
        }

        public void WaitUntilServerShutdownComplete()
        {
            serverThreadFinished.WaitOne();
        }

        private void BeginAcceptTcpClientCallback(IAsyncResult asyncResult)
        {
            try
            {
                TcpClient client = listener.EndAcceptTcpClient(asyncResult);
                endpoint.AddConnection(Guid.NewGuid(), client.GetStream());
                listener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, null);
            }
            catch (Exception)
            {
            }
        }

        public void Start()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, null);
            new Thread(ServerThreadMain) { IsBackground = true }.Start();
        }

        void ServerThreadMain()
        {
            while (true)
            {
                var nextMessage = endpoint.Receive();
                writer.Write(nextMessage.Item1, nextMessage.Item2);
                if (nextMessage.Item2 is ShutdownRequested)
                {
                    logic.Shutdown();
                    writer.Dispose();
                    break;
                }
                else
                {
                    logic.Process(nextMessage.Item1, nextMessage.Item2);
                }
            }
            serverThreadFinished.Set();
        }

        public void RequestDisconnect(Guid connectionId)
        {
            endpoint.Disconnect(connectionId);
        }

        public void RequestShutdown()
        {
            listener.Stop();
            endpoint.Close();
        }

        public void Send(Poco poco, params Guid[] recipients)
        {
            endpoint.Send(poco, recipients);
        }
    }

    class PlaybackServer : IServerLogicCallbacks
    {
        PocoServerRecordReader reader;
        IServerLogic logic;

        public PlaybackServer(Stream source, IServerLogicFactory factory)
        {
            this.reader = new PocoServerRecordReader(source);
            logic = factory.Create(this);
        }

        public void Replay()
        {
            while (true)
            {
                var nextPoco = reader.Read();
                var sender = nextPoco.Item1;
                var poco = nextPoco.Item2;

                if (poco is ShutdownRequested)
                {
                    logic.Shutdown();
                    return;
                }
                else
                {
                    logic.Process(sender, poco);
                }
            }
        }

        public void Send(Poco poco, params Guid[] recipients)
        {
            // Ignore for now; maybe log later
        }

        public void RequestDisconnect(Guid connectionId)
        {
            // Ignore for now; maybe log later
        }

        public void RequestShutdown()
        {
            // Ignore for now; maybe log later
        }
    }

    public class PocoServerRecordWriter : IDisposable
    {
        Stream destination;

        public PocoServerRecordWriter(Stream destination)
        {
            this.destination = destination;
        }

        public void Dispose()
        {
            if (destination != null)
                destination.Dispose();
        }

        public void Write(Guid sender, Poco poco)
        {
            if (destination == null)
                return;

            destination.Write(sender.ToByteArray(), 0, 16);
            destination.WriteByte(poco is ServerControlPoco ?
                ServerControlPoco.TypeIdentifier : (byte)0x00);
            poco.SerializeWithId(destination);
        }

        public static void WriteRecord(IEnumerable<Tuple<Guid, Poco>> messages,
                Stream destination)
        {
            var writer = new PocoServerRecordWriter(destination);
            foreach (var message in messages)
                writer.Write(message.Item1, message.Item2);
        }
    }

    public class PocoServerRecordReader : IDisposable
    {
        Stream source;

        public PocoServerRecordReader(Stream source)
        {
            this.source = source;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        public Tuple<Guid, Poco> Read()
        {
            var buffer = new byte[16];
            var bytesRead = source.Read(buffer, 0, 16);
            if (bytesRead == 0)
                return Tuple.Create(Guid.Empty, (Poco)new ShutdownRequested());
            if (bytesRead != 16)
                throw new InvalidDataException(""Unexpected end-of-stream"");

            var pocoType = source.ReadByte();
            if (pocoType == -1)
                throw new InvalidDataException(""Unexpected end-of-stream"");

            Poco poco;
            if (pocoType == ServerControlPoco.TypeIdentifier)
                poco = ServerControlPoco.DeserializeControlPocoWithId(source);
            else
                poco = Poco.DeserializeWithId(source);

            return Tuple.Create(new Guid(buffer), poco);
        }

        public static List<Tuple<Guid, Poco>> ReadRecord(Stream source)
        {
            var reader = new PocoServerRecordReader(source);
            var result = new List<Tuple<Guid, Poco>>();
            while (true)
            {
                Tuple<Guid, Poco> nextMessage = reader.Read();
                result.Add(nextMessage);
                if (nextMessage.Item2 is ShutdownRequested)
                    break;
            }
            return result;
        }
    }");
        }
    }
}
