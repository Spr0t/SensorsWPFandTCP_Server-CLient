using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;

        static void Main(string[] args)
        {
            client = new TcpClient();

            client.Connect(host, port);     
            stream = client.GetStream();    
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage)); 
            receiveThread.Name = "reciveThread";
            receiveThread.Start();                                          
            
            Console.ReadLine();
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close(); 
            if (client != null)
                client.Close(); 
            Environment.Exit(0);
        }


        static void ReceiveMessage() 
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[512];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();

                    if (message.Contains("Alarm"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (message.Contains("Warning"))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine(message);
                }
                catch
                {
                    Console.WriteLine("Disconnected!"); 
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }
    }
}
