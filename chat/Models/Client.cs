using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace chat.Models
{
    public class Client
    {
        private string _host;
        private int _port;
        private string _name;
        private TcpClient _tcpClient;
        private StreamReader _reader;
        private StreamWriter _writer;

        public event Action<bool> ConnectionChanged;
        public event Action<string> MessageRecieved;

        public Client(string host, int port, string name)
        {
            _host = host;
            _port = port;
            _name = name;
            _tcpClient = new TcpClient();
        }

        public async Task Start()
        {
            try
            {
                _tcpClient.Connect(_host, _port);

                _reader = new StreamReader(_tcpClient.GetStream());
                _writer = new StreamWriter(_tcpClient.GetStream());

                Task.Run(() => ReceiveMessageAsync());
                await Send(_name);

                ConnectionChanged?.Invoke(true);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                ConnectionChanged?.Invoke(false);
            }
        }

        public async Task Send(string message)
        {
            try
            {
                await _writer.WriteLineAsync(message);
                await _writer.FlushAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                ConnectionChanged?.Invoke(false);
            }
        }

        private async Task ReceiveMessageAsync()
        {
            while (true)
            {
                try
                {
                    string message = await _reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(message)) continue;

                    MessageRecieved?.Invoke(message);
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
