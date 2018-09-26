using System.IO;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteNetworkingClientFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                dataModel.WriteNamespaceHeader(file);
                WriteNetworkingClientClasses(file);
                dataModel.WriteNamespaceFooter(file);
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

        protected override int getIdentifier()
        {
            return Identifier;
        }

        public override string ToString()
        {
            return ""<DISCONNECTED>"";
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
