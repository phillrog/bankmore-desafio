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
        private readonly RoleManager<IdentityRole> _roleManager; // 🎯 ADIÇÃO 1: Injeção do RoleManager

        public CustomProfileService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager) // 🎯 ADIÇÃO 2: Injeção no construtor
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Chamado para obter as claims que serão incluídas no token
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subjectId);

            if (user == null) return;

            // --- SEU CÓDIGO INICIAL (MANTIDO) ---

            // 1. Adicionar o "sub" (Subject ID)
            var claims = new List<Claim>
            {
                new Claim("sub", user.Id),
                new Claim("numero_conta", user.Conta)
            };

            // Adicionar Claims ligadas DIRETAMENTE ao usuário
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // 2. ADICIONAR ROLES
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim("role", role)); // Adiciona a claim 'role'               
            }

            // --- FIM DO SEU CÓDIGO INICIAL ---

            // 🎯 ADIÇÃO 3: LÓGICA PARA INCLUIR CLAIMS DE PERMISSÃO DO ROLE
            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role is not null)
                {
                    // Obtém as claims associadas ao Role (Admin_Write, Admin_Read, etc.)
                    var roleClaims = await _roleManager.GetClaimsAsync(role);

                    // Adiciona essas claims de permissão ao token
                    claims.AddRange(roleClaims);
                }
            }
            // 🎯 FIM DA ADIÇÃO 3

            // Adiciona as claims ao contexto para inclusão no token
            // Usamos Distinct para evitar claims duplicadas (ex: numero_conta)
            context.IssuedClaims.AddRange(claims.Distinct(new ClaimComparer()));

            // Se estiver pedindo para incluir no Access Token (MANTIDO)
            if (context.RequestedResources.Resources.ApiResources.Any() ||
                context.RequestedResources.Resources.ApiScopes.Any())
            {

            }
        }

        // Indica se o usuário está ativo (MANTIDO)
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());
            context.IsActive = user != null;
        }

        // 🎯 ADIÇÃO 4: Helper para evitar duplicatas (essencial com o AddRange e Claims)
        public class ClaimComparer : IEqualityComparer<Claim>
        {
            public bool Equals(Claim x, Claim y) => x.Type == y.Type && x.Value == y.Value;
            public int GetHashCode(Claim obj) => HashCode.Combine(obj.Type, obj.Value);
        }
    }
}