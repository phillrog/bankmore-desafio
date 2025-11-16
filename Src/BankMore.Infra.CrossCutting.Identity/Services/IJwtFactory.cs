using System.Security.Claims;
using System.Threading.Tasks;

namespace BankMore.Infra.CrossCutting.Identity.Services;

public interface IJwtFactory
{
    Task<JwtToken> GenerateJwtToken(ClaimsIdentity claimsIdentity);
}
