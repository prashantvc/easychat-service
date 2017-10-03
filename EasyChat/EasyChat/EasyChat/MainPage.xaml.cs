using System;
using System.Net.WebSockets;
using System.Threading;
using Xamarin.Forms;

namespace EasyChat
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

	    async void Button_OnClicked(object sender, EventArgs e)
	    {
            if (string.IsNullOrEmpty(UserName.Text))
            {
                await DisplayAlert("Easy Chat", "Please enter username", "OK");
                return;
            }

            await Navigation.PushAsync(new ChatPage());


         //   var client = new ClientWebSocket();
	        //var token = new CancellationToken();

	        //await client.ConnectAsync(new Uri("ws://10.0.2.2:5000"), token);

	        //Console.WriteLine($"Websocket state {client.State}");
        }
	}
}
