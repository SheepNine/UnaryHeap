using System.Globalization;
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
                file.WriteLine();
                WriteStreamingBaseClass(file, dataModel);

                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    WriteClassStreamingImplementation(pocoClass, file);
                }

                WriteNamespaceFooter(file);
            }
        }

        static void WriteClassStreamingImplementation(PocoClass clasz, StreamWriter output)
        {
            output.Write("\tpublic partial class ");
            output.WriteLine(clasz.Name);
            output.WriteLine("\t{");
            output.Write("\t\tpublic const int Identifier = ");
            output.Write(clasz.StreamingId.ToString(CultureInfo.InvariantCulture));
            output.WriteLine(";");
            output.WriteLine();
            output.WriteLine("\t\tprotected override int getIdentifier()");
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\treturn Identifier;");
            output.WriteLine("\t\t}");
            output.WriteLine("\t}");
        }

        static void WriteStreamingCommonClasses(TextWriter output)
        {
            output.WriteLine(@"    public interface IPocoSource : global::System.IDisposable
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
            return Poco.DeserializeWithId(source);
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
            if (id == null) throw new global::System.IO.InvalidDataException(
                    ""Unexpected end of stream"");
            if (id == -1) return null;

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
    }
}
