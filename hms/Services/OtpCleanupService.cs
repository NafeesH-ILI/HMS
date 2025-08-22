using hms.Repos;
using hms.Repos.Interfaces;
using hms.Utils;
using System.Timers;

namespace hms.Services
{
    public class OtpCleanupService(
        ILogger<OtpCleanupService> logger,
        IServiceProvider services) : IHostedService, IDisposable
    {
        private readonly ILogger<OtpCleanupService> _logger = logger;
        private readonly IServiceProvider _services = services;
        private System.Timers.Timer? _timer = null;

        private async Task Fire()
        {
            using var scope = _services.CreateScope();
            IPassResetRepository? otpRepo = scope.ServiceProvider.GetService<IPassResetRepository>();
            if (otpRepo == null)
            {
                _logger.LogError("Failed to GetService<IPassResetRepository>");
                return;
            }
            _logger.LogInformation("OtpCleanup starting");
            await otpRepo.Cleanup();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OtpCleanupService starting");
            TimeSpan duration = TimeSpan.FromMinutes(Consts.OtpCleanupMinutes);
            _timer = new System.Timers.Timer(duration);
            _timer.Elapsed += async (_, _) => await Fire();
            _timer.AutoReset = true;
            _timer.Enabled = true;
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OtpCleanupService stopping");
            _timer?.Stop();
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
