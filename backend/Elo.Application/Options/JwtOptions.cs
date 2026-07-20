using Elo.Domain.Enums;

namespace Elo.Application.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = "sistema-elo";
    public string Audience { get; set; } = "sistema-elo";
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
