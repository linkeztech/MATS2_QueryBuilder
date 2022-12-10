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


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(1000, stoppingToken);

                    //string str = GlobalVar.DBOpp.GetDBConnectionString();
                    // When completed, the entire app host will stop.
                    _hostApplicationLifetime.StopApplication();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }

        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start  {time}", DateTimeOffset.Now);

            //This line is to check which code version is getting used 
            Console.WriteLine("Welcome to MATS Querybuilder - {0}", "V.0.1");
        }
    }
}