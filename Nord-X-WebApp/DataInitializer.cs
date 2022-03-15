using Microsoft.AspNetCore.Identity;
using Nord_X_WebApp.Hangfire.Interfaces;

namespace Nord_X_WebApp
{
    public class DataInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHangfireJobController _jobController;

        public DataInitializer(IServiceProvider serviceProvider)
        {
            _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            _jobController = serviceProvider.GetRequiredService<IHangfireJobController>();
        }

        public async Task AddDefaultRoles(string[]? roleNames = null)
        {
            if (roleNames is null)
                roleNames = new string[] { "Administrator" };

            foreach (string roleName in roleNames)
            {
                bool roleExsists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExsists)
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public async Task AddDefaultUser()
        {
            string email = _configuration["DefaultAdminUser:Mail"];
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                string password = _configuration["DefaultAdminUser:Pass"];
                var result = await _userManager.CreateAsync(new IdentityUser { UserName = email, Email = email, EmailConfirmed = true }, password);
                if (result.Succeeded)
                {
                    user = await _userManager.FindByEmailAsync(email);
                    await _userManager.AddToRoleAsync(user, "Administrator");
                }
            }
        }

        public void QueueHangfireJobs()
        {
            _jobController.QueueAll();
        }
    }
}
