using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Auto.Website.Hubs;

public class AutoHub : Hub
{
    public async Task NotifyWebUsers(string user, string message) {
        await Clients.All.SendAsync("DisplayNotification", user, message);
    }
}