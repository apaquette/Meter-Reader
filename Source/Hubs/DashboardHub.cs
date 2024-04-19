using Microsoft.AspNetCore.SignalR;

namespace EnergyInsightHub.Hubs;

/// <summary>
/// Represents a SignalR hub for updating the dashboard in real-time.
/// </summary>
public class DashboardHub : Hub
{
    /// <summary>
    /// Updates the dashboard for all connected clients.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateDashboard()
    {
        await Clients.All.SendAsync("UpdateDashboard");
    }
}
