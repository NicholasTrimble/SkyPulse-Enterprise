using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkyPulse.Core.Models
{
    [Table("TelemetrySnapshots")]
    public class TelemetrySnapshot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime TimeTag { get; set; }

        [Required]
        [StringLength(50)]
        public string SatelliteSource { get; set; } = "DSCOVR";

        // Raw Space Weather Telemetry
        public double ProtonSpeed { get; set; }       // Measured in km/s
        public double ProtonDensity { get; set; }     // Measured in particles/cm³
        public double ProtonTemperature { get; set; } // Measured in Kelvin

        // Custom Analytics Layer for Power BI and UI
        [Required]
        [StringLength(20)]
        public string RiskLevel { get; set; } = "Normal"; // Normal, Elevated, High, Critical

        public double EnterpriseRiskScore { get; set; } // Custom calculated index (0 to 100)
    }
}