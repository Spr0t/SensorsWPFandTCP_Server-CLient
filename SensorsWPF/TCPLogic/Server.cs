using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SensorsWPF.TCPLogic
{
    public class Server
    {
        static ServerObject server; // Сервер
        static Thread listenThread; // Поток для прослушивания


        public Server()
        {
            try
            {
                server = new ServerObject();                // Старт сервера
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Name = "StartServer";
                listenThread.Start();                       // Старт потока
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }


    }
}
