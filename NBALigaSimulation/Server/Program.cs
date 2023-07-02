global using NBALigaSimulation.Shared.Models;
global using NBALigaSimulation.Shared.Dtos;
global using NBALigaSimulation.Shared.Engine;
global using NBALigaSimulation.Server.Data;
global using Microsoft.EntityFrameworkCore;
global using NBALigaSimulation.Server.Services.PlayerService;
global using NBALigaSimulation.Server.Services.TeamService;
global using NBALigaSimulation.Server.Services.GameService;
global using NBALigaSimulation.Server.Services.SeasonService;
global using NBALigaSimulation.Server.Services.TradeService;
global using NBALigaSimulation.Server.Services.FAService;
global using NBALigaSimulation.Server.Services.AuthService;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(DbContextOptions =>
          DbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:NbaligaDBConnectionString"]));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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
builder.Services.AddScoped<IAuthService, AuthService>();

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

var app = builder.Build();

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

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();