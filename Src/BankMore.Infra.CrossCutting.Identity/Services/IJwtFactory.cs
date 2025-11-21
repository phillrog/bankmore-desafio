using System.Security.Claims;

namespace BankMore.Infra.CrossCutting.Identity.Services;

public interface IJwtFactory
{
    Task<JwtToken> GenerateJwtToken(ClaimsIdentity claimsIdentity);
}
