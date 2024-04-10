using EnergyInsightHub.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;

namespace EnergyInsightHub.Services;

public class DashboardUpdater
{
    private FileSystemWatcher watcher;
    private readonly IHubContext<DashboardHub> hubContext;

    public DashboardUpdater(IConfiguration configuration, IHubContext<DashboardHub> hubContext)
    {
        this.hubContext = hubContext;

        string connectionString = configuration.GetConnectionString("EnergyHubDB");
        var connectionStringBuilder = new SqliteConnectionStringBuilder(connectionString);

        string directoryPath = Path.GetDirectoryName(connectionStringBuilder.DataSource);
        string fileName = Path.GetFileName(connectionStringBuilder.DataSource);

        watcher = new();
        watcher.Path = "Data";// directoryPath;
        watcher.Filter = "update.txt";// fileName;
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += OnFileChanged;
        watcher.EnableRaisingEvents = true;
    }

    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        await hubContext.Clients.All.SendAsync("UpdateDashboard");
    }
}
