using System.Security.Claims;

namespace StarSolitaireTournament.Authentication
{
    public interface IJwtTokenManager
    {
        string GenerateJwtToken(string id, string role);
        ClaimsPrincipal GetPrincipals(string jwtToken);
        string GenerateRegistrationToken(string phoneNumber, string countryCode, string role);
        ClaimsPrincipal GetRegistrationPrincipals(string jwtToken);
    }
}
