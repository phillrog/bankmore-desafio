using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankMore.Infra.CrossCutting.Identity.Models.AccountViewModels;

public class LoginViewModel
{
    [Required]
    public string Cpf { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}
