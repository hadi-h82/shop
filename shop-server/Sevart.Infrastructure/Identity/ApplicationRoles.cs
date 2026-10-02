namespace Sevart.Infrastructure.Identity;

public static class ApplicationRoles
{
    public const string Admin = "Admin";

    public const string Customer = "Customer";

    public static readonly string[] All =
    [
        Admin,
        Customer
    ];
}