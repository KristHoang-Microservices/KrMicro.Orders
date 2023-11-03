using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KrMicro.Core.CQS.Command.Jwt;
using Microsoft.IdentityModel.Tokens;
using JwtConstants = KrMicro.Core.Constants.JwtConstants;

namespace KrMicro.Core.Services;

public static class JwtService
{
    public static string GenerateToken(GenerateJwtCommandRequest user, string? jwtIssuer = null,
        string? jwtAudience = null)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConstants.JwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim("username", user.UserName),
            new Claim("role", user.Role)
        };
        var token = new JwtSecurityToken(
            jwtIssuer,
            jwtAudience,
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}