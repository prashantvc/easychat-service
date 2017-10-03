using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EasyChat
{
    public sealed class ChatPageViewModel : INotifyPropertyChanged
    {
        readonly ClientWebSocket client;
        readonly CancellationToken token;
        Command connect;

        public ChatPageViewModel()
        {
            client = new ClientWebSocket();
            token = new CancellationToken();
        }

        public Command Connect => connect ?? (connect = new Command(ConnectToServerAsync));

        async void ConnectToServerAsync()
        {
            await client.ConnectAsync(new Uri("ws://10.0.2.2:5000"), token);
            OnPropertyChanged(nameof(IsConnected));

            Console.WriteLine($"Websocket state {client.State}");
        }

        public bool IsConnected => client.State == WebSocketState.Open;

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
