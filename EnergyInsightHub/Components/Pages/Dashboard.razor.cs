using EnergyInsightHub.Data;
using EnergyInsightHub.Models;
using EnergyInsightHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace EnergyInsightHub.Components.Pages;

/// <summary>
/// Represents the dashboard component of the web application.
/// </summary>
public partial class Dashboard : ComponentBase {
    /// <summary>
    /// Gets or sets the list of rows for the datagrid.
    /// </summary>
    private List<Row> GridRows { get; set; } = new();
    /// <summary>
    /// Gets or sets the readings to display the column chart.
    /// </summary>
    private List<ChartData> ChartColumns { get; set; } = new();
    /// <summary>
    /// Gets or sets the hourly consumption data for the line chart.
    /// </summary>
    private List<ChartData> Intervals { get; set; } = new();
    /// <summary>
    /// The step value for the charts, defaults to hourly, and is calculated dynamically for better data visualization.
    /// </summary>
    private TimeSpan StepValue { get; set; } = TimeSpan.FromHours(1);
    /// <summary>
    /// Gets or sets the factory for creating instances of <see cref="EnergyHubContext"/>.
    /// </summary>
    [Inject]
    private IDbContextFactory<EnergyHubContext>? EnergyHubContextFactory { get; set; }
    /// <summary>
    /// Gets or sets the navigation manager for the web application.
    /// </summary>
    [Inject]
    private NavigationManager? Navigation { get; set; }
    /// <summary>
    /// Transmission Service that simulates the transmission of meter images. <see cref="TransmissionService"/>
    /// </summary>
    [Inject]
    private TransmissionService? TransmissionService { get; set; }
    /// <summary>
    /// Gets or sets the hub connection.
    /// </summary>
    private HubConnection? hubConnection { get; set; }

    /// <summary>
    /// Method invoked when the component is initialized asynchronously. Performs initial reads load, and defines hubConnection event to update dashboard live.
    /// </summary>
    protected override async Task OnInitializedAsync() {
        LoadReads(); // first load

        hubConnection = new HubConnectionBuilder()
          .WithUrl(Navigation.ToAbsoluteUri("/dashboardhub"))
          .Build();

        hubConnection.On("UpdateDashboard", () => {
            LoadReads();
            InvokeAsync(StateHasChanged);
        });

        await hubConnection.StartAsync();
    }

    /// <summary>
    /// Loads readings asynchronously from database and calculates GridRows, ChartColumns, Intervals, and StepValue.
    /// </summary>
    protected async void LoadReads() {
        GridRows = new();
        ChartColumns = new();
        Intervals = new();
        StepValue = TimeSpan.FromHours(1);

        var _context = await EnergyHubContextFactory.CreateDbContextAsync();
        if (_context != null) {
            try {
                var days = _context.Readings.ToList().GroupBy(item => item.Time.Date);
                _context.Dispose();
                foreach (var day in days) {
                    var date = day.Key;
                    List<Reading?> readings = day?.OrderBy(r => r.Time)?.ToList();

                    List<ChartData> columns = readings?.Select(r => new ChartData {
                        XAxisData = r.Amount,
                        YAxisData = r.Time
                    }).ToList() ?? new();
                    for (int i = 1; i < columns.Count; ++i) {
                        TimeSpan timeDifference = columns[i].YAxisData - columns[i - 1].YAxisData;

                        if (timeDifference == TimeSpan.FromHours(1)) {
                            decimal intervalAmount = columns[i].XAxisData - columns[i - 1].XAxisData;
                            Intervals.Add(new ChartData { XAxisData = intervalAmount, YAxisData = columns[i].YAxisData });
                        }
                    }

                    //handle missing readings
                    if (readings.Count < 24) {
                        readings = PadReadings(readings);
                    }

                    GridRows.Add(
                        new() {
                            Date = date,
                            Readings = readings
                        }
                    );
                    ChartColumns.AddRange(columns);

                    if(ChartColumns.Count > 0) {
                        int count = ChartColumns.Count / 10;
                        if (count < 1) count = 1;
                        StepValue = TimeSpan.FromHours(count);
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }

    /// <summary>
    /// Pads the readings list to ensure it has 24 elements to match 24 hours in a day.
    /// </summary>
    /// <param name="readings">A List of readings that needs to be padded</param>
    /// <returns>Returns a padded list of readings</returns>
    private List<Reading?> PadReadings(List<Reading?> readings) {
        //early exit is there are no readings at all
        if (readings.Count == 0) return Enumerable.Repeat((Reading?)null, 24).ToList();

        List<Reading?> result = new();
        for (int hour = 0; hour <= 23; ++hour) {
            var existingReading = readings.FirstOrDefault(r => r?.Time.Hour == hour);
            result.Add(existingReading);
        }
        return result;
    }

    /// <summary>
    /// Resets the database to default values, and moves meter images back into the test folder.
    /// </summary>
    private async void ResetDatabase() {
        var _context = await EnergyHubContextFactory.CreateDbContextAsync();

        if (_context != null) {
            _context.ResetDB();
        }

        string sourceDir = "MeterReader";
        string destinationDir = "TestMeterImages";
        List<string> imageFiles = Directory.GetFiles(sourceDir, "*.png").ToList();
        foreach (string image in imageFiles) {
            string newImageDir = Path.Combine(destinationDir, Path.GetFileName(image));
            File.Move(image, newImageDir);
        }

        LoadReads();
    }

    /// <summary>
    /// Represents a row in the datagrid, which contains 24 hours of reads data.
    /// </summary>
    private class Row {
        /// <summary>
        /// The date of the row.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// The list of readings representing hourly reads for a day.
        /// </summary>
        public List<Reading?> Readings { get; set; } = new();
        /// <summary>
        /// The total amount of energy consumed in that day. Calculated by subtracting the last read from the first read.
        /// </summary>
        public decimal? Total => Readings?.Last(r => r != null)?.Amount ?? 0 - Readings?.First(r => r != null)?.Amount ?? 0;
    }

    /// <summary>
    /// Represents data for the charts.
    /// </summary>
    private class ChartData {
        /// <summary>
        /// The value for the x-axis of the chart.
        /// </summary>
        public decimal XAxisData { get; set; }
        /// <summary>
        /// The value for the y-axis of the chart.
        /// </summary>
        public DateTime YAxisData { get; set; }
    }
}
