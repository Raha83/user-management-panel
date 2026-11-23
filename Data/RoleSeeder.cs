using ManagementPanel.Models;
using Microsoft.AspNetCore.Identity;

namespace ManagementPanel.Data
{
    public class RoleSeeder()
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            String[] roles = { "Admin", "User", "Manager" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new Role
                    {
                        Name = role
                    });
                }
            }
        }
    }

}
