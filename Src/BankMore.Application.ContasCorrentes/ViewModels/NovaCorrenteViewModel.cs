using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankMore.Application.ContasCorrentes.ViewModels;

/// <summary>
/// Modelo de dados para cadastro e alteração de conta corrente.
/// </summary>
public class NovaCorrenteViewModel
{
    
    /// <summary>
    /// Nome completo do titular da conta.
    /// </summary>
    [Required(ErrorMessage = "ObrigatÃ³rio")]
    [MinLength(2)]
    [MaxLength(100)]
    [DisplayName("Nome")]
    public string Nome { get; set; }

    /// <summary>
    /// CPF do titular.
    /// </summary>
    [Required(ErrorMessage = "ObrigatÃ³rio")]
    [DisplayName("Cpf")]
    public string Cpf { get; set; }

    /// <summary>
    /// Senha de acesso.
    /// </summary>
    [Required(ErrorMessage = "ObrigatÃ³rio")]
    [DisplayName("Senha")]
    public string Senha { get; set; }    
}