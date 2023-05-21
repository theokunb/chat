using chat.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace chat.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private static string Host = "127.0.0.1";
        private static int Port = 8888;

        private string _name;
        private bool _isConnected = false;
        private Client _client;
        private Dispatcher _dispatcher;

        public MainViewModel()
        {
            SendCommand = new Command((parameter) => OnSend(parameter));
            ConnectCommand = new Command((param) => OnConnect(param));
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();
        public Command SendCommand { get; private set; }
        public Command ConnectCommand { get; private set; }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }

        private async void OnSend(object parameter)
        {
            await _client.Send(parameter.ToString());
        }

        private void OnMessageRecieved(string message)
        {
            _dispatcher.Invoke(new Action(() =>
            {
                Messages.Add(message);
            }));
        }

        private async void OnConnect(object parameter)
        {
            _client = new Client(Host, Port, Name);
            _client.MessageRecieved += OnMessageRecieved;
            _client.ConnectionChanged += OnConnectionChanged;
            await _client.Start();
        }

        private void OnConnectionChanged(bool connectionStatus)
        {
            IsConnected = connectionStatus;
        }
    }
}