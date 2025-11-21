namespace BankMore.Domain.Common.Interfaces
{
    public interface IExratoRepository
    {
        Task<IExtratoService> Extrato();
    }
}
