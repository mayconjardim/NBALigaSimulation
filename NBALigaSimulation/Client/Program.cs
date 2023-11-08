global using System.Net.Http.Json;
global using NBALigaSimulation.Shared.Models;
global using NBALigaSimulation.Shared.Dtos;
global using NBALigaSimulation.Client.Utilities;
global using NBALigaSimulation.Client.Shared.Services;
global using NBALigaSimulation.Client.Services.PlayerService;
global using NBALigaSimulation.Client.Services.TeamService;
global using NBALigaSimulation.Client.Services.GameService;
global using NBALigaSimulation.Client.Services.AuthService;
global using NBALigaSimulation.Client.Services.TradeService;
global using NBALigaSimulation.Client.Services.FAService;
global using NBALigaSimulation.Client.Services.SeasonService;
global using NBALigaSimulation.Client.Services.StatsService;
global using NBALigaSimulation.Client.Services.LeagueService;
global using NBALigaSimulation.Client.Services.PlayoffsService;
global using NBALigaSimulation.Client.Services.DraftService;
global using NBALigaSimulation.Client.Services.NewsService;
global using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using NBALigaSimulation.Client;
using pax.BlazorChartJs;
using Blazored.LocalStorage;
using Plk.Blazor.DragDrop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ITradeService, TradeService>();
builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<IFAService, FAService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<IPlayoffsService, PlayoffsService>();
builder.Services.AddScoped<IDraftService, DraftService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddBlazorDragDrop();

builder.Services.AddChartJs(options =>
{
    // default
    options.ChartJsLocation = "https://cdn.jsdelivr.net/npm/chart.js@3.0.0/dist/chart.min.js";
    options.ChartJsPluginDatalabelsLocation = "https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2.0.0";
});

await builder.Build().RunAsync();