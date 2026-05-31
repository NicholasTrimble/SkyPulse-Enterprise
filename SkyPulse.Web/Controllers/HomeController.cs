using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SkyPulse.Infrastructure.Services;
using SkyPulse.Web.Models;

namespace SkyPulse.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SkyPulseDbContext _dbContext; // ──> Inject our database gateway

        public HomeController(ILogger<HomeController> logger, SkyPulseDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }


        public IActionResult BiReport()
        {
            return View();
        }



        // ──> NEW HISTORICAL ANALYTICS ROUTE
        public IActionResult Analytics()
        {
            // 1. Pull all snapshots from SQL Server, ordered by newest first
            var allSnapshots = _dbContext.TelemetrySnapshots
                .OrderByDescending(s => s.TimeTag)
                .ToList();

            // 2. Compute high-level analytical business metrics safely
            ViewBag.TotalRecords = allSnapshots.Count;
            ViewBag.MaxRiskScore = allSnapshots.Any() ? allSnapshots.Max(s => s.EnterpriseRiskScore) : 0.0;
            ViewBag.AvgProtonSpeed = allSnapshots.Any() ? Math.Round(allSnapshots.Average(s => s.ProtonSpeed), 2) : 0.0;
            ViewBag.AvgProtonDensity = allSnapshots.Any() ? Math.Round(allSnapshots.Average(s => s.ProtonDensity), 2) : 0.0;

            // 3. Pass the full list of historical records to the view model
            return View(allSnapshots);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}