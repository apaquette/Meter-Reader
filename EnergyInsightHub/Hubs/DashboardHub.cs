using Microsoft.AspNetCore.SignalR;

namespace EnergyInsightHub.Hubs;

public class DashboardHub : Hub
{
    public async Task UpdateDashboard()
    {
        await Clients.All.SendAsync("UpdateDashboard");
    }
}
