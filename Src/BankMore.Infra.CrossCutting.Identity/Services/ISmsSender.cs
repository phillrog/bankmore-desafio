using System.Threading.Tasks;

namespace BankMore.Infra.CrossCutting.Identity.Services;

public interface ISmsSender
{
    Task SendSmsAsync(string number, string message);
}
