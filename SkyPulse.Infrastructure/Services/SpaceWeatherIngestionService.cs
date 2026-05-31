using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using SkyPulse.Core.Models;

namespace SkyPulse.Infrastructure.Services
{
    public class SpaceWeatherIngestionService : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SpaceWeatherIngestionService> _logger;
        private readonly IHubContext<TelemetryHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;

        private const string WindApiUrl = "https://services.swpc.noaa.gov/json/rtsw/rtsw_wind_1m.json";

        // 2. Add IServiceScopeFactory to your constructor parameters
        public SpaceWeatherIngestionService(
            HttpClient httpClient,
            ILogger<SpaceWeatherIngestionService> logger,
            IHubContext<TelemetryHub> hubContext,
            IServiceScopeFactory scopeFactory)
        {
            _httpClient = httpClient;
            _logger = logger;
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SkyPulse Telemetry Daemon initialized successfully.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Polling live NOAA satellite data arrays...");

                    var rawData = await _httpClient.GetFromJsonAsync<List<NoaaWindEntry>>(WindApiUrl, stoppingToken);

                    if (rawData != null && rawData.Any())
                    {
                        var latestEntry = rawData.Last();

                        double safeSpeed = latestEntry.proton_speed ?? 0.0;
                        double safeDensity = latestEntry.proton_density ?? 0.0;
                        double safeTemp = latestEntry.proton_temperature ?? 0.0;

                        double calculatedScore = CalculateRiskScore(safeSpeed, safeDensity);
                        string riskText = DetermineRiskLevel(calculatedScore);

                        var snapshot = new TelemetrySnapshot
                        {
                            TimeTag = DateTime.UtcNow,
                            SatelliteSource = latestEntry.source ?? "DSCOVR",
                            ProtonSpeed = safeSpeed,
                            ProtonDensity = safeDensity,
                            ProtonTemperature = safeTemp,
                            EnterpriseRiskScore = calculatedScore,
                            RiskLevel = riskText
                        };

                        _logger.LogInformation($"Telemetry calculated successfully! Current state: {snapshot.RiskLevel} (Score: {snapshot.EnterpriseRiskScore})");

                        // ──> 3. OPEN A TEMPORARY GATEWAY TO THE DATABASE AND SAVE THE SNAPSHOT
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<SkyPulseDbContext>();
                            dbContext.TelemetrySnapshots.Add(snapshot);
                            await dbContext.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation("Telemetry snapshot successfully persisted to SQL Server.");
                        }

                        // 4. Stream it live over WebSockets to open browser windows
                        await _hubContext.Clients.All.SendAsync("ReceiveLatestTelemetry", snapshot);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Telemetry fetch pass failed.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private double CalculateRiskScore(double speed, double density)
        {
            // 1. Speed Weight (75% Max Weight): Normalized against a max super-storm speed of 1500 km/s
            double speedWeight = Math.Min((speed / 1500.0) * 75.0, 75.0);

            // 2. Density Weight (25% Max Weight): Normalized against a max super-storm density of 50 n/cc
            double densityWeight = Math.Min((density / 50.0) * 25.0, 25.0);

            // 3. Combined Risk Index: Safely totals between 0.00 and 100.00
            return Math.Round(speedWeight + densityWeight, 2);
        }

        private string DetermineRiskLevel(double score) => score switch
        {
            > 75 => "Critical",
            > 50 => "High",
            > 25 => "Elevated",
            _ => "Normal"
        };

        private class NoaaWindEntry
        {
            public string? time_tag { get; set; }
            public string? source { get; set; }
            public double? proton_speed { get; set; }
            public double? proton_density { get; set; }
            public double? proton_temperature { get; set; }
        }
    }
}