using Nord_X_WebApp.Data.Entities;
using Nord_X_WebApp.Data.Repositories;

/// The unit of work class serves one purpose: to make sure that when you use multiple repositories, they share a single database context. 
/// That way, when a unit of work is complete you can call the SaveChanges method on that instance of the context and be assured that all related changes will be coordinated. 
/// All that the class needs is a Save method and a property for each repository.
/// Find more here: https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application#creating-the-unit-of-work-class

namespace Nord_X_WebApp.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private GenericRepository<Company>? companyRepository;
        private GenericRepository<Sensor>? sensorRepository;
        private GenericRepository<MeasurementType>? measurementTypeRepository;
        private GenericRepository<Measurement>? measurementRepository;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        public DataContext Context
        {
            get { return _context; }
        }

        public GenericRepository<Company> CompanyRepository
        {
            get
            {
                if (companyRepository is null)
                    companyRepository = new GenericRepository<Company>(_context);
                return companyRepository;
            }
        }

        public GenericRepository<Sensor> SensorRepository
        {
            get
            {
                if (sensorRepository is null)
                    sensorRepository = new GenericRepository<Sensor>(_context);
                return sensorRepository;
            }
        }

        public GenericRepository<MeasurementType> MeasurementTypeRepository
        {
            get
            {
                if (measurementTypeRepository is null)
                    measurementTypeRepository = new GenericRepository<MeasurementType>(_context);
                return measurementTypeRepository;
            }
        }

        public GenericRepository<Measurement> MeasurementRepository
        {
            get
            {
                if (measurementRepository is null)
                    measurementRepository = new GenericRepository<Measurement>(_context);
                return measurementRepository;
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
