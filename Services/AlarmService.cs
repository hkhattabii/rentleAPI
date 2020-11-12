using DnsClient.Internal;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RentleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RentleAPI.Services
{
    public class AlarmService : BackgroundService
    {
        private readonly IHubContext<AlarmHub> _hubContext;
        private readonly LeaseService _leaseService;

        public AlarmService(IHubContext<AlarmHub> hubContext, IRentleDatabaseSettings settings)
        {
            _hubContext = hubContext;
            _leaseService = new LeaseService(settings);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Lease> leases;
            while (!stoppingToken.IsCancellationRequested)
            {
                leases = _leaseService.FindAlarms(DateTime.Now);
                await _hubContext.Clients.All.SendAsync("sendAlarm", leases);
                await Task.Delay(10000);
            }
        }

    }
}
