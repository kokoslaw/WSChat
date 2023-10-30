using System.Net;
using System.Net.Sockets;
using System.Text;


namespace WebSocketTcpServer
{
    public class ServerService
    {
       // TODO: Wysyłanie wiadomości między klientami

        public event Action<NamedClient, string> OnMessageReceivedEventChandler;

        public readonly Dictionary<int, NamedClient> connectedClients;
        private readonly Socket server;
        private byte[] gBuffer = new byte[1024];

        public ServerService()
        {
            connectedClients = new Dictionary<int, NamedClient>();
            server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void ServerStart() 
        {
            server.Bind(new IPEndPoint(IPAddress.Any, 100));
            server.Listen(10);
            server.BeginAccept(onClientAccept, null);
        }

        public void SendMessageToClient(Socket client, string message)
        {
            byte[] buffer = new byte[1024];
            buffer = Encoding.ASCII.GetBytes(message);

            client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, onMessageSend, client);
        }

        public void DisconnectClient(Socket client, string? message = null)
        {
            if (message != null)
                SendMessageToClient(client, message);

            client.Disconnect(false);
            client.Close();
        }

        // New client connected
        private void onClientAccept(IAsyncResult ar)
        {
            Socket newClient = server.EndAccept(ar);

            int clientIndex = FindFreeIndex();
            try
            {
                connectedClients.Add(clientIndex, new NamedClient(newClient, clientIndex, "guest " + clientIndex));
            }
            catch (ArgumentException ex)
            {
                SendMessageToClient(newClient, "Server Overload - " + ex.Message);
                newClient.Disconnect(false);
                newClient.Close();
            }

            // after "server.EndAccept" server needs to reOpen connection
            server.BeginAccept(onClientAccept, null);
            newClient.BeginReceive(gBuffer, 0, gBuffer.Length, SocketFlags.None, onMessageReceived, connectedClients[clientIndex]);
        }

        private int FindFreeIndex()
        {
            for (int i = 0; i < 100; i++)
            {
                if (!connectedClients.ContainsKey(i))
                    return i;
            }
            return 0;
        }

        // New Message from client
        private void onMessageReceived(IAsyncResult ar)
        {
            NamedClient nClient = (NamedClient)ar.AsyncState;
            Socket client = nClient.Client;

            try
            {
                int bytesReceived = client.EndReceive(ar);
                byte[] buffer = new byte[bytesReceived];
                Array.Copy(gBuffer, buffer, bytesReceived);

                string message = Encoding.ASCII.GetString(buffer);

                if (message.StartsWith("|USER_NAME|"))
                {
                    nClient.Name = message.Remove(0, 12);
                    OnMessageReceivedEventChandler?.Invoke(nClient, $"Name changed to: {nClient.Name}");
                }

                else if (message.StartsWith("/msg "))
                {
                    int messageStart = message.IndexOf(":");

                    if(messageStart >= 0)
                    {
                        string name = message.Remove(messageStart).Remove(0, 5);
                        string clientMsg = message.Remove(0, messageStart + 2);

                        var receiver = connectedClients.FirstOrDefault(o => o.Value.Name == name);

                        if (receiver.Value != null)
                            OnMessageReceivedEventChandler?.Invoke(receiver.Value, clientMsg);
                        else
                            OnMessageReceivedEventChandler?.Invoke(nClient, "User not found");
                    }
                    else 
                        OnMessageReceivedEventChandler?.Invoke(nClient, "Unknown command");
                }

                else if (message.StartsWith("/all"))
                {
                    string userList = default;

                    foreach (var user in connectedClients)
                    {
                        userList += user.Value.Name + "\n";
                    }
                    OnMessageReceivedEventChandler?.Invoke(nClient, userList);
                }

                else OnMessageReceivedEventChandler?.Invoke(nClient, "Unknown command");

                client.BeginReceive(gBuffer, 0, gBuffer.Length, SocketFlags.None, onMessageReceived, connectedClients[nClient.Index]);
            }
            catch (SocketException)
            {
                connectedClients.Remove(nClient.Index);
                client.Close();
            }
        }

        // Server sends message
        private void onMessageSend(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndSend(ar);
        }


    }
}