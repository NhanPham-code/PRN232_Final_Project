using Microsoft.AspNetCore.SignalR.Client;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HubConnection _connection;

        public NotificationService()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7126/notificationHub") // Địa chỉ URL của SignalR Hub
                .Build();

            _ = _connection.StartAsync();
        }

        public async Task NotifyProfileUpdatedAsync(int userId)
        {
            if(_connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("NotifyProfileUpdated", userId);
            }
        }

        public async Task NotifyProductUpdate()
        {
            if(_connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("NotifyProductUpdate");
            }
        }
        public async Task NotifyCartChanged()
        {
            await _connection.InvokeAsync("NotifyCartChanged");
        }
    }
}
