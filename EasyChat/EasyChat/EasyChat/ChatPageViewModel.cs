using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.DeviceInfo;
using Newtonsoft.Json;

namespace EasyChat
{
    public sealed class ChatPageViewModel : INotifyPropertyChanged
    {

        public ChatPageViewModel()
        {
            client = new ClientWebSocket();
            cts = new CancellationTokenSource();
            messages = new ObservableCollection<Message>();

            userName = "Prashant";
        }

        public bool IsConnected => client.State == WebSocketState.Open;
        public Command Connect => connect ?? (connect = new Command(ConnectToServerAsync));
        public Command SendMessage => sendMessageCommand ??
            (sendMessageCommand = new Command<string>(SendMessageAsync, CanSendMessage));
        public ObservableCollection<Message> Messages => messages;

        public string MessageText
        {
            get
            {
                return messageText;
            }
            set
            {
                messageText = value;
                OnPropertyChanged();

                sendMessageCommand.ChangeCanExecute();
            }
        }

        async void ConnectToServerAsync()
        {
            
#if __IOS__
            await client.ConnectAsync(new Uri("ws://localhost:5000"), cts.Token);
#else
            await client.ConnectAsync(new Uri("ws://10.0.2.2:5000"), cts.Token);
#endif

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
                        string serialisedMessae = Encoding.UTF8.GetString(messageBytes);

                        try
                        {
                            var msg = JsonConvert.DeserializeObject<Message>(serialisedMessae);
                            Messages.Add(msg);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Invalide message format. {ex.Message}");
                        }

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

        async void SendMessageAsync(string message)
        {
            var msg = new Message
            {
                Name = userName,
                MessagDateTime = DateTime.Now,
                Text = message,
                UserId = CrossDeviceInfo.Current.Id
            };

            string serialisedMessage = JsonConvert.SerializeObject(msg);

            var byteMessage = Encoding.UTF8.GetBytes(serialisedMessage);
            var segmnet = new ArraySegment<byte>(byteMessage);

            await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts.Token);
            MessageText = string.Empty;
        }

        bool CanSendMessage(string message)
        {
            return IsConnected && !string.IsNullOrEmpty(message);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        readonly ClientWebSocket client;
        readonly CancellationTokenSource cts;
        readonly string userName;

        Command connect;
        Command<string> sendMessageCommand;
        ObservableCollection<Message> messages;
        string messageText;
    }
}
