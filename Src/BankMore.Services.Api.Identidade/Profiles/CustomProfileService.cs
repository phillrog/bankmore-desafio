using BankMore.Infra.CrossCutting.Identity.Models;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BankMore.Services.Api.Identidade
{
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Chamado para obter as claims que serão incluídas no token
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subjectId);

            if (user == null) return;

            // 1. Adicionar o "sub" (Subject ID)
            var claims = new List<Claim>
            {
                new Claim("sub", user.Id),
                new Claim("numero_conta", user.Conta)
            };

            // 2. ADICIONAR ROLES
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role)); // Adiciona a claim 'role'
                claims.Add(new Claim("numero_conta", user.Conta));
            }

            // Adiciona as claims ao contexto para inclusão no token
            context.IssuedClaims.AddRange(claims);

            // Se estiver pedindo para incluir no Access Token
            if (context.RequestedResources.Resources.ApiResources.Any() ||
                context.RequestedResources.Resources.ApiScopes.Any())
            {
               
            }
        }

        // Indica se o usuário está ativo
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());
            context.IsActive = user != null;
        }
    }
}