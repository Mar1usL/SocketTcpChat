using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketTcpServer
{
    public class Server
    {
        private IPEndPoint _endPoint;
        private readonly string _address = "127.0.0.1";
        private readonly int _port = 8005;
        private Socket _handler;

        public Socket TcpSocket { get; set; }
        public List<Client> Clients { get; set; }

        public Server()
        {
            Clients = new List<Client>();
            _endPoint = new IPEndPoint(IPAddress.Parse(_address), _port);
        }

        public void AddConnection(Client client)
        {
            Clients.Add(client);
        }

        public void RemoveConnection(string id)
        {
            var client = Clients.FirstOrDefault(c => c.Id == id);

            if (client != null)
                Clients.Remove(client);
        }

        public void Listen()
        {
            try
            {
                TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                TcpSocket.Bind(_endPoint);
                TcpSocket.Listen(10);
                Console.WriteLine($"Server has started on port {_port}");

                while(true)
                {
                    // Accepting incoming requests
                    _handler = TcpSocket.Accept();
                    
                    Client client = new Client(_handler, this);
                    // Processing the client on a separtated thread
                    Thread clientThread = new Thread(new ThreadStart(client.Process));
                    clientThread.Start();
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public void BroadcastMessage(string message)
        {
            byte[] msgBuffer = Encoding.UTF8.GetBytes(message);
            
            for(int i = 0; i < Clients.Count; i++)
            {
                Clients[i].ClientSocket.Send(msgBuffer);
            }

        }

        public void Disconnect()
        {
            TcpSocket.Shutdown(SocketShutdown.Both);
            TcpSocket.Close();

            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].Disconnect();
            }
        }
    }
}
