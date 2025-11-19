namespace BankMore.Domain.Common.Interfaces
{
    public interface IInformacoesContaRespository
    {
        Task<InformacoesContaCorrenteDto> GetByCpf(string cpf);
        Task<InformacoesContaCorrenteDto> GetByNumero(int numero);
    }
}
