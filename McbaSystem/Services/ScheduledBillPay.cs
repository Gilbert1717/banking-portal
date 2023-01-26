using McbaSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace McbaSystem.Services
{
    public class ScheduledBillPay : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ScheduledBillPay> _logger;


        public ScheduledBillPay(IServiceProvider services, ILogger<ScheduledBillPay> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Service is running.");

            while (!cancellationToken.IsCancellationRequested)
            {
                await DoBillPay(cancellationToken);

                _logger.LogInformation("Background Service is waiting a minute.");

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        private async Task DoBillPay(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Service work start.");

            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<McbaContext>();
            var now = DateTime.Now.ToUniversalTime();
            MenuService menuService = new MenuService(context);
            var billPays = await context.BillPays
                .Where(pay => pay.ScheduleTimeUtc < now && pay.ErrorMessage == null)
                .ToListAsync(cancellationToken);
            foreach (var billPay in billPays)
            {
                menuService.BillPayExecute(billPay);
            }

            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Background Service work complete.");
        }
    }
}