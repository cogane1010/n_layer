using System;
using System.Threading;
using System.Threading.Tasks;
using App.BookingOnline.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.BookingOnline.MobileApi.BackgroudService
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> logger;

        private readonly IServiceProvider services;

        private Timer _timer;

        private Task _executingTask;

        private CancellationTokenSource _stoppingCts;
        public IConfiguration Configuration { get; }
        public TimedHostedService(IServiceProvider services, IConfiguration configuration,
            ILogger<TimedHostedService> logger) =>
            (this.services, this.Configuration, this.logger) = (services, configuration, logger);

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var lockTeetime = Configuration.GetSection("loginTime").GetValue<int>("TimeServiceInterval");
            _timer = new Timer(FireTask, null, TimeSpan.FromSeconds(lockTeetime), TimeSpan.FromSeconds(lockTeetime));

            return Task.CompletedTask;
        }

        private void FireTask(object state)
        {
            if (_executingTask == null || _executingTask.IsCompleted)
            {
                //logger.LogInformation("[BackgroundService] No task is running, check for new job");
                _executingTask = ExecuteNextJobAsync(_stoppingCts.Token);
            }
            else
            {
                // logger.LogInformation("[BackgroundService] There is a task still running, wait for next cycle");
            }
        }

        private async Task ExecuteNextJobAsync(CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookingOnlineDbContext>();
            // whatever logic to retrieve the next job
           
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);

            if (_executingTask == null || _executingTask.IsCompleted)
            {
                return;
            }
            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
            _stoppingCts?.Cancel();
        }
    }
}