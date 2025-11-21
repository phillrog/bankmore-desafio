using BankMore.Domain.Common.Dtos;


namespace BankMore.Domain.Common.Interfaces
{
    public interface IExtratoService
    {
        Task<IEnumerable<ExtratoDto>> Gerar(int numero);
    }
}
