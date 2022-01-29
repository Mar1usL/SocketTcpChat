using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketTcpServer
{
    public class Client
    {
        private readonly Server _server;

        public string Id { get; set; }
        public string Username { get; set; }
        public Socket ClientSocket { get; set; }

        public Client(Socket socket, Server server)
        {
            Id = Guid.NewGuid().ToString();
            ClientSocket = socket;
            _server = server;
            _server.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                string message = GetMessage();
                Username = message;
                message = String.Format("{0}: {1} has joined the chat", DateTime.Now.ToShortTimeString(), Username);
                _server.BroadcastMessage(message);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0} {1}: {2}", DateTime.Now.ToShortTimeString(), Username, message);
                        _server.BroadcastMessage(message);
                        Console.WriteLine(message);
                    }
                    catch
                    {

                        message = String.Format("{0}: {1} has disconnected", DateTime.Now.ToShortTimeString(), Username);
                        Console.WriteLine(message);
                        _server.BroadcastMessage(message);
                        break;
                    }
                }
            }
            catch
            {
                _server.RemoveConnection(this.Id);
                Disconnect();
            }
            
        }

        private string GetMessage()
        {
            byte[] data = new byte[1024];
            StringBuilder builder = new StringBuilder();
            int count = 0;

            try
            {
                do
                {
                    count = ClientSocket.Receive(data);
                    builder.Append(Encoding.UTF8.GetString(data, 0, count));
                }
                while (ClientSocket.Available > 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return builder.ToString();
        }

        public void Disconnect()
        {
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
        }
    }
}
