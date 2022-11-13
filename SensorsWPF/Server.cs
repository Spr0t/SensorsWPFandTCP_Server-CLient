using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SensorsWPF
{
    internal class Server
    {
        public string Result { get; set; }
        Socket listener;
        public int port { get; set; }

        public void Start()
        {
            const string ip = "127.0.0.1";
            


            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(5);

            listener = tcpSocket.Accept();



        }

        public void Send()
        {
            listener.Send(Encoding.UTF8.GetBytes(Result));
            //listener.Shutdown(SocketShutdown.Both);
            //listener.Close();

        }


    }
}
