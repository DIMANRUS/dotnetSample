using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;

namespace SignalRForms
{
    public partial class MainPage : ContentPage
    {
        private HubConnection hubConnection;
        private ObservableCollection<string> messages = new ObservableCollection<string>();
        public MainPage()
        {
            InitializeComponent();
            listView.ItemsSource = messages;
            Task.Run(async ()=> {
                hubConnection = new HubConnectionBuilder().WithUrl("https://api.personalhelper.dimanrus.ru/chat").Build();
                hubConnection.On<string, string>("ReceiveMessage", (userName, message) =>
                {
                    messages.Add($"{userName}: {message}");
                });
                await hubConnection.StartAsync();
            });
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await hubConnection.SendAsync("SendMessage", "IOS", message.Text);
        }
    }
}
