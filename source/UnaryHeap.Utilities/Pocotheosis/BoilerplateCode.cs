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
            output.WriteLine(@"    class ConnectionLost : Poco
    {
        public ConnectionLost()
        {
        }

        public override void Serialize(global::System.IO.Stream output)
        {
            throw new global::System.InvalidOperationException();
        }

        protected override int getIdentifier()
        {
            throw new global::System.InvalidOperationException();
        }

        public override string ToString()
        {
            return ""<DISCONNECTED>"";
        }
    }

    public class PocoClientEndpoint : LengthPrefixedPocoStreamer, IPocoSource
    {
        private global::System.Collections.Concurrent.BlockingCollection<Poco> readObjects;

        public PocoClientEndpoint(global::System.IO.Stream stream) : base(stream)
        {
            readObjects = new global::System.Collections.Concurrent.BlockingCollection<Poco>();
        }

        protected override void Deliver(Poco poco)
        {
            readObjects.Add(poco);
        }

        public bool HasData
        {
            get { return readObjects.Count > 0; }
        }

        public Poco Receive()
        {
            return readObjects.Take();
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
            BeginRead();
            new global::System.Threading.Thread(WriterMain) { IsBackground = true }.Start();
        }

        private void BeginRead()
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
                    Deliver(new ConnectionLost());
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
                Deliver(new ConnectionLost());
            }
        }

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

        public void Send(Poco poco)
        {
            writeObjects.Add(poco);
        }
    }");
        }

        public static void WriteNetworkingServerClasses(TextWriter output)
        {
            output.WriteLine(@"    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;

    class ConnectionAdded : Poco
    {
        public ConnectionAdded()
        {
        }

        public override void Serialize(Stream output)
        {
            throw new InvalidOperationException();
        }

        protected override int getIdentifier()
        {
            throw new InvalidOperationException();
        }

        public override string ToString()
        {
            return ""<JOINED>"";
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
            }

            protected override void Deliver(Poco poco)
            {
                readObjects.Add(Tuple.Create(id, poco));
            }
        }

        private SortedDictionary<Guid, PocoServerConnection> connections;
        private BlockingCollection<Tuple<Guid, Poco>> readObjects;
        private object connectionLock = new object();

        public PocoServerEndpoint()
        {
            connections = new SortedDictionary<Guid, PocoServerConnection>();
            readObjects = new BlockingCollection<Tuple<Guid, Poco>>();
        }

        public void AddConnection(Guid id, Stream stream)
        {
            lock (connectionLock)
            {
                connections.Add(id, new PocoServerConnection(readObjects, id, stream));
            }
            readObjects.Add(Tuple.Create(id, (Poco)new ConnectionAdded()));
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
                {
                    connection.Value.Close();
                }
            }
        }
    }");
        }
    }
}
