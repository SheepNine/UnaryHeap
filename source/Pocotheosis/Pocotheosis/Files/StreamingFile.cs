using System.IO;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteStreamingFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using var file = File.CreateText(outputFileName);
            WriteNamespaceHeader(dataModel, file,
                new[] { "_nsS_", "_nsI_", "_nsTh_", "_nsCDC_", "_nsCC_" });
            WriteStreamingCommonClasses(dataModel, file);
            WriteNamespaceFooter(file);
        }

        static void WriteStreamingCommonClasses(PocoNamespace dataModel, TextWriter output)
        {
            output.EmitCode(
@"    public interface IPocoSource : _nsS_.IDisposable
    {
        IPoco Receive();
    }
",
$"    [_nsCDC_.GeneratedCode(\"Pocotheosis\", \"{GeneratorVersion}\")]",
@"    public class PocoReader : IPocoSource
    {
        _nsI_.Stream source;

        public PocoReader(_nsI_.Stream source)
        {
            this.source = source;
        }

        public void Dispose()
        {
            source.Dispose();
            _nsS_.GC.SuppressFinalize(this);
        }

        public IPoco Receive()
        {
            return Poco.DeserializeWithId(source);
        }
    }

    public interface IPocoSink : _nsS_.IDisposable
    {
        IPocoSink Send(ISerializablePoco poco);
        IPocoSink Flush();
    }
",
$"    [_nsCDC_.GeneratedCode(\"Pocotheosis\", \"{GeneratorVersion}\")]",
@"    public class PocoWriter : IPocoSink
    {
        _nsI_.Stream destination;

        public PocoWriter(_nsI_.Stream destination)
        {
            this.destination = destination;
        }

        public void Dispose()
        {
            destination.Dispose();
            _nsS_.GC.SuppressFinalize(this);
        }

        public IPocoSink Send(ISerializablePoco poco)
        {
            poco.SerializeWithId(destination);
            return this;
        }

        public IPocoSink Flush()
        {
            destination.Flush();
            return this;
        }
    }
",
$"    [_nsCDC_.GeneratedCode(\"Pocotheosis\", \"{GeneratorVersion}\")]",
@"    public abstract class LengthPrefixedPocoStreamer : _nsS_.IDisposable
    {
        const int BUFFER_SIZE = 2 + 0xFFFF;

        byte[] readBuffer = new byte[BUFFER_SIZE];
        byte[] writeBuffer = new byte[BUFFER_SIZE];
        int validBytes;
        _nsI_.Stream stream;
        bool isClosed;
        _nsCC_.BlockingCollection<ISerializablePoco> writeObjects
                = new _nsCC_.BlockingCollection<ISerializablePoco>();

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

        protected abstract IPoco MakeConnectionLostPoco();

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
                    Deliver(Poco.DeserializeWithId(tempStream));

                var validDataOffset = 2 + frameDataSize;
                var remainingBytes = validBytes - validDataOffset;

                for (int i = 0; i < remainingBytes; i++)
                    readBuffer[i] = readBuffer[i + validDataOffset];

                validBytes = remainingBytes;
            }
        }

        protected abstract void Deliver(IPoco poco);

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

        public LengthPrefixedPocoStreamer Send(ISerializablePoco poco)
        {
            writeObjects.Add(poco);
            return this;
        }
    }
"
            );
        }
    }
}
