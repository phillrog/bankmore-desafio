using BankMore.Domain.Common.Providers.Hash;
using BankMore.Infra.CrossCutting.Identity.Models;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace BankMore.Services.Api.Identidade.Validators
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher _passwordHasher;

        // Injetamos o UserManager e o seu IPasswordHasher customizado
        public CustomResourceOwnerPasswordValidator(
            UserManager<ApplicationUser> userManager,
            IPasswordHasher passwordHasher)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        // Este é o método que o Duende IdentityServer chamará
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            // 1. Encontra o usuário pelo nome de usuário (que você usa como CPF)
            var appUser = await _userManager.FindByNameAsync(context.UserName);

            if (appUser is null)
            {
                // Usuário não encontrado
                context.Result = new GrantValidationResult(
                    TokenRequestErrors.InvalidGrant,
                    "Usuário ou senha inválidos.");
                return;
            }

            // 2. RECUPERAÇÃO DA LÓGICA DE HASH CUSTOMIZADA

            // A lógica do seu AccountController: senha + ID do usuário como salt
            var senhaComSalt = context.Password + appUser.Id.ToLower();

            // Usa o seu IPasswordHasher customizado para verificar o hash
            var (isPasswordValid, needsUpgrade) = _passwordHasher.Check(appUser.PasswordHash, senhaComSalt);

            // 3. Valida a senha
            if (isPasswordValid)
            {
                // Sucesso!
                // O SubjectId (sub) será o ID do usuário
                // O AuthenticationMethod é a forma como ele se autenticou
                context.Result = new GrantValidationResult(
                    subject: appUser.Id,
                    authenticationMethod: OidcConstants.AuthenticationMethods.Password
                );
            }
            else
            {
                // Falha na senha
                context.Result = new GrantValidationResult(
                    TokenRequestErrors.InvalidGrant,
                    "Usuário ou senha inválidos.");
            }
        }
    }
}
