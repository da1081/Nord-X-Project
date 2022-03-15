using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nord_X_WebApp;
using Nord_X_WebApp.Data;
using Nord_X_WebApp.Hangfire.HangfireControllers;
using Nord_X_WebApp.Hangfire.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DevelopmentExpressConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddControllersWithViews();


// Add Hangfire.
builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

// Add hangfire default parallel execution queue server.
builder.Services.AddHangfireServer(options =>
{
    options.ServerName = $"{Environment.MachineName}:default";
    options.Queues = new[] { "default" };
});

/* Serial execution is currently not i use.
// Add hangfire serial execution queue server.
builder.Services.AddHangfireServer(options =>
{
options.ServerName = $"{Environment.MachineName}:serial";
options.Queues = new[] { "serial" };
options.WorkerCount = 1;
});
*/

// DEPENDENCY INJECTION SETUP:
// Add Scoped dependency injection.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IHangfireJobController, HangfireJobController>();

// Add Transient dependency injection.
// Nope

// Add Singleton dependency injection.
// Nope

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    // Add Hangfire dashboard.
    app.UseHangfireDashboard();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


// Initialize seed data.
using (var scope = app.Services.CreateScope())
{
    IServiceProvider serviceProvider = scope.ServiceProvider;

    DataContext context = serviceProvider.GetRequiredService<DataContext>();
    bool dbCreated = context.Database.EnsureCreated();

    DataInitializer dataInitializer = new(serviceProvider);
    await dataInitializer.AddDefaultRoles();
    await dataInitializer.AddDefaultUser();
    dataInitializer.QueueHangfireJobs();

}

app.Run();