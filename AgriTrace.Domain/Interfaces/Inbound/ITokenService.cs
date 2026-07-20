using System.Security.Claims;
using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface ITokenService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();

    ClaimsPrincipal? ValidateToken(string token);
}
