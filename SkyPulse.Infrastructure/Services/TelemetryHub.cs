using Microsoft.AspNetCore.SignalR;
using SkyPulse.Core.Models;
using System.Threading.Tasks;

namespace SkyPulse.Infrastructure.Services // <── CHECK THIS EXACT STRING
{
    public class TelemetryHub : Hub
    {
        public async Task BroadcastTelemetry(TelemetrySnapshot snapshot)
        {
            await Clients.All.SendAsync("ReceiveLatestTelemetry", snapshot);
        }
    }
}