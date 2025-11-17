namespace BankMore.Domain.ContasCorrentes.Dtos
{
    public class NumeroContaCorrenteDto
    {
        public int NumeroConta { get;  }
        public NumeroContaCorrenteDto(int numero)
        {
            NumeroConta = numero;
        }
    }
}
