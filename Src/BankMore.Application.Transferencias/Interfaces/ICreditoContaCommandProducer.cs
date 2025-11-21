using BankMore.Application.Transferencias.Commands;

namespace BankMore.Application.Transferencias.Interfaces
{
    public interface ICreditoContaCommandProducer
    {
        Task ProduceCreditoAsync(CreditarContaCommand command, string key);
    }
}
