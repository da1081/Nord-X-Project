using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nord_X_WebApp.Data.Entities;

// Migration creation and DB-Update Notes:
// Refrence: https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli
//      1.  Add-Migration InitialCreate
//      2.  Update-Database

#pragma warning disable CS8618
namespace Nord_X_WebApp.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Company { get; set; }
        public DbSet<Measurement> Measurement { get; set; }
        public DbSet<MeasurementType> MeasurementType { get; set; }
        public DbSet<Sensor> Sensor { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /* Some prefere table names in singular format... We do not.
                modelBuilder.Entity<SomeEntity>().ToTable("SomeEntity");
            */

            // Configure database default values.
            /* TODO: Remove this when correctly implementet on override save methods.
            modelBuilder.Entity<Company>().Property(entity => entity.IsActive).HasDefaultValue(true);
            modelBuilder.Entity<Measurement>().Property(entity => entity.IsActive).HasDefaultValue(true);
            modelBuilder.Entity<MeasurementType>().Property(entity => entity.IsActive).HasDefaultValue(true);
            modelBuilder.Entity<Sensor>().Property(entity => entity.IsActive).HasDefaultValue(true);
            */
        }

        // Override SaveChangesAsync and SaveChanges to apply date auditing properties.
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyDateAudit();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyDateAudit();
            return base.SaveChanges();
        }

        // Fill out DateAudit Fields.
        private void ApplyDateAudit()
        {
            var now = DateTime.UtcNow;
            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity.GetType().IsSubclassOf(typeof(BaseEntity)))
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            changedEntity.Property("IsActive").CurrentValue = true;
                            changedEntity.Property("AddedDate").CurrentValue = now;
                            changedEntity.Property("ModifiedDate").CurrentValue = now;
                            break;
                        case EntityState.Modified:
                            changedEntity.Property("ModifiedDate").CurrentValue = now;
                            break;
                    }
                }
            }
        }
    }
}