using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SensorsWPF.TCPLogic
{
    public class ServerObject
    {
        private static TcpListener tcpListener;
        private List<ClientObject> clients = new List<ClientObject>();
        public static ClientObject client { get;  set; }

        internal void AddConnection(ClientObject clientObject)    // добавляем подключение
        {
           client = clientObject;
           clients.Add(clientObject);
        }


        internal void RemoveConnection(string id)                             // убираем подключение
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);  // получаем по id закрытое подключение   

            if (client != null)                                             // и удаляем его из списка подключений
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
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Name = "ClientThread";
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public static void SendMessage(string message, ClientObject client)   // передача данных     
        {
    
                byte[] data = Encoding.UTF8.GetBytes(message);
                
                client.Stream.Write(data, 0, data.Length);
         
        }


        internal void Disconnect()                                            // отключение всех клиентов
        {
            tcpListener.Stop();                                             //остановка сервера


            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();                                         //отключение клиента
            }
            Environment.Exit(0);                                            //завершение процесса
        }

    }
}
