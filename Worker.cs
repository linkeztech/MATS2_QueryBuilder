using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Newtonsoft.Json.Linq;

namespace MATS2_QueryBuilder
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public Worker (IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker> logger) =>
        (_hostApplicationLifetime, _logger) = (hostApplicationLifetime, logger);

       
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start  {time}", DateTimeOffset.Now);

            //This line is to check which code version is getting used 
            Console.WriteLine("Welcome to MATS Querybuilder - {0}", "V.0.1");

            await base.StartAsync(cancellationToken);
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // DO YOUR STUFF HERE

            _logger.LogInformation("Stop  {time}", DateTimeOffset.Now);
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}