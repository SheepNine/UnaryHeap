using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Pocotheosis.Tests
{
    class LoopbackBuilder : IDisposable
    {
        public Stream Server { get; private set; }
        public Stream Client { get; private set; }
        readonly ManualResetEvent mre = new(false);

        public LoopbackBuilder(string pipeName)
        {
            var server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1,
                PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            server.BeginWaitForConnection(Callback, server);
            var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut,
                PipeOptions.Asynchronous);
            client.Connect();
            Client = client;
            mre.WaitOne();
        }

        public void Dispose()
        {
            mre.Dispose();
        }

        private void Callback(IAsyncResult ar)
        {
            var server = ar.AsyncState as NamedPipeServerStream;
            server.EndWaitForConnection(ar);
            Server = server;
            mre.Set();
        }
    }

    static class ServerConnector
    {
        public static PocoClientEndpoint AddClient(this PocoServerEndpoint server, Guid id)
        {
            using var loopback = new LoopbackBuilder(id.ToString());
            server.AddConnection(id, loopback.Server);
            return new PocoClientEndpoint(loopback.Client);
        }

        public static void ShouldHaveReceived(this PocoServerEndpoint server, Guid id, Poco poco)
        {
            var package = server.Receive();
            Assert.AreEqual(id, package.Item1);
            Assert.AreEqual(poco, package.Item2);
        }

        public static void ShouldHaveReceived(this PocoClientEndpoint client, Poco poco)
        {
            Assert.AreEqual(poco, client.Receive());
        }
    }

    public class ClientServerTests
    {
        [Test]
        public void TestServer()
        {
            var id1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var id2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var id3 = Guid.Parse("33333333-3333-3333-3333-333333333333");
       
            var server = new PocoServerEndpoint();
       
            var client1 = server.AddClient(id1);
            server.ShouldHaveReceived(id1, new ClientConnectionAdded());

            var client2 = server.AddClient(id2);
            server.ShouldHaveReceived(id2, new ClientConnectionAdded());

            client1.Send(new PrimitiveValue(150));
            server.ShouldHaveReceived(id1, new PrimitiveValue(150));

            var client3 = server.AddClient(id3);
            server.ShouldHaveReceived(id3, new ClientConnectionAdded());

            client2.Close();
            server.ShouldHaveReceived(id2, new ClientConnectionLost());
       
            server.Send(new PrimitiveValue(100), id1);
            client1.ShouldHaveReceived(new PrimitiveValue(100));

            server.Send(new PrimitiveValue(30), id3);
            client3.ShouldHaveReceived(new PrimitiveValue(30));

            server.Close();
            client1.ShouldHaveReceived(new ServerConnectionLost());
            client3.ShouldHaveReceived(new ServerConnectionLost());
       
            client1.Close();
            client3.Close();
        }
    }
}
