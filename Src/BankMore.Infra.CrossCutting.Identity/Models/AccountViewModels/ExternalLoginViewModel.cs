using System.ComponentModel.DataAnnotations;

namespace BankMore.Infra.CrossCutting.Identity.Models.AccountViewModels;

public class ExternalLoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
