using Microsoft.AspNetCore.Mvc;

namespace BankMore.BFF.Web.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Retorna o valor da claim 'numero_conta' do usuário autenticado.
        /// Retorna null se a claim não for encontrada.
        /// </summary>
        public static string? GetNumeroConta(this ControllerBase controller)
        {
            // O objeto User é um ClaimsPrincipal
            return controller.User?.FindFirst("numero_conta")?.Value;
        }
    }
}
