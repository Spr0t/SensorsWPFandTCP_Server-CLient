using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Threading;
using SensorsWPF.Entity;
using System.Text;

namespace SensorsWPF.TCPLogic
{
    public class ClientObject
    {
        public string Id { get; private set; }
        public NetworkStream Stream { get; private set; }

        public TcpClient client;
        public ServerObject server;                                 

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();

                string message = $"Successfully connected to server!";
                ServerObject.SendMessage(message, this);

                while (true) 
                {
                    //TODO receive some logic for client / logic for disconnecting
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally 
            {
                server.RemoveConnection(Id);
                Close();
            }
        }

        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }

    }
}