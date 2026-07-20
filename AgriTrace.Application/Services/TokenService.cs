using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AgriTrace.Application.Services;

public class TokenService : ITokenService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryMinutes;

    public TokenService(
        IConfiguration configuration)
    {
        var jwt = configuration.GetSection("Jwt");

        _secret = jwt["Secret"] ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
        _issuer = jwt["Issuer"] ?? "AgriTrace";
        _audience = jwt["Audience"] ?? "AgriTrace";
        _expiryMinutes = int.TryParse(jwt["ExpiryMinutes"], out var m) ? m : 60;
    }

    public string GenerateAccessToken(
        User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_secret));

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("role", user.Role.ToString())
        };

        if (user.OrganizationId.HasValue)
        {
            claims.Add(new("organizationId", user.OrganizationId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
    }

    public ClaimsPrincipal? ValidateToken(
        string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_secret)),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            return tokenHandler.ValidateToken(
                token,
                validationParameters,
                out _);
        }
        catch
        {
            return null;
        }
    }
}
