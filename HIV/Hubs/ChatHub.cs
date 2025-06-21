using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace HIV.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _users = new();

        public override Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];
            if (!string.IsNullOrEmpty(userId))
            {
                _users[userId] = Context.ConnectionId;
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _users.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (!string.IsNullOrEmpty(userId))
            {
                _users.TryRemove(userId, out _);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivateMessage(string fromUser, string toUser, string message)
        {
            if (_users.TryGetValue(toUser, out var toConn))
            {
                await Clients.Client(toConn).SendAsync("ReceivePrivateMessage", fromUser, message, toUser);
            }

            if (_users.TryGetValue(fromUser, out var fromConn))
            {
                await Clients.Client(fromConn).SendAsync("ReceivePrivateMessage", fromUser, message, toUser);
            }
        }
    }
}
