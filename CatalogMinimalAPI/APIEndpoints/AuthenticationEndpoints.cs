using CatalogMinimalAPI.Models;
using CatalogMinimalAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace CatalogMinimalAPI.APIEndpoints {
    public static class AuthenticationEndpoints {
        public static void MapAuthenticationEndpoints(this WebApplication app) {
            app.MapPost("/login", [AllowAnonymous] (User user, ITokenService tokenService) => {
                if (user == null) {
                    return Results.BadRequest("Login Inválido");
                }

                if (user.Username == "jackson" && user.Password == "1234") {
                    var tokenString = tokenService.GenerateToken(app.Configuration["Jwt:Key"],
                        app.Configuration["Jwt:Issuer"],
                        app.Configuration["Jwt:Audience"],
                        user);

                    return Results.Ok(new { token = tokenString });
                }
                else {
                    return Results.BadRequest("Login Inválido");
                }
            }).Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status200OK).WithName("Login").WithTags("Authentication");
        }
    }
}
