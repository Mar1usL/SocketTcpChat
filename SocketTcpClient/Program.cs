using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace SocketTcpClient
{
    class Program
    {
        private static Socket _clientSocket;
        private const string _address = "127.0.0.1";
        private const int _port = 8005;

        static void Main(string[] args)
        {

            Console.WriteLine("Enter your username: ");
            string username = Console.ReadLine();

            try
            {
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _clientSocket.Connect(_address, _port);

                string msg = username;
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                _clientSocket.Send(buffer);

                Thread clientThread = new Thread(new ThreadStart(ReceiveMessage));
                clientThread.Start();
                Console.WriteLine($"Welcome {username}");
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }

        }

        static void SendMessage()
        {

            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(message);
                _clientSocket.Send(data);
            }
        }

        static void ReceiveMessage()
        {

            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    StringBuilder builder = new StringBuilder();
                    int count = 0;

                    do
                    {
                        count = _clientSocket.Receive(buffer);
                        builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
                    }
                    while (_clientSocket.Available > 0);

                    string message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Enter message: ");
            }
        }

        static void Disconnect()
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }
    }
}
