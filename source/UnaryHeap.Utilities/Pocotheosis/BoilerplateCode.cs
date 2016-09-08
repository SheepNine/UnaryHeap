using System.IO;

namespace Pocotheosis
{
    static class BoilerplateCode
    {
        public static void WriteSerializationHelperClass(TextWriter output)
        {
            output.WriteLine(@"    static class SerializationHelpers
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
        }

        public static bool DeserializeBool(global::System.IO.Stream input)
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
        }

        public static string DeserializeString(global::System.IO.Stream input)
        {
            var bytes = new byte[DeserializeInt32(input)];
            foreach (var i in global::System.Linq.Enumerable.Range(0, bytes.Length))
                bytes[i] = DeserializeByte(input);
            return global::System.Text.Encoding.UTF8.GetString(bytes);
        }
    }");
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
        void Send(Poco poco);
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

        public void Send(Poco poco)
        {
            poco.SerializeWithId(destination);
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
    }");
        }

        public static void WriteNetworkingClientClasses(TextWriter output)
        {
            output.WriteLine(@"
    public interface IPocoEndpoint : IPocoSource, IPocoSink, global::System.IDisposable { }

    public class PocoClientEndpoint : IPocoEndpoint
    {
        global::System.IO.Stream stream;
        bool streamOkay;
        PocoReader reader;
        PocoWriter writer;
        global::System.Collections.Concurrent.BlockingCollection<Poco> queue;

        public PocoClientEndpoint(global::System.IO.Stream stream)
        {
            this.stream = stream;
            streamOkay = true;
            reader = new PocoReader(stream);
            writer = new PocoWriter(stream);
            queue = new global::System.Collections.Concurrent.BlockingCollection<Poco>();
            new global::System.Threading.Thread(ReaderThread) { IsBackground = true }.Start();
        }

        void ReaderThread()
        {
            while (true)
            {
                try
                {
                    var poco = reader.Receive();
                    if (poco != null)
                    {
                        queue.Add(poco);
                    }
                    else
                    {
                        Dispose();
                        return;
                    }
                }
                catch (global::System.Exception)
                {
                    Dispose();
                    return;
                }
            }
        }

        public void Dispose()
        {
            lock (stream)
            {
                stream.Close();
                if (streamOkay)
                {
                    streamOkay = false;
                    queue.Add(null);
                }
            }
        }

        public Poco Receive()
        {
            var result = queue.Take();
            if (result == null)
                queue.Add(null);
            return result;
        }

        public void Send(Poco poco)
        {
            if (!streamOkay) return;

            try
            {
                writer.Send(poco);
            }
            catch (global::System.Exception)
            {
                Dispose();
            }
        }
    }");
        }
    }
}
