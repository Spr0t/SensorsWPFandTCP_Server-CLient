using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Threading;
using SensorsWPF.Entity;
using System.Text;

namespace SensorsWPF.Server
{
    public class ClientObject
    {
        public string Id { get; private set; }
        public NetworkStream Stream { get; private set; }

        public TcpClient client;
        public ServerObject server;                                 // Объект сервера

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

                

                string message = $"#Здравствуйте!  вы успешно подключились к системе";  // Посылаем сообщение о успешном подключении к серверу 
                ServerObject.SendMessage(message, this);

                while (true) // В бесконечном цикле получаем сообщения от клиента
                {
                    try
                    {
                         message = GetMessage();

                    }
                    catch
                    {

                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally // В случае выхода из цикла закрываем ресурсы
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }



        public string GetMessage() // Чтение входящего сообщения и преобразование в строку
        {
            byte[] data = new byte[512];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        public void Close() // Закрытие подключения
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }

    }
}