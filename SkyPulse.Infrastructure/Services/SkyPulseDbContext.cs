using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using SkyPulse.Core.Models;

namespace SkyPulse.Infrastructure.Services
{
    public class SkyPulseDbContext : DbContext
    {
        public SkyPulseDbContext(DbContextOptions<SkyPulseDbContext> options) : base(options)
        {
        }

        // Define your DbSets here
        // public DbSet<YourEntity> YourEntities { get; set; }
        public DbSet<TelemetrySnapshot> TelemetrySnapshots { get; set; } = null!;
    }
}
