using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace server
{
    public class Server : IDisposable
    {
        private static string Host = "127.0.0.1";
        private static int Port = 8888;

        private TcpListener tcpListener = new TcpListener(IPAddress.Parse(Host), Port);
        private List<Client> clients = new List<Client>();

        public void RemoveConnection(string id)
        {
            Client client = clients.FirstOrDefault(c => c.Id == id);

            if (client != null)
            {
                clients.Remove(client);
            }

            client?.Close();
        }

        public async Task ListenAsync()
        {
            try
            {
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                    Client clientObject = new Client(tcpClient, this);
                    clients.Add(clientObject);
                    Task.Run(clientObject.ProcessAsync);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                CloseAll();
            }
        }

        public async Task BroadcastMessageAsync(string message, string id)
        {
            foreach (var client in clients)
            {
                if (client.IsConnected)
                {
                    await client.Writer.WriteLineAsync(message);
                    await client.Writer.FlushAsync();
                }
            }
        }

        public void Dispose()
        {
            CloseAll();
        }

        private void CloseAll()
        {
            foreach (var client in clients)
            {
                client.Close();
            }
            tcpListener.Stop();
        }
    }
}
