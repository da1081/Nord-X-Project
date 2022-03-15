using Nord_X_WebApp.Data.Entities;
using Nord_X_WebApp.Data.Repositories;

namespace Nord_X_WebApp.Data
{
    public interface IUnitOfWork
    {
        DataContext Context { get; }
        GenericRepository<Company> CompanyRepository { get; }
        GenericRepository<Measurement> MeasurementRepository { get; }
        GenericRepository<MeasurementType> MeasurementTypeRepository { get; }
        GenericRepository<Sensor> SensorRepository { get; }

        Task<int> SaveAsync();
        int Save();
    }
}