using NLog;
using SensorsWPF.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SensorsWPF.TCPLogic
{
    public static class Server
    {
        static ServerObject server;
        static Thread listenThread; 

        public static void Start()
        {
            try
            {
                server = new ServerObject();                
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Name = "StartServer";
                listenThread.Start();                       
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class ServerObject
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();
        private static TcpListener tcpListener;
        private List<ClientObject> clients = new List<ClientObject>();

        internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
            SensorFactory.AppendClient(clientObject);
        }

        internal void RemoveConnection(string id)                           
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);  

            if (client != null)                                             
            {
                clients.Remove(client);
            }
        }

        public void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();

                while (true)
                {
                    var tcpClient = tcpListener.AcceptTcpClient();

                    var clientObject = new ClientObject(tcpClient, this);
                    var clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public static void SendMessage(string message, ClientObject client)  
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                client.Stream.Write(data, 0, data.Length);
            }
            catch
            {
                client.Close();
                Logger.Info($"client has disconnected : {client.Id}");
            }

        }

        internal void Disconnect()                                           
        {
            tcpListener.Stop();                                        
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();                                      
            }
            Environment.Exit(0);                                            
        }

    }
}
