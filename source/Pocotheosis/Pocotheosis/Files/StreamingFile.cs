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
                new[] { "_nsS_", "_nsI_" });
            WriteStreamingCommonClasses(dataModel, file);
            WriteNamespaceFooter(file);
        }

        static void WriteStreamingCommonClasses(PocoNamespace dataModel, TextWriter output)
        {
            output.EmitCode(
@"    public interface IPocoSource : _nsS_.IDisposable
    {
        Poco Receive();
    }

    public class PocoReader : IPocoSource
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

        public Poco Receive()
        {
            return Poco.DeserializeWithId<Poco>(source);
        }
    }

    public interface IPocoSink : _nsS_.IDisposable
    {
        IPocoSink Send(Poco poco);
        IPocoSink Flush();
    }

    public class PocoWriter : IPocoSink
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
    }"
            );
        }
    }
}
