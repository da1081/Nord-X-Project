using Hangfire;
using Nord_X_WebApp.Hangfire.Interfaces;
using Nord_X_WebApp.Hangfire.Jobs;

namespace Nord_X_WebApp.Hangfire.HangfireControllers
{
    public class HangfireJobController : BaseJobController, IHangfireJobController
    {
        private readonly ILogger<HangfireJobController> _logger;
        public HangfireJobController(ILogger<HangfireJobController> logger)
        {
            _logger = logger;
        }

        public void QueueAll()
        {
            _logger.LogInformation("Queueing all hangfire jobs.");

            QueueSensorScannerJob(JobType.Requiring);

            _logger.LogInformation("All hangfire jobs queued.");
        }

        public void QueueSensorScannerJob(JobType jobType = JobType.FireAndForget, string? cronExpresion = "0 0 12 * * ?")
        {
            switch (jobType)
            {
                case JobType.Requiring:
                    if (cronExpresion == null)
                        throw new ArgumentNullException(nameof(cronExpresion));
                    RecurringJob.AddOrUpdate<SensorScannerJob>(job => job.RunAsync(), cronExpresion);
                    break;
                case JobType.FireAndForget:
                    BackgroundJob.Enqueue<SensorScannerJob>(job => job.RunAsync());
                    break;
                default:
                    throw new ArgumentNullException(nameof(jobType));
            }
        }
    }
}
