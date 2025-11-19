namespace BankMore.Domain.Common.Interfaces
{
    public interface IInformacoesContaRespository
    {
        InformacoesContaCorrenteDto GetByCpf(string cpf);
        InformacoesContaCorrenteDto GetByNumero(int numero);
    }
}
