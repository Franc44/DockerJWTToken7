using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ExampleJTToken7.Services;

public class TokenService 
{
    private const int ExpirationMinutes = 30;
    public string CreateToken(IdentityUser user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration           
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, 
        DateTime expiration) => new(
            "superpaco",
            "superpaco",
            claims,
            expires: expiration,
            signingCredentials : credentials
    );

    private List<Claim> CreateClaims(IdentityUser user)
    {
        try
        {
            var claims =  new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "TokenForThisApi"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };
            return claims;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
   }

   private SigningCredentials CreateSigningCredentials()
   {
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("$S%u&p0e0r0S0e?c!r0e0t")
            ),
            SecurityAlgorithms.HmacSha256
        );
   }

}