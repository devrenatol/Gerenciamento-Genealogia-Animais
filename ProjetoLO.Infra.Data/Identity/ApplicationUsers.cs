using Microsoft.AspNetCore.Identity;

namespace ProjetoLO.Infra.Data.Identity;

public class ApplicationUsers : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
