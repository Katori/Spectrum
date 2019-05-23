using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpectrumCore.Models;
using Microsoft.Extensions.Logging;

namespace SpectrumCore.Services
{
    public class ServerTimeChecker : BackgroundService
    {
        public IServiceScopeFactory _serviceScopeFactory;
        public ILogger<ServerTimeChecker> _logger;

        public ServerTimeChecker(IServiceScopeFactory serviceScopeFactory, ILoggerFactory logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger.CreateLogger<ServerTimeChecker>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
           _logger.LogInformation("Starting time checker");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true && !stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    ServersContext scoped = scope.ServiceProvider.GetRequiredService<ServersContext>();
                    foreach (var item in scoped.Set<Server>())
                    {
                        if (System.DateTime.Now.ToUniversalTime() - item.LastCheckedIn > System.TimeSpan.FromSeconds(5))
                        {
                            _logger.LogInformation("Removing server "+item.ConnectionString+" because it didn't check in on time");
                            scoped.Remove(item);
                        }
                    }
                    await scoped.SaveChangesAsync();
                    scoped.Dispose();
                }
                await Task.Delay(5000);
            }
        }


    }
}
