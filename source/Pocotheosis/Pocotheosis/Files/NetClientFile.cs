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
                new[] { "_nsS_", "_nsI_", "_nsCC_", "_nsTh_", "_nsCDC_" });
            WriteNetworkingClientClasses(file);
            WriteNamespaceFooter(file);
        }

        static void WriteNetworkingClientClasses(TextWriter output)
        {
            output.EmitCode(
$"    [_nsCDC_.GeneratedCode(\"Pocotheosis\", \"{GeneratorVersion}\")]",
@"    public class ServerConnectionLost : Poco, _nsS_.IEquatable<ServerConnectionLost>
    {
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
",
$"    [_nsCDC_.GeneratedCode(\"Pocotheosis\", \"{GeneratorVersion}\")]",
@"    public class PocoClientEndpoint : LengthPrefixedPocoStreamer
    {
        private _nsS_.EventHandler receiveHandler;
        private _nsCC_.BlockingCollection<IPoco> readObjects;

        public PocoClientEndpoint(_nsI_.Stream stream) : this(stream, null)
        {
        }

        public PocoClientEndpoint(_nsI_.Stream stream,
            _nsS_.EventHandler receiveHandler) : base(stream)
        {
            this.receiveHandler = receiveHandler ?? ((sender, e) => { });
            readObjects = new _nsCC_.BlockingCollection<IPoco>();
            BeginRead();
        }

        protected override void Deliver(IPoco poco)
        {
            readObjects.Add(poco);
            receiveHandler(this, _nsS_.EventArgs.Empty);
        }

        public bool HasData
        {
            get { return readObjects.Count > 0; }
        }

        public IPoco Receive()
        {
            return readObjects.Take();
        }

        protected override IPoco MakeConnectionLostPoco()
        {
            return new ServerConnectionLost();
        }
    }"
            );
        }
    }
}
