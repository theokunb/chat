using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace server
{
    public class Client
    {
        private TcpClient client;
        private Server server;

        public Client(TcpClient tcpClient, Server serverObject)
        {
            client = tcpClient;
            server = serverObject;

            var stream = client.GetStream();
            Reader = new StreamReader(stream);
            Writer = new StreamWriter(stream);
        }

        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public StreamWriter Writer { get; private set; }
        public StreamReader Reader { get; private set; }
        public bool IsConnected => client.Connected;


        public async Task ProcessAsync()
        {
            try
            {
                string userName = await Reader.ReadLineAsync();
                string message = $"{userName} вошел в чат";
                await server.BroadcastMessageAsync(message, Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();

                        if (message == null) 
                            continue;

                        message = $"{userName}: {message}";
                        Console.WriteLine(message);
                        await server.BroadcastMessageAsync(message, Id);
                    }
                    catch
                    {
                        message = $"{userName} покинул чат";
                        Console.WriteLine(message);
                        await server.BroadcastMessageAsync(message, Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(Id);
            }
        }

        public void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
