using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Transferencias.Models;


namespace BankMore.Domain.Transferencias.Interfaces;

public interface ITransferenciaRepository : IRepository<Transferencia>
{
    Task AtualizarStatusAsync(Transferencia transferencia);
    bool Exist(Guid id);
}