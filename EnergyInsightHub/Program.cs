using EnergyInsightHub.Components;
using EnergyInsightHub.Data;
using Microsoft.EntityFrameworkCore;
using EnergyInsightHub.Hubs;
using EnergyInsightHub.Services;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("EnergyHubDB");

// Add services to the container.
builder.Services.AddDbContextFactory<EnergyHubContext>(options => options.UseSqlite(connectionString))
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});

builder.Services.AddSingleton<DashboardUpdater>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Error", createScopeForErrors: true);
    //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
    app.UseHttpsRedirection();
}


app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseResponseCompression();

// SignalR
app.MapHub<DashboardHub>("/dashboardhub");
//app.MapFallbackToPage("/_Host");

app.Run();
