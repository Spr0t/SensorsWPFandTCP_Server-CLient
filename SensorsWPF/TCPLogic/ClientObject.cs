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
        //public int SensorId { get; private set; }
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

                string message = $"Successfuly connected to server!";
                ServerObject.SendMessage(message, this);

                while (true) 
                {
                    //try
                    //{
                    //     message = GetMessage();
                    //}
                    //catch
                    //{

                    //    break;
                    //}
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



        //public string GetMessage() 
        //{
        //    byte[] data = new byte[512];
        //    StringBuilder builder = new StringBuilder();
        //    int bytes = 0;
        //    do
        //    {
        //        bytes = Stream.Read(data, 0, data.Length);
        //        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
        //    }
        //    while (Stream.DataAvailable);

        //    return builder.ToString();
        //}

        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }

    }
}