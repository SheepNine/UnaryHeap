using System.IO;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteNetworkingServerFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using (var file = File.CreateText(outputFileName))
            {
                WriteNamespaceHeader(dataModel, file);
                WriteNetworkingServerClasses(file);
                WriteNamespaceFooter(file);
            }
        }

        static void WriteNetworkingServerClasses(TextWriter output)
        {
            output.WriteLine(@"    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;

    abstract partial class ServerControlPoco : Poco
    {
        public const byte TypeIdentifier = 0xff;

        public static Poco DeserializeControlPocoWithId(Stream input)
        {
            var id = SerializationHelpers.DeserializePocoIdentifier(input);
            if (id == null) return null;

            switch (id)
            {
                case ClientConnectionAdded.Identifier:
                    return ClientConnectionAdded.Deserialize(input);
                case ClientConnectionLost.Identifier:
                    return ClientConnectionLost.Deserialize(input);
                case ShutdownRequested.Identifier:
                    return ShutdownRequested.Deserialize(input);
                default:
                    throw new InvalidDataException();
            }
        }
    }

    class ClientConnectionLost : ServerControlPoco
    {
        public const int Identifier = 1;

        public ClientConnectionLost()
        {
        }

        public override void Serialize(global::System.IO.Stream output)
        {
        }

        public static ClientConnectionLost Deserialize(global::System.IO.Stream input)
        {
            return new ClientConnectionLost();
        }

        protected override int GetIdentifier()
        {
            return Identifier;
        }

        public override string ToString()
        {
            return ""<DISCONNECTED>"";
        }
    }

    class ClientConnectionAdded : ServerControlPoco
    {
        public const int Identifier = 2;

        public ClientConnectionAdded()
        {
        }

        public override void Serialize(Stream output)
        {
        }

        public static ClientConnectionAdded Deserialize(Stream input)
        {
            return new ClientConnectionAdded();
        }

        protected override int GetIdentifier()
        {
            return Identifier;
        }

        public override string ToString()
        {
            return ""<JOINED>"";
        }
    }

    class ShutdownRequested : ServerControlPoco
    {
        public const int Identifier = 3;

        public ShutdownRequested()
        {
        }

        public override void Serialize(Stream output)
        {
        }

        public static ShutdownRequested Deserialize(Stream input)
        {
            return new ShutdownRequested();
        }

        protected override int GetIdentifier()
        {
            return Identifier;
        }

        public override string ToString()
        {
            return ""<SHUTDOWN>"";
        }
    }

    public interface IPocoServerEndpoint : global::System.IDisposable
    {
        void Send(Poco poco, IEnumerable<Guid> recipients);
        void Send(Poco poco, params Guid[] recipients);

        bool HasData { get; }
        Tuple<Guid, Poco> Receive();

        void Disconnect(Guid id);
        void DisconnectAll();
    }

    public class PocoServerEndpoint : IPocoServerEndpoint
    {
        class PocoServerConnection : LengthPrefixedPocoStreamer
        {
            private BlockingCollection<Tuple<Guid, Poco>> readObjects;
            Guid id;

            public PocoServerConnection(
                BlockingCollection<Tuple<Guid, Poco>> readObjects, Guid id, Stream stream)
                : base(stream)
            {
                this.readObjects = readObjects;
                this.id = id;
                BeginRead();
            }

            protected override void Deliver(Poco poco)
            {
                readObjects.Add(Tuple.Create(id, poco));
            }

            protected override Poco MakeConnectionLostPoco()
            {
                return new ClientConnectionLost();
            }
        }

        private SortedDictionary<Guid, PocoServerConnection> connections;
        private BlockingCollection<Tuple<Guid, Poco>> readObjects;
        private object connectionLock = new object();
        private Boolean isOpen = true;

        public PocoServerEndpoint()
        {
            connections = new SortedDictionary<Guid, PocoServerConnection>();
            readObjects = new BlockingCollection<Tuple<Guid, Poco>>();
        }

        public void Dispose()
        {
            readObjects.Dispose();
            global::System.GC.SuppressFinalize(this);
        }

        public void AddConnection(Guid id, Stream stream)
        {
            lock (connectionLock)
            {
                if (isOpen)
                {
                    connections.Add(id, new PocoServerConnection(readObjects, id, stream));
                    readObjects.Add(Tuple.Create(id, (Poco)new ClientConnectionAdded()));
                }
                else
                {
                    stream.Close();
                }
            }
        }

        public void Send(Poco poco, IEnumerable<Guid> recipients)
        {
            lock (connectionLock)
            {
                foreach (var recipient in recipients)
                    connections[recipient].Send(poco);
            }
        }

        public void Send(Poco poco, params Guid[] recipients)
        {
            Send(poco, (IEnumerable<Guid>)recipients);
        }

        public bool HasData
        {
            get { return readObjects.Count > 0; }
        }

        public Tuple<Guid, Poco> Receive()
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

        public void Disconnect(Guid id)
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
                readObjects.Add(Tuple.Create(Guid.Empty, (Poco)new ShutdownRequested()));
                foreach (var connection in connections)
                    connection.Value.Close();
                isOpen = false;
            }
        }
    }");
        }
    }
}
