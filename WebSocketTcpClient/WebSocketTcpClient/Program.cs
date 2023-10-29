using System.Net;

namespace WebSocketTcpClient
{
    public class Program
    {
        // |USER_NAME| to change userName
        static private ClientService client;

        static void Main()
        {
            client = new ClientService();
            client.ConnectTo(new IPEndPoint(IPAddress.Loopback, 100));

            while (true) 
            {
                string mesage = Console.ReadLine();

                client.SendMessageToServer(mesage);
            }
        }
    }
}