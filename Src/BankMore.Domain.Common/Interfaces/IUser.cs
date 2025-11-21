using System.Security.Claims;

namespace BankMore.Domain.Common.Interfaces;

public interface IUser
{
    string Name { get; }

    bool IsAuthenticated();

    IEnumerable<Claim> GetClaimsIdentity();

    string Conta { get; }
}
