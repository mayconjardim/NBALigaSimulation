global using Microsoft.AspNetCore.Components.Authorization;
global using NBALigaSimulation.Client.Services.AuthService;
global using NBALigaSimulation.Client.Services.PlayersService;
global using NBALigaSimulation.Client.Services.TeamsService;
global using NBALigaSimulation.Client.Services.GameService;
global using NBALigaSimulation.Client.Services.StatsService;
global using NBALigaSimulation.Client.Services.NewsService;
global using NBALigaSimulation.Client.Services.DataService;
global using NBALigaSimulation.Client.Services.FAService;
global using NBALigaSimulation.Client.Services.SeasonsService;
global using NBALigaSimulation.Client.Services.LeagueService;
global using NBALigaSimulation.Client.Services.TradesService;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NBALigaSimulation.Client;
using Blazored.LocalStorage;
using pax.BlazorChartJs;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IFAService, FAService>();
builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<ITradeService, TradeService>();

builder.Services.AddChartJs(options =>
{
    // default
    options.ChartJsLocation = "https://cdn.jsdelivr.net/npm/chart.js";
    options.ChartJsPluginDatalabelsLocation = "https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2";
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOptions();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddBlazorBootstrap();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();