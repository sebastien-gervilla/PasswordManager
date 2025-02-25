using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Api.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _secretKey;

    public AuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _secretKey = configuration["Jwt:Key"];
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            AttachUserIdToContext(context, token);
        }

        await _next(context);
    }

    private void AttachUserIdToContext(HttpContext context, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.NameId).Value);

            context.Items["UserId"] = userId; // Attach user ID to HttpContext
        }
        catch
        {
            // Do nothing if the token is invalid
            // The request won't have access to the user ID
        }
    }
}