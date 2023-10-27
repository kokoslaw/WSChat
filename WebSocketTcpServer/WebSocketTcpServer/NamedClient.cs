

using System.Net.Sockets;

namespace WebSocketTcpServer
{
    public class NamedClient
    {
        public NamedClient(Socket client, int index, string? name = null)
        {
            Client = client;
            Index = index;

            if (name != null)
                Name = name;
        }
        public Socket Client { get; }
        public string Name { get; set; } = "guest";
        public int Index { get; }

    }
}
