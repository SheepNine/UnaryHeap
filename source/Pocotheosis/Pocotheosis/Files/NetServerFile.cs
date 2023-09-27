using System.IO;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteNetworkingServerFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using var file = File.CreateText(outputFileName);
            WriteNamespaceHeader(dataModel, file,
                new[] { "_nsS_", "_nsG_", "_nsI_", "_nsCC_", "_nsCDC_" });
            WriteNetworkingServerClasses(file);
            WriteNamespaceFooter(file);
        }

        static void WriteNetworkingServerClasses(TextWriter output)
        {
            output.EmitCode(
$"    [_nsCDC_.GeneratedCode(\"Pocotheosis\", \"{GeneratorVersion}\")]",
@"    public class ClientConnectionLost : Poco, _nsS_.IEquatable<ClientConnectionLost>
    {
        public bool Equals(ClientConnectionLost other)
        {
            return other != null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ClientConnectionLost);
        }

        public override int GetHashCode()
        {
            return 42;
        }
    }
",
$"    [_nsCDC_.GeneratedCode(\"Pocotheosis\", \"{GeneratorVersion}\")]",
@"    public class ClientConnectionAdded : Poco, _nsS_.IEquatable<ClientConnectionAdded>
    {
        public bool Equals(ClientConnectionAdded other)
        {
            return other != null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ClientConnectionAdded);
        }

        public override int GetHashCode()
        {
            return 42;
        }
    }
",
$"    [_nsCDC_.GeneratedCode(\"Pocotheosis\", \"{GeneratorVersion}\")]",
@"    public class ShutdownRequested : Poco, _nsS_.IEquatable<ShutdownRequested>
    {
        public bool Equals(ShutdownRequested other)
        {
            return other != null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ShutdownRequested);
        }

        public override int GetHashCode()
        {
            return 42;
        }
    }

    public interface IPocoServerEndpoint : _nsS_.IDisposable
    {
        void Send(ISerializablePoco poco, _nsG_.IEnumerable<_nsS_.Guid> recipients);
        void Send(ISerializablePoco poco, params _nsS_.Guid[] recipients);

        bool HasData { get; }
        _nsS_.Tuple<_nsS_.Guid, IPoco> Receive();

        void Disconnect(_nsS_.Guid id);
        void DisconnectAll();
    }
",
$"    [_nsCDC_.GeneratedCode(\"Pocotheosis\", \"{GeneratorVersion}\")]",
@"    public class PocoServerEndpoint : IPocoServerEndpoint
    {
        class PocoServerConnection : LengthPrefixedPocoStreamer
        {
            private _nsCC_.BlockingCollection<_nsS_.Tuple<_nsS_.Guid, IPoco>> readObjects;
            _nsS_.Guid id;

            public PocoServerConnection(
                    _nsCC_.BlockingCollection<_nsS_.Tuple<_nsS_.Guid, IPoco>> readObjects,
                    _nsS_.Guid id, _nsI_.Stream stream)
                : base(stream)
            {
                this.readObjects = readObjects;
                this.id = id;
                BeginRead();
            }

            protected override void Deliver(IPoco poco)
            {
                readObjects.Add(_nsS_.Tuple.Create(id, poco));
            }

            protected override IPoco MakeConnectionLostPoco()
            {
                return new ClientConnectionLost();
            }
        }

        private _nsG_.SortedDictionary<_nsS_.Guid, PocoServerConnection> connections;
        private _nsCC_.BlockingCollection<_nsS_.Tuple<_nsS_.Guid, IPoco>> readObjects;
        private object connectionLock = new object();
        private bool isOpen = true;

        public PocoServerEndpoint()
        {
            connections = new _nsG_.SortedDictionary<_nsS_.Guid, PocoServerConnection>();
            readObjects = new _nsCC_.BlockingCollection<_nsS_.Tuple<_nsS_.Guid, IPoco>>();
        }

        public void Dispose()
        {
            readObjects.Dispose();
            _nsS_.GC.SuppressFinalize(this);
        }

        public void AddConnection(_nsS_.Guid id, _nsI_.Stream stream)
        {
            lock (connectionLock)
            {
                if (isOpen)
                {
                    connections.Add(id, new PocoServerConnection(readObjects, id, stream));
                    readObjects.Add(_nsS_.Tuple.Create(id, (IPoco)new ClientConnectionAdded()));
                }
                else
                {
                    stream.Close();
                }
            }
        }

        public void Send(ISerializablePoco poco, _nsG_.IEnumerable<_nsS_.Guid> recipients)
        {
            lock (connectionLock)
            {
                foreach (var recipient in recipients)
                    connections[recipient].Send(poco);
            }
        }

        public void Send(ISerializablePoco poco, params _nsS_.Guid[] recipients)
        {
            Send(poco, (_nsG_.IEnumerable<_nsS_.Guid>)recipients);
        }

        public bool HasData
        {
            get { return readObjects.Count > 0; }
        }

        public _nsS_.Tuple<_nsS_.Guid, IPoco> Receive()
        {
            return readObjects.Take();
        }

        public void Disconnect(_nsS_.Guid id)
        {
            connections[id].Close();
        }

        public void DisconnectAll()
        {
            lock (connectionLock)
            {
                foreach (var connection in connections)
                    connection.Value.Close();
            }
        }

        public void Close()
        {
            lock (connectionLock)
            {
                readObjects.Add(_nsS_.Tuple.Create(
                    _nsS_.Guid.Empty, (IPoco)new ShutdownRequested()));
                foreach (var connection in connections)
                    connection.Value.Close();
                isOpen = false;
            }
        }
    }");
        }
    }
}
