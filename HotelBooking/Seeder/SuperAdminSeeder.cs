using HotelBooking.Interfaces;
using HotelBooking.Models;
using HotelBooking.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

public static class SeedSuperAdmin
{
    public static async Task SeedAsync(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        string email = "superadmin@gmail.com";
        string password = "Admin@123";

        if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));

        var super = await userManager.FindByEmailAsync(email);

        if (super == null)
        {
            super = new User
            {
                UserName = email,
                Email = email,
                Name = "Super Administrator"
            };

            var result = await userManager.CreateAsync(super, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(super, "SuperAdmin");
                var admin = new Admin
                {
                    UserId = super.Id,
                    Role = "SuperAdmin",
                    Salary = 50000
                };

                unitOfWork.Admins.Insert(admin);
            }
        }
    }
}