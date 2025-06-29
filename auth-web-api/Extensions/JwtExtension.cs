using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
namespace AuthWebApi.Extensions;
public static class JwtExtension
{
    public static void AddJwtAuthorization(this IServiceCollection serCollection)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string jwtKey = configuration.GetSection("JwtSettings").GetValue<string>("Token") ?? "";

        if (string.IsNullOrEmpty(jwtKey))
            throw new Exception("JWT Key is not found in appsettings.json");


        var keyInBytes = Encoding.UTF8.GetBytes(jwtKey);

        serCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration.GetSection("JwtSettings").GetValue<string>("Issuer") ?? "",
                ValidAudience = configuration.GetSection("JwtSettings").GetValue<string>("Audience") ?? "",
                IssuerSigningKey = new SymmetricSecurityKey(keyInBytes),
                RoleClaimType = ClaimTypes.Role,
            };
        });
    }
}


