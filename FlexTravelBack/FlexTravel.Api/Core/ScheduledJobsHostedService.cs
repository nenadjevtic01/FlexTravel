using FlexTravel.Application.Jobs;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Jobs;

namespace FlexTravel.Api.Core
{
    public class ScheduledJobsHostedService : BackgroundService, IJobSchedule
    {
        private readonly IServiceScopeFactory _scopes;
        private readonly ILogger<ScheduledJobsHostedService> _logger;
        private readonly ScheduledJobsSettings _settings;

        public ScheduledJobsHostedService(IServiceScopeFactory scopes, ILogger<ScheduledJobsHostedService> logger, AppSettings settings)
        {
            _scopes = scopes;
            _logger = logger;
            _settings = settings.ScheduledJobs ?? new ScheduledJobsSettings();
        }

        public int IntervalMinutes => _settings.IntervalMinutes;
        public DateTime? NextRunAt { get; private set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_settings.IntervalMinutes <= 0)
            {
                _logger.LogInformation("Background jobs are switched off (ScheduledJobs:IntervalMinutes = 0).");
                return;
            }

            try
            {
                var delay = TimeSpan.FromSeconds(Math.Max(0, _settings.StartDelaySeconds));
                NextRunAt = DateTime.UtcNow.Add(delay);
                await Task.Delay(delay, stoppingToken);

                using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_settings.IntervalMinutes));
                do
                {
                    RunOnce();
                    NextRunAt = DateTime.UtcNow.AddMinutes(_settings.IntervalMinutes);
                }
                while (await timer.WaitForNextTickAsync(stoppingToken));
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void RunOnce()
        {
            try
            {
                using var scope = _scopes.CreateScope();
                var run = scope.ServiceProvider.GetRequiredService<ScheduledJobs>().Run("Schedule", DateTime.Now);

                _logger.LogInformation(
                    "Background jobs: {Expired} expired, {Reminders} payment reminders, {Invitations} review invitations, {Alerts} price alerts, {Waitlists} waitlists closed{Error}",
                    run.ExpiredReservations, run.PaymentReminders, run.ReviewInvitations, run.PriceAlerts, run.ClosedWaitlists,
                    run.Error == null ? "" : ". Error: " + run.Error);
            }
            catch (ConflictException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background jobs failed.");
            }
        }
    }
}
