using hms.Services.Interfaces;
using hms.Utils;

namespace hms.Services
{
    public class AppointmentAutoCancellationService(
        ILogger<AppointmentAutoCancellationService> logger,
        IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly ILogger<AppointmentAutoCancellationService> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                IAppointmentService? apptService = scope.ServiceProvider.GetService<IAppointmentService>();
                if (apptService == null)
                {
                    _logger.LogError("Failed to GetService<IAppointmentService>");
                    await Task.Delay(TimeSpan.FromMinutes(Consts.ApptAutoCancelMinutes), stoppingToken);
                    continue;
                }
                await apptService.AutoCancel();
                await Task.Delay(TimeSpan.FromMinutes(Consts.ApptAutoCancelMinutes), stoppingToken);
            }
        }
    }
}
