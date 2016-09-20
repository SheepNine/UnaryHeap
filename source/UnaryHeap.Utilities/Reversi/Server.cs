using Reversi.Generated;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Reversi
{
    class Server
    {
        PocoServerEndpoint endpoint;
        ServerLogic logic;
        TcpListener listener;

        public Server(IPAddress address, int port)
        {
            endpoint = new PocoServerEndpoint();
            logic = new ServerLogic();
            listener = new TcpListener(address, port);
            listener.Start();
            listener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, null);
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
            new Thread(ServerThreadMain) { IsBackground = true }.Start();
        }

        void ServerThreadMain()
        {
            while (true)
            {
                var nextMessage = endpoint.Receive();
                if (nextMessage.Item2 == null)
                    return;
                logic.Process(nextMessage.Item1, nextMessage.Item2,
                    (poco, guid) => endpoint.Send(poco, guid));
            }
        }

        public void Stop()
        {
            listener.Stop();
            endpoint.DisconnectAll();
            endpoint.Close();
        }
    }
}
