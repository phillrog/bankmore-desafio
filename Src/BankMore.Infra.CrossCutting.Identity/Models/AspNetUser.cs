namespace BankMore.Infra.CrossCutting.Identity.Models;

using System.Collections.Generic;
using System.Security.Claims;
using BankMore.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Http;

public class AspNetUser : IUser
{
    private readonly IHttpContextAccessor _accessor;

    public AspNetUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string Name => _accessor.HttpContext.User.Identity.Name;

    public bool IsAuthenticated()
    {
        return _accessor.HttpContext.User.Identity.IsAuthenticated;
    }

    public IEnumerable<Claim> GetClaimsIdentity()
    {
        return _accessor.HttpContext.User.Claims;
    }

    public string Conta => _accessor?.HttpContext?.User?.Claims?.Where(c => c.Type.Equals("numero_conta")).FirstOrDefault()?.Value;
}
