using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankMore.Application.ContasCorrentes.ViewModels;

/// <summary>
/// Modelo de dados para cadastro e alteração de conta corrente.
/// </summary>
public class ContaCorrenteViewModel
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
    /// Senha de acesso.
    /// </summary>
    [Required(ErrorMessage = "Obrigatório")]
    [DisplayName("Senha")]
    public string Senha { get; set; }

    /// <summary>
    /// Senha anterior (necessária apenas para troca de senha ou PUT em alguns casos).
    /// </summary>
    public string SenhaAnterior { get; set; }


    /// <summary>
    /// Indica se a conta está ativa.
    /// </summary>
    public bool? Ativo { get; set; }
}