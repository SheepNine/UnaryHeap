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
                new[] { "_nsS_", "_nsG_", "_nsI_", "_nsCC_" });
            WriteNetworkingServerClasses(file);
            WriteNamespaceFooter(file);
        }

        static void WriteNetworkingServerClasses(TextWriter output)
        {
            output.EmitCode(
@"    class ClientConnectionLost : Poco, _nsS_.IEquatable<ClientConnectionLost>
    {
        protected override int Identifier => 1;

        public ClientConnectionLost()
        {
        }

        public override void Serialize(_nsI_.Stream output)
        {
        }

        public static ClientConnectionLost Deserialize(_nsI_.Stream input)
        {
            return new ClientConnectionLost();
        }

        public override string ToString()
        {
            return ""<DISCONNECTED>"";
        }

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

    class ClientConnectionAdded : Poco, _nsS_.IEquatable<ClientConnectionAdded>
    {
        protected override int Identifier => 2;

        public ClientConnectionAdded()
        {
        }

        public override void Serialize(_nsI_.Stream output)
        {
        }

        public static ClientConnectionAdded Deserialize(_nsI_.Stream input)
        {
            return new ClientConnectionAdded();
        }

        public override string ToString()
        {
            return ""<JOINED>"";
        }

        public bool Equals(ClientConnectionAdded other)
        {
            return other != null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EmptyPoco);
        }

        public override int GetHashCode()
        {
            return 42;
        }
    }

    class ShutdownRequested : Poco, _nsS_.IEquatable<ShutdownRequested>
    {
        protected override int Identifier => 3;

        public ShutdownRequested()
        {
        }

        public override void Serialize(_nsI_.Stream output)
        {
        }

        public static ShutdownRequested Deserialize(_nsI_.Stream input)
        {
            return new ShutdownRequested();
        }

        public override string ToString()
        {
            return ""<SHUTDOWN>"";
        }

        public bool Equals(ShutdownRequested other)
        {
            return other != null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EmptyPoco);
        }

        public override int GetHashCode()
        {
            return 42;
        }
    }

    public interface IPocoServerEndpoint : _nsS_.IDisposable
    {
        void Send(Poco poco, _nsG_.IEnumerable<_nsS_.Guid> recipients);
        void Send(Poco poco, params _nsS_.Guid[] recipients);

        bool HasData { get; }
        _nsS_.Tuple<_nsS_.Guid, Poco> Receive();

        void Disconnect(_nsS_.Guid id);
        void DisconnectAll();
    }

    public class PocoServerEndpoint : IPocoServerEndpoint
    {
        class PocoServerConnection : LengthPrefixedPocoStreamer
        {
            private _nsCC_.BlockingCollection<_nsS_.Tuple<_nsS_.Guid, Poco>> readObjects;
            _nsS_.Guid id;

            public PocoServerConnection(
                    _nsCC_.BlockingCollection<_nsS_.Tuple<_nsS_.Guid, Poco>> readObjects,
                    _nsS_.Guid id, _nsI_.Stream stream)
                : base(stream)
            {
                this.readObjects = readObjects;
                this.id = id;
                BeginRead();
            }

            protected override void Deliver(Poco poco)
            {
                readObjects.Add(_nsS_.Tuple.Create(id, poco));
            }

            protected override Poco MakeConnectionLostPoco()
            {
                return new ClientConnectionLost();
            }
        }

        private _nsG_.SortedDictionary<_nsS_.Guid, PocoServerConnection> connections;
        private _nsCC_.BlockingCollection<_nsS_.Tuple<_nsS_.Guid, Poco>> readObjects;
        private object connectionLock = new object();
        private bool isOpen = true;

        public PocoServerEndpoint()
        {
            connections = new _nsG_.SortedDictionary<_nsS_.Guid, PocoServerConnection>();
            readObjects = new _nsCC_.BlockingCollection<_nsS_.Tuple<_nsS_.Guid, Poco>>();
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
                    readObjects.Add(_nsS_.Tuple.Create(id, (Poco)new ClientConnectionAdded()));
                }
                else
                {
                    stream.Close();
                }
            }
        }

        public void Send(Poco poco, _nsG_.IEnumerable<_nsS_.Guid> recipients)
        {
            lock (connectionLock)
            {
                foreach (var recipient in recipients)
                    connections[recipient].Send(poco);
            }
        }

        public void Send(Poco poco, params _nsS_.Guid[] recipients)
        {
            Send(poco, (_nsG_.IEnumerable<_nsS_.Guid>)recipients);
        }

        public bool HasData
        {
            get { return readObjects.Count > 0; }
        }

        public _nsS_.Tuple<_nsS_.Guid, Poco> Receive()
        {
            var result = readObjects.Take();

            if (result.Item2 == null)
            {
                lock (connectionLock)
                {
                    connections.Remove(result.Item1);
                }
            }

            return result;
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
                    _nsS_.Guid.Empty, (Poco)new ShutdownRequested()));
                foreach (var connection in connections)
                    connection.Value.Close();
                isOpen = false;
            }
        }
    }");
        }
    }
}
