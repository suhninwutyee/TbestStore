using Microsoft.AspNetCore.Identity;
using MVCtesting.Models;
using System;
using System.Threading.Tasks;

public class RoleInitializer
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleInitializer(RoleManager<IdentityRole> roleManager,
                           UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task CreateRolesAsync()
    {
        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create role: {role}");
                }
            }
        }

        string adminEmail = "admin@example.com";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(newAdmin, "Admin123!");

            if (!createResult.Succeeded)
            {
                throw new Exception("Failed to create default admin user.");
            }

            await _userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}
