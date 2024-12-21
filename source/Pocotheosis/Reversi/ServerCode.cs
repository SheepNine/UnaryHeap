using Reversi.Generated;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Reversi
{
    public interface IServer : IDisposable
    {
        void Start();
        void RequestShutdown();
        void WaitUntilServerShutdownComplete();
    }

    public interface IServerLogicFactory
    {
        IServerLogic Create(IServerLogicCallbacks callbacks);
    }

    public interface IServerLogic
    {
        void Process(Guid sender, IPoco poco);
        void Shutdown();
    }

    public interface IServerLogicCallbacks
    {
        void Send(ISerializablePoco poco, params Guid[] recipients);
        void RequestDisconnect(Guid connectionId);
        void RequestShutdown();
    }


    sealed class Server : IServerLogicCallbacks, IServer
    {
        public static IServer Create(IPAddress address, int port, IServerLogicFactory factory)
        {
            return new Server(address, port, factory);
        }

        PocoServerEndpoint endpoint;
        IServerLogic logic;
        TcpListener listener;
        ManualResetEvent serverThreadFinished;

        private Server(IPAddress address, int port, IServerLogicFactory factory)
        {
            endpoint = new PocoServerEndpoint();
            listener = new TcpListener(address, port);
            logic = factory.Create(this);
            serverThreadFinished = new ManualResetEvent(false);
        }

        public void Dispose()
        {
            serverThreadFinished.Dispose();
            GC.SuppressFinalize(this);
        }

        public void WaitUntilServerShutdownComplete()
        {
            serverThreadFinished.WaitOne();
        }

        private void BeginAcceptTcpClientCallback(IAsyncResult asyncResult)
        {
            try
            {
                TcpClient client = listener.EndAcceptTcpClient(asyncResult);
                endpoint.AddConnection(Guid.NewGuid(), client.GetStream());
                listener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, null);
            }
            catch (Exception)
            {
            }
        }

        public void Start()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, null);
            new Thread(ServerThreadMain) { IsBackground = true }.Start();
        }

        void ServerThreadMain()
        {
            while (true)
            {
                var nextMessage = endpoint.Receive();
                if (nextMessage.Item2 is ShutdownRequested)
                {
                    logic.Shutdown();
                    break;
                }
                else
                {
                    logic.Process(nextMessage.Item1, nextMessage.Item2);
                }
            }
            serverThreadFinished.Set();
        }

        public void RequestDisconnect(Guid connectionId)
        {
            endpoint.Disconnect(connectionId);
        }

        public void RequestShutdown()
        {
            listener.Stop();
            endpoint.Close();
        }

        public void Send(ISerializablePoco poco, params Guid[] recipients)
        {
            endpoint.Send(poco, recipients);
        }
    }
}
