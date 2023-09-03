using System.IO;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteNetworkingClientFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using var file = File.CreateText(outputFileName);
            WriteNamespaceHeader(dataModel, file,
                new[] { "_nsS_", "_nsI_", "_nsCC_", "_nsTh_" });
            WriteNetworkingClientClasses(file);
            WriteNamespaceFooter(file);
        }

        static void WriteNetworkingClientClasses(TextWriter output)
        {
            output.EmitCode(
@"    class ServerConnectionLost : Poco, _nsS_.IEquatable<ServerConnectionLost>
    {
        protected override int Identifier => 1;

        public ServerConnectionLost()
        {
        }

        public override void Serialize(_nsI_.Stream output)
        {
        }

        public static ServerConnectionLost Deserialize(_nsI_.Stream input)
        {
            return new ServerConnectionLost();
        }

        public override string ToString()
        {
            return ""<DISCONNECTED>"";
        }

        public bool Equals(ServerConnectionLost other)
        {
            return other != null;
        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as ServerConnectionLost);
        }

        public override int GetHashCode()
        {
            return 42;
        }
    }

    public abstract class LengthPrefixedPocoStreamer : IPocoSink
    {
        const int BUFFER_SIZE = 2 + 0xFFFF;

        byte[] readBuffer = new byte[BUFFER_SIZE];
        byte[] writeBuffer = new byte[BUFFER_SIZE];
        int validBytes;
        _nsI_.Stream stream;
        bool isClosed;
        _nsCC_.BlockingCollection<Poco> writeObjects = new _nsCC_.BlockingCollection<Poco>();

        public LengthPrefixedPocoStreamer(_nsI_.Stream stream)
        {
            this.stream = stream;
            new _nsTh_.Thread(WriterMain) { IsBackground = true }.Start();
        }

        public void Dispose()
        {
            writeObjects.Dispose();
            _nsS_.GC.SuppressFinalize(this);
        }

        protected void BeginRead()
        {
            stream.BeginRead(readBuffer, validBytes,
                BUFFER_SIZE - validBytes, ReaderMain, null);
        }

        private void ReaderMain(_nsS_.IAsyncResult result)
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
            catch (_nsS_.Exception)
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

                using (var tempStream = new _nsI_.MemoryStream(
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

                using (var tempStream = new _nsI_.MemoryStream(
                        writeBuffer, 2, BUFFER_SIZE - 2))
                {
                    while (true)
                    {
                        tempStream.Seek(0, _nsI_.SeekOrigin.Begin);
                        writeObjects.Take().SerializeWithId(tempStream);
                        var frameSize = tempStream.Position;
                        writeBuffer[0] = (byte)(frameSize & 0xFF);
                        writeBuffer[1] = (byte)((frameSize >> 8) & 0xFF);
                        stream.Write(writeBuffer, 0, (int)(frameSize + 2));
                    }
                }
            }
            catch (_nsS_.Exception)
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
        private _nsS_.EventHandler receiveHandler;
        private _nsCC_.BlockingCollection<Poco> readObjects;

        public PocoClientEndpoint(_nsI_.Stream stream) : this(stream, null)
        {
        }

        public PocoClientEndpoint(_nsI_.Stream stream,
            _nsS_.EventHandler receiveHandler) : base(stream)
        {
            this.receiveHandler = receiveHandler ?? ((sender, e) => { });
            readObjects = new _nsCC_.BlockingCollection<Poco>();
            BeginRead();
        }

        protected override void Deliver(Poco poco)
        {
            readObjects.Add(poco);
            receiveHandler(this, _nsS_.EventArgs.Empty);
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
    }"
            );
        }
    }
}
