using Microsoft.EntityFrameworkCore;
using SkyPulse.Infrastructure.Services;
using SkyPulse.Core.Models;
using SkyPulse.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

// 1. Add native ASP.NET Core MVC services (Controllers and Views)
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

// 2. Add an HTTP Client tool so our background service can talk to the NOAA server
builder.Services.AddHttpClient<SpaceWeatherIngestionService>();

// 3. Register our space weather engine to run automatically in the background
builder.Services.AddHostedService<SpaceWeatherIngestionService>();

builder.Services.AddDbContext<SkyPulseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("SkyPulse.Web")));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 4. Map the default routing scheme so the browser knows to load HomeController on startup
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<TelemetryHub>("/hubs/telemetry");






app.Run();