using System.IO;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteNetworkingClientFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using (var file = File.CreateText(outputFileName))
            {
                WriteNamespaceHeader(dataModel, file);
                WriteNetworkingClientClasses(file);
                WriteNamespaceFooter(file);
            }
        }

        static void WriteNetworkingClientClasses(TextWriter output)
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

        protected override int GetIdentifier()
        {
            return Identifier;
        }

        public override string ToString()
        {
            return ""<DISCONNECTED>"";
        }
    }

    public abstract class LengthPrefixedPocoStreamer : IPocoSink
    {
        const int BUFFER_SIZE = 2 + 0xFFFF;

        byte[] readBuffer = new byte[BUFFER_SIZE];
        byte[] writeBuffer = new byte[BUFFER_SIZE];
        int validBytes;
        global::System.IO.Stream stream;
        bool isClosed;
        global::System.Collections.Concurrent.BlockingCollection<Poco> writeObjects =
            new global::System.Collections.Concurrent.BlockingCollection<Poco>();

        public LengthPrefixedPocoStreamer(global::System.IO.Stream stream)
        {
            this.stream = stream;
            new global::System.Threading.Thread(WriterMain) { IsBackground = true }.Start();
        }

        public void Dispose()
        {
            writeObjects.Dispose();
            global::System.GC.SuppressFinalize(this);
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
                    Deliver(Poco.DeserializeWithId<Poco>(tempStream));

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
    }
}
