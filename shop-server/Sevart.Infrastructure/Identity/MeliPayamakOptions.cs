
namespace Sevart.Infrastructure.Identity;

public sealed class MeliPayamakOptions
{
    public const string SectionName = "MeliPayamak";

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int BodyId { get; set; }
}
