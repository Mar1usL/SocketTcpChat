using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace SocketTcpServer
{
    class Program
    {
        static Server _server;
        static Thread _serverThread;

        static void Main(string[] args)
        {
            try
            {
                _server = new Server();
                _serverThread = new Thread(new ThreadStart(_server.Listen));
                _serverThread.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                _server.Disconnect();
            }


        }
    }
}
