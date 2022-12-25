using BusinessLogic.Abstractions;
using BusinessLogic.Extensions;
using BusinessLogic.Options;
using DataAccess.Entities;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BusinessLogic.Services;

public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly UserManager<AppUser> _userManager;
    private readonly SymmetricSecurityKey _key;
    public TokenService(
        IOptions<JwtOptions> options, 
        TokenValidationParameters tokenValidationParameters,
        UserManager<AppUser> userManager)
    {
        _jwtOptions = options.Value;
        _userManager = userManager;
        _key = new SymmetricSecurityKey(_jwtOptions.Key.ToByteArray());
    }

    public async Task<Result<string>> CreateTokenAsync(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(_jwtOptions.LifetimeMinutes)),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Result.Ok(tokenHandler.WriteToken(token));
    }
}
