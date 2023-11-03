using System.Net;

namespace WebSocketTcpClient
{
    public class Program
    {
        static private ClientService client;

        static void Main()
        {
            int port;
            string ip;

            Console.WriteLine("Enter IPAddress");

            ip = Console.ReadLine()!;
            if(string.IsNullOrEmpty(ip)) 
                ip = "192.168.1.1";

            Console.WriteLine("Enter IPAddress");
 
            if(!int.TryParse(Console.ReadLine(), out port))
                port = 100;

            client = new ClientService();
            client.ConnectTo(new IPEndPoint(IPAddress.Parse(ip), port));

            while (true) 
            {
                string mesage = Console.ReadLine()!;

                client.SendMessageToServer(mesage);
            }
        }
    }
}