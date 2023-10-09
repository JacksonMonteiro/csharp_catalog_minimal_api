using CatalogMinimalAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CatalogMinimalAPI.Services {
    public interface ITokenService {
        string GenerateToken(string key, string issuer, string audience, User user);

    }
}
