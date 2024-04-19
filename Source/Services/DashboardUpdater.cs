using EnergyInsightHub.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;

namespace EnergyInsightHub.Services;

/// <summary>
/// Monitors a file for changes and updates the dashboard accordingly.
/// </summary>
public class DashboardUpdater
{
    /// <summary>
    /// The file system watcher instance.
    /// </summary>
    private FileSystemWatcher Watcher { get; set; }
    /// <summary>
    /// The SignalR hub context for communicating with clients. See <see cref="DashboardUpdater"/>
    /// </summary>
    private readonly IHubContext<DashboardHub> HubContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardUpdater"/> class.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="hubContext">The SignalR hub context for updating the dashboard.</param>
    public DashboardUpdater(IConfiguration configuration, IHubContext<DashboardHub> hubContext)
    {
        HubContext = hubContext;

        string connectionString = configuration.GetConnectionString("EnergyHubDB");
        var connectionStringBuilder = new SqliteConnectionStringBuilder(connectionString);

        string directoryPath = Path.GetDirectoryName(connectionStringBuilder.DataSource);
        string fileName = Path.GetFileName(connectionStringBuilder.DataSource);

        Watcher = new();
        Watcher.Path = "Data";// directoryPath;
        Watcher.Filter = "update.txt";// fileName;
        Watcher.NotifyFilter = NotifyFilters.LastWrite;
        Watcher.Changed += OnFileChanged;
        Watcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Handles the file change event and updates the dashboard.
    /// </summary>
    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        await HubContext.Clients.All.SendAsync("UpdateDashboard");
    }
}
