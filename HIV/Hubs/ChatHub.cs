using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace HIV.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _users = new();
        private static readonly ConcurrentDictionary<string, string> _userRoles = new();
        private static readonly ConcurrentDictionary<string, string> _assignedStaff = new();

        private bool IsUserStaff(string userId)
        {
            return _userRoles.TryGetValue(userId, out var role) && role == "Staff";
        }


        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.Request.Query["userId"].ToString();
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;


            if (!string.IsNullOrEmpty(userId))
            {
                _users[userId] = Context.ConnectionId;

                if (!string.IsNullOrEmpty(role))
                {
                    _userRoles[userId] = role;
                }
            }

            return base.OnConnectedAsync();
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _users.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

            if (!string.IsNullOrEmpty(userId))
            {
                _users.TryRemove(userId, out _);
                _userRoles.TryRemove(userId, out var role);

                // Nếu là staff, xóa các patient đang được gán với staff này
                if (role == "Staff")
                {
                    foreach (var entry in _assignedStaff.Where(p => p.Value == userId).ToList())
                    {
                        _assignedStaff.TryRemove(entry.Key, out _);
                    }
                }
            }

            return base.OnDisconnectedAsync(exception);
        }


        public async Task SendPrivateMessage(string fromUser, string toUser, string message)
        {
            var fromRole = Context.User?.FindFirst("role")?.Value;

            // Nếu người gửi là Patient
            if (fromRole == "Patient")
            {
                // Nếu chưa có staff được gán
                if (!_assignedStaff.TryGetValue(fromUser, out var assignedStaffId))
                {
                    // Gửi đến tất cả staff đang online
                    var staffIds = _userRoles
                        .Where(kvp => kvp.Value == "Staff" && _users.ContainsKey(kvp.Key))
                        .Select(kvp => kvp.Key)
                        .ToList();

                    foreach (var staffId in staffIds)
                    {
                        var connId = _users[staffId];
                        await Clients.Client(connId).SendAsync("ReceivePrivateMessage", fromUser, message, staffId);
                    }

                    // Gửi lại cho chính bệnh nhân hiển thị
                    if (_users.TryGetValue(fromUser, out var fromConn))
                    {
                        await Clients.Client(fromConn).SendAsync("ReceivePrivateMessage", fromUser, message, "");
                    }

                    return;
                }

                // Nếu đã có staff được gán → gửi như bình thường
                if (_users.TryGetValue(assignedStaffId, out var staffConn))
                {
                    await Clients.Client(staffConn).SendAsync("ReceivePrivateMessage", fromUser, message, assignedStaffId);
                }

                if (_users.TryGetValue(fromUser, out var fromConn2))
                {
                    await Clients.Client(fromConn2).SendAsync("ReceivePrivateMessage", fromUser, message, assignedStaffId);
                }
            }
            else
            {
                // Người gửi là Staff → gửi đến bệnh nhân
                if (_users.TryGetValue(toUser, out var patientConn))
                {
                    await Clients.Client(patientConn).SendAsync("ReceivePrivateMessage", fromUser, message, toUser);
                }

                if (_users.TryGetValue(fromUser, out var staffConn))
                {
                    await Clients.Client(staffConn).SendAsync("ReceivePrivateMessage", fromUser, message, toUser);
                }

                // Nếu bệnh nhân chưa được gán → gán staff này làm người hỗ trợ
                if (!_assignedStaff.ContainsKey(toUser))
                {
                    _assignedStaff[toUser] = fromUser;
                }
            }
        }

    }
}
