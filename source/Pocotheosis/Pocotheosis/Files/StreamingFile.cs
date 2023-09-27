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
                new[] { "_nsS_", "_nsI_", "_nsCDC_" });
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
    }"
            );
        }
    }
}
