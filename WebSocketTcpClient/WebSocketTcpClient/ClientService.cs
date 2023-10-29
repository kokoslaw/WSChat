
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebSocketTcpClient
{
    public class ClientService
    {
        private readonly Socket client;
        private byte[] gBuffer = new byte[1024];

        public ClientService()
        {
            client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void ConnectTo(IPEndPoint endPoint)
        {
            client.Connect(endPoint);
            client.BeginReceive(gBuffer, 0, gBuffer.Length, SocketFlags.None, onMessageReceived, null);
        }

        private void onMessageReceived(IAsyncResult ar)
        {
            client.EndReceive(ar);

            int bytesReceived = client.EndReceive(ar);
            byte[] buffer = new byte[bytesReceived];
            Array.Copy(gBuffer, buffer, bytesReceived);
        }

        public void SendMessageToServer(string message)
        {
            byte[] buffer = new byte[1024];
            buffer = Encoding.ASCII.GetBytes(message);

            Console.WriteLine($"You sended Message: {message}\n");


            client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, onMessageSend, null);
        }

        private void onMessageSend(IAsyncResult ar)
        {
            client.EndSend(ar);
        }
    }
}
