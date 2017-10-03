using System;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EasyChat
{
    public sealed class ChatPageViewModel : INotifyPropertyChanged
    {

        public ChatPageViewModel()
        {
            client = new ClientWebSocket();
            cts = new CancellationTokenSource();
        }

        public bool IsConnected => client.State == WebSocketState.Open;
        public Command Connect => connect ?? (connect = new Command(ConnectToServerAsync));
        public Command SendMessage => sendMessageCommand ?? 
            (sendMessageCommand = new Command<string>(SendMessageAsync, CanSendMessage));
        
        async void ConnectToServerAsync()
        {
            await client.ConnectAsync(new Uri("ws://10.0.2.2:5000"), cts.Token);

            UpdateClientState();

            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    WebSocketReceiveResult result;
                    var message = new ArraySegment<byte>(new byte[4096]);
                    do
                    {
                        result = await client.ReceiveAsync(message, cts.Token);
                        var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                        string receivedMessage = Encoding.UTF8.GetString(messageBytes);
                        Console.WriteLine("Received: {0}", receivedMessage);

                    } while (!result.EndOfMessage);
                }
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            void UpdateClientState()
            {
                OnPropertyChanged(nameof(IsConnected));
                sendMessageCommand.ChangeCanExecute();
                Console.WriteLine($"Websocket state {client.State}");
            }
        }

        async void SendMessageAsync(string message = "This is test message ")
        {
            if (!CanSendMessage(message))
                return;

            var byteMessage = Encoding.UTF8.GetBytes(message + DateTime.Now.ToString());
            var segmnet = new ArraySegment<byte>(byteMessage);

            await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts.Token);
        }

        bool CanSendMessage(string message)
        {
            return IsConnected;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        readonly ClientWebSocket client;
        readonly CancellationTokenSource cts;

        Command connect;
        Command<string> sendMessageCommand;
    }
}
