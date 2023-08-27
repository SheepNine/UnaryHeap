using System.IO;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteStreamingFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using (var file = File.CreateText(outputFileName))
            {
                WriteNamespaceHeader(dataModel, file);
                WriteStreamingCommonClasses(file);
                WriteStreamingBaseClass(file, dataModel);
                foreach (var pocoClass in dataModel.Classes)
                    WriteClassStreamingImplementation(pocoClass, file);
                WriteNamespaceFooter(file);
            }
        }

        static void WriteStreamingCommonClasses(TextWriter output)
        {
            output.EmitCode(
@"    public interface IPocoSource : global::System.IDisposable
    {
        Poco Receive();
    }

    public class PocoReader : IPocoSource
    {
        global::System.IO.Stream source;

        public PocoReader(global::System.IO.Stream source)
        {
            this.source = source;
        }

        public void Dispose()
        {
            source.Dispose();
            global::System.GC.SuppressFinalize(this);
        }

        public Poco Receive()
        {
            return Poco.DeserializeWithId<Poco>(source);
        }
    }

    public interface IPocoSink : global::System.IDisposable
    {
        IPocoSink Send(Poco poco);
        IPocoSink Flush();
    }

    public class PocoWriter : IPocoSink
    {
        global::System.IO.Stream destination;

        public PocoWriter(global::System.IO.Stream destination)
        {
            this.destination = destination;
        }

        public void Dispose()
        {
            destination.Dispose();
            global::System.GC.SuppressFinalize(this);
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

        static void WriteStreamingBaseClass(TextWriter output, PocoNamespace dataModel)
        {
            output.EmitCode(
@"
    public partial class Poco
    {
        protected abstract int GetIdentifier();

        public void SerializeWithId(_nsI_.Stream output)
        {
            SerializationHelpers.Serialize(GetIdentifier(), output);
            Serialize(output);
        }

        public static T DeserializeWithId<T>(_nsI_.Stream input) where T: Poco
        {
            var id = SerializationHelpers.DeserializePocoIdentifier(input);
            if (id == null) throw new _nsI_.InvalidDataException(""Unexpected end of stream"");
            if (id == -1) return null;

            Poco result;
            switch (id)
            {"
            );
            foreach (var pocoClass in dataModel.Classes) output.EmitCode(
$"                case {pocoClass.Name}.Identifier:",
$"                    result = {pocoClass.Name}.Deserialize(input);",
$"                    break;"
            );
            output.EmitCode(
@"            default:
                throw new _nsI_.InvalidDataException();
            }

            if (result is not T)
                throw new _nsI_.InvalidDataException(""Unexpected POCO type found in stream"");

            return result as T;
        }
    }"
            );
        }

        static void WriteClassStreamingImplementation(PocoClass clasz, StreamWriter output)
        {
            output.EmitCode(
$"",
$"    public partial class {clasz.Name}",
$"    {{",
$"        public const int Identifier = {clasz.StreamingId};",
$"",
$"        protected override int GetIdentifier()",
$"        {{",
$"            return Identifier;",
$"        }}",
$"    }}"
            );
        }
    }
}
