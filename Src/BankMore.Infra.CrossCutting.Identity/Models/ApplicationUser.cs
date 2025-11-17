using Microsoft.AspNetCore.Identity;

namespace BankMore.Infra.CrossCutting.Identity.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string Cpf { get; set; }
}
