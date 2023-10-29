

using WebSocketTcpServer;

namespace SocketTcpServer
{
    public class Program
    {
        static private ServerService server;

        public static void Main() 
        {
            server = new ServerService();

            server.ServerStart();
            server.OnMessageReceivedEventChandler += OnMessageReceivedEventChandler;

            Console.ReadKey();
        }

        private static void OnMessageReceivedEventChandler(NamedClient nClient, string message)
        {
            var client = nClient.Client;
            Console.WriteLine($"Client - {nClient.Index}: Name {nClient.Name} send message {message}");

            server.SendMessageToClient(client, $"{nClient.Index}: {nClient.Name} Your message was {message}");
        }
    }
}
