using BankMore.Application.Common.Commands;

namespace BankMore.Application.Transferencias.Interfaces
{
    public interface IEstornoDebitoCommandProducer
    {
        Task ProduceEstornoAsync(EstornarDebitoContaCommand command, string key);
    }
}
