using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", user, message);
        }

        public async Task SendProductUpdate(string action, object product)
        {
            await Clients.All.SendAsync("ProductUpdated", action, product);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveNotification", "System", "Connected to notification hub");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
