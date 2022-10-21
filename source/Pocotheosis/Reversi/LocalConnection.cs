using System;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Reversi
{
    static class LocalConnection
    {
        public static void CreateNamedPipe(out PipeStream streamA, out PipeStream streamB)
        {
            string pipeName = Guid.NewGuid().ToString();

            var serverStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1,
                PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            var asyncResult = serverStream.BeginWaitForConnection(WaitForConnectionCallback,
                serverStream);

            var clientStream = new NamedPipeClientStream(pipeName);
            clientStream.Connect();

            asyncResult.AsyncWaitHandle.WaitOne();

            streamA = serverStream;
            streamB = clientStream;
        }

        static void WaitForConnectionCallback(IAsyncResult result)
        {
            (result.AsyncState as NamedPipeServerStream).EndWaitForConnection(result);
        }

        public static void CreateTcpLoopback(out NetworkStream streamA, out NetworkStream streamB)
        {
            var helper = new TcpLoopbackHelper();
            helper.Accept();

            var clientA = new TcpClient();
            clientA.Connect(IPAddress.Loopback, TcpLoopbackHelper.Port);

            var clientB = helper.GetClient();

            streamA = clientA.GetStream();
            streamB = clientB.GetStream();
        }

        class TcpLoopbackHelper
        {
            public const int Port = 28446;

            TcpListener listener;
            TcpClient client;
            IAsyncResult result;
            ManualResetEvent signal;

            public TcpLoopbackHelper()
            {
                this.listener = new TcpListener(IPAddress.Loopback, Port);
                signal = new ManualResetEvent(false);
            }

            public void Accept()
            {
                listener.Start();
                result = listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
            }

            void AcceptTcpClientCallback(IAsyncResult result)
            {
                client = listener.EndAcceptTcpClient(result);
                listener.Stop();
                signal.Set();
            }

            public TcpClient GetClient()
            {
                signal.WaitOne();
                return client;
            }
        }
    }
}
