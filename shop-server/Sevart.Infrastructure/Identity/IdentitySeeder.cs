using Microsoft.AspNetCore.Identity;

namespace Sevart.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var roleName in ApplicationRoles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result =
                await roleManager.CreateAsync(
                    new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName
                    });

            if (result.Succeeded)
            {
                continue;
            }

            var errors = string.Join(
                " | ",
                result.Errors.Select(
                    error => error.Description));

            throw new InvalidOperationException(
                $"Creating role '{roleName}' failed: {errors}");
        }
    }
}