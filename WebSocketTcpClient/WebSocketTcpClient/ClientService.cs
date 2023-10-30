using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebSocketTcpClient
{
    public class ClientService  // message format: /msg UserName: Message
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
            try
            {
                client.EndReceive(ar);

                int bytesReceived = client.EndReceive(ar);
                byte[] buffer = new byte[bytesReceived];
                Array.Copy(gBuffer, buffer, bytesReceived);

                string msg = Encoding.UTF8.GetString(buffer);
                Console.WriteLine(msg);

                client.BeginReceive(gBuffer, 0, gBuffer.Length, SocketFlags.None, onMessageReceived, null);
            }
            catch (SocketException)
            {
                Console.WriteLine("Connection Lose");
                client.Close();
            }
        }

        public void SendMessageToServer(string message)
        {
            byte[] buffer = new byte[1024];
            buffer = Encoding.ASCII.GetBytes(message);

            client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, onMessageSend, null);
        }

        private void onMessageSend(IAsyncResult ar)
        {
            client.EndSend(ar);
        }
    }
}
