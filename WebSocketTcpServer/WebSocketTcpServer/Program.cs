

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
            Console.WriteLine($"|{nClient.Index}:_{nClient.Name}| {message}");

            server.SendMessageToClient(client, message);
        }
    }
}
