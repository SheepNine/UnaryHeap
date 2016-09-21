using Reversi.Generated;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Reversi
{
    public interface IServerLogic
    {
        void Process(Guid sender, Poco poco);
        void Shutdown();
    }

    public interface IServerLogicFactory
    {
        IServerLogic Create(IServerLogicCallbacks callbacks);
    }

    public interface IServerLogicCallbacks
    {
        void Send(Poco poco, params Guid[] recipients);
    }


    class Server : IServerLogicCallbacks
    {
        PocoServerEndpoint endpoint;
        IServerLogic logic;
        TcpListener listener;

        public Server(IPAddress address, int port, IServerLogicFactory factory)
        {
            endpoint = new PocoServerEndpoint();
            listener = new TcpListener(address, port);
            logic = factory.Create(this);
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
                if (nextMessage.Item2 != null)
                {
                    logic.Process(nextMessage.Item1, nextMessage.Item2);
                }
                else
                {
                    logic.Shutdown();
                    return;
                }
            }
        }

        public void Stop()
        {
            endpoint.Close();
            listener.Stop();
            endpoint.DisconnectAll();
        }

        public void Send(Poco poco, params Guid[] recipients)
        {
            endpoint.Send(poco, recipients);
        }
    }
}
