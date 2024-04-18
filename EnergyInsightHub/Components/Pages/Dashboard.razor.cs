using EnergyInsightHub.Data;
using EnergyInsightHub.Models;
using EnergyInsightHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace EnergyInsightHub.Components.Pages; 
public partial class Dashboard : ComponentBase {
    private List<Row> GridRows { get; set; } = new();
    private List<Column> ChartColumns { get; set; } = new();
    private List<Interval> Intervals { get; set; } = new();

    [Inject]
    private IDbContextFactory<EnergyHubContext>? EnergyHubContextFactory { get; set; }
    [Inject]
    private NavigationManager? Navigation { get; set; }
    [Inject]
    private DashboardUpdater? DashboardUpdater { get; set; }

    [Inject]
    private TransmissionService? TransmissionService { get; set; }
    private HubConnection? hubConnection { get; set; }

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

    protected async void LoadReads() {
        GridRows = new();
        ChartColumns = new();
        Intervals = new();
        var _context = await EnergyHubContextFactory.CreateDbContextAsync();
        if (_context != null) {
            try {
                var days = _context.Readings.ToList().GroupBy(item => item.Time.Date);
                _context.Dispose();
                foreach (var day in days) {
                    var date = day.Key;
                    List<Reading?> readings = day?.OrderBy(r => r.Time)?.ToList();

                    List<Column> columns = readings?.Select(r => new Column {
                        Amount = r.Amount,
                        Date = r.Time
                    }).ToList() ?? new();
                    for (int i = 1; i < columns.Count; ++i) {
                        TimeSpan timeDifference = columns[i].Date - columns[i - 1].Date;

                        if (timeDifference == TimeSpan.FromHours(1)) {
                            decimal intervalAmount = columns[i].Amount - columns[i - 1].Amount;
                            Intervals.Add(new Interval { Amount = intervalAmount, Time = columns[i].Date });
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
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        //StateHasChanged();
    }

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

    // this works, but state has changed doesn't
    private async void ResetDatabase() {
        var _context = await EnergyHubContextFactory.CreateDbContextAsync();

        if (_context != null) {
            _context.ResetDB();
            LoadReads();
        }
    }

    private class Row {
        public DateTime Date { get; set; }
        public string DisplayDate => Date.ToString("YYYY-MM-dd");
        public List<Reading?> Readings { get; set; } = new();
        public decimal? Total => Readings?.Last(r => r != null)?.Amount ?? 0 - Readings?.First(r => r != null)?.Amount ?? 0;
    }

    private class Column {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string CategoryDisplay => Date.TimeOfDay == TimeSpan.Zero ? $"{Date.ToString("dd-MMM")}" : Date.ToString("HH:mm");
    }

    private class Interval {
        public decimal Amount { get; set; }
        public DateTime Time { get; set; }
        public string CategoryDisplay => Time.TimeOfDay == TimeSpan.Zero ? $"{Time.ToString("dd-MMM")}" : Time.ToString("HH:mm");
    }
}
