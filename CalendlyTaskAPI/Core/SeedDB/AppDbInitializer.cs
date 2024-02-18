using CalendlyTaskAPI.Core.Consts;
using CalendlyTaskAPI.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace CalendlyTaskAPI.Core.SeedDB
{
    public static class AppDbInitializer
    {
        public static async Task SeedDB(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(StaticUserRoles.ADMIN))
                    await roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));

                if (!await roleManager.RoleExistsAsync(StaticUserRoles.USER))
                    await roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                if (await userManager.FindByEmailAsync("william_asiama@mail.com") is null)
                {
                    ApplicationUser adminUser = new ApplicationUser()
                    {
                        FullName = "William Asiama",
                        Email = "william_asiama@mail.com",
                        UserName = "WilliamAsiama",
                        SecurityStamp = Guid.NewGuid().ToString(),
                    };

                    await userManager.CreateAsync(adminUser, "123");

                    await userManager.AddToRoleAsync(adminUser, StaticUserRoles.ADMIN);
                }
            }
        }
    }
}
