using Nord_X_WebApp.Hangfire.HangfireControllers;

namespace Nord_X_WebApp.Hangfire.Interfaces
{
    public interface IHangfireJobController
    {
        void QueueAll();
        void QueueSensorScannerJob(BaseJobController.JobType jobType = BaseJobController.JobType.FireAndForget, string? cronExpresion = "0 0 12 * * ?");
    }
}
