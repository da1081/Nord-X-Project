using Hangfire;
using Nord_X_WebApp.Data;
using Nord_X_WebApp.Data.Entities;
using Nord_X_WebApp.Hangfire.Interfaces;

namespace Nord_X_WebApp.Hangfire.Jobs
{
    public class MeasurementResponseModel
    {
        public DateTime Time { get; set; }
        public string Measure { get; set; } = string.Empty;
    }

    public class SensorScannerJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;

        public SensorScannerJob(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Queue("default")]
        [AutomaticRetry(Attempts = 0)]
        [JobDisplayName("Sensor Data Retrieval")]
        public async Task RunAsync()
        {
            bool doSave = false;

            var sensors = await _unitOfWork.SensorRepository.GetAllAsync(
                includeProperties: new string[] { "MeasurementType", "DataPoints" },
                orderBy: x => x.OrderBy(y => y.Name));

            if (sensors.Any())
                using (HttpClient httpClient = new())
                {
                    foreach (var sensor in sensors)
                    {
                        try
                        {
                            using HttpResponseMessage response = await httpClient.GetAsync(sensor.Endpoint);
                            response.EnsureSuccessStatusCode();
                            var result = await response.Content.ReadFromJsonAsync<List<MeasurementResponseModel>>();
                            if (result is not null)
                            {
                                if (sensor.IsFailing == true)
                                {
                                    sensor.IsFailing = false;
                                    _unitOfWork.SensorRepository.Update(sensor);
                                    if (!doSave)
                                        doSave = true;
                                }
                                foreach (MeasurementResponseModel measurementResponseModel in result)
                                {
                                    if (!sensor.DataPoints.Any(x => x.Measure == measurementResponseModel.Measure
                                        && x.OriginRegTime == measurementResponseModel.Time))
                                    {
                                        Measurement measurement = new()
                                        {
                                            Measure = measurementResponseModel.Measure,
                                            OriginRegTime = measurementResponseModel.Time,
                                            SensorId = sensor.Id
                                        };
                                        sensor.DataPoints.Add(measurement);
                                        _unitOfWork.SensorRepository.Update(sensor);
                                        if (!doSave)
                                            doSave = true;
                                    }
                                }
                            }
                        }
                        catch (HttpRequestException)
                        {
                            sensor.IsFailing = true;
                            _unitOfWork.SensorRepository.Update(sensor);
                            if (!doSave)
                                doSave = true;
                            continue;
                        }
                    }
                }

            if (doSave)
                await _unitOfWork.SaveAsync();
        }
    }
}
