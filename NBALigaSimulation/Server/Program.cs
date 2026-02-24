global using NBALigaSimulation.Shared.Models;
global using NBALigaSimulation.Shared.Dtos;
global using NBALigaSimulation.Shared.Engine;
global using NBALigaSimulation.Server.Data;
global using Microsoft.EntityFrameworkCore;
global using NBALigaSimulation.Server.Services.PlayersService;
global using NBALigaSimulation.Server.Services.TeamService;
global using NBALigaSimulation.Server.Services.GameService;
global using NBALigaSimulation.Server.Services.SeasonService;
global using NBALigaSimulation.Server.Services.TradeService;
global using NBALigaSimulation.Server.Services.FAService;
global using NBALigaSimulation.Server.Services.StatsService;
global using NBALigaSimulation.Server.Services.DraftService;
global using NBALigaSimulation.Server.Repositories.Interfaces;
global using NBALigaSimulation.Server.Repositories.Implementations;
global using NBALigaSimulation.Server.Services.AuthService;
using NBALigaSimulation.Server.Services.NewsService;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NBALigaSimulation.Server.Services.LeagueService;
using NBALigaSimulation.Server.Services.PlayoffsService;
using NBALigaSimulation.Server.Services.AwardsService;
using NBALigaSimulation.Server.Services.GmService;
using EFCore.NamingConventions;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Log da connection string (senha mascarada) para conferir qual banco está sendo usado
var connStr = builder.Configuration["ConnectionStrings:NbaligaDBConnectionString"];
if (!string.IsNullOrEmpty(connStr))
{
    var masked = System.Text.RegularExpressions.Regex.Replace(connStr, @"Password=[^;]*", "Password=***");
    Console.WriteLine($"[DB] Connection: {masked}");
}

// Add services to the container.

builder.Services.AddDbContext<DataContext>(DbContextOptions =>
          DbContextOptions.UseNpgsql(builder.Configuration["ConnectionStrings:NbaligaDBConnectionString"])
              .UseLowerCaseNamingConvention());

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<ITradeService, TradeService>();
builder.Services.AddScoped<IFAService, FAService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<IPlayoffsService, PlayoffsService>();
builder.Services.AddScoped<IDraftService, DraftService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAwardsService, AwardsService>();
builder.Services.AddScoped<IGmService, GmService>();

builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddHttpContextAccessor();

var cultureInfo = new CultureInfo("en-US");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var app = builder.Build();

// Diagnóstico: confere se o banco tem dados (mesma conexão que a API usa)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
        var teamsCount = ctx.Teams.Count();
        var seasonsCount = ctx.Seasons.Count();
        Console.WriteLine($"[DB] Ao iniciar - Teams: {teamsCount}, Seasons: {seasonsCount}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB] Erro ao testar conexão: {ex.Message}");
    }
}

app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseHttpsRedirection();

// Habilitar CORS
app.UseCors("AllowAll");

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();