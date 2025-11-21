using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BankMore.Application.ContasCorrentes.ViewModels;

/// <summary>
/// Modelo de dados para cadastro e alteração de conta corrente.
/// </summary>
public class NovaCorrenteViewModel
{

    /// <summary>
    /// Nome completo do titular da conta.
    /// </summary>
    [Required(ErrorMessage = "Obrigatório")]
    [MinLength(2)]
    [MaxLength(100)]
    [DisplayName("Nome")]
    public string Nome { get; set; }

    /// <summary>
    /// CPF do titular.
    /// </summary>
    [Required(ErrorMessage = "Obrigatório")]
    [DisplayName("Cpf")]
    public string Cpf { get; set; }

    /// <summary>
    /// Senha de acesso.
    /// </summary>
    [Required(ErrorMessage = "Obrigatório")]
    [DisplayName("Senha")]
    public string Senha { get; set; }
}