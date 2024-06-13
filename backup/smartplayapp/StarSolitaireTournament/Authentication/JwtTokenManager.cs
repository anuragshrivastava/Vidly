using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Constants;

namespace StarSolitaireTournament.Authentication
{
    public class JwtTokenManager : IJwtTokenManager
    {
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region generate token

        public string GenerateJwtToken(string id, string role)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, id), new Claim(ClaimTypes.Role, role) };
            var credentials = new SigningCredentials(JwtToken.SECURITY_KEY, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(JwtToken.ISSUER, JwtToken.AUDIENCE, claims, expires: DateTime.Now.AddDays(JwtToken.EXPIRE_IN_DAYS), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region validation

        public ClaimsPrincipal GetPrincipals(string jwtToken)
        {
            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = JwtToken.AUDIENCE;
            validationParameters.ValidIssuer = JwtToken.ISSUER;
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtToken.SECURITY_KEY_VALUE));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region generate token

        public string GenerateRegistrationToken(string phoneNumber, string countryCode, string role)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, phoneNumber), new Claim(ClaimTypes.Country, countryCode), new Claim(ClaimTypes.Role, role) };
            var credentials = new SigningCredentials(JwtToken.SECURITY_KEY, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(JwtToken.ISSUER, JwtToken.AUDIENCE, claims, expires: DateTime.Now.AddDays(JwtToken.EXPIRE_IN_DAYS), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region validation

        public ClaimsPrincipal GetRegistrationPrincipals(string jwtToken)
        {
            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = JwtToken.AUDIENCE;
            validationParameters.ValidIssuer = JwtToken.ISSUER;
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtToken.SECURITY_KEY_VALUE));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
}
